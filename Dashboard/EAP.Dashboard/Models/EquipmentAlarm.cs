using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EAP.Dashboard.Models
{
    [SugarTable("EQUIPMENTALARM")]
    public class EquipmentAlarm
    {
        [SugarColumn(ColumnName = "EQID")]
        public string AlarmEqp { get; set; }
        [SugarColumn(ColumnName = "ALARMCODE")]
        public string AlarmCode { get; set; }

        [SugarColumn(ColumnName = "ALARMTEXT")]
        public string AlarmText { get; set; }

        [SugarColumn(ColumnName = "ALARMSOURCE")]
        public string AlarmSource { get; set; }

        [SugarColumn(ColumnName = "ALARMTIME")]
        public DateTime? AlarmTime { get; set; }

        [SugarColumn(ColumnName = "ALARMSET")]
        public bool AlarmSet { get; set; }
    }

    public class AlarmItem
    {
        public string AlarmDate { get; set; }
        public string EQID { get; set; }
        public string AlarmCode { get; set; }
        public string AlarmText { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public double Duration { get; set; }

    }
}