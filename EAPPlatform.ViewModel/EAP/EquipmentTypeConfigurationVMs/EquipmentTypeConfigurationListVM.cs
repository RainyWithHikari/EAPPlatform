using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using EAPPlatform.Model.EAP;
using EAPPlatform.Model;

namespace EAPPlatform.ViewModel.EAP.EquipmentTypeConfigurationVMs
{
    public partial class EquipmentTypeConfigurationListVM : BasePagedListVM<EquipmentTypeConfiguration_View, EquipmentTypeConfigurationSearcher>
    {
        
        protected override List<GridAction> InitGridAction()
        {
            return new List<GridAction>
            {
                this.MakeAction("EquipmentTypeConfiguration","Create",Localizer["Sys.Create"].Value,Localizer["Sys.Create"].Value,GridActionParameterTypesEnum.SingleIdWithNull,"EAP",800).SetShowInRow(false).SetHideOnToolBar(false).SetIconCls("fa fa-plus"),
                this.MakeAction("EquipmentTypeConfiguration","Edit",Localizer["Sys.Edit"].Value,Localizer["Sys.Edit"].Value,GridActionParameterTypesEnum.SingleIdWithNull,"EAP",800).SetShowInRow(true).SetHideOnToolBar(true).SetIconCls("fa fa-pencil-square"),
                this.MakeAction("EquipmentTypeConfiguration","Details",Localizer["Page.详情"].Value,Localizer["Page.详情"].Value,GridActionParameterTypesEnum.SingleIdWithNull,"EAP",800).SetShowInRow(true).SetHideOnToolBar(true).SetIconCls("fa fa-info-circle"),
                this.MakeStandardAction("EquipmentTypeConfiguration", GridActionStandardTypesEnum.SimpleDelete, Localizer["Sys.Delete"].Value, "EAP", dialogWidth: 800).SetIconCls("fa fa-trash").SetButtonClass("layui-btn-danger"),
                this.MakeStandardAction("EquipmentTypeConfiguration", GridActionStandardTypesEnum.SimpleBatchDelete, Localizer["Sys.BatchDelete"].Value, "EAP", dialogWidth: 800).SetIconCls("fa fa-trash").SetButtonClass("layui-btn-danger"),
                this.MakeAction("EquipmentTypeConfiguration","BatchEdit",Localizer["Sys.BatchEdit"].Value,Localizer["Sys.BatchEdit"].Value,GridActionParameterTypesEnum.MultiIds,"EAP",800).SetShowInRow(false).SetHideOnToolBar(false).SetIconCls("fa fa-pencil-square"),
                this.MakeAction("EquipmentTypeConfiguration","Import",Localizer["Sys.Import"].Value,Localizer["Sys.Import"].Value,GridActionParameterTypesEnum.SingleIdWithNull,"EAP",800).SetShowInRow(false).SetHideOnToolBar(false).SetIconCls("fa fa-tasks"),
                this.MakeAction("EquipmentTypeConfiguration","EquipmentTypeConfigurationExportExcel",Localizer["Sys.Export"].Value,Localizer["Sys.Export"].Value,GridActionParameterTypesEnum.MultiIdWithNull,"EAP").SetShowInRow(false).SetShowDialog(false).SetHideOnToolBar(false).SetIsExport(true).SetIconCls("fa fa-arrow-circle-down"),

            };
        }
 

        protected override IEnumerable<IGridColumn<EquipmentTypeConfiguration_View>> InitGridHeader()
        {
            return new List<GridColumn<EquipmentTypeConfiguration_View>>{
                
                this.MakeGridHeader(x => x.EquipmentTypeConfiguration_TypeName).SetTitle(Localizer["Page.设备EAP类型"].Value).SetSort(true),
                this.MakeGridHeader(x => x.EquipmentTypeConfiguration_ConfigurationItem).SetTitle(Localizer["Page.配置项目"].Value).SetSort(true),
                this.MakeGridHeader(x => x.EquipmentTypeConfiguration_ConfigurationValue).SetTitle(Localizer["Page.配置值"].Value).SetSort(true),
                this.MakeGridHeader(x => x.EquipmentTypeConfiguration_ConfigurationName).SetTitle(Localizer["Page.配置项名称"].Value).SetSort(true),
                this.MakeGridHeader(x => x.EquipmentTypeConfiguration_IsValid).SetTitle(Localizer["_Admin.IsValid"].Value).SetSort(true),
                this.MakeGridHeader(x => x.EquipmentTypeConfiguration_DisplayOrder).SetTitle(Localizer["_Admin.DisplayOrder"].Value).SetSort(true),
                this.MakeGridHeader(x => x.EquipmentTypeConfiguration_CreateTime).SetTitle(Localizer["_Admin.CreateTime"].Value).SetSort(true),
                this.MakeGridHeader(x => x.EquipmentTypeConfiguration_UpdateTime).SetTitle(Localizer["_Admin.UpdateTime"].Value).SetSort(true),
                this.MakeGridHeader(x => x.EquipmentTypeConfiguration_CreateBy).SetTitle(Localizer["_Admin.CreateBy"].Value).SetSort(true),
                this.MakeGridHeader(x => x.EquipmentTypeConfiguration_UpdateBy).SetTitle(Localizer["_Admin.UpdateBy"].Value).SetSort(true),

                this.MakeGridHeaderAction(width: 200)
            };
        }

        

        public override IOrderedQueryable<EquipmentTypeConfiguration_View> GetSearchQuery()
        {
            var query = DC.Set<EquipmentTypeConfiguration>()
                
                .CheckEqual(Searcher.TypeNameId, x=>x.TypeNameId)
                .CheckContain(Searcher.ConfigurationItem, x=>x.ConfigurationItem)
                .CheckContain(Searcher.ConfigurationValue, x=>x.ConfigurationValue)
                .Select(x => new EquipmentTypeConfiguration_View
                {
				    ID = x.ID,
                    
                    EquipmentTypeConfiguration_TypeName = x.TypeName.Name,
                    EquipmentTypeConfiguration_ConfigurationItem = x.ConfigurationItem,
                    EquipmentTypeConfiguration_ConfigurationValue = x.ConfigurationValue,
                    EquipmentTypeConfiguration_ConfigurationName = x.ConfigurationName,
                    EquipmentTypeConfiguration_IsValid = x.IsValid,
                    EquipmentTypeConfiguration_DisplayOrder = x.DisplayOrder,
                    EquipmentTypeConfiguration_CreateTime = x.CreateTime,
                    EquipmentTypeConfiguration_UpdateTime = x.UpdateTime,
                    EquipmentTypeConfiguration_CreateBy = x.CreateBy,
                    EquipmentTypeConfiguration_UpdateBy = x.UpdateBy,
                })
                .OrderBy(x => x.ID);
            return query;
        }

    }
    public class EquipmentTypeConfiguration_View: EquipmentTypeConfiguration
    {
        
        public string EquipmentTypeConfiguration_TypeName { get; set; }
        public string EquipmentTypeConfiguration_ConfigurationItem { get; set; }
        public string EquipmentTypeConfiguration_ConfigurationValue { get; set; }
        public string EquipmentTypeConfiguration_ConfigurationName { get; set; }
        public bool EquipmentTypeConfiguration_IsValid { get; set; }
        public int? EquipmentTypeConfiguration_DisplayOrder { get; set; }
        public DateTime? EquipmentTypeConfiguration_CreateTime { get; set; }
        public DateTime? EquipmentTypeConfiguration_UpdateTime { get; set; }
        public string EquipmentTypeConfiguration_CreateBy { get; set; }
        public string EquipmentTypeConfiguration_UpdateBy { get; set; }

    }

}