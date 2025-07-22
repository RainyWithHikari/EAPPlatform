using Antlr.Runtime.Misc;
using EAP.Dashboard.Models;
using EAP.Dashboard.Utils;
using EAP.Models.Database;
using OfficeOpenXml;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Microsoft.IO;
using System.Data;
using System.Configuration;
using Microsoft.Kiota.Abstractions;
using Org.BouncyCastle.Asn1.Pkcs;
using System.Runtime.InteropServices.ComTypes;
using MySqlX.XDevAPI.Common;
using System.Web.WebPages;
using System.Net.Mail;
using System.Web.UI.WebControls;
using static Org.BouncyCastle.Math.EC.ECCurve;
using Newtonsoft.Json;
using EAP.Dashboard.Models.Database;
using Microsoft.IdentityModel.Tokens;
using System.IO.Compression;

namespace EAP.Dashboard.Controllers
{

    public class EquipmentDashboardController : BaseController
    {
        //protected string roleCode => Session["RoleCode"] as string;
        // GET: EquipmentDashboardController
        public ActionResult Index()
        {
            var isDebugMode = ConfigurationManager.AppSettings["IsDebugMode"].ToUpper().ToString() == "TRUE";
            if (isDebugMode) return View();

            if (Session["RoleCode"] == null)
            {
                return RedirectToAction("Login", "Account");
                
            }
            var controllerName = ControllerContext.RouteData.Values["controller"].ToString();
            if (controllers.Contains(controllerName)) return View();

            else
            {
                // 设置错误信息
                TempData["ErrorMessage"] = "您暂时没有权限访问该看板，请联系管理员申请访问！";
               
                return RedirectToAction("index", "home");
            }
            
        }

        public ActionResult Report()
        {
            if (Session["RoleCode"] == null)
            {
                return RedirectToAction("Login", "Account");

            }

            return View();
        }

        public ActionResult Details()
        {
            if (Session["RoleCode"] == null)
            {
                return RedirectToAction("Login", "Account");

            }

            ViewBag.eqp = Request.QueryString["eqp"];

            return View();
        }

        public JsonResult GetData(string statusFilter,string typeFilter)
        {
            //var db = new DataContext()
            var db = DbFactory.GetSqlSugarClient();
            
            var roleID = db.Queryable<FrameworkRoles>().Where(it => it.RoleCode == roleCode).First();
            //var test = (string[])Session["eqpTypeIDs"];
            var eqptypeids = db.Queryable<EquipmentTypeRole>().Where(it => it.FRAMEWORKROLEID == roleID.ID).Select(it => it.EQUIPMENTTYPEID).ToList();//.Where(it => roleID.Contains(it.EQUIPMENTROLEID)).Select(it => it.EQUIPMENTTYPEID).ToList();// DC.Set<EquipmentTypeRole>().Where(it => roleids.Contains((Guid)it.FrameworkRoleId)).Select(it => it.EquipmentTypeId).ToList();
            var eqpidids = db.Queryable<Equipment>().Where(it => eqptypeids.Contains(it.EquipmentTypeId)).OrderBy(it=>it.Sort).Select(it => it.EQID).ToList();//DC.Set<Equipment>().Where(it => eqptypeids.Contains(it.EquipmentTypeId)).Select(it => it.EQID).ToList();
            if (typeFilter != null) eqpidids = db.Queryable<Equipment,EquipmentType>((e,et) => e.EquipmentTypeId == et.ID).Where((e,et) => eqptypeids.Contains(e.EquipmentTypeId) && et.Name == typeFilter).Select((e,et) => e.EQID).ToList();


            var carddata = db.Queryable<V_EquipmentStatus>().Where(it => eqpidids.Contains(it.EQID)).OrderBy(it=>it.Name).ToList();
            if (statusFilter != null) carddata = db.Queryable<V_EquipmentStatus>().Where(it => eqpidids.Contains(it.EQID) && it.Status == statusFilter).ToList();
            var typedata = carddata.GroupBy(it => it.Type).Select(it => new { name = it.Key, value = it.Count() }).OrderBy(it=>it.name).ToList();
            var statusdata = carddata.GroupBy(it => it.Status).Select(it => new { name = it.Key, value = it.Count() }).ToList();

            //10.26 Rainy Add (OEE Related)
            var oeerealtimedata = db.Queryable<EquipmentParamsRealtime>().Where(it => eqpidids.Contains(it.EQID) && it.SVID.Equals("1006")).ToList();

            var data = new
            {

                carddata = carddata,
                typedata = typedata,
                statusdata = statusdata,
                oeedata = oeerealtimedata
            };
            return Json(data);

        }

        public ActionResult RefreshSession()
        {
            // 访问会话以刷新会话时间
            Session["LastRefreshTime"] = DateTime.Now;
            return new HttpStatusCodeResult(200); // 返回成功状态
        }

        public JsonResult GetYield(string EQID, string datetime, string duration)
        {
            var db = DbFactory.GetSqlSugarClient();
            var date = Convert.ToDateTime(datetime).Date;
            switch (duration)
            {
                case "by date":
                    var dashboarddata = db.Queryable<EquipmentParamsHistoryRaw>().Where(it => it.EQID == EQID && it.UpdateTime >= date.AddDays(-7)).OrderBy(it => it.UpdateTime);
                    break;
            }

            return Json(date);

        }


        public JsonResult GetEqpAlarmDetails(int page, int limit, string EQID, string startdate, string enddate)
        {
            var db = DbFactory.GetSqlSugarClient();
            var start = Convert.ToDateTime(startdate).Date;
            var end = Convert.ToDateTime(enddate).Date;
            if(startdate == enddate)
            {
                start = start.AddHours(9);  
                end = end.AddDays(1).AddHours(9);

            }
            else
            {
                start = start.AddHours(9);
                end = end.AddHours(9);
            }
           
            var alarmList = DashBoardService.GetAlarmDetails(start, end, EQID,9)
                .OrderBy(it => it.StartTime)
                .Select(it => new { START = it.StartTime, END = it.EndTime, ALARMCODE = it.AlarmCode, ALARMTEXT = it.AlarmText, Duration = it.Duration }).ToList();

            var data = db.Queryable<EquipmentAlarm>().Where(it => it.AlarmEqp == EQID && it.AlarmTime > start && it.AlarmTime <= end).OrderByDescending(it => it.AlarmTime)
                .Select(it => new { ALARMTIME = it.AlarmTime, ALARMCODE = it.AlarmCode, ALARMTEXT = it.AlarmText }).ToList();
            var pageList = alarmList.Skip((page - 1) * limit).Take(limit).ToList();

            return Json(new { data = pageList, code = 0, count = alarmList.Count }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDetailsData( string equipment, string datetime)
        {
            var db = DbFactory.GetSqlSugarClient();
            var date = Convert.ToDateTime(datetime).Date;
            var mtbadata = db.Queryable<HistoryMTBAResult>().Where(it => it.EQID == equipment && it.DataTime.Date >= date.AddDays(-14) && it.DataTime.Date <= date).OrderBy(it => it.DataTime).ToList();
            var runratedata = db.Queryable<HistoryRunrateResult>().Where(it => it.EQID == equipment && it.DataTime.Date >= date.AddDays(-14) && it.DataTime.Date <= date).OrderBy(it => it.DataTime).ToList();

            var parameterdata = db.Queryable<EquipmentParamsRealtime>().Where(it => it.EQID == equipment).ToList();
            var status = db.Queryable<EquipmentRealtimeStatus>().Where(it => it.EQID == equipment).First();
            var runrate = db.Queryable<RunrateResult>().Where(it => it.EQID == equipment).First().RunrateValue;

            if (date != DateTime.Now.Date)
            {
                runrate = runratedata.Where(it => it.DataTime.Date == date).Select(it => it.RunrateValue).First();//?.RunrateValue ?? 0.0;
            }


            // for trend chart
            var dates = runratedata.Select(it => it.DataTime.ToString("yyyy-MM-dd")).ToList();
            var mtbas = mtbadata.Select(it => it.MTBAValue.ToString("0.00")).ToList();
            var runrates = runratedata.Select(it => (it.RunrateValue * 100).ToString("0.00")).ToList();

            //10.26 Rainy Add (OEE Related)
            //var oeedata = new { realtimedata = oeerealtimedata, historydata = oeehistorydata };
            var trenddata = new { dates = dates, mtbas = mtbas, runrates = runrates };

            var alarmdata = db.Queryable<EquipmentAlarm>().Where(it => it.AlarmEqp == equipment && it.AlarmTime > date.AddHours(9) && it.AlarmTime <= date.AddDays(1).AddHours(9)).OrderByDescending(it => it.AlarmTime)
                .Select(it => new { ALARMTIME = it.AlarmTime, ALARMCODE = it.AlarmCode, ALARMTEXT = it.AlarmText }).ToList();
            
            var chartdata = DashBoardService.GetChartData( equipment, datetime);
            //oeehistorydata
            return Json(new { trenddata, alarmdata, chartdata, parameterdata, status, runrate = (runrate * 100).ToString("0.00") });

        }

        public FileContentResult DownloadReportData(List<string> eqps, string datetime, string starttime, List<string> reports)//FileContentResult
        {
            var db = DbFactory.GetSqlSugarClient();
            var datePreFix = Convert.ToDateTime(datetime).ToString("yyyyMMdd");

            var targetDate = Convert.ToDateTime(datetime);
            var startdate = starttime.IsNullOrEmpty() ? targetDate.AddDays(-7) : Convert.ToDateTime(starttime);
            int cyclingTimes = (int)(targetDate - startdate).TotalDays;
            if (cyclingTimes == 0) cyclingTimes++;
            // 创建一个临时目录
            string tempDirectory = Path.Combine("\\\\127.0.0.1\\e$\\tmp\\EquipmentReports", Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDirectory);

            foreach (var eqp in eqps)
            {
                string subDirectory = Path.Combine(tempDirectory, eqp);
                Directory.CreateDirectory(subDirectory);

                foreach (var report in reports)
                {
                    DashBoardService.GenerateReport(eqp, targetDate, cyclingTimes, report, subDirectory, 9);
                }


            }

            // 创建一个内存流来存储压缩文件
            using (var memoryStream = new MemoryStream())
            {
                // 创建一个新的zip存档
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    // 添加临时目录中的所有文件和子目录到zip存档中
                    AddDirectoryToZip(archive, tempDirectory, string.Empty);
                }

                // 重置内存流的位置
                memoryStream.Position = 0;

                // 返回文件内容作为FileResult

                return new FileContentResult(memoryStream.ToArray(), "application/zip")
                {
                    FileDownloadName = $"{datePreFix}_报表.zip"
                };
            }

        }


        public FileContentResult ExportStatusData(string EQID, string datetime)
        {
            var db = DbFactory.GetSqlSugarClient();

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
                    duration = timeSpan.TotalMinutes.ToString("0.000"),
                });

            }
          
            //DataTable details = DashBoardService.ToDataTable(chardata);
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            var filename = EQID + "_" + datetime + "_" + $"StatusData.xlsx";
            using (ExcelPackage ep = new ExcelPackage())
            {
               
                ExcelWorkbook wb = ep.Workbook;
                ExcelWorksheet ws = wb.Worksheets.Add("Details");
                ws.Cells["A1"].LoadFromDataTable(DashBoardService.ToDataTable(chardata), true);
                ws.Cells["D1"].Value = "duration (min)";

                //ws.Column(1).Style.Numberformat.Format = "yyyy/m/d";
                ws.Column(2).Style.Numberformat.Format = "yyyy/m/d h:mm:ss";
                ws.Column(3).Style.Numberformat.Format = "yyyy/m/d h:mm:ss";
                ws.Column(4).Style.Numberformat.Format = "0.000";


                //var filename = EQID + "_" + datetime + "_" + $"StatusData.xlsx";
                var datastring = ep.GetAsByteArray();
                var result = File(datastring, "application/x-xls", filename);






                return result;
            }
            //ExcelPackage ep = new ExcelPackage();

        }
    
        public FileContentResult ExportAlarmData(string EQID, string starttime, string endtime)
        {
            var db = DbFactory.GetSqlSugarClient();

            var alarmList = new List<AlarmItem>();
            var start = Convert.ToDateTime(starttime).Date;
            var end = Convert.ToDateTime(endtime).Date;
            if (starttime == endtime)
            {
                start = start.AddHours(9);
                end = end.AddDays(1).AddHours(9);

            }
            else
            {
                start = start.AddHours(9);
                end = end.AddHours(9);
            }
            alarmList = DashBoardService.GetAlarmDetails(start, end, EQID,9);
            var alarmtotal = alarmList.GroupBy(
               it => new
               {
                   Date = it.AlarmDate,
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

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            var filename = EQID + "_" + start.ToShortDateString()+"~"+ end.ToShortDateString() + "_" + $"AlarmData.xlsx";
            using (ExcelPackage ep = new ExcelPackage())
            {
                DataTable total = DashBoardService.ToDataTable(alarmtotal);
                DataTable details = DashBoardService.ToDataTable(alarmList);
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


                //var filename = EQID + "_" + datetime + "_" + $"StatusData.xlsx";
                var datastring = ep.GetAsByteArray();
                var result = File(datastring, "application/x-xls", filename);

                return result;
            }
        }

        public JsonResult GetReportDevices()
        {
            var db = DbFactory.GetSqlSugarClient();
            var data = db.Queryable<NEEDREPORTDEVICE>()
                .Select(it=> new
                {
                    name = it.EQID,
                    value = it.REMARK ?? it.EQID,
                    selected = false
                }).ToList();
            return Json(new { data });
        }

        public JsonResult GetDeviceList()
        {
            var db = DbFactory.GetSqlSugarClient();
            var data = db.Queryable<NEEDREPORTDEVICE>().ToList();
            return Json(new { data = data, code = 0, count = data.Count }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEmailAddressList()
        {
            var db = DbFactory.GetSqlSugarClient();
            var data = db.Queryable<FrameworkUsers>()
                .Where(it=>it.Email != null)
                .Select(it => new
                {
                    it.ITCode,
                    it.Name,
                    it.Email
                }).ToList();
            return Json(new { data = data, code = 0, count = data.Count }, JsonRequestBehavior.AllowGet);
        }

        public FileContentResult DownloadDeviceReport(string startday, int starthour, string endday, int endhour, List<XmSelectModel> devices)
        {
            var startDate = Convert.ToDateTime(startday);
            var startTime = startDate.AddHours(starthour);
            var endTime = Convert.ToDateTime(endday).AddHours(endhour);

            var fileBytes = CreateGivenTimeReport(devices, startDate, startTime, endTime);
            var filename = startTime.ToString("yyyyMMddHH") + "-" + endTime.ToString("yyyyMMddHH") + "状态数据报表.xlsx";
            return File(fileBytes, "application/mx-excel", filename);
        }

        private byte[] CreateGivenTimeReport(List<XmSelectModel> devices, DateTime startDate, DateTime startTime, DateTime endTime)
        {
            var device_ids = devices.Select(it => it.value).ToList();
            var device_names = devices.Select(it => it.name).ToList();

            var db = DbFactory.GetSqlSugarClient();
            string sql = string.Format(@"SELECT EQID, STATUS, DATETIME
FROM(SELECT EQID, EQTYPE, STATUS, DATETIME,
LAG(STATUS) OVER(PARTITION BY EQID ORDER BY DATETIME) AS PRIOR_STSTUS,
LEAD(STATUS) OVER(PARTITION BY EQID ORDER BY DATETIME) AS NEXT_STATUS
FROM(SELECT * FROM EQUIPMENTSTATUS
WHERE EQID IN ('{0}')
AND DATETIME > = to_date('{1}','yyyy-mm-dd hh24:mi:ss')
AND DATETIME < = to_date('{2}','yyyy-mm-dd hh24:mi:ss')
AND STATUS <> 'no difine'
ORDER BY DATETIME)
)
WHERE STATUS<> PRIOR_STSTUS OR PRIOR_STSTUS IS NULL
ORDER BY EQID,DATETIME", string.Join("','", device_ids), startTime.ToString("yyyy-MM-dd HH:mm:ss"), endTime.ToString("yyyy-MM-dd HH:mm:ss"));
            var statusData = db.SqlQueryable<DeviceStatusModel>(sql).ToList();

            //产出代表EQID——SX没有output数据
            //            string sql_output = string.Format(@"SELECT EQ_ID, EVENT_OCCUR_DATE, SUM(OEE_CNT) OEE_CNT FROM
            //(SELECT a.OEE_CNT,a.EVENT_OCCUR_DATE,b.EQID EQ_ID FROM
            //(SELECT EQ_ID,OEE_CNT,EVENT_OCCUR_DATE FROM USI_MONITOR.GET_STATION_OEE
            //WHERE EVENT_OCCUR_DATE > TO_DATE('{0}', 'yyyy-MM-dd HH24:mi:ss')
            //AND EVENT_OCCUR_DATE < TO_DATE('{1}', 'yyyy-MM-dd HH24:mi:ss')
            //AND EQ_ID IN (SELECT NVL(OUTPUTEQID, EQID) OUTPUTEQID FROM NEEDREPORTDEVICE)
            //) a
            //LEFT JOIN
            //(SELECT NVL(OUTPUTEQID, EQID) OUTPUTEQID, EQID FROM NEEDREPORTDEVICE) b
            //ON a.EQ_ID = b.OUTPUTEQID
            //)
            //GROUP BY EQ_ID, EVENT_OCCUR_DATE", startTime.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss"), endTime.ToString("yyyy-MM-dd HH:mm:ss"));
            //var outputData = db.SqlQueryable<GET_STATION_OEE>(sql_output).ToList();

            //计算每天的百分比
            //var url = "http://192.168.150.193:8085/MES_WEB_API/api/mesapi";
            var url = "http://10.5.1.227:54188/MES_WEB_API/api/mesapi";
            var devices_info = db.Queryable<NEEDREPORTDEVICE>().ToList();
            var sfis_stations = devices_info.Where(it => it.OUTPUTSTATION != null && device_names.Contains(it.EQID)).Select(it => it.OUTPUTSTATION).Distinct().ToList();
            List<DailyOutputDataModel> outputRawData = new List<DailyOutputDataModel>();
            List<StatusRateModel> rateList = new List<StatusRateModel>();
            for (int i = 0; startTime.AddDays(i) < endTime; i++)
            {
                var _startday = startTime.AddDays(i);
                var _endday = _startday.AddDays(1);
                if(_endday < endTime) _endday = endTime;

                string sql_raw_status = string.Format(@"SELECT EQID, STATUS, DATETIME, NVL(LEAD(DATETIME) OVER (PARTITION BY EQID ORDER BY DATETIME), TO_DATE('{2}', 'yyyy-mm-dd hh24:mi:ss')) AS NEXT_STSTUS_DATETIME
FROM
(SELECT EQID, STATUS, DATETIME
FROM (SELECT EQID, STATUS, DATETIME, 
LAG(STATUS) OVER (PARTITION BY EQID ORDER BY DATETIME) AS PRIOR_STSTUS
FROM (SELECT * FROM EQUIPMENTSTATUS
WHERE EQID IN ('{0}')
AND DATETIME >= to_date('{1}','yyyy-mm-dd hh24:mi:ss')
AND DATETIME <= to_date('{2}','yyyy-mm-dd hh24:mi:ss')
AND STATUS <> 'no difine'
ORDER BY EQID, DATETIME)
)
WHERE STATUS <> PRIOR_STSTUS OR PRIOR_STSTUS IS NULL
ORDER BY EQID, DATETIME)", string.Join("','", device_ids), _startday.ToString("yyyy-MM-dd HH:mm:ss"), _endday.ToString("yyyy-MM-dd HH:mm:ss"));
                var raw_data = db.SqlQueryable<StatusDurationModel>(sql_raw_status).ToList();

                var cal_data = raw_data.GroupBy(it => new { it.EQID, it.STATUS })
                    .Select(group => new StatusRateModel()
                    {
                        EQID = group.Key.EQID,
                        STATUSNAME = group.Key.STATUS,
                        DATE = _startday.Date,
                        RATE = group.Sum(it => it.DURATION) / raw_data.Where(it=>it.EQID == group.Key.EQID).Sum(it => it.DURATION)
                    }).ToList();

                rateList.AddRange(cal_data);

                //webapi查output
                foreach (var st in sfis_stations)
                {
                    var parasdata = new OutputRequestModel()
                    {
                        Condition = new RequestInfoModel()
                        {
                            FUNCTION = "GET_STATION_OEE",
                            GROUP_NAME = st,
                            FROM_TIME = _startday.ToString("yyyy-MM-dd HH:mm:ss"),
                            END_TIME = _endday.ToString("yyyy-MM-dd HH:mm:ss")
                        }
                    };
                    string bodytext = JsonConvert.SerializeObject(parasdata);
                    WebApiHelper.PostWithProxy(url, bodytext, out string result, out string messge);
                    if (string.IsNullOrEmpty(messge))
                    {
                        RequestResultModel returnResult = JsonConvert.DeserializeObject<RequestResultModel>(result);
                        if (returnResult != null && returnResult.RESULT != null && returnResult.RESULT.RES_DATA != null && returnResult.RESULT.RES_DATA.Count > 0)
                        {
                            var daily_output = returnResult.RESULT.RES_DATA.Select(it => new DailyOutputDataModel(){
                                NO = it.NO,
                                STATION_NAME = it.STATION_NAME,
                                MODEL_NAME = it.MODEL_NAME,
                                FAMILY_CODE = it.FAMILY_CODE,
                                OEE_CNT = it.OEE_CNT,
                                DATE = _startday.Date
                            }).ToList();
                            outputRawData.AddRange(daily_output);
                        }
                    }
                }

            }

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (ExcelPackage package = new ExcelPackage())
            {
                foreach (var dv in devices)
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(dv.name);
                    worksheet.Cells[1, 1].Value = "Date";
                    worksheet.Cells[1, 2].Value = "Time";
                    worksheet.Cells[1, 3].Value = "Status";
                    worksheet.Cells[1, 4].Value = "Time Slot";
                    worksheet.Cells[1, 1, 1, 4].Style.Font.Name = "Calibri Light";
                    worksheet.Cells[1, 1, 1, 4].Style.Font.Size = 12;
                    worksheet.Cells[1, 1, 1, 4].Style.Font.Bold = true;

                    int row = 2;
                    var device_status = statusData.Where(it => it.EQID == dv.value).ToList();
                    foreach( var st in device_status)
                    {
                        worksheet.Cells[row, 1].Value = st.DATETIME.ToString("yyyy/MM/dd");
                        worksheet.Cells[row, 2].Value = st.DATETIME.ToString("HH:mm");
                        worksheet.Cells[row, 3].Value = st.STATUS;
                        worksheet.Cells[row, 4].Formula = $"=IF((A{row+1}-A{row}+B{row+1}-B{row})*24<=0,0,(A{row + 1}-A{row}+B{row + 1}-B{row})*24)";
                        row++;
                    }

                    int split_row = row++;
                    worksheet.Cells[row, 1].Value = "Item";
                    worksheet.Cells[row, 2].Value = "Time/H";
                    worksheet.Cells[row, 3].Value = "Rate";
                    worksheet.Cells[row, 1, row, 3].Style.Font.Name = "Calibri Light";
                    worksheet.Cells[row, 1, row, 3].Style.Font.Size = 12;
                    worksheet.Cells[row, 1, row, 3].Style.Font.Bold = true;

                    worksheet.Cells[row + 1, 1].Value = "Run";
                    worksheet.Cells[row + 1, 2].Formula = $"SUMIF(C{2}:C{split_row - 1},A{row + 1},D{2}:D{split_row - 1})";
                    worksheet.Cells[row + 1, 3].Formula = $"=B{row + 1}/B{row + 5}";
                    worksheet.Cells[row + 2, 1].Value = "Down";
                    worksheet.Cells[row + 2, 2].Formula = $"SUMIF(C{2}:C{split_row - 1},A{row + 2},D{2}:D{split_row - 1})";
                    worksheet.Cells[row + 2, 3].Formula = $"=B{row + 2}/B{row + 5}";
                    worksheet.Cells[row + 3, 1].Value = "Idle";
                    worksheet.Cells[row + 3, 2].Formula = $"SUMIF(C{2}:C{split_row - 1},A{row + 3},D{2}:D{split_row - 1})";
                    worksheet.Cells[row + 3, 3].Formula = $"=B{row + 3}/B{row + 5}";
                    worksheet.Cells[row + 4, 1].Value = "Alarm";
                    worksheet.Cells[row + 4, 2].Formula = $"SUMIF(C{2}:C{split_row - 1},A{row + 4},D{2}:D{split_row - 1})";
                    worksheet.Cells[row + 4, 3].Formula = $"=B{row + 4}/B{row + 5}";

                    worksheet.Cells[row + 5, 1].Value = "Total";
                    worksheet.Cells[row + 5, 2].Formula = string.Format("SUBTOTAL(9,{0})", new ExcelAddress(row + 1, 2, row + 4, 2).Address);
                    worksheet.Cells[row + 5, 3].Formula = string.Format("SUBTOTAL(9,{0})", new ExcelAddress(row + 1, 3, row + 4, 3).Address);
                    worksheet.Cells[row + 5, 1, row + 5, 3].Style.Font.Bold = true;
                    //百分数格式
                    worksheet.Cells[row + 1, 3, row + 5, 3].Style.Numberformat.Format = "0.00%";

                    row += 6;
                    int split_row2 = row++;
                    worksheet.Cells[row, 1].Value = "Date";
                    worksheet.Cells[row, 2].Value = "Run";
                    worksheet.Cells[row, 3].Value = "Alarm";
                    worksheet.Cells[row, 4].Value = "Down";
                    worksheet.Cells[row, 5].Value = "Idle";
                    worksheet.Cells[row, 6].Value = "SFIS Output";
                    worksheet.Cells[row, 1, row, 6].Style.Font.Name = "Calibri Light";
                    worksheet.Cells[row, 1, row, 6].Style.Font.Size = 12;
                    worksheet.Cells[row, 1, row, 6].Style.Font.Bold = true;

                    for(var day = startDate; day< endTime; day = day.AddDays(1))
                    {
                        row++;
                        worksheet.Cells[row, 1].Value = day.ToString("yyyy/MM/dd");
                        var run_rate = rateList.Where(it => it.EQID == dv.value && it.DATE == day && it.STATUSNAME == "Run").FirstOrDefault()?.RATE;
                        worksheet.Cells[row, 2].Value = run_rate != null ? run_rate : 0;
                        var alarm_rate = rateList.Where(it => it.EQID == dv.value && it.DATE == day && it.STATUSNAME == "Alarm").FirstOrDefault()?.RATE;
                        worksheet.Cells[row, 3].Value = alarm_rate != null ? alarm_rate : 0;
                        var down_rate = rateList.Where(it => it.EQID == dv.value && it.DATE == day && it.STATUSNAME == "Down").FirstOrDefault()?.RATE;
                        worksheet.Cells[row, 4].Value = down_rate != null ? down_rate : 0;
                        var idle_rate = rateList.Where(it => it.EQID == dv.value && it.DATE == day && it.STATUSNAME == "Idle").FirstOrDefault()?.RATE;
                        worksheet.Cells[row, 5].Value = idle_rate != null ? idle_rate : 0;
                        var eqid = devices_info.Where(it => it.EQID == dv.name && it.OUTPUTSTATION != null).Select(it => it.OUTPUTEQID ?? it.EQID).FirstOrDefault();
                        if (eqid != null)
                        {
                            var day_output = outputRawData.Where(it => it.STATION_NAME == eqid && it.DATE == day).Sum(it => Convert.ToInt32(it.OEE_CNT));
                            worksheet.Cells[row, 6].Value = day_output;
                        }
                        else
                        {
                            worksheet.Cells[row, 6].Value = 0;
                        }
                        //worksheet.Cells[row, 6].Value = 0;
                    }
                    //百分数格式
                    worksheet.Cells[split_row2 + 2, 2, row, 5].Style.Numberformat.Format = "0.00%";
                }

                using (MemoryStream ms = new MemoryStream())
                {
                    package.SaveAs(ms);
                    var result = ms.ToArray();
                    ms.Dispose();
                    return result;
                }
            }
        }

        public JsonResult SendReportEmail(string startday, int starthour, string endday, int endhour, List<XmSelectModel> devices)
        {
            try
            {
                var startDate = Convert.ToDateTime(startday);
                var startTime = startDate.AddHours(starthour);
                var endTime = Convert.ToDateTime(endday).AddHours(endhour);

                var fileBytes = CreateGivenTimeReport(devices, startDate, startTime, endTime);
                var filename = startTime.ToString("yyyyMMddHH") + "-" + endTime.ToString("yyyyMMddHH") + "状态数据报表.xlsx";

                using (MemoryStream ms = new MemoryStream(fileBytes))
                {
                    ms.Position = 0;
                    // 添加附件
                    Attachment attachment = new Attachment(ms, filename);

                    var subject = $"[REPORT]三色灯设备状态数据报表";
                    var mailbody = $"附件为{startday} {starthour}:00-{endday} {endhour}:00时间段 {string.Join(",", devices.Select(it => it.name))}的状态数据报表";
                    List<Attachment> attachments = new List<Attachment>() { attachment };
                    var db = DbFactory.GetSqlSugarClient();
                    var mailto = db.Queryable<FrameworkUsers>().Where(it => it.Email != null).Select(it => it.Email).ToArray();

                    bool ret = false;
                    if(mailto.Length > 0)
                    {
                        ret = EmailHelper.SendMail(mailto, subject, mailbody, attachments);
                    }
                    attachment.Dispose();
                    return Json(new { res = ret, msg = "发送失败！" });
                }
            }
            catch(Exception ex)
            {
                return Json(new { res = false, msg = ex.Message });
            }
        }

        public FileContentResult DownloadSummaryReport(string startdate, string enddate, int hour, List<XmSelectModel> devices)
        {
            var startDate = Convert.ToDateTime(startdate).AddHours(hour);
            var endDate = Convert.ToDateTime(enddate).AddHours(hour);
            var device_ids = devices.Select(it => it.value).ToList();
            var device_names = devices.Select(it => it.name).ToList();

            var db = DbFactory.GetSqlSugarClient();
            var raw_staion_dev = db.Queryable<NEEDREPORTDEVICE>()
                .Where(it => device_names.Contains(it.EQID))
                .OrderByDescending(it => it.STATION)
                .ToList();
            var station_devices = raw_staion_dev.GroupBy(it => it.STATION)
                .Select(group=> new
                {
                    STATION = group.Key,
                    EQIDS = group.Select(it=> new { it.EQID, EAPDevid = it.REMARK ?? it.EQID }).ToList()
                }).ToList();

            string sql_raw_status = string.Format(@"SELECT EQID, STATUS, DATETIME, NVL(LEAD(DATETIME) OVER (PARTITION BY EQID ORDER BY DATETIME), TO_DATE('{2}', 'yyyy-mm-dd hh24:mi:ss')) AS NEXT_STSTUS_DATETIME
FROM
(SELECT EQID, STATUS, DATETIME
FROM (SELECT EQID, STATUS, DATETIME, 
LAG(STATUS) OVER (PARTITION BY EQID ORDER BY DATETIME) AS PRIOR_STSTUS
FROM (SELECT * FROM EQUIPMENTSTATUS
WHERE EQID IN ('{0}')
AND DATETIME >= to_date('{1}','yyyy-mm-dd hh24:mi:ss')
AND DATETIME <= to_date('{2}','yyyy-mm-dd hh24:mi:ss')
AND STATUS <> 'no difine'
ORDER BY EQID, DATETIME)
)
WHERE STATUS <> PRIOR_STSTUS OR PRIOR_STSTUS IS NULL
ORDER BY EQID, DATETIME)", string.Join("','", device_ids), startDate.ToString("yyyy-MM-dd HH:mm:ss"), endDate.ToString("yyyy-MM-dd HH:mm:ss"));
            var raw_data = db.SqlQueryable<StatusDurationModel>(sql_raw_status).ToList();
            var rateList = raw_data.GroupBy(it => new { it.EQID, it.STATUS })
                    .Select(group => new StatusRateModel()
                    {
                        EQID = group.Key.EQID,
                        STATUSNAME = group.Key.STATUS,
                        RATE = group.Sum(it => it.DURATION) / raw_data.Where(it => it.EQID == group.Key.EQID).Sum(it => it.DURATION)
                    }).ToList();

            //产出代表EQID——SX没有output数据
//            string sql_output = string.Format(@"SELECT EQ_ID, SUM(OEE_CNT) OEE_CNT FROM
//(SELECT a.OEE_CNT,a.EVENT_OCCUR_DATE,b.EQID EQ_ID FROM
//(SELECT EQ_ID,OEE_CNT,EVENT_OCCUR_DATE FROM USI_MONITOR.GET_STATION_OEE
//WHERE EVENT_OCCUR_DATE >= TO_DATE('{0}', 'yyyy-MM-dd HH24:mi:ss')
//AND EVENT_OCCUR_DATE < TO_DATE('{1}', 'yyyy-MM-dd HH24:mi:ss')
//AND EQ_ID IN (SELECT NVL(OUTPUTEQID, EQID) OUTPUTEQID FROM NEEDREPORTDEVICE WHERE EQID IN ('{2}'))
//) a
//LEFT JOIN
//(SELECT NVL(OUTPUTEQID, EQID) OUTPUTEQID, EQID FROM NEEDREPORTDEVICE) b
//ON a.EQ_ID = b.OUTPUTEQID
//)
//GROUP BY EQ_ID", startDate.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss"), endDate.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss"), string.Join("','", device_names));
//var outputData = db.SqlQueryable<GET_STATION_OEE>(sql_output).ToList();

            //webapi查output
            List<ResDataModel> outputRawData = new List<ResDataModel>();
            //var url = "http://192.168.150.193:8085/MES_WEB_API/api/mesapi";
            var url = "http://10.5.1.227:54188/MES_WEB_API/api/mesapi";
            var devices_info = db.Queryable<NEEDREPORTDEVICE>().ToList();
            var sfis_stations = devices_info.Where(it => it.OUTPUTSTATION != null && device_names.Contains(it.EQID)).Select(it => it.OUTPUTSTATION).Distinct().ToList();
            foreach (var st in sfis_stations)
            {
                var parasdata = new OutputRequestModel()
                {
                    Condition = new RequestInfoModel()
                    {
                        FUNCTION = "GET_STATION_OEE",
                        GROUP_NAME = st,
                        FROM_TIME = startDate.ToString("yyyy-MM-dd HH:mm:ss"),
                        END_TIME = endDate.ToString("yyyy-MM-dd HH:mm:ss")
                    }
                };
                string bodytext = JsonConvert.SerializeObject(parasdata);
                WebApiHelper.PostWithProxy(url, bodytext, out string result, out string messge);
                if (string.IsNullOrEmpty(messge))
                {
                    RequestResultModel returnResult = JsonConvert.DeserializeObject<RequestResultModel>(result);
                    if (returnResult != null && returnResult.RESULT != null && returnResult.RESULT.RES_DATA != null && returnResult.RESULT.RES_DATA.Count > 0)
                    {
                        outputRawData.AddRange(returnResult.RESULT.RES_DATA);
                    }
                }
            }

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("sheet1");
                worksheet.Cells[1, 1].Value = "Station";
                worksheet.Cells[1, 2].Value = "EQID";
                worksheet.Cells[1, 3].Value = "Run";
                worksheet.Cells[1, 4].Value = "Down";
                worksheet.Cells[1, 5].Value = "Idle";
                worksheet.Cells[1, 6].Value = "Alarm";
                worksheet.Cells[1, 7].Value = "Total";
                worksheet.Cells[1, 8].Value = "SFIS Output";
                worksheet.Cells[1, 1, 1, 8].Style.Font.Name = "Calibri Light";
                worksheet.Cells[1, 1, 1, 8].Style.Font.Size = 12;
                worksheet.Cells[1, 1, 1, 8].Style.Font.Bold = true;

                int row = 2;
                foreach (var st in station_devices)
                {
                    worksheet.Cells[row, 1].Value = st.STATION;
                    worksheet.Cells[row, 2].Value = "All";
                    worksheet.Cells[row, 3].Formula = string.Format("SUBTOTAL(1,{0})", new ExcelAddress(row + 1, 3, row + st.EQIDS.Count, 3).Address);
                    worksheet.Cells[row, 4].Formula = string.Format("SUBTOTAL(1,{0})", new ExcelAddress(row + 1, 4, row + st.EQIDS.Count, 4).Address);
                    worksheet.Cells[row, 5].Formula = string.Format("SUBTOTAL(1,{0})", new ExcelAddress(row + 1, 5, row + st.EQIDS.Count, 5).Address);
                    worksheet.Cells[row, 6].Formula = string.Format("SUBTOTAL(1,{0})", new ExcelAddress(row + 1, 6, row + st.EQIDS.Count, 6).Address);
                    worksheet.Cells[row, 7].Formula = string.Format("SUM({0})", new ExcelAddress(row, 3, row, 6).Address);
                    worksheet.Cells[row, 8].Formula = string.Format("SUBTOTAL(9,{0})", new ExcelAddress(row + 1, 8, row + st.EQIDS.Count, 8).Address);
                    row++;
                    foreach(var eq in st.EQIDS)
                    {
                        worksheet.Cells[row, 2].Value = eq.EQID;
                        //var eap_eqid = devices.Where(it => it.name == eq).FirstOrDefault()?.value;
                        var run_rate = rateList.Where(it => it.EQID == eq.EAPDevid && it.STATUSNAME == "Run").FirstOrDefault()?.RATE;
                        worksheet.Cells[row, 3].Value = run_rate != null ? run_rate : 0;
                        var alarm_rate = rateList.Where(it => it.EQID == eq.EAPDevid && it.STATUSNAME == "Alarm").FirstOrDefault()?.RATE;
                        worksheet.Cells[row, 4].Value = alarm_rate != null ? alarm_rate : 0;
                        var down_rate = rateList.Where(it => it.EQID == eq.EAPDevid && it.STATUSNAME == "Down").FirstOrDefault()?.RATE;
                        worksheet.Cells[row, 5].Value = down_rate != null ? down_rate : 0;
                        var idle_rate = rateList.Where(it => it.EQID == eq.EAPDevid && it.STATUSNAME == "Idle").FirstOrDefault()?.RATE;
                        worksheet.Cells[row, 6].Value = idle_rate != null ? idle_rate : 0;
                        worksheet.Cells[row, 7].Formula = string.Format("SUM({0})", new ExcelAddress(row, 3, row, 6).Address);
                        var eqid = devices_info.Where(it => it.EQID == eq.EQID && it.OUTPUTSTATION != null).Select(it => it.OUTPUTEQID ?? it.EQID).FirstOrDefault();
                        if(eqid != null)
                        {
                            var day_output = outputRawData.Where(it => it.STATION_NAME == eqid).Sum(it => Convert.ToInt32(it.OEE_CNT));
                            worksheet.Cells[row, 8].Value = day_output;
                        }
                        else
                        {
                            worksheet.Cells[row, 8].Value = 0;
                        }
                        //worksheet.Cells[row, 8].Value = 0;
                        row++;
                    }
                }
                //百分数格式
                worksheet.Cells[2, 3, row, 7].Style.Numberformat.Format = "0.00%";

                using (MemoryStream ms = new MemoryStream())
                {
                    package.SaveAs(ms);
                    var result = ms.ToArray();
                    ms.Dispose();
                    var filename = "状态报表.xlsx";
                    return File(result, "application/mx-excel", filename);
                }
            }

        }

        public JsonResult UpdateReportDevices(string id, string value, string field)
        {
            try
            {
                var db = DbFactory.GetSqlSugarClient();
                var device = db.Queryable<NEEDREPORTDEVICE>().Where(it => it.ID == id).First();
                if (device == null)
                {
                    return Json(new { res = false, msg = "未查到数据，请刷新重试！" });
                }

                switch (field)
                {
                    case "STATION":
                        device.STATION = value;
                        break;
                    case "EQID":
                        device.EQID = value;
                        break;
                    case "OUTPUTEQID":
                        device.OUTPUTEQID = value;
                        break;
                    case "OUTPUTSTATION":
                        device.OUTPUTSTATION = value;
                        break;
                    case "REMARK":
                        device.REMARK = value;
                        break;
                }

                var ret = db.Updateable(device).ExecuteCommand();

                return Json(new { res = ret > 0, msg = "更新失败！" });
            }
            catch (Exception ex)
            {
                return Json(new { res = false, msg = ex.Message });
            }
        }

        public JsonResult AddReportDevices()
        {
            try
            {
                var eqid = Request["EQID"];
                var station = Request["STATION"];
                var sfisstation = Request["OUTPUTSTATION"];
                var outputeqid = Request["OUTPUTEQID"];
                var remark = Request["REMARK"];

                var db = DbFactory.GetSqlSugarClient();
                NEEDREPORTDEVICE device = new NEEDREPORTDEVICE()
                {
                    ID = Guid.NewGuid().ToString("N"),
                    EQID = eqid,
                    STATION = station,
                    OUTPUTSTATION = sfisstation,
                    OUTPUTEQID = outputeqid,
                    REMARK = remark
                };
                var row = db.Insertable(device).ExecuteCommand();
                return Json(new { res = row > 0, msg = "新增失败！" });
            }
            catch (Exception ex)
            {
                return Json(new { res = false, msg = ex.Message });
            }
        }

        public JsonResult DeleteDevices(List<string> list)
        {
            try
            {
                var db = DbFactory.GetSqlSugarClient();
                var rows = db.Deleteable<NEEDREPORTDEVICE>().Where(it => list.Contains(it.ID)).ExecuteCommand();
                return Json(new { res = rows > 0, msg = "删除失败！" });
            }
            catch (Exception ex)
            {
                return Json(new { res = false, msg = ex.Message });
            }
        }
    }
}
