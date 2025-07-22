using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using EAPPlatform.Model.EAP;


namespace EAPPlatform.ViewModel.EAP.TestModelVMs
{
    public partial class TestModelListVM : BasePagedListVM<TestModel_View, TestModelSearcher>
    {
        protected override List<GridAction> InitGridAction()
        {
            return new List<GridAction>
            {
                this.MakeStandardAction("TestModel", GridActionStandardTypesEnum.Create, Localizer["Sys.Create"],"EAP", dialogWidth: 800),
                this.MakeStandardAction("TestModel", GridActionStandardTypesEnum.Edit, Localizer["Sys.Edit"], "EAP", dialogWidth: 800),
                this.MakeStandardAction("TestModel", GridActionStandardTypesEnum.Delete, Localizer["Sys.Delete"], "EAP", dialogWidth: 800),
                this.MakeStandardAction("TestModel", GridActionStandardTypesEnum.Details, Localizer["Sys.Details"], "EAP", dialogWidth: 800),
                this.MakeStandardAction("TestModel", GridActionStandardTypesEnum.BatchEdit, Localizer["Sys.BatchEdit"], "EAP", dialogWidth: 800),
                this.MakeStandardAction("TestModel", GridActionStandardTypesEnum.BatchDelete, Localizer["Sys.BatchDelete"], "EAP", dialogWidth: 800),
                this.MakeStandardAction("TestModel", GridActionStandardTypesEnum.Import, Localizer["Sys.Import"], "EAP", dialogWidth: 800),
                this.MakeStandardAction("TestModel", GridActionStandardTypesEnum.ExportExcel, Localizer["Sys.Export"], "EAP"),
            };
        }


        protected override IEnumerable<IGridColumn<TestModel_View>> InitGridHeader()
        {
            return new List<GridColumn<TestModel_View>>{
                this.MakeGridHeader(x => x.Name),
                this.MakeGridHeaderAction(width: 200)
            };
        }

        public override IOrderedQueryable<TestModel_View> GetSearchQuery()
        {
            var query = DC.Set<TestModel>()
                .CheckContain(Searcher.Name, x=>x.Name)
                .Select(x => new TestModel_View
                {
				    ID = x.ID,
                    Name = x.Name,
                })
                .OrderBy(x => x.ID);
            return query;
        }

    }

    public class TestModel_View : TestModel{

    }
}
