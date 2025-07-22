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
    [Table("EQUIPMENTPARAMSHISRAW")]
    public class EquipmentParamsHistoryRaw
    {
        [Column("EQID")]
        public string EQID { get; set; }

        [Column("NAME")]
        public string Name { get; set; }

        [Column("VALUE")]
        public string Value { get; set; }
        
        [Column("UPDATETIME")]
        public DateTime UpdateTime { get; set; }

        
    }

   
}
