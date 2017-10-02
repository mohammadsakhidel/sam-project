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
        Queue<Slide> _initialQueue;
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
                    #region LOOOOOOOOOOOOOOOOOOOOOOOOOOOP:
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
                            }
                            #endregion

                            CreateQueue();
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
                #region get prerequisits:
                var setting = srepo.Get();
                if (setting == null)
                    throw new Exception("Client Settings Not Found!");

                var allConsolations = crepo.GetConsolationsToDisplay();
                var allBanners = brepo.GetBannersToDisplay();
                var slideList = new List<Slide>();
                #endregion

                #region add start banners:
                foreach (var banner in allBanners.Where(b => b.Item1.ShowOnStart).OrderByDescending(b => b.Item1.Priority))
                {
                    var slide = Dispatcher.Invoke(() => { return CreateSlide(banner.Item1, banner.Item2); });
                    slideList.Add(slide);
                }
                #endregion

                #region add consolations:
                if (allConsolations.Any())
                {
                    var maxIntervals = allBanners.Max(b => b.Item1.Interval);
                    foreach (var c in allConsolations)
                    {
                        var slide = Dispatcher.Invoke(() => { return CreateSlide(c.Item1, c.Item2, setting.DefaultSlideDurationMilliSeconds / 1000); });
                        slideList.Add(slide);
                    }
                    while (slideList.Where(s => s.Type == SamUxLib.Code.Enums.SlideType.consolation).Count() < maxIntervals)
                    {
                        foreach (var c in allConsolations)
                        {
                            if (slideList.Where(s => s.Type == SamUxLib.Code.Enums.SlideType.consolation).Count() < maxIntervals)
                            {
                                var slide = Dispatcher.Invoke(() => { return CreateSlide(c.Item1, c.Item2, setting.DefaultSlideDurationMilliSeconds / 1000); });
                                slideList.Add(slide);
                            }
                        }
                    }
                }
                #endregion

                #region add interval banners:
                foreach (var b in allBanners)
                {
                    var interval = b.Item1.Interval;
                    var index = 0;
                    var counter = 0;
                    while (index < slideList.Count)
                    {
                        if (slideList[index].Type == SamUxLib.Code.Enums.SlideType.consolation)
                        {
                            counter++;

                            if (counter % interval == 0)
                            {
                                var slide = Dispatcher.Invoke(() => { return CreateSlide(b.Item1, b.Item2); });
                                if (index < slideList.Count - 1)
                                    slideList.Insert(index + 1, slide);
                                else
                                    slideList.Add(slide);
                            }
                        }

                        index++;
                    }
                }
                #endregion

                _queue = new Queue<Slide>(slideList);
                _initialQueue = new Queue<Slide>(slideList);
            }
        }
        private void UpdateQueue()
        {
            using (var crepo = new ConsolationRepo())
            using (var srepo = new ClientSettingRepo(crepo.Context))
            {
                var setting = srepo.Get();
                var newConsolations = crepo.GetConsolationsToDisplay()
                                      .Where(c => !_initialQueue.Where(s => s.Type == SamUxLib.Code.Enums.SlideType.consolation)
                                                                .Select(s => ((Consolation)s.DataObject).ID).Contains(c.Item1.ID))
                                      .ToList();
                foreach (var newItem in newConsolations)
                {
                    var slide = Dispatcher.Invoke(() => { return CreateSlide(newItem.Item1, newItem.Item2, setting.DefaultSlideDurationMilliSeconds / 1000); });
                    _queue.Enqueue(slide);
                    _initialQueue.Enqueue(slide);
                }
            }
        }
        private void DisplaySlide(Slide slide)
        {
            #region show:
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

            #region delay:
            Thread.Sleep(slide.DurationSeconds * 1000);
            #endregion
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
        private Slide CreateSlide(Banner banner, Blob blob)
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
        private Slide CreateSlide(Consolation consolation, ConsolationImage image, int durationSecs)
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