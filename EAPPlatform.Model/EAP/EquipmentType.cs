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
    /// <summary>
    /// 设备类型
    /// </summary>
	[Table("EQUIPMENTTYPE")]
    [Display(Name = "_Model.EquipmentType")]
    public class EquipmentType : BasePoco,IPersistPoco
    {
        [Display(Name = "_Model._EquipmentType._Name")]
        [StringLength(100, ErrorMessage = "Validate.{0}stringmax{1}")]
        [Required(ErrorMessage = "Validate.{0}required")]
        [Column("NAME")]
        public string Name { get; set; }

        [Display(Name = "设备清单")]
        [InverseProperty("EquipmentType")]
        public List<Equipment> Equipment_EquipmentType { get; set; }

        [Display(Name = "设备类型配置")]
        [InverseProperty("TypeName")]
        public List<EquipmentTypeConfiguration> EquipmentTypeConfiguration_TypeName { get; set; }

        [Display(Name = "_Model._EquipmentType._IsValid")]
        [Required(ErrorMessage = "Validate.{0}required")]
        [Column("ISVALID")]
        public bool IsValid { get; set; } = true;
	}

}
