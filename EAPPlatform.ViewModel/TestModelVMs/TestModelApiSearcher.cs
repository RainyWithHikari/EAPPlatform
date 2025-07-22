using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using EAPPlatform.Model.EAP;


namespace EAPPlatform.ViewModel.TestModelVMs
{
    public partial class TestModelApiSearcher : BaseSearcher
    {
        [Display(Name = "_Model._TestModel._Name")]
        public String Name { get; set; }

        protected override void InitVM()
        {
        }

    }
}
