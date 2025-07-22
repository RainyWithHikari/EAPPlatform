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
    /// 设备配置
    /// </summary>
    [Table("EQUIPMENTCONFIGURATION")]
    [Display(Name = "_Model.EquipmentConfiguration")]
    public class EquipmentConfiguration : BasePoco,IPersistPoco
    {
        [Display(Name = "_Model._EquipmentConfiguration._EQID")]
        public Equipment EQID { get; set; }

        [Display(Name = "_Model._EquipmentConfiguration._EQID")]
        [Required(ErrorMessage = "Validate.{0}required")]
        [Column("EQIDID")]
        public Guid? EQIDId { get; set; }

        [Display(Name = "_Model._EquipmentConfiguration._ConfigurationItem")]
        [StringLength(100, ErrorMessage = "Validate.{0}stringmax{1}")]
        [Required(ErrorMessage = "Validate.{0}required")]
        [Column("CONFIGURATIONITEM")]
        public string ConfigurationItem { get; set; }

        [Display(Name = "_Model._EquipmentConfiguration._ConfigurationValue")]
        [StringLength(2000, ErrorMessage = "Validate.{0}stringmax{1}")]
        [Column("CONFIGURATIONVALUE")]
        public string ConfigurationValue { get; set; }

        [Display(Name = "_Model._EquipmentConfiguration._ConfigurationName")]
        [StringLength(300, ErrorMessage = "Validate.{0}stringmax{1}")]
        [Column("CONFIGURATIONNAME")]
        public string ConfigurationName { get; set; }

        [Display(Name = "_Model._EquipmentConfiguration._DisplayOrder")]
        public int? DisplayOrder { get; set; }

        [Display(Name = "_Model._EquipmentConfiguration._IsValid")]
        [Required(ErrorMessage = "Validate.{0}required")]
        [Column("ISVALID")]
        public bool IsValid { get; set; } = true;
        [Column("ISREADONLY")]
        public bool IsReadOnly { get; set; }
    }

}
