using EAP.Dashboard.Models;
using EAP.Dashboard.Utils;
using EAP.Models.Database;
using Microsoft.IdentityModel.Tokens;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Data;
using OfficeOpenXml.Core.ExcelPackage;
using OfficeOpenXml;
using ExcelPackage = OfficeOpenXml.ExcelPackage;
using ExcelWorkbook = OfficeOpenXml.ExcelWorkbook;
using ExcelWorksheet = OfficeOpenXml.ExcelWorksheet;
using Microsoft.Ajax.Utilities;
using System.Web.DynamicData;
using System.Configuration;

namespace EAP.Dashboard.Controllers
{
    public class ETechDashboardController : BaseController
    {
        public ActionResult Index()
        {
            var isDebugMode = ConfigurationManager.AppSettings["IsDebugMode"].ToUpper().ToString() == "TRUE";
            if (isDebugMode) return View();

            return View();
        }
        public ActionResult setWeek()
        {
            return View();
        }

        public ActionResult setTarget()
        {
            return View();
        }
        public ActionResult alarmdetails()
        {
            return View();
        }
        public ActionResult DownloadReports()
        {

            return View();

        }
        public JsonResult GetEQPList()
        {
            var db = DbFactory.GetSqlSugarClient();

            var eqTypeId = db.Queryable<EquipmentType>().Where(it => it.Name == "ETECH")?.First().ID;
            var eqList = db.Queryable<Equipment>().Where(it => it.EquipmentTypeId == eqTypeId).OrderBy(it => it.EQID).ToList();
            string option = "";

            foreach (var item in eqList)
            {
                option += "<option value=\"" + item.EQID + "\" >" + item.Name + "</option>";
                //checkbox += $"<input type=\"checkbox\" name=\"like[{item}]\" title=\"{item.Replace("EQAMS0000", string.Empty)}号机\">" ;
            }
            return Json(new { eqList = eqList.Select(it => it.EQID), option, defaultoption = eqList.ElementAt(0) });
        }
        public JsonResult GetStatus()
        {
            var db = DbFactory.GetSqlSugarClient();
            var eqtype = db.Queryable<EquipmentType>().Where(it => it.Name == "ETECH").Select(it => it.ID).ToList();
            var eqpidids = db.Queryable<Equipment>().Where(it => eqtype.Contains(it.EquipmentTypeId)).OrderBy(it => it.EQID).Select(it => it.EQID).ToList();
            var status = db.Queryable<V_EquipmentStatus>().Where(it => eqpidids.Contains(it.EQID)).ToList();
            var statuslist = new List<V_EquipmentStatus>();
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
            
            return Json(new { eqpProfiles });
        }

        public JsonResult GetEQPRealtimeData(string EQID, string datetime)
        {
            var db = DbFactory.GetSqlSugarClient();
            var date = datetime.IsNullOrEmpty() ? DateTime.Now : Convert.ToDateTime(datetime);

            var eqptypeids = db.Queryable<EquipmentType>().Where(it => it.Name == "ETECH").Select(it => it.ID).ToList();
            // status, runrate
            var realtimeData = db.Queryable<V_EquipmentStatus>().Where(it => it.EQID == EQID)?.First();

            var realtimeOutput = db.Queryable<EquipmentParamsRealtime>().Where(it => it.EQID == EQID && it.Name == "output")?.First().Value;
            if (date.Date != DateTime.Today)
            {
                var outputCachedList = db.Queryable<EquipmentParamsHistoryRaw>()
                            .Where(it => it.EQID == EQID && it.Name == "output" && it.UpdateTime.Date > date.Date.AddDays(-1) && it.UpdateTime.Date < date.Date.AddDays(2))
                            .OrderBy(it => it.UpdateTime).ToList();
                var maxOutputPerDay = outputCachedList
                            .GroupBy(item =>
                            {
                                var Itemdate = item.UpdateTime.Date;
                                if (item.UpdateTime.Hour < 9)
                                {
                                    Itemdate = Itemdate.AddDays(-1);
                                }
                                return new { Itemdate, item.Name };
                            }

                            ) // 按日期分组
                            .Select(group => group.OrderByDescending(item => item.UpdateTime).First()) // 选择每个分组中值最后的项
                            .ToList()
                            .Select(it =>
                            {
                                if (it.UpdateTime.Hour < 9)
                                {
                                    return new { it.EQID, it.Name, Date = it.UpdateTime.AddDays(-1).Date, Value = it.Value };//.ToString("yy-MM-dd")
                                }
                                else
                                {
                                    return new { it.EQID, it.Name, Date = it.UpdateTime.Date, Value = it.Value };//.ToString("yy-MM-dd")
                                }
                            }
                            ).Where(it => it.Date == date.Date)?.First().Value;
                realtimeOutput = maxOutputPerDay;
            }
            var outputTarget = db.Queryable<EquipmentTypeConfiguration>().Where(it => it.TypeNameId == eqptypeids.First() && it.ConfigurationItem == "UPD").Select(it => it.ConfigurationValue)?.First();

            var startTime = date.Date.AddHours(9);
            var endTime = date.Date.AddDays(1).AddHours(9);
            var totalTime = endTime - startTime;
            if (date.Date == DateTime.Today)
            {
                if (DateTime.Now.Hour < 9)
                {
                    startTime = date.Date.AddDays(-1).AddHours(9);
                    endTime = date.Date.AddHours(9);
                }

                totalTime = DateTime.Now - startTime;

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

ORDER BY DATETIME", //EQID, DATETIME
EQID, startTime.ToString("yyyy-MM-dd HH:mm:ss"), endTime.ToString("yyyy-MM-dd HH:mm:ss"));
            var statusdata = OracleHelper.GetDTFromCommand(sql);
            // 统计 idle 状态的累计时长
            TimeSpan totalIdleTime = TimeSpan.Zero;

            // 获取所有记录，按时间排序
            var records = statusdata.AsEnumerable()
                .OrderBy(row => row.Field<DateTime>("DATETIME"))
                .ToList();

            // 记录上一个非 idle 状态的时间
            DateTime? lastNonIdleTime = null;
            DateTime? lastIdleTime = null;

            foreach (var row in records)
            {
                string status = row.Field<string>("STATUS");
                DateTime currentTime = row.Field<DateTime>("DATETIME");

                if (status == "Idle")
                {
                    lastIdleTime = currentTime;
                    // 如果上一个非 idle 时间存在，计算时间差
                    //if (lastIdleTime.HasValue)
                    //{
                    //    totalIdleTime += currentTime - lastIdleTime.Value;
                    //}
                }
                else
                {
                    // 更新上一个非 idle 状态的时间
                    // lastNonIdleTime = currentTime;
                    if (lastIdleTime.HasValue)
                    {
                        totalIdleTime += currentTime - lastIdleTime.Value;
                    }

                    lastIdleTime = null;
                }
            }
            var idleRate = totalIdleTime.TotalMinutes / totalTime.TotalMinutes;
            //是否有alarm rate 计算记录
            var record = db.Queryable<EquipmentParamsHistoryCalculate>().Where(it => it.EQID == EQID && it.Name == "alarm rate" && it.UpdateTime.Date == date.Date).First();
            if (record != null) return Json(new { realtimeData, AlarmRate = Convert.ToDouble(record.Value), realtimeOutput,outputTarget, idleRate });

         

            var alarmData = DashBoardService.GetAlarmDetails(startTime, endTime, EQID, 9);



            // 合并同一天的报警时间段
            TimeSpan totalDuration = DashBoardService.MergeAndCalculateDuration(alarmData);

            var downRate = totalDuration.TotalMinutes / totalTime.TotalMinutes;

            if (date.Date != DateTime.Today)
            {
                record = new EquipmentParamsHistoryCalculate
                {
                    EQID = EQID,
                    Name = "alarm rate",
                    Value = downRate.ToString("0.00"),
                    UpdateTime = date
                };
                db.Insertable(record).ExecuteCommand();
            }


            return Json(new { realtimeData, AlarmRate = downRate, realtimeOutput, outputTarget, idleRate });
        }
        public JsonResult GetTotalRealtimeData()
        {
            var db = DbFactory.GetSqlSugarClient();
            var eqTypeId = db.Queryable<EquipmentType>().Where(it => it.Name == "ETECH")?.First().ID;
            var eqList = db.Queryable<Equipment>().Where(it => it.EquipmentTypeId == eqTypeId)
                .OrderBy(it => it.EQID)
                .Select(it => it.EQID).ToList();

            // status, runrate
            var realtimeTotalStatus = db.Queryable<EquipmentRealtimeStatus>().Where(it => eqList.Contains(it.EQID)).ToList();

            var realtimeTotalOutput = db.Queryable<EquipmentParamsRealtime>().Where(it => eqList.Contains(it.EQID) && it.Name == "output").ToList();

            var mergeList = realtimeTotalStatus.GroupJoin(realtimeTotalOutput,
                a => a.EQID,
                b => b.EQID,
                (a, bGroup) => new
                {
                    EQID = a.EQID,
                    Status = a.Status,
                    Output = bGroup.FirstOrDefault()?.Value ?? "0"
                })
                 .Union(
                realtimeTotalOutput.
                Where(b => !realtimeTotalStatus.Any(a => a.EQID == b.EQID)).Select(b => new
                {
                    EQID = b.EQID,
                    Status = "Run",
                    Output = b.Value
                })
                ).ToList();


            return Json(new { mergeList });
        }

        public JsonResult GetEQPAlarmDetailsByDate(string EQID, string datetime)
        {
            var db = DbFactory.GetSqlSugarClient();

            var date = datetime.IsNullOrEmpty() ? DateTime.Today.AddHours(9) : Convert.ToDateTime(datetime).Date.AddHours(9);

            var eqTypeId = db.Queryable<EquipmentType>().Where(it => it.Name == "ETECH")?.First().ID;
            var eqList = db.Queryable<Equipment>().Where(it => it.EquipmentTypeId == eqTypeId)
                .OrderBy(it => it.EQID)
                .Select(it => it.EQID).ToList();

            var alarmList = db.Queryable<EquipmentAlarm>()
                .Where(it => eqList.Contains(it.AlarmEqp) && it.AlarmTime >= date && it.AlarmTime <= date.AddDays(1) && it.AlarmSet == true).ToList();

            return Json(new { alarmList = alarmList.OrderByDescending(it => it.AlarmTime) });
        }

        public FileContentResult DownloadReportData(string week)//FileContentResult
        {

            var db = DbFactory.GetSqlSugarClient();
            var eqtype = db.Queryable<EquipmentType>().Where(it => it.Name == "ETECH").Select(it => it.ID).ToList();
            var eqpidids = db.Queryable<Equipment>().Where(it => eqtype.Contains(it.EquipmentTypeId)).OrderBy(it => it.Sort).Select(it => it.EQID).ToList();

            var alarmList = new List<AlarmItem>();
            DateTime startdate = db.Queryable<EquipmentParamsHistoryCalculate>()
               .Where(it => it.Name == "starttime"
               && it.Remark == week
               && it.EQID == "ETECH").ToList()
               .OrderBy(it => Convert.ToDateTime(it.Value)).Select(it => Convert.ToDateTime(it.Value)).First();
            DateTime enddate = db.Queryable<EquipmentParamsHistoryCalculate>()
                .Where(it => it.Name == "endtime"
                && it.Remark == week
                && it.EQID == "ETECH").ToList()
                .OrderByDescending(it => Convert.ToDateTime(it.Value)).Select(it => Convert.ToDateTime(it.Value)).First();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage ep = new ExcelPackage();
            ExcelWorkbook wb = ep.Workbook;
            for (int i = 0; i < eqpidids.Count; i++)
            {
                var EQID = eqpidids[i];
                alarmList = DashBoardService.GetAlarmDetails(startdate, enddate, EQID, 9);
                DataTable details = DashBoardService.ToDataTable(alarmList);
                details.Columns.Add("CarrierID", typeof(string)).SetOrdinal(3);
                details.Columns.Add("SN", typeof(string)).SetOrdinal(4);

                // 遍历每一行并拆分列
                foreach (DataRow row in details.Rows)
                {
                    string[] parts = row["AlarmText"].ToString().Split(';');

                    row["CarrierID"] = parts.Length == 3 ? parts[0] : string.Empty;
                    row["SN"] = parts.Length == 3 ? parts[1] : string.Empty;
                    row["AlarmText"] = parts.Length == 3 ? parts[2] : row["AlarmText"].ToString();
                }

                ExcelWorksheet ws = wb.Worksheets.Add(EQID);
                ws.Cells["A1"].LoadFromDataTable(details, true);
                ws.Column(5).Style.Numberformat.Format = "yyyy/m/d h:mm:ss";
                ws.Column(6).Style.Numberformat.Format = "yyyy/m/d h:mm:ss";

            }
            var filename = "ETECH_" + week + "_" + $"AlarmData.xlsx";
            var datastring = ep.GetAsByteArray();
            return new FileContentResult(datastring, "application/x-xls")
            {
                FileDownloadName = filename
            };







        }
    }
}