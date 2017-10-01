using Newtonsoft.Json;
using RamancoLibrary.Utilities;
using SamClientDataAccess.ClientModels;
using SamClientDataAccess.Repos;
using SamClientDataAccess.Repos;
using SamModels.Entities;
using SamUtils.Enums;
using SamUxLib.Code.Objects;
using SamUxLib.Code.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Transitionals;
using Transitionals.Transitions;

namespace SamClient.Views.Windows
{
    public partial class PlayerWindow : Window
    {
        #region Fields:
        Queue<Slide> _queue;
        Queue<Slide> _temp;
        CancellationTokenSource _cancellationTokenSource;
        #endregion

        #region Ctors:
        public PlayerWindow()
        {
            InitializeComponent();
        }
        #endregion

        #region Event Handlers:
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                PlaySlides();
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_cancellationTokenSource != null)
                {
                    _cancellationTokenSource.Cancel();
                }

                Close();
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        #endregion

        #region Methods:
        private void PlaySlides()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            var token = _cancellationTokenSource.Token;
            Task.Run(() =>
            {
                try
                {
                    CreateQueue();
                    #region LOOOOOOP:
                    while (!token.IsCancellationRequested)
                    {
                        try
                        {
                            #region display all slides:
                            while (_queue.Any())
                            {
                                var slideToDisplay = _queue.Dequeue();
                                DisplaySlide(slideToDisplay);
                                UpdateQueue();
                                Thread.Sleep(slideToDisplay.DurationSeconds * 1000);
                            }
                            #endregion
                            #region create queue for next cycle:
                            CreateQueue();
                            #endregion
                        }
                        catch (Exception ex)
                        {
                            ExceptionManager.Handle(ex);
                        }
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    ExceptionManager.Handle(ex);
                }
            }, token);
        }
        private void CreateQueue()
        {
            using (var crepo = new ConsolationRepo())
            using (var brepo = new BannerRepo(crepo.Context))
            using (var srepo = new ClientSettingRepo(crepo.Context))
            {
                #region get prereqs data:
                var setting = srepo.Get();
                if (setting == null)
                    throw new Exception("Client Settings Not Found!");

                var consolations = crepo.GetConsolationsToDisplay();
                var banners = brepo.GetBannersToDisplay();
                var slideList = new List<Slide>();
                #endregion
                #region add starting banners to queue:
                var startBanners = banners.Where(b => b.Item1.ShowOnStart).OrderByDescending(b => b.Item1.Priority);
                if (startBanners.Any())
                {
                    foreach (var sb in startBanners)
                    {
                        var slide = Dispatcher.Invoke<Slide>(() =>
                        {
                            return CreateBannerSlide(sb.Item1, sb.Item2);
                        });
                        slideList.Add(slide);
                    }
                }
                #endregion
                #region add consolations:
                foreach (var c in consolations)
                {
                    var slide = Dispatcher.Invoke<Slide>(() =>
                    {
                        return CreateConsolationSlide(c.Item1, c.Item2, setting.DefaultSlideDurationMilliSeconds / 1000);
                    });
                    slideList.Add(slide);
                }
                #endregion
                #region add interval banners:
                foreach (var b in banners)
                {
                    var interval = b.Item1.Interval;
                    var index = 0;
                    var counter = 0;
                    while (index < slideList.Count)
                    {
                        if (slideList[index].Type == SamUxLib.Code.Enums.SlideType.consolation)
                        {
                            counter++;

                            if (counter > 0 && counter % interval == 0)
                            {
                                var slide = Dispatcher.Invoke<Slide>(() =>
                                {
                                    return CreateBannerSlide(b.Item1, b.Item2);
                                });
                                if (index + 1 < slideList.Count)
                                    slideList.Insert(index + 1, slide);
                                else
                                    slideList.Add(slide);
                            }
                        }

                        index++;
                    }
                }
                #endregion
                #region create queue:
                _queue = new Queue<Slide>();
                foreach (var slide in slideList)
                {
                    _queue.Enqueue(slide);
                }
                #endregion

                _temp = new Queue<Slide>(_queue);
            }
        }
        private void UpdateQueue()
        {
            using (var crepo = new ConsolationRepo())
            using (var srepo = new ClientSettingRepo(crepo.Context))
            {
                var setting = srepo.Get();
                var newConsolations = crepo.GetConsolationsToDisplay()
                                      .Where(c => !_queue.Where(s => s.Type == SamUxLib.Code.Enums.SlideType.consolation)
                                                         .Select(s => ((Consolation)s.DataObject).ID).Contains(c.Item1.ID))
                                      .ToList();
                foreach (var newItem in newConsolations)
                {
                    var slide = Dispatcher.Invoke(() =>
                    {
                        return CreateConsolationSlide(newItem.Item1, newItem.Item2, setting.DefaultSlideDurationMilliSeconds / 1000);
                    });
                    _queue.Enqueue(slide);
                }
            }
        }
        private void DisplaySlide(Slide slide)
        {
            #region calculate image size:
            double calculatedWidth = 0.0, calculatedHeight = 0.0;
            var screenWidth = ActualWidth;
            var screenHeight = ActualHeight;
            double screenRatio = screenWidth / screenHeight;
            double slideWidth = slide.Image.Width;
            double slideHeight = slide.Image.Height;
            double slideRatio = slideWidth / slideHeight;
            if (screenRatio > slideRatio)
            {
                calculatedHeight = screenHeight;
                calculatedWidth = calculatedHeight * slideWidth / slideHeight;
            }
            else
            {
                calculatedWidth = screenWidth;
                calculatedHeight = calculatedWidth * slideHeight / slideWidth;
            }

            Dispatcher.Invoke(() =>
            {
                var slideImage = new System.Windows.Controls.Image();
                slideImage.Width = calculatedWidth;
                slideImage.Height = calculatedHeight;
                slideImage.Source = ImageUtils.ToBitmapSource(slide.Image);
                slideImage.VerticalAlignment = VerticalAlignment.Center;
                slideImage.HorizontalAlignment = HorizontalAlignment.Center;
                transitionBox.Transition = slide.InTransition;
                transitionBox.Content = slideImage;
            });
            #endregion

            #region save display:
            if (slide.Type == SamUxLib.Code.Enums.SlideType.consolation)
            {
                using (var drep = new DisplayRepo())
                {
                    var display = new Display
                    {
                        ConsolationID = (slide.DataObject as Consolation).ID,
                        DurationMilliSeconds = slide.DurationSeconds * 1000,
                        SyncStatus = DisplaySyncStatus.pending.ToString(),
                        TimeOfDisplay = DateTimeUtils.Now,
                        CreationTime = DateTimeUtils.Now
                    };
                    drep.AddWithSave(display);
                }
            }
            #endregion
        }
        private void ShowConsolation(Consolation consolation, Bitmap backgroundImage)
        {
            //#region Set Container Size:
            //_container = new Canvas();
            //var screenWidth = ActualWidth;
            //var screenHeight = ActualHeight;
            //double screenRatio = screenWidth / screenHeight;
            //double cWidth = consolation.Template.WidthRatio;
            //double cHeight = consolation.Template.HeightRatio;
            //double cRatio = cWidth / cHeight;
            //double containerWidth = 0.0;
            //double containerHeight = 0.0;
            //if (screenRatio > cRatio)
            //{
            //    containerHeight = screenHeight;
            //    containerWidth = containerHeight * cWidth / cHeight;
            //}
            //else
            //{
            //    containerWidth = screenWidth;
            //    containerHeight = containerWidth * cHeight / cWidth;
            //}

            //Dispatcher.Invoke(() =>
            //{
            //    _container.Width = containerWidth;
            //    _container.Height = containerHeight;
            //});
            //#endregion
            //#region Set Background and Fields:
            //_container.Background = new ImageBrush(ImageUtils.ToBitmapSource(backgroundImage));
            //// fiedls:
            //var info = JsonConvert.DeserializeObject<Dictionary<string, string>>(consolation.TemplateInfo);
            //foreach (var field in consolation.Template.TemplateFields)
            //{
            //    var textBlock = new TextBlock();
            //    textBlock.Text = info.ContainsKey(field.Name) ? info[field.Name] : "";
            //    textBlock.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(field.TextColor));
            //    textBlock.FontWeight = field.Bold.HasValue && field.Bold.Value ? FontWeights.Bold : FontWeights.Normal;
            //    textBlock.TextWrapping = field.WrapContent.HasValue && field.WrapContent.Value ? TextWrapping.Wrap : TextWrapping.NoWrap;
            //    textBlock.HorizontalAlignment = StringToHorizontalAlignment(field.HorizontalContentAlignment);
            //    textBlock.VerticalAlignment = StringToVerticalAlignment(field.VerticalContentAlignment);
            //    textBlock.FontFamily = new System.Windows.Media.FontFamily(field.FontFamily);
            //    textBlock.FontSize = StringToFontSize(field.FontSize);

            //    var box = new Border();
            //    box.Width = field.BoxWidth * _container.Width / 100;
            //    box.Height = field.BoxHeight * _container.Height / 100;
            //    Canvas.SetLeft(box, _container.Width * field.X / 100);
            //    Canvas.SetTop(box, _container.Height * field.Y / 100);

            //    box.Child = textBlock;
            //    _container.Children.Add(box);
            //}
            //#endregion

            //_current = consolation;

            //transitionBox.Transition = GetRandomTransition();
            //transitionBox.Content = _container;
        }
        private Transition GetRandomTransition()
        {
            var all = new List<Transition> {
                //new DoorTransition(),
                //new ExplosionTransition(),
                //new FadeAndBlurTransition(),
                //new FadeAndGrowTransition(),
                //new FlipTransition(),
                //new PageTransition(),
                //new RollTransition(),
                //new TranslateTransition()
                new RotateTransition() { Direction = RotateDirection.Left },
                new RotateTransition() { Direction = RotateDirection.Up },
                new RotateTransition() { Direction = RotateDirection.Right },
                new RotateTransition() { Direction = RotateDirection.Down }
            };
            var count = all.Count();
            var randomIndex = TextUtils.GetRandomNumbers(0, count, 1)[0];
            return all[randomIndex];
        }
        private Slide CreateBannerSlide(Banner banner, Blob blob)
        {
            var image = IOUtils.ByteArrayToBitmap(blob.Bytes);
            var transition = GetRandomTransition();
            return new Slide()
            {
                Type = SamUxLib.Code.Enums.SlideType.banner,
                Image = image,
                DurationSeconds = banner.DurationSeconds,
                DataObject = banner,
                InTransition = transition
            };
        }
        private Slide CreateConsolationSlide(Consolation consolation, ConsolationImage image, int durationSecs)
        {
            return new Slide()
            {
                Type = SamUxLib.Code.Enums.SlideType.consolation,
                Image = IOUtils.ByteArrayToBitmap(image.Bytes),
                DurationSeconds = durationSecs,
                DataObject = consolation,
                InTransition = GetRandomTransition()
            };
        }
        #endregion
    }
}