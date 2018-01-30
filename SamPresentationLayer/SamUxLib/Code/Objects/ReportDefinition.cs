using SamUxLib.Code.Enums;
using SamUxLib.Resources.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamUxLib.Code.Objects
{
    public class ReportDefinition
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public ReportType Type { get; set; }
        public string Data { get; set; }

        public static List<ReportDefinition> All
        {
            get
            {
                return new List<ReportDefinition>() {
                    new ReportDefinition { ID = "mosque_turnover", Title = Strings.ReportType_MosquesTurnover, Type = ReportType.sti },
                    new ReportDefinition { ID = "mosque_turnover_chart", Title = Strings.ReportType_MosquesTurnoverChart, Type = ReportType.sti }
                    //new ReportDefinition { ID = "daily_turnover", Title = Strings.ReportType_DailyTurnover, Type = ReportType.sti }
                };
            }
        }
    }
}
