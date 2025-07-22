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
    public partial class TestModelApiBatchVM : BaseBatchVM<TestModel, TestModelApi_BatchEdit>
    {
        public TestModelApiBatchVM()
        {
            ListVM = new TestModelApiListVM();
            LinkedVM = new TestModelApi_BatchEdit();
        }

    }

	/// <summary>
    /// Class to define batch edit fields
    /// </summary>
    public class TestModelApi_BatchEdit : BaseVM
    {
        [Display(Name = "_Model._TestModel._Name")]
        public String Name { get; set; }

        protected override void InitVM()
        {
        }

    }

}
