using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WalkingTec.Mvvm.Core;

namespace EAPPlatform.Model.EAP
{
    [Table("TestModel")]
    [Display(Name = "_Model._TestModel")]
    public class TestModel : BasePoco
    {
        [Display(Name = "_Model._TestModel._Name")]
        [StringLength(100, ErrorMessage = "Validate.{0}stringmax{1}")]
        [Required(ErrorMessage = "Validate.{0}required")]
        public string Name { get; set; }

        public string Name1 { get; set; }
    }
}
