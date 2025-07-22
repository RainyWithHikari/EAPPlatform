using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAP.Models.Database.WMS
{
    [SugarTable("WMS_SAP_MAT_ISSUEING")]
    public class WMS_SAP_MAT_ISSUEING
    {
        public string ID { get; set; } //订单号，主键
        public string ISSUE_DATE { get; set; }
        public string PART_NUMBER { get; set; }
        public string PLANT_CODE { get; set; }

        public string SLOC { get; set; }
        public decimal QTY { get; set; }

        public string MVT { get; set; }

        public string BATCH_ID { get; set; }
        public string EUN { get; set; }
        public string WORK_ORDER { get; set; }

        public string MAT_GROUP { get; set; }



    }

}
