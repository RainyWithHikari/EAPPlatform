
using System;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Attributes;

namespace EAPPlatform.Model
{
    public class _PlaceHolder_
    {
    }
    [MiddleTable]
    public class FrameworkRoleFrameworkUser_MT_Wtm : BasePoco
    {
        [Display(Name = "_Model.FrameworkRole")]
        public FrameworkRole FrameworkRole { get; set; }
        [Display(Name = "_Model.FrameworkRole")]
        public Guid? FrameworkRoleId { get; set; }
        [Display(Name = "_Model.FrameworkUser")]
        public FrameworkUser FrameworkUser { get; set; }
        [Display(Name = "_Model.FrameworkUser")]
        public Guid? FrameworkUserId { get; set; }
    }
    [MiddleTable]
    public class FrameworkGroupFrameworkUser_MT_Wtm : BasePoco
    {
        [Display(Name = "_Model.FrameworkGroup")]
        public FrameworkGroup FrameworkGroup { get; set; }
        [Display(Name = "_Model.FrameworkGroup")]
        public Guid? FrameworkGroupId { get; set; }
        [Display(Name = "_Model.FrameworkUser")]
        public FrameworkUser FrameworkUser { get; set; }
        [Display(Name = "_Model.FrameworkUser")]
        public Guid? FrameworkUserId { get; set; }
    }
}
