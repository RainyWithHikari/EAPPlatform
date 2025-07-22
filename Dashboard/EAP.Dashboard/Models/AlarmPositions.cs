using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAP.Dashboard.Models

{
    [SugarTable("ALARMPOSITIONS")]
    public class AlarmPositions
    {
        [SugarColumn(ColumnName = "EQID")]
        public string EQID { get; set; }
        [SugarColumn(ColumnName = "ALARMCODE")]
        public string AlarmCode { get; set; }
        [SugarColumn(ColumnName = "POSITIONID")]
        public string PositionID { get; set; }

    }
    public class AlarmPositionsDetails {
        public string EQID { get; set; }
        public string AlarmCode { get; set; }
        public DateTime AlarmTime { get; set; }
        public string AlarmText { get; set; }
        public string PositionID { get; set; }
    }
}
