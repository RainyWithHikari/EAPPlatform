using EAPPlatform.Areas.Statistics.Extension;
using EAPPlatform.DataAccess;
using EAPPlatform.Model.CustomVMs;
using EAPPlatform.Model.EAP;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Mvc;
using EAPPlatform.Util;
using System.IO;
using OfficeOpenXml;
using System.Globalization;
using System.Data;

namespace EAPPlatform.Areas.Statistics.Controllers
{
    [Area("Statistics")]
    public class OtmsDashboardController : BaseController
    {
        public ActionResult Index()
        {
            return View("Areas/Statistics/Views/OtmsDashboard/Index.cshtml");
        }
        public ActionResult dashboard()
        {
            return View("Areas/Statistics/Views/OtmsDashboard/dashboard.cshtml");
        }
        public ActionResult GetEqp()
        {
            var otmsDB = DbFactory.GetSqlSugarClient();
            string sql = @"SELECT a.EQUIPMENT_ID EQID, a.CHAMBER_ID CHAMBERID, a.STATUS , a.TEMPERTURE , b.LOCATION FROM OTMS_OVEN a, OTMS_EQUIPMENT_LOCATION b
WHERE a.CHAMBER_ID = b.CHAMBER_ID";
            var data = otmsDB.SqlQueryable<OTMSStatus>(sql).ToList().OrderBy(x => x.EQID.Split('N')[1].ToString());
            return Json(new { data });
        }
        
    }
}
