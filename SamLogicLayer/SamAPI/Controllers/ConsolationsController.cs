using AutoMapper;
using Newtonsoft.Json;
using RamancoLibrary.Utilities;
using SamAPI.Code.Utils;
using SamAPI.Resources;
using SamDataAccess.Repos.Interfaces;
using SamModels.DTOs;
using SamModels.Entities.Blobs;
using SamModels.Entities.Core;
using SamUtils.Enums;
using SamUtils.Utils;
using SmsLib.Objects;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Web.Hosting;
using System.Web.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SamAPI.Controllers
{
    public class ConsolationsController : ApiController
    {
        #region Fields:
        IConsolationRepo _consolationRepo;
        IBlobRepo _blobRepo;
        ISmsManager _smsManager;
        #endregion

        #region Ctors:
        public ConsolationsController(IConsolationRepo consolationRepo, IBlobRepo blobRepo, ISmsManager smsManager)
        {
            _consolationRepo = consolationRepo;
            _blobRepo = blobRepo;
            _smsManager = smsManager;
        }
        #endregion

        #region POST Actions:
        [HttpPost]
        public IHttpActionResult Create(ConsolationDto model)
        {
            try
            {
                #region Create Consolation:
                var consolation = Mapper.Map<ConsolationDto, Consolation>(model);
                consolation.TrackingNumber = IDGenerator.GenerateTrackingNumber();
                consolation.CreationTime = DateTimeUtils.Now;
                consolation.LastUpdateTime = consolation.CreationTime;
                consolation.Customer.IsMember = false;
                _consolationRepo.Add(consolation);
                _consolationRepo.Save();
                #endregion

                #region Send SMS To Customer:
                try
                {
                    string messageText = string.Format(SmsMessages.ConsolationCreationSms, consolation.TrackingNumber);
                    _smsManager.SendAsync(messageText, new string[] { model.Customer.CellPhoneNumber });
                }
                catch { }
                #endregion

                return Ok(new { ID = consolation.ID, TrackingNumber = consolation.TrackingNumber });
            }
            catch (Exception ex)
            {
                return ResponseMessage(ExceptionManager.GetExceptionResponse(this, ex));
            }
        }

        [HttpPost]
        public IHttpActionResult Find([FromBody] string[] trackingNumbers)
        {
            try
            {
                var consolations = _consolationRepo.Find(trackingNumbers);
                var dtos = consolations.Select(c => Mapper.Map<Consolation, ConsolationDto>(c)).ToList();
                return Ok(dtos);
            }
            catch (Exception ex)
            {
                return ResponseMessage(ExceptionManager.GetExceptionResponse(this, ex));
            }
        }
        #endregion

        #region GET Actions:
        [HttpGet]
        public IHttpActionResult Filter(int cityId = 0, string status = "", int count = 30)
        {
            try
            {
                var consolations = _consolationRepo.Filter(cityId, status, count);
                var dtos = consolations.Select(c => Mapper.Map<Consolation, ConsolationDto>(c)).ToList();
                return Ok(dtos);
            }
            catch (Exception ex)
            {
                return ResponseMessage(ExceptionManager.GetExceptionResponse(this, ex));
            }
        }

        [HttpGet]
        public IHttpActionResult GetPreview(int id, bool? thumb = false)
        {
            try
            {
                var consolation = _consolationRepo.Get(id);
                if (consolation == null)
                    return NotFound();

                var blob = _blobRepo.Get(consolation.Template.BackgroundImageID);
                if (blob == null || !(blob is ImageBlob))
                    return NotFound();

                #region generate preview image:
                var imgBlob = (ImageBlob)blob;
                var imgBytes = thumb.HasValue && thumb.Value ? imgBlob.ThumbImageBytes : imgBlob.Bytes;
                var imgBitmap = IOUtils.ByteArrayToBitmap(imgBytes);
                var tempBitmap = GeneratePreview(consolation, imgBitmap);
                var previewBitmap = new Bitmap(tempBitmap);
                tempBitmap.Dispose();
                tempBitmap = null;
                var previewBytes = IOUtils.BitmapToByteArray(previewBitmap, System.Drawing.Imaging.ImageFormat.Jpeg);
                #endregion

                #region Return As Image:
                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                result.Content = new ByteArrayContent(previewBytes);
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpg");
                return ResponseMessage(result);
                #endregion
            }
            catch (Exception ex)
            {
                return ResponseMessage(ExceptionManager.GetExceptionResponse(this, ex));
            }
        }
        #endregion

        #region PUT ACTIONS:
        [HttpPut]
        public IHttpActionResult Update(int id, string newStatus, Dictionary<string, string> fields,
            int obitId = -1, int templateId = -1)
        {
            try
            {
                var consolationToEdit = _consolationRepo.Get(id);
                if (consolationToEdit == null)
                    return NotFound();

                #region update fields:
                if (obitId > 0)
                    consolationToEdit.ObitID = obitId;

                if (templateId > 0)
                    consolationToEdit.TemplateID = templateId;

                if (fields.ContainsKey("Audience"))
                    consolationToEdit.Audience = fields["Audience"];
                if (fields.ContainsKey("From"))
                    consolationToEdit.From = fields["From"];

                var json = JsonConvert.SerializeObject(fields);
                consolationToEdit.TemplateInfo = json;
                #endregion

                #region update status:
                if (!string.IsNullOrEmpty(newStatus))
                {
                    var newstatus = (ConsolationStatus)Enum.Parse(typeof(ConsolationStatus), newStatus);
                    if (newstatus == ConsolationStatus.confirmed)
                    {
                        // TODO: verify payment if no problem then change the status to cofirmed.
                        if (consolationToEdit.Status == ConsolationStatus.canceled.ToString())
                        {
                            var isDisplayed = _consolationRepo.IsDisplayed(consolationToEdit.ID);
                            consolationToEdit.Status = (!isDisplayed ? ConsolationStatus.confirmed.ToString() : ConsolationStatus.displayed.ToString());
                        }
                        else
                        {
                            consolationToEdit.Status = ConsolationStatus.confirmed.ToString();
                        }
                    }
                    else if (newstatus == ConsolationStatus.canceled)
                    {
                        consolationToEdit.Status = newstatus.ToString();
                    }
                }
                #endregion

                consolationToEdit.LastUpdateTime = DateTimeUtils.Now;
                _consolationRepo.Save();

                return Ok();
            }
            catch (Exception ex)
            {
                return ResponseMessage(ExceptionManager.GetExceptionResponse(this, ex));
            }
        }
        #endregion

        #region Methods:
        private Bitmap GeneratePreview(Consolation consolation, Bitmap backgroundImage, int targetScreenWidth = 0, int targetScreenHeight = 0)
        {
            Bitmap bitmap = null;
            Thread t = new Thread(() =>
            {
                #region Set Container Size:
                var container = new Canvas();
                var screenWidth = targetScreenWidth > 0 ? targetScreenWidth : backgroundImage.Width;
                var screenHeight = targetScreenHeight > 0 ? targetScreenHeight : backgroundImage.Height;
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

                container.Width = containerWidth;
                container.Height = containerHeight;
                #endregion
                #region Set Background and Fields:
                container.Background = new ImageBrush(ImageUtils.ToBitmapSource(backgroundImage));
                //fiedls:
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

                    // font family:
                    var fontsFolder = new Uri(HostingEnvironment.MapPath("~/Content/Fonts/#"));
                    var fontFamilies = Fonts.GetFontFamilies(fontsFolder).ToList();
                    var fontFamily = fontFamilies.Where(ff => ff.FamilyNames.Values.Contains(field.FontFamily)).FirstOrDefault();
                    if (fontFamily != null)
                        textBlock.FontFamily = fontFamily;

                    textBlock.FontSize = StringToFontSize(field.FontSize);

                    var box = new Border();
                    box.Width = field.BoxWidth * container.Width / 100;
                    box.Height = field.BoxHeight * container.Height / 100;
                    Canvas.SetLeft(box, container.Width * field.X / 100);
                    Canvas.SetTop(box, container.Height * field.Y / 100);

                    box.Child = textBlock;
                    container.Children.Add(box);
                }
                #endregion

                Viewbox viewbox = new Viewbox();
                viewbox.Child = container;
                viewbox.Measure(new System.Windows.Size(container.Width, container.Height));
                viewbox.Arrange(new Rect(0, 0, container.Width, container.Height));
                viewbox.UpdateLayout();

                bitmap = ImageUtils.DrawWpfControl(viewbox, (int)container.Width, (int)container.Height);
            });

            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();

            return bitmap;
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
            var dic = new Dictionary<string, double>();
            dic.Add("tiny", 20);
            dic.Add("small", 40);
            dic.Add("normal", 60);
            dic.Add("large", 80);
            dic.Add("huge", 100);
            return dic.ContainsKey(text) ? dic[text] : dic["normal"];
        }
        #endregion
    }
}
