using iTextSharp.text;
using iTextSharp.text.pdf;
using SamModels.DTOs;
using SamUtils.Constants;
using SamUtils.Utils;
using SamUxLib.Resources.Values;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SamWeb.Controllers
{
    public class ObitsController : Controller
    {
        #region GET:
        public ActionResult Resume()
        {
            return View();
        }
        public ActionResult DownloadResume(string tn)
        {
            var url = $"~/Content/Downloads/Resumes/{tn}.pdf";
            return Redirect(url);
            //var filePath = Server.MapPath(url);
            //return File(filePath, "application/pdf", $"{tn}.pdf");
        }
        #endregion

        #region POST:
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Resume(string trackingNumber)
        {
            try
            {
                using (var hc = HttpUtil.CreateClient())
                {
                    Thread.Sleep(2000);

                    #region create and save pdf file:
                    #region query data:
                    List<ConsolationDto> consolations = new List<ConsolationDto>();
                    var response = await hc.GetAsync($"{ApiActions.consolations_findbyobit}?trackingNumber={trackingNumber}");
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                        throw new Exception(Messages.NoItem);
                    response.EnsureSuccessStatusCode();
                    consolations = await response.Content.ReadAsAsync<List<ConsolationDto>>();
                    #endregion
                    #region save pdf file:
                    var filePath = Server.MapPath($"~/Content/Downloads/Resumes/{trackingNumber}.pdf");
                    using (var fs = new FileStream(filePath, FileMode.Create))
                    {
                        #region open doc:
                        var margin = 50;
                        var pSize = PageSize.A4;
                        var document = new Document(pSize, 0, 0, margin, 0);
                        var writer = PdfWriter.GetInstance(document, fs);
                        document.Open();
                        #endregion

                        #region prepare document:
                        if (consolations.Any())
                        {
                            #region add consolations to document:
                            var table = new PdfPTable(1);
                            table.DefaultCell.Border = Rectangle.NO_BORDER;
                            var cellIndex = 0;

                            foreach (var c in consolations)
                            {
                                #region get image bytes:
                                byte[] imageBytes = await hc.GetByteArrayAsync($"{ApiActions.consolations_getpreview}/{c.ID}");
                                #endregion

                                #region add image:
                                var image = Image.GetInstance(imageBytes);
                                var h = (pSize.Height - margin * 3) / 2;
                                var w = Math.Min(image.Width * h / image.Height, image.Width - margin * 2);
                                image.ScaleToFit(w, h);
                                image.Alignment = Element.ALIGN_CENTER;

                                var cell = new PdfPCell();
                                cell.AddElement(image);
                                cell.Border = Rectangle.NO_BORDER;
                                if (cellIndex % 2 == 0)
                                    cell.PaddingBottom = margin;
                                table.AddCell(cell);
                                #endregion

                                cellIndex++;
                            }

                            document.Add(table);
                            #endregion
                        }
                        else
                        {
                            document.Add(new Paragraph(" "));
                        }
                        #endregion

                        #region close:
                        document.Close();
                        writer.Close();
                        #endregion
                    }
                    #endregion
                    #endregion

                    ViewBag.TrackingNumber = trackingNumber;
                    return PartialView("Partials/_ObitResumeDownloadLink");
                }
            }
            catch (Exception ex)
            {
                return PartialView("Partials/_Error", ex);
            }
        }
        #endregion

        #region Methods:
        public async Task<ObitDto> FindByTrackingNumberFromApi(string trackingNumber)
        {
            using (var hc = HttpUtil.CreateClient())
            {
                var response = await hc.GetAsync($"{ApiActions.obits_find}?trackingnumber={trackingNumber}");
                HttpUtil.EnsureSuccessStatusCode(response);
                var obit = await response.Content.ReadAsAsync<ObitDto>();
                return obit;
            }
        }
        #endregion
    }
}