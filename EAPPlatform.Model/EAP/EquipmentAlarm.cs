using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;

namespace EAPPlatform.Model.EAP
{
    [Table("EQUIPMENTALARM")]
    public class EquipmentAlarm: TopBasePoco
    {
        [StringLength(100)]
        [Display(Name = "_Model._EquipmentAlarm._AlarmEqp")]
        [Column("EQID")]
        public string AlarmEqp { get; set; }

        [StringLength(100)]
        [Display(Name = "_Model._EquipmentAlarm._AlarmCode")]
        [Column("ALARMCODE")]
        public string AlarmCode { get; set; }

        [StringLength(1000)]
        [Display(Name = "_Model._EquipmentAlarm._AlarmText")]
        [Column("ALARMTEXT")]
        public string AlarmText { get; set; }

        [StringLength(100)]
        [Display(Name = "_Model._EquipmentAlarm._AlarmSource")]
        [Column("ALARMSOURCE")]
        public string AlarmSource { get; set; }

        [Display(Name = "_Model._EquipmentAlarm._AlarmTime")]
        [Column("ALARMTIME")]
        public DateTime? AlarmTime { get; set; }

        /// <summary>
        /// 表示报警开始，结束
        /// </summary>
        [Display(Name = "_Model._EquipmentAlarm._AlarmSet")]
        [Column("ALARMSET")]
        public bool AlarmSet { get; set; }

    }

    public class AlarmItem
    {
        public string Date { get; set; }
        public string EQID { get; set; }
        public string AlarmCode { get; set; }
        public string AlarmText { get; set; }
    
        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public double Duration { get; set; }

    }
}
