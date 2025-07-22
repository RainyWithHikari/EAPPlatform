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
using EAPPlatform.ViewModel.EAP.EquipmentTypeConfigurationVMs;

namespace EAPPlatform.EAP.Controllers
{
    public partial class EquipmentTypeConfigurationController : BaseController
    {
        
        [ActionDescription("_Page.EAP.EquipmentTypeConfiguration.Create")]
        public ActionResult Create()
        {
            var vm = Wtm.CreateVM<EAPPlatform.ViewModel.EAP.EquipmentTypeConfigurationVMs.EquipmentTypeConfigurationVM>();
            return PartialView(vm);
        }

        
        [ActionDescription("_Page.EAP.EquipmentTypeConfiguration.Edit")]
        public ActionResult Edit(string id)
        {
            var vm = Wtm.CreateVM<EAPPlatform.ViewModel.EAP.EquipmentTypeConfigurationVMs.EquipmentTypeConfigurationVM>(id);
            return PartialView(vm);
        }

        
        [ActionDescription("_Page.EAP.EquipmentTypeConfiguration.Index", IsPage = true)]
        public ActionResult Index(string id)
        {
            var vm = Wtm.CreateVM<EAPPlatform.ViewModel.EAP.EquipmentTypeConfigurationVMs.EquipmentTypeConfigurationListVM>();
            if (string.IsNullOrEmpty(id) == false)
            {
            }
            return PartialView(vm);
        }

        
        [ActionDescription("_Page.EAP.EquipmentTypeConfiguration.Details")]
        public ActionResult Details(string id)
        {
            var vm = Wtm.CreateVM<EAPPlatform.ViewModel.EAP.EquipmentTypeConfigurationVMs.EquipmentTypeConfigurationVM>(id);
            return PartialView(vm);
        }

        
        [ActionDescription("_Page.EAP.EquipmentTypeConfiguration.Import")]
        public ActionResult Import()
        {
            var vm = Wtm.CreateVM<EAPPlatform.ViewModel.EAP.EquipmentTypeConfigurationVMs.EquipmentTypeConfigurationImportVM>();
            return PartialView(vm);
        }

        
        [ActionDescription("_Page.EAP.EquipmentTypeConfiguration.BatchEdit")]
        [HttpPost]
        public ActionResult BatchEdit(string[] IDs)
        {
            var vm = Wtm.CreateVM<EAPPlatform.ViewModel.EAP.EquipmentTypeConfigurationVMs.EquipmentTypeConfigurationBatchVM>(Ids: IDs);
            return PartialView(vm);
        }

        
        [ActionDescription("_Page.EAP.EquipmentTypeConfiguration.EqpTypeConfig")]
        [HttpPost]
        public ActionResult EqpTypeConfig(string[] IDs)
        {
            var vm = Wtm.CreateVM<EAPPlatform.ViewModel.EAP.EquipmentTypeConfigurationVMs.EquipmentTypeConfigurationBatchVM>(Ids: IDs);
            return PartialView(vm);
        }


        #region Search
        [ActionDescription("SearchEquipmentTypeConfiguration")]
        [HttpPost]
        public string SearchEquipmentTypeConfiguration(EAPPlatform.ViewModel.EAP.EquipmentTypeConfigurationVMs.EquipmentTypeConfigurationSearcher searcher)
        {
            var vm = Wtm.CreateVM<EAPPlatform.ViewModel.EAP.EquipmentTypeConfigurationVMs.EquipmentTypeConfigurationListVM>(passInit: true);
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
        public IActionResult EquipmentTypeConfigurationExportExcel(EAPPlatform.ViewModel.EAP.EquipmentTypeConfigurationVMs.EquipmentTypeConfigurationListVM vm)
        {
            return vm.GetExportData();
        }
        
    }
}


