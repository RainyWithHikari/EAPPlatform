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
using EAPPlatform.ViewModel.EAP.EquipmentVMs;

namespace EAPPlatform.EAP.Controllers
{
    public partial class EquipmentController : BaseController
    {
        
        [ActionDescription("_Page.EAP.Equipment.Create")]
        public ActionResult Create()
        {
            var vm = Wtm.CreateVM<EAPPlatform.ViewModel.EAP.EquipmentVMs.EquipmentVM>();
            return PartialView(vm);
        }

        
        [ActionDescription("_Page.EAP.Equipment.Edit")]
        public ActionResult Edit(string id)
        {
            var vm = Wtm.CreateVM<EAPPlatform.ViewModel.EAP.EquipmentVMs.EquipmentVM>(id);
            return PartialView(vm);
        }

        
        [ActionDescription("_Page.EAP.Equipment.Index", IsPage = true)]
        public ActionResult Index(string id)
        {
            var vm = Wtm.CreateVM<EAPPlatform.ViewModel.EAP.EquipmentVMs.EquipmentListVM>();
            if (string.IsNullOrEmpty(id) == false)
            {
            }
            return PartialView(vm);
        }

        
        [ActionDescription("_Page.EAP.Equipment.Details")]
        public ActionResult Details(string id)
        {
            var vm = Wtm.CreateVM<EAPPlatform.ViewModel.EAP.EquipmentVMs.EquipmentVM>(id);
            return PartialView(vm);
        }

        
        [ActionDescription("_Page.EAP.Equipment.Import")]
        public ActionResult Import()
        {
            var vm = Wtm.CreateVM<EAPPlatform.ViewModel.EAP.EquipmentVMs.EquipmentImportVM>();
            return PartialView(vm);
        }

        
        [ActionDescription("_Page.EAP.Equipment.BatchEdit")]
        [HttpPost]
        public ActionResult BatchEdit(string[] IDs)
        {
            var vm = Wtm.CreateVM<EAPPlatform.ViewModel.EAP.EquipmentVMs.EquipmentBatchVM>(Ids: IDs);
            return PartialView(vm);
        }


        #region Search
        [ActionDescription("SearchEquipment")]
        [HttpPost]
        public string SearchEquipment(EAPPlatform.ViewModel.EAP.EquipmentVMs.EquipmentSearcher searcher)
        {
            var vm = Wtm.CreateVM<EAPPlatform.ViewModel.EAP.EquipmentVMs.EquipmentListVM>(passInit: true);
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
        public IActionResult EquipmentExportExcel(EAPPlatform.ViewModel.EAP.EquipmentVMs.EquipmentListVM vm)
        {
            return vm.GetExportData();
        }

        [ActionDescription("EqpConfig")]
        //[HttpPost]

        public ActionResult EqpConfig(string id)
        {
            ViewBag.id = id;
            //return View();
            return View("EqpConfig");
            // var vm = Wtm.CreateVM<EAPPlatform.ViewModel.EAP.EquipmentVMs.EquipmentVM>(id);
            var vm = Wtm.CreateVM<EAPPlatform.ViewModel.EAP.EquipmentConfigurationVMs.EquipmentConfigurationBatchVM>(id);
            return PartialView(vm);
        }

    }
}


