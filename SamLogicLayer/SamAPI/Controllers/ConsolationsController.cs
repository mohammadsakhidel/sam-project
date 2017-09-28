using AutoMapper;
using Newtonsoft.Json;
using RamancoLibrary.Utilities;
using SamAPI.Code.Utils;
using SamAPI.Resources;
using SamDataAccess.Repos.Interfaces;
using SamModels.DTOs;
using SamModels.Entities;
using SamUtils.Enums;
using SamUtils.Utils;
using SmsLib.Objects;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Http;

namespace SamAPI.Controllers
{
    public class ConsolationsController : ApiController
    {
        #region Fields:
        IConsolationRepo _consolationRepo;
        IBlobRepo _blobRepo;
        ISmsManager _smsManager;
        ICustomerRepo _customerRepo;
        ITemplateRepo _templateRepo;
        #endregion

        #region Ctors:
        public ConsolationsController(IConsolationRepo consolationRepo, IBlobRepo blobRepo,
            ISmsManager smsManager, ICustomerRepo customerRepo, ITemplateRepo templateRepo)
        {
            _consolationRepo = consolationRepo;
            _blobRepo = blobRepo;
            _smsManager = smsManager;
            _customerRepo = customerRepo;
            _templateRepo = templateRepo;
        }
        #endregion

        #region POST Actions:
        [HttpPost]
        public IHttpActionResult Create(ConsolationDto model)
        {
            try
            {
                #region Get Associated Template:
                var template = _templateRepo.Get(model.TemplateID);
                if (template == null)
                    throw new Exception("Template Not Found!");
                #endregion

                #region Create Consolation:
                var consolation = Mapper.Map<ConsolationDto, Consolation>(model);
                consolation.Status = ConsolationStatus.pending.ToString();
                consolation.PaymentStatus = (template.Price > 0 ? PaymentStatus.pending.ToString() : PaymentStatus.free.ToString());
                consolation.AmountToPay = template.Price;
                consolation.TrackingNumber = IDGenerator.GenerateTrackingNumber();
                consolation.CreationTime = DateTimeUtils.Now;
                consolation.LastUpdateTime = consolation.CreationTime;
                #region Customer Info:
                var customer = _customerRepo.Find(consolation.Customer.CellPhoneNumber);
                if (customer != null)
                {
                    consolation.CustomerID = customer.ID;
                    consolation.Customer = null;
                }
                else
                {
                    consolation.Customer.IsMember = false;
                }
                #endregion
                _consolationRepo.Add(consolation);
                _consolationRepo.Save();
                #endregion

                #region Send SMS To Customer:
                Task.Run(() =>
                {
                    try
                    {
                        string messageText = string.Format(SmsMessages.ConsolationCreationSms, consolation.TrackingNumber);
                        _smsManager.Send(messageText, new string[] { model.Customer.CellPhoneNumber });
                    }
                    catch { }
                });
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

                #region return from temp folder if available:
                #region create folders if not exist:
                if (!Directory.Exists(HostingEnvironment.MapPath("~/Content/Temp/")))
                    Directory.CreateDirectory(HostingEnvironment.MapPath("~/Content/Temp/"));

                if (!Directory.Exists(HostingEnvironment.MapPath("~/Content/Temp/Consolations/")))
                    Directory.CreateDirectory(HostingEnvironment.MapPath("~/Content/Temp/Consolations/"));
                #endregion
                var tempFolderPath = HostingEnvironment.MapPath("~/Content/Temp/Consolations");
                var tempFilePath = $"{tempFolderPath}/{consolation.ID}.jpg";
                FileInfo fileInfo = File.Exists(tempFilePath) ? new FileInfo(tempFilePath) : null;
                if (fileInfo != null && (!consolation.LastUpdateTime.HasValue || consolation.LastUpdateTime.Value < fileInfo.LastWriteTime))
                {
                    return ResponseMessage(JpgResponse(File.ReadAllBytes(tempFilePath)));
                }
                #endregion

                var blob = _blobRepo.Get(consolation.Template.BackgroundImageID);
                if (blob == null || !(blob is ImageBlob))
                    return NotFound();

                #region generate preview image:
                var imgBlob = (ImageBlob)blob;
                var imgBytes = imgBlob.Bytes;
                var imgBitmap = IOUtils.ByteArrayToBitmap(imgBytes);
                var tempBitmap = GeneratePreview(consolation, imgBitmap);
                var previewBitmap = new Bitmap(tempBitmap);
                tempBitmap.Dispose();
                tempBitmap = null;
                if (thumb.HasValue && thumb.Value)
                {
                    var resizer = new ImageResizer(previewBitmap.Width, previewBitmap.Height, ResizeType.ShorterFix, 100);
                    previewBitmap = ImageUtils.GetThumbnailImage(previewBitmap, resizer.NewWidth, resizer.NewHeight);
                }
                var previewBytes = IOUtils.BitmapToByteArray(previewBitmap, System.Drawing.Imaging.ImageFormat.Jpeg);
                #endregion

                File.WriteAllBytes(tempFilePath, previewBytes);
                return ResponseMessage(JpgResponse(previewBytes));
            }
            catch (Exception ex)
            {
                return ResponseMessage(ExceptionManager.GetExceptionResponse(this, ex));
            }
        }

        [HttpGet]
        public IHttpActionResult FindByID(int id)
        {
            try
            {
                var consolation = _consolationRepo.Get(id);
                if (consolation == null)
                    return NotFound();

                var dto = Mapper.Map<Consolation, ConsolationDto>(consolation);
                return Ok(dto);
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
        private Bitmap GeneratePreview(Consolation consolation, Bitmap backgroundImage)
        {
            Bitmap bitmap = null;
            Thread t = new Thread(() =>
            {
                try
                {
                    #region 1 ::: WPF Draw Control:
                    /*
                    #region Set Container Size:
                    var container = new Canvas();
                    var screenWidth = backgroundImage.Width;
                    var screenHeight = backgroundImage.Height;
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
                    container.Background = System.Windows.Media.Brushes.Red; //new ImageBrush(ImageUtils.ToBitmapSource(backgroundImage));

                    #region fields:
                    var info = JsonConvert.DeserializeObject<Dictionary<string, string>>(consolation.TemplateInfo);
                    var fields = consolation.Template.TemplateFields.ToList();

                    foreach (var field in fields)
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
                    #endregion

                    #region view box:
                    Viewbox viewbox = new Viewbox();
                    viewbox.Child = container;
                    viewbox.Measure(new System.Windows.Size(container.Width, container.Height));
                    viewbox.Arrange(new Rect(0, 0, container.Width, container.Height));
                    viewbox.UpdateLayout();
                    #endregion

                    bitmap = ImageUtils.DrawWpfControl(viewbox, (int)container.Width, (int)container.Height);
                    */
                    #endregion
                    #region 2 ::: Graphics Library:
                    var maxWith = 1500;
                    var resizer = new ImageResizer(backgroundImage.Width, backgroundImage.Height, ResizeType.LongerFix, maxWith);
                    var resizedBG = ImageUtils.GetThumbnailImage(backgroundImage, resizer.NewWidth, resizer.NewHeight);
                    #region draw fields:
                    using (var g = Graphics.FromImage(resizedBG))
                    {
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                        var info = JsonConvert.DeserializeObject<Dictionary<string, string>>(consolation.TemplateInfo);
                        var fields = consolation.Template.TemplateFields.ToList();

                        foreach (var field in fields)
                        {
                            var box = new Rectangle();
                            box.Width = (int)(field.BoxWidth * resizedBG.Width / 100);
                            box.Height = (int)(field.BoxHeight * resizedBG.Height / 100);
                            box.X = (int)(resizedBG.Width * field.X / 100);
                            box.Y = (int)(resizedBG.Height * field.Y / 100);

                            //text:
                            var text = info.ContainsKey(field.Name) ? info[field.Name] : "";
                            var textAlignmentFormat = new StringFormat()
                            {
                                Alignment = StringToStringAlignment(field.HorizontalContentAlignment),
                                LineAlignment = StringToStringAlignment(field.VerticalContentAlignment)
                            };
                            var textColor = new SolidBrush(ColorTranslator.FromHtml(field.TextColor));
                            //font:
                            var fontsFolder = HostingEnvironment.MapPath("~/Content/Fonts/");
                            var fontCollection = new PrivateFontCollection();
                            var fontFiles = Directory.GetFiles(fontsFolder);
                            foreach (var fontFile in fontFiles)
                            {
                                fontCollection.AddFontFile(fontFile);
                            }
                            var fontFamily = fontCollection.Families.Where(ff => ff.Name == field.FontFamily).FirstOrDefault();
                            var font = new Font(fontFamily, StringToFontSize(field.FontSize), (field.Bold.HasValue && field.Bold.Value ? FontStyle.Bold : FontStyle.Regular));

                            g.DrawString(text, font, textColor, box, textAlignmentFormat);
                        }
                    }
                    #endregion
                    bitmap = resizedBG;
                    #endregion

                }
                catch { }
            });

            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();

            return bitmap;
        }
        private HttpResponseMessage JpgResponse(byte[] imageBytes)
        {
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new ByteArrayContent(imageBytes);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpg");
            return result;
        }
        private StringAlignment StringToStringAlignment(string text)
        {
            if (text == SamUtils.Enums.TextAlignment.center.ToString())
                return StringAlignment.Center;
            else if (text == SamUtils.Enums.TextAlignment.left.ToString() || text == SamUtils.Enums.TextAlignment.top.ToString())
                return StringAlignment.Near;
            else if (text == SamUtils.Enums.TextAlignment.right.ToString() || text == SamUtils.Enums.TextAlignment.bottom.ToString())
                return StringAlignment.Far;

            return StringAlignment.Center;
        }
        private float StringToFontSize(string text)
        {
            var dic = new Dictionary<string, float>();
            dic.Add("tiny", 25f);
            dic.Add("small", 35f);
            dic.Add("normal", 55f);
            dic.Add("large", 70f);
            dic.Add("huge", 80f);
            return dic.ContainsKey(text) ? dic[text] : dic["normal"];
        }
        #endregion
    }
}
