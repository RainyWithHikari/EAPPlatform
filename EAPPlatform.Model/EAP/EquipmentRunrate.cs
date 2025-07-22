using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WalkingTec.Mvvm.Core;
using System.Text.Json.Serialization;
using EAPPlatform.Model;
using EAPPlatform.Model.EAP;

namespace EAPPlatform.Model.EAP
{
    [Table("RUNRATERESULT")]
    public class EquipmentRunrate 
    {
        [Key]
        [StringLength(100)]
        [Display(Name = "_Model.EquipmentRunrate._EQID")]
        [Column("EQID")]
        public string EQID { get; set; }

        [Display(Name = "_Model.EquipmentRunrate._RunrateValue")]
        [Column("RUNRATEVALUE", TypeName = "NUMBER")]
        public double RunrateValue { get; set; }

        [Display(Name = "_Model.EquipmentRunrate._CheckTime")]
        [Column("CHECKTIME")]
        public DateTime CheckTime { get; set; }
    }
}
