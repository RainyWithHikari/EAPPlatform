using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using EAPPlatform.Model.EAP;
using EAPPlatform.Model;

namespace EAPPlatform.ViewModel.EAP.EquipmentTypeVMs
{
    public partial class EquipmentTypeListVM : BasePagedListVM<EquipmentType_View, EquipmentTypeSearcher>
    {
        
        protected override List<GridAction> InitGridAction()
        {
            return new List<GridAction>
            {
                this.MakeAction("EquipmentType","Create",Localizer["Sys.Create"].Value,Localizer["Sys.Create"].Value,GridActionParameterTypesEnum.SingleIdWithNull,"EAP",800).SetShowInRow(false).SetHideOnToolBar(false).SetIconCls("fa fa-plus"),
                this.MakeAction("EquipmentType","Edit",Localizer["Sys.Edit"].Value,Localizer["Sys.Edit"].Value,GridActionParameterTypesEnum.SingleIdWithNull,"EAP",800).SetShowInRow(true).SetHideOnToolBar(true).SetIconCls("fa fa-pencil-square"),
                //this.MakeAction("EquipmentType","Details",Localizer["Page.详情"].Value,Localizer["Page.详情"].Value,GridActionParameterTypesEnum.SingleIdWithNull,"EAP",800).SetShowInRow(true).SetHideOnToolBar(true).SetIconCls("fa fa-info-circle"),
                this.MakeStandardAction("EquipmentType", GridActionStandardTypesEnum.SimpleDelete, Localizer["Sys.Delete"].Value, "EAP", dialogWidth: 800).SetIconCls("fa fa-trash").SetButtonClass("layui-btn-danger"),
                this.MakeStandardAction("EquipmentType", GridActionStandardTypesEnum.SimpleBatchDelete, Localizer["Sys.BatchDelete"].Value, "EAP", dialogWidth: 800).SetIconCls("fa fa-trash").SetButtonClass("layui-btn-danger"),
                this.MakeAction("EquipmentType","BatchEdit",Localizer["Sys.BatchEdit"].Value,Localizer["Sys.BatchEdit"].Value,GridActionParameterTypesEnum.MultiIds,"EAP",800).SetShowInRow(false).SetHideOnToolBar(false).SetIconCls("fa fa-pencil-square"),
                this.MakeAction("EquipmentType","Import",Localizer["Sys.Import"].Value,Localizer["Sys.Import"].Value,GridActionParameterTypesEnum.SingleIdWithNull,"EAP",800).SetShowInRow(false).SetHideOnToolBar(false).SetIconCls("fa fa-tasks"),
                this.MakeAction("EquipmentType","EquipmentTypeExportExcel",Localizer["Sys.Export"].Value,Localizer["Sys.Export"].Value,GridActionParameterTypesEnum.MultiIdWithNull,"EAP").SetShowInRow(false).SetShowDialog(false).SetHideOnToolBar(false).SetIsExport(true).SetIconCls("fa fa-arrow-circle-down"),
                this.MakeAction("EquipmentTypeConfiguration","EqpTypeConfig",Localizer["Page.配置"].Value,Localizer["Page.设备类型配置"].Value,GridActionParameterTypesEnum.SingleIdWithNull,"EAP",800).SetShowInRow(true).SetHideOnToolBar(true).SetIconCls("fa fa-cogs"),
                this.MakeAction("EquipmentType","Privilege","权限","权限",GridActionParameterTypesEnum.SingleIdWithNull,"EAP",800).SetShowInRow(true).SetShowDialog(true).SetHideOnToolBar(true).SetIconCls("fa fa-cog"),

            };
        }
 

        protected override IEnumerable<IGridColumn<EquipmentType_View>> InitGridHeader()
        {
            return new List<GridColumn<EquipmentType_View>>{
                
                this.MakeGridHeader(x => x.EquipmentType_Name).SetTitle(Localizer["Page.设备EAP类型"].Value),
                this.MakeGridHeader(x => x.EquipmentType_CreateTime).SetTitle(Localizer["_Admin.CreateTime"].Value),
                this.MakeGridHeader(x => x.EquipmentType_UpdateTime).SetTitle(Localizer["_Admin.UpdateTime"].Value),
                this.MakeGridHeader(x => x.EquipmentType_CreateBy).SetTitle(Localizer["_Admin.CreateBy"].Value),
                this.MakeGridHeader(x => x.EquipmentType_UpdateBy).SetTitle(Localizer["_Admin.UpdateBy"].Value),
                this.MakeGridHeader(x => x.EquipmentType_IsValid).SetTitle(Localizer["_Admin.IsValid"].Value),

                this.MakeGridHeaderAction(width: 240)
            };
        }

        

        public override IOrderedQueryable<EquipmentType_View> GetSearchQuery()
        {
            var query = DC.Set<EquipmentType>()
                
                .CheckContain(Searcher.Name, x=>x.Name)
                .Select(x => new EquipmentType_View
                {
				    ID = x.ID,
                    
                    EquipmentType_Name = x.Name,
                    EquipmentType_CreateTime = x.CreateTime,
                    EquipmentType_UpdateTime = x.UpdateTime,
                    EquipmentType_CreateBy = x.CreateBy,
                    EquipmentType_UpdateBy = x.UpdateBy,
                    EquipmentType_IsValid = x.IsValid,
                })
                .OrderBy(x => x.ID);
            return query;
        }

    }
    public class EquipmentType_View: EquipmentType
    {
        
        public string EquipmentType_Name { get; set; }
        public DateTime? EquipmentType_CreateTime { get; set; }
        public DateTime? EquipmentType_UpdateTime { get; set; }
        public string EquipmentType_CreateBy { get; set; }
        public string EquipmentType_UpdateBy { get; set; }
        public bool EquipmentType_IsValid { get; set; }

    }

}