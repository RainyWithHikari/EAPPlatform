using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Mvc;
using WalkingTec.Mvvm.Core.Extensions;
using System.Linq;
using System.Collections.Generic;
using EAPPlatform.Model.EAP;
using EAPPlatform.ViewModel.EAP.EquipmentVMs;
using EAPPlatform.Model;
using System.Diagnostics;
using Microsoft.Extensions.Hosting;

namespace EAPPlatform.EAP.Controllers
{
    [Area("EAP")]
    
    [ActionDescription("_Model.Equipment")]
    public partial class EquipmentController : BaseController
    {
        #region Create
        [HttpPost]
        [ActionDescription("Sys.Create")]
        public async Task<ActionResult> Create(EquipmentVM vm)
        {
            if (!ModelState.IsValid)
            {
                
                return PartialView(vm.FromView, vm);
            }
            else
            {
                await vm.DoAddAsync();
                if (!ModelState.IsValid)
                {
                    
                    vm.DoReInit();
                    return PartialView("../Equipment/Create", vm);
                }
                else
                {
                    return FFResult().CloseDialog().RefreshGrid();
                }
            }
        }
        #endregion

        #region Edit
       
        [ActionDescription("Sys.Edit")]
        [HttpPost]
        [ValidateFormItemOnly]
        public async Task<ActionResult> Edit(EquipmentVM vm)
        {
            if (!ModelState.IsValid)
            {
                
                return PartialView(vm.FromView, vm);
            }
            else
            {
                await vm.DoEditAsync();
                if (!ModelState.IsValid)
                {
                    
                    vm.DoReInit();
                    return PartialView("../Equipment/Edit", vm);
                }
                else
                {
                    return FFResult().CloseDialog().RefreshGridRow(CurrentWindowId);
                }
            }
        }
        #endregion
      
        #region BatchEdit

        [HttpPost]
        [ActionDescription("Sys.BatchEdit")]
        public ActionResult DoBatchEdit(EquipmentBatchVM vm, IFormCollection nouse)
        {
            if (!ModelState.IsValid || !vm.DoBatchEdit())
            {
                return PartialView(vm.FromView, vm);
            }
            else
            {
                return FFResult().CloseDialog().RefreshGrid().Alert(Localizer["Sys.BatchEditSuccess", vm.Ids.Length]);
            }
        }
        #endregion

        #region BatchDelete
        [HttpPost]
        [ActionDescription("Sys.BatchDelete")]
        public ActionResult BatchDelete(string[] ids)
        {
            var vm = Wtm.CreateVM<EquipmentBatchVM>();
            if (ids != null && ids.Length > 0)
            {
                vm.Ids = ids;
            }
            else
            {
                return Ok();
            }
            if (!ModelState.IsValid || !vm.DoBatchDelete())
            {
                return FFResult().Alert(ModelState.GetErrorJson().GetFirstError());
            }
            else
            {
                return FFResult().RefreshGrid(CurrentWindowId).Alert(Localizer["Sys.BatchDeleteSuccess",vm.Ids.Length]);
            }
        }
        #endregion
      
        #region Import
        [HttpPost]
        [ActionDescription("Sys.Import")]
        public ActionResult Import(EquipmentImportVM vm, IFormCollection nouse)
        {
            if (vm.ErrorListVM.EntityList.Count > 0 || !vm.BatchSaveData())
            {
                return PartialView(vm.FromView, vm);
            }
            else
            {
                return FFResult().CloseDialog().RefreshGrid().Alert(Localizer["Sys.ImportSuccess", vm.EntityList.Count.ToString()]);
            }
        }
        #endregion



        
        public ActionResult GetEquipmentTypes()
        {
            return JsonMore(DC.Set<EquipmentType>().GetSelectListItems(Wtm, x => x.Name));
        }
        public ActionResult Select_GetEquipmentByEquipmentType(List<string> id)
        {
            var rv = DC.Set<Equipment>().CheckIDs(id, x => x.EquipmentTypeId).GetSelectListItems(Wtm,x=>x.EQID.ToString());
            return JsonMore(rv);
        }

        public ActionResult Select_GetEquipmentTypeByEquipmentTypeId(List<string> id)
        {
            var rv = DC.Set<EquipmentType>().CheckIDs(id).GetSelectListItems(Wtm, x => x.Name.ToString());
            return JsonMore(rv);
        }

        public ActionResult GetConfigutionByEquipmentID(Guid id)
        {
            var data = DC.Set<EquipmentConfiguration>().Where(it => it.EQIDId == id && it.IsValid == true);
            return JsonMore(data);
            //return Json(new { data = data, code = 200, count = data.Count() });
        }

        public async Task<JsonResult> SendModifyRequest(Guid ID,string field,string value)
        {
            string eapurl = Wtm.ConfigInfo.AppSettings["EapServiceApiUrl"].TrimEnd('/');
            var   apiResult = await Wtm.CallAPI(domainName:"",url: $"{eapurl}/api/seteqpconfig", HttpMethodEnum.POST, postdata: new { ID = ID, field = field, value = value },timeout : 1);
            var result = apiResult.Data;
            string message = "Fail";
            if (result?.Contains("success")??false) message = "发送请求成功，请稍后刷新查看。";

            return Json(new { message = message });
        }
    }
}