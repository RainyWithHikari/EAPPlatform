using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAP.Models.Database.WMS
{
    [SugarTable("WMS_PART")]
    public partial class WMS_PART
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string PART_NUMBER { get; set; } //料号

        public string BIN_NO { get; set; }

        public string SLOC { get; set; }

        public string PLAN { get; set; }

        public string MAT_GROUP { get; set; }

        public string BATCH_ID { get; set; } //批次号，SAP生成


        public int ROUND_VALUE { get; set; } //最小包装量

        public string REEL_SIZE { get; set; } //卷盘规格

        public string PACK_TYPE { get; set; } //包装方式

        public string LAST_EDITOR { get; set; }

        public DateTime? LAST_EDIT_TIME { get; set; }

    }
}
