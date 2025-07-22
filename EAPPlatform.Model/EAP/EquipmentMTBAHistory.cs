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
    [Table("HISTORYMTBARESULT")]
    public class EquipmentMTBAHistory : TopBasePoco
    {
        [Column("EQID")]
        public string EQID { get; set; }

        [Column("MTBAVALUE", TypeName = "NUMBER")]
        public double MTBAValue { get; set; }

        [Column("CHECKTIME")]
        public DateTime CheckTime { get; set; }

        [Column("DATATIME")]
        public DateTime DataTime { get; set; }

        [Column("SHIFT")]
        public string Shift { get; set; }
    }
}
