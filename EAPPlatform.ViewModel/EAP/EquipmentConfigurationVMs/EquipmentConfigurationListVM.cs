using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using EAPPlatform.Model.EAP;
using EAPPlatform.Model;

namespace EAPPlatform.ViewModel.EAP.EquipmentConfigurationVMs
{
    public partial class EquipmentConfigurationListVM : BasePagedListVM<EquipmentConfiguration_View, EquipmentConfigurationSearcher>
    {
        
        protected override List<GridAction> InitGridAction()
        {
            return new List<GridAction>
            {
                this.MakeAction("EquipmentConfiguration","Create",Localizer["Sys.Create"].Value,Localizer["Sys.Create"].Value,GridActionParameterTypesEnum.SingleIdWithNull,"EAP",800).SetShowInRow(false).SetHideOnToolBar(false).SetIconCls("fa fa-plus"),
                this.MakeAction("EquipmentConfiguration","Edit",Localizer["Sys.Edit"].Value,Localizer["Sys.Edit"].Value,GridActionParameterTypesEnum.SingleIdWithNull,"EAP",800).SetShowInRow(true).SetHideOnToolBar(true).SetIconCls("fa fa-pencil-square"),
                this.MakeStandardAction("EquipmentConfiguration", GridActionStandardTypesEnum.SimpleDelete, Localizer["Sys.Delete"].Value, "EAP", dialogWidth: 800).SetIconCls("fa fa-trash").SetButtonClass("layui-btn-danger"),
                this.MakeStandardAction("EquipmentConfiguration", GridActionStandardTypesEnum.SimpleBatchDelete, Localizer["Sys.BatchDelete"].Value, "EAP", dialogWidth: 800).SetIconCls("fa fa-trash").SetButtonClass("layui-btn-danger"),
                this.MakeAction("EquipmentConfiguration","Import",Localizer["Sys.Import"].Value,Localizer["Sys.Import"].Value,GridActionParameterTypesEnum.SingleIdWithNull,"EAP",800).SetShowInRow(false).SetHideOnToolBar(false).SetIconCls("fa fa-tasks"),
                this.MakeAction("EquipmentConfiguration","ExportExcel",Localizer["Sys.Export"].Value,Localizer["Sys.Export"].Value,GridActionParameterTypesEnum.MultiIds,"EAP",800).SetShowInRow(false).SetHideOnToolBar(false).SetIconCls("fa fa-arrow-circle-down"),
            };
        }
 

        protected override IEnumerable<IGridColumn<EquipmentConfiguration_View>> InitGridHeader()
        {
            return new List<GridColumn<EquipmentConfiguration_View>>{
                
                this.MakeGridHeader(x => x.EquipmentConfiguration_EQID).SetTitle(Localizer["Page.EQID"].Value).SetSort(true),
                this.MakeGridHeader(x => x.EquipmentConfiguration_ConfigurationItem).SetTitle(Localizer["Page.配置项目"].Value).SetSort(true),
                this.MakeGridHeader(x => x.EquipmentConfiguration_ConfigurationName).SetTitle(Localizer["Page.配置项名称"].Value).SetSort(true),
                this.MakeGridHeader(x => x.EquipmentConfiguration_ConfigurationValue).SetTitle(Localizer["Page.配置值"].Value).SetSort(true),
                this.MakeGridHeader(x => x.EquipmentConfiguration_IsValid).SetTitle(Localizer["_Admin.IsValid"].Value).SetSort(true),
                this.MakeGridHeader(x => x.EquipmentConfiguration_DisplayOrder).SetTitle(Localizer["_Admin.DisplayOrder"].Value).SetSort(true),
                this.MakeGridHeader(x => x.EquipmentConfiguration_CreateTime).SetTitle(Localizer["_Admin.CreateTime"].Value).SetSort(true),
                this.MakeGridHeader(x => x.EquipmentConfiguration_UpdateTime).SetTitle(Localizer["_Admin.UpdateTime"].Value).SetSort(true),
                this.MakeGridHeader(x => x.EquipmentConfiguration_CreateBy).SetTitle(Localizer["_Admin.CreateBy"].Value).SetSort(true),
                this.MakeGridHeader(x => x.EquipmentConfiguration_UpdateBy).SetTitle(Localizer["_Admin.UpdateBy"].Value).SetSort(true),

                this.MakeGridHeaderAction(width: 200)
            };
        }

        

        public override IOrderedQueryable<EquipmentConfiguration_View> GetSearchQuery()
        {
            var query = DC.Set<EquipmentConfiguration>()
                
                .CheckEqual(Searcher.EQIDId, x=>x.EQIDId)
                .CheckContain(Searcher.ConfigurationItem, x=>x.ConfigurationItem)
                .CheckContain(Searcher.ConfigurationValue, x=>x.ConfigurationValue)
                .Select(x => new EquipmentConfiguration_View
                {
				    ID = x.ID,
                    
                    EquipmentConfiguration_EQID = x.EQID.EQID,
                    EquipmentConfiguration_ConfigurationItem = x.ConfigurationItem,
                    EquipmentConfiguration_ConfigurationName = x.ConfigurationName,
                    EquipmentConfiguration_ConfigurationValue = x.ConfigurationValue,
                    EquipmentConfiguration_IsValid = x.IsValid,
                    EquipmentConfiguration_DisplayOrder = x.DisplayOrder,
                    EquipmentConfiguration_CreateTime = x.CreateTime,
                    EquipmentConfiguration_UpdateTime = x.UpdateTime,
                    EquipmentConfiguration_CreateBy = x.CreateBy,
                    EquipmentConfiguration_UpdateBy = x.UpdateBy,
                })
                .OrderBy(x => x.ID);
            return query;
        }

    }
    public class EquipmentConfiguration_View: EquipmentConfiguration
    {
        
        public string EquipmentConfiguration_EQID { get; set; }
        public string EquipmentConfiguration_ConfigurationItem { get; set; }
        public string EquipmentConfiguration_ConfigurationName { get; set; }
        public string EquipmentConfiguration_ConfigurationValue { get; set; }
        public bool EquipmentConfiguration_IsValid { get; set; }
        public int? EquipmentConfiguration_DisplayOrder { get; set; }
        public DateTime? EquipmentConfiguration_CreateTime { get; set; }
        public DateTime? EquipmentConfiguration_UpdateTime { get; set; }
        public string EquipmentConfiguration_CreateBy { get; set; }
        public string EquipmentConfiguration_UpdateBy { get; set; }

    }

}