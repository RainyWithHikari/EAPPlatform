using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace EAP.Models
{
    [SugarTable("EQUIPMENTEVENT")]
    public class EventMessage
    {
        public byte[] ID { get; set; }

        [SugarColumn(ColumnName = "EQID")]
        public string EventEqp { get; set; }

        //[SugarColumn(ColumnName = "EVENTID")]
        //public string EventID { get; set; }

        [SugarColumn(ColumnName = "EQPTYPE")]
        public string EqpType { get; set; } 

        [SugarColumn(ColumnName = "EVENTTIME")]
        public DateTime EventTime { get; set; }

        [SugarColumn(ColumnName = "EQPSTATUS")]
        public string EqpStatus { get; set; }

    }
}
