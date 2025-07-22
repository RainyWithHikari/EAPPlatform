using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using EAPPlatform.Model.EAP;
using EAPPlatform.Model;

namespace EAPPlatform.ViewModel.EAP.EquipmentVMs
{
    public partial class EquipmentListVM : BasePagedListVM<Equipment_View, EquipmentSearcher>
    {
        
        protected override List<GridAction> InitGridAction()
        {
            return new List<GridAction>
            {
                this.MakeAction("Equipment","Create",Localizer["Sys.Create"].Value,Localizer["Sys.Create"].Value,GridActionParameterTypesEnum.SingleIdWithNull,"EAP",800).SetShowInRow(false).SetHideOnToolBar(false).SetIconCls("fa fa-plus"),
                this.MakeAction("Equipment","Edit",Localizer["Sys.Edit"].Value,Localizer["Sys.Edit"].Value,GridActionParameterTypesEnum.SingleIdWithNull,"EAP",800).SetShowInRow(true).SetHideOnToolBar(true).SetIconCls("fa fa-pencil-square"),
                this.MakeAction("Equipment","Details",Localizer["Page.详情"].Value,Localizer["Page.详情"].Value,GridActionParameterTypesEnum.SingleIdWithNull,"EAP",800).SetShowInRow(true).SetHideOnToolBar(true).SetIconCls("fa fa-info-circle"),
                this.MakeStandardAction("Equipment", GridActionStandardTypesEnum.SimpleDelete, Localizer["Sys.Delete"].Value, "EAP", dialogWidth: 800).SetIconCls("fa fa-trash").SetButtonClass("layui-btn-danger"),
                this.MakeStandardAction("Equipment", GridActionStandardTypesEnum.SimpleBatchDelete, Localizer["Sys.BatchDelete"].Value, "EAP", dialogWidth: 800).SetIconCls("fa fa-trash").SetButtonClass("layui-btn-danger"),
                this.MakeAction("Equipment","BatchEdit",Localizer["Sys.BatchEdit"].Value,Localizer["Sys.BatchEdit"].Value,GridActionParameterTypesEnum.MultiIds,"EAP",800).SetShowInRow(false).SetHideOnToolBar(false).SetIconCls("fa fa-pencil-square"),
                this.MakeAction("Equipment","Import",Localizer["Sys.Import"].Value,Localizer["Sys.Import"].Value,GridActionParameterTypesEnum.SingleIdWithNull,"EAP",800).SetShowInRow(false).SetHideOnToolBar(false).SetIconCls("fa fa-tasks"),
                this.MakeAction("Equipment","EquipmentExportExcel",Localizer["Sys.Export"].Value,Localizer["Sys.Export"].Value,GridActionParameterTypesEnum.MultiIdWithNull,"EAP").SetShowInRow(false).SetShowDialog(false).SetHideOnToolBar(false).SetIsExport(true).SetIconCls("fa fa-arrow-circle-down"),
                this.MakeAction("Equipment","EqpConfig",Localizer["Page.配置"].Value,Localizer["Page.设备配置"].Value,GridActionParameterTypesEnum.SingleIdWithNull,"EAP",800).SetShowInRow(true).SetShowDialog(true).SetHideOnToolBar(true).SetIconCls("fa fa-cog"),
            };
        }
 

        protected override IEnumerable<IGridColumn<Equipment_View>> InitGridHeader()
        {
            return new List<GridColumn<Equipment_View>>{
                
                this.MakeGridHeader(x => x.Equipment_EQID).SetTitle(Localizer["Page.设备ID"].Value).SetSort(true),
                this.MakeGridHeader(x => x.Equipment_EquipmentType).SetTitle(Localizer["Page.设备类型"].Value).SetSort(true),
                this.MakeGridHeader(x => x.Equipment_Name).SetTitle(Localizer["Page.设备名"].Value).SetSort(true),
                this.MakeGridHeader(x => x.Equipment_CreateTime).SetTitle(Localizer["_Admin.CreateTime"].Value).SetSort(true),
                this.MakeGridHeader(x => x.Equipment_UpdateTime).SetTitle(Localizer["_Admin.UpdateTime"].Value).SetSort(true),
                this.MakeGridHeader(x => x.Equipment_CreateBy).SetTitle(Localizer["_Admin.CreateBy"].Value).SetSort(true),
                this.MakeGridHeader(x => x.Equipment_UpdateBy).SetTitle(Localizer["_Admin.UpdateBy"].Value).SetSort(true),
                this.MakeGridHeader(x => x.Equipment_IsValid).SetTitle(Localizer["_Admin.IsValid"].Value).SetSort(true),

                this.MakeGridHeaderAction(width: 240)
            };
        }

        

        public override IOrderedQueryable<Equipment_View> GetSearchQuery()
        {
            var query = DC.Set<Equipment>()
                
                .CheckContain(Searcher.EQID, x=>x.EQID)
                .CheckEqual(Searcher.EquipmentTypeId, x=>x.EquipmentTypeId)
                .CheckContain(Searcher.Name, x=>x.Name)
                .Select(x => new Equipment_View
                {
				    ID = x.ID,
                    
                    Equipment_EQID = x.EQID,
                    Equipment_EquipmentType = x.EquipmentType.Name,
                    Equipment_Name = x.Name,
                    Equipment_CreateTime = x.CreateTime,
                    Equipment_UpdateTime = x.UpdateTime,
                    Equipment_CreateBy = x.CreateBy,
                    Equipment_UpdateBy = x.UpdateBy,
                    Equipment_IsValid = x.IsValid,
                })
                .OrderBy(x => x.ID);
            return query;
        }

    }
    public class Equipment_View: Equipment
    {
        
        public string Equipment_EQID { get; set; }
        public string Equipment_EquipmentType { get; set; }
        public string Equipment_Name { get; set; }
        public DateTime? Equipment_CreateTime { get; set; }
        public DateTime? Equipment_UpdateTime { get; set; }
        public string Equipment_CreateBy { get; set; }
        public string Equipment_UpdateBy { get; set; }
        public bool Equipment_IsValid { get; set; }

    }

}