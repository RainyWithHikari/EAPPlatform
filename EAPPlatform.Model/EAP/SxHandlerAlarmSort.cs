using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;

namespace EAPPlatform.Model.EAP
{
    [Table("SXHANDLERALARMSORT")]
    public class SxHandlerAlarmSort
    {
        [Column("ALARMCODE")]
        public string ALARMCODE { get; set; }

        [Column("HANDLER_JAM")]
        public int HANDLER_JAM { get; set; }

        [Column("CLEAN_JAM")]
        public int CLEAN_JAM { get; set; }

        [Column("FIXTURE_JAM")]
        public int FIXTURE_JAM { get; set; }
    }

    public class AlarmData
    {
        public string Date { get; set; }
        public string AlarmType { get; set; }
        public int TotalCount { get; set; }
        public int TotalSeconds { get; set; }
        public int StandbySeconds { get; set; }
    }

    public class KeyParamsData
    {
        public string Date { get; set; }
        public string AlarmType { get; set; }
        public double Value { get; set; }
    }

    public class HandlerData
    {
        public List<string> Equipments { set; get; }
        public List<double> MTTAData { get; set; }
        public List<double> MTBAData { get; set; }
        public List<double> JAMData { get; set; }

        //public string Date { get; set; }
        //public string EQNAME { get; set; }
        //public double MTTA { get; set; }
        //public double MTBA { get; set; }
        //public double JAM { get; set; }
    }

    public class HandlerStandbyData
    {
        public string EQP_ALID { get; set; }
        public string OCCURRED_TIME { get; set; }
        public string EQID { get; set; }
        public string NEXT_ALID { get; set; }
        public string NEXT_OCCURRED_TIME { get; set; }
        public double DURATION
        {
            get
            {
                var startTime = Convert.ToDateTime(OCCURRED_TIME);
                var endTime = Convert.ToDateTime(NEXT_OCCURRED_TIME);
                return (endTime - startTime).TotalSeconds;
            }
        }
    }
}
