using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAPPlatform.Model.EAP
{
    [Table("EQUIPMENTPARAMSHISCAL")]
    
    public class EquipmentParamsHistoryCalculate
    {
        [Column("EQID")]
        public string EQID { get; set; }

        [Column("NAME")]
        public string Name { get; set; }

        [Column("VALUE")]
        public string Value { get; set; }

        [Column("UPDATETIME")]
        public DateTime UpdateTime { get; set; }

        [Column("REMARK")]
        public string Remark { get; set; }
    }

    public class EquipmentAlarmTimes
    {
        public string time { get; set; }
        public double alarmtimes { get; set; }

        public double mfgtimes { get; set; }
    }
}
