using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace EAP.Models.Database
{
    [SugarTable("RUNRATERESULT")]
    public class RunrateResult
    {
        //public byte[] ID { get; set; }

        [SugarColumn(ColumnName = "EQID")]
        public string EQID { get; set; }

        [SugarColumn(ColumnName = "RUNRATEVALUE")]
        public double RunrateValue { get; set; }

        [SugarColumn(ColumnName = "CHECKTIME")]
        public DateTime CheckTime { get; set; }

    }
}
