using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace EAP.Models.Database
{
    [SugarTable("MTBARESULT")]
    public class MTBAResult
    {
        //public byte[] ID { get; set; }

        [SugarColumn(ColumnName = "EQID")]
        public string EQID { get; set; }

        [SugarColumn(ColumnName = "MTBAVALUE")]
        public double MTBAValue { get; set; }

        [SugarColumn(ColumnName = "CHECKTIME")]
        public DateTime CheckTime { get; set; }

    }
}
