using EAPPlatform.Areas.Statistics.Extension;
using EAPPlatform.DataAccess;
using EAPPlatform.Model.CustomVMs;
using EAPPlatform.Model.EAP;
using EAPPlatform.Util;
using EAPPlatform.ViewModel.EAP.EquipmentVMs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Mvc;

namespace EAPPlatform.Areas.Statistics.Controllers
{
    [Area("Statistics")]
    public class EquipmentDashboardController : BaseController
    {
        // GET: EquipmentDashboardController
        public ActionResult Index()
        {
            return View("Areas/Statistics/Views/EquipmentDashboard/Index.cshtml");
        }

        public ActionResult dashboard()
        {

            return View("Areas/Statistics/Views/EquipmentDashboard/dashboard.cshtml");
        }

        public ActionResult equipmentdetails()
        {
            ViewBag.eqp = Request.Query["eqp"];

            return View("/Areas/Statistics/Views/EquipmentDashboard/equipmentdetails.cshtml");
        }
        public ActionResult accboxdetails()
        {

            return View("Areas/Statistics/Views/EquipmentDashboard/accboxdetails.cshtml");
        }
        public ActionResult sxHandlerIndex()
        {

            return View("Areas/Statistics/Views/EquipmentDashboard/sxHandlerIndex.cshtml");
        }
        public ActionResult sxHandlerDetails()
        {
            ViewBag.date = Request.Query["date"];
            ViewBag.alarmtype = Request.Query["alarmtype"];
            return View("Areas/Statistics/Views/EquipmentDashboard/sxHandlerDetails.cshtml");
        }
        public ActionResult sxHandlerForPd()
        {

            return View("Areas/Statistics/Views/EquipmentDashboard/sxHandlerForPd.cshtml");
        }

        public JsonResult GetData(string statusFilter,string typeFilter)
        {
            //var db = new DataContext()
            DataContext db = (DataContext)Wtm.DC;
            var user = Wtm.LoginUserInfo;
            var rolecodes = DC.Set<FrameworkUserRole>().Where(it => it.UserCode == Wtm.LoginUserInfo.ITCode).Select(it => it.RoleCode).ToList();
            var roleids = DC.Set<FrameworkRole>().Where(it => rolecodes.Contains(it.RoleCode)).Select(it => it.ID).ToList();
            var eqptypeids = DC.Set<EquipmentTypeRole>().Where(it => roleids.Contains((Guid)it.FrameworkRoleId)).Select(it => it.EquipmentTypeId).ToList();
            var eqpidids = DC.Set<Equipment>().Where(it => eqptypeids.Contains(it.EquipmentTypeId)).Select(it => it.EQID).ToList();
            if (typeFilter != null) eqpidids = DC.Set<Equipment>().Where(it => eqptypeids.Contains(it.EquipmentTypeId) && it.EquipmentType.Name == typeFilter).Select(it => it.EQID).ToList();


            var carddata = db.V_EquipmentStatus.Where(it => eqpidids.Contains(it.EQID)).ToList();
            if(statusFilter != null) carddata = db.V_EquipmentStatus.Where(it => eqpidids.Contains(it.EQID) && it.Status == statusFilter).ToList();
            var typedata = carddata.GroupBy(it => it.Type).Select(it => new { name = it.Key, value = it.Count() }).ToList();
            var statusdata = carddata.GroupBy(it => it.Status).Select(it => new { name = it.Key, value = it.Count() }).ToList();

            //10.26 Rainy Add (OEE Related)
            var oeerealtimedata = DC.Set<EquipmentParamsRealtime>().Where(it => eqpidids.Contains(it.EQID) && it.SVID.Equals("1006")).ToList();

            var data = new
            {
                carddata = carddata,
                typedata = typedata,
                statusdata = statusdata,
                oeedata = oeerealtimedata
            };
            return Json(data);

        }

        public JsonResult GetDetailsData(string equipment, string datetime)
        {
            DataContext db = (DataContext)Wtm.DC;
            var date = Convert.ToDateTime(datetime).Date;
            var mtbadata = db.EquipmentMTBAHistory.Where(it => it.EQID == equipment && it.DataTime.Date >= date.AddDays(-7)).OrderBy(it => it.DataTime);
            var runratedata = db.EquipmentRunrateHistory.Where(it => it.EQID == equipment && it.DataTime.Date >= date.AddDays(-7)).OrderBy(it => it.DataTime);

            var parameterdata = db.EquipmentParamsRealtime.Where(it => it.EQID == equipment).ToList();
            var status = db.EquipmentRealtimeStatus.Where(it => it.EQID == equipment).FirstOrDefault();
            var runrate = db.EquipmentRunrate.Where(it => it.EQID == equipment).FirstOrDefault().RunrateValue;

            if (date != DateTime.Now.Date)
            {
                //runrate = 
                runrate = runratedata.Where(it => it.DataTime.Date == date).Select(it =>it.RunrateValue).FirstOrDefault();//?.RunrateValue ?? 0.0;
            }
            

            // for trend chart
            var dates = mtbadata.Select(it => it.DataTime.ToString("yyyy-MM-dd")).ToList();
            var mtbas = mtbadata.Select(it => it.MTBAValue.ToString("0.00")).ToList();
            var runrates = runratedata.Select(it => (it.RunrateValue * 100).ToString("0.00")).ToList();

            //10.26 Rainy Add (OEE Related)
            //var oeedata = new { realtimedata = oeerealtimedata, historydata = oeehistorydata };
            var trenddata = new { dates = dates, mtbas = mtbas, runrates = runrates };

            var alarmdata = db.EquipmentAlarm.Where(it => it.AlarmEqp == equipment && it.AlarmTime > date.AddHours(9) && it.AlarmTime <= date.AddDays(1).AddHours(9)).OrderByDescending(it => it.AlarmTime)
                .Select(it => new { ALARMTIME = it.AlarmTime, ALARMCODE = it.AlarmCode, ALARMTEXT = it.AlarmText }).ToList();

            var chartdata = DashBoardService.GetChartData(db, equipment, datetime);
            //oeehistorydata
            return Json(new { trenddata, alarmdata, chartdata, parameterdata, status, runrate = (runrate * 100).ToString("0.00") });

        }
        public JsonResult GetEQP()
        {
            DataContext db = (DataContext)Wtm.DC;
            var eqtype = db.EquipmentTypes.Where(it => it.Name == "ACCBOX").Select(it => it.ID).ToList();
            var eqpidids = DC.Set<Equipment>().Where(it => eqtype.Contains((Guid)it.EquipmentTypeId)).ToList();


            string option = "<option value=\"TEST001\">Unselect</option>";
            foreach (var item in eqpidids)
            {
                option += "<option value=\"" + item.EQID + "\" >" + item.Name + "</option>";
            }
            return Json(option);
        }
        public JsonResult GetStation(string EQID, string datetime, string selecttype)
        {
            DataContext db = (DataContext)Wtm.DC;
            var eqid = EQID;
            List<string> yields = new List<string>();
            List<string> yieldsdates = new List<string>();
            List<string> fpysdates = new List<string>();
            List<string> fpy = new List<string>();
            List<string> runrates = new List<string>();
            List<string> runratedates = new List<string>();
            if (eqid != null)
            {
                var date = Convert.ToDateTime(datetime).Date;
                var data = db.EquipmentParamsHistoryCalculate.Where(it => it.EQID == eqid).ToList();
                var runratedata = db.EquipmentRunrateHistory.Where(it => it.EQID == eqid && it.DataTime.Date >= date.AddDays(-7)).OrderBy(it => it.DataTime);


                switch (selecttype)
                {
                    case "Date":
                        data = data.Where(it => it.UpdateTime >= date.AddDays(-7)).OrderBy(it => it.UpdateTime).ToList();
                        for (int i = 0; i < 7; i++)
                        {
                            var tmp = data.Where(it => it.Name.Equals("output") && it.UpdateTime >= date.AddDays(-i)).OrderByDescending(it => it.UpdateTime).Select(it => it.Value).First();
                            yields.Add(tmp);
                        }
                        break;
                    case "Week":
                        data = data.Where(it => it.UpdateTime >= date.AddDays(-7)).OrderBy(it => it.UpdateTime).ToList();
                        break;
                    case "Month":
                        data = data.Where(it => it.UpdateTime >= date.AddMonths(-1)).OrderBy(it => it.UpdateTime).ToList();
                        break;
                    case "Default":
                        data = data.Where(it => it.UpdateTime >= date.AddDays(-7)).OrderBy(it => it.UpdateTime).ToList();

                        break;
                }



                yields = data.Where(it => it.Name.Equals("output")).Select(it => it.Value).ToList();
                fpy = data.Where(it => it.Name.Equals("yield")).Select(it => it.Value).ToList();
                yieldsdates = data.Where(it => it.Name.Equals("output")).Select(it => it.UpdateTime.AddDays(-1).ToString("yyyy-MM-dd")).ToList();
                fpysdates = data.Where(it => it.Name.Equals("yield")).Select(it => it.UpdateTime.AddDays(-1).ToString("yyyy-MM-dd")).ToList();
                runrates = runratedata.Select(it => (it.RunrateValue * 100).ToString("0.00")).ToList();
                runratedates = runratedata.Select(it => it.DataTime.ToString("yyyy-MM-dd")).ToList();

                var yieldrtdata = DC.Set<EquipmentParamsRealtime>().Where(it => it.EQID == eqid && it.Name.Equals("yield")).Select(it => it.Value).First();
                var outputrtdata = DC.Set<EquipmentParamsRealtime>().Where(it => it.EQID == eqid && it.Name.Equals("output")).Select(it => it.Value).First();
                var runratertdata = DC.Set<EquipmentRunrate>().Where(it => it.EQID == eqid).Select(it => (it.RunrateValue * 100).ToString("0.00")).First();
                var trenddata = new { yield = yields, fpys = fpy, yieldsdates = yieldsdates, fpysdates = fpysdates, runrates = runrates, runratesdates = runratedates };
                var alarmdata = db.EquipmentAlarm.Where(it => it.AlarmEqp.ToUpper() == EQID && it.AlarmTime > date.AddHours(9) && it.AlarmTime <= date.AddDays(1).AddHours(9) && it.AlarmSet == true).OrderByDescending(it => it.AlarmTime)
                    .Select(it => new { ALARMTIME = it.AlarmTime, ALARMCODE = it.AlarmCode, ALARMTEXT = it.AlarmText }).ToList();
                var status = db.V_EquipmentStatus.Where(it => it.EQID == eqid).Select(it => it.Status).First();
                return Json(new { trenddata, alarmdata, status, yieldrtdata, outputrtdata, runratertdata });
            }
            return Json(new { });
        }
        public JsonResult GetYield(string EQID, string datetime, string duration)
        {
            DataContext db = (DataContext)Wtm.DC;
            var date = Convert.ToDateTime(datetime).Date;
            switch (duration)
            {
                case "by date":
                    var dashboarddata = db.EquipmentParamsHistoryRaw.Where(it => it.EQID == EQID && it.UpdateTime >= date.AddDays(-7)).OrderBy(it => it.UpdateTime);
                    break;
            }

            return Json(date);

        }

        public JsonResult GetEquipments()
        {
            DataContext db = (DataContext)Wtm.DC;

            var eqdata = DC.Set<SxHandler>().ToList();
            var rawdata = eqdata.Select(it => new { name = it.NAME, value = it.EQID, selected = true }).ToList();

            return Json(new { data = rawdata });
        }

        public JsonResult GetAlarmTableData(string date, string strprod)
        {
            DataContext db = (DataContext)Wtm.DC;
            var eqfilter = strprod.Replace(",", "','");

            string sql = string.Format(@"SELECT * FROM SXHANDLERLOG WHERE SUBSTR(OCCURRED_TIME, 0, 10) = '{0}' AND EQID IN ('{1}')", date, eqfilter);
            var alarmdata = DashBoardService.TableToListModel<SxHandlerAlarm>(db.RunSQL(sql));
            var sortdata = DC.Set<SxHandlerAlarmSort>().ToList();

            //查询到所有81,96时间差
//            DateTime startTime = Convert.ToDateTime(date);
//            DateTime endTime = startTime.AddDays(1);
//            string sql_standby = string.Format(@"SELECT * FROM
//(SELECT EQP_ALID, OCCURRED_TIME, EQID, LEAD(EQP_ALID) OVER (PARTITION BY EQID ORDER BY OCCURRED_TIME) AS REAL_NEXT_ALID, LEAD(OCCURRED_TIME) OVER (PARTITION BY EQID ORDER BY OCCURRED_TIME) AS NEXT_OCCURRED_TIME
//FROM
//(SELECT EQP_ALID, OCCURRED_TIME, EQID,
//LEAD(EQP_ALID) OVER (PARTITION BY EQID ORDER BY OCCURRED_TIME) AS NEXT_ALID
//FROM
//(SELECT * FROM SXHANDLERLOG
//WHERE EQP_ALID IN ('81', '96') 
//AND TO_DATE(SUBSTR(OCCURRED_TIME, 0, 19), 'YYYY-MM-DD HH24:mi:ss') >= TO_DATE('{0}', 'YYYY-MM-DD HH24:mi:ss')
//AND TO_DATE(SUBSTR(OCCURRED_TIME, 0, 19), 'YYYY-MM-DD HH24:mi:ss') < TO_DATE('{1}', 'YYYY-MM-DD HH24:mi:ss')
//AND RECOVERY = 'OK'
//ORDER BY EQID,TO_DATE(SUBSTR(OCCURRED_TIME, 0, 19), 'YYYY-MM-DD HH24:mi:ss')))
//WHERE EQP_ALID <> NEXT_ALID OR NEXT_ALID IS NULL)
//WHERE EQP_ALID = '81' AND REAL_NEXT_ALID = '96'", startTime.ToString("yyyy-MM-dd HH:mm:ss"), endTime.ToString("yyyy-MM-dd HH:mm:ss"));
//            var standbyData = DashBoardService.TableToListModel<HandlerStandbyData>(db.RunSQL(sql_standby));
//            var total_standby_time = standbyData.Sum(it=>it.DURATION);

            List<AlarmData> data = new List<AlarmData>();
            var not_jam = sortdata.Select(it => it.ALARMCODE).ToList();
            data.Add(new AlarmData
            {
                Date = date,
                AlarmType = "Not JAM",
                TotalCount = alarmdata.Where(it=>!not_jam.Contains(it.EQP_ALID)).Count(),
                TotalSeconds = alarmdata.Where(it => !not_jam.Contains(it.EQP_ALID)).Sum(it=>Convert.ToInt32(it.STOP_TIME)),
                //StandbySeconds = Convert.ToInt32(total_standby_time)
            });

            var handler_jam = sortdata.Where(it => it.HANDLER_JAM == 1).Select(it => it.ALARMCODE).ToList();
            data.Add(new AlarmData
            {
                Date = date,
                AlarmType = "Handler JAM",
                TotalCount = alarmdata.Where(it => handler_jam.Contains(it.EQP_ALID)).Count(),
                TotalSeconds = alarmdata.Where(it => handler_jam.Contains(it.EQP_ALID)).Sum(it => Convert.ToInt32(it.STOP_TIME)),
                //StandbySeconds = 0
            });

            //var clean_jam = sortdata.Where(it => it.CLEAN_JAM == 1).Select(it => it.ALARMCODE).ToList();
            //data.Add(new AlarmData
            //{
            //    Date = date,
            //    AlarmType = "Clean JAM",
            //    TotalCount = alarmdata.Where(it => clean_jam.Contains(it.EQP_ALID)).Count(),
            //    TotalSeconds = alarmdata.Where(it => clean_jam.Contains(it.EQP_ALID)).Sum(it => Convert.ToInt32(it.STOP_TIME))
            //});

            var fixture_jam = sortdata.Where(it => it.FIXTURE_JAM == 1).Select(it => it.ALARMCODE).ToList();
            data.Add(new AlarmData
            {
                Date = date,
                AlarmType = "Fixture JAM",
                TotalCount = alarmdata.Where(it => fixture_jam.Contains(it.EQP_ALID)).Count(),
                TotalSeconds = alarmdata.Where(it => fixture_jam.Contains(it.EQP_ALID)).Sum(it => Convert.ToInt32(it.STOP_TIME)),
                //StandbySeconds = 0
            });

            return Json(new { data = data, code = 0, count = data.Count });
        }

        public JsonResult GetAlarmChartData(string date, string[] strprod)
        {
            DataContext db = (DataContext)Wtm.DC;
            var ddate = Convert.ToDateTime(date + " 00:00:00");
            var eqfilter = string.Join("','", strprod);

            string sql = string.Format(@"SELECT * FROM SXHANDLERLOG WHERE TO_DATE(SUBSTR(OCCURRED_TIME, 0, 19),'yyyy-MM-dd HH24:mi:ss') >= TO_DATE('{0}','yyyy-MM-dd HH24:mi:ss') AND EQID IN ('{1}')", ddate.AddDays(-14).ToString("yyyy-MM-dd HH:mm:ss"),eqfilter); 
            var alarmdata = DashBoardService.TableToListModel<SxHandlerAlarm>(db.RunSQL(sql));
            var sortdata = DC.Set<SxHandlerAlarmSort>().ToList();
            var eqdata = DC.Set<SxHandler>().ToList();
            string inputsql = string.Format(@"SELECT d.NAME, p.EQID, p.LOAD as INPUT, p.DATETIME FROM SXHANDLERINPUT p
RIGHT JOIN SXHANDLER d
ON p.EQID = d.EQID
WHERE p.DATETIME >= TO_DATE('{0}','yyyy-MM-dd HH24:mi:ss') AND p.EQID IN ('{1}')", ddate.AddDays(-14).ToString("yyyy-MM-dd HH:mm:ss"), eqfilter);
            var inputdata = DashBoardService.TableToListModel<InputSumbyDay>(db.RunSQL(inputsql));

            List<AlarmData> data = new List<AlarmData>();
            //List<HandlerData> handlerdata = new List<HandlerData>();
            List<KeyParamsData> keyparamsdata = new List<KeyParamsData>();
            List<string> eqnames = new List<string>();
            List<double> MttaData = new List<double>();
            List<double> MtbaData = new List<double>();
            List<double> JamData = new List<double>();
            for (int i = -14; i <= 0; i++)
            {
                var tempdate = ddate.AddDays(i);
                var daydata = alarmdata.Where(it => it.OCCURRED_TIME.Trim().Substring(0,10) == tempdate.ToString("yyyy-MM-dd")).ToList();
                var dayinputdata = inputdata.Where(it => it.DATETIME.ToString("yyyy-MM-dd") == tempdate.ToString("yyyy-MM-dd")).ToList();

                var not_jam = sortdata.Select(it => it.ALARMCODE).ToList();
                data.Add(new AlarmData
                {
                    Date = tempdate.ToString("MM-dd"),
                    AlarmType = "Not JAM",
                    TotalCount = daydata.Where(it => !not_jam.Contains(it.EQP_ALID)).Count(),
                    TotalSeconds = daydata.Where(it => !not_jam.Contains(it.EQP_ALID)).Sum(it => Convert.ToInt32(it.STOP_TIME))
                });

                var handler_jam = sortdata.Where(it => it.HANDLER_JAM == 1).Select(it => it.ALARMCODE).ToList();
                data.Add(new AlarmData
                {
                    Date = tempdate.ToString("MM-dd"),
                    AlarmType = "Handler JAM",
                    TotalCount = daydata.Where(it => handler_jam.Contains(it.EQP_ALID)).Count(),
                    TotalSeconds = daydata.Where(it => handler_jam.Contains(it.EQP_ALID)).Sum(it => Convert.ToInt32(it.STOP_TIME))
                });

                //var clean_jam = sortdata.Where(it => it.CLEAN_JAM == 1).Select(it => it.ALARMCODE).ToList();
                //data.Add(new AlarmData
                //{
                //    Date = tempdate.ToString("MM-dd"),
                //    AlarmType = "Clean JAM",
                //    TotalCount = daydata.Where(it => clean_jam.Contains(it.EQP_ALID)).Count(),
                //    TotalSeconds = daydata.Where(it => clean_jam.Contains(it.EQP_ALID)).Sum(it => Convert.ToInt32(it.STOP_TIME))
                //});

                var fixture_jam = sortdata.Where(it => it.FIXTURE_JAM == 1).Select(it => it.ALARMCODE).ToList();
                data.Add(new AlarmData
                {
                    Date = tempdate.ToString("MM-dd"),
                    AlarmType = "Fixture JAM",
                    TotalCount = daydata.Where(it => fixture_jam.Contains(it.EQP_ALID)).Count(),
                    TotalSeconds = daydata.Where(it => fixture_jam.Contains(it.EQP_ALID)).Sum(it => Convert.ToInt32(it.STOP_TIME))
                });

                var totalStopTimebyDay = Convert.ToInt32(daydata.Where(it => not_jam.Contains(it.EQP_ALID)).Sum(it => it.STOP_TIME));
                var totalJamCountbyDay = daydata.Where(it => not_jam.Contains(it.EQP_ALID)).Count();
                var totalHandlerJambyDay = daydata.Where(it => handler_jam.Contains(it.EQP_ALID)).Count();
                double TotalMtba = 0, TotalMtta = 0, TotalJam = 0;
                if (totalJamCountbyDay != 0)
                {
                    TotalMtba = (24.0 - totalStopTimebyDay / 3600.0) / totalJamCountbyDay;
                    TotalMtta = (double)totalStopTimebyDay / totalJamCountbyDay;
                }
                var totalInputbyDay = dayinputdata.Sum(it => Convert.ToInt32(it.INPUT));
                if (totalHandlerJambyDay != 0)
                    TotalJam = (double)totalInputbyDay / totalHandlerJambyDay;

                keyparamsdata.Add(new KeyParamsData()
                {
                    Date = tempdate.ToString("MM-dd"),
                    AlarmType = "MTTA",
                    Value = TotalMtta
                });

                keyparamsdata.Add(new KeyParamsData()
                {
                    Date = tempdate.ToString("MM-dd"),
                    AlarmType = "MTBA",
                    Value = TotalMtba
                });

                keyparamsdata.Add(new KeyParamsData()
                {
                    Date = tempdate.ToString("MM-dd"),
                    AlarmType = "1/JAM",
                    Value = TotalJam
                });

                if (i == 0)
                {
                    foreach (var eq in strprod)
                    {
                        var eqname = eqdata.Where(it => it.EQID == eq).FirstOrDefault()?.NAME;
                        var totalStopTime = Convert.ToInt32(daydata.Where(it => not_jam.Contains(it.EQP_ALID) && it.EQID == eq).Sum(it => it.STOP_TIME));
                        var totalJamCount = daydata.Where(it => not_jam.Contains(it.EQP_ALID) && it.EQID == eq).Count();
                        double mtba = 0, mtta = 0, jam = 0;
                        if (totalJamCount != 0)
                        {
                            mtba = (24.0 - totalStopTime / 3600.0) / totalJamCount;
                            mtta = (double)totalStopTime / totalJamCount;
                        }
                        var totalHandlerJam = daydata.Where(it => handler_jam.Contains(it.EQP_ALID) && it.EQID == eq).Count();
                        var dayInputbyEq = dayinputdata.Where(it => it.EQID == eq).FirstOrDefault();
                        if (dayInputbyEq != null)
                        {
                            var totalInput = Convert.ToInt32(dayInputbyEq.INPUT);
                            if (totalHandlerJam != 0)
                                jam = (double)totalInput / totalHandlerJam;
                        }

                        //handlerdata.Add(new HandlerData()
                        //{
                        //    Date = tempdate.ToString("MM-dd"),
                        //    EQNAME = eqname,
                        //    MTTA = mtta,
                        //    MTBA = mtba,
                        //    JAM = jam
                        //});
                        eqnames.Add(eqname);
                        MttaData.Add(mtta);
                        MtbaData.Add(mtba);
                        JamData.Add(jam);
                    }
                }
            }

            HandlerData handlerdata = new HandlerData()
            {
                Equipments = eqnames,
                MTTAData = MttaData,
                MTBAData = MtbaData,
                JAMData = JamData
            };
            return Json(new { data, handlerdata, keyparamsdata });

        }

        public JsonResult GetAlarmDetailsData(string date, string alarmtype, string strprod)
        {
            DataContext db = (DataContext)Wtm.DC;
            var eqfilter = strprod.Replace(",", "','");
            var confilter = string.Empty;

            switch(alarmtype)
            {
                case "Not JAM":
                    confilter = "WHERE s.ALARMCODE IS NULL";
                    break;
                case "Handler JAM":
                    confilter = "WHERE s.HANDLER_JAM = 1";
                    break;
                case "Clean JAM":
                    confilter = "WHERE s.CLEAN_JAM = 1";
                    break;
                case "Fixture JAM":
                    confilter = "WHERE s.FIXTURE_JAM = 1";
                    break;
            }

            string sql = string.Format(@"SELECT b.NAME AS Equipment, SUBSTR(b.OCCURRED_TIME, 0, 10) AS OccuredDate, SUBSTR(b.OCCURRED_TIME, 12, 8) AS OccuredTime, b.EQP_ALID AS AlarmId, b.PART AS Part, b.MESSAGE AS Message, b.RECOVERY AS Recovery, b.STOP_TIME AS StopTime FROM SXHANDLERALARMSORT s
RIGHT JOIN
(SELECT * FROM SXHANDLERLOG a
LEFT JOIN SXHANDLER h
ON a.EQID = h.EQID
WHERE SUBSTR(a.OCCURRED_TIME, 0, 10) = '{0}'
AND a.EQID in ('{1}')) b
ON s.ALARMCODE = b.EQP_ALID
{2}", date, eqfilter, confilter);
            var data = DashBoardService.TableToListModel<AlarmDetails>(db.RunSQL(sql));

            return Json(new { data = data, code = 0, count = data.Count });
        }

        public FileResult ExportReport(string startday, string endday, string workday)
        {
            var sdate = DateTime.Parse(startday);
            var endtitleday = DateTime.Parse(endday);
            var edate = endtitleday.AddDays(1);
            var worktime = Convert.ToInt32(workday) *24;

            DataContext db = (DataContext)Wtm.DC;
            string sql = string.Format(@"SELECT b.NAME AS Equipment, SUBSTR(b.OCCURRED_TIME, 0, 10) AS OccuredDate, SUBSTR(b.OCCURRED_TIME, 12, 8) AS OccuredTime, b.EQP_ALID AS AlarmId, b.PART AS Part, b.MESSAGE AS Message, b.RECOVERY AS Recovery, b.STOP_TIME AS StopTime, s.HANDLER_JAM, s.CLEAN_JAM, s.FIXTURE_JAM FROM SXHANDLERALARMSORT s
RIGHT JOIN
(SELECT * FROM SXHANDLERLOG a
LEFT JOIN SXHANDLER h
ON a.EQID = h.EQID
WHERE TO_DATE(SUBSTR(OCCURRED_TIME, 0, 19),'yyyy-MM-dd HH24:mi:ss') >= TO_DATE('{0}','yyyy-MM-dd HH24:mi:ss')
AND TO_DATE(SUBSTR(OCCURRED_TIME, 0, 19),'yyyy-MM-dd HH24:mi:ss') < TO_DATE('{1}','yyyy-MM-dd HH24:mi:ss')
) b
ON s.ALARMCODE = b.EQP_ALID
WHERE s.ALARMCODE is not NULL AND b.NAME IS not NULL
ORDER BY EQUIPMENT,OCCUREDDATE,OCCUREDTIME", sdate.ToString("yyyy-MM-dd HH:mm:ss"), edate.ToString("yyyy-MM-dd HH:mm:ss"));
            var data = DashBoardService.TableToListModel<AlarmDetailsSort>(db.RunSQL(sql));

            string sql_notjam = string.Format(@"SELECT b.NAME AS Equipment, SUBSTR(b.OCCURRED_TIME, 0, 10) AS OccuredDate, SUBSTR(b.OCCURRED_TIME, 12, 8) AS OccuredTime, b.EQP_ALID AS AlarmId, b.PART AS Part, b.MESSAGE AS Message, b.RECOVERY AS Recovery, b.STOP_TIME AS StopTime, s.HANDLER_JAM, s.CLEAN_JAM, s.FIXTURE_JAM FROM SXHANDLERALARMSORT s
RIGHT JOIN
(SELECT * FROM SXHANDLERLOG a
LEFT JOIN SXHANDLER h
ON a.EQID = h.EQID
WHERE TO_DATE(SUBSTR(OCCURRED_TIME, 0, 19),'yyyy-MM-dd HH24:mi:ss') >= TO_DATE('{0}','yyyy-MM-dd HH24:mi:ss')
AND TO_DATE(SUBSTR(OCCURRED_TIME, 0, 19),'yyyy-MM-dd HH24:mi:ss') < TO_DATE('{1}','yyyy-MM-dd HH24:mi:ss')
) b
ON s.ALARMCODE = b.EQP_ALID
WHERE s.ALARMCODE is NULL AND b.NAME IS not NULL
ORDER BY EQUIPMENT,OCCUREDDATE,OCCUREDTIME", sdate.ToString("yyyy-MM-dd HH:mm:ss"), edate.ToString("yyyy-MM-dd HH:mm:ss"));
            var data_notjam = DashBoardService.TableToListModel<AlarmDetailsSort>(db.RunSQL(sql_notjam));

            string input_sql = string.Format(@"SELECT d.NAME,SUM(p.LOAD) as INPUT FROM SXHANDLERINPUT p
RIGHT JOIN SXHANDLER d
ON p.EQID = d.EQID
WHERE p.DATETIME >= TO_DATE('{0}','yyyy-MM-dd HH24:mi:ss')
AND p.DATETIME < TO_DATE('{1}','yyyy-MM-dd HH24:mi:ss')
AND d.NAME IS not NULL
GROUP BY d.NAME", sdate.ToString("yyyy-MM-dd HH:mm:ss"), edate.ToString("yyyy-MM-dd HH:mm:ss"));
            var inputs = DashBoardService.TableToListModel<InputSum>(db.RunSQL(input_sql));

//            string standby_sql = string.Format(@"SELECT a.EQP_ALID, a.OCCURRED_TIME,s.NAME EQID, a.REAL_NEXT_ALID, a.NEXT_OCCURRED_TIME FROM
//(SELECT * FROM
//(SELECT EQP_ALID, OCCURRED_TIME, EQID, LEAD(EQP_ALID) OVER (PARTITION BY EQID ORDER BY OCCURRED_TIME) AS REAL_NEXT_ALID, LEAD(OCCURRED_TIME) OVER (PARTITION BY EQID ORDER BY OCCURRED_TIME) AS NEXT_OCCURRED_TIME
//FROM
//(SELECT EQP_ALID, OCCURRED_TIME, EQID,
//LEAD(EQP_ALID) OVER (PARTITION BY EQID ORDER BY OCCURRED_TIME) AS NEXT_ALID
//FROM
//(SELECT * FROM SXHANDLERLOG
//WHERE EQP_ALID IN ('81', '96') 
//AND TO_DATE(SUBSTR(OCCURRED_TIME, 0, 19), 'YYYY-MM-DD HH24:mi:ss') >= TO_DATE('{0}', 'YYYY-MM-DD HH24:mi:ss')
//AND TO_DATE(SUBSTR(OCCURRED_TIME, 0, 19), 'YYYY-MM-DD HH24:mi:ss') < TO_DATE('{1}', 'YYYY-MM-DD HH24:mi:ss')
//AND RECOVERY = 'OK'
//ORDER BY EQID,TO_DATE(SUBSTR(OCCURRED_TIME, 0, 19), 'YYYY-MM-DD HH24:mi:ss')))
//WHERE EQP_ALID <> NEXT_ALID OR NEXT_ALID IS NULL)
//WHERE EQP_ALID = '81' AND REAL_NEXT_ALID = '96') a
//LEFT JOIN SXHANDLER s
//ON a.EQID = s.EQID", sdate.ToString("yyyy-MM-dd HH:mm:ss"), edate.ToString("yyyy-MM-dd HH:mm:ss"));
//            var standbyData = DashBoardService.TableToListModel<HandlerStandbyData>(db.RunSQL(standby_sql));

            //var alarm_handlers = data.Select(it => it.Equipment).ToList();
            //var standby_handlers = standbyData.Select(it => it.EQID).ToList();
            var handlers = inputs.Select(it => it.NAME).Distinct().ToList();
            //var cleanJam_data = data.Where(it => it.CLEAN_JAM == 1).ToList();
            //var fixtureJam_data = data.Where(it => it.FIXTURE_JAM == 1).ToList();
            //alarm_handlers.AddRange(standby_handlers);
            //var handlers = alarm_handlers.Distinct().ToList();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage package = new ExcelPackage();
            ExcelWorksheet sumworksheet = package.Workbook.Worksheets.Add("SUM");
            ExcelWorksheet totalworksheet = package.Workbook.Worksheets.Add("Total");
            //ExcelWorksheet cjamworksheet = package.Workbook.Worksheets.Add("Clean JAM");
            ExcelWorksheet fjamworksheet = package.Workbook.Worksheets.Add("Fixture JAM");
            ExcelWorksheet notjamworksheet = package.Workbook.Worksheets.Add("Not JAM");

            //sum表头
            sumworksheet.Cells[1, 1].Value = "Handler";
            sumworksheet.Cells[1, 2].Value = "input";
            sumworksheet.Cells[1, 3].Value = "Jam Count";
            sumworksheet.Cells[1, 4].Value = "Handler JAM";
            sumworksheet.Cells[1, 5].Value = "Fixture JAM";
            //sumworksheet.Cells[1, 6].Value = "Clean JAM";
            sumworksheet.Cells[1, 6].Value = "StopTime";
            //加一列96-81的时间差
            //sumworksheet.Cells[1, 6].Value = "StandbyTime";
            sumworksheet.Cells[1, 7].Value = "MTBA(hrs)";
            sumworksheet.Cells[1, 8].Value = "MTTA(S)";
            sumworksheet.Cells[1, 9].Value = "1/JAM";

            int n = 2;
            handlers.ForEach(h =>
            {
                var list = data.Where(it => it.Equipment.Equals(h)).ToList();
                var input = inputs.Where(it => it.NAME.Equals(h)).FirstOrDefault()?.INPUT;
                var total = list.Count;
                var h_total = list.Where(it => it.HANDLER_JAM == 1).Count();
                var f_total = list.Where(it => it.FIXTURE_JAM == 1).Count();
                //var c_total = list.Where(it => it.CLEAN_JAM == 1).Count();
                var stoptime = list.Sum(it => it.StopTime);
                //var standbytime = Convert.ToInt32(standbyData.Where(it => it.EQID == h).Sum(it => it.DURATION));

                sumworksheet.Cells[n, 1].Value = h;
                sumworksheet.Cells[n, 2].Value = input;
                sumworksheet.Cells[n, 3].Value = total;
                sumworksheet.Cells[n, 4].Value = h_total;
                sumworksheet.Cells[n, 5].Value = f_total;
                //sumworksheet.Cells[n, 6].Value = c_total;
                sumworksheet.Cells[n, 6].Value = stoptime;
                //TODO:加一列96-81的时间差
                //sumworksheet.Cells[n, 7].Value = standbytime;
                sumworksheet.Cells[n, 7].Formula = $"=IFERROR(({worktime}-F{n}/3600)/C{n},{worktime})";
                sumworksheet.Cells[n, 8].Formula = $"=IFERROR(F{n}/C{n},F{n})";
                sumworksheet.Cells[n, 9].Formula = $"=IFERROR(B{n}/D{n},B{n})";

                n++;
            });
            //overall
            sumworksheet.Cells[n, 1].Value = "Overall";
            sumworksheet.Cells[n, 2].Formula = string.Format("SUBTOTAL(9,{0})", new ExcelAddress(2, 2, n - 1, 2).Address);
            sumworksheet.Cells[n, 3].Formula = string.Format("SUBTOTAL(9,{0})", new ExcelAddress(2, 3, n - 1, 3).Address);
            sumworksheet.Cells[n, 4].Formula = string.Format("SUBTOTAL(9,{0})", new ExcelAddress(2, 4, n - 1, 4).Address);
            sumworksheet.Cells[n, 5].Formula = string.Format("SUBTOTAL(9,{0})", new ExcelAddress(2, 5, n - 1, 5).Address);
            sumworksheet.Cells[n, 6].Formula = string.Format("SUBTOTAL(9,{0})", new ExcelAddress(2, 6, n - 1, 6).Address);
            //sumworksheet.Cells[n, 7].Formula = string.Format("SUBTOTAL(9,{0})", new ExcelAddress(2, 7, n - 1, 7).Address);
            sumworksheet.Cells[n, 7].Formula = string.Format("SUBTOTAL(1,{0})", new ExcelAddress(2, 7, n - 1, 7).Address);
            sumworksheet.Cells[n, 8].Formula = string.Format("SUBTOTAL(1,{0})", new ExcelAddress(2, 8, n - 1, 8).Address);
            sumworksheet.Cells[n, 9].Formula = $"=IFERROR(B{n}/D{n},B{n})";
            n++;

            sumworksheet.Cells[1, 1, n - 1, 9].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            sumworksheet.Cells[1, 1, n - 1, 9].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            sumworksheet.Cells[1, 1, n - 1, 9].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            sumworksheet.Cells[1, 1, n - 1, 9].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            //border.BorderAround(ExcelBorderStyle.Thin);
            sumworksheet.Cells[1, 1, n - 1, 9].Style.Font.Name = "Calibri Light";
            sumworksheet.Cells[1, 1, n - 1, 9].Style.Font.Size = 12;
            sumworksheet.Cells[1, 1, 1, 9].Style.Font.Bold = true;

            //total表头
            totalworksheet.Cells[1, 1].Value = "Handler";
            totalworksheet.Cells[1, 2].Value = "OccuredDate";
            totalworksheet.Cells[1, 3].Value = "OccuredTime";
            totalworksheet.Cells[1, 4].Value = "ALID";
            totalworksheet.Cells[1, 5].Value = "PART";
            totalworksheet.Cells[1, 6].Value = "Message";
            totalworksheet.Cells[1, 7].Value = "Recovery";
            totalworksheet.Cells[1, 8].Value = "Times";
            totalworksheet.Cells[1, 9].Value = "StopTime";

            //cleanjam表头
            //cjamworksheet.Cells[1, 1].Value = "Handler";
            //cjamworksheet.Cells[1, 2].Value = "OccuredTime";
            //cjamworksheet.Cells[1, 3].Value = "ALID";
            //cjamworksheet.Cells[1, 4].Value = "PART";
            //cjamworksheet.Cells[1, 5].Value = "Message";
            //cjamworksheet.Cells[1, 6].Value = "Recovery";
            //cjamworksheet.Cells[1, 7].Value = "Times";
            //cjamworksheet.Cells[1, 8].Value = "StopTime";

            //fixturejam表头
            fjamworksheet.Cells[1, 1].Value = "Handler";
            fjamworksheet.Cells[1, 2].Value = "OccuredDate";
            fjamworksheet.Cells[1, 3].Value = "OccuredTime";
            fjamworksheet.Cells[1, 4].Value = "ALID";
            fjamworksheet.Cells[1, 5].Value = "PART";
            fjamworksheet.Cells[1, 6].Value = "Message";
            fjamworksheet.Cells[1, 7].Value = "Recovery";
            fjamworksheet.Cells[1, 8].Value = "Times";
            fjamworksheet.Cells[1, 9].Value = "StopTime";

            int i = 2, k = 2;//j = 2,
            data.ForEach(it =>
            {
                totalworksheet.Cells[i, 1].Value = it.Equipment;
                totalworksheet.Cells[i, 2].Value = it.OccuredDate;
                totalworksheet.Cells[i, 3].Value = it.OccuredTime;
                totalworksheet.Cells[i, 4].Value = it.AlarmId;
                totalworksheet.Cells[i, 5].Value = it.Part;
                totalworksheet.Cells[i, 6].Value = it.Message;
                totalworksheet.Cells[i, 7].Value = it.Recovery;
                totalworksheet.Cells[i, 8].Value = it.Times;
                totalworksheet.Cells[i, 9].Value = it.StopTime;

                i++;

                //if(it.CLEAN_JAM == 1)
                //{
                //    cjamworksheet.Cells[j, 1].Value = it.Equipment;
                //    cjamworksheet.Cells[j, 2].Value = it.OccuredTime;
                //    cjamworksheet.Cells[j, 3].Value = it.AlarmId;
                //    cjamworksheet.Cells[j, 4].Value = it.Part;
                //    cjamworksheet.Cells[j, 5].Value = it.Message;
                //    cjamworksheet.Cells[j, 6].Value = it.Recovery;
                //    cjamworksheet.Cells[j, 7].Value = it.Times;
                //    cjamworksheet.Cells[j, 8].Value = it.StopTime;

                //    j++;
                //}

                if(it.FIXTURE_JAM == 1)
                {
                    fjamworksheet.Cells[k, 1].Value = it.Equipment;
                    fjamworksheet.Cells[k, 2].Value = it.OccuredDate;
                    fjamworksheet.Cells[k, 3].Value = it.OccuredTime;
                    fjamworksheet.Cells[k, 4].Value = it.AlarmId;
                    fjamworksheet.Cells[k, 5].Value = it.Part;
                    fjamworksheet.Cells[k, 6].Value = it.Message;
                    fjamworksheet.Cells[k, 7].Value = it.Recovery;
                    fjamworksheet.Cells[k, 8].Value = it.Times;
                    fjamworksheet.Cells[k, 9].Value = it.StopTime;

                    k++;
                }
            });
            totalworksheet.Cells[1, 1, i - 1, 10].Style.Font.Name = "等线";
            totalworksheet.Cells[1, 1, i - 1, 10].Style.Font.Size = 12;
            //cjamworksheet.Cells[1, 1, j - 1, 10].Style.Font.Name = "等线";
            //cjamworksheet.Cells[1, 1, j - 1, 10].Style.Font.Size = 12;
            fjamworksheet.Cells[1, 1, k - 1, 10].Style.Font.Name = "等线";
            fjamworksheet.Cells[1, 1, k - 1, 10].Style.Font.Size = 12;

            //notjam表头
            notjamworksheet.Cells[1, 1].Value = "Handler";
            notjamworksheet.Cells[1, 2].Value = "OccuredDate";
            notjamworksheet.Cells[1, 3].Value = "OccuredTime";
            notjamworksheet.Cells[1, 4].Value = "ALID";
            notjamworksheet.Cells[1, 5].Value = "PART";
            notjamworksheet.Cells[1, 6].Value = "Message";
            notjamworksheet.Cells[1, 7].Value = "Recovery";
            notjamworksheet.Cells[1, 8].Value = "Times";
            notjamworksheet.Cells[1, 9].Value = "StopTime";

            int t = 2;
            foreach (var item in data_notjam)
            {
                notjamworksheet.Cells[t, 1].Value = item.Equipment;
                notjamworksheet.Cells[t, 2].Value = item.OccuredDate;
                notjamworksheet.Cells[t, 3].Value = item.OccuredTime;
                notjamworksheet.Cells[t, 4].Value = item.AlarmId;
                notjamworksheet.Cells[t, 5].Value = item.Part;
                notjamworksheet.Cells[t, 6].Value = item.Message;
                notjamworksheet.Cells[t, 7].Value = item.Recovery;
                notjamworksheet.Cells[t, 8].Value = item.Times;
                notjamworksheet.Cells[t, 9].Value = item.StopTime;

                t++;
            }
            notjamworksheet.Cells[1, 1, t - 1, 10].Style.Font.Name = "等线";
            notjamworksheet.Cells[1, 1, t - 1, 10].Style.Font.Size = 12;

            using (MemoryStream ms = new MemoryStream())
            {
                package.SaveAs(ms);
                var result = ms.ToArray();
                ms.Dispose();

                var filename = "JAM" + sdate.ToString("MMdd") + "_" + endtitleday.ToString("MMdd");
                return File(result, "application/mx-excel", filename + ".xlsx");
            }

        }

        public JsonResult GetStopTimeTableData(string date, string strprod)
        {
            DataContext db = (DataContext)Wtm.DC;
            var eqfilter = strprod.Replace(",", "','");

            string sql = string.Format(@"SELECT * FROM SXHANDLERLOG WHERE SUBSTR(OCCURRED_TIME, 0, 10) = '{0}' AND EQID IN ('{1}')", date, eqfilter);
            var alarmdata = DashBoardService.TableToListModel<SxHandlerAlarm>(db.RunSQL(sql));
            var sortdata = DC.Set<SxHandlerAlarmSort>().ToList();

            //查询到所有81,96时间差
            DateTime startTime = Convert.ToDateTime(date);
            DateTime endTime = startTime.AddDays(1);
            string sql_standby = string.Format(@"SELECT * FROM
(SELECT EQP_ALID, OCCURRED_TIME, EQID,
LEAD(EQP_ALID) OVER (PARTITION BY EQID ORDER BY OCCURRED_TIME) AS NEXT_ALID,
LEAD(OCCURRED_TIME) OVER (PARTITION BY EQID ORDER BY OCCURRED_TIME) AS NEXT_OCCURRED_TIME
FROM
(SELECT * FROM SXHANDLERLOG
WHERE EQP_ALID IN ('81', '96') 
AND TO_DATE(SUBSTR(OCCURRED_TIME, 0, 19), 'YYYY-MM-DD HH24:mi:ss') >= TO_DATE('{0}', 'YYYY-MM-DD HH24:mi:ss')
AND TO_DATE(SUBSTR(OCCURRED_TIME, 0, 19), 'YYYY-MM-DD HH24:mi:ss') < TO_DATE('{1}', 'YYYY-MM-DD HH24:mi:ss')
AND EQID IN ('{2}')
AND RECOVERY = 'OK'
ORDER BY EQID,TO_DATE(SUBSTR(OCCURRED_TIME, 0, 19), 'YYYY-MM-DD HH24:mi:ss')))
WHERE EQP_ALID = '81' AND NEXT_ALID = '96'", startTime.ToString("yyyy-MM-dd HH:mm:ss"), endTime.ToString("yyyy-MM-dd HH:mm:ss"), eqfilter);
            //            string sql_standby = string.Format(@"SELECT * FROM
            //(SELECT EQP_ALID, OCCURRED_TIME, EQID, LEAD(EQP_ALID) OVER (PARTITION BY EQID ORDER BY OCCURRED_TIME) AS NEXT_ALID, LEAD(OCCURRED_TIME) OVER (PARTITION BY EQID ORDER BY OCCURRED_TIME) AS NEXT_OCCURRED_TIME
            //FROM
            //(SELECT EQP_ALID, OCCURRED_TIME, EQID,
            //LEAD(EQP_ALID) OVER (PARTITION BY EQID ORDER BY OCCURRED_TIME) AS NEXT_ALID
            //FROM
            //(SELECT * FROM SXHANDLERLOG
            //WHERE EQP_ALID IN ('81', '96') 
            //AND TO_DATE(SUBSTR(OCCURRED_TIME, 0, 19), 'YYYY-MM-DD HH24:mi:ss') >= TO_DATE('{0}', 'YYYY-MM-DD HH24:mi:ss')
            //AND TO_DATE(SUBSTR(OCCURRED_TIME, 0, 19), 'YYYY-MM-DD HH24:mi:ss') < TO_DATE('{1}', 'YYYY-MM-DD HH24:mi:ss')
            //AND EQID IN ('{2}')
            //AND RECOVERY = 'OK'
            //ORDER BY EQID,TO_DATE(SUBSTR(OCCURRED_TIME, 0, 19), 'YYYY-MM-DD HH24:mi:ss')))
            //WHERE EQP_ALID <> NEXT_ALID OR NEXT_ALID IS NULL)
            //WHERE EQP_ALID = '81' AND NEXT_ALID = '96'", startTime.ToString("yyyy-MM-dd HH:mm:ss"), endTime.ToString("yyyy-MM-dd HH:mm:ss"), eqfilter);
            var standbyData = DashBoardService.TableToListModel<HandlerStandbyData>(db.RunSQL(sql_standby));
            var total_standby_time = standbyData.Sum(it => it.DURATION);

            List<AlarmData> data = new List<AlarmData>();
            data.Add(new AlarmData
            {
                Date = date,
                AlarmType = "Handler Wait Time",
                TotalSeconds = alarmdata.Where(it => it.EQP_ALID == "79" || it.EQP_ALID == "80").Sum(it => Convert.ToInt32(it.STOP_TIME)) + Convert.ToInt32(total_standby_time),
                //StandbySeconds = Convert.ToInt32(total_standby_time)
            });

            var handler_jam = sortdata.Where(it => it.HANDLER_JAM == 1).Select(it => it.ALARMCODE).ToList();
            data.Add(new AlarmData
            {
                Date = date,
                AlarmType = "Handler Down Time",
                TotalSeconds = alarmdata.Where(it => handler_jam.Contains(it.EQP_ALID)).Sum(it => Convert.ToInt32(it.STOP_TIME)),
                //StandbySeconds = 0
            });

            var fixture_jam = sortdata.Where(it => it.FIXTURE_JAM == 1).Select(it => it.ALARMCODE).ToList();
            data.Add(new AlarmData
            {
                Date = date,
                AlarmType = "Fixture Down Time",
                TotalSeconds = alarmdata.Where(it => fixture_jam.Contains(it.EQP_ALID)).Sum(it => Convert.ToInt32(it.STOP_TIME)),
                //StandbySeconds = 0
            });

            return Json(new { data = data, code = 0, count = data.Count });
        }

        public JsonResult GetStopTimeChartData(string date, string[] strprod)
        {
            DataContext db = (DataContext)Wtm.DC;
            var ddate = Convert.ToDateTime(date + " 00:00:00");
            var eqfilter = string.Join("','", strprod);

            string sql = string.Format(@"SELECT * FROM SXHANDLERLOG WHERE TO_DATE(SUBSTR(OCCURRED_TIME, 0, 19),'yyyy-MM-dd HH24:mi:ss') >= TO_DATE('{0}','yyyy-MM-dd HH24:mi:ss') AND EQID IN ('{1}')", ddate.AddDays(-14).ToString("yyyy-MM-dd HH:mm:ss"), eqfilter);
            var alarmdata = DashBoardService.TableToListModel<SxHandlerAlarm>(db.RunSQL(sql));
            var sortdata = DC.Set<SxHandlerAlarmSort>().ToList();
            string standby_sql = string.Format(@"SELECT * FROM
(SELECT EQP_ALID, OCCURRED_TIME, EQID,
 LEAD(EQP_ALID) OVER (PARTITION BY EQID ORDER BY OCCURRED_TIME) AS NEXT_ALID,
 LEAD(OCCURRED_TIME) OVER (PARTITION BY EQID ORDER BY OCCURRED_TIME) AS NEXT_OCCURRED_TIME
 FROM
 (SELECT * FROM SXHANDLERLOG
 WHERE EQP_ALID IN ('81', '96') 
 AND TO_DATE(SUBSTR(OCCURRED_TIME, 0, 19), 'YYYY-MM-DD HH24:mi:ss') >= TO_DATE('{0}', 'YYYY-MM-DD HH24:mi:ss')
AND EQID IN ('{1}')
 AND RECOVERY = 'OK'
 ORDER BY EQID,TO_DATE(SUBSTR(OCCURRED_TIME, 0, 19), 'YYYY-MM-DD HH24:mi:ss')))
 WHERE EQP_ALID = '81' AND NEXT_ALID = '96'", ddate.AddDays(-14).ToString("yyyy-MM-dd HH:mm:ss"), eqfilter);
            var standbyData = DashBoardService.TableToListModel<HandlerStandbyData>(db.RunSQL(standby_sql));

            List<AlarmData> data = new List<AlarmData>();
            for (int i = -14; i <= 0; i++)
            {
                var tempdate = ddate.AddDays(i);
                var daydata = alarmdata.Where(it => it.OCCURRED_TIME.Trim().Substring(0, 10) == tempdate.ToString("yyyy-MM-dd")).ToList();
                var standbytime = standbyData.Where(it => it.OCCURRED_TIME.Trim().Substring(0, 10) == tempdate.ToString("yyyy-MM-dd")).Sum(it => it.DURATION);

                data.Add(new AlarmData
                {
                    Date = tempdate.ToString("MM-dd"),
                    AlarmType = "Handler Wait Time",
                    TotalSeconds = daydata.Where(it => it.EQP_ALID == "79" || it.EQP_ALID == "80").Sum(it => Convert.ToInt32(it.STOP_TIME))+ Convert.ToInt32(standbytime)
                });

                var handler_jam = sortdata.Where(it => it.HANDLER_JAM == 1).Select(it => it.ALARMCODE).ToList();
                data.Add(new AlarmData
                {
                    Date = tempdate.ToString("MM-dd"),
                    AlarmType = "Handler Down Time",
                    TotalSeconds = daydata.Where(it => handler_jam.Contains(it.EQP_ALID)).Sum(it => Convert.ToInt32(it.STOP_TIME))
                });

                var fixture_jam = sortdata.Where(it => it.FIXTURE_JAM == 1).Select(it => it.ALARMCODE).ToList();
                data.Add(new AlarmData
                {
                    Date = tempdate.ToString("MM-dd"),
                    AlarmType = "Fixture Down Time",
                    TotalSeconds = daydata.Where(it => fixture_jam.Contains(it.EQP_ALID)).Sum(it => Convert.ToInt32(it.STOP_TIME))
                });
            }

            return Json(new { data });
        }

        public FileResult ExportStopTimeReport(string startday, string endday)
        {
            var sdate = DateTime.Parse(startday);
            var endtitleday = DateTime.Parse(endday);
            var edate = endtitleday.AddDays(1);

            DataContext db = (DataContext)Wtm.DC;
            string sql = string.Format(@"SELECT b.NAME AS Equipment, SUBSTR(b.OCCURRED_TIME, 0, 10) AS OccuredDate, SUBSTR(b.OCCURRED_TIME, 12, 8) AS OccuredTime, b.EQP_ALID AS AlarmId, b.PART AS Part, b.MESSAGE AS Message, b.RECOVERY AS Recovery, b.STOP_TIME AS StopTime, s.HANDLER_JAM, s.CLEAN_JAM, s.FIXTURE_JAM FROM SXHANDLERALARMSORT s
RIGHT JOIN
(SELECT * FROM SXHANDLERLOG a
LEFT JOIN SXHANDLER h
ON a.EQID = h.EQID
WHERE TO_DATE(SUBSTR(OCCURRED_TIME, 0, 19),'yyyy-MM-dd HH24:mi:ss') >= TO_DATE('{0}','yyyy-MM-dd HH24:mi:ss')
AND TO_DATE(SUBSTR(OCCURRED_TIME, 0, 19),'yyyy-MM-dd HH24:mi:ss') < TO_DATE('{1}','yyyy-MM-dd HH24:mi:ss')
) b
ON s.ALARMCODE = b.EQP_ALID
WHERE s.ALARMCODE is not NULL AND b.NAME IS not NULL
ORDER BY EQUIPMENT,OCCUREDDATE,OCCUREDTIME", sdate.ToString("yyyy-MM-dd HH:mm:ss"), edate.ToString("yyyy-MM-dd HH:mm:ss"));
            var data = DashBoardService.TableToListModel<AlarmDetailsSort>(db.RunSQL(sql));

            string sql_notjam = string.Format(@"SELECT b.NAME AS Equipment, SUBSTR(b.OCCURRED_TIME, 0, 10) AS OccuredDate, SUBSTR(b.OCCURRED_TIME, 12, 8) AS OccuredTime, b.EQP_ALID AS AlarmId, b.PART AS Part, b.MESSAGE AS Message, b.RECOVERY AS Recovery, b.STOP_TIME AS StopTime, s.HANDLER_JAM, s.CLEAN_JAM, s.FIXTURE_JAM FROM SXHANDLERALARMSORT s
RIGHT JOIN
(SELECT * FROM SXHANDLERLOG a
LEFT JOIN SXHANDLER h
ON a.EQID = h.EQID
WHERE TO_DATE(SUBSTR(OCCURRED_TIME, 0, 19),'yyyy-MM-dd HH24:mi:ss') >= TO_DATE('{0}','yyyy-MM-dd HH24:mi:ss')
AND TO_DATE(SUBSTR(OCCURRED_TIME, 0, 19),'yyyy-MM-dd HH24:mi:ss') < TO_DATE('{1}','yyyy-MM-dd HH24:mi:ss')
AND EQP_ALID IN ('79','80','81','96')
) b
ON s.ALARMCODE = b.EQP_ALID
WHERE s.ALARMCODE is NULL AND b.NAME IS not NULL
ORDER BY EQUIPMENT,OCCUREDDATE,OCCUREDTIME", sdate.ToString("yyyy-MM-dd HH:mm:ss"), edate.ToString("yyyy-MM-dd HH:mm:ss"));
            var data_notjam = DashBoardService.TableToListModel<AlarmDetailsSort>(db.RunSQL(sql_notjam));

            string input_sql = string.Format(@"SELECT d.NAME,SUM(p.LOAD) as INPUT FROM SXHANDLERINPUT p
RIGHT JOIN SXHANDLER d
ON p.EQID = d.EQID
WHERE p.DATETIME >= TO_DATE('{0}','yyyy-MM-dd HH24:mi:ss')
AND p.DATETIME < TO_DATE('{1}','yyyy-MM-dd HH24:mi:ss')
AND d.NAME IS not NULL
GROUP BY d.NAME", sdate.ToString("yyyy-MM-dd HH:mm:ss"), edate.ToString("yyyy-MM-dd HH:mm:ss"));
            var inputs = DashBoardService.TableToListModel<InputSum>(db.RunSQL(input_sql));

            string standby_sql = string.Format(@"SELECT a.EQP_ALID, a.OCCURRED_TIME,s.NAME EQID, a.NEXT_ALID, a.NEXT_OCCURRED_TIME FROM
(SELECT * FROM
(SELECT EQP_ALID, OCCURRED_TIME, EQID,
 LEAD(EQP_ALID) OVER (PARTITION BY EQID ORDER BY OCCURRED_TIME) AS NEXT_ALID,
 LEAD(OCCURRED_TIME) OVER (PARTITION BY EQID ORDER BY OCCURRED_TIME) AS NEXT_OCCURRED_TIME
 FROM
 (SELECT * FROM SXHANDLERLOG
 WHERE EQP_ALID IN ('81', '96')
 AND TO_DATE(SUBSTR(OCCURRED_TIME, 0, 19), 'YYYY-MM-DD HH24:mi:ss') >= TO_DATE('{0}', 'YYYY-MM-DD HH24:mi:ss')
 AND TO_DATE(SUBSTR(OCCURRED_TIME, 0, 19), 'YYYY-MM-DD HH24:mi:ss') < TO_DATE('{1}', 'YYYY-MM-DD HH24:mi:ss')
 AND RECOVERY = 'OK'
 ORDER BY EQID,TO_DATE(SUBSTR(OCCURRED_TIME, 0, 19), 'YYYY-MM-DD HH24:mi:ss')))
 WHERE EQP_ALID = '81' AND NEXT_ALID = '96') a
LEFT JOIN SXHANDLER s
ON a.EQID = s.EQID
WHERE s.NAME IS not NULL", sdate.ToString("yyyy-MM-dd HH:mm:ss"), edate.ToString("yyyy-MM-dd HH:mm:ss"));
            var standbyData = DashBoardService.TableToListModel<HandlerStandbyData>(db.RunSQL(standby_sql));

            var alarm_handlers = data.Select(it => it.Equipment).ToList();
            var standby_handlers = standbyData.Select(it => it.EQID).ToList();
            //var handlers = inputs.Select(it => it.NAME).Distinct().ToList();
            alarm_handlers.AddRange(standby_handlers);
            var handlers = alarm_handlers.Distinct().ToList();

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet sumworksheet = package.Workbook.Worksheets.Add("SUM");
                ExcelWorksheet totalworksheet = package.Workbook.Worksheets.Add("Handler Down Time");
                ExcelWorksheet fjamworksheet = package.Workbook.Worksheets.Add("Fixture Down Time");
                ExcelWorksheet notjamworksheet = package.Workbook.Worksheets.Add("Handler Wait Time");

                //sum表头
                sumworksheet.Cells[1, 1].Value = "Handler";
                sumworksheet.Cells[1, 2].Value = "input";
                sumworksheet.Cells[1, 3].Value = "Handler Wait Time";
                sumworksheet.Cells[1, 4].Value = "Handler Down Time";
                sumworksheet.Cells[1, 5].Value = "Fixture Down Time";
                sumworksheet.Cells[1, 6].Value = "Total StopTime";

                int n = 2;
                handlers.ForEach(h =>
                {
                    var list = data.Where(it => it.Equipment.Equals(h)).ToList();
                    var input = inputs.Where(it => it.NAME.Equals(h)).FirstOrDefault()?.INPUT;
                    var h_total = list.Where(it => it.HANDLER_JAM == 1).Sum(it => it.StopTime);
                    var f_total = list.Where(it => it.FIXTURE_JAM == 1).Sum(it => it.StopTime);
                    var stoptime = data_notjam.Where(it=> it.Equipment.Equals(h) && (it.AlarmId == "79" || it.AlarmId == "80")).Sum(it => it.StopTime);
                    var standbytime = standbyData.Where(it => it.EQID == h).Sum(it => it.DURATION) + (double)stoptime;

                    sumworksheet.Cells[n, 1].Value = h;
                    sumworksheet.Cells[n, 2].Value = input??0;
                    sumworksheet.Cells[n, 3].Value = standbytime;
                    sumworksheet.Cells[n, 4].Value = h_total;
                    sumworksheet.Cells[n, 5].Value = f_total;
                    sumworksheet.Cells[n, 6].Formula = string.Format("SUBTOTAL(9,{0})", new ExcelAddress(n, 3, n, 5).Address); ;

                    n++;
                });
                //overall
                sumworksheet.Cells[n, 1].Value = "Overall";
                sumworksheet.Cells[n, 2].Formula = string.Format("SUBTOTAL(9,{0})", new ExcelAddress(2, 2, n - 1, 2).Address);
                sumworksheet.Cells[n, 3].Formula = string.Format("SUBTOTAL(9,{0})", new ExcelAddress(2, 3, n - 1, 3).Address);
                sumworksheet.Cells[n, 4].Formula = string.Format("SUBTOTAL(9,{0})", new ExcelAddress(2, 4, n - 1, 4).Address);
                sumworksheet.Cells[n, 5].Formula = string.Format("SUBTOTAL(9,{0})", new ExcelAddress(2, 5, n - 1, 5).Address);
                sumworksheet.Cells[n, 6].Formula = string.Format("SUM({0})", new ExcelAddress(2, 6, n - 1, 6).Address);
                n++;

                sumworksheet.Cells[1, 1, n - 1, 6].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                sumworksheet.Cells[1, 1, n - 1, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                sumworksheet.Cells[1, 1, n - 1, 6].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                sumworksheet.Cells[1, 1, n - 1, 6].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                sumworksheet.Cells[1, 1, n - 1, 6].Style.Font.Name = "Calibri Light";
                sumworksheet.Cells[1, 1, n - 1, 6].Style.Font.Size = 12;
                sumworksheet.Cells[1, 1, 1, 6].Style.Font.Bold = true;
                sumworksheet.Cells[1, 3, n-1, 3].Style.Numberformat.Format = "#";

                //total表头
                totalworksheet.Cells[1, 1].Value = "Handler";
                totalworksheet.Cells[1, 2].Value = "OccuredDate";
                totalworksheet.Cells[1, 3].Value = "OccuredTime";
                totalworksheet.Cells[1, 4].Value = "ALID";
                totalworksheet.Cells[1, 5].Value = "PART";
                totalworksheet.Cells[1, 6].Value = "Message";
                totalworksheet.Cells[1, 7].Value = "Recovery";
                totalworksheet.Cells[1, 8].Value = "Times";
                totalworksheet.Cells[1, 9].Value = "StopTime";

                //fixturejam表头
                fjamworksheet.Cells[1, 1].Value = "Handler";
                fjamworksheet.Cells[1, 2].Value = "OccuredDate";
                fjamworksheet.Cells[1, 3].Value = "OccuredTime";
                fjamworksheet.Cells[1, 4].Value = "ALID";
                fjamworksheet.Cells[1, 5].Value = "PART";
                fjamworksheet.Cells[1, 6].Value = "Message";
                fjamworksheet.Cells[1, 7].Value = "Recovery";
                fjamworksheet.Cells[1, 8].Value = "Times";
                fjamworksheet.Cells[1, 9].Value = "StopTime";

                int i = 2, k = 2;
                data.ForEach(it =>
                {
                    if (it.HANDLER_JAM == 1)
                    {
                        totalworksheet.Cells[i, 1].Value = it.Equipment;
                        totalworksheet.Cells[i, 2].Value = it.OccuredDate;
                        totalworksheet.Cells[i, 3].Value = it.OccuredTime;
                        totalworksheet.Cells[i, 4].Value = it.AlarmId;
                        totalworksheet.Cells[i, 5].Value = it.Part;
                        totalworksheet.Cells[i, 6].Value = it.Message;
                        totalworksheet.Cells[i, 7].Value = it.Recovery;
                        totalworksheet.Cells[i, 8].Value = it.Times;
                        totalworksheet.Cells[i, 9].Value = it.StopTime;

                        i++;
                    }

                    if (it.FIXTURE_JAM == 1)
                    {
                        fjamworksheet.Cells[k, 1].Value = it.Equipment;
                        fjamworksheet.Cells[k, 2].Value = it.OccuredDate;
                        fjamworksheet.Cells[k, 3].Value = it.OccuredTime;
                        fjamworksheet.Cells[k, 4].Value = it.AlarmId;
                        fjamworksheet.Cells[k, 5].Value = it.Part;
                        fjamworksheet.Cells[k, 6].Value = it.Message;
                        fjamworksheet.Cells[k, 7].Value = it.Recovery;
                        fjamworksheet.Cells[k, 8].Value = it.Times;
                        fjamworksheet.Cells[k, 9].Value = it.StopTime;

                        k++;
                    }
                });
                totalworksheet.Cells[1, 1, i - 1, 10].Style.Font.Name = "等线";
                totalworksheet.Cells[1, 1, i - 1, 10].Style.Font.Size = 12;
                fjamworksheet.Cells[1, 1, k - 1, 10].Style.Font.Name = "等线";
                fjamworksheet.Cells[1, 1, k - 1, 10].Style.Font.Size = 12;

                //notjam表头
                notjamworksheet.Cells[1, 1].Value = "Handler";
                notjamworksheet.Cells[1, 2].Value = "OccuredDate";
                notjamworksheet.Cells[1, 3].Value = "OccuredTime";
                notjamworksheet.Cells[1, 4].Value = "ALID";
                notjamworksheet.Cells[1, 5].Value = "PART";
                notjamworksheet.Cells[1, 6].Value = "Message";
                notjamworksheet.Cells[1, 7].Value = "Recovery";
                notjamworksheet.Cells[1, 8].Value = "Times";
                notjamworksheet.Cells[1, 9].Value = "StopTime";

                int t = 2;
                foreach (var item in data_notjam)
                {
                    notjamworksheet.Cells[t, 1].Value = item.Equipment;
                    notjamworksheet.Cells[t, 2].Value = item.OccuredDate;
                    notjamworksheet.Cells[t, 3].Value = item.OccuredTime;
                    notjamworksheet.Cells[t, 4].Value = item.AlarmId;
                    notjamworksheet.Cells[t, 5].Value = item.Part;
                    notjamworksheet.Cells[t, 6].Value = item.Message;
                    notjamworksheet.Cells[t, 7].Value = item.Recovery;
                    notjamworksheet.Cells[t, 8].Value = item.Times;
                    notjamworksheet.Cells[t, 9].Value = item.StopTime;

                    t++;
                }
                notjamworksheet.Cells[1, 1, t - 1, 10].Style.Font.Name = "等线";
                notjamworksheet.Cells[1, 1, t - 1, 10].Style.Font.Size = 12;

                using (MemoryStream ms = new MemoryStream())
                {
                    package.SaveAs(ms);
                    var result = ms.ToArray();
                    ms.Dispose();

                    var filename = "StopTime" + sdate.ToString("MMdd") + "_" + endtitleday.ToString("MMdd");
                    return File(result, "application/mx-excel", filename + ".xlsx");
                }
            }

        }

        public FileContentResult ExportStatusData(string EQID, string datetime)
        {
            DataContext db = (DataContext)Wtm.DC;

            DateTime starttime = new DateTime();
            DateTime chartstarttime = new DateTime();
            DateTime endtime = new DateTime();
            DateTime nowtime = DateTime.Now;
            if (nowtime.Date == Convert.ToDateTime(datetime).Date)
            {
                starttime = DateTime.Now.AddHours(-36);
                chartstarttime = DateTime.Now.AddHours(-24);
                endtime = DateTime.Now;
            }
            else
            {
                chartstarttime = Convert.ToDateTime(datetime + " 09:00:00");
                starttime = chartstarttime.AddHours(-48);
                endtime = Convert.ToDateTime(datetime + " 09:00:00").AddDays(1);
            }

            string sql = string.Format(@"SELECT STATUS, DATETIME
FROM(SELECT EQID, EQTYPE, STATUS, DATETIME,
LAG(STATUS) OVER(PARTITION BY EQID ORDER BY DATETIME) AS PRIOR_STSTUS,
LEAD(STATUS) OVER(PARTITION BY EQID ORDER BY DATETIME) AS NEXT_STATUS
FROM(SELECT * FROM EQUIPMENTSTATUS
WHERE EQID = '{0}'
AND DATETIME > = to_date('{1}','yyyy-mm-dd hh24:mi:ss')
AND DATETIME < = to_date('{2}','yyyy-mm-dd hh24:mi:ss')
AND STATUS <> 'no difine'
ORDER BY DATETIME)
)
WHERE STATUS<> PRIOR_STSTUS OR PRIOR_STSTUS IS NULL
ORDER BY DATETIME", EQID, starttime.ToString("yyyy-MM-dd HH:mm:ss"), endtime.ToString("yyyy-MM-dd HH:mm:ss"));
            var statusdata = OracleHelper.GetDTFromCommand(sql);
            List<DetailChartDataForTrip> chardata = new List<DetailChartDataForTrip>();
            Dictionary<string, int> statusInt = new Dictionary<string, int> { { "Run", 1 }, { "Alarm", 2 }, { "Idle", 3 }, { "Offline", 4 }, { "Down", 5 }, { "Unknow", 6 } };
            for (int i = 0; i < statusdata.Rows.Count; i++)
            {
                DateTime periodStartTime = Convert.ToDateTime(statusdata.Rows[i][1].ToString());
                DateTime periodEndTime = Convert.ToDateTime(statusdata.Rows[i][1].ToString());

                string periodStatus = string.Empty;
                string periodColor = string.Empty;
                int periodStatusInt = 0;
                TimeSpan timeSpan = new TimeSpan();
                string alarmtext = string.Empty;

                if (i == 0)
                {
                    periodStartTime = Convert.ToDateTime(statusdata.Rows[i][1].ToString());
                    if (periodStartTime < chartstarttime)
                    {
                        if (statusdata.Rows.Count > (i + 1))
                        {
                            if (Convert.ToDateTime(statusdata.Rows[i + 1][1]) > chartstarttime)
                            {
                                periodStartTime = chartstarttime;
                                periodEndTime = Convert.ToDateTime(statusdata.Rows[i + 1][1]);
                            }
                            else
                            {
                                continue;
                            }

                        }
                        else
                        {
                            periodStartTime = chartstarttime;
                            periodEndTime = endtime;


                        }
                    }
                    else
                    {
                        if (statusdata.Rows.Count > (i + 1))
                        {
                            periodEndTime = Convert.ToDateTime(statusdata.Rows[i + 1][1]);
                            periodStartTime = chartstarttime;
                        }
                        else
                        {
                            periodStartTime = chartstarttime;
                            periodEndTime = endtime;


                        }


                    }

                }
                else if (i == statusdata.Rows.Count - 1)
                {
                    periodStartTime = Convert.ToDateTime(statusdata.Rows[i][1].ToString());
                    periodEndTime = endtime;
                    if (periodStartTime < chartstarttime) periodStartTime = chartstarttime;

                }
                else
                {
                    periodStartTime = Convert.ToDateTime(statusdata.Rows[i][1].ToString());
                    if (periodStartTime < chartstarttime)
                    {
                        if (Convert.ToDateTime(statusdata.Rows[i + 1][1]) > chartstarttime)
                        {
                            periodStartTime = chartstarttime;
                            periodEndTime = Convert.ToDateTime(statusdata.Rows[i + 1][1]);

                        }
                        else
                        {
                            continue;
                        }


                    }
                    else
                    {
                        periodEndTime = Convert.ToDateTime(statusdata.Rows[i + 1][1]);

                    }
                }

                periodStatus = statusdata.Rows[i][0].ToString();
            
                timeSpan = periodEndTime - periodStartTime;
                
                chardata.Add(new DetailChartDataForTrip
                {
                    name = periodStatus,
                    start = periodStartTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    end = periodEndTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    duration = timeSpan.ToString(),
                });

            }
            #region old method
            //for (int i = 0; i < statusdata.Rows.Count; i++)
            //{
            //    DateTime periodStartTime = new DateTime();
            //    DateTime periodEndTime = Convert.ToDateTime(statusdata.Rows[i][1].ToString());

            //    string periodStatus = string.Empty;
            //    string periodColor = string.Empty;
            //    int periodStatusInt = 0;
            //    TimeSpan timeSpan = new TimeSpan();

            //    if (i == statusdata.Rows.Count - 1)//设置最后一段状态/只查到一个状态（时间还在9点之前）
            //    {
            //        periodEndTime = endtime;
            //        periodStartTime = Convert.ToDateTime(statusdata.Rows[i][1].ToString());
            //        periodStatus = statusdata.Rows[i][0].ToString();
            //        //periodColor = color[statusdata.Rows[i][0].ToString()];
            //        periodStatusInt = statusInt[statusdata.Rows[i][0].ToString()];

            //        timeSpan = periodEndTime - periodStartTime;
            //        //alarmtext = GetAlarmText(rawalarmdata, equipment, periodStartTime, periodEndTime);
            //        if (timeSpan.TotalMinutes != 0)
            //        {
            //            chardata.Add(new DetailChartDataForTrip
            //            {
            //                name = periodStatus,
            //                start = periodStartTime.ToString("yyyy-MM-dd HH:mm:ss"),
            //                end = periodEndTime.ToString("yyyy-MM-dd HH:mm:ss"),

            //                duration = timeSpan.ToString(),

            //            });
            //        }



            //        if (i == 0) break;
            //    }



            //    if (periodEndTime < chartstarttime) continue;

            //    if (i == 0)//前面没有筛到数据,就用后面状态
            //    {
            //        periodStartTime = chartstarttime;
            //        periodStatus = statusdata.Rows[i][0].ToString();
            //        //periodColor = color[statusdata.Rows[i][0].ToString()];
            //        periodStatusInt = statusInt[statusdata.Rows[i][0].ToString()];
            //    }
            //    else
            //    {
            //        periodStartTime = Convert.ToDateTime(statusdata.Rows[i - 1][1].ToString());
            //        periodEndTime = Convert.ToDateTime(statusdata.Rows[i][1].ToString());
            //        periodStatus = statusdata.Rows[i - 1][0].ToString();
            //        //periodColor = color[statusdata.Rows[i - 1][0].ToString()];
            //        periodStatusInt = statusInt[statusdata.Rows[i - 1][0].ToString()];
            //    }
            //    if (periodStartTime < chartstarttime) periodStartTime = chartstarttime;
            //    timeSpan = periodEndTime - periodStartTime;
            //    //alarmtext = GetAlarmText(rawalarmdata, equipment, periodStartTime, periodEndTime);
            //    if (timeSpan.TotalMinutes != 0)
            //    {
            //        chardata.Add(new DetailChartDataForTrip
            //        {
            //            name = periodStatus,
            //            start = periodStartTime.ToString("yyyy-MM-dd HH:mm:ss"),
            //            end = periodEndTime.ToString("yyyy-MM-dd HH:mm:ss"),
            //            duration = timeSpan.ToString(),

            //        });
            //    }

            //}

            #endregion
            //DataTable details = DashBoardService.ToDataTable(chardata);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage ep = new();
            ExcelWorkbook wb = ep.Workbook;
            ExcelWorksheet ws = wb.Worksheets.Add("Details");
            ws.Cells["A1"].LoadFromDataTable(DashBoardService.ToDataTable(chardata), true);
            //ws.Column(1).Style.Numberformat.Format = "yyyy/m/d";
            ws.Column(2).Style.Numberformat.Format = "yyyy/m/d h:mm:ss";
            ws.Column(3).Style.Numberformat.Format = "yyyy/m/d h:mm:ss";
            ws.Column(4).Style.Numberformat.Format = "0.000";

     
            var filename = EQID + "_" + datetime + "_" + $"StatusData.xlsx";
            var datastring = ep.GetAsByteArray();
            var result = File(datastring, "application/x-xls", filename);






            return result;
        }

        public JsonResult GetAlarmDetails(string EQID, string startdate, string enddate)
        {
            DataContext db = (DataContext)Wtm.DC;
            var start = Convert.ToDateTime(startdate).Date;
            var end = Convert.ToDateTime(enddate).Date;
            if(startdate == enddate)
            {
                start = start.AddHours(9);  
                end = end.AddDays(1).AddHours(9);
                
            }
           
            var data = db.EquipmentAlarm.Where(it => it.AlarmEqp == EQID && it.AlarmTime > start && it.AlarmTime <= end).OrderByDescending(it => it.AlarmTime)
                .Select(it => new { ALARMTIME = it.AlarmTime, ALARMCODE = it.AlarmCode, ALARMTEXT = it.AlarmText }).ToList();

            return Json(new { data = data, code = 0, count = data.Count });
        }
    }
}
