using AutoMapper;
using Newtonsoft.Json;
using RamancoLibrary.Utilities;
using SamAPI.Code.Payment;
using SamAPI.Code.Utils;
using SamAPI.Resources;
using SamDataAccess.Repos.Interfaces;
using SamModels.DTOs;
using SamModels.Entities;
using SamUtils.Constants;
using SamUtils.Enums;
using SamUtils.Utils;
using SmsLib.Objects;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Hosting;
using System.Web.Http;

namespace SamAPI.Controllers
{
    public class ConsolationsController : ApiController
    {
        #region Fields:
        IConsolationRepo _consolationRepo;
        IObitRepo _obitRepo;
        IBlobRepo _blobRepo;
        ICustomerRepo _customerRepo;
        ITemplateRepo _templateRepo;
        IPaymentRepo _paymentRepo;
        IPaymentService _paymentService;
        IIdentityRepo _identityRepo;
        #endregion

        #region Ctors:
        public ConsolationsController(IConsolationRepo consolationRepo, IBlobRepo blobRepo,
            ICustomerRepo customerRepo, ITemplateRepo templateRepo, IObitRepo obitRepo,
            IPaymentRepo paymentRepo, IPaymentService paymentService, IIdentityRepo identityRepo)
        {
            _consolationRepo = consolationRepo;
            _blobRepo = blobRepo;
            _customerRepo = customerRepo;
            _templateRepo = templateRepo;
            _obitRepo = obitRepo;
            _paymentRepo = paymentRepo;
            _paymentService = paymentService;
            _identityRepo = identityRepo;
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

                #region Create Consolation & Payment:
                var amountToPay = template.Price;
                Consolation consolation;
                Payment payment = null;
                Customer customer;
                using (var ts = new TransactionScope())
                {
                    #region Payment:
                    if (amountToPay > 0)
                    {
                        #region epay:
                        if (!model.PayedByPOS)
                        {
                            var paymentToken = _paymentService.GetToken((int)amountToPay);
                            payment = new Payment()
                            {
                                ID = _paymentService.UniqueID,
                                Amount = (int)amountToPay,
                                Provider = _paymentService.ProviderName,
                                Status = PaymentStatus.pending.ToString(),
                                Token = paymentToken,
                                Type = PaymentType.consolation.ToString(),
                                CreationTime = DateTimeUtils.Now,
                                LastUpdateTime = DateTimeUtils.Now
                            };
                            _paymentRepo.Add(payment);
                            _paymentRepo.Save();
                        }
                        #endregion
                    }
                    #endregion

                    #region Consolation:
                    consolation = Mapper.Map<ConsolationDto, Consolation>(model);
                    consolation.Status = ConsolationStatus.pending.ToString();
                    consolation.PaymentStatus = (amountToPay <= 0 ? PaymentStatus.free.ToString() : PaymentStatus.pending.ToString());
                    consolation.PaymentID = payment?.ID;
                    consolation.AmountToPay = amountToPay;
                    consolation.TrackingNumber = IDGenerator.GenerateConsolationTrackingNumber();
                    consolation.CreationTime = DateTimeUtils.Now;
                    consolation.LastUpdateTime = consolation.CreationTime;
                    #region Customer Info:
                    customer = _customerRepo.Find(consolation.Customer.CellPhoneNumber);
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

                    ts.Complete();
                }
                #endregion

                #region Send SMS To Customer:
                if (consolation.PaymentStatus == PaymentStatus.free.ToString())
                {
                    string messageText = string.Format(SmsMessages.ConsolationCreationSms, consolation.TrackingNumber);
                    SmsUtil.Send(messageText, customer.CellPhoneNumber);
                }
                #endregion

                return Ok(new
                {
                    ID = consolation.ID,
                    TrackingNumber = consolation.TrackingNumber,
                    PaymentID = payment?.ID,
                    PaymentToken = payment?.Token,
                    BankPageUrl = _paymentService.BankPageUrl
                });
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
        [AllowAnonymous]
        public HttpResponseMessage GetPreview(int id, bool? thumb = false)
        {
            try
            {
                var consolation = _consolationRepo.Get(id);
                if (consolation == null)
                    return Request.CreateResponse(HttpStatusCode.NotFound);

                #region return from temp folder if available:
                #region create folders if not exist:
                if (!Directory.Exists(HostingEnvironment.MapPath("~/Content/Temp/")))
                    Directory.CreateDirectory(HostingEnvironment.MapPath("~/Content/Temp/"));

                if (!Directory.Exists(HostingEnvironment.MapPath("~/Content/Temp/Consolations/")))
                    Directory.CreateDirectory(HostingEnvironment.MapPath("~/Content/Temp/Consolations/"));

                if (!Directory.Exists(HostingEnvironment.MapPath("~/Content/Temp/Consolations/Thumbs/")))
                    Directory.CreateDirectory(HostingEnvironment.MapPath("~/Content/Temp/Consolations/Thumbs/"));
                #endregion
                var tempFolderPath = HostingEnvironment.MapPath("~/Content/Temp/Consolations");
                var tempFilePath = $"{tempFolderPath}/{(thumb.HasValue && thumb.Value ? "Thumbs/" : "")}{consolation.ID}.jpg";
                FileInfo fileInfo = File.Exists(tempFilePath) ? new FileInfo(tempFilePath) : null;
                if (fileInfo != null && (!consolation.LastUpdateTime.HasValue || consolation.LastUpdateTime.Value < fileInfo.LastWriteTime))
                {
                    return JpgResponse(File.ReadAllBytes(tempFilePath));
                }
                #endregion

                var blob = _blobRepo.Get(consolation.Template.BackgroundImageID);
                if (blob == null || !(blob is ImageBlob))
                    return Request.CreateResponse(HttpStatusCode.NotFound);

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

                return JpgResponse(previewBytes);
            }
            catch (Exception ex)
            {
                return ExceptionManager.GetExceptionResponse(this, ex);
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

        [HttpGet]
        public IHttpActionResult FindByTrackingNumber(string tn)
        {
            try
            {
                var consolation = _consolationRepo.Find(tn);
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

        [HttpGet]
        public IHttpActionResult FindByObit(string trackingNumber)
        {
            try
            {
                var obit = _obitRepo.FindByTrackingNumber(trackingNumber);
                if (obit == null)
                    return NotFound();

                var consolations = obit.Consolations.ToList();
                var dtos = consolations.Select(c => Mapper.Map<Consolation, ConsolationDto>(c));
                return Ok(dtos);
            }
            catch (Exception ex)
            {
                return ResponseMessage(ExceptionManager.GetExceptionResponse(this, ex));
            }
        }

        [HttpGet]
        public IHttpActionResult GetPreviewInfo(int id)
        {
            try
            {
                var consolation = _consolationRepo.Get(id);
                if (consolation == null)
                    return NotFound();

                var payment = _paymentRepo.Get(consolation.PaymentID);
                if (payment == null)
                    return NotFound();

                var consolationDto = Mapper.Map<Consolation, ConsolationDto>(consolation, opts =>
                {
                    opts.AfterMap((src, dst) =>
                    {
                        dst.Obit = null;
                        dst.Template = null;
                    });
                });
                var paymentDto = Mapper.Map<Payment, PaymentDto>(payment);

                var dto = new ConsolationPreviewDto()
                {
                    Consolation = consolationDto,
                    Payment = paymentDto,
                    MoreData = _paymentService.BankPageUrl
                };

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
        public IHttpActionResult Update(int id, Dictionary<string, string> fields,
            string newStatus = "", int obitId = -1, int templateId = -1)
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
                {
                    var newTemplate = _templateRepo.Get(templateId);

                    consolationToEdit.TemplateID = templateId;
                    consolationToEdit.AmountToPay = newTemplate.Price;
                }

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
                        if (consolationToEdit.PaymentStatus != PaymentStatus.verified.ToString())
                            throw new Exception("Only consolations with verified payments can be confirmed.");

                        if (consolationToEdit.Status == ConsolationStatus.canceled.ToString())
                        {
                            var isDisplayed = _consolationRepo.IsDisplayed(consolationToEdit.ID);
                            consolationToEdit.Status = (!isDisplayed ? ConsolationStatus.confirmed.ToString() : ConsolationStatus.displayed.ToString());
                        }
                        else
                        {
                            consolationToEdit.Status = ConsolationStatus.confirmed.ToString();
                            #region Send SMS:

                            string messageText = String.Format(SmsMessages.ConsolationConfirmSms, consolationToEdit.TrackingNumber);
                            SmsUtil.Send(messageText, consolationToEdit.Customer.CellPhoneNumber);

                            #endregion
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

        [HttpPut]
        public IHttpActionResult UpdateV2(ConsolationDto model)
        {
            try
            {
                var consolationToEdit = _consolationRepo.Get(model.ID);
                if (consolationToEdit == null)
                    return NotFound();

                #region update fields:
                if (model.ObitID > 0)
                    consolationToEdit.ObitID = model.ObitID;

                if (model.TemplateID > 0)
                {
                    var newTemplate = _templateRepo.Get(model.TemplateID);

                    consolationToEdit.TemplateID = model.TemplateID;
                    consolationToEdit.AmountToPay = newTemplate.Price;
                }

                if (!string.IsNullOrEmpty(model.TemplateInfo))
                {
                    var fields = JsonConvert.DeserializeObject<Dictionary<string, string>>(model.TemplateInfo);

                    if (fields.ContainsKey("Audience"))
                        consolationToEdit.Audience = fields["Audience"];
                    if (fields.ContainsKey("From"))
                        consolationToEdit.From = fields["From"];

                    consolationToEdit.TemplateInfo = model.TemplateInfo;
                }
                #endregion

                #region update customer:
                if (model.Customer != null)
                {
                    var customer = _customerRepo.Find(model.Customer.CellPhoneNumber);
                    if (customer != null)
                    {
                        if (consolationToEdit.CustomerID != customer.ID)
                            consolationToEdit.CustomerID = customer.ID;
                    }
                    else
                    {
                        var newCustomer = Mapper.Map<CustomerDto, Customer>(model.Customer);
                        newCustomer.IsMember = false;

                        consolationToEdit.Customer = newCustomer;
                    }
                }
                #endregion

                #region update status:
                if (!string.IsNullOrEmpty(model.Status))
                {
                    var newstatus = (ConsolationStatus)Enum.Parse(typeof(ConsolationStatus), model.Status);
                    if (newstatus == ConsolationStatus.confirmed)
                    {
                        if (consolationToEdit.PaymentStatus != PaymentStatus.verified.ToString())
                            throw new Exception("Only consolations with verified payments can be confirmed.");

                        if (consolationToEdit.Status == ConsolationStatus.canceled.ToString())
                        {
                            var isDisplayed = _consolationRepo.IsDisplayed(consolationToEdit.ID);
                            consolationToEdit.Status = (!isDisplayed ? ConsolationStatus.confirmed.ToString() : ConsolationStatus.displayed.ToString());
                        }
                        else
                        {
                            consolationToEdit.Status = ConsolationStatus.confirmed.ToString();
                            #region Send SMS:
                            string messageText = String.Format(SmsMessages.ConsolationConfirmSms, consolationToEdit.TrackingNumber);
                            SmsUtil.Send(messageText, consolationToEdit.Customer.CellPhoneNumber);
                            #endregion
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
                    //var maxWith = 800;
                    //var resizer = new ImageResizer(backgroundImage.Width, backgroundImage.Height, ResizeType.LongerFix, maxWith);
                    //var resizedBG = ImageUtils.GetThumbnailImage(backgroundImage, resizer.NewWidth, resizer.NewHeight);
                    var resizedBG = backgroundImage;
                    #region draw fields:
                    using (var g = Graphics.FromImage(resizedBG))
                    {
                        g.SmoothingMode = SmoothingMode.AntiAlias;
                        g.TextRenderingHint = TextRenderingHint.AntiAlias;
                        var info = JsonConvert.DeserializeObject<Dictionary<string, string>>(consolation.TemplateInfo);
                        var fields = consolation.Template.TemplateFields.ToList();

                        #region font collection:
                        var fontsFolder = HostingEnvironment.MapPath("~/Content/Fonts/");
                        var fontCollection = new PrivateFontCollection();
                        var fontFiles = Directory.GetFiles(fontsFolder);
                        foreach (var fontFile in fontFiles)
                        {
                            fontCollection.AddFontFile(fontFile);
                        }
                        #endregion

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
                            var fontFamily = fontCollection.Families.Where(ff => ff.Name == field.FontFamily).FirstOrDefault();
                            var fontSize = StringToFontSize(field.FontSize);
                            var font = new Font(fontFamily, fontSize, (field.Bold.HasValue && field.Bold.Value ? FontStyle.Bold : FontStyle.Regular));
                            #region scale to fit:
                            var dist = CalcDistanceToFit(g, box, text, font);
                            if (dist > 0)
                            {
                                var margin = 1;
                                var maxIncrease = 7;
                                var increasedSize = (dist > margin ? (dist - margin < maxIncrease ? dist - margin : maxIncrease) : 0);
                                font = new Font(font.FontFamily, font.SizeInPoints + increasedSize, (field.Bold.HasValue && field.Bold.Value ? FontStyle.Bold : FontStyle.Regular));
                            }
                            else if (dist < 0)
                            {
                                font = new Font(font.FontFamily, font.SizeInPoints + dist, (field.Bold.HasValue && field.Bold.Value ? FontStyle.Bold : FontStyle.Regular));
                            }
                            #endregion
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
            dic.Add("tiny", 13f);
            dic.Add("small", 20f);
            dic.Add("normal", 30f);
            dic.Add("large", 37f);
            dic.Add("huge", 43f);
            return dic.ContainsKey(text) ? dic[text] : dic["normal"];
        }
        private int CalcDistanceToFit(Graphics g, Rectangle box, string text, Font font)
        {
            var dist = 0;
            var stringSize = g.MeasureString(text, font, box.Width);
            if (stringSize.Height < box.Height)
            {
                while (stringSize.Height < box.Height)
                {
                    font = new Font(font.FontFamily, font.SizeInPoints + 1);
                    stringSize = g.MeasureString(text, font, box.Width);
                    dist++;
                }
            }
            else
            {
                while (stringSize.Height > box.Height)
                {
                    font = new Font(font.FontFamily, font.SizeInPoints - 1);
                    stringSize = g.MeasureString(text, font, box.Width);
                    dist--;
                }
            }
            return dist;
        }
        #endregion
    }
}
