using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAPPlatform.Model.EAP
{
    [Table("ALARMPOSITIONS")]
    public class AlarmPositions
    {
        [Column("EQID")]
        public string EQID { get; set; }
        [Column("ALARMCODE")]
        public string AlarmCode { get; set; }
        [Column("POSITIONID")]
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
