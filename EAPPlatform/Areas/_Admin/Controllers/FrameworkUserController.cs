using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Mvc;
using WalkingTec.Mvvm.Core.Extensions;
using System.Collections.Generic;
using EAPPlatform.Model;
using EAPPlatform.ViewModel._Admin.FrameworkUserVMs;

namespace EAPPlatform._Admin.Controllers
{
    public partial class FrameworkUserController : BaseController
    {
        
        [ActionDescription("_Page._Admin.FrameworkUser.Create")]
        public ActionResult Create()
        {
            var vm = Wtm.CreateVM<EAPPlatform.ViewModel._Admin.FrameworkUserVMs.FrameworkUserVM>();
            return PartialView(vm);
        }

        
        [ActionDescription("_Page._Admin.FrameworkUser.Edit")]
        public ActionResult Edit(string id)
        {
            var vm = Wtm.CreateVM<EAPPlatform.ViewModel._Admin.FrameworkUserVMs.FrameworkUserVM>(id);
            vm.Entity.Password = "";
            return PartialView(vm);
        }

        
        [ActionDescription("_Page._Admin.FrameworkUser.Index", IsPage = true)]
        public ActionResult Index(string id)
        {
            var vm = Wtm.CreateVM<EAPPlatform.ViewModel._Admin.FrameworkUserVMs.FrameworkUserListVM>();
            if (string.IsNullOrEmpty(id) == false)
            {
            }
            return PartialView(vm);
        }

        
        [ActionDescription("_Page._Admin.FrameworkUser.Password")]
        public ActionResult Password(string id)
        {
            var vm = Wtm.CreateVM<EAPPlatform.ViewModel._Admin.FrameworkUserVMs.FrameworkUserVM>(id);
            vm.Entity.Password = "";
            return PartialView(vm);
        }

        
        [ActionDescription("_Page._Admin.FrameworkUser.Details")]
        public ActionResult Details(string id)
        {
            var vm = Wtm.CreateVM<EAPPlatform.ViewModel._Admin.FrameworkUserVMs.FrameworkUserVM>(id);
            return PartialView(vm);
        }

        
        [ActionDescription("_Page._Admin.FrameworkUser.Import")]
        public ActionResult Import()
        {
            var vm = Wtm.CreateVM<EAPPlatform.ViewModel._Admin.FrameworkUserVMs.FrameworkUserImportVM>();
            return PartialView(vm);
        }

        
        [ActionDescription("_Page._Admin.FrameworkUser.BatchEdit")]
        [HttpPost]
        public ActionResult BatchEdit(string[] IDs)
        {
            var vm = Wtm.CreateVM<EAPPlatform.ViewModel._Admin.FrameworkUserVMs.FrameworkUserBatchVM>(Ids: IDs);
            return PartialView(vm);
        }


        #region Search
        [ActionDescription("SearchFrameworkUser")]
        [HttpPost]
        public string SearchFrameworkUser(EAPPlatform.ViewModel._Admin.FrameworkUserVMs.FrameworkUserSearcher searcher)
        {
            var vm = Wtm.CreateVM<EAPPlatform.ViewModel._Admin.FrameworkUserVMs.FrameworkUserListVM>(passInit: true);
            if (ModelState.IsValid)
            {
                vm.Searcher = searcher;
                return vm.GetJson(false);
            }
            else
            {
                return vm.GetError();
            }
        }
        #endregion

        [ActionDescription("Sys.Export")]
        [HttpPost]
        public IActionResult FrameworkUserExportExcel(EAPPlatform.ViewModel._Admin.FrameworkUserVMs.FrameworkUserListVM vm)
        {
            return vm.GetExportData();
        }
        
    }
}


