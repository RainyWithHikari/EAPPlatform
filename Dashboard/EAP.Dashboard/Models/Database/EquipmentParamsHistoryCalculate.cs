using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace EAP.Models.Database
{
    [SugarTable("EQUIPMENTPARAMSHISCAL")]

    public class EquipmentParamsHistoryCalculate
    {
        [SugarColumn(ColumnName = "EQID")]
        public string EQID { get; set; }

        [SugarColumn(ColumnName = "NAME")]
        public string Name { get; set; }

        [SugarColumn(ColumnName = "VALUE")]
        public string Value { get; set; }

        [SugarColumn(ColumnName = "UPDATETIME")]
        public DateTime UpdateTime { get; set; }

        [SugarColumn(ColumnName = "REMARK")]
        public string Remark { get; set; }
    }
    public class HistoryParamsCheck
    {
        public string EQID { get; set; }
        public DateTime LAST_UPDATE_TIME { get; set; }
    }

    public class EquipmentAlarmTimes
    {
        public string time { get; set; }
        public double alarmtimes { get; set; }

        public double mfgtimes { get; set; }
    }
}
