using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace EAP.Models.Database
{
    [SugarTable("EQUIPMENTALARM")]
    public class AlarmMessage
    {
        public byte[] ID { get; set; }

        [SugarColumn(ColumnName = "EQID")]
        public string AlarmEqp { get; set; }

        [SugarColumn(ColumnName = "ALARMCODE")]
        public string AlarmCode { get; set; }


        [SugarColumn(ColumnName = "ALARMTEXT")]
        public string AlarmText { get; set; }

        [SugarColumn(ColumnName = "ALARMSOURCE")]
        public string AlarmSource { get; set; }

        [SugarColumn(ColumnName = "ALARMTIME")]
        public DateTime AlarmTime { get; set; }

        /// <summary>
        /// 表示报警开始，结束
        /// </summary>
        [SugarColumn(ColumnName = "ALARMSET")]
        public bool AlarmSet { get; set; } = true;
    }
}
