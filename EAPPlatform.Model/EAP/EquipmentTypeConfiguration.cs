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
    /// 设备类型配置
    /// </summary>
	[Table("EQUIPMENTTYPECONFIGURATION")]
    [Display(Name = "_Model.EquipmentTypeConfiguration")]
    public class EquipmentTypeConfiguration : BasePoco,IPersistPoco
    {
        [Display(Name = "_Model._EquipmentTypeConfiguration._TypeName")]
        public EquipmentType TypeName { get; set; }

        [Display(Name = "_Model._EquipmentTypeConfiguration._TypeName")]
        [Required(ErrorMessage = "Validate.{0}required")]
        [Column("TYPENAMEID")]
        public Guid? TypeNameId { get; set; }

        [Display(Name = "_Model._EquipmentTypeConfiguration._ConfigurationItem")]
        [StringLength(100, ErrorMessage = "Validate.{0}stringmax{1}")]
        [Required(ErrorMessage = "Validate.{0}required")]
        [Column("CONFIGURATIONITEM")]
        public string ConfigurationItem { get; set; }

        [Display(Name = "_Model._EquipmentTypeConfiguration._ConfigurationValue")]
        [StringLength(300, ErrorMessage = "Validate.{0}stringmax{1}")]
        [Column("CONFIGURATIONVALUE")]
        public string ConfigurationValue { get; set; }

        [Display(Name = "_Model._EquipmentTypeConfiguration._ConfigurationName")]
        [StringLength(300, ErrorMessage = "Validate.{0}stringmax{1}")]
        [Column("CONFIGURATIONNAME")]
        public string ConfigurationName { get; set; }

        [Display(Name = "_Model._EquipmentTypeConfiguration._DisplayOrder")]
        public int? DisplayOrder { get; set; }
        [Display(Name = "_Model._EquipmentTypeConfiguration._IsValid")]
        [Required(ErrorMessage = "Validate.{0}required")]
        [Column("ISVALID")]
        public bool IsValid { get; set; } = true;
	}

}
