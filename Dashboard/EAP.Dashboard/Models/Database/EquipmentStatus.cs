using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAP.Models.Database
{
    [SugarTable("EQUIPMENTSTATUS")]
    public class EquipmentStatus
    {
        [SugarColumn(ColumnName = "EQID")]
        public string EQID { get; set; }

        [SugarColumn(ColumnName = "EQTYPE")]
        public string EQType { get; set; } = String.Empty;
        [SugarColumn(ColumnName = "STATUS")]
        public string Status { get; set; }

        [SugarColumn(ColumnName = "DATETIME")] 
        public DateTime DateTime { get; set; } = DateTime.Now;
    }

    public class OTMSStatus
    {
        public string EQID { get; set; }
        public string CHAMBERID { get; set; }
        public string STATUS { get; set; }

        public string TEMPERTURE { get; set; }
        public string LOCATION { get; set; }
    }
}
