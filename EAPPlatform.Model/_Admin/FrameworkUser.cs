using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WalkingTec.Mvvm.Core;
using System.Text.Json.Serialization;
using EAPPlatform.Model;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// 用户
    /// </summary>
	[Table("FrameworkUsers")]
    [Display(Name = "_Model.FrameworkUser")]
    public class FrameworkUser : FrameworkUserBase
    {
        [Display(Name = "_Model._FrameworkUser._Email")]
        [StringLength(50, ErrorMessage = "Validate.{0}stringmax{1}")]
        [RegularExpression("^[a-zA-Z0-9_-]+@[a-zA-Z0-9_-]+(\\.[a-zA-Z0-9_-]+)+$", ErrorMessage = "Validate.{0}formaterror")]
        public string Email { get; set; }
        [Display(Name = "_Model._FrameworkUser._Gender")]
        public GenderEnum? Gender { get; set; }
        [Display(Name = "_Model._FrameworkUser._CellPhone")]
        [RegularExpression("^[1][345789][0-9]{9}$", ErrorMessage = "Validate.{0}formaterror")]
        public string CellPhone { get; set; }
        [Display(Name = "_Model._FrameworkUser._HomePhone")]
        [StringLength(30, ErrorMessage = "Validate.{0}stringmax{1}")]
        [RegularExpression("^[-0-9\\s]{8,30}$", ErrorMessage = "Validate.{0}formaterror")]
        public string HomePhone { get; set; }
        [Display(Name = "_Model._FrameworkUser._Address")]
        [StringLength(200, ErrorMessage = "Validate.{0}stringmax{1}")]
        public string Address { get; set; }
        [Display(Name = "_Model._FrameworkUser._ZipCode")]
        [RegularExpression("^[0-9]{6,6}$", ErrorMessage = "Validate.{0}formaterror")]
        public string ZipCode { get; set; }
	}

}
