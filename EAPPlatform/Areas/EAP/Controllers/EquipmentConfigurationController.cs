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
using EAPPlatform.ViewModel.EAP.EquipmentConfigurationVMs;

namespace EAPPlatform.EAP.Controllers
{
    public partial class EquipmentConfigurationController : BaseController
    {
        
        [ActionDescription("_Page.EAP.EquipmentConfiguration.Create")]
        public ActionResult Create()
        {
            var vm = Wtm.CreateVM<EAPPlatform.ViewModel.EAP.EquipmentConfigurationVMs.EquipmentConfigurationVM>();
            return PartialView(vm);
        }

        
        [ActionDescription("_Page.EAP.EquipmentConfiguration.Edit")]
        public ActionResult Edit(string id)
        {
            var vm = Wtm.CreateVM<EAPPlatform.ViewModel.EAP.EquipmentConfigurationVMs.EquipmentConfigurationVM>(id);
            return PartialView(vm);
        }

        
        [ActionDescription("_Page.EAP.EquipmentConfiguration.Index", IsPage = true)]
        public ActionResult Index(string id)
        {
            var vm = Wtm.CreateVM<EAPPlatform.ViewModel.EAP.EquipmentConfigurationVMs.EquipmentConfigurationListVM>();
            if (string.IsNullOrEmpty(id) == false)
            {
            }
            return PartialView(vm);
        }

        
        [ActionDescription("_Page.EAP.EquipmentConfiguration.Import")]
        public ActionResult Import()
        {
            var vm = Wtm.CreateVM<EAPPlatform.ViewModel.EAP.EquipmentConfigurationVMs.EquipmentConfigurationImportVM>();
            return PartialView(vm);
        }

        



        #region Search
        [ActionDescription("SearchEquipmentConfiguration")]
        [HttpPost]
        public string SearchEquipmentConfiguration(EAPPlatform.ViewModel.EAP.EquipmentConfigurationVMs.EquipmentConfigurationSearcher searcher)
        {
            var vm = Wtm.CreateVM<EAPPlatform.ViewModel.EAP.EquipmentConfigurationVMs.EquipmentConfigurationListVM>(passInit: true);
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
        public IActionResult EquipmentConfigurationExportExcel(EAPPlatform.ViewModel.EAP.EquipmentConfigurationVMs.EquipmentConfigurationListVM vm)
        {
            return vm.GetExportData();
        }
        
    }
}


