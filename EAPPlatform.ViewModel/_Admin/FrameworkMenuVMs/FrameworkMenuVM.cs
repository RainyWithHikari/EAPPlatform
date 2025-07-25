// WTM默认页面 Wtm buidin page
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Core.Support.Json;
using WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkRoleVMs;

namespace WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkMenuVMs
{
    public class FrameworkMenuVM : BaseCRUDVM<FrameworkMenu>
    {
        [Display(Name = "_Admin.IconFont")]
        public string IconFont { get; set; }

        [Display(Name = "_Admin.Icon")]
        public string IconFontItem { get; set; }

        [JsonIgnore]
        public List<ComboSelectListItem> AllParents { get; set; }
        [JsonIgnore]
        public List<ComboSelectListItem> AllModules { get; set; }
        [JsonIgnore]
        public List<ComboSelectListItem> AllActions { get; set; }

        [Display(Name = "_Admin.Action")]
        public List<string> SelectedActionIDs { get; set; }

        [Display(Name = "_Admin.Module")]
        public string SelectedModule { get; set; }

        [Display(Name = "_Admin.AllowedRole")]
        public List<string> SelectedRolesIds { get; set; }

        [JsonIgnore]
        public FrameworkRoleListVM RoleListVM { get; set; }

        public List<ComboSelectListItem> IconSelectItems { get; set; }

        public FrameworkMenuVM()
        {
            RoleListVM = new FrameworkRoleListVM();
            AllActions = new List<ComboSelectListItem>();
            AllModules = new List<ComboSelectListItem>();

            SelectedRolesIds = new List<string>();
        }

        protected override void InitVM()
        {
            if (!string.IsNullOrEmpty(Entity.Icon))
            {
                var res = Entity.Icon.Split(' ');
                IconFont = res[0];
                IconFontItem = res[1];
            }

            SelectedRolesIds.AddRange(DC.Set<FunctionPrivilege>().Where(x => x.MenuItemId == Entity.ID && x.RoleCode != null && x.Allowed == true).Select(x => x.RoleCode).ToList());

            var data = DC.Set<FrameworkMenu>().AsNoTracking().ToList();
            var topMenu = data.Where(x => x.ParentId == null).ToList().FlatTree(x => x.DisplayOrder);
            var pids = Entity.GetAllChildrenIDs(DC);
            AllParents = data.Where(x => x.ID != Entity.ID && !pids.Contains(x.ID) && x.FolderOnly == true).ToList().ToListItems(y => y.PageName, x => x.ID);

            foreach (var p in AllParents)
            {
                if (p.Text.StartsWith("MenuKey."))
                {
                    p.Text = Localizer[p.Text];
                }
            }

            var modules = Wtm.GlobaInfo.AllModule;
            var m = Utils.ResetModule(modules);
            m = m.GroupBy(x => new { x.Area?.AreaName, x.IsApi }).SelectMany(x => x).ToList();
            string area = "";
            bool? isapi = null;
            for(int i = 0; i < m.Count; i++)
            {
                if(area != m[i].Area?.AreaName || isapi != m[i].IsApi)
                {
                    area = m[i].Area?.AreaName;
                    isapi = m[i].IsApi;
                    var mm = "-----" + (m[i].Area?.AreaName ?? "Default") +(m[i].IsApi == true?"(api)":"")+ "-----";
                    m.Insert(i, new SimpleModule { ModuleName = mm, NameSpace = "", ClassName = "" });
                    i++;
                }
            }
            AllModules = m.ToListItems(y => y.ModuleName, y => y.FullName);
            foreach (var item in AllModules)
            {
                if(item.Value.ToString() == ",")
                {
                    item.Disabled = true;
                }
            }
            if (string.IsNullOrEmpty(SelectedModule) == false || (string.IsNullOrEmpty(Entity.Url) == false && Entity.IsInside == true))
            {
                if (string.IsNullOrEmpty(SelectedModule))
                {
                    SelectedModule = m.Where(x => (x.FullName == Entity.ClassName)).FirstOrDefault()?.FullName;
                }
                var mm = m.Where(x => x.FullName == SelectedModule).SelectMany(x => x.Actions).Where(x => x.MethodName != "Index" && x.IgnorePrivillege == false).ToList();
                AllActions = mm.ToListItems(y => y.ActionName, y => y.Url);
                if (SelectedActionIDs == null)
                {
                    SelectedActionIDs = DC.Set<FrameworkMenu>().Where(x => AllActions.Select(y => y.Value).Contains(x.Url) && x.IsInside == true && x.FolderOnly == false).Select(x => x.Url).ToList();
                }
            }
        }



        public override void Validate()
        {
            if (Entity.IsInside == true && Entity.FolderOnly == false)
            {
                if (string.IsNullOrEmpty(SelectedModule) == true)
                {
                    MSD.AddModelError("SelectedModule", Localizer["Validate.{0}required", Localizer["_Admin.Module"]]);
                }
                var modules = Wtm.GlobaInfo.AllModule;
                var test = DC.Set<FrameworkMenu>().Where(x => x.ClassName == this.SelectedModule && (string.IsNullOrEmpty(x.MethodName) || x.MethodName == "Index") && x.ID != Entity.ID).FirstOrDefault();
                if (test != null)
                {
                    MSD.AddModelError(" error", Localizer["_Admin.ModuleHasSet"]);
                }

            }
            base.Validate();
        }

        public override void DoEdit(bool updateAllFields = false)
        {
            Entity.Icon = $"{IconFont} {IconFontItem}";
            FC.Add("Entity.Icon", " ");
            if (Entity.IsInside == false)
            {
                if (Entity.Url != null && Entity.Url != "" && Entity.Url.StartsWith("/") == false)
                {
                    if (Entity.Url.ToLower().StartsWith("http://") == false && Entity.Url.ToLower().StartsWith("https://") == false)
                    {
                        Entity.Url = "http://" + Entity.Url;
                    }
                }
                if(Entity.Url != null)
                {
                    Entity.Url = Entity.Url.TrimEnd('/');
                }
            }
            else
            {
                if (string.IsNullOrEmpty(SelectedModule) == false && Entity.FolderOnly == false)
                {
                    var modules = Wtm.GlobaInfo.AllModule;
                    var m = Utils.ResetModule(modules, false);
                    var actionPage = m.Where(x => x.FullName == this.SelectedModule)
                      .SelectMany(x => x.Actions).Where(x => x.MethodName == "Index" || x.ActionDes?.IsPage == true)
                      .FirstOrDefault();
                    if (actionPage == null && Entity.ShowOnMenu == true)
                    {
                        MSD.AddModelError("Entity.ModuleId", Localizer["_Admin.NoIndexInModule"]);
                        return;
                    }
                    List<SimpleAction> otherActions = null;
                 
                    var mainModule = m.Where(x => x.FullName == this.SelectedModule).FirstOrDefault();
                    if (actionPage == null)
                    {
                        actionPage = new SimpleAction
                        {
                            Module = mainModule,
                            Url = "/" + mainModule.ClassName,
                            ActionName = mainModule.ModuleName
                        };
                    }
                    var mainAction = actionPage;

                    Entity.Url = mainAction.Url;
                    Entity.ModuleName = mainModule.ModuleName;
                    Entity.ActionName = mainAction.ActionDes?.Description ?? mainAction.ActionName;
                    Entity.ClassName = mainModule.FullName;
                    Entity.MethodName = null;

                    otherActions = m.Where(x => x.FullName == this.SelectedModule)
                                            .SelectMany(x => x.Actions)
                                            .Where(x => x.MethodName != mainAction.MethodName)
                                            .ToList();
                    var actionsInDB = DC.Set<FrameworkMenu>().AsNoTracking().Where(x => x.ParentId == Entity.ID).ToList();
                    int order = 1;
                    Entity.Children = new List<FrameworkMenu>();
                    foreach (var action in otherActions)
                    {
                        if (SelectedActionIDs != null && SelectedActionIDs.Contains(action.Url))
                        {
                            Guid aid = action.ID;
                            var adb = actionsInDB.Where(x => x.Url.ToLower() == action.Url.ToLower()).FirstOrDefault();
                            if (adb != null)
                            {
                                aid = adb.ID;
                            }
                            var menu = new FrameworkMenu();
                            menu.FolderOnly = false;
                            menu.IsPublic = Entity.IsPublic;
                            menu.Parent = Entity;
                            menu.ShowOnMenu = false;
                            menu.DisplayOrder = order++;
                            menu.Privileges = new List<FunctionPrivilege>();
                            menu.IsInside = true;
                            menu.Domain = Entity.Domain;
                            menu.PageName = action.ActionDes?.Description ?? action.ActionName;
                            menu.ModuleName = mainModule.ModuleName;
                            menu.ActionName = action.ActionDes?.Description ?? action.ActionName;
                            menu.ClassName = mainModule.FullName;
                            menu.MethodName = action.MethodName;
                            menu.Url = action.Url;
                            menu.ID = aid;
                            Entity.Children.Add(menu);
                        }
                    }
                }
                else
                {
                   
                    //Entity.Children = new List<FrameworkMenu>();
                    Entity.ModuleName = Entity.PageName;
                    Entity.Url = null;
                }
            }
            if (Entity.FolderOnly == false)
            {
                if (FC.ContainsKey("Entity.Children") == false)
                {
                    FC.Add("Entity.Children", 0);
                    FC.Add("Entity.Children[0].IsPublic", 0);
                    FC.Add("Entity.Children[0].PageName", 0);
                    FC.Add("Entity.Children[0].ModuleName", 0);
                    FC.Add("Entity.Children[0].ActionName", 0);
                    FC.Add("Entity.Children[0].ClassName", 0);
                    FC.Add("Entity.Children[0].MethodName", 0);
                    FC.Add("Entity.Children[0].Url", 0);
                }
            }
            FC.Add("Entity.ModuleName", 0);
            base.DoEdit();
            List<Guid> guids = new List<Guid>();
            guids.Add(Entity.ID);
            if (Entity.Children != null)
            {
                guids.AddRange(Entity.Children?.Select(x => x.ID).ToList());
            }
            AddPrivilege(guids);
        }

        public override void DoAdd()
        {
            Entity.Icon = $"{IconFont} {IconFontItem}";
            if (Entity.IsInside == false)
            {
                if (Entity.Url != null && Entity.Url != "" && Entity.Url.StartsWith("/") == false)
                {
                    if (Entity.Url.ToLower().StartsWith("http://") == false && Entity.Url.ToLower().StartsWith("https://") == false)
                    {
                        Entity.Url = "http://" + Entity.Url;
                    }
                }
                if(Entity.Url != null)
                {
                    Entity.Url = Entity.Url.TrimEnd('/');
                }
            }
            else
            {

                if (string.IsNullOrEmpty(SelectedModule) == false && Entity.FolderOnly == false)
                {
                    var modules = Wtm.GlobaInfo.AllModule;
                    
                    var m = Utils.ResetModule(modules, false);
                    var actionPage = m.Where(x => x.FullName == this.SelectedModule)
                       .SelectMany(x => x.Actions).Where(x => x.MethodName == "Index" || x.ActionDes?.IsPage == true)
                       .FirstOrDefault();
                    if (actionPage == null && Entity.ShowOnMenu == true)
                    {
                        MSD.AddModelError("Entity.ModuleId", Localizer["_Admin.NoIndexInModule"]);
                        return;
                    }
                    List<SimpleAction> otherActions = null;
                    var mainModule = m.Where(x => x.FullName == this.SelectedModule).FirstOrDefault();
                    if(actionPage == null)
                    {
                        actionPage = new SimpleAction
                        {
                            Module = mainModule,
                            Url = "/" + mainModule.ClassName,
                            ActionName = mainModule.ModuleName
                        };
                    }
                    var mainAction = actionPage;

                    Entity.Url = mainAction.Url;
                    Entity.ModuleName = mainModule.ModuleName;
                    Entity.ActionName = mainAction.ActionDes?.Description ?? mainAction.ActionName;
                    Entity.ClassName = mainModule.FullName;
                    Entity.MethodName = null;
                    Entity.Children = new List<FrameworkMenu>();

                    otherActions = m.Where(x => x.FullName == this.SelectedModule).SelectMany(x => x.Actions).Where(x => x.MethodName != mainAction.MethodName).ToList();
                    int order = 1;
                    foreach (var action in otherActions)
                    {
                        if (SelectedActionIDs?.Contains(action.Url) == true)
                        {
                            FrameworkMenu menu = new FrameworkMenu();
                            menu.FolderOnly = false;
                            menu.IsPublic = Entity.IsPublic;
                            menu.Parent = Entity;
                            menu.ShowOnMenu = false;
                            menu.DisplayOrder = order++;
                            menu.Privileges = new List<FunctionPrivilege>();
                            menu.IsInside = true;
                            menu.Domain = Entity.Domain;
                            menu.PageName = action.ActionDes?.Description ?? action.ActionName;
                            menu.ModuleName = mainModule.ModuleName;
                            menu.ActionName = action.ActionDes?.Description ?? action.ActionName;
                            menu.ClassName = mainModule.FullName;
                            menu.MethodName = action.MethodName;
                            menu.Url = action.Url;
                            Entity.Children.Add(menu);
                        }
                    }
                }

                else
                {
                    Entity.Children = null;
                    Entity.Url = null;
                }

            }
            base.DoAdd();
            List<Guid> guids = new List<Guid>();
            guids.Add(Entity.ID);
            if (Entity.Children != null)
            {
                guids.AddRange(Entity.Children?.Select(x => x.ID).ToList());
            }
            AddPrivilege(guids);
        }

        public void AddPrivilege(List<Guid> menuids)
        {
            var admin = DC.Set<FrameworkRole>().Where(x => x.RoleCode == "001").SingleOrDefault();
            if (admin != null && SelectedRolesIds.Contains(admin.RoleCode) == false)
            {
                SelectedRolesIds.Add(admin.RoleCode);
            }
            foreach (var menuid in menuids)
            {

                if (SelectedRolesIds != null)
                {
                    foreach (var code in SelectedRolesIds)
                    {
                        FunctionPrivilege fp = new FunctionPrivilege();
                        fp.MenuItemId = menuid;
                        fp.RoleCode = code;
                        fp.Allowed = true;
                        DC.Set<FunctionPrivilege>().Add(fp);
                    }
                }
            }

            DC.SaveChanges();
        }


        public override void DoDelete()
        {
            try
            {
                DC.CascadeDelete(Entity);
                DC.SaveChanges();
            }
            catch
            { }
        }
    }
}
