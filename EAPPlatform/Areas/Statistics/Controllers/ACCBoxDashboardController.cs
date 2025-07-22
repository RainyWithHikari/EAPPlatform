using EAPPlatform.Areas.Statistics.Extension;
using EAPPlatform.DataAccess;
using EAPPlatform.Model.CustomVMs;
using EAPPlatform.Model.EAP;
using EAPPlatform.Util;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.Formula.Functions;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Mvc;

namespace EAPPlatform.Areas.Statistics.Controllers
{
    [Area("Statistics")]
    public class ACCBoxDashboardController : BaseController
    {
        public ActionResult Index()
        {
            return View("Areas/Statistics/Views/AccboxDashboard/Index.cshtml");
        }
        public ActionResult accboxdashboard()
        {


            return View("Areas/Statistics/Views/AccboxDashboard/accboxDashboard.cshtml");
        }
        public ActionResult accboxdetails()
        {


            return View("Areas/Statistics/Views/AccboxDashboard/accboxdetails.cshtml");
        }
        public ActionResult setWeek()
        {


            return View("Areas/Statistics/Views/AccboxDashboard/setWeek.cshtml");
        }
        public ActionResult weekSelector()
        {


            return View("Areas/Statistics/Views/AccboxDashboard/weekSelector.cshtml");
        }
        public ActionResult setTarget()
        {
            return View("Areas/Statistics/Views/AccboxDashboard/setTarget.cshtml");
        }
        public ActionResult alarmdetails()
        {

            ViewBag.EQID = Request.Query["EQID"];
            ViewBag.week = Request.Query["week"];
            return View("Areas/Statistics/Views/AccboxDashboard/alarmdetails.cshtml");
        }

        public ActionResult ThreeTest()
        {

            return View("Areas/Statistics/Views/AccboxDashboard/ThreeTest.cshtml");
        }
        public JsonResult GetEQP()
        {
            DataContext db = (DataContext)Wtm.DC;
            var eqtype = db.EquipmentTypes.Where(it => it.Name == "ACCBOX").Select(it => it.ID).ToList();
            var eqpidids = DC.Set<Equipment>().Where(it => eqtype.Contains((Guid)it.EquipmentTypeId)).OrderBy(it => it.Sort).ToList();


            string option = "<option value=\"" + eqpidids.ElementAt(0).EQID + "\">" + eqpidids.ElementAt(0).Name + "</option>";
            foreach (var item in eqpidids)
            {
                option += "<option value=\"" + item.EQID + "\" >" + item.Name + "</option>";
            }
            return Json(new { option = option, defaultoption = eqpidids.ElementAt(0).EQID });
        }
        public JsonResult GetStation(string EQID, string datetime, string selecttype)
        {
            DataContext db = (DataContext)Wtm.DC;
            var eqid = EQID;
            Guid eqguid = Guid.NewGuid();
            List<string> output = new();
            List<string> outputdates = new();
            List<string> fpysdates = new();
            List<string> fpy = new();
            List<string> oee = new();
            List<string> oeedates = new();
            List<string> runrates = new();
            List<string> runratedates = new();
            if (eqid != null)
            {
                //ExportDataByStation(EQID, datetime);
                var date = Convert.ToDateTime(datetime).Date;
                var data = db.EquipmentParamsHistoryCalculate.Where(it => it.EQID == eqid).ToList();
                var eqtype = db.EquipmentTypes.Where(it => it.Name == "ACCBOX").Select(it => it.ID).ToList();
                var eqpidids = DC.Set<Equipment>().Where(it => eqtype.Contains((Guid)it.EquipmentTypeId)).OrderBy(it => it.Sort).Select(it => it.EQID).ToList();
                //var runratedata = db.EquipmentRunrateHistory.Where(it => it.EQID == eqid && it.DataTime.Date >= date.AddDays(-7) && it.DataTime <= date).OrderBy(it => it.DataTime);
                if (eqpidids.Contains(eqid))
                    eqguid = DC.Set<Equipment>().Where(it => it.EQID == eqid).Select(it => it.ID).Single();

                switch (selecttype)
                {
                    case "Date":
                        data = data.Where(it => it.UpdateTime >= date.AddDays(-7)).OrderBy(it => it.UpdateTime).ToList();
                        for (int i = 0; i < 7; i++)
                        {
                            var tmp = data.Where(it => it.Name.Equals("output") && it.UpdateTime >= date.AddDays(-i)).OrderByDescending(it => it.UpdateTime).Select(it => it.Value).First();
                            output.Add(tmp);
                        }
                        break;
                    case "Week":
                        data = data.Where(it => it.UpdateTime >= date.AddDays(-7)).OrderBy(it => it.UpdateTime).ToList();
                        break;
                    case "Month":
                        data = data.Where(it => it.UpdateTime >= date.AddMonths(-1)).OrderBy(it => it.UpdateTime).ToList();
                        break;
                    case "Default":
                        data = data.Where(it => it.UpdateTime >= date.AddDays(-7) && it.UpdateTime <= date.AddDays(1)).OrderBy(it => it.UpdateTime).ToList();

                        break;
                }



                output = data.Where(it => it.Name.Equals("output")).Select(it => it.Value).ToList();
                fpy = data.Where(it => it.Name.Equals("yield")).Select(it => it.Value).ToList();
                oee = data.Where(it => it.Name.Equals("oee")).Select(it => it.Value).ToList();
                outputdates = data.Where(it => it.Name.Equals("output")).Select(it => it.UpdateTime.AddDays(-1).ToString("MM-dd")).ToList();
                fpysdates = data.Where(it => it.Name.Equals("yield")).Select(it => it.UpdateTime.AddDays(-1).ToString("MM-dd")).ToList();
                oeedates = data.Where(it => it.Name.Equals("oee")).Select(it => it.UpdateTime.AddDays(-1).ToString("MM-dd")).ToList();

                //runrates = runratedata.Select(it => (it.RunrateValue * 100).ToString("0.00")).ToList();
                //runratedates = runratedata.Select(it => it.DataTime.ToString("MM-dd")).ToList();

                var rtdata = DC.Set<EquipmentParamsRealtime>().Where(it => it.EQID == eqid).ToList();
                List<EquipmentParamsRealtime> yieldrtdata = new();
                List<EquipmentParamsRealtime> outputrtdata = new();
                if (rtdata.Count > 0)
                {
                    yieldrtdata = rtdata.Where(it => it.Name.Equals("yield")).ToList();

                    outputrtdata = rtdata.Where(it => it.Name.Equals("output")).ToList();
                }

                //var runratertdata = DC.Set<EquipmentRunrate>().Where(it => it.EQID == eqid).Select(it => (it.RunrateValue * 100).ToString("0.00")).First();
                var trenddata = new { outputs = output, fpys = fpy, outputdates = outputdates, fpysdates = fpysdates, oee = oee, oeedates = oeedates };
                var alarmcodeList = db.EquipmentConfigurations.Where(it => it.EQIDId == eqguid).Select(it => it.ConfigurationValue).First();
                String[] alarmcodes = alarmcodeList.Split(",");

                var alarmdata = DC.Set<EquipmentAlarm>()
                    .Where(it => it.AlarmEqp.ToUpper() == EQID
                    && it.AlarmTime > date.AddHours(9)
                    && it.AlarmTime < date.AddDays(1).AddHours(9) && it.AlarmSet == true && alarmcodes.Contains(it.AlarmCode))
                    .OrderByDescending(it => it.AlarmTime)
                    .Select(it => new { ALARMTIME = it.AlarmTime, ALARMCODE = it.AlarmCode, ALARMTEXT = it.AlarmText }).ToList();
                var status = db.Set<EquipmentRealtimeStatus>().Where(it => it.EQID == eqid).First();
                var target = db.Set<EquipmentTypeConfiguration>().Where(it => it.ConfigurationItem == "ACCUPD").Select(it => it.ConfigurationValue);
                return Json(new { trenddata, alarmdata, status, yieldrtdata, outputrtdata, target, alarmduration = date.AddHours(9) + "~" + date.AddDays(1).AddHours(9) });
            }

            return Json(new { });
        }
        public JsonResult GetRealTimeAlarms()
        {
            DataContext db = (DataContext)Wtm.DC;
            var eqptypeids = db.EquipmentTypes.Where(it => it.Name == "ACCBOX").Select(it => it.ID).ToList();
            var eqpidids = DC.Set<Equipment>().Where(it => eqptypeids.Contains((Guid)it.EquipmentTypeId)).OrderBy(it => it.Sort).Select(it => it.EQID).ToList();
            var date = DateTime.Now;
            //var data = db.EquipmentParamsHistoryCalculate.Where(it => it.EQID == eqid).ToList();
            List<string> alarmcodes = new();
            for (int i = 0; i < eqpidids.Count; i++)
            {
                var eqid = eqpidids[i];
                var eqguid = DC.Set<Equipment>().Where(it => it.EQID == eqid).Select(it => it.ID).Single();

                var alarmcodeList = db.EquipmentConfigurations.Where(it => it.EQIDId == eqguid).Select(it => it.ConfigurationValue).First();
                var tmpList = alarmcodeList.Split(",");
                foreach (var item in tmpList)
                {

                    alarmcodes.Add(item);
                }
                //alarmcodes.Append()
                //alarmcodes.Add(alarmcodeList.Split(","));

            }


            var alarmdata = DC.Set<EquipmentAlarm>()
                .Where(it => it.AlarmSource == "ACCBOX"
                && it.AlarmTime > date.AddDays(-1)
                && it.AlarmTime < date && it.AlarmSet == true && alarmcodes.Contains(it.AlarmCode))
                .OrderByDescending(it => it.AlarmTime).Take(10)
                .Select(it => new { EQID = it.AlarmEqp, ALARMTIME = it.AlarmTime, ALARMCODE = it.AlarmCode, ALARMTEXT = it.AlarmText }).ToList();



            return Json(new { alarmdata });
        }
        public JsonResult SetStartEnd(DateTime starttime, DateTime endtime, int idleduration)
        {
            //User set the start time and end time firstly.
            DataContext db = (DataContext)Wtm.DC;
            var eqptypeids = db.EquipmentTypes.Where(it => it.Name == "ACCBOX").Select(it => it.ID).ToList();
            var eqpidids = DC.Set<Equipment>().Where(it => eqptypeids.Contains((Guid)it.EquipmentTypeId)).ToList();
            GregorianCalendar gregorianCalendar = new GregorianCalendar();

            int week = gregorianCalendar.GetWeekOfYear(starttime, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            string w = DateTime.Today.Year.ToString() + "W" + week.ToString().PadLeft(2, '0');
            //string w = "W" + week.ToString();
            string sqlstart = String.Format(@"insert into EQUIPMENTPARAMSHISCAL(EQID,NAME,VALUE,UPDATETIME,REMARK) values('{0}','{1}','{2}',to_timestamp('{3}','yyyy-mm-dd hh24:mi:ss'),'{4}')", "ACCBOX", "starttime", starttime.ToString("yyyy-MM-dd HH:mm"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), w);
            string sqlend = String.Format(@"insert into EQUIPMENTPARAMSHISCAL(EQID,NAME,VALUE,UPDATETIME,REMARK) values('{0}','{1}','{2}',to_timestamp('{3}','yyyy-mm-dd hh24:mi:ss'),'{4}')", "ACCBOX", "endtime", endtime.ToString("yyyy-MM-dd HH:mm"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), w);
            string sqlduration = String.Format(@"insert into EQUIPMENTPARAMSHISCAL(EQID,NAME,VALUE,UPDATETIME,REMARK) values('{0}','{1}','{2}',to_timestamp('{3}','yyyy-mm-dd hh24:mi:ss'),'{4}')", "ACCBOX", "idleduration", idleduration, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), w);
            db.RunSQL(sqlstart);
            db.RunSQL(sqlend);
            db.RunSQL(sqlduration);
            db.SaveChanges();

            //Post Date to function and calculate the AlarmRate
            if (DashBoardService.CalculateAlarmRate(db, eqpidids, starttime, endtime, w, idleduration))
            {
                JsonResult result = GetAlarmRate(w);
                return result;
            }
            else
            {
                return Json(new { });
            }
            //JsonResult result = CalculateAlarmRate(eqpidids, starttime, endtime, w, idleduration);



        }
        public JsonResult CalculateAlarmRate(List<Equipment> eqids, DateTime starttime, DateTime endtime, string week, int idleduration)
        {
            DataContext db = (DataContext)Wtm.DC;

            string sqlrate = string.Empty;
            string sqltime = string.Empty;

            for (int i = 0; i < eqids.Count; i++)
            {
                var eqid = eqids[i].EQID.ToString();
                var eqguid = eqids[i].ID;

                //var startdateList = db.EquipmentParamsHistoryCalculate.Where(it => it.EQID == eqid && it.Name == "starttime").ToList();
                //starttime = Convert.ToDateTime(startdateList.OrderByDescending(it => Convert.ToDateTime(it.Value)).First().Value);
                //var enddateList = db.EquipmentParamsHistoryCalculate.Where(it => it.EQID == eqid && it.Name == "endtime").ToList();
                //endtime = Convert.ToDateTime(enddateList.OrderByDescending(it => Convert.ToDateTime(it.Value)).First().Value);
                TimeSpan mfgtime = endtime - starttime;
                var alarmcodeList = db.EquipmentConfigurations.Where(it => it.EQIDId == eqguid).Select(it => it.ConfigurationValue).First();
                if (alarmcodeList != null)
                {
                    String[] alarmcodes = alarmcodeList.Split(",");
                    //var eqpalarm = db.EquipmentAlarm.Where(it => it.AlarmEqp.ToUpper() == eqid && it.AlarmTime > startdate && it.AlarmTime <= enddate && alarmcodes.Contains(it.AlarmCode)).OrderByDescending(it => it.AlarmTime)
                    //    .Select(it => new { ALARMTIME = it.AlarmTime, ALARMCODE = it.AlarmCode, ALARMTEXT = it.AlarmText, ALARMSET = it.AlarmSet }).ToList();
                    var eqpalarm = db.EquipmentAlarm.Where(it => it.AlarmEqp.ToUpper() == eqid
                    && it.AlarmTime > starttime && it.AlarmTime <= endtime
                        && alarmcodes.Contains(it.AlarmCode)).OrderByDescending(it => it.AlarmTime)
                        .Select(it => new { ALARMTIME = it.AlarmTime, ALARMCODE = it.AlarmCode, ALARMTEXT = it.AlarmText, ALARMSET = it.AlarmSet }).ToList();
                    TimeSpan alarmspan = new();
                    if (eqpalarm.Count != 0)
                    {
                        if (eqpalarm[0].ALARMSET == false)
                        {
                            for (int j = 0; j < eqpalarm.Count; j++)
                            {
                                if (eqpalarm[j].ALARMSET == false)
                                {
                                    for (int k = j + 1; k < eqpalarm.Count; k++)
                                    {
                                        if (eqpalarm[j].ALARMCODE == eqpalarm[k].ALARMCODE && eqpalarm[k].ALARMSET == true)
                                        {
                                            alarmspan += (DateTime)eqpalarm[j].ALARMTIME - (DateTime)eqpalarm[k].ALARMTIME;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            alarmspan = endtime - (DateTime)eqpalarm[0].ALARMTIME;
                            for (int j = 1; j < eqpalarm.Count; j++)
                            {
                                for (int k = j + 1; k < eqpalarm.Count; k++)
                                {
                                    if (eqpalarm[j].ALARMCODE == eqpalarm[k].ALARMCODE && eqpalarm[j].ALARMSET == false && eqpalarm[k].ALARMSET == true)
                                    {
                                        alarmspan += (DateTime)eqpalarm[j].ALARMTIME - (DateTime)eqpalarm[j + 1].ALARMTIME;
                                        break;
                                    }
                                }


                            }
                        }
                    }
                    var alarmrate = ((alarmspan.TotalMinutes / (mfgtime.TotalMinutes - idleduration)) * 100).ToString("0.00");
                    var alarmtime = alarmspan.TotalMinutes.ToString("0.00");

                    sqlrate = String.Format(@"insert into EQUIPMENTPARAMSHISCAL(EQID,NAME,VALUE,UPDATETIME,REMARK) 
                                                values('{0}','{1}','{2}',to_timestamp('{3}','yyyy-mm-dd hh24:mi:ss'),'{4}')"
                                                , eqid, "alarmrate", alarmrate,
                                                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), week);
                    sqltime = String.Format(@"insert into EQUIPMENTPARAMSHISCAL(EQID,NAME,VALUE,UPDATETIME,REMARK) 
                                                values('{0}','{1}','{2}',to_timestamp('{3}','yyyy-mm-dd hh24:mi:ss'),'{4}')"
                                                , eqid, "alarmspan", alarmtime,
                                                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), week);
                }
                else
                {

                    sqlrate = String.Format(@"insert into EQUIPMENTPARAMSHISCAL(EQID,NAME,VALUE,UPDATETIME,REMARK) 
                                                values('{0}','{1}','{2}',to_timestamp('{3}','yyyy-mm-dd hh24:mi:ss'),'{4}')"
                                                , eqid, "alarmrate", "0.00",
                                                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), week);
                    sqltime = String.Format(@"insert into EQUIPMENTPARAMSHISCAL(EQID,NAME,VALUE,UPDATETIME,REMARK) 
                                                values('{0}','{1}','{2}',to_timestamp('{3}','yyyy-mm-dd hh24:mi:ss'),'{4}')"
                                                , eqid, "alarmspan", "0.00",
                                                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), week);
                }

                db.RunSQL(sqlrate);
                db.RunSQL(sqltime);
                db.SaveChanges();

                //GetHistoryAlarmRate();
            }
            JsonResult result = GetAlarmRate(week);
            return result;
        }
        public JsonResult GetAlarmRate(string selectweek)
        {
            DataContext db = (DataContext)Wtm.DC;
            var eqptypeids = db.EquipmentTypes.Where(it => it.Name == "ACCBOX").Select(it => it.ID).ToList();
            var eqpidids = DC.Set<Equipment>().Where(it => eqptypeids.Contains((Guid)it.EquipmentTypeId)).OrderBy(it => it.Sort).Select(it => it.EQID).ToList();
            var eqpNames = DC.Set<Equipment>().Where(it => eqptypeids.Contains((Guid)it.EquipmentTypeId)).OrderBy(it => it.Sort).Select(it => it.Name).ToList();
            string alarmSQL = string.Format(@"
SELECT EQID, NAME, to_char(sum(cast(VALUE as DECIMAL(6,2))),'fm9990.99') VALUE FROM EQUIPMENTPARAMSHISCAL
WHERE REMARK = '{0}'
AND NAME <> 'starttime'
AND NAME <> 'endtime'
AND NAME <> 'idleduration'
GROUP BY EQID, NAME", selectweek);
            var alarmList = DashBoardService.TableToListModel<EquipmentParamsHistoryCalculate>(db.RunSQL(alarmSQL));

            var alarmSpanByStation = alarmList.Where(it => it.Name == "alarmspan").Select(it => new { name = it.EQID, value = it.Value }).ToList();

            List<double> alarmrates = new List<double>();
            List<double> alarmtimes = new List<double>();
            List<string> eqdata = new List<string>();
            TimeSpan mfgspan = new();
            List<DateTime> startdateList = db.EquipmentParamsHistoryCalculate
               .Where(it => it.Name == "starttime"
               && it.Remark == selectweek
               && it.EQID == "ACCBOX").ToList()
               .OrderBy(it => Convert.ToDateTime(it.Value)).Select(it => Convert.ToDateTime(it.Value)).ToList();
            List<DateTime> enddateList = db.EquipmentParamsHistoryCalculate
                .Where(it => it.Name == "endtime"
                && it.Remark == selectweek
                && it.EQID == "ACCBOX").ToList()
                .OrderBy(it => Convert.ToDateTime(it.Value)).Select(it => Convert.ToDateTime(it.Value)).ToList();
            var idleTime = db.EquipmentParamsHistoryCalculate
                .Where(it => it.Name == "idleduration"
                && it.Remark == selectweek
                && it.EQID == "ACCBOX").Sum(it => Convert.ToDouble(it.Value));


            for (int i = 0; i < startdateList.Count; i++)
            {
                mfgspan += enddateList[i] - startdateList[i];
            }
            for (int j = 0; j < eqpidids.Count; j++)
            {
                var eqid = eqpidids[j];
                for (int m = 0; m < alarmList.Count; m++)
                {
                    if (eqid == alarmList[m].EQID)
                    {
                        //if (alarmList[m].Name == "alarmrate") alarmrates.Add(Convert.ToDouble(alarmList[m].Value));
                        if (alarmList[m].Name == "alarmspan")
                        {
                            alarmtimes.Add(Convert.ToDouble(alarmList[m].Value));
                            var alarmrate = (Convert.ToDouble(alarmList[m].Value) / (mfgspan.TotalMinutes - idleTime)) * 100;
                            alarmrates.Add(double.Parse(alarmrate.ToString("0.00")));
                        }

                    }
                }
            }
            var maxalarmrates = Math.Ceiling(alarmrates.Max());
            var maxalarmtimes = Math.Ceiling(alarmtimes.Max());
            #region old method
            //for (int i = 0; i < eqpidids.Count; i++)
            //{
            //    var eqid = eqpidids[i].EQID.ToString();
            //    var eqguid = eqpidids[i].ID;
            //    eqdata.Add(eqid);
            //    var startdateList = db.EquipmentParamsHistoryCalculate.Where(it => it.EQID == eqid && it.Name == "starttime").ToList();
            //    startdate = Convert.ToDateTime(startdateList.OrderByDescending(it => Convert.ToDateTime(it.Value)).First().Value);
            //    var enddateList = db.EquipmentParamsHistoryCalculate.Where(it => it.EQID == eqid && it.Name == "endtime").ToList();
            //    enddate = Convert.ToDateTime(enddateList.OrderByDescending(it => Convert.ToDateTime(it.Value)).First().Value);
            //    TimeSpan mfgtime = enddate - startdate;
            //    var alarmcodeList = db.EquipmentConfigurations.Where(it => it.EQIDId == eqguid).Select(it => it.ConfigurationValue).First();
            //    if (alarmcodeList != null)
            //    {
            //        String[] alarmcodes = alarmcodeList.Split(",");
            //        //var eqpalarm = db.EquipmentAlarm.Where(it => it.AlarmEqp.ToUpper() == eqid && it.AlarmTime > startdate && it.AlarmTime <= enddate && alarmcodes.Contains(it.AlarmCode)).OrderByDescending(it => it.AlarmTime)
            //        //    .Select(it => new { ALARMTIME = it.AlarmTime, ALARMCODE = it.AlarmCode, ALARMTEXT = it.AlarmText, ALARMSET = it.AlarmSet }).ToList();
            //        var eqpalarm = db.EquipmentAlarm.Where(it => it.AlarmEqp.ToUpper() == eqid && alarmcodes.Contains(it.AlarmCode)).OrderByDescending(it => it.AlarmTime)
            //            .Select(it => new { ALARMTIME = it.AlarmTime, ALARMCODE = it.AlarmCode, ALARMTEXT = it.AlarmText, ALARMSET = it.AlarmSet }).ToList();
            //        TimeSpan alarmspan = new();
            //        if (eqpalarm.Count != 0)
            //        {
            //            if (eqpalarm[0].ALARMSET == false)
            //            {
            //                for (int j = 0; j < eqpalarm.Count; j++)
            //                {
            //                    if (eqpalarm[j].ALARMSET == false)
            //                    {
            //                        for (int k = j + 1; k < eqpalarm.Count; k++)
            //                        {
            //                            if (eqpalarm[j].ALARMCODE == eqpalarm[k].ALARMCODE && eqpalarm[k].ALARMSET == true)
            //                            {
            //                                alarmspan += (DateTime)eqpalarm[j].ALARMTIME - (DateTime)eqpalarm[k].ALARMTIME;
            //                                break;
            //                            }
            //                        }
            //                    }



            //                }
            //            }
            //            else
            //            {
            //                alarmspan += (DateTime)enddate - (DateTime)eqpalarm[0].ALARMTIME;
            //                for (int j = 1; j < eqpalarm.Count; j++)
            //                {
            //                    for (int k = j + 1; k < eqpalarm.Count; k++)
            //                    {
            //                        if (eqpalarm[j].ALARMCODE == eqpalarm[k].ALARMCODE && eqpalarm[j].ALARMSET == false && eqpalarm[k].ALARMSET == true)
            //                        {
            //                            alarmspan += (DateTime)eqpalarm[j].ALARMTIME - (DateTime)eqpalarm[j + 1].ALARMTIME;
            //                            break;
            //                        }
            //                    }


            //                }
            //            }
            //        }
            //        var alarmrate = (alarmspan.TotalMinutes / mfgtime.TotalMinutes) * 100;
            //        var alarmtime = alarmspan.TotalMinutes.ToString("0.00");
            //        alarmtimes.Add(alarmtime);
            //        alarmrates.Add(alarmrate.ToString("0.00"));
            //    }
            //    else
            //    {
            //        alarmtimes.Add("0.00");
            //        alarmrates.Add("0.00");
            //    }


            //}
            #endregion
            return Json(new { eqdata = eqpNames, eqids = eqpidids, alarmtimes = alarmtimes, alarmrates = alarmrates, starttime = startdateList[0].ToString(), endtime = enddateList[enddateList.Count - 1].ToString(), maxtime = maxalarmtimes, maxrate = maxalarmrates, alarmSpanByStation });

        }
        public JsonResult GetHistoryAlarmRate()
        {
            DataContext db = (DataContext)Wtm.DC;
            var eqptypeids = db.EquipmentTypes.Where(it => it.Name == "ACCBOX").Select(it => it.ID).ToList();
            var eqpidids = DC.Set<Equipment>().Where(it => eqptypeids.Contains((Guid)it.EquipmentTypeId)).OrderBy(it => it.Sort).Select(it => it.EQID).ToList();
            var weeks = DC.Set<EquipmentParamsHistoryCalculate>()
                .Where(it => it.EQID == "ACCBOX" && it.Remark != null && it.Name == "starttime")
                .Select(it => it.Remark)
                .Distinct()
                .ToList()
                .OrderBy(it => Convert.ToDouble(it.Replace("W", ""))).TakeLast(8);
            //.OrderBy(it => Convert.ToDouble(it.Split("W")[1])).TakeLast(8);
            var target = DC.Set<EquipmentTypeConfiguration>().Where(it => it.ConfigurationItem == "ACCAlarmRateTarget").Select(it => it.ConfigurationValue);

            List<double> totalrate = new();
            List<double> totaltime = new();
            List<double> totalmfg = new();
            List<EquipmentAlarmTimes> alarmArr = DashBoardService.GetAlarmTimeList(db);
            List<EquipmentAlarmTimes> alarmMonArr = DashBoardService.GetAlarmMonthlyList(alarmArr);
            List<EquipmentAlarmTimes> alarmWkArr = alarmArr.GetRange(alarmArr.Count - 8, 8);

            //alarmArr = alarmArr.GetRange(alarmArr.Count - 8, 8);
            double maxtime = 0;
            double maxrate = 0;


            foreach (var item in weeks)
            {
                TimeSpan mfgspan = new();
                List<DateTime> startdateList = db.EquipmentParamsHistoryCalculate
                .Where(it => it.Name == "starttime"
                && it.Remark == item
                && it.EQID == "ACCBOX").ToList()
                .OrderBy(it => Convert.ToDateTime(it.Value)).Select(it => Convert.ToDateTime(it.Value)).ToList();
                List<DateTime> enddateList = db.EquipmentParamsHistoryCalculate
                    .Where(it => it.Name == "endtime"
                    && it.Remark == item
                    && it.EQID == "ACCBOX").ToList()
                    .OrderBy(it => Convert.ToDateTime(it.Value)).Select(it => Convert.ToDateTime(it.Value)).ToList();
                var idleTime = db.EquipmentParamsHistoryCalculate
                    .Where(it => it.Name == "idleduration"
                    && it.Remark == item
                    && it.EQID == "ACCBOX").Sum(it => Convert.ToDouble(it.Value));


                for (int i = 0; i < startdateList.Count; i++)
                {
                    mfgspan += enddateList[i] - startdateList[i];
                }


                List<double> alarmrates = new();
                List<double> alarmtimes = new();

                double alarmtime = 0;
                //alarmrates = DC.Set<EquipmentParamsHistoryCalculate>()
                //.Where(it => eqpidids.Contains(it.EQID) && it.Remark == item && it.Name == "alarmrate").Select(it => Convert.ToDouble(it.Value)).ToList();
                alarmtimes = DC.Set<EquipmentParamsHistoryCalculate>()
                    .Where(it => eqpidids.Contains(it.EQID) && it.Remark == item && it.Name == "alarmspan").Select(it => Convert.ToDouble(it.Value)).ToList();
                //foreach (var rate in alarmrates)
                //{
                //    alarmrate += rate;
                //}
                foreach (var time in alarmtimes)
                {
                    alarmtime += time;
                }
                double alarmrate = (alarmtime / (mfgspan.TotalMinutes - idleTime)) * 100;
                double restTime = mfgspan.TotalMinutes - idleTime - alarmtime;
                EquipmentAlarmTimes alarmItem = new EquipmentAlarmTimes() { time = item, alarmtimes = alarmtime, mfgtimes = mfgspan.TotalMinutes - idleTime };
                alarmArr.Add(alarmItem);
                totalrate.Add(double.Parse(alarmrate.ToString("0.0")));
                totaltime.Add(double.Parse(alarmtime.ToString("0.0")));
                totalmfg.Add(double.Parse(restTime.ToString("0.0")));

            }
            maxtime = (double)Math.Ceiling(totaltime.Max());
            maxrate = (double)Math.Ceiling(totalrate.Max()) + 1;

            return Json(new { alarmWkArr, alarmMonArr, weekdata = weeks, totaltimes = totaltime, totalrates = totalrate, maxtime = maxtime, maxrate = maxrate, totalmfg = totalmfg, target });

        }
        public JsonResult GetWeek()
        {
            DataContext db = (DataContext)Wtm.DC;
            //var eqtype = db.EquipmentTypes.Where(it => it.Name == "ACCBOX").Select(it => it.ID).ToList();
            //var eqpidids = DC.Set<Equipment>().Where(it => eqtype.Contains((Guid)it.EquipmentTypeId)).Select(it => it.EQID).ToList();
            //var weeks = DC.Set<EquipmentParamsHistoryCalculate>().Where(it => eqpidids.Contains(it.EQID) && it.Remark != null && it.Name == "starttime").OrderBy(it => it.UpdateTime).Select(it => it.Remark).Distinct().ToList();
            var weeks = DC.Set<EquipmentParamsHistoryCalculate>().Where(it => it.EQID == "ACCBOX" && it.Remark != null && it.Name == "starttime").OrderBy(it => it.UpdateTime).Select(it => it.Remark).Distinct().ToList();

            string option = "<option value=\"" + weeks.ElementAt(weeks.Count - 1) + "\">" + weeks.ElementAt(weeks.Count - 1) + "</option>";
            foreach (var item in weeks)
            {
                option += "<option value=\"" + item + "\" >" + item + "</option>";
            }
            return Json(new { option = option, latestweek = weeks.ElementAt(weeks.Count - 1) });
        }
        public JsonResult GetStatus()
        {
            DataContext db = (DataContext)Wtm.DC;
            var eqtype = db.EquipmentTypes.Where(it => it.Name == "ACCBOX").Select(it => it.ID).ToList();
            var eqpidids = DC.Set<Equipment>().Where(it => eqtype.Contains((Guid)it.EquipmentTypeId)).OrderBy(it => it.Sort).Select(it => it.EQID).ToList();
            var status = db.V_EquipmentStatus.Where(it => eqpidids.Contains(it.EQID)).ToList();
            List<V_EquipmentStatus> statuslist = new List<V_EquipmentStatus>();
            for (int i = 0; i < eqpidids.Count; i++)
            {
                for (int j = 0; j < status.Count; j++)
                {
                    if (eqpidids[i] == status[j].EQID)
                    {
                        statuslist.Add(status[j]);
                        break;
                    }
                }

            }


            var output = db.EquipmentParamsRealtime.Where(it => eqpidids.Contains(it.EQID) && it.Name == "output").ToList();
            var yield = db.EquipmentParamsRealtime.Where(it => eqpidids.Contains(it.EQID) && it.Name == "yield").ToList();

            //var eqpProfile = new EquipmentProfile();
            List<EquipmentProfile> eqpProfiles = new List<EquipmentProfile>();

            for (int i = 0; i < eqpidids.Count; i++)
            {
                for (int j = 0; j < status.Count; j++)
                {
                    if (eqpidids[i] == status[j].EQID)
                    {
                        var eqpProfile = new EquipmentProfile()
                        {
                            EQID = eqpidids[i],
                            Status = status[j].Status
                        };
                        eqpProfiles.Add(eqpProfile);
                        break;
                    }

                }
            }
            for (int i = 0; i < eqpProfiles.Count; i++)
            {
                for (int j = 0; j < output.Count; j++)
                {
                    if (eqpProfiles[i].EQID == output[j].EQID)
                    {
                        eqpProfiles[i].output = output[j].Value;
                        break;
                    }

                }
                for (int j = 0; j < yield.Count; j++)
                {
                    if (eqpProfiles[i].EQID == yield[j].EQID)
                    {
                        eqpProfiles[i].yield = yield[j].Value;
                        break;
                    }


                }
            }
            return Json(new { eqpProfiles = eqpProfiles });
        }
        public FileContentResult ExportDataByStation(string EQID, string datetime)
        {
            DataContext db = (DataContext)Wtm.DC;

            List<AlarmItem> alarmList = new();
            DateTime startdate = Convert.ToDateTime(datetime).Date.AddHours(9);
            DateTime enddate = Convert.ToDateTime(datetime).Date.AddDays(1).AddHours(9);
            string route = string.Empty;

            alarmList = DashBoardService.GetAlarmDetails(db, startdate, enddate, EQID);


            var alarmtotal = alarmList.GroupBy(
                it => new
                {
                    Date = it.Date,
                    EQID = it.EQID,
                    AlarmCode = it.AlarmCode,
                    AlarmText = it.AlarmText
                }).Select(it => new
                {
                    Date = it.Key.Date,
                    EQID = it.Key.EQID,
                    AlarmCode = it.Key.AlarmCode,
                    AlarmText = it.Key.AlarmText,
                    Duration = it.Sum(x => x.Duration)
                }).ToList();

            DataTable details = DashBoardService.ToDataTable(alarmList);
            DataTable total = DashBoardService.ToDataTable(alarmtotal);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage ep = new();
            ExcelWorkbook wb = ep.Workbook;
            ExcelWorksheet ws = wb.Worksheets.Add("Details");
            ws.Cells["A1"].LoadFromDataTable(details, true);
            ws.Column(1).Style.Numberformat.Format = "yyyy/m/d";
            ws.Column(5).Style.Numberformat.Format = "yyyy/m/d h:mm:ss";
            ws.Column(6).Style.Numberformat.Format = "yyyy/m/d h:mm:ss";
            ws.Column(7).Style.Numberformat.Format = "0.00";

            ExcelWorksheet ws2 = wb.Worksheets.Add("Overall");
            ws2.Cells["A1"].LoadFromDataTable(total, true);
            ws2.Column(1).Style.Numberformat.Format = "yyyy/m/d";
            ws2.Column(5).Style.Numberformat.Format = "0.00";

            var filename = EQID + "_" + datetime + "_" + $"AlarmData.xlsx";
            var datastring = ep.GetAsByteArray();
            var result = File(datastring, "application/x-xls", filename);






            return result;
        }

        public FileContentResult ExportDataByWeek(string week)
        {
            DataContext db = (DataContext)Wtm.DC;
            var eqtype = db.EquipmentTypes.Where(it => it.Name == "ACCBOX").Select(it => it.ID).ToList();
            var eqpidids = DC.Set<Equipment>().Where(it => eqtype.Contains((Guid)it.EquipmentTypeId)).OrderBy(it => it.Sort).ToList();
            List<AlarmItem> alarmList = new();
            DateTime startdate = db.EquipmentParamsHistoryCalculate
               .Where(it => it.Name == "starttime"
               && it.Remark == week
               && it.EQID == "ACCBOX").ToList()
               .OrderBy(it => Convert.ToDateTime(it.Value)).Select(it => Convert.ToDateTime(it.Value)).First();
            DateTime enddate = db.EquipmentParamsHistoryCalculate
                .Where(it => it.Name == "endtime"
                && it.Remark == week
                && it.EQID == "ACCBOX").ToList()
                .OrderByDescending(it => Convert.ToDateTime(it.Value)).Select(it => Convert.ToDateTime(it.Value)).First();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage ep = new();
            ExcelWorkbook wb = ep.Workbook;
            for (int i = 0; i < eqpidids.Count; i++)
            {
                var EQID = eqpidids[i].EQID;
                alarmList = DashBoardService.GetAlarmDetails(db, startdate, enddate, EQID);
                DataTable details = DashBoardService.ToDataTable(alarmList);
                ExcelWorksheet ws = wb.Worksheets.Add(EQID);
                ws.Cells["A1"].LoadFromDataTable(details, true);
                ws.Column(5).Style.Numberformat.Format = "yyyy/m/d h:mm:ss";
                ws.Column(6).Style.Numberformat.Format = "yyyy/m/d h:mm:ss";

            }
            var filename = "ACCBOX_" + week + "_" + $"AlarmData.xlsx";
            var datastring = ep.GetAsByteArray();

            var result = File(datastring, "application/x-xls", filename);



            return result;
        }
        public FileContentResult ExportYieldData(DateTime start, DateTime end)
        {
            DataContext db = (DataContext)Wtm.DC;

            start = start.Date;
            end = end.Date;

            var eqtype = db.EquipmentTypes.Where(it => it.Name == "ACCBOX").Select(it => it.ID).ToList();
            var eqpidids = DC.Set<Equipment>().Where(it => eqtype.Contains((Guid)it.EquipmentTypeId)).OrderBy(it => it.Sort).ToList();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage ep = new();
            ExcelWorkbook wb = ep.Workbook;
            foreach (var eqp in eqpidids)
            {
                var list = db.EquipmentParamsHistoryCalculate.Where(it => it.EQID == eqp.EQID
                && (it.Name == "output" || it.Name == "yield")
                && it.UpdateTime >= start
                && it.UpdateTime <= end).OrderBy(it => it.UpdateTime).ToList();

                var query = from row1 in list
                            join row2 in list
                            on row1.UpdateTime.ToShortDateString() equals row2.UpdateTime.ToShortDateString()
                            where row1.Name == "output" && row2.Name == "yield"

                            select new DashBoardService.ParamsList
                            {
                                EQID = row1.EQID,
                                OUTPUT = row1.Value,
                                YIELD = list.FirstOrDefault(d => d.UpdateTime.Date == row1.UpdateTime.Date && d.Name == "yield")?.Value,
                                DATE = row1.UpdateTime,
                            };
                var resultList = new List<DashBoardService.ParamsList>();
                resultList.AddRange(query.Cast<DashBoardService.ParamsList>().ToList());
                ExcelWorksheet ws = wb.Worksheets.Add(eqp.EQID);
                ws.Cells["A1"].LoadFromDataTable(DashBoardService.ToDataTable(resultList), true);
                //ws.Column(2).Style.Numberformat.Format = "yyyy/m/d h:mm:ss";
                ws.Column(4).Style.Numberformat.Format = "yyyy/m/d h:mm:ss";
            }
            var filename = $"Total.xlsx";
            //$"ZJ_ACCBOX_{start.ToShortDateString()}~{end.ToShortDateString()}_生产情况.xlsx";
            var datastring = ep.GetAsByteArray();

            var fileBytesArr = DashBoardService.GenerateFiles(db, start, end);
            fileBytesArr.Add(datastring, filename);


            using (var memoryStream = new MemoryStream())
            {
                using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var (fileBytes, fileName) in fileBytesArr)
                    {
                        var zipEntry = zipArchive.CreateEntry(fileName);

                        using (var entryStream = zipEntry.Open())
                        {
                            entryStream.Write(fileBytes, 0, fileBytes.Length);
                        }
                    }
                }

                memoryStream.Seek(0, SeekOrigin.Begin);
                return new FileContentResult(memoryStream.ToArray(), "application/zip")
                {
                    FileDownloadName = $"ZJ_ACCBOX_{start.ToShortDateString()}~{end.ToShortDateString()}_生产情况.zip"
                };
            }
        }
        public JsonResult GetAlarmDetails(string EQID, string week)
        {
            DataContext db = (DataContext)Wtm.DC;

            List<AlarmItem> alarmList = new();

            DateTime startdate = db.EquipmentParamsHistoryCalculate
               .Where(it => it.Name == "starttime"
               && it.Remark == week
               && it.EQID == "ACCBOX").ToList()
               .OrderBy(it => Convert.ToDateTime(it.Value)).Select(it => Convert.ToDateTime(it.Value)).First();
            DateTime enddate = db.EquipmentParamsHistoryCalculate
                .Where(it => it.Name == "endtime"
                && it.Remark == week
                && it.EQID == "ACCBOX").ToList()
                .OrderByDescending(it => Convert.ToDateTime(it.Value)).Select(it => Convert.ToDateTime(it.Value)).First();

            alarmList = DashBoardService.GetAlarmDetails(db, startdate, enddate, EQID);


            var alarmtotal = alarmList.GroupBy(
                    it => new
                    {
                        //Date = it.Date.Split(' ')[0],
                        EQID = it.EQID,
                        AlarmCode = it.AlarmCode,
                        AlarmText = it.AlarmText
                    }).Select(it => new
                    {
                        //Date = it.Key.Date,
                        EQID = it.Key.EQID,
                        AlarmCode = it.Key.AlarmCode,
                        AlarmText = it.Key.AlarmText,
                        Duration = it.Sum(x => x.Duration).ToString("0.00")
                    }).ToList();
            var totalcodelist = alarmtotal.Select(it => it.AlarmCode).ToList();
            var totalcodetext = alarmtotal.Select(it => new { AlarmText = it.AlarmText, AlarmCode = it.AlarmCode }).ToList();
            var totalalarmtime = alarmtotal.Select(it => it.Duration).ToList();
            var totalalarmpie = alarmtotal.Select(it => new { name = it.AlarmCode, value = it.Duration }).ToList();
            //Dictionary<string, string> ListToDictionary = totalalarmpie.ToDictionary(name => name.AlarmCode, value => value.Duration);
            //Dictionary<string, string> totalalarmdic = totalalarmpie.ToDictionary(key => key.AlarmCode, Duration => Duration);
            var totalalarm = new { totalcodelist, totalcodetext, totalalarmtime, totalalarmpie };
            return Json(new { alarmList = alarmList.OrderBy(it => Convert.ToDateTime(it.Date)), alarmtotal = totalalarm });
        }

        public JsonResult GetStatusDetails(string week)
        {
            //DataContext db = (DataContext)Wtm.DC;
            //var eqtype = db.EquipmentTypes.Where(it => it.Name == "ACCBOX").Select(it => it.ID).ToList();
            //var eqpidids = DC.Set<Equipment>().Where(it => eqtype.Contains((Guid)it.EquipmentTypeId)).Select(it => it.EQID).ToList();
            //List<DetailChartData> chartData = new();
            //var EQID = "MAIN2";
            //chartData = DashBoardService.GetChartData(db, EQID, week);
            //for (int i = 0; i < eqpidids.Count; i++)
            //{
            //    var EQID = eqpidids[i];
            //    chartData = DashBoardService.GetChartData(db, EQID, week);
            //}
            DataContext db = (DataContext)Wtm.DC;
            string sql = @"SELECT VALUE,UPDATETIME FROM (select EQID,NAME,VALUE,UPDATETIME,LAG(VALUE) OVER(PARTITION BY EQID ORDER BY UPDATETIME) AS PRIOR_VALUE,
LEAD(VALUE) OVER(PARTITION BY EQID ORDER BY UPDATETIME) AS NEXT_VALUE
FROM ( SELECT * FROM EQUIPMENTPARAMSHISRAW WHERE EQID = 'CONTI_001' ORDER BY UPDATETIME) )
WHERE VALUE<> PRIOR_VALUE OR PRIOR_VALUE IS NULL
ORDER BY EQID,UPDATETIME";
            var test = OracleHelper.GetDTFromCommand(sql);
            for (int i = 0; i < test.Rows.Count; i++)
            {
                var row = test.Rows[i];

            }
            //var contiList = db.EquipmentParamsHistoryRaw.Where(it => it.EQID == "CONTI_001" && it.Name == "capacity").OrderByDescending(it => it.UpdateTime).ToList();
            return Json(new { });
        }

        public JsonResult SetUPD(string target)
        {
            //User set the start time and end time firstly.
            DataContext db = (DataContext)Wtm.DC;
            //var eqptypeids = db.EquipmentTypes.Where(it => it.Name == "ACCBOX").Select(it => it.ID).ToList();
            string sql = string.Format(@"UPDATE EQUIPMENTTYPECONFIGURATION SET CONFIGURATIONVALUE = '{0}' WHERE CONFIGURATIONITEM ='ACCUPD'", target);
            db.RunSQL(sql);
            db.SaveChanges();
            return Json(new { });

            //JsonResult result = CalculateAlarmRate(eqpidids, starttime, endtime, w, idleduration);



        }
        public JsonResult SetAlarmRateTarget(string target)
        {
            //User set the start time and end time firstly.
            DataContext db = (DataContext)Wtm.DC;
            //var eqptypeids = db.EquipmentTypes.Where(it => it.Name == "ACCBOX").Select(it => it.ID).ToList();
            string sql = string.Format(@"UPDATE EQUIPMENTTYPECONFIGURATION SET CONFIGURATIONVALUE = '{0}' WHERE CONFIGURATIONITEM ='ACCAlarmRateTarget'", target);
            db.RunSQL(sql);
            db.SaveChanges();
            return Json(new { });

            //JsonResult result = CalculateAlarmRate(eqpidids, starttime, endtime, w, idleduration);



        }

        public JsonResult GetStationProfile(string EQID)
        {

            var rtdata = DC.Set<EquipmentParamsRealtime>().Where(it => it.EQID == EQID).ToList();
            var target = DC.Set<EquipmentTypeConfiguration>().Where(it => it.ConfigurationItem == "ACCUPD").Select(it => it.ConfigurationValue);
            var status = DC.Set<V_EquipmentStatus>().Where(it => it.EQID == EQID).Single();

            return Json(new { rtdata, target, status });

        }

        public JsonResult GetStationAlarms(string EQID, string datetime)
        {
            var date = DateTime.Now;
            DataContext db = (DataContext)Wtm.DC;
            //var date = Convert.ToDateTime(datetime).Date;
            //var eqguid = DC.Set<Equipment>().Where(it => it.EQID == EQID).Select(it => it.ID).Single();
            //var alarmcodeList = db.EquipmentConfigurations.Where(it => it.EQIDId == eqguid).Select(it => it.ConfigurationValue).First();
            //String[] alarmcodes = alarmcodeList.Split(",");

            //var alarmdata_old = DC.Set<EquipmentAlarm>()
            //    .Where(it => it.AlarmEqp.ToUpper() == EQID
            //    && it.AlarmTime > date.AddHours(-0.2))
            //    .OrderByDescending(it => it.AlarmTime)
            //    .Select(it => new { it.AlarmTime, it.AlarmCode, it.AlarmText })
            //    .GroupBy(it => it.AlarmCode)
            //    .Select(group => new { ALARMTIME = group.FirstOrDefault().AlarmTime, ALARMCODE = group.FirstOrDefault().AlarmCode, ALARMTEXT = group.FirstOrDefault().AlarmText })
            //    .ToList();
            //var positiondata = DC.Set<AlarmPositions>()
            //    .Where(it => it.EQID == EQID).ToList();
            //var alarmdata = DC.Set<EquipmentAlarm>()
            //    .Where(it => it.AlarmEqp.ToUpper() == EQID
            //    && it.AlarmTime > date.AddHours(-0.2))
            //    .OrderByDescending(it => it.AlarmTime)
            //    .Select(it => new { it.AlarmTime, it.AlarmCode, it.AlarmText }).FirstOrDefault();
            string sql = string.Format(@"SELECT a.EQID AS EQID,a.ALARMCODE AS AlarmCode,a.ALARMTIME AS AlarmTime,a.ALARMTEXT AS AlarmText , b.POSITIONID AS PositionID
FROM EQUIPMENTALARM a,ALARMPOSITIONS b
WHERE a.EQID = LOWER( '{0}')
AND a.ALARMTIME >= to_date('{1}','yyyy-mm-dd hh24:mi:ss')
AND a.EQID = LOWER(b.EQID)
AND a.ALARMCODE = b.ALARMCODE
AND ROWNUM <= 1
ORDER BY a.ALARMTIME DESC", EQID, date.AddHours(-1).ToString("yyyy-MM-dd HH:mm:ss"));
            var result = DashBoardService.TableToListModel<AlarmPositionsDetails>(db.RunSQL(sql));
            //var result = OracleHelper.GetDTFromCommand(sql);
            //var result = from alarm in alarmdata
            //join position in positiondata on alarm.ALARMCODE equals position.AlarmCode
            //select new { position.EQID, alarm.ALARMTEXT, alarm.ALARMCODE, alarm.ALARMTIME, position.PositionID };

            return Json(new { result }); //, result
        }
    }
}
