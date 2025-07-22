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
    public partial class TestModelTemplateVM : BaseTemplateVM
    {
        [Display(Name = "_Model._TestModel._Name")]
        public ExcelPropety Name_Excel = ExcelPropety.CreateProperty<TestModel>(x => x.Name);

	    protected override void InitVM()
        {
        }

    }

    public class TestModelImportVM : BaseImportVM<TestModelTemplateVM, TestModel>
    {

    }

}
