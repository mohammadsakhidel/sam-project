using SamDataAccess.Repos.Interfaces;
using SamReportLib.Models;
using SamReportLib.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

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

            #region retrieve data:
            var records = _mosqueRepo.GetMosquesTurnover(query.BeginDate, query.EndDate, query.ProvinceID, query.CityID, query.MosqueID);
            #endregion

            #region prepare report model:
            var dataItems = new List<MosqueTurnoverRecord>();
            foreach (var r in records)
            {
                var item = new MosqueTurnoverRecord();
                item.MosqueName = r.Item1.Name;
                item.ObitsCount = r.Item2.Count();
                item.ConsolationCount = r.Item3.Count();
                item.ConsolationAverage = item.ObitsCount > 0 ? (double)item.ConsolationCount / item.ObitsCount : 0;
                item.TotalIncome = (int)r.Item3.Sum(c => c.AmountToPay);

                dataItems.Add(item);
            }
            #endregion

            return Ok(dataItems.OrderBy(d => d.MosqueName));
        }
        #endregion

        #region Methods:

        #endregion
    }
}
