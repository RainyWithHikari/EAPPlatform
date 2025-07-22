using Antlr.Runtime.Misc;
using CsvHelper;
using EAP.Dashboard.Models;
using EAP.Dashboard.Utils;
using EAP.Models.Database;
using MathNet.Numerics.LinearAlgebra.Factorization;
using Microsoft.Ajax.Utilities;
using Microsoft.Graph;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data.Common;
using System.Formats.Asn1;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using CsvHelper.Configuration;
using OfficeOpenXml;
using ExcelPackage = OfficeOpenXml.ExcelPackage;
using ExcelWorkbook = OfficeOpenXml.ExcelWorkbook;
using ExcelWorksheet = OfficeOpenXml.ExcelWorksheet;
using System.Data;
using System.Web.UI.WebControls;
using static OfficeOpenXml.ExcelErrorValue;
using System.Web.Util;
using System.Drawing;
using System.IO.Packaging;
using EAP.Models.Database.WMS;
using System.Threading.Tasks;
using System.Collections;
using static log4net.Appender.RollingFileAppender;
using OfficeOpenXml.Drawing.Chart;
using MySqlX.XDevAPI.Common;
using Newtonsoft.Json;
using Mysqlx.Crud;
using Mysqlx.Session;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;
using static System.Runtime.CompilerServices.RuntimeHelpers;



namespace EAP.Dashboard.Controllers
{
    public class WarehouseDashboardController : BaseController
    {
        private static log4net.ILog Log = log4net.LogManager.GetLogger("Logger");
        public DashBoardService serviceHandler;
        public ActionResult Index()
        {
            var isDebugMode = ConfigurationManager.AppSettings["IsDebugMode"].ToUpper().ToString() == "TRUE";
            if (isDebugMode) return View();
            return View();

        }

        public ActionResult DownloadReports()
        {

            return View();

        }

        public JsonResult GetRealtimeData()
        {
            var db = DbFactory.GetWMSClient();
            var unloadPosition = new List<string>() { "S-01", "S-03" };
            var cacheWallPosition = new List<string>() { "H-01-01-03","H-01-02-03","H-01-03-03","H-01-04-03","H-01-05-03","H-01-06-03","H-01-07-03","H-01-08-03","H-01-01-02","H-01-02-02","H-01-03-02","H-01-04-02","H-01-05-02","H-01-06-02","H-01-07-02","H-01-08-02","H-01-01-01","H-01-02-01","H-01-03-01","H-01-04-01","H-01-05-01","H-01-06-01","H-01-07-01","H-01-08-01"
 };
            var unloadCnt = db.Queryable<WMS_TASK>()
                .Where(it => it.TASK_CREATE_TIME >= DateTime.Today
                && unloadPosition.Contains(it.CTU_END_POSITION)
                //&& cacheWallPosition.Contains(it.CTU_START_POSITION)
            ).Count();

            var cacheWallCnt = db.Queryable<WMS_TASK>()
                .Where(it => it.TASK_CREATE_TIME >= DateTime.Today
                //&& unloadPosition.Contains(it.CTU_END_POSITION)
                && cacheWallPosition.Contains(it.CTU_END_POSITION)
            ).Count();
            var totalCnt = db.Queryable<WMS_TASK>()
                .Where(it => it.TASK_CREATE_TIME >= DateTime.Today
                && (unloadPosition.Contains(it.CTU_END_POSITION)
                || cacheWallPosition.Contains(it.CTU_END_POSITION))
            ).Count();

            // 7寸盘发料达成率
            double achieveRatio = totalCnt == 0 ? 0 : (double)unloadCnt / totalCnt;


            return Json(new { achieveRatio, cacheWallCnt, totalCnt, unloadCnt });
        }
        public JsonResult GetHistoryData(string datetime)
        {
            var db = DbFactory.GetWMSClient();
            var date = Convert.ToDateTime(datetime);
            var unloadPosition = new List<string>() { "S-01", "S-03" };
            var cacheWallPosition = new List<string>() { "H-01-01-03","H-01-02-03","H-01-03-03","H-01-04-03","H-01-05-03","H-01-06-03","H-01-07-03","H-01-08-03","H-01-01-02","H-01-02-02","H-01-03-02","H-01-04-02","H-01-05-02","H-01-06-02","H-01-07-02","H-01-08-02","H-01-01-01","H-01-02-01","H-01-03-01","H-01-04-01","H-01-05-01","H-01-06-01","H-01-07-01","H-01-08-01"
 };

            var result = db.Queryable<WMS_TASK>()
    .Where(it => it.TASK_CREATE_TIME >= date.AddDays(-7) && it.TASK_CREATE_TIME < date)// 查询最近7天的数据（可改）
    .GroupBy(it => it.TASK_CREATE_TIME.Date) // 按天分组
    .Select(it => new
    {
        Date = it.TASK_CREATE_TIME.Date.ToString("yy-MM-dd"), // 任务日期

        S1_Count = SqlFunc.AggregateSum(SqlFunc.IIF(it.CTU_END_POSITION == "S-01", 1, 0)), // 终点为 S-01 或 S-03
        S3_Count = SqlFunc.AggregateSum(SqlFunc.IIF(it.CTU_END_POSITION == "S-03", 1, 0)), // 终点为 S-01 或 S-03
        H_Count = SqlFunc.AggregateSum(SqlFunc.IIF(cacheWallPosition.Contains(it.CTU_END_POSITION), 1, 0))
    })
    .ToList()
    .Select(it => new
    {
        it.Date,
        //it.S_Count,
        //it.H_Count,
        Rate = ((it.S1_Count + it.S3_Count) * 1.0 / (it.S1_Count + it.S3_Count + it.H_Count))==0?1: ((it.S1_Count + it.S3_Count) * 1.0 / (it.S1_Count + it.S3_Count + it.H_Count)),// 计算达成率
        Unload1 = 1,//it.S1_Count == 0 ? 0 : 1,
        Unload2 = 1//it.S3_Count == 0 ? 0 : 1,
    })
    .ToList();


            return Json(result);
        }
        public JsonResult GetAVGSuceessRate()
        {
            var db = DbFactory.GetWMSClient();

            var result = db.Queryable<WMS_TASK>()
    .Where(t => t.TASK_CREATE_TIME >= DateTime.Today && !string.IsNullOrEmpty(t.CTU_ID) && !string.IsNullOrEmpty(t.TASK_STATUS)) // 筛选今天零点开始的任务
    .GroupBy(t => t.CTU_ID) // 按机器人编号分组
    .Select(t => new
    {
        t.CTU_ID,
        TotalTasks = SqlFunc.AggregateCount(t.TASK_ID), // 计算总任务数
        Status4Count = SqlFunc.AggregateSum(t.TASK_STATUS == "4" ? 1 : 0), // 计算状态码 4 的任务数量
        Status6Count = SqlFunc.AggregateSum(t.TASK_STATUS == "6" ? 1 : 0), // 计算状态码 6 的任务数量
    })
    .ToList() // 执行查询
    .Select(t => new
    {
        t.CTU_ID,
        t.TotalTasks,
        t.Status4Count,
        t.Status6Count,
        Ratio = t.TotalTasks == 0 ? "0%" : Math.Round((double)t.Status4Count / t.TotalTasks * 100, 2) + "%"// 计算状态 4 占比
    })
    .ToList();



            return Json(result);
        }
        public JsonResult GetAGVHistoryData(string datetime)
        {
            var db = DbFactory.GetWMSClient();
            var date = Convert.ToDateTime(datetime);
        

            var result = db.Queryable<WMS_TASK>()
    .Where(it => it.TASK_CREATE_TIME >= date.AddDays(-7) && it.TASK_CREATE_TIME < date 
    && !string.IsNullOrEmpty(it.CTU_ID) 
    && !string.IsNullOrEmpty(it.TASK_STATUS)
    && !it.CTU_ID.Contains(","))// 查询最近7天的数据（可改）
    .GroupBy(it => new { it.TASK_CREATE_TIME.Date, it.CTU_ID, it.TASK_STATUS }) // 按天分组
    .Select(it => new
    {
        Date = it.TASK_CREATE_TIME.Date.ToString("yy-MM-dd"), // 任务日期
        it.CTU_ID,
        Status4Count = SqlFunc.AggregateSum(it.TASK_STATUS == "4" ? 1 : 0), // 计算状态码 4 的任务数量
        //Status6Count = SqlFunc.AggregateSum(it.TASK_STATUS == "6" ? 1 : 0), // 计算状态码 6 的任务数量
    })
    .ToList();


            return Json(result);
        }
        public JsonResult GetCacheWallStorageA()
        {
            var db = DbFactory.GetWMSClient();

            //总库位数
            var totalStock = db.Queryable<WMS_STORAGE_POSITION>().Distinct().Count();

            //有料数
            var stockNotEmpty = db.Queryable<WMS_STORAGE_POSITION>().Where(it => it.REEL_NUM != 0).Distinct().Count();

            //空箱
            var stockEmpty = db.Queryable<WMS_STORAGE_POSITION>().Where(it => it.REEL_NUM == 0).Distinct().ToList();


            var result = db.Queryable<WMS_STORAGE_POSITION>()
                .Where(it => it.LOCATION_ZONE == "A" || it.LOCATION_ZONE == "B")
                .GroupBy(it => it.LOCATION_ZONE)
                .Select(it => new
                {
                    ZONE = it.LOCATION_ZONE,
                    EmptyPositions = SqlFunc.AggregateSum(SqlFunc.IIF(it.REEL_NUM == 0, 1, 0)),
                    NotEmptyPositions = SqlFunc.AggregateSum(SqlFunc.IIF(it.REEL_NUM != 0, 1, 0)),
                }).ToList();




            return Json(new { totalStock , stockNotEmpty, stockEmpty , result });
        }

        public class ExcelMapping
        {
            public int ExcelX { get; set; }
            public int ExcelY { get; set; }
            public string CodePrefix { get; set; } // 只存储 "A-011-12"
        }
        public JsonResult GetEQPRealtimeData(string EQID)
        {
            var db = DbFactory.GetSqlSugarClient();

            var paramList = db.Queryable<EquipmentParamsRealtime>().Where(it => it.EQID == EQID &&
           (it.Name.Contains("to") ? it.UpdateTime >= DateTime.Today.AddHours(9) : true)).ToList();
            if (DateTime.Now.Hour < 9)
            {
                paramList = db.Queryable<EquipmentParamsRealtime>().Where(it => it.EQID == EQID &&
            (it.Name.Contains("to") ? it.UpdateTime >= DateTime.Today.AddDays(-1).AddHours(9) : true)).ToList();
            }


            return Json(new { paramList });
        }
        public JsonResult GetEQPHistoryData(string EQID, string datetime)
        {
            try
            {
                using (var db = DbFactory.GetSqlSugarClient())
                {
                    Log.Debug("GetEQPHistoryData:" + EQID + "; " + datetime);
                    var date = Convert.ToDateTime(datetime);

                    var redisdb = RedisHelper.Database;
                    string cacheKey_Utilization = EQID + "_historyUtilization";
                    List<EquipmentParamsHistoryRaw> utilizationCachedList = RedisHelper.GetList<EquipmentParamsHistoryRaw>(cacheKey_Utilization);

                    string cacheKey_Output = EQID + "_historyOutput";
                    List<EquipmentParamsHistoryRaw> outputCachedList = RedisHelper.GetList<EquipmentParamsHistoryRaw>(cacheKey_Output);




                    if (date == DateTime.Today)
                    {
                        if (utilizationCachedList == null)
                        {

                            var utilizationList = db.Queryable<EquipmentParamsHistoryRaw>()
                           .Where(it => it.EQID == EQID && it.Name == "utilization" && it.UpdateTime.Date >= date.Date.AddDays(-8) && it.UpdateTime.Date < date.Date)
                           .OrderBy(it => it.UpdateTime).ToList();
                            utilizationCachedList = utilizationList;
                            RedisHelper.SetList(cacheKey_Utilization, utilizationCachedList, TimeSpan.FromHours(1));
                        }
                        if (outputCachedList == null)
                        {
                            var outputList = db.Queryable<EquipmentParamsHistoryRaw>()
                            .Where(it => it.EQID == EQID && it.Name.Contains("to") && it.UpdateTime.Date > date.Date.AddDays(-8) && it.UpdateTime.Date < date.Date)
                            .OrderBy(it => it.UpdateTime).ToList();
                            outputCachedList = outputList;
                            RedisHelper.SetList(cacheKey_Output, outputCachedList, TimeSpan.FromHours(1));
                        }
                        // 使用LINQ查询每天的最大值
                        var maxUtilizationPerDay = utilizationCachedList
                            .GroupBy(item =>

                            item.UpdateTime.Date) // 按日期分组
                            .Select(group => group.OrderByDescending(item => item.UpdateTime).First()) // 选择每个分组中值最大的项
                            .ToList()
                            .Select(it => new { Date = it.UpdateTime.Date.ToString("yy-MM-dd"), Value = it.Value }).ToList(); // 转换为列表


                        //.GroupBy(it => new {it.Name,it.Value}).Select(g => g.OrderBy(t=>t.UpdateTime).First()).ToList();
                        // 使用LINQ查询每天的最大值
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
                                    return new { it.EQID, it.Name, Date = it.UpdateTime.AddDays(-1).Date.ToString("yy-MM-dd"), Value = it.Value };
                                }
                                else
                                {
                                    return new { it.EQID, it.Name, Date = it.UpdateTime.Date.ToString("yy-MM-dd"), Value = it.Value };
                                }
                            }
                            ).ToList(); // 转换为列表 
                        var runratedata = db.Queryable<HistoryRunrateResult>().Where(it => it.EQID == EQID && it.DataTime.Date >= date.AddDays(-7)).OrderBy(it => it.DataTime).ToList();
                        //maxUtilizationPerDay = runratedata.Select(it => new { Date = it.DataTime.Date.ToString("yy-MM-dd"), Value = it.RunrateValue.ToString("0.000") }).ToList(); // 转换为列表
                        return Json(new { maxUtilizationPerDay, maxOutputPerDay }); //maxUtilizationPerDay
                    }
                    else
                    {
                        var utilizationList = db.Queryable<EquipmentParamsHistoryRaw>()
                          .Where(it => it.EQID == EQID && it.Name == "utilization" && it.UpdateTime.Date > date.Date.AddDays(-8) && it.UpdateTime.Date < date.Date)
                          .OrderBy(it => it.UpdateTime).ToList();
                        // 使用LINQ查询每天的最大值
                        var maxUtilizationPerDay = utilizationList
                            .GroupBy(item => item.UpdateTime.Date) // 按日期分组
                            .Select(group => group.OrderByDescending(item => item.UpdateTime).First()) // 选择每个分组中值最大的项
                            .ToList()
                            .Select(it => new { Date = it.UpdateTime.Date.ToString("yy-MM-dd"), Value = it.Value }).ToList(); // 转换为列表

                        var outputList = db.Queryable<EquipmentParamsHistoryRaw>()
                            .Where(it => it.EQID == EQID && it.Name.Contains("to") && it.UpdateTime.Date > date.Date.AddDays(-8) && it.UpdateTime.Date < date.Date)
                            .OrderBy(it => it.UpdateTime).ToList();
                        var maxOutputPerDay = outputList
                            .GroupBy(item => new { item.UpdateTime.Date, item.Name }) // 按日期分组
                            .Select(group => group.OrderByDescending(item => item.UpdateTime).First()) // 选择每个分组中值最大的项
                            .ToList()
                            .Select(it => new { it.EQID, it.Name, Date = it.UpdateTime.Date.ToString("yy-MM-dd"), Value = it.Value }).ToList(); // 转换为列表
                        var runratedata = db.Queryable<HistoryRunrateResult>().Where(it => it.EQID == EQID && it.DataTime.Date >= date.AddDays(-7)).OrderBy(it => it.DataTime).ToList();
                        //maxUtilizationPerDay = runratedata.Select(it => new { Date = it.DataTime.Date.ToString("yy-MM-dd"), Value = it.RunrateValue.ToString("0.000") }).ToList(); // 转换为列表
                        return Json(new { maxUtilizationPerDay, maxOutputPerDay }); //maxUtilizationPerDay
                    }

                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return Json(new { });
            }

        }

        public JsonResult GetPartNumberList()
        {
            var db = DbFactory.GetWMSClient();
            var pns = Request.Params["PartNumbers[]"];
            var pnArr = pns != null ? pns.Split(',') : new string[0];
            var pnList = db.Queryable<WMS_PART>()
                .Where(it => (pns == null) || pnArr.Contains(it.PART_NUMBER))
            .ToList();
            //var pageList = pnList.Skip((page - 1) * limit).Take(limit).ToList();
            //foreach(var item in pnList)
            //{
            //    item.REEL_SIZE = "7";
            //    item.LAST_EDIT_TIME = DateTime.Now;
            //    item.LAST_EDITOR = "admin";
            //    db.Updateable(item).ExecuteCommand();
            //}
            var data = new
            {
                code = 0,
                data = pnList,
                count = pnList.Count

            };
            return Json(data);
        }
        public JsonResult EditPartNumber()
        {
            var db = DbFactory.GetWMSClient();
            string msg = string.Empty;
            var user = (EAP.Dashboard.Models.FrameworkUsers)Session["user_account"];
            if (user == null)
            {
                msg = "请先登录！";
                return Json(new { message = msg });
            }
            try
            {

                db.BeginTran();

                var part_number = Request.Params["targetData[PART_NUMBER]"];
                var mat_group = Request.Params["targetData[MAT_GROUP]"];
                var round_value = Request.Params["targetData[ROUND_VALUE]"];
                var reel_size = Request.Params["targetData[REEL_SIZE]"];
                var targetPN = db.Queryable<WMS_PART>().Where(it => it.PART_NUMBER == part_number).First();


                targetPN.REEL_SIZE = reel_size;
                targetPN.ROUND_VALUE = Convert.ToInt32(round_value);
                targetPN.MAT_GROUP = mat_group;
                targetPN.LAST_EDITOR = user.Name;
                targetPN.LAST_EDIT_TIME = DateTime.Now;



                db.Updateable(targetPN).ExecuteCommand();
                db.CommitTran();
                msg = $"{targetPN.PART_NUMBER} 更新成功！";
            }
            catch (Exception ex)
            {
                Log.Error($"更新料号失败！Details: " + ex.Message);
                msg = $"更新失败！";
                db.RollbackTran();
            }


            return Json(new { message = msg });
        }
        public JsonResult GetHistoryUtilization(string EQID, string datetime)
        {
            var db = DbFactory.GetSqlSugarClient();
            var date = Convert.ToDateTime(datetime);

            var utilizationList = db.Queryable<EquipmentParamsHistoryRaw>()
                .Where(it => it.EQID == EQID && it.Name == "utilization" && it.UpdateTime.Date > date.Date.AddDays(-8) && it.UpdateTime.Date < date.Date)
                .OrderBy(it => it.UpdateTime).ToList();
            // 使用LINQ查询每天的最大值
            var maxUtilizationPerDay = utilizationList
                .GroupBy(item => item.UpdateTime.Date) // 按日期分组
                .Select(group => group.OrderByDescending(item => item.UpdateTime).First()) // 选择每个分组中值最大的项
                .ToList()
                .Select(it => new { Date = it.UpdateTime.Date.ToString("yy-MM-dd"), Value = it.Value }).ToList(); // 转换为列表

            var outputList = db.Queryable<EquipmentParamsHistoryRaw>()
                .Where(it => it.EQID == EQID && it.Name.Contains("to") && it.UpdateTime.Date > date.Date.AddDays(-8) && it.UpdateTime.Date <= date.Date)
                .OrderBy(it => it.UpdateTime).ToList();
            //.GroupBy(it => new {it.Name,it.Value}).Select(g => g.OrderBy(t=>t.UpdateTime).First()).ToList();
            // 使用LINQ查询每天的最大值
            var maxOutputPerDay = outputList
                .GroupBy(item => new { item.UpdateTime.Date, item.Name }) // 按日期分组
                .Select(group => group.OrderByDescending(item => item.UpdateTime).First()) // 选择每个分组中值最大的项
                .ToList()
                .Select(it => new { it.EQID, it.Name, Date = it.UpdateTime.Date.ToString("yy-MM-dd"), Value = it.Value }).ToList(); // 转换为列表

            return Json(new { maxUtilizationPerDay, maxOutputPerDay });
        }

        public JsonResult GetHistoryOutput(string EQID, string datetime)
        {
            var db = DbFactory.GetSqlSugarClient();
            var date = Convert.ToDateTime(datetime);

            var outputList = db.Queryable<EquipmentParamsHistoryRaw>()
                .Where(it => it.EQID == EQID && it.Name.Contains("to") && it.UpdateTime.Date > date.Date.AddDays(-8) && it.UpdateTime.Date <= date.Date)
                .OrderBy(it => it.UpdateTime).ToList();
            //.GroupBy(it => new {it.Name,it.Value}).Select(g => g.OrderBy(t=>t.UpdateTime).First()).ToList();
            // 使用LINQ查询每天的最大值
            var maxValuesPerDay = outputList
                .GroupBy(item => new { item.UpdateTime.Date, item.Name }) // 按日期分组
                .Select(group => group.OrderByDescending(item => item.UpdateTime).First()) // 选择每个分组中值最大的项
                .ToList()
                .Select(it => new { it.EQID, it.Name, Date = it.UpdateTime.Date.ToString("yy-MM-dd"), Value = it.Value }).ToList(); // 转换为列表

            return Json(new { maxValuesPerDay });
        }
        public JsonResult GetTotalOutput(string datetime)
        {
            try
            {
                var redisdb = RedisHelper.Database;
                string cacheKey = "historyTotalOutput";
                List<EquipmentParamsHistoryRaw> cachedList = RedisHelper.GetList<EquipmentParamsHistoryRaw>(cacheKey);

                if (cachedList == null)
                {
                    // 缓存中没有值，从数据源获取数据
                    var db = DbFactory.GetSqlSugarClient();
                    var date = Convert.ToDateTime(datetime);
                    var eqList = db.Queryable<Equipment>().Where(it => it.EQID.StartsWith("EQAMS")).Select(it => it.EQID).ToList();
                    var resultList = new List<EquipmentParamsHistoryRaw>();

                    foreach (var eq in eqList)
                    {
                        var output = db.Queryable<EquipmentParamsHistoryRaw>()
                        .Where(it => it.EQID == eq && it.Name.Contains("to") && it.UpdateTime.Date <= date.Date)
                        .OrderBy(it => it.UpdateTime).ToList();

                        // 使用LINQ查询每天的最大值
                        var maxValuesPerDay = output
                            .GroupBy(item => new { item.UpdateTime.Date, item.Name }) // 按日期分组
                            .Select(group => group.OrderByDescending(item => item.UpdateTime).First()) // 选择每个分组中值最大的项
                            .ToList();
                        resultList.AddRange(maxValuesPerDay);
                    }

                    //.GroupBy(it => new {it.Name,it.Value}).Select(g => g.OrderBy(t=>t.UpdateTime).First()).ToList();
                    //var data = resultList.Select(it => new { it.EQID, it.Name, Date = it.UpdateTime.Date.ToString("yy-MM-dd"), Value = it.Value }).ToList();

                    cachedList = resultList;

                    RedisHelper.SetList(cacheKey, cachedList, TimeSpan.FromDays(1));
                }
                var data = cachedList.Select(it => new { it.EQID, it.Name, Date = it.UpdateTime.Date.ToString("yy-MM-dd"), Value = it.Value }).ToList();
                return Json(new { data });
            }
            catch (Exception ex)
            {

                Log.Error(ex);
                return Json(new { });
            }
        }
        public FileContentResult DownloadReportData(List<string> eqps, string endtime, string starttime)//FileContentResult
        {
            //var db = DbFactory.GetSqlSugarClient();
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            var package = new ExcelPackage();

            try
            {
                serviceHandler.GenerateFileTemplate(package, Convert.ToDateTime(starttime).Month);

                var timeGap = (Convert.ToDateTime(endtime) - Convert.ToDateTime(starttime)).Days;
                var user = (EAP.Dashboard.Models.FrameworkUsers)Session["user_account"];
                Log.Info($"{user.Name}-Downloading RIDM OEE Report: {starttime} ~ {endtime}");
                for (int i = timeGap; i >= 0; i--)
                {
                    var dataList = serviceHandler.GetOEERawData(Convert.ToDateTime(endtime).AddDays(-i));

                    serviceHandler.UpdateReport(package, dataList, Convert.ToDateTime(endtime).AddDays(-i));
                }
                var datastring = package.GetAsByteArray();
                return new FileContentResult(datastring, "application/x-xls");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                var datastring = package.GetAsByteArray();
                return new FileContentResult(datastring, "application/x-xls");
            }
        }


        public FileContentResult DownloadLogData(List<string> eqps, string endtime, string starttime)//FileContentResult
        {
            // 创建一个临时目录
            string tempDirectory = Path.Combine("\\\\127.0.0.1\\e$\\tmp\\RIDM\\Log", Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDirectory);
            try
            {
                var db = DbFactory.GetSqlSugarClient();
                int cyclingTimes = (int)(Convert.ToDateTime(endtime) - Convert.ToDateTime(starttime)).TotalDays;
                //var datePreFix = Convert.ToDateTime(datetime).ToString("yyyyMMdd");

                var user = (EAP.Dashboard.Models.FrameworkUsers)Session["user_account"];
                Log.Info($"{user.Name}-Downloading Log Report: {starttime} ~ {endtime}");

                foreach (var eqp in eqps)
                {
                    var EQIDid = db.Queryable<Equipment>().Where(it => it.EQID == eqp).Select(it => it.ID).First();
                    var ipStr = db.Queryable<EquipmentConfiguration>().Where(it => it.EQIDId == EQIDid && it.ConfigurationItem == "IP.Address").First();
                    var accountStr = db.Queryable<EquipmentConfiguration>().Where(it => it.EQIDId == EQIDid && it.ConfigurationItem == "IP.Account").First();
                    var pswStr = db.Queryable<EquipmentConfiguration>().Where(it => it.EQIDId == EQIDid && it.ConfigurationItem == "IP.Password").First();
                    if (ipStr == null || accountStr == null || pswStr == null) continue;
                    var url = $"\\\\{ipStr.ConfigurationValue}\\d$";//Path.Combine(ipStr.ConfigurationValue.ToString(),"d$");//$"\\\\192.168.53.174\\e$";
                    if (eqp == "EQAMS00005") url = $"\\\\{ipStr.ConfigurationValue}\\d";
                    //var url = $"\\\\127.0.0.1\\d$\\test\\合料output\\{eqp}";
                    var account = accountStr.ConfigurationValue.ToString();
                    var password = pswStr.ConfigurationValue.ToString();
                    //ConnectLan(url, account, password);

                    if (!Directory.Exists(url))
                    {
                        NetworkCredential networkCredential = new NetworkCredential(account, password);
                        try
                        {
                            var connection = new NetworkConnection(url, networkCredential);
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex.ToString());
                            ConnectLan(url, account, password);
                        }

                        if (!Directory.Exists(url))
                        {
                            Log.Error($"{eqp}-{url}:路径不存在！");
                            continue;
                        }
                        //continue;
                    }
                    var paths = new List<string>
                    {
                        url+"\\报警信息",
                        url+"\\Report\\AlloyPlate\\Labe"
                    };
                    var files = new List<string>();
                    foreach (var path in paths)
                    {
                        DirectoryInfo directory = new DirectoryInfo(path);


                        for (int d = 0; d <= cyclingTimes; d++)
                        {
                            var datetime = Convert.ToDateTime(starttime).AddDays(d);
                            var datePreFix = Convert.ToDateTime(datetime).ToString("yyyyMMdd");
                            // 获取文件夹中所有以指定前缀开头的文件
                            var isExist = Directory.GetFiles(path, $"{datePreFix}*").FirstOrDefault();
                            if (isExist == null) continue;
                            files.Add(isExist);
                        }

                    }
                    string subDirectory = Path.Combine(tempDirectory, eqp);
                    Directory.CreateDirectory(subDirectory);

                    foreach (var file in files)
                    {

                        string destinationPath = Path.Combine(subDirectory, Path.GetFileName(file));

                        System.IO.File.Copy(file, destinationPath);

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

                    return new FileContentResult(memoryStream.ToArray(), "application/zip");
                }

            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return new FileContentResult(new MemoryStream().ToArray(), "application/zip");
            }

        }

        public ActionResult PrecheckRoundValue(string endtime, string starttime, string week)
        {
            #region readFile
            //var filePath = Path.Combine("\\\\127.0.0.1\\e$\\tmp\\RIDM\\OEE", "sap_tmp.xlsx");
            //var result = new List<WMS_SAP_MAT_ISSUEING>();
            //ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            //// 打开 Excel 文件
            //FileInfo fileInfo = new FileInfo(filePath);

            //using (var package = new ExcelPackage(fileInfo))
            //{
            //    var worksheet = package.Workbook.Worksheets[0];

            //    for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
            //    {
            //        var rowData = new WMS_SAP_MAT_ISSUEING
            //        {
            //            ISSUE_DATE = worksheet.Cells[row, 1].Text,
            //            PART_NUMBER = worksheet.Cells[row, 2].Text,
            //            PLANT_CODE = worksheet.Cells[row, 4].Text,

            //            SLOC = worksheet.Cells[row, 5].Text,
            //            MVT = worksheet.Cells[row, 6].Text,
            //            BATCH_ID = worksheet.Cells[row, 7].Text,

            //            QTY = Math.Abs(Convert.ToDecimal(worksheet.Cells[row, 15].Value)),
            //            EUN = worksheet.Cells[row, 16].Text,
            //            WORK_ORDER = worksheet.Cells[row, 17].Text,
            //            MAT_GROUP = worksheet.Cells[row, 18].Text,


            //        };

            //        result.Add(rowData);
            //    }


            //}
            #endregion
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            var package = new ExcelPackage();
            var summarySheet = package.Workbook.Worksheets.Add("Summary");
            var filterMatGroups = new List<string>() { "1", "4", "5", "59" };

            var db = DbFactory.GetWMSClient();

            //先筛选出合料机中，最小包装量或料盘大小未设置的记录
            var integrate_reels = db.Queryable<WMS_REEL_INTEGRATE>()
                      .Where(a => a.OperateTime <= Convert.ToDateTime(endtime) && a.OperateTime >= Convert.ToDateTime(starttime))
                      .LeftJoin<WMS_PART>((a, part) => a.PartNumber == part.PART_NUMBER)
                      //.GroupBy(a => a.PartNumber)
                      .Select((a, part) => new
                      {
                          a.IntegratedType,
                          a.PartNumber,
                          a.ReelSize,
                          a.WorkOrder,
                          a.ModelName,
                          a.OperateTime,
                          //SUB_REEL_CNT = Convert.ToInt32(a.IntegratedType),
                          part.PART_NUMBER,
                          part.REEL_SIZE,
                          part.PACK_TYPE,
                          part.ROUND_VALUE

                      })//.ToList()
                        //.Where(it => it.REEL_SIZE.StartsWith("7"))
                      .ToList();
            var filtered_eqp_data = integrate_reels.Where(it => it.ROUND_VALUE == 0 || string.IsNullOrEmpty(it.REEL_SIZE)).Select(it => it.PartNumber).Distinct().ToList();
            var filtered_eqp_workorders = integrate_reels.Select(it => it.WorkOrder).Distinct().ToList();

            var part_list = db.Queryable<WMS_PART>().ToList();
            var sap_records = db.Queryable<WMS_SAP_MAT_ISSUEING>()
            .Where(sap => sap.MVT == "971"
            && !filterMatGroups.Contains(sap.MAT_GROUP)
            //&& Convert.ToDateTime(sap.ISSUE_DATE) >= Convert.ToDateTime(starttime)
            && Convert.ToDateTime(sap.ISSUE_DATE) <= Convert.ToDateTime(endtime)
            && filtered_eqp_workorders.Contains(sap.WORK_ORDER.Substring(sap.WORK_ORDER.Length - 7, 7))).ToList();
            var joinResult = (from sap in sap_records
                                  //where !filterMatGroups.Contains(sap.MAT_GROUP) && sap.MVT == "971"
                              join part in part_list
                              on sap.PART_NUMBER equals part.PART_NUMBER into results
                              from item in results.DefaultIfEmpty()
                              select new
                              {
                                  Date = sap.ISSUE_DATE,
                                  SAP_PART_NUMBER = sap.PART_NUMBER,
                                  WORK_ORDER = sap.WORK_ORDER.Substring(sap.WORK_ORDER.Length - 7, 7),
                                  sap.MVT,
                                  sap.QTY,
                                  sap.MAT_GROUP,
                                  PART_NUMBER = item == null ? null : item.PART_NUMBER,
                                  ROUND_VALUE = item == null ? 0 : item.ROUND_VALUE,
                                  REEL_SIZE = item == null ? null : item.REEL_SIZE,
                                  REEL_CNT = item == null ? 0 : (int)sap.QTY / item.ROUND_VALUE,
                                  WEIPAN_CNT = item == null ? 0 : ((int)sap.QTY % item.ROUND_VALUE != 0 ? 1 : 0),

                              }).ToList();
            //这里分成两步：
            //筛选出SAP和料号表里都有，但是最小包装量是0或者没有reel size的料号，需要用户预设
            var filtered_sap_data = joinResult.Where(it => string.IsNullOrEmpty(it.PART_NUMBER) || it.ROUND_VALUE == 0 || string.IsNullOrEmpty(it.REEL_SIZE)).ToList();//.Select(it => it.SAP_PART_NUMBER).Distinct().ToList();
            //筛选出SAP有的料号，但是料号表里没有的料号，直接新建插入
            var insert_data = filtered_sap_data.Where(it => string.IsNullOrEmpty(it.PART_NUMBER)).Select(it => it.PART_NUMBER).ToList();
            insert_data.AddRange(integrate_reels.Where(it => string.IsNullOrEmpty(it.PART_NUMBER)).Select(it => it.PartNumber).Distinct().ToList());
            foreach (var item in insert_data)
            {
                var part_item = new WMS_PART()
                {
                    PART_NUMBER = item
                };
                db.Insertable(part_item).ExecuteCommand();
            }
            var data = filtered_eqp_data.Union(filtered_sap_data.Select(it => it.SAP_PART_NUMBER)).ToList();

            //filtered_eqp_data.Distinct();

            return Json(data, JsonRequestBehavior.AllowGet);


        }

        public ActionResult DownloadIntegrateRate(string endtime, string starttime, string week)
        {

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            var package = new ExcelPackage();
            var summarySheet = package.Workbook.Worksheets.Add("Summary");
            var eqpSheet = package.Workbook.Worksheets.Add("合料机明细");
            var sapSheet = package.Workbook.Worksheets.Add("发料明细");

            var filterMatGroups = new List<string>() { "1", "4", "5", "59" };

            var db = DbFactory.GetWMSClient();

            //先筛选出合料机中，最小包装量或料盘大小未设置的记录
            var integrate_reels = db.Queryable<WMS_REEL_INTEGRATE>()
                      .Where(a => a.OperateTime <= Convert.ToDateTime(endtime) && a.OperateTime >= Convert.ToDateTime(starttime))
                      .LeftJoin<WMS_PART>((a, part) => a.PartNumber == part.PART_NUMBER)
                      //.GroupBy(a => a.PartNumber)
                      .Select((a, part) => new
                      {
                          a.IntegratedType,
                          a.PartNumber,
                          a.ReelSize,
                          a.WorkOrder,
                          a.ModelName,
                          a.OperateTime,
                          //SUB_REEL_CNT = Convert.ToInt32(a.IntegratedType),
                          part.REEL_SIZE,
                          part.PACK_TYPE,
                          part.ROUND_VALUE

                      })//.ToList()
                        //.Where(it => it.REEL_SIZE.StartsWith("7"))
                      .ToList();
            var filtered_eqp_data = integrate_reels.Where(it => it.ROUND_VALUE == 0 || string.IsNullOrEmpty(it.REEL_SIZE)).Select(it => it.PartNumber).Distinct().ToList();
            eqpSheet.Cells["A1"].LoadFromDataTable(DashBoardService.ToDataTable(integrate_reels), true);

            var filtered_eqp_workorders = integrate_reels.Select(it => it.WorkOrder).Distinct().ToList();

            var part_list = db.Queryable<WMS_PART>().ToList();
            var sap_records = db.Queryable<WMS_SAP_MAT_ISSUEING>()
            .Where(sap => sap.MVT == "971"
            && !filterMatGroups.Contains(sap.MAT_GROUP)
            //&& Convert.ToDateTime(sap.ISSUE_DATE) >= Convert.ToDateTime(starttime)
            && Convert.ToDateTime(sap.ISSUE_DATE) <= Convert.ToDateTime(endtime)
            && filtered_eqp_workorders.Contains(sap.WORK_ORDER.Substring(sap.WORK_ORDER.Length - 7, 7))).ToList();
            var joinResult = (from sap in sap_records
                                  //where !filterMatGroups.Contains(sap.MAT_GROUP) && sap.MVT == "971"
                              join part in part_list
                              on sap.PART_NUMBER equals part.PART_NUMBER into results
                              from item in results.DefaultIfEmpty()
                              select new
                              {
                                  Date = sap.ISSUE_DATE,
                                  SAP_PART_NUMBER = sap.PART_NUMBER,
                                  WORK_ORDER = sap.WORK_ORDER.Substring(sap.WORK_ORDER.Length - 7, 7),
                                  sap.MVT,
                                  sap.QTY,
                                  sap.MAT_GROUP,
                                  PART_NUMBER = item == null ? null : item.PART_NUMBER,
                                  ROUND_VALUE = item == null ? 0 : item.ROUND_VALUE,
                                  REEL_SIZE = item == null ? null : item.REEL_SIZE,
                                  REEL_CNT = item == null ? 0 : (int)sap.QTY / item.ROUND_VALUE,
                                  WEIPAN_CNT = item == null ? 0 : ((int)sap.QTY % item.ROUND_VALUE != 0 ? 1 : 0),

                              }).ToList();

            var filtered_sap_data = joinResult.Where(it => string.IsNullOrEmpty(it.PART_NUMBER) || it.ROUND_VALUE == 0 || string.IsNullOrEmpty(it.REEL_SIZE)).Select(it => it.SAP_PART_NUMBER).Distinct().ToList();

            sapSheet.Cells["A1"].LoadFromDataTable(DashBoardService.ToDataTable(joinResult), true);

            filtered_eqp_data.AddRange(filtered_sap_data);

            filtered_eqp_data.Distinct();

            if (filtered_eqp_data.Count != 0)
            {

                return Json(new { msg = "存在料号未设置最小包装量/料盘尺寸，请先点击检查料号！" });
            }



            //没有该记录，继续计算
            //var part_list = db.Queryable<WMS_PART>().ToList();

            var total_issue_reels = joinResult.GroupBy(g => g.WORK_ORDER)
                        .Select(it => new
                        {
                            workorder = it.Key,
                            reel_cnt = it.Sum(x => x.REEL_CNT),
                            qty = it.Sum(x => x.QTY),
                            mantissa_cnt = it.Sum(x => x.WEIPAN_CNT),
                        }).ToList();


            var total_integrated_reels = integrate_reels

                .Where(it => it.REEL_SIZE.StartsWith("7") && it.IntegratedType != "1")
                .GroupBy(g => new { g.WorkOrder, g.ModelName })
                .Select(it => new
                {
                    model = it.Key.ModelName,
                    workorder = it.Key.WorkOrder,
                    reel_cnt = it.Sum(x => Convert.ToDecimal(x.IntegratedType)),
                    big_reel_cnt = it.Count(),

                }).ToList();

            var joint_reels = (from eqp in total_integrated_reels
                                   //where !filterMatGroups.Contains(sap.MAT_GROUP) && sap.MVT == "971"
                               join sap in total_issue_reels
                               on eqp.workorder equals sap.workorder into results
                               from item in results.DefaultIfEmpty()
                               select new
                               {
                                   MODEL = eqp.model,
                                   WO = item == null ? eqp.workorder : item.workorder,
                                   QTY = item == null ? 0 : item.qty,
                                   ISSUE_CNT = item == null ? 0 : item.reel_cnt,
                                   WEIPAN = item == null ? 0 : item.mantissa_cnt,
                                   INTEGRATED_CNT = eqp.reel_cnt,
                                   REEL_13 = eqp.big_reel_cnt

                               }).ToList();



            summarySheet.Cells[2, 2].Value = $"合料率统计 {week}";

            summarySheet.Cells[2, 2, 2, 13].Merge = true;
            summarySheet.Cells[2, 14, 2, 15].Merge = true;

            summarySheet.Cells[3, 2].Value = $"线别";
            summarySheet.Cells[3, 3].Value = $"机种";
            summarySheet.Cells[3, 4].Value = $"W/O";
            summarySheet.Cells[3, 5].Value = $"QTY";
            summarySheet.Cells[3, 6].Value = $"发料盘数";
            summarySheet.Cells[3, 2, 5, 2].Merge = true;
            summarySheet.Cells[3, 3, 5, 3].Merge = true;
            summarySheet.Cells[3, 4, 5, 4].Merge = true;
            summarySheet.Cells[3, 5, 5, 5].Merge = true;
            summarySheet.Cells[3, 6, 5, 6].Merge = true;

            summarySheet.Cells[3, 7].Value = $"合料比例";
            summarySheet.Cells[3, 7, 3, 9].Merge = true;
            summarySheet.Cells[3, 10].Value = $"未合料比例";
            summarySheet.Cells[3, 10, 3, 13].Merge = true;
            summarySheet.Cells[3, 14].Value = $"Total";
            summarySheet.Cells[3, 14, 4, 15].Merge = true;

            summarySheet.Cells[4, 7].Value = $"7寸";
            summarySheet.Cells[4, 7, 4, 9].Merge = true;

            summarySheet.Cells[4, 10].Value = $"尾数盘";
            summarySheet.Cells[4, 10, 4, 11].Merge = true;
            summarySheet.Cells[4, 12].Value = $"单LOT";
            summarySheet.Cells[4, 12, 4, 13].Merge = true;

            summarySheet.Cells[5, 7].Value = "7寸合料盘数";
            summarySheet.Cells[5, 8].Value = "已合13寸（盘）";
            summarySheet.Cells[5, 9].Value = "合料率";

            summarySheet.Cells[5, 10].Value = "盘数";
            summarySheet.Cells[5, 11].Value = "占比";
            summarySheet.Cells[5, 12].Value = "盘数";
            summarySheet.Cells[5, 13].Value = "占比";
            summarySheet.Cells[5, 14].Value = "合料占比";
            summarySheet.Cells[5, 15].Value = "未合料占比";



            var row = 6;
            foreach (var record in joint_reels)
            {
                //机种
                summarySheet.Cells[row, 3].Value = record.MODEL;

                //工单
                summarySheet.Cells[row, 4].Value = record.WO;

                //wms发料颗数
                summarySheet.Cells[row, 5].Value = record.QTY;
                //wms发料盘数
                summarySheet.Cells[row, 6].Value = record.ISSUE_CNT;

                //7寸
                summarySheet.Cells[row, 7].Value = record.INTEGRATED_CNT;

                //13寸
                summarySheet.Cells[row, 8].Value = record.REEL_13;

                //合料率
                summarySheet.Cells[row, 9].FormulaR1C1 = "IF(RC[-2]/RC[-3]>1,1,RC[-2]/RC[-3])";
                summarySheet.Cells[row, 9].Style.Numberformat.Format = "0.00%";
                //尾数盘
                summarySheet.Cells[row, 10].Value = record.WEIPAN;

                //尾数盘占比
                summarySheet.Cells[row, 11].FormulaR1C1 = "IF(RC[-1]/RC[-5]>1,1,RC[-1]/RC[-5])";
                summarySheet.Cells[row, 11].Style.Numberformat.Format = "0.00%";


                //单lot
                var singleLotCnt = integrate_reels.Where(it => it.IntegratedType == "1" && it.WorkOrder == record.WO).Count();
                summarySheet.Cells[row, 12].Value = singleLotCnt;
                //单lot占比
                summarySheet.Cells[row, 13].FormulaR1C1 = "IF(RC[-1]/RC[-7]>1,1,RC[-1]/RC[-7])";
                summarySheet.Cells[row, 13].Style.Numberformat.Format = "0.00%";

                //Total by workorder
                summarySheet.Cells[row, 14].FormulaR1C1 = "IF(RC[-7]/RC[-8]>1,1,RC[-7]/RC[-8])";
                summarySheet.Cells[row, 14].Style.Numberformat.Format = "0.00%";

                summarySheet.Cells[row, 15].FormulaR1C1 = "IF((RC[-3]+RC[-5])/RC[-9]>1,1,(RC[-3]+RC[-5])/RC[-9])";
                summarySheet.Cells[row, 15].Style.Numberformat.Format = "0.00%";
                row++;

            }

            // Total
            summarySheet.Cells[row, 2].Value = $"Total";
            summarySheet.Cells[row, 2, row, 5].Merge = true;

            summarySheet.Cells[row, 6].FormulaR1C1 = $"SUM(R[-{joint_reels.Count}]C:R[-1]C)";
            summarySheet.Cells[row, 7].FormulaR1C1 = $"SUM(R[-{joint_reels.Count}]C:R[-1]C)";
            summarySheet.Cells[row, 8].FormulaR1C1 = $"SUM(R[-{joint_reels.Count}]C:R[-1]C)";
            summarySheet.Cells[row, 9].FormulaR1C1 = $"AVERAGE(R[-{joint_reels.Count}]C:R[-1]C)";
            summarySheet.Cells[row, 9].Style.Numberformat.Format = "0.00%";
            summarySheet.Cells[row, 10].FormulaR1C1 = $"SUM(R[-{joint_reels.Count}]C:R[-1]C)";
            summarySheet.Cells[row, 11].FormulaR1C1 = $"AVERAGE(R[-{joint_reels.Count}]C:R[-1]C)";
            summarySheet.Cells[row, 11].Style.Numberformat.Format = "0.00%";

            summarySheet.Cells[row, 12].FormulaR1C1 = $"SUM(R[-{joint_reels.Count}]C:R[-1]C)";

            summarySheet.Cells[row, 13].FormulaR1C1 = $"AVERAGE(R[-{joint_reels.Count}]C:R[-1]C)";
            summarySheet.Cells[row, 13].Style.Numberformat.Format = "0.00%";
            summarySheet.Cells[row, 14].FormulaR1C1 = $"AVERAGE(R[-{joint_reels.Count}]C:R[-1]C)";
            summarySheet.Cells[row, 14].Style.Numberformat.Format = "0.00%";
            summarySheet.Cells[row, 15].FormulaR1C1 = $"AVERAGE(R[-{joint_reels.Count}]C:R[-1]C)";
            summarySheet.Cells[row, 15].Style.Numberformat.Format = "0.00%";

            var cellrange = summarySheet.Cells[2, 2, row, 15];
            cellrange.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            cellrange.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            cellrange.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            cellrange.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;

            cellrange.Style.Font.Bold = true;
            cellrange.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            cellrange.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

            summarySheet.Cells[2, 2, 5, 15].Style.Font.Color.SetColor(Color.White);
            summarySheet.Cells[2, 2, 5, 15].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            summarySheet.Cells[2, 2, 5, 15].Style.Fill.BackgroundColor.SetColor(Color.Blue);

            summarySheet.Cells[row, 2, row, 15].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            summarySheet.Cells[row, 2, row, 15].Style.Fill.BackgroundColor.SetColor(Color.Orange);

            //FileInfo fileInfo = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "report.xlsx"));
            //package.SaveAs(fileInfo);
            //package.GetAsByteArray();
            return new FileContentResult(package.GetAsByteArray(), "application/x-xls");

            //return File(new { result = true,data = joint_reels });

        }



    }
}