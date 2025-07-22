using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EAP.Dashboard.Models.Database
{
    [SugarTable("NEEDREPORTDEVICE")]
    public class NEEDREPORTDEVICE
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }
        public string STATION {  get; set; }
        public string EQID { get; set; }
        public string OUTPUTEQID { get; set; }
        public string REMARK {  get; set; }
        public string OUTPUTSTATION { get; set; }
    }
}