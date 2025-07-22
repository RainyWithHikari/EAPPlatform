using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EAP.Dashboard.Models.Database
{
    //[SugarTable("USI_MONITOR.GET_STATION_OEE")]
    public class GET_STATION_OEE
    {
        public string EQ_ID { get; set; }
        public string OEE_CNT { get; set; }
        public DateTime EVENT_OCCUR_DATE { get; set; }
    }
}