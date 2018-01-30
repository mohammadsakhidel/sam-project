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
        #endregion

        #region Constructors:
        public ReportsController(IConsolationRepo consolationRepo)
        {
            _consolationRepo = consolationRepo;
        }
        #endregion

        #region POST Methods:
        [HttpPost]
        public IHttpActionResult MosqueTurnoverRecords(MosquesTurnoverQuery query)
        {

            #region retrieve data:
            var records = _consolationRepo.Query(query.BeginDate, query.EndDate, query.ProvinceID, query.CityID, query.MosqueID);
            var groups = records.GroupBy(r => r.Item2.ID);
            #endregion

            #region prepare report model:
            var dataItems = new List<MosqueTurnoverRecord>();
            foreach (var g in groups)
            {
                
            }
            #endregion

            return Ok();
        }
        #endregion

        #region Methods:

        #endregion
    }
}
