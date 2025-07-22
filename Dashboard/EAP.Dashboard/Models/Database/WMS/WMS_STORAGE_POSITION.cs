using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAP.Models.Database.WMS
{
    [SugarTable("WMS_STORAGE_POSITION")]
    public class WMS_STORAGE_POSITION
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; } //主键
        public string LOCATION_NAME { get; set; }
        public string LOCATION_CODE { get; set; }
        public string LOCATION_X { get; set; }

        public string LOCATION_Y { get; set; }

        public string LOCATION_Z { get; set; }


        public string TRAY_CODE { get; set; }

        public decimal REEL_NUM { get; set; }

        public DateTime UPDATE_TIME { get; set; }

        public string LOCATION_ZONE { get; set; }


    }
}
