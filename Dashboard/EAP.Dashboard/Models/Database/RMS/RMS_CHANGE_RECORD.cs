using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAP.Models.Database.RMS
{
    [SugarTable("RMS_CHANGE_RECORD")]
    public partial class RMS_CHANGE_RECORD
    {
        public RMS_CHANGE_RECORD()
        {
        }


        [SugarColumn(ColumnName ="EQP_CONFIG_ID")]
        public string EQID { get; set; }
        public string FROM_RECIPE_NAME { get; set; }
        public string FROM_RECIPE_VERSION { get; set; }
        public string TO_RECIPE_NAME { get; set; }
        public string TO_RECIPE_VERSION { get; set; }
        public string CREATOR { get; set; } = "Default";
        public DateTime? CREATETIME { get; set; } = DateTime.Now;



    }
}
