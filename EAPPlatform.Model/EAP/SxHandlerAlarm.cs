using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;

namespace EAPPlatform.Model.EAP
{
    [Table("SXHANDLERLOG")]
    public class SxHandlerAlarm
    {
        [Column("EQP_ALID")]
        public string EQP_ALID { get; set; }

        [Column("PART")]
        public string PART { get; set; }

        [Column("MESSAGE")]
        public string MESSAGE { get; set; }

        [Column("OCCURRED_TIME")]
        public string OCCURRED_TIME { get; set; }

        [Column("RECOVERY")]
        public string RECOVERY { get; set; }

        [Column("STOP_TIME")]
        public decimal STOP_TIME { get; set; }

        [Column("USER_INFO")]
        public string USER_INFO { get; set; }

        [Column("ERROR_LEVEL")]
        public decimal ERROR_LEVEL { get; set; }

        [Column("POSITION")]
        public decimal POSITION { get; set; }

        [Column("EQID")]
        public string EQID { get; set; }
    }

    public class AlarmDetails
    {
        public string Equipment { get; set; }
        public string OccuredDate { get; set; }
        public string OccuredTime { get; set; }
        public string AlarmId { get; set; }
        public string Part { get; set; }
        public string Message { get; set; }
        public string Recovery { get; set; }
        public decimal StopTime { get; set; }
    }

    public class AlarmDetailsSort: AlarmDetails
    {
        public string Times { get; set; } = "1";
        public decimal HANDLER_JAM { get; set; }
        public decimal CLEAN_JAM { get; set; }
        public decimal FIXTURE_JAM { get; set; }
    }

    public class InputSum
    {
        public string NAME { get; set; }
        public string EQID { get; set; }
        public decimal INPUT { get; set; }
    }

    public class InputSumbyDay:InputSum
    {
        public DateTime DATETIME { get; set; }
    }

}
