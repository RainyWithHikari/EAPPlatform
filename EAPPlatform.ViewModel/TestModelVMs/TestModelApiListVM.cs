using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using EAPPlatform.Model.EAP;


namespace EAPPlatform.ViewModel.TestModelVMs
{
    public partial class TestModelApiListVM : BasePagedListVM<TestModelApi_View, TestModelApiSearcher>
    {

        protected override IEnumerable<IGridColumn<TestModelApi_View>> InitGridHeader()
        {
            return new List<GridColumn<TestModelApi_View>>{
                this.MakeGridHeader(x => x.Name),
                this.MakeGridHeaderAction(width: 200)
            };
        }

        public override IOrderedQueryable<TestModelApi_View> GetSearchQuery()
        {
            var query = DC.Set<TestModel>()
                .CheckContain(Searcher.Name, x=>x.Name)
                .Select(x => new TestModelApi_View
                {
				    ID = x.ID,
                    Name = x.Name,
                })
                .OrderBy(x => x.ID);
            return query;
        }

    }

    public class TestModelApi_View : TestModel{

    }
}
