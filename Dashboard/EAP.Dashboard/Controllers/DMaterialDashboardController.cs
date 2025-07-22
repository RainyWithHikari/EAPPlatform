using EAP.Dashboard.Models;
using EAP.Dashboard.Utils;
using EAP.Models.Database;
using Microsoft.IdentityModel.Tokens;
using SqlSugar.Extensions;
using SqlSugar.SplitTableExtensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using static EAP.Dashboard.Utils.DashBoardService;
using Microsoft.Kiota.Abstractions;
using System.Runtime.InteropServices.ComTypes;
using System.Configuration;
using Newtonsoft.Json.Linq;
using System.Web.Script.Serialization;

namespace EAP.Dashboard.Controllers
{
    public class DMaterialDashboardController : BaseController
    {
        private static log4net.ILog Log = log4net.LogManager.GetLogger("Logger");
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

        public JsonResult GetEQPList()
        {
            var db = DbFactory.GetSqlSugarClient();

            var eqTypeId = db.Queryable<EquipmentType>().Where(it => it.Name == "拆料头")?.First().ID;
            var eqList = db.Queryable<Equipment>().Where(it => it.EquipmentTypeId == eqTypeId).OrderBy(it => it.Sort).Select(it => it.EQID).ToList();
            string option = "";

            foreach (var item in eqList)
            {
                option += "<option value=\"" + item + "\" >" + item + "</option>";
                //checkbox += $"<input type=\"checkbox\" name=\"like[{item}]\" title=\"{item.Replace("EQAMS0000", string.Empty)}号机\">" ;
            }
            return Json(new { eqList, option, defaultoption = eqList.ElementAt(0) });
        }
        public JsonResult GetEQPRealtimeData(string EQID, string datetime)
        {
            var db = DbFactory.GetSqlSugarClient();
            var date = datetime.IsNullOrEmpty() ? DateTime.Now : Convert.ToDateTime(datetime);

            var realtimeStatus = db.Queryable<EquipmentRealtimeStatus>().Where(it => it.EQID == EQID)?.Select(it => it.Status).First();

            var startTime = date.Date.AddHours(5);
            var endTime = date.Date.AddDays(1).AddHours(5);
            var realtimeOutputRaw = db.Queryable<EquipmentParamsHistoryRaw>()
                .Where(it => it.EQID == EQID && it.Name == "output" && it.UpdateTime >= startTime && it.UpdateTime <= endTime).ToList();
            if (date.Date == DateTime.Today && DateTime.Now.Hour < 5)
            {
                startTime = date.Date.AddDays(-1).AddHours(5);
                endTime = date.Date.AddHours(5);
                realtimeOutputRaw = db.Queryable<EquipmentParamsHistoryRaw>()
                .Where(it => it.EQID == EQID && it.Name == "output" && it.UpdateTime >= startTime && it.UpdateTime <= endTime).ToList();
            }
            var realtimeOutput = realtimeOutputRaw.Select(item =>
            {

                var values = item.Value.Split(',')
                .Select(v => v.Split('='))
                .ToDictionary(k => k[0], v => v[1]);

                return new
                {
                    CT = values.ContainsKey("CT") ? values["CT"] : "0",
                    PN = values.ContainsKey("PN") ? values["PN"] : "null",
                    ReelID = values.ContainsKey("reelID") ? values["reelID"] : "null",
                    IsFirst = values.ContainsKey("IsFirst") ? values["IsFirst"]=="Y" : false,
                    UpdateTime = item.UpdateTime

                };
            });
            string option = "<option value=\"all\" >" + "all</option>";//"";
            var pnList = realtimeOutput.Select(it => it.PN).Distinct().ToList();
            foreach (var item in pnList)
            {
                option += "<option value=\"" + item + "\" >" + item + "</option>";
                //checkbox += $"<input type=\"checkbox\" name=\"like[{item}]\" title=\"{item.Replace("EQAMS0000", string.Empty)}号机\">" ;
            }

            //success-rate
            var successRate = db.Queryable<EquipmentParamsRealtime>().Where(it=>it.EQID == EQID && it.Name == "successRatio").First()?.Value;
            var loadingRate = db.Queryable<EquipmentParamsRealtime>().Where(it => it.EQID == EQID && it.Name == "loading rate").First()?.Value;
            var downRate = db.Queryable<EquipmentParamsRealtime>().Where(it => it.EQID == EQID && it.Name == "down rate").First()?.Value;
            var throughput = db.Queryable<EquipmentParamsRealtime>().Where(it => it.EQID == EQID && it.Name == "throughput").First()?.Value;

            return Json(new { 
                status = realtimeStatus, 
                output = realtimeOutput,
                throughput,
                loadingRate,
                downRate,
                startTime = startTime.ToString(), 
                endTime = endTime.ToString(), 
                option, 
                SuccessRate = successRate });
        }

        public JsonResult GetEQPHistoryData(string EQID, string datetime, string starttime)
        {
            var db = DbFactory.GetSqlSugarClient();
            var date = datetime.IsNullOrEmpty() ? DateTime.Now : Convert.ToDateTime(datetime);
            if (date.Date == DateTime.Today) date = date.AddDays(-1);
            var startdate = starttime.IsNullOrEmpty() ? date.AddDays(-7) : Convert.ToDateTime(starttime);

            var dateSpan = (int)(date - startdate).TotalDays;

            var sql = string.Format(@"SELECT TO_CHAR(UPDATETIME,'yyyy/MM/DD') name, TO_NUMBER(VALUE) value 
FROM EQUIPMENTPARAMSHISCAL
WHERE UPDATETIME >= to_date( '{0}', 'yyyy-MM-DD HH24:MI:SS' )- INTERVAL '{1}' DAY
AND UPDATETIME <= TRUNC( to_date( '{0}', 'yyyy-MM-DD HH24:MI:SS' ) +1) + 5 / 24 
AND EQID = '{2}'
AND (NAME = 'loading rate' OR NAME = 'down rate' OR NAME = 'successRatio' OR NAME = 'alarmtimes' OR NAME = 'throughput')
", date, dateSpan, EQID);
            var resultList = db.Queryable<EquipmentParamsHistoryCalculate>()
                .Where(it=>it.UpdateTime >= date.AddDays(-dateSpan) && it.UpdateTime <= date.AddDays(1)
                && it.EQID == EQID &&
                (it.Name == "loading rate" || it.Name == "down rate" || it.Name == "successRatio" || it.Name == "alarmtimes" || it.Name == "throughput"))
                .OrderBy(it=>it.UpdateTime)
                .ToList();

            var runrateList = resultList.Where(it => it.Name == "loading rate").Select(it => new DataResult
            {
                name = it.UpdateTime.ToString("yyyy-MM-dd"),
                value = Convert.ToDecimal(it.Value)

            }).ToList();
            var downrateList = resultList.Where(it => it.Name == "down rate").Select(it => new DataResult
            {
                name = it.UpdateTime.ToString("yyyy-MM-dd"),
                value = Convert.ToDecimal(it.Value)

            }).ToList();
            var successrateList = resultList.Where(it => it.Name == "successRatio").Select(it => new DataResult
            {
                name = it.UpdateTime.ToString("yyyy-MM-dd"),
                value = Convert.ToDecimal(it.Value)

            }).ToList();
            var alarmtimesList = resultList.Where(it => it.Name == "alarmtimes").Select(it => new DataResult
            {
                name = it.UpdateTime.ToString("yyyy-MM-dd"),
                value = Convert.ToDecimal(it.Value)

            }).ToList();
            var throughputList = resultList.Where(it => it.Name == "throughput").Select(it => new DataResult
            {
                name = it.UpdateTime.ToString("yyyy-MM-dd"),
                value = Convert.ToDecimal(it.Value)

            }).ToList();
            return Json(new
            {
                runrateList,
                downrateList,
                successrateList,
                alarmtimesList,
                throughputList
               
            });

        }
        public JsonResult GetEQPRunrateByDate(string EQID, string datetime)
        {
            var db = DbFactory.GetSqlSugarClient();
            var date = datetime.IsNullOrEmpty() ? DateTime.Now : Convert.ToDateTime(datetime);

            //是否有计算记录
            var record = db.Queryable<EquipmentParamsHistoryCalculate>().Where(it => it.EQID == EQID && it.Name == "loading rate" && it.UpdateTime.Date == date.Date).First();
            if (record != null) return Json(new { loadingRate = Convert.ToDouble(record.Value) });

            var startTime = date.Date.AddHours(5);
            var endTime = date.Date.AddDays(1).AddHours(5);
            var totalTime = endTime - startTime;



            if (date.Date == DateTime.Today)
            {
                if (DateTime.Now.Hour < 5)
                {
                    startTime = date.Date.AddDays(-1).AddHours(5);
                    endTime = date.Date.AddHours(5);
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

            // 初始化变量
            TimeSpan onlineDuration = TimeSpan.Zero;
            DateTime? lastTimestamp = null;
            string lastStatus = null;

            // 遍历 DataTable 计算持续时间
            foreach (DataRow row in statusdata.Rows)
            {
                string currentStatus = row["STATUS"].ToString();
                DateTime currentTimestamp = (DateTime)row["DATETIME"];

                if (lastStatus != null && lastTimestamp.HasValue)
                {
                    // 计算上一段状态的持续时间
                    TimeSpan duration = currentTimestamp - lastTimestamp.Value;

                    // 如果上一段状态是 "run"，累加持续时间
                    if (lastStatus == "Run")
                    {
                        onlineDuration += duration;
                    }
                }

                // 更新状态和时间戳
                lastStatus = currentStatus;
                lastTimestamp = currentTimestamp;
            }
            var loadingRate = totalTime.TotalMinutes == 0 ? 0 : (onlineDuration.TotalMinutes / totalTime.TotalMinutes) * 100;
            if (date.Date < DateTime.Today)
            {
                //今天之前的记录，添加
                record = new EquipmentParamsHistoryCalculate
                {
                    EQID = EQID,
                    Name = "loading rate",
                    Value = loadingRate.ToString("0.00"),
                    UpdateTime = date
                };

                db.Insertable(record).ExecuteCommand();
            }


            return Json(new { loadingRate });
        }

        public JsonResult GetEQPDownrateByDate(string EQID, string datetime)
        {
            var db = DbFactory.GetSqlSugarClient();
            var date = datetime.IsNullOrEmpty() ? DateTime.Now : Convert.ToDateTime(datetime);

            //是否有计算记录
            var record = db.Queryable<EquipmentParamsHistoryCalculate>().Where(it => it.EQID == EQID && it.Name == "down rate" && it.UpdateTime.Date == date.Date).First();
            if (record != null) return Json(new { DownRate = Convert.ToDouble(record.Value) });

            var startTime = date.Date.AddHours(5);
            var endTime = date.Date.AddDays(1).AddHours(5);
            var totalTime = endTime - startTime;



            if (date.Date == DateTime.Today)
            {
                if (DateTime.Now.Hour < 5)
                {
                    startTime = date.Date.AddDays(-1).AddHours(5);
                    endTime = date.Date.AddHours(5);
                }

                totalTime = DateTime.Now - startTime;

            }

            var alarmData = DashBoardService.GetAlarmDetails(startTime, endTime, EQID, 5);



            // 合并同一天的报警时间段
            TimeSpan totalDuration = DashBoardService.MergeAndCalculateDuration(alarmData);

            var downRate = totalDuration.TotalMinutes / totalTime.TotalMinutes;

            if (date.Date < DateTime.Today)
            {
                //今天之前的记录，添加
                record = new EquipmentParamsHistoryCalculate
                {
                    EQID = EQID,
                    Name = "down rate",
                    Value = downRate.ToString("0.00"),
                    UpdateTime = date
                };

                db.Insertable(record).ExecuteCommand();
            }

            return Json(new { DownRate = downRate });
        }
        public JsonResult GetEQPSuccessRateByDate(string EQID, string datetime)
        {
            var db = DbFactory.GetSqlSugarClient();
            var date = datetime.IsNullOrEmpty() ? DateTime.Now : Convert.ToDateTime(datetime);
            var sql_Count = string.Format(@"SELECT 
    TRUNC(CASE 
        WHEN TO_CHAR(UPDATETIME, 'HH24') < 5 THEN UPDATETIME - INTERVAL '1' DAY 
        ELSE  UPDATETIME
    END) + INTERVAL '5' HOUR AS period_start,
    SUM(CASE WHEN name = 'output' THEN 1 ELSE 0 END) AS success_count,
    SUM(CASE WHEN name = 'FailCount' THEN 1 ELSE 0 END) AS fail_count,
    COUNT(*) AS total_count
FROM EQUIPMENTPARAMSHISRAW
WHERE UPDATETIME > to_date( '{0}', 'yyyy-MM-DD HH24:MI:SS' ) 
AND UPDATETIME <= TRUNC( to_date( '{0}', 'yyyy-MM-DD HH24:MI:SS' ) +1) + 5 / 24 
AND EQID = '{1}'
GROUP BY 
    TRUNC(CASE 
        WHEN TO_CHAR(UPDATETIME, 'HH24') < 5 THEN UPDATETIME - INTERVAL '1' DAY 
        ELSE UPDATETIME 
    END) + INTERVAL '5' HOUR
ORDER BY period_start;", date.AddHours(5), EQID);
            var success_fail_result = db.SqlQueryable<SuccessFail>(sql_Count).ToList();

            var success_fail = success_fail_result.Select(it => new
            {

                Date = it.period_start.ToString("yyyy/MM/dd"),

                SuccessRatio = ((1 - it.fail_count / it.total_count) * 100).ToString("0.00")

            }).FirstOrDefault()?.SuccessRatio;



            return Json(new { SuccessRate = success_fail });
        }
        public JsonResult GetEQPAlarmDetailsByDate(string EQID, string datetime)
        {
            var db = DbFactory.GetSqlSugarClient();

            var date = datetime.IsNullOrEmpty() ? DateTime.Today.AddHours(5) : Convert.ToDateTime(datetime).Date.AddHours(5);

            var redisdb = RedisHelper.Database;
            string cacheKey_historyAlarms = $"{EQID}_{date.ToShortDateString()}_historyAlarmsDetails";
            var alarmCachedList = RedisHelper.GetList<EquipmentAlarm>(cacheKey_historyAlarms);

            if ((date.Date == DateTime.Today.Date) || alarmCachedList == null)
            {

                var sql = string.Format(@"WITH AlarmTimes AS (
    SELECT 
        EQID,
        ALARMCODE,
				ALARMTEXT,
        ALARMSET,
        ALARMTIME,
        ROW_NUMBER() OVER (PARTITION BY EQID, ALARMCODE ORDER BY ALARMTIME) AS rn
    FROM EQUIPMENTALARM
    WHERE  ALARMTIME >= TRUNC( to_date( '{0}', 'yyyy-MM-DD HH24:MI:SS' ) ) + 5 / 24 
		AND ALARMTIME < TRUNC( to_date( '{0}', 'yyyy-MM-DD HH24:MI:SS' ) +1) + 5 / 24 
		
		--ALARMTIME > to_date( '2024/11/1 5:00:00', 'yyyy-MM-DD HH24:MI:SS' ) - INTERVAL '1' DAY
    --AND ALARMTIME <= TRUNC( to_date( '2024/11/1 5:00:00', 'yyyy-MM-DD HH24:MI:SS' ) +1) + 5 / 24 
		AND EQID = '{1}' 
		AND ALARMCODE <> '900002' 
		AND ALARMCODE <> '900006' 
		AND ALARMCODE <> '900007' 
),
FilteredAlarm AS (SELECT 
    AT1.EQID,
    AT1.ALARMCODE,
		AT1.ALARMTEXT,
    AT1.ALARMTIME AS START_TIME,
		AT2.ALARMTIME AS END_TIME,
		ROW_NUMBER ( ) OVER ( PARTITION BY AT1.ALARMCODE ORDER BY AT1.ALARMTIME ) AS rn 
    --COALESCE(AT2.ALARMTIME, TRUNC( to_date( '{0}' ) +1) + INTERVAL '5' HOUR) AS END_TIME
FROM AlarmTimes AT1
LEFT JOIN AlarmTimes AT2
    ON AT1.EQID = AT2.EQID 
    AND AT1.ALARMCODE = AT2.ALARMCODE
    AND AT2.ALARMSET = 0
    AND AT2.rn = AT1.rn +1
WHERE AT1.ALARMSET = 1
AND AT2.ALARMTIME IS NOT NULL
ORDER BY START_TIME)
SELECT
a1.EQID,
a1.ALARMCODE,
a1.ALARMTEXT,
a1.START_TIME AS ALARMTIME
FROM FilteredAlarm a1
LEFT JOIN FilteredAlarm a2 ON a1.ALARMCODE = a2.ALARMCODE 
AND a1.rn = a2.rn + 1 
AND a1.START_TIME <= a2.START_TIME + INTERVAL '2' MINUTE 
WHERE a2.START_TIME IS NULL 
ORDER BY ALARMTIME", date, EQID);

                var sql_new = string.Format(@"WITH AlarmTimes AS (
    SELECT 
        EQID,
        ALARMCODE,
        ALARMTEXT,
        ALARMSET,
        ALARMTIME,
        ROW_NUMBER() OVER (PARTITION BY EQID, ALARMCODE ORDER BY ALARMTIME) AS rn
    FROM EQUIPMENTALARM
    WHERE  
        ALARMTIME >=  TRUNC(TO_DATE('{0}', 'yyyy-MM-DD HH24:MI:SS'))+5/24
        AND ALARMTIME < TRUNC(TO_DATE('{0}', 'yyyy-MM-DD HH24:MI:SS')+1)+5/24
        AND UPPER(EQID) = '{1}'
),
FilteredAlarm AS (
    SELECT 
        AT1.EQID,
        AT1.ALARMCODE,
        AT1.ALARMTEXT,
        AT1.ALARMTIME AS START_TIME,
        AT2.ALARMTIME AS END_TIME,
        ROW_NUMBER() OVER (PARTITION BY AT1.ALARMCODE ORDER BY AT1.ALARMTIME) AS rn 
    FROM AlarmTimes AT1
    LEFT JOIN AlarmTimes AT2
        ON AT1.EQID = AT2.EQID 
        AND AT1.ALARMCODE = AT2.ALARMCODE
        AND AT2.ALARMSET = 0
        AND AT2.rn = AT1.rn + 1
    WHERE AT1.ALARMSET = 1
    AND AT2.ALARMTIME IS NOT NULL
    ORDER BY START_TIME
),
FinalAlarmData AS (
    SELECT
        a1.EQID,
        a1.ALARMCODE AS AlarmCode,
        a1.ALARMTEXT AS AlarmText,
        a1.START_TIME AS StartTime,
        a1.END_TIME AS EndTime,
        EXTRACT(SECOND FROM (a1.END_TIME - a1.START_TIME)) AS Duration,
				ROW_NUMBER() OVER (PARTITION BY a1.ALARMCODE ORDER BY a1.START_TIME) AS rn ,
        CASE
            WHEN TO_CHAR(a1.START_TIME, 'HH24:MI:SS') >= '05:00:00' THEN
                TRUNC(a1.START_TIME) + 5 / 24 
            ELSE 
                TRUNC(a1.START_TIME - 1) + 5 / 24 
        END AS AlarmDate
    FROM FilteredAlarm a1
    LEFT JOIN FilteredAlarm a2 
        ON a1.ALARMCODE = a2.ALARMCODE 
        AND a1.rn = a2.rn + 1
				--AND a1.START_TIME <= a2.START_TIME + INTERVAL '2' MINUTE 
    WHERE
        -- 排除接续于90002, 90006或90007的报警
        NOT EXISTS (
            SELECT 1 
            FROM FilteredAlarm fa 
            WHERE fa.ALARMCODE IN ('900002', '900006', '900007') 
              AND fa.END_TIME >= a1.START_TIME 
              AND fa.START_TIME < a1.START_TIME
        )
        -- 排除独立且自身不作为接续的90002, 90006, 90007报警
        AND (
            a1.ALARMCODE NOT IN ('900002', '900006', '900007') 
            OR (a1.ALARMCODE IN ('900002', '900006', '900007') 
                AND NOT EXISTS (
                    SELECT 1 
                    FROM FilteredAlarm fa2 
                    WHERE fa2.START_TIME > a1.START_TIME
                      AND fa2.START_TIME <= a1.END_TIME
                )
            )
        )
				--AND a2.START_TIME IS NULL 
    ORDER BY StartTime
)
SELECT 
a1.EQID,
--a1.ALARMDATE,
a1.ALARMCODE,
a1.ALARMTEXT,
a1.STARTTIME AS ALARMTIME
--a1.ENDTIME,
--a1.DURATION AS DURATION_SECONDS

FROM FinalAlarmData a1
LEFT JOIN FinalAlarmData a2 ON a1.ALARMCODE = a2.ALARMCODE
AND a1.rn = a2.rn + 1  
AND a1.STARTTIME <= a2.STARTTIME + INTERVAL '2' MINUTE 
WHERE a2.STARTTIME IS NULL
AND a1.ALARMCODE NOT IN ('900002', '900006', '900007') 
ORDER BY a1.STARTTIME", date, EQID);
                alarmCachedList = db.SqlQueryable<EquipmentAlarm>(sql_new).ToList();
                RedisHelper.SetList(cacheKey_historyAlarms, alarmCachedList, TimeSpan.FromHours(4));
            }


            var alarmtotal = alarmCachedList.GroupBy(
                    it => new
                    {
                        //Date = it.Date.Split(' ')[0],
                        EQID = it.AlarmEqp,
                        AlarmCode = it.AlarmCode,
                        AlarmText = it.AlarmText
                    }).Select(it => new
                    {
                        //Date = it.Key.Date,
                        EQID = it.Key.EQID,
                        AlarmCode = it.Key.AlarmCode,
                        AlarmText = it.Key.AlarmText,
                        Counts = it.Count()
                    }).OrderBy(it => it.Counts).ToList();


            return Json(new { alarmtotal, alarmList = alarmCachedList.OrderByDescending(it => it.AlarmTime) });
        }
        public JsonResult GetEQPHisRunrate(string EQID, string datetime, string starttime)
        {
            var db = DbFactory.GetSqlSugarClient();
            var date = datetime.IsNullOrEmpty() ? DateTime.Now : Convert.ToDateTime(datetime);
            if (date.Date == DateTime.Today) date = date.AddDays(-1);
            var startdate = starttime.IsNullOrEmpty() ? date.AddDays(-7) : Convert.ToDateTime(starttime);

            var dateSpan = (int)(date - startdate).TotalDays;


            var redisdb = RedisHelper.Database;
            string cacheKey_historyLoadingrate = $"{EQID}_{date.ToShortDateString()}_{dateSpan}_historyLoadingrate";
            var loadingRateCachedList = RedisHelper.GetList<DataResult>(cacheKey_historyLoadingrate);

            if (loadingRateCachedList == null)
            {

                var sql = string.Format(@"SELECT TO_CHAR(UPDATETIME,'yyyy/MM/DD') name, TO_NUMBER(VALUE) value 
FROM EQUIPMENTPARAMSHISCAL
WHERE UPDATETIME >= to_date( '{0}', 'yyyy-MM-DD HH24:MI:SS' )- INTERVAL '{1}' DAY
AND UPDATETIME <= TRUNC( to_date( '{0}', 'yyyy-MM-DD HH24:MI:SS' ) +1) + 5 / 24 
AND EQID = '{2}'
AND NAME = 'loading rate'
", date, dateSpan, EQID);
                var runrateList = db.SqlQueryable<DataResult>(sql).ToList();

                var existingDates = runrateList.Select(item => DateTime.Parse(item.name)).ToHashSet();

                // 遍历日期范围，找出缺失的日期

                for (DateTime i = startdate; i <= date; i = i.AddDays(1))
                {
                    if (!existingDates.Contains(i))
                    {
                        var runrateDate = i.ToString("yyyy/MM/dd");

                        var runrateVal = GetEQPRunrateByDate(EQID, runrateDate);// as Dictionary<string,object>;
                        JavaScriptSerializer jsStr = new JavaScriptSerializer();
                        var str = jsStr.Serialize(runrateVal.Data);
                        var tmp = JObject.Parse(str);
                        runrateList.Add(new DataResult
                        {
                            name = runrateDate,
                            value = (decimal)tmp["loadingRate"]
                        });
                    }
                }

                loadingRateCachedList = runrateList;
                RedisHelper.SetList(cacheKey_historyLoadingrate, loadingRateCachedList, TimeSpan.FromHours(8));
            }



            return Json(new { runrateList = loadingRateCachedList.OrderBy(it => it.name) });
        }
        public JsonResult GetEQPHisDataOutput(string EQID, string datetime, string starttime)
        {
            var db = DbFactory.GetSqlSugarClient();

            var date = datetime.IsNullOrEmpty() ? DateTime.Now : Convert.ToDateTime(datetime).AddHours(5);

            if (date.Date == DateTime.Today) date = date.AddDays(-1);
            var startdate = starttime.IsNullOrEmpty() ? date.AddDays(-7) : Convert.ToDateTime(starttime).AddHours(5);

            var dateSpan = (date - startdate).TotalDays;

            string cacheKey_historyOutput = $"{EQID}_{date.ToShortDateString()}_{dateSpan}_historyOutput";
            var outputCachedList = RedisHelper.GetList<DataResult>(cacheKey_historyOutput);
            if (outputCachedList == null)
            {
                var sql = string.Format(@"SELECT
    TO_CHAR(START_TIME, 'YYYY-MM-DD') AS name, COUNT(*) AS value
FROM (
    SELECT
        CASE
            WHEN TO_CHAR(UPDATETIME, 'HH24:MI:SS') >= '05:00:00' THEN TRUNC(UPDATETIME) + 5/24
            ELSE TRUNC(UPDATETIME - 1) + 5/24
        END AS START_TIME
    FROM ""EQUIPMENTPARAMSHISRAW""
    WHERE UPDATETIME >=to_date('{0}','yyyy-MM-DD HH24:MI:SS') - INTERVAL '{1}' DAY
      AND UPDATETIME < TRUNC(to_date('{0}','yyyy-MM-DD HH24:MI:SS') +1) + 5/24
			AND EQID = '{2}'
			AND NAME = 'output'
) 
GROUP BY TO_CHAR(START_TIME, 'YYYY-MM-DD')
ORDER BY name ASC;", date, dateSpan, EQID);

                outputCachedList = db.SqlQueryable<DataResult>(sql).ToList();
                RedisHelper.SetList(cacheKey_historyOutput, outputCachedList, TimeSpan.FromHours(8));
            }

            string cacheKey_historySuccess = $"{EQID}_{date.ToShortDateString()}_{dateSpan}_historySuccess";
            var successCachedList = RedisHelper.GetList<SuccessFail>(cacheKey_historySuccess);
            if (successCachedList == null)
            {
                var sql_Count = string.Format(@"SELECT 
    TRUNC(CASE 
        WHEN TO_CHAR(UPDATETIME, 'HH24') < 5 THEN UPDATETIME - INTERVAL '1' DAY 
        ELSE  UPDATETIME
    END) + INTERVAL '5' HOUR AS period_start,
    SUM(CASE WHEN name = 'output' THEN 1 ELSE 0 END) AS success_count,
    SUM(CASE WHEN name = 'FailCount' THEN 1 ELSE 0 END) AS fail_count,
    COUNT(*) AS total_count
FROM EQUIPMENTPARAMSHISRAW
WHERE UPDATETIME >= to_date( '{0}', 'yyyy-MM-DD HH24:MI:SS' ) - INTERVAL '{1}' DAY
AND UPDATETIME < TRUNC( to_date( '{0}', 'yyyy-MM-DD HH24:MI:SS' ) +1) + 5 / 24 
AND EQID = '{2}'
GROUP BY 
    TRUNC(CASE 
        WHEN TO_CHAR(UPDATETIME, 'HH24') < 5 THEN UPDATETIME - INTERVAL '1' DAY 
        ELSE UPDATETIME 
    END) + INTERVAL '5' HOUR
ORDER BY period_start;", date, dateSpan, EQID);
                successCachedList = db.SqlQueryable<SuccessFail>(sql_Count).ToList();
            }


            var success_fail_list = successCachedList.Select(it => new
            {

                Date = it.period_start.ToString("yyyy/MM/dd"),

                SuccessRatio = ((1 - it.fail_count / it.total_count) * 100).ToString("0.00")

            }).ToList();



            return Json(new { result = outputCachedList, success_fail_list = success_fail_list.OrderBy(it => it.Date) });
        }
        public JsonResult GetEQPHisDataAlarm(string EQID, string datetime, string starttime)
        {
            var db = DbFactory.GetSqlSugarClient();

            var date = datetime.IsNullOrEmpty() ? DateTime.Now : Convert.ToDateTime(datetime);
            if (date.Date != DateTime.Today) date = date.AddDays(1);
            var startdate = starttime.IsNullOrEmpty() ? date.AddDays(-7) : Convert.ToDateTime(starttime);

            var dateSpan = (date - startdate).TotalDays;

            var redisdb = RedisHelper.Database;
            string cacheKey_historyAlarm = $"{EQID}_{date.ToShortDateString()}_{dateSpan}_historyAlarm";
            var alarmCachedList = RedisHelper.GetList<DataResult>(cacheKey_historyAlarm);

            if (alarmCachedList == null)
            {

                var sql_filter_repeat_alarm = string.Format(@"SELECT TO_CHAR( START_TIME, 'YYYY-MM-DD' ) AS name,COUNT( * ) AS value 
FROM(
WITH AlarmTimes AS (
    SELECT 
        EQID,
        ALARMCODE,
				ALARMTEXT,
        ALARMSET,
        ALARMTIME,
        ROW_NUMBER() OVER (PARTITION BY EQID, ALARMCODE ORDER BY ALARMTIME) AS rn
    FROM EQUIPMENTALARM
    WHERE  ALARMTIME >= TRUNC( to_date( '{0}', 'yyyy-MM-DD HH24:MI:SS' ) - {2} ) + 5 / 24 
		AND ALARMTIME < TRUNC( to_date( '{0}', 'yyyy-MM-DD HH24:MI:SS' ) ) + 5 / 24 

		AND EQID = '{1}' 
		AND ALARMCODE <> '900002' 
		AND ALARMCODE <> '900006' 
		AND ALARMCODE <> '900007' 
),
FilteredAlarm AS (SELECT 
    
    AT1.ALARMCODE,
		AT1.ALARMTEXT,
		AT1.ALARMTIME,
    AT1.ALARMTIME AS START_TIME,
		AT2.ALARMTIME AS END_TIME,
		ROW_NUMBER ( ) OVER ( PARTITION BY AT1.ALARMCODE ORDER BY AT1.ALARMTIME ) AS rn 
    --COALESCE(AT2.ALARMTIME, TRUNC( to_date( '2024/10/29 5:00:00' ) +1) + INTERVAL '5' HOUR) AS END_TIME
FROM AlarmTimes AT1
LEFT JOIN AlarmTimes AT2
    ON AT1.EQID = AT2.EQID 
    AND AT1.ALARMCODE = AT2.ALARMCODE
    AND AT2.ALARMSET = 0
    AND AT2.rn = AT1.rn +1
WHERE AT1.ALARMSET = 1
AND AT2.ALARMTIME IS NOT NULL
ORDER BY START_TIME) SELECT
	CASE
		WHEN
			TO_CHAR( a1.ALARMTIME, 'HH24:MI:SS' ) >= '05:00:00' THEN
				TRUNC( a1.ALARMTIME ) + 5 / 24 ELSE TRUNC( a1.ALARMTIME - 1 ) + 5 / 24 
				END AS START_TIME , a1.ALARMTIME,a1.ALARMCODE
		FROM FilteredAlarm a1
		LEFT JOIN FilteredAlarm a2 ON a1.ALARMCODE = a2.ALARMCODE 
AND a1.rn = a2.rn + 1 
AND a1.START_TIME <= a2.START_TIME + INTERVAL '2' MINUTE 
WHERE a2.START_TIME IS NULL 
		) 
	GROUP BY
		TO_CHAR(START_TIME, 'YYYY-MM-DD') 
	ORDER BY
		name ASC;", date, EQID, dateSpan);

                var sql_filter_repeat_alarm_new = string.Format(@"SELECT TO_CHAR(DATETIME,'yyyy-MM-DD') AS name, COUNT( * ) AS value 
FROM( WITH AlarmTimes AS (
    SELECT 
        EQID,
        ALARMCODE,
        ALARMTEXT,
        ALARMSET,
        ALARMTIME,
        ROW_NUMBER() OVER (PARTITION BY EQID, ALARMCODE ORDER BY ALARMTIME) AS rn
    FROM EQUIPMENTALARM
    WHERE  
        ALARMTIME >=  TRUNC(TO_DATE('{0}', 'yyyy-MM-DD HH24:MI:SS')-{2})+5/24
        AND ALARMTIME < TRUNC(TO_DATE('{0}', 'yyyy-MM-DD HH24:MI:SS'))+5/24
        AND UPPER(EQID) = '{1}'
),
FilteredAlarm AS (
    SELECT 
        AT1.EQID,
        AT1.ALARMCODE,
        AT1.ALARMTEXT,
        AT1.ALARMTIME AS START_TIME,
        AT2.ALARMTIME AS END_TIME,
        ROW_NUMBER() OVER (PARTITION BY AT1.ALARMCODE ORDER BY AT1.ALARMTIME) AS rn 
    FROM AlarmTimes AT1
    LEFT JOIN AlarmTimes AT2
        ON AT1.EQID = AT2.EQID 
        AND AT1.ALARMCODE = AT2.ALARMCODE
        AND AT2.ALARMSET = 0
        AND AT2.rn = AT1.rn + 1
    WHERE AT1.ALARMSET = 1
    AND AT2.ALARMTIME IS NOT NULL
    ORDER BY START_TIME
),
FinalAlarmData AS (
    SELECT
        a1.EQID,
        a1.ALARMCODE AS AlarmCode,
        a1.ALARMTEXT AS AlarmText,
        a1.START_TIME AS StartTime,
        a1.END_TIME AS EndTime,
        EXTRACT(SECOND FROM (a1.END_TIME - a1.START_TIME)) AS Duration,
				ROW_NUMBER() OVER (PARTITION BY a1.ALARMCODE ORDER BY a1.START_TIME) AS rn ,
        CASE
            WHEN TO_CHAR(a1.START_TIME, 'HH24:MI:SS') >= '05:00:00' THEN
                TRUNC(a1.START_TIME) + 5 / 24 
            ELSE 
                TRUNC(a1.START_TIME - 1) + 5 / 24 
        END AS AlarmDate
    FROM FilteredAlarm a1
    LEFT JOIN FilteredAlarm a2 
        ON a1.ALARMCODE = a2.ALARMCODE 
        AND a1.rn = a2.rn + 1
				--AND a1.START_TIME <= a2.START_TIME + INTERVAL '2' MINUTE 
    WHERE
        -- 排除接续于90002, 90006或90007的报警
        NOT EXISTS (
            SELECT 1 
            FROM FilteredAlarm fa 
            WHERE fa.ALARMCODE IN ('900002', '900006', '900007') 
              AND fa.END_TIME >= a1.START_TIME 
              AND fa.START_TIME < a1.START_TIME
        )
        -- 排除独立且自身不作为接续的90002, 90006, 90007报警
        AND (
            a1.ALARMCODE NOT IN ('900002', '900006', '900007') 
            OR (a1.ALARMCODE IN ('900002', '900006', '900007') 
                AND NOT EXISTS (
                    SELECT 1 
                    FROM FilteredAlarm fa2 
                    WHERE fa2.START_TIME > a1.START_TIME
                      AND fa2.START_TIME <= a1.END_TIME
                )
            )
        )
				--AND a2.START_TIME IS NULL 
    ORDER BY StartTime
)
SELECT 
a1.EQID,
a1.ALARMDATE AS DATETIME,
a1.ALARMCODE,
a1.ALARMTEXT,
a1.STARTTIME,
a1.ENDTIME,
a1.DURATION,
a1.ALARMDATE
FROM FinalAlarmData a1
LEFT JOIN FinalAlarmData a2 ON a1.ALARMCODE = a2.ALARMCODE
AND a1.rn = a2.rn + 1  
AND a1.STARTTIME <= a2.STARTTIME + INTERVAL '2' MINUTE 
WHERE a2.STARTTIME IS NULL
AND a1.ALARMCODE NOT IN ('900002', '900006', '900007') 
--ORDER BY a1.STARTTIME
)
GROUP BY
		DATETIME
	ORDER BY
		name ASC;", date, EQID, dateSpan);
                var result = db.SqlQueryable<DataResult>(sql_filter_repeat_alarm_new).ToList();
                alarmCachedList = result;
                RedisHelper.SetList(cacheKey_historyAlarm, alarmCachedList, TimeSpan.FromHours(4));
            }


            return Json(new { result = alarmCachedList });
        }

        public JsonResult GetEQPHisDownRate(string EQID, string datetime, string starttime)
        {
            var db = DbFactory.GetSqlSugarClient();
            var date = datetime.IsNullOrEmpty() ? DateTime.Now : Convert.ToDateTime(datetime);
            if (date.Date == DateTime.Today) date = date.AddDays(-1);
            var startdate = starttime.IsNullOrEmpty() ? date.AddDays(-7) : Convert.ToDateTime(starttime);

            var dateSpan = (date - startdate).TotalDays;

            var redisdb = RedisHelper.Database;
            string cacheKey_historyDownrate = $"{EQID}_{date.ToShortDateString()}_{dateSpan}_historyDownRate";
            var downRateCachedList = RedisHelper.GetList<DataResult>(cacheKey_historyDownrate);

            if (downRateCachedList == null)
            {

                var sql = string.Format(@"SELECT TO_CHAR(UPDATETIME,'yyyy/MM/DD') name, TO_NUMBER(VALUE) value 
FROM EQUIPMENTPARAMSHISCAL
WHERE UPDATETIME >= to_date( '{0}', 'yyyy-MM-DD HH24:MI:SS' ) - INTERVAL '{1}' DAY
AND UPDATETIME <= TRUNC( to_date( '{0}', 'yyyy-MM-DD HH24:MI:SS' ) +1) + 5 / 24 
AND EQID = '{2}'
AND NAME = 'down rate'
", date, dateSpan, EQID);

                List<DataResult> totalAlarmDurationPerDay = db.SqlQueryable<DataResult>(sql).ToList();

                var existingDates = totalAlarmDurationPerDay.Select(item => DateTime.Parse(item.name)).ToHashSet();

                // 遍历日期范围，找出缺失的日期

                for (DateTime i = startdate; i <= date; i = i.AddDays(1))
                {
                    if (!existingDates.Contains(i))
                    {
                        var downrateDate = i.ToString("yyyy/MM/dd");

                        var downrateVal = GetEQPDownrateByDate(EQID, downrateDate);// as Dictionary<string,object>;
                        JavaScriptSerializer jsStr = new JavaScriptSerializer();
                        var str = jsStr.Serialize(downrateVal.Data);
                        var tmp = JObject.Parse(str);
                        totalAlarmDurationPerDay.Add(new DataResult
                        {
                            name = downrateDate,
                            value = (decimal)tmp["DownRate"]
                        });
                    }
                }

                downRateCachedList = totalAlarmDurationPerDay;
                RedisHelper.SetList(cacheKey_historyDownrate, downRateCachedList, TimeSpan.FromHours(8));
            }







            return Json(new { totalAlarmDurationPerDay = downRateCachedList.OrderBy(it => it.name) });
        }

        public JsonResult GetEQPHisSuccessRate(string EQID, string datetime, string starttime)
        {
            var db = DbFactory.GetSqlSugarClient();
            var date = datetime.IsNullOrEmpty() ? DateTime.Now : Convert.ToDateTime(datetime);

            if (date.Date == DateTime.Today) date = date.AddDays(-1);
            var startdate = starttime.IsNullOrEmpty() ? date.AddDays(-7) : Convert.ToDateTime(starttime);

            var dateSpan = (date - startdate).TotalDays;


            var sql_Count = string.Format(@"SELECT 
    TRUNC(CASE 
        WHEN TO_CHAR(UPDATETIME, 'HH24') < 5 THEN UPDATETIME - INTERVAL '1' DAY 
        ELSE  UPDATETIME
    END) + INTERVAL '5' HOUR AS period_start,
    SUM(CASE WHEN name = 'output' THEN 1 ELSE 0 END) AS success_count,
    SUM(CASE WHEN name = 'FailCount' THEN 1 ELSE 0 END) AS fail_count,
    COUNT(*) AS total_count
FROM EQUIPMENTPARAMSHISRAW
WHERE UPDATETIME >= to_date( '{0}', 'yyyy-MM-DD HH24:MI:SS' ) - INTERVAL '{1}' DAY
AND UPDATETIME < TRUNC( to_date( '{0}', 'yyyy-MM-DD HH24:MI:SS' ) +1) + 5 / 24 
AND EQID = '{2}'
GROUP BY 
    TRUNC(CASE 
        WHEN TO_CHAR(UPDATETIME, 'HH24') < 5 THEN UPDATETIME - INTERVAL '1' DAY 
        ELSE UPDATETIME 
    END) + INTERVAL '5' HOUR
ORDER BY period_start;", date, dateSpan, EQID);
            var success_fail_result = db.SqlQueryable<SuccessFail>(sql_Count).ToList();

            var success_fail_list = success_fail_result.Select(it => new
            {

                Date = it.period_start.ToString("yyyy/MM/dd"),

                SuccessRatio = ((1 - it.fail_count / it.total_count) * 100).ToString("0.00")

            }).ToList();



            return Json(new { success_fail_list = success_fail_list.OrderBy(it => it.Date) });
        }
        public FileContentResult DownloadReportData(List<string> eqps, string datetime, string starttime, List<string> reports)//FileContentResult
        {
            var user = (EAP.Dashboard.Models.FrameworkUsers)Session["user_account"];
            var db = DbFactory.GetSqlSugarClient();
            var datePreFix = Convert.ToDateTime(datetime).ToString("yyyyMMdd");

            var targetDate = Convert.ToDateTime(datetime).AddHours(5);
            var startdate = starttime.IsNullOrEmpty() ? targetDate.AddDays(-7) : Convert.ToDateTime(starttime).AddHours(5);
            int cyclingTimes = (int)(targetDate - startdate).TotalDays;
            //if (cyclingTimes == 0) cyclingTimes++;
            // 创建一个临时目录
            string tempDirectory = Path.Combine("\\\\127.0.0.1\\e$\\tmp\\拆料头报表", Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDirectory);
            
            foreach (var eqp in eqps)
            {
                string subDirectory = Path.Combine(tempDirectory, eqp);
                Directory.CreateDirectory(subDirectory);

                foreach (var report in reports)
                {
                    
                    Log.Info($"{user.Name}-Downloading - {eqp} - {report}: {starttime} ~ {datetime}");

                    DashBoardService.GenerateReport(eqp, targetDate, cyclingTimes, report, subDirectory ,5);
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


                //Directory.Delete(tempDirectory, true);
                Log.Info($"Download Complete.");
                // 返回文件内容作为FileResult
                return new FileContentResult(memoryStream.ToArray(), "application/zip")
                {
                    FileDownloadName = $"{datePreFix}_报表.zip"
                };
            }

        }
        public class DataResult
        {
            public string name { get; set; }
            public decimal value { get; set; }
        }
    }
}