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
    /// 设备清单
    /// </summary>
	[Table("EQUIPMENT")]
    [Display(Name = "_Model.Equipment")]
    public class Equipment : BasePoco,IPersistPoco
    {
        [Display(Name = "_Model._Equipment._EQID")]
        [StringLength(100, ErrorMessage = "Validate.{0}stringmax{1}")]
        [Required(ErrorMessage = "Validate.{0}required")]
        public string EQID { get; set; }
        [Display(Name = "_Model._Equipment._EquipmentType")]
        public EquipmentType EquipmentType { get; set; }
        [Display(Name = "_Model._Equipment._EquipmentType")]
        [Column("EQUIPMENTTYPEID")]
        public Guid? EquipmentTypeId { get; set; }
        [Display(Name = "_Model._Equipment._Name")]
        [StringLength(100, ErrorMessage = "Validate.{0}stringmax{1}")]
        [Required(ErrorMessage = "Validate.{0}required")]
        [Column("NAME")]
        public string Name { get; set; }
        [Display(Name = "设备配置")]
        [InverseProperty("EQID")]
        public List<EquipmentConfiguration> EquipmentConfiguration_EQID { get; set; }
        [Display(Name = "_Model._Equipment._IsValid")]
        [Required(ErrorMessage = "Validate.{0}required")]
        [Column("ISVALID")]
        public bool IsValid { get; set; } = true;

        [Column("SITE")]
        public string Site { get; set; }

        [Column("PD")]
        public string PD { get; set; }

        [Column("LINE")]
        public string Line { get; set; }

        [Column("SORT")]
        public int Sort { get; set; }

    }

}
