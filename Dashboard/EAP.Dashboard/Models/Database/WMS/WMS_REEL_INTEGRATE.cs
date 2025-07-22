using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAP.Models.Database.WMS
{
    [SugarTable("WMS_REEL_INTEGRATE")]
    public class WMS_REEL_INTEGRATE
    {
        [SugarColumn(ColumnName = "EQID")]
        public string EQID { get; set; }

        [SugarColumn(ColumnName = "OUTPUT_DATE")]
        public string DateTime { get; set; } //记录产出日期

        [SugarColumn(ColumnName = "OPERATE_TIME")]
        public DateTime OperateTime { get; set; }

        [SugarColumn(ColumnName = "INTEGRATED_TYPE")]
        //合盘种类：四合一，三合一，二合一
        public string IntegratedType { get; set; }

        [SugarColumn(ColumnName = "REEL_PACK_NUMBER")]
        //料盘大小：10k，15k，20k
        public decimal ReelSize { get; set; }
        [SugarColumn(ColumnName = "WORK_ORDER")]
        public string WorkOrder { get; set; }

        [SugarColumn(ColumnName = "MODEL_NAME")]
        public string ModelName { get; set; }

        [SugarColumn(ColumnName = "PART_NUMBER")]
        public string PartNumber { get; set; }

        [SugarColumn(ColumnName = "LOT_CODE")]
        public string LotNo { get; set; }

        [SugarColumn(ColumnName = "DC_CODE")]
        public string DcNo { get; set; }

        [SugarColumn(ColumnName = "REEL_QTY")]
        public int QTY { get; set; }

        [SugarColumn(ColumnName = "CT")]
        public double CT { get; set; }

        [SugarColumn(ColumnName = "WORK_TIME")]
        public double WorkTime { get; set; }


    }
}
