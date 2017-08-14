using Newtonsoft.Json;
using RamancoLibrary.Utilities;
using SamClientDataAccess.Repos;
using SamClientDataAccess.Repos;
using SamModels.Entities.Core;
using SamUxLib.Code.Utils;
using System;
using System.Collections.Generic;
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
        Canvas _container;
        Consolation _current;
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
                Play();
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        #endregion

        #region Methods:
        private void Play()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        using (var crep = new ConsolationRepo())
                        using (var brep = new BlobRepo())
                        {
                            #region find next consolation:
                            var nextItem = crep.GetNext(_current?.ID);
                            var nextConsolation = nextItem.Item1; ;
                            var nextDuration = nextItem.Item2;
                            #endregion

                            #region show:
                            if (nextConsolation != null)
                            {
                                var blob = brep.Get(nextConsolation.Template.BackgroundImageID);
                                if (blob != null)
                                {
                                    var bg = IOUtils.ByteArrayToBitmap(blob.Bytes);
                                    Dispatcher.Invoke(() =>
                                    {
                                        try
                                        {
                                            ShowConsolation(nextConsolation, bg);
                                        }
                                        catch (Exception ex)
                                        {
                                            ExceptionManager.Log(ex);
                                        }
                                    });
                                }
                            }
                            #endregion

                            #region Delay:
                            Thread.Sleep(nextDuration);
                            #endregion
                        }
                    }
                    catch (Exception ex)
                    {
                        ExceptionManager.Log(ex);
                    }
                }
            });
        }
        private void ShowConsolation(Consolation consolation, Bitmap backgroundImage)
        {
            #region Set Container Size:
            _container = new Canvas();
            var screenWidth = ActualWidth;
            var screenHeight = ActualHeight;
            double screenRatio = screenWidth / screenHeight;
            double cWidth = consolation.Template.WidthRatio;
            double cHeight = consolation.Template.HeightRatio;
            double cRatio = cWidth / cHeight;
            double containerWidth = 0.0;
            double containerHeight = 0.0;
            if (screenRatio > cRatio)
            {
                containerHeight = screenHeight;
                containerWidth = containerHeight * cWidth / cHeight;
            }
            else
            {
                containerWidth = screenWidth;
                containerHeight = containerWidth * cHeight / cWidth;
            }

            Dispatcher.Invoke(() =>
            {
                _container.Width = containerWidth;
                _container.Height = containerHeight;
            });
            #endregion
            #region Set Background and Fields:
            _container.Background = new ImageBrush(ImageUtils.ToBitmapSource(backgroundImage));
            // fiedls:
            var info = JsonConvert.DeserializeObject<Dictionary<string, string>>(consolation.TemplateInfo);
            foreach (var field in consolation.Template.TemplateFields)
            {
                var textBlock = new TextBlock();
                textBlock.Text = info.ContainsKey(field.Name) ? info[field.Name] : "";
                textBlock.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(field.TextColor));
                textBlock.FontWeight = field.Bold.HasValue && field.Bold.Value ? FontWeights.Bold : FontWeights.Normal;
                textBlock.TextWrapping = field.WrapContent.HasValue && field.WrapContent.Value ? TextWrapping.Wrap : TextWrapping.NoWrap;
                textBlock.HorizontalAlignment = StringToHorizontalAlignment(field.HorizontalContentAlignment);
                textBlock.VerticalAlignment = StringToVerticalAlignment(field.VerticalContentAlignment);
                textBlock.FontFamily = new System.Windows.Media.FontFamily(field.FontFamily);
                textBlock.FontSize = StringToFontSize(field.FontSize);

                var box = new Border();
                box.Width = field.BoxWidth * _container.Width / 100;
                box.Height = field.BoxHeight * _container.Height / 100;
                Canvas.SetLeft(box, _container.Width * field.X / 100);
                Canvas.SetTop(box, _container.Height * field.Y / 100);

                box.Child = textBlock;
                _container.Children.Add(box);
            }
            #endregion

            _current = consolation;

            transitionBox.Transition = GetRandomTransition();
            transitionBox.Content = _container;
        }
        private Transition GetRandomTransition()
        {
            //var types = typeof(Transition).Assembly.GetTypes().Where(t => t.Name.EndsWith("Transition")).ToList();
            var all = new List<Transition> {
                new DoorTransition(),
                new ExplosionTransition(),
                new FadeAndBlurTransition(),
                new FadeAndGrowTransition(),
                new FlipTransition(),
                new PageTransition(),
                new RollTransition(),
                new RotateTransition() { Direction = RotateDirection.Left },
                new RotateTransition() { Direction = RotateDirection.Up },
                new TranslateTransition()
            };
            var count = all.Count();
            var randomIndex = TextUtils.GetRandomNumbers(0, count, 1)[0];
            return all[randomIndex];
        }
        private HorizontalAlignment StringToHorizontalAlignment(string text)
        {
            if (text == SamUtils.Enums.TextAlignment.center.ToString())
                return HorizontalAlignment.Center;
            else if (text == SamUtils.Enums.TextAlignment.left.ToString())
                return HorizontalAlignment.Left;
            else if (text == SamUtils.Enums.TextAlignment.right.ToString())
                return HorizontalAlignment.Right;

            return HorizontalAlignment.Center;
        }
        private VerticalAlignment StringToVerticalAlignment(string text)
        {
            if (text == SamUtils.Enums.TextAlignment.center.ToString())
                return VerticalAlignment.Center;
            else if (text == SamUtils.Enums.TextAlignment.top.ToString())
                return VerticalAlignment.Top;
            else if (text == SamUtils.Enums.TextAlignment.bottom.ToString())
                return VerticalAlignment.Bottom;

            return VerticalAlignment.Center;
        }
        private double StringToFontSize(string text)
        {
            var res = FindResource($"font_consolation_{text}") as double?;

            if (!res.HasValue)
                return (double)FindResource("font_consolation_normal");

            return res.Value;
        }
        #endregion
    }
}