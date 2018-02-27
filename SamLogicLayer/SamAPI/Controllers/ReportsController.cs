using SamDataAccess.Repos.Interfaces;
using SamReportLib.Models;
using SamReportLib.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using SamAPI.Code.Utils;

namespace SamAPI.Controllers
{
    public class ReportsController : ApiController
    {
        #region Fields:
        private IConsolationRepo _consolationRepo;
        private IMosqueRepo _mosqueRepo;
        #endregion

        #region Constructors:
        public ReportsController(IConsolationRepo consolationRepo, IMosqueRepo mosqueRepo)
        {
            _consolationRepo = consolationRepo;
            _mosqueRepo = mosqueRepo;
        }
        #endregion

        #region POST Methods:
        [HttpPost]
        public IHttpActionResult MosqueTurnoverRecords(MosquesTurnoverQuery query)
        {
            try
            {
                #region retrieve data:
                var records = _mosqueRepo.GetMosquesTurnover(query.BeginDate, query.EndDate, query.ProvinceID, query.CityID, query.MosqueID);
                #endregion

                #region prepare report model:
                var dataItems = new List<MosqueTurnoverRecord>();
                foreach (var r in records)
                {
                    var item = new MosqueTurnoverRecord
                    {
                        MosqueName = r.Item1.Name,
                        ObitsCount = r.Item2.Count(),
                        ConsolationCount = r.Item3.Count()
                    };
                    item.ConsolationAverage = item.ObitsCount > 0 ? (double)item.ConsolationCount / item.ObitsCount : 0;
                    item.TotalIncome = (int)r.Item3.Sum(c => c.AmountToPay);

                    dataItems.Add(item);
                }
                #endregion

                return Ok(dataItems.OrderBy(d => d.MosqueName));
            }
            catch (Exception ex)
            {
                return ResponseMessage(ExceptionManager.GetExceptionResponse(this, ex));
            }
        }

        [HttpPost]
        public IHttpActionResult ConsolationsReport(ConsolationsQuery query)
        {
            try
            {
                #region retrieve data:
                var records = _consolationRepo.Query(query.BeginDate, query.EndDate, query.ProvinceID, 
                    query.CityID, query.MosqueID, query.Status, query.CustomerCellphone, query.TrackingNumber);
                #endregion

                #region prepare report model:
                var dataItems = new List<ConsolationRecord>();
                foreach (var c in records)
                {
                    dataItems.Add(new ConsolationRecord()
                    {
                        ID = c.ID,
                        Status = c.Status,
                        TrackingNumber = c.TrackingNumber,
                        Amount = (int)c.AmountToPay,
                        Content = c.TemplateInfo,
                        CreationTime = c.CreationTime,
                        CustomerCellPhone = c.Customer.CellPhoneNumber,
                        MosqueName = c.Obit.Mosque.Name,
                        ObitTitle = c.Obit.Title,
                        TemplateTitle = c.Template.Name
                    });
                }
                #endregion

                return Ok(dataItems.OrderBy(d => d.CreationTime));
            }
            catch (Exception ex)
            {
                return ResponseMessage(ExceptionManager.GetExceptionResponse(this, ex));
            }
        }
        #endregion

        #region Methods:

        #endregion
    }
}
