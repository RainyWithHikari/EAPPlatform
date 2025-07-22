using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using EAPPlatform.Model;

namespace EAPPlatform.ViewModel._Admin.FrameworkUserVMs
{
    public partial class FrameworkUserListVM : BasePagedListVM<FrameworkUser_View, FrameworkUserSearcher>
    {
        
        protected override List<GridAction> InitGridAction()
        {
            return new List<GridAction>
            {
                this.MakeAction("FrameworkUser","Create",Localizer["Sys.Create"].Value,Localizer["Sys.Create"].Value,GridActionParameterTypesEnum.SingleIdWithNull,"_Admin",800).SetShowInRow(false).SetHideOnToolBar(false).SetIconCls("fa fa-plus"),
                this.MakeAction("FrameworkUser","Edit",Localizer["Sys.Edit"].Value,Localizer["Sys.Edit"].Value,GridActionParameterTypesEnum.SingleIdWithNull,"_Admin",800).SetShowInRow(true).SetHideOnToolBar(true).SetIconCls("fa fa-pencil-square"),
                this.MakeAction("FrameworkUser","Details",Localizer["Page.详情"].Value,Localizer["Page.详情"].Value,GridActionParameterTypesEnum.SingleIdWithNull,"_Admin",800).SetShowInRow(true).SetHideOnToolBar(true).SetIconCls("fa fa-info-circle"),
                this.MakeAction("FrameworkUser","Import",Localizer["Sys.Import"].Value,Localizer["Sys.Import"].Value,GridActionParameterTypesEnum.SingleIdWithNull,"_Admin",800).SetShowInRow(false).SetHideOnToolBar(false).SetIconCls("fa fa-tasks"),
                this.MakeStandardAction("FrameworkUser", GridActionStandardTypesEnum.SimpleDelete, Localizer["Sys.Delete"].Value, "_Admin", dialogWidth: 800).SetIconCls("fa fa-trash"),
                this.MakeStandardAction("FrameworkUser", GridActionStandardTypesEnum.SimpleBatchDelete, Localizer["Sys.BatchDelete"].Value, "_Admin", dialogWidth: 800).SetIconCls("fa fa-trash"),
                this.MakeAction("FrameworkUser","BatchEdit",Localizer["Sys.BatchEdit"].Value,Localizer["Sys.BatchEdit"].Value,GridActionParameterTypesEnum.MultiIds,"_Admin",800).SetShowInRow(false).SetHideOnToolBar(false).SetIconCls("fa fa-pencil-square"),
                this.MakeAction("FrameworkUser","FrameworkUserExportExcel",Localizer["Sys.Export"].Value,Localizer["Sys.Export"].Value,GridActionParameterTypesEnum.MultiIdWithNull,"_Admin").SetShowInRow(false).SetShowDialog(false).SetHideOnToolBar(false).SetIsExport(true).SetIconCls("fa fa-arrow-circle-down"),
                this.MakeAction("FrameworkUser","Password",Localizer["Login.ChangePassword"].Value,Localizer["Login.ChangePassword"].Value,GridActionParameterTypesEnum.SingleIdWithNull,"_Admin",800).SetShowInRow(true).SetHideOnToolBar(true).SetIconCls("fa fa-unlock-alt"),
            };
        }
 

        protected override IEnumerable<IGridColumn<FrameworkUser_View>> InitGridHeader()
        {
            return new List<GridColumn<FrameworkUser_View>>{
                
                this.MakeGridHeader(x => x.FrameworkUser_ITCode).SetTitle(Localizer["Sys.Account"].Value),
                this.MakeGridHeader(x => x.FrameworkUser_Name).SetTitle(Localizer["_Admin.Name"].Value),
                this.MakeGridHeader(x => x.FrameworkUser_Gender).SetTitle(Localizer["_Admin.Gender"].Value),
                this.MakeGridHeader(x => x.FrameworkUser_CellPhone).SetTitle(Localizer["_Admin.CellPhone"].Value),
                this.MakeGridHeader(x => x.FrameworkUser_Role).SetTitle(Localizer["_Admin.Role"].Value),
                this.MakeGridHeader(x => x.FrameworkUser_Group).SetTitle(Localizer["_Admin.Group"].Value),
                this.MakeGridHeader(x => x.FrameworkUser_IsValid).SetTitle(Localizer["_Admin.IsValid"].Value),
                this.MakeGridHeader(x => x.FrameworkUser_Photo).SetTitle(Localizer["_Admin.Photo"].Value).SetFormat(FrameworkUser_PhotoFormat),

                this.MakeGridHeaderAction(width: 240)
            };
        }

        
        private List<ColumnFormatInfo> FrameworkUser_PhotoFormat(FrameworkUser_View entity, object val)
        {
            return new List<ColumnFormatInfo>
            {
                ColumnFormatInfo.MakeDownloadButton(ButtonTypesEnum.Button,entity.FrameworkUser_Photo),
                ColumnFormatInfo.MakeViewButton(ButtonTypesEnum.Button,entity.FrameworkUser_Photo,640,480),
            };
        }


        public override IOrderedQueryable<FrameworkUser_View> GetSearchQuery()
        {
            var query = DC.Set<FrameworkUser>()
                
                .CheckContain(Searcher.ITCode, x=>x.ITCode)
                .CheckContain(Searcher.Name, x=>x.Name)
                .CheckEqual(Searcher.IsValid, x=>x.IsValid)
                .Select(x => new FrameworkUser_View
                {
				    ID = x.ID,
                    
                    FrameworkUser_ITCode = x.ITCode,
                    FrameworkUser_Name = x.Name,
                    FrameworkUser_Gender = x.Gender,
                    FrameworkUser_CellPhone = x.CellPhone,
                    FrameworkUser_Role =  DC.Set<FrameworkUserRole>().Where(y => y.UserCode == x.ITCode)
                        .Join(DC.Set<FrameworkRole>(), ur => ur.RoleCode, role => role.RoleCode, (ur, role) => role).Select(y0=>y0.RoleName).ToSepratedString(null,","),
                    FrameworkUser_Group =  DC.Set<FrameworkUserGroup>().Where(y => y.UserCode == x.ITCode)
                        .Join(DC.Set<FrameworkGroup>(), ug => ug.GroupCode, group => group.GroupCode, (ug, group) => group ).Select(y0=>y0.GroupName).ToSepratedString(null,","),
                    FrameworkUser_IsValid = x.IsValid,
                    FrameworkUser_Photo = x.PhotoId,
                })
                .OrderBy(x => x.ID);
            return query;
        }

    }
    public class FrameworkUser_View: FrameworkUser
    {
        
        public string FrameworkUser_ITCode { get; set; }
        public string FrameworkUser_Name { get; set; }
        public GenderEnum? FrameworkUser_Gender { get; set; }
        public string FrameworkUser_CellPhone { get; set; }
        public string FrameworkUser_Role { get; set; }
        public string FrameworkUser_Group { get; set; }
        public bool? FrameworkUser_IsValid { get; set; }
        public Guid? FrameworkUser_Photo { get; set; }

    }

}