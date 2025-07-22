using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAP.Models.Database.RMS
{
    public class RMS_RECIPE_DATA
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; } = Guid.NewGuid().ToString("N");
        [SugarColumn(ColumnName ="RECIPE_NAME")]
        public string NAME { get; set; }
        public string VERSION_ID { get; set; }
        public byte[] CONTENT { get; set; }
        public string CREATOR { get; set; } = "Default";
        public DateTime CREATE_TIME { get; set; } = DateTime.Now;
    }
}
