using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using EAPPlatform.Model.EAP;


namespace EAPPlatform.ViewModel.EAP.ClientLogVMs
{
    public partial class ClientLogListVM : BasePagedListVM<ClientLog_View, ClientLogSearcher>
    {
        protected override List<GridAction> InitGridAction()
        {
            return new List<GridAction>
            {
                this.MakeStandardAction("ClientLog", GridActionStandardTypesEnum.Create, Localizer["Sys.Create"],"EAP", dialogWidth: 800),
                this.MakeStandardAction("ClientLog", GridActionStandardTypesEnum.Edit, Localizer["Sys.Edit"], "EAP", dialogWidth: 800),
                this.MakeStandardAction("ClientLog", GridActionStandardTypesEnum.Delete, Localizer["Sys.Delete"], "EAP", dialogWidth: 800),
                this.MakeStandardAction("ClientLog", GridActionStandardTypesEnum.Details, Localizer["Sys.Details"], "EAP", dialogWidth: 800),
                this.MakeStandardAction("ClientLog", GridActionStandardTypesEnum.BatchEdit, Localizer["Sys.BatchEdit"], "EAP", dialogWidth: 800),
                this.MakeStandardAction("ClientLog", GridActionStandardTypesEnum.BatchDelete, Localizer["Sys.BatchDelete"], "EAP", dialogWidth: 800),
                this.MakeStandardAction("ClientLog", GridActionStandardTypesEnum.Import, Localizer["Sys.Import"], "EAP", dialogWidth: 800),
                this.MakeStandardAction("ClientLog", GridActionStandardTypesEnum.ExportExcel, Localizer["Sys.Export"], "EAP"),
            };
        }


        protected override IEnumerable<IGridColumn<ClientLog_View>> InitGridHeader()
        {
            return new List<GridColumn<ClientLog_View>>{
                this.MakeGridHeader(x => x.DateTime).SetSort(true),
                this.MakeGridHeader(x => x.EQID).SetSort(true),
                this.MakeGridHeader(x => x.LogType).SetSort(true),
                this.MakeGridHeader(x => x.LogLevel).SetSort(true),
                this.MakeGridHeader(x => x.LogContent),
                this.MakeGridHeaderAction(width: 200)
            };
        }

        public override IOrderedQueryable<ClientLog_View> GetSearchQuery()
        {
            var query = DC.Set<ClientLog>()
                .CheckBetween(Searcher.DateTime?.GetStartTime(), Searcher.DateTime?.GetEndTime(), x => x.DateTime, includeMax: false)
                .CheckContain(Searcher.EQID, x=>x.EQID)
                .Select(x => new ClientLog_View
                {
				    ID = x.ID,
                    DateTime = x.DateTime,
                    EQID = x.EQID,
                    LogType = x.LogType,
                    LogLevel = x.LogLevel,
                    LogContent = x.LogContent,
                })
                .OrderBy(x => x.ID);
            return query;
        }

    }

    public class ClientLog_View : ClientLog{

    }
}
