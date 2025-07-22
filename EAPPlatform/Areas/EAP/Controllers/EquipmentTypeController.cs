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
using EAPPlatform.ViewModel.EAP.EquipmentTypeVMs;

namespace EAPPlatform.EAP.Controllers
{
    public partial class EquipmentTypeController : BaseController
    {
        
        [ActionDescription("_Page.EAP.EquipmentType.Create")]
        public ActionResult Create()
        {
            var vm = Wtm.CreateVM<EAPPlatform.ViewModel.EAP.EquipmentTypeVMs.EquipmentTypeVM>();
            return PartialView(vm);
        }

        
        [ActionDescription("_Page.EAP.EquipmentType.Edit")]
        public ActionResult Edit(string id)
        {
            var vm = Wtm.CreateVM<EAPPlatform.ViewModel.EAP.EquipmentTypeVMs.EquipmentTypeVM>(id);
            return PartialView(vm);
        }

        
        [ActionDescription("_Page.EAP.EquipmentType.Index", IsPage = true)]
        public ActionResult Index(string id)
        {
            var vm = Wtm.CreateVM<EAPPlatform.ViewModel.EAP.EquipmentTypeVMs.EquipmentTypeListVM>();
            if (string.IsNullOrEmpty(id) == false)
            {
            }
            return PartialView(vm);
        }

        
        [ActionDescription("_Page.EAP.EquipmentType.Details")]
        public ActionResult Details(string id)
        {
            var vm = Wtm.CreateVM<EAPPlatform.ViewModel.EAP.EquipmentTypeVMs.EquipmentTypeVM>(id);
            return PartialView(vm);
        }

        
        [ActionDescription("_Page.EAP.EquipmentType.Import")]
        public ActionResult Import()
        {
            var vm = Wtm.CreateVM<EAPPlatform.ViewModel.EAP.EquipmentTypeVMs.EquipmentTypeImportVM>();
            return PartialView(vm);
        }

        
        [ActionDescription("_Page.EAP.EquipmentType.BatchEdit")]
        [HttpPost]
        public ActionResult BatchEdit(string[] IDs)
        {
            var vm = Wtm.CreateVM<EAPPlatform.ViewModel.EAP.EquipmentTypeVMs.EquipmentTypeBatchVM>(Ids: IDs);
            return PartialView(vm);
        }


        #region Search
        [ActionDescription("SearchEquipmentType")]
        [HttpPost]
        public string SearchEquipmentType(EAPPlatform.ViewModel.EAP.EquipmentTypeVMs.EquipmentTypeSearcher searcher)
        {
            var vm = Wtm.CreateVM<EAPPlatform.ViewModel.EAP.EquipmentTypeVMs.EquipmentTypeListVM>(passInit: true);
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
        public IActionResult EquipmentTypeExportExcel(EAPPlatform.ViewModel.EAP.EquipmentTypeVMs.EquipmentTypeListVM vm)
        {
            return vm.GetExportData();
        }


        public ActionResult Privilege(string id)
        {
            var vm = Wtm.CreateVM<EAPPlatform.ViewModel.EAP.EquipmentTypeVMs.EquipmentTypeVM>(id);
            return PartialView(vm);
        }
    }
}


