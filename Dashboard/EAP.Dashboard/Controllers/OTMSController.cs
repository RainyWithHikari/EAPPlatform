using EAP.Dashboard.Models;
using EAP.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EAP.Dashboard.Controllers
{
    public class OTMSController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetEqp()
        {
            var otmsDB = DbFactory.GetSqlSugarClient_OTMS();
            string sql = @"SELECT a.EQUIPMENT_ID EQID, a.CHAMBER_ID CHAMBERID, a.STATUS , a.TEMPERTURE , b.LOCATION FROM OTMS_OVEN a, OTMS_EQUIPMENT_LOCATION b
WHERE a.CHAMBER_ID = b.CHAMBER_ID";
            var data = otmsDB.SqlQueryable<OTMSStatus>(sql).ToList().OrderBy(x => x.EQID.Split('N')[1].ToString());
            return Json(new { data });
        }
    }
}