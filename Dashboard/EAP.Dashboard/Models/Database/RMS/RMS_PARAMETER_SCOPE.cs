using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAP.Models.Database.RMS
{
    [SugarTable("RMS_PARAMETER_SCOPE")]
    public partial class RMS_PARAMETER_SCOPE
    { 
        [SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }

        public double LCL { get; set; }

        public double UCL { get; set; }
        public string ENUMVALUE { get; set; }

        public string RECIPE_ID { get; set; }

        public string PARAMETER_NAME { get; set; }
        public string TYPE { get; set; }

        public string LAST_EDITOR { get; set; }

        public DateTime LAST_EDIT_TIME { get; set; }
    }
}
