using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace EAP.Models.Database
{
    [SugarTable("HISTORYMTBARESULT")]
    public class HistoryMTBAResult
    {
        public byte[] ID { get; set; }
        [SugarColumn(ColumnName = "EQID")]
        public string EQID { get; set; }

        [SugarColumn(ColumnName = "MTBAVALUE")]
        public double MTBAValue { get; set; }

        [SugarColumn(ColumnName = "CHECKTIME")]
        public DateTime CheckTime { get; set; }

        [SugarColumn(ColumnName = "DATATIME")]
        public DateTime DataTime { get; set; }

        [SugarColumn(ColumnName = "SHIFT")]
        public string Shift { get; set; }
    }
    public class HistoryMTBACheck
    {
        public string EQID { get; set; }
        public DateTime LAST_UPDATE_TIME { get; set; }
    }
}
