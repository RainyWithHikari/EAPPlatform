using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using EAPPlatform.Model.EAP;


namespace EAPPlatform.ViewModel.EAP.TestModelVMs
{
    public partial class TestModelBatchVM : BaseBatchVM<TestModel, TestModel_BatchEdit>
    {
        public TestModelBatchVM()
        {
            ListVM = new TestModelListVM();
            LinkedVM = new TestModel_BatchEdit();
        }

    }

	/// <summary>
    /// Class to define batch edit fields
    /// </summary>
    public class TestModel_BatchEdit : BaseVM
    {
        [Display(Name = "_Model._TestModel._Name")]
        public String Name { get; set; }

        protected override void InitVM()
        {
        }

    }

}
