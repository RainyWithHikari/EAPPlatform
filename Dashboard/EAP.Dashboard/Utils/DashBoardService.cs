
using CsvHelper;
using EAP.Dashboard.Models;
using EAP.Models.Database;
using EAP.Models.Database.WMS;
using MathNet.Numerics;
using Microsoft.Kiota.Abstractions;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Drawing.Chart.Style;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SqlSugar;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Script.Serialization;
using static EAP.Dashboard.Controllers.DMaterialDashboardController;
using static EAP.Dashboard.Controllers.RIDMController;

namespace EAP.Dashboard.Utils
{
    public class DashBoardService
    {
        private static log4net.ILog Log = log4net.LogManager.GetLogger("Logger");
        public class ParamsList
        {
            public string EQID { get; set; }
            public string OUTPUT { get; set; }
            public string YIELD { get; set; }
            public DateTime DATE { get; set; }

        }
        public static List<DetailChartData> GetChartData(string equipment, string datetime)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
            var db = DbFactory.GetSqlSugarClient();
            DateTime starttime = new DateTime();
            DateTime chartstarttime = new DateTime();
            DateTime endtime = new DateTime();
            DateTime nowtime = DateTime.Now;
            if (!datetime.StartsWith("W"))
            {
                if (nowtime.Date == Convert.ToDateTime(datetime).Date)
                {
                    starttime = DateTime.Now.AddHours(-48);
                    chartstarttime = DateTime.Now.AddHours(-24);
                    endtime = DateTime.Now;
                }
                else
                {

                    chartstarttime = Convert.ToDateTime(datetime + " 09:00:00");
                    starttime = chartstarttime.AddHours(-48);
                    endtime = Convert.ToDateTime(datetime + " 09:00:00").AddDays(1);
                    nowtime = DateTime.Now;
                }

            }
            else
            {
                var startdateList = db.Queryable<EquipmentParamsHistoryCalculate>()
              .Where(it => it.Name == "starttime"
              && it.Remark == datetime
              && it.EQID == "ACCBOX").ToList();
                var enddateList = db.Queryable<EquipmentParamsHistoryCalculate>()
                    .Where(it => it.Name == "endtime"
                    && it.Remark == datetime
                    && it.EQID == "ACCBOX").ToList();
                starttime = Convert.ToDateTime(startdateList.OrderByDescending(it => Convert.ToDateTime(it.Value)).First().Value);
                endtime = Convert.ToDateTime(enddateList.OrderByDescending(it => Convert.ToDateTime(it.Value)).First().Value);
                chartstarttime = starttime;
            }

            var rawalarmdata = db.Queryable<EquipmentAlarm>().Where(it => it.AlarmEqp == equipment && it.AlarmTime > starttime && it.AlarmTime <= endtime).OrderBy(it => it.AlarmTime).ToList();
            if (endtime > nowtime) endtime = nowtime;
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
equipment, starttime.ToString("yyyy-MM-dd HH:mm:ss"), endtime.ToString("yyyy-MM-dd HH:mm:ss"));
            var statusdata = OracleHelper.GetDTFromCommand(sql);
            List<DetailChartData> chardata = new List<DetailChartData>();
            Dictionary<string, string> color = new Dictionary<string, string> { { "Run", "#75d874" }, { "Alarm", "#e3170d" }, { "Idle", "#ff9912" }, { "Offline", "#cccccc" }, { "Down", "#cccccc" }, { "Unknow", "#082e54" } };
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
                if (!color.TryGetValue(statusdata.Rows[i][0].ToString(), out periodColor) || !statusInt.TryGetValue(statusdata.Rows[i][0].ToString(), out periodStatusInt))
                {
                    periodColor = "#082e54";
                    periodStatusInt = 6;
                }
                //periodColor = color[statusdata.Rows[i][0].ToString()];
                //periodStatusInt = statusInt[statusdata.Rows[i][0].ToString()];
                timeSpan = periodEndTime - periodStartTime;
                alarmtext = GetAlarmText(rawalarmdata, equipment, periodStartTime, periodEndTime);
                chardata.Add(new DetailChartData
                {
                    name = periodStatus,
                    value = new object[3] { periodStatusInt, periodStartTime.ToString("yyyy-MM-dd HH:mm:ss"), periodEndTime.ToString("yyyy-MM-dd HH:mm:ss") },
                    itemStyle = new itemStyle { normal = new normal { color = periodColor } },
                    duration = timeSpan,
                    alarmtext = alarmtext
                });
                continue;

                #region old
                if (i == statusdata.Rows.Count - 1)//设置最后一段状态/只查到一个状态（时间还在9点之前）
                {
                    periodEndTime = endtime;
                    periodStartTime = Convert.ToDateTime(statusdata.Rows[i][1].ToString());
                    periodStatus = statusdata.Rows[i][0].ToString();
                    periodColor = color[statusdata.Rows[i][0].ToString()];
                    periodStatusInt = statusInt[statusdata.Rows[i][0].ToString()];
                    if (periodStartTime < chartstarttime) periodStartTime = chartstarttime;
                    timeSpan = periodEndTime - periodStartTime;
                    alarmtext = GetAlarmText(rawalarmdata, equipment, periodStartTime, periodEndTime);
                    chardata.Add(new DetailChartData
                    {
                        name = periodStatus,
                        value = new object[3] { periodStatusInt, periodStartTime.ToString("yyyy-MM-dd HH:mm:ss"), periodEndTime.ToString("yyyy-MM-dd HH:mm:ss") },
                        itemStyle = new itemStyle { normal = new normal { color = periodColor } },
                        duration = timeSpan,
                        alarmtext = alarmtext
                    });



                    if (i == 0) break;


                }

                if (periodEndTime < chartstarttime) continue;//筛选5点之前的事件



                if (i == 0)//前面没有筛到数据,就用后面状态
                {
                    periodStartTime = chartstarttime;
                    periodStatus = statusdata.Rows[i][0].ToString();
                    periodColor = color[statusdata.Rows[i][0].ToString()];
                    periodStatusInt = statusInt[statusdata.Rows[i][0].ToString()];
                }
                else
                {
                    periodStartTime = Convert.ToDateTime(statusdata.Rows[i - 1][1].ToString());
                    periodEndTime = Convert.ToDateTime(statusdata.Rows[i][1].ToString());
                    periodStatus = statusdata.Rows[i - 1][0].ToString();
                    periodColor = color[statusdata.Rows[i - 1][0].ToString()];
                    periodStatusInt = statusInt[statusdata.Rows[i - 1][0].ToString()];
                }
                if (periodStartTime < chartstarttime) periodStartTime = chartstarttime;
                timeSpan = periodEndTime - periodStartTime;
                alarmtext = GetAlarmText(rawalarmdata, equipment, periodStartTime, periodEndTime);
                chardata.Add(new DetailChartData
                {
                    name = periodStatus,
                    value = new object[3] { periodStatusInt, periodStartTime.ToString("yyyy-MM-dd HH:mm:ss"), periodEndTime.ToString("yyyy-MM-dd HH:mm:ss") },
                    itemStyle = new itemStyle { normal = new normal { color = periodColor } },
                    duration = timeSpan,
                    alarmtext = alarmtext
                });
                #endregion
            }
            return chardata;
        }

        private static string GetAlarmText(List<EquipmentAlarm> rawalarm, string equipment, DateTime periodStartTime, DateTime periodEndTime)
        {
            string alarmtext = string.Empty;
            try
            {
                var alarmdata = rawalarm.Where(it => it.AlarmEqp == equipment && it.AlarmTime > periodStartTime && it.AlarmTime <= periodEndTime).OrderBy(it => it.AlarmTime)
               .Select(it => new { ALARMTIME = it.AlarmTime, ALARMTEXT = it.AlarmText }).ToList();
                foreach (var item in alarmdata)
                {
                    alarmtext += Convert.ToDateTime(item.ALARMTIME).ToString("yyyy-MM-dd HH:mm:ss") + " " + item.ALARMTEXT;
                    alarmtext += "<br>";
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }

            return alarmtext;
        }

        public static DataTable ToDataTable<T>(IEnumerable<T> list)
        {
            //Transfer List to DataTable
            PropertyInfo[] modelItemType = typeof(T).GetProperties();
            DataTable dataTable = new DataTable();
            dataTable.Columns.AddRange(modelItemType.Select(Columns => new DataColumn(Columns.Name, Nullable.GetUnderlyingType(Columns.PropertyType) ?? Columns.PropertyType)).ToArray());
            if (list.Count() > 0)
            {
                for (int i = 0; i < list.Count(); i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in modelItemType)
                    {
                        object obj = pi.GetValue(list.ElementAt(i), null);
                        tempList.Add(obj);
                    }
                    object[] dataRow = tempList.ToArray();
                    dataTable.LoadDataRow(dataRow, true);
                }
            }
            return dataTable;
        }

        public static List<T> TableToListModel<T>(DataTable dt) where T : new()
        {
            // 定义集合    
            List<T> ts = new List<T>();

            // 获得此模型的类型   
            Type type = typeof(T);
            string tempName = "";

            foreach (DataRow dr in dt.Rows)
            {
                T t = new T();
                // 获得此模型的公共属性      
                PropertyInfo[] propertys = t.GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    tempName = pi.Name;  // 检查DataTable是否包含此列    

                    if (dt.Columns.Contains(tempName))
                    {
                        // 判断此属性是否有Setter      
                        if (!pi.CanWrite) continue;

                        object value = dr[tempName];
                        if (value != DBNull.Value)
                            pi.SetValue(t, value, null);
                    }
                }
                ts.Add(t);
            }
            return ts;
        }
        public static List<AlarmItem> GetAlarmDetails(DateTime startdate, DateTime enddate, string EQID, int hour)
        {
            var alarmList = new List<AlarmItem>();
            try
            {
                var db = DbFactory.GetSqlSugarClient();

                var startstr= startdate.ToString("yyyy-MM-dd HH:mm:ss");
                var endstr = enddate.ToString("yyyy-MM-dd HH:mm:ss");

                var eqguid = db.Queryable<Equipment>().Where(it => it.EQID == EQID).Select(it => it.ID).Single();
                var alarmcodeList = db.Queryable<EquipmentConfiguration>().Where(it => it.EQIDId == eqguid && it.ConfigurationItem == "Equipment.AlarmCode").Select(it => it.ConfigurationValue).First();
                String[] alarmcodes = alarmcodeList != null ? alarmcodeList.Split(',') : new String[0];


                var sql_total = string.Format(@"WITH AlarmTimes AS (
                            SELECT 
                                EQID,
                                ALARMCODE,
		                        ALARMTEXT,
                                ALARMSET,
                                ALARMTIME,
                                ROW_NUMBER() OVER (PARTITION BY EQID, ALARMCODE ORDER BY ALARMTIME) AS rn
                            FROM EQUIPMENTALARM
                            WHERE  ALARMTIME >= TO_DATE( '{0}', 'yyyy-mm-dd hh24:mi:ss' )
		                        AND ALARMTIME <= TO_DATE( '{1}', 'yyyy-mm-dd hh24:mi:ss' )
		                        AND UPPER(EQID) = '{2}' 
                        ),
                        FilteredAlarm AS (SELECT 
                            AT1.EQID,
                            AT1.ALARMCODE,
		                        AT1.ALARMTEXT,
                            AT1.ALARMTIME AS START_TIME,
		                        AT2.ALARMTIME AS END_TIME,
		                        ROW_NUMBER ( ) OVER ( PARTITION BY AT1.ALARMCODE ORDER BY AT1.ALARMTIME ) AS rn 
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
                        a1.ALARMCODE AS AlarmCode,
                        a1.ALARMTEXT AS AlarmText,
                        a1.START_TIME AS StartTime,
                        a1.END_TIME AS EndTime,
                        --ROUND(EXTRACT(SECOND FROM (a1.END_TIME -a1.START_TIME) )/60,2) AS Duration
                        ROUND((CAST(a1.END_TIME AS DATE) - CAST(a1.START_TIME AS DATE)) * 86400/60,2) AS Duration
                        FROM FilteredAlarm a1
                        LEFT JOIN FilteredAlarm a2 ON a1.ALARMCODE = a2.ALARMCODE 
                        AND a1.rn = a2.rn + 1 
                        AND a1.START_TIME <= a2.START_TIME + INTERVAL '2' MINUTE 
                        WHERE a2.START_TIME IS NULL 
                        ORDER BY StartTime", startstr, endstr, EQID.ToUpper(), hour);

                var sql_total_backup = string.Format(@"WITH AlarmTimes AS (
                            SELECT 
                                EQID,
                                ALARMCODE,
		                        ALARMTEXT,
                                ALARMSET,
                                ALARMTIME,
                                ROW_NUMBER() OVER (PARTITION BY EQID, ALARMCODE ORDER BY ALARMTIME) AS rn
                            FROM EQUIPMENTALARM
                            WHERE  ALARMTIME >= to_date( '{0}', 'yyyy-MM-DD HH24:MI:SS' )
		                        AND ALARMTIME <= to_date( '{1}', 'yyyy-MM-DD HH24:MI:SS' )
		                        AND UPPER(EQID) = '{2}' 
                        ),
                        FilteredAlarm AS (SELECT 
                            AT1.EQID,
                            AT1.ALARMCODE,
		                        AT1.ALARMTEXT,
                            AT1.ALARMTIME AS START_TIME,
		                        AT2.ALARMTIME AS END_TIME,
		                        ROW_NUMBER ( ) OVER ( PARTITION BY AT1.ALARMCODE ORDER BY AT1.ALARMTIME ) AS rn 
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
                        a1.ALARMCODE AS AlarmCode,
                        a1.ALARMTEXT AS AlarmText,
                        a1.START_TIME AS StartTime,
                        a1.END_TIME AS EndTime,
                        ROUND(EXTRACT(SECOND FROM (a1.END_TIME -a1.START_TIME) )/60,2) AS Duration,
	                        CASE
		                        WHEN
			                        TO_CHAR( a1.START_TIME, 'HH24:MI:SS' ) >= '0{3}:00:00' THEN
				                        TRUNC( a1.START_TIME ) + {3} / 24 ELSE TRUNC( a1.START_TIME - 1 ) + {3} / 24 
				                        END AS AlarmDate
                        FROM FilteredAlarm a1
                        LEFT JOIN FilteredAlarm a2 ON a1.ALARMCODE = a2.ALARMCODE 
                        AND a1.rn = a2.rn + 1 
                        AND a1.START_TIME <= a2.START_TIME + INTERVAL '2' MINUTE 
                        WHERE a2.START_TIME IS NULL 
                        ORDER BY StartTime", startdate, enddate, EQID, hour);
                //var alarmDetailTable = OracleHelper.GetDTFromCommand(sql_total);
                alarmList = db.SqlQueryable<AlarmItem>(sql_total).ToList();

                //if (alarmcodes.Length > 0) alarmList = alarmList.Where(it => alarmcodes.Contains(it.AlarmCode)).ToList();
                Log.Info("GET alarm table - " + EQID +" Count:"+ alarmList.Count);
                return alarmList;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return alarmList;
            }


        }
        public static List<AlarmItem> GetAlarmDetailsOld(DateTime startdate, DateTime enddate, string EQID)
        {
            var db = DbFactory.GetSqlSugarClient();
            var alarmList = new List<AlarmItem>();
            var alarmspan = new TimeSpan();
            var date = Convert.ToDateTime(startdate).Date;

            var eqguid = db.Queryable<Equipment>().Where(it => it.EQID == EQID).Select(it => it.ID).Single();

            var alarmcodeList = db.Queryable<EquipmentConfiguration>().Where(it => it.EQIDId == eqguid && it.ConfigurationName == "AlarmCode").Select(it => it.ConfigurationValue).First();
            //var alarmcodeList = db.Queryable<EquipmentConfiguration>().Where(it => it.EQID == EQID).Select(it => it.ConfigurationValue).First();

            var eqpalarm = db.Queryable<EquipmentAlarm>().Where(it => it.AlarmEqp.ToUpper() == EQID
                    && it.AlarmTime > startdate && it.AlarmTime <= enddate
                        ).OrderByDescending(it => it.AlarmTime)
                        .Select(it => new { ALARMTIME = it.AlarmTime, ALARMCODE = it.AlarmCode, ALARMTEXT = it.AlarmText, ALARMSET = it.AlarmSet }).ToList();
            if (alarmcodeList != null)
            {
                String[] alarmcodes = alarmcodeList.Split(',');
                eqpalarm = eqpalarm.Where(it => alarmcodes.Contains(it.ALARMCODE)).ToList();//db.Queryable<EquipmentAlarm>().Where(it => it.AlarmEqp.ToUpper() == EQID
                                                                                            //&& it.AlarmTime > startdate && it.AlarmTime <= enddate
                                                                                            //&& alarmcodes.Contains(it.AlarmCode)).OrderByDescending(it => it.AlarmTime)
                                                                                            //.Select(it => new { ALARMTIME = it.AlarmTime, ALARMCODE = it.AlarmCode, ALARMTEXT = it.AlarmText, ALARMSET = it.AlarmSet }).ToList();
            }

            if (eqpalarm.Count > 0)
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
                                    alarmspan = (DateTime)eqpalarm[j].ALARMTIME - (DateTime)eqpalarm[k].ALARMTIME;
                                    AlarmItem alarmItem = new AlarmItem
                                    {
                                        AlarmDate = eqpalarm[j].ALARMTIME.ToString().Split(' ')[0],
                                        EQID = EQID,
                                        AlarmCode = eqpalarm[k].ALARMCODE,
                                        AlarmText = eqpalarm[k].ALARMTEXT,
                                        StartTime = (DateTime)eqpalarm[k].ALARMTIME,
                                        EndTime = (DateTime)eqpalarm[j].ALARMTIME,
                                        Duration = double.Parse(alarmspan.TotalMinutes.ToString("0.000"))

                                    };
                                    alarmList.Add(alarmItem);
                                    break;
                                }
                            }
                        }



                    }
                }
                else
                {
                    alarmspan = date.AddDays(1).AddHours(9) - (DateTime)eqpalarm[0].ALARMTIME;
                    for (int j = 1; j < eqpalarm.Count; j++)
                    {
                        for (int k = j + 1; k < eqpalarm.Count; k++)
                        {
                            if (eqpalarm[j].ALARMCODE == eqpalarm[k].ALARMCODE && eqpalarm[j].ALARMSET == false && eqpalarm[k].ALARMSET == true)
                            {
                                alarmspan = (DateTime)eqpalarm[j].ALARMTIME - (DateTime)eqpalarm[j + 1].ALARMTIME;
                                AlarmItem alarmItem = new AlarmItem
                                {
                                    AlarmDate = eqpalarm[j].ALARMTIME.ToString().Split(' ')[0],
                                    EQID = EQID,
                                    AlarmCode = eqpalarm[k].ALARMCODE,
                                    AlarmText = eqpalarm[k].ALARMTEXT,
                                    StartTime = (DateTime)eqpalarm[k].ALARMTIME,
                                    EndTime = (DateTime)eqpalarm[j].ALARMTIME,
                                    Duration = double.Parse(alarmspan.TotalMinutes.ToString("0.000"))

                                };
                                alarmList.Add(alarmItem);
                                break;
                            }
                        }


                    }
                }





            }
            return alarmList;

        }

        // 合并重叠的时间段，并计算总持续时间
        public static TimeSpan MergeAndCalculateDuration(List<AlarmItem> alarms)
        {
            if (alarms.Count == 0) return TimeSpan.Zero;

            // 按开始时间排序
            var sortedAlarms = alarms.OrderBy(a => a.StartTime).ToList();
            TimeSpan totalDuration = TimeSpan.Zero;

            DateTime currentStart = sortedAlarms[0].StartTime;
            DateTime currentEnd = (DateTime)sortedAlarms[0].EndTime;

            foreach (var alarm in sortedAlarms)
            {
                if (alarm.StartTime <= currentEnd) // 重叠或相邻
                {
                    // 延长结束时间
                    currentEnd = (DateTime)alarm.EndTime > currentEnd ? (DateTime)alarm.EndTime : currentEnd;
                }
                else
                {
                    // 不重叠，累加持续时间
                    totalDuration += currentEnd - currentStart;
                    currentStart = alarm.StartTime;
                    currentEnd = (DateTime)alarm.EndTime;
                }
            }

            // 最后一段持续时间
            totalDuration += currentEnd - currentStart;

            return totalDuration;
        }
        public static Dictionary<byte[], string> GenerateFiles(DateTime start, DateTime end, string parmType)
        {
            var db = DbFactory.GetSqlSugarClient();
            var eqptypeids = db.Queryable<EquipmentType>().Where(it => it.Name == "ACCBOX").Select(it => it.ID).ToList();
            var eqpidids = db.Queryable<Equipment>().Where(it => eqptypeids.Contains(it.EquipmentTypeId)).OrderBy(it => it.Sort).Select(it => it.EQID).ToList().ToList();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var filesDict = new Dictionary<byte[], string>();
            List<byte[]> fileBytesArr = new List<byte[]>();
            foreach (var eqp in eqpidids)
            {
                var ep = new ExcelPackage();
                using (MemoryStream memoryStream = new MemoryStream())
                {

                    ExcelWorkbook wb = ep.Workbook;
                    var list = db.Queryable<EquipmentParamsHistoryRaw>().Where(it => it.EQID == eqp
                    && it.Name == parmType
                    && it.UpdateTime >= start
                    && it.UpdateTime <= end).OrderBy(it => it.UpdateTime).ToList();
                    var groupedData = list.GroupBy(item => item.UpdateTime.Date)
                            .Select(group => group.ToList());
                    foreach (var dayData in groupedData)
                    {
                        // 处理同一天的数据
                        ExcelWorksheet ws = wb.Worksheets.Add(dayData[0].UpdateTime.Date.ToShortDateString());
                        ws.Cells["A1"].LoadFromDataTable(ToDataTable(dayData), true);
                        ws.Column(4).Style.Numberformat.Format = "yyyy/m/d h:mm:ss";
                    }
                    var filename = $"{eqp}_{parmType}.xlsx";
                    var datastring = ep.GetAsByteArray();
                    filesDict.Add(datastring, filename);


                }


            }
            return filesDict;
        }

        public static List<EquipmentAlarmTimes> GetAlarmTimeList(string eqpType)
        {
            var db = DbFactory.GetSqlSugarClient();
            var alarmArr = new List<EquipmentAlarmTimes>();

            var eqptypeids = db.Queryable<EquipmentType>().Where(it => it.Name == eqpType).Select(it => it.ID).ToList();
            var eqpidids = db.Queryable<Equipment>().Where(it => eqptypeids.Contains(it.EquipmentTypeId)).OrderBy(it => it.Sort).Select(it => it.EQID).ToList();
            var weeks = db.Queryable<EquipmentParamsHistoryCalculate>()
                .Where(it => it.EQID == eqpType && it.Remark != null && it.Name == "starttime")
                .Select(it => it.Remark)
                .Distinct()
                .ToList()
                .OrderByDescending(it => Convert.ToDouble(it.Replace("W", ""))).Take(24)
                .ToList()
                .OrderBy(it => Convert.ToDouble(it.Replace("W", "")));
            //.OrderBy(it => Convert.ToDouble(it.Split("W")[1]));//.TakeLast(8);
            foreach (var item in weeks)
            {
                var mfgspan = new TimeSpan();
                List<DateTime> startdateList = db.Queryable<EquipmentParamsHistoryCalculate>()
                .Where(it => it.Name == "starttime"
                && it.Remark == item
                && it.EQID == eqpType).ToList()
                .OrderBy(it => Convert.ToDateTime(it.Value)).Select(it => Convert.ToDateTime(it.Value)).ToList();
                List<DateTime> enddateList = db.Queryable<EquipmentParamsHistoryCalculate>()
                    .Where(it => it.Name == "endtime"
                    && it.Remark == item
                    && it.EQID == eqpType).ToList()
                    .OrderBy(it => Convert.ToDateTime(it.Value)).Select(it => Convert.ToDateTime(it.Value)).ToList();
                var idleTime = db.Queryable<EquipmentParamsHistoryCalculate>()
                    .Where(it => it.Name == "idleduration"
                    && it.Remark == item
                    && it.EQID == eqpType).Sum(it => Convert.ToDouble(it.Value));


                for (int i = 0; i < startdateList.Count; i++)
                {
                    mfgspan += enddateList[i] - startdateList[i];
                }


                //List<double> alarmrates = new();
                var alarmtimes = new List<double>();

                double alarmtime = 0;

                alarmtimes = db.Queryable<EquipmentParamsHistoryCalculate>()
                    .Where(it => eqpidids.Contains(it.EQID) && it.Remark == item && it.Name == "alarmspan")
                    .Select(it => Convert.ToDouble(it.Value)).ToList();
                foreach (var time in alarmtimes)
                {
                    alarmtime += time;
                }
                double alarmrate = (alarmtime / (mfgspan.TotalMinutes - idleTime)) * 100;
                double restTime = mfgspan.TotalMinutes - idleTime - alarmtime;
                var alarmItem = new EquipmentAlarmTimes() { time = item, alarmtimes = alarmtime, mfgtimes = mfgspan.TotalMinutes - idleTime };
                alarmArr.Add(alarmItem);

            }

            return alarmArr;
        }


        // 时间裁剪，确保时间不越界

        private static DateTime Clamp(DateTime value, DateTime min, DateTime max)

        {

            return value < min ? min : (value > max ? max : value);

        }

        // 统一统计核心

        private static double CalculateDownTime(List<AlarmItem> alarms, DateTime periodStart, DateTime periodEnd, int overlapThreshold = 2)

        {

            var events = new List<(DateTime Time, int Change)>();

            foreach (var alarm in alarms)

            {

                DateTime start = Clamp(alarm.StartTime, periodStart, periodEnd);

                DateTime end = Clamp(alarm.EndTime ?? periodEnd, periodStart, periodEnd);

                if (start < end)

                {

                    events.Add((start, +1));

                    events.Add((end, -1));

                }

            }

            events.Sort((a, b) => a.Time.CompareTo(b.Time));

            int active = 0;

            DateTime? downStart = null;

            double downMinutes = 0;

            foreach (var e in events)

            {

                active += e.Change;

                if (active >= overlapThreshold && !downStart.HasValue)

                {

                    downStart = e.Time;

                }

                else if (active < overlapThreshold && downStart.HasValue)

                {

                    downMinutes += (e.Time - downStart.Value).TotalMinutes;

                    downStart = null;

                }

            }

            if (downStart.HasValue)

                downMinutes += (periodEnd - downStart.Value).TotalMinutes;

            return Math.Round(downMinutes, 2);

        }

        public static List<StatusTimePoint> CalculateHourlyDownTime(List<AlarmItem> alarms, DateTime targetDate, DateTime? currentTime = null)

        {

            var results = new List<StatusTimePoint>();

            DateTime dayStart = targetDate.Date;

            DateTime now = currentTime ?? DateTime.Now;

            DateTime endLimit = (targetDate.Date == now.Date) ? now : dayStart.AddDays(1);

            int totalHours = (int)(endLimit - dayStart).TotalHours;

            if (totalHours > 24) totalHours = 24;

            for (int hour = 0; hour < totalHours; hour++)

            {

                DateTime periodStart = dayStart.AddHours(hour);

                DateTime periodEnd = periodStart.AddHours(1);

                if (periodEnd > endLimit) periodEnd = endLimit;

                var periodAlarms = alarms.Where(a => a.StartTime < periodEnd && (a.EndTime ?? periodEnd) > periodStart).ToList();

                double downMinutes = CalculateDownTime(periodAlarms, periodStart, periodEnd);

                results.Add(new StatusTimePoint

                {

                    TimeLabel = periodStart.ToString("HH:00"),

                    Minutes = downMinutes

                });

            }

            return results;

        }
        public static List<StatusTimePoint> AggregateDailyFromHourly(List<StatusTimePoint> hourlyData, DateTime targetDate)

        {

            double totalMinutes = hourlyData.Sum(h => h.Minutes);

            return new List<StatusTimePoint>

            {

                new StatusTimePoint

                {

                    TimeLabel = targetDate.ToString("yyyy-MM-dd"),

                    Minutes = Math.Round(totalMinutes, 2)

                }

            };

        }

        public static List<StatusTimePoint> AggregateDailyFromHourlyBatch(List<AlarmItem> alarms, DateTime startDate, DateTime endDate)

        {

            var results = new List<StatusTimePoint>();

            DateTime cursor = startDate.Date;

            while (cursor <= endDate.Date)

            {

                var hourly = CalculateHourlyDownTime(alarms, cursor, cursor.AddDays(1));

                double totalMinutes = hourly.Sum(h => h.Minutes);

                results.Add(new StatusTimePoint

                {

                    TimeLabel = cursor.ToString("yyyy-MM-dd"),

                    Minutes = Math.Round(totalMinutes, 2)

                });

                cursor = cursor.AddDays(1);

            }

            return results;

        }


        public static List<StatusTimePoint> CalculateDailyDownTime(List<AlarmItem> alarms, DateTime startDate, DateTime endDate)

        {

            var results = new List<StatusTimePoint>();

            DateTime cursor = startDate.Date;

            DateTime final = endDate.Date;

            while (cursor <= final)

            {

                DateTime dayStart = cursor;

                DateTime dayEnd = cursor.AddDays(1);

                if (dayEnd > endDate) dayEnd = endDate;

                var dayAlarms = alarms.Where(a => a.StartTime < dayEnd && (a.EndTime ?? dayEnd) > dayStart).ToList();

                double downMinutes = CalculateDownTime(dayAlarms, dayStart, dayEnd);

                results.Add(new StatusTimePoint

                {

                    TimeLabel = dayStart.ToString("yyyy-MM-dd"),

                    Minutes = downMinutes

                });

                cursor = dayStart.AddDays(1);

            }

            return results;

        }

        public static List<EquipmentAlarmTimes> GetAlarmMonthlyList(List<EquipmentAlarmTimes> arr)
        {
            var alarmArr = new List<EquipmentAlarmTimes>();

            arr.ForEach(it =>
            {
                int year = Convert.ToInt32(it.time.Split('W')[0]);

                DateTime startOfYear = new DateTime(year, 1, 1);
                int offset = startOfYear.DayOfWeek == DayOfWeek.Sunday ? 6 : ((int)startOfYear.DayOfWeek - 1);

                DateTime targetWeek = startOfYear.AddDays(offset + (Convert.ToDouble(it.time.Split('W')[1]) - 1) * 7);
                int targetMonth = targetWeek.Month;
                string MonthNumber = year.ToString() + 'M' + targetMonth.ToString();
                if (!alarmArr.Any(alarm => alarm.time.Contains(MonthNumber)))
                {
                    var alarmItem = new EquipmentAlarmTimes()
                    {
                        time = MonthNumber,
                        alarmtimes = it.alarmtimes,
                        mfgtimes = it.mfgtimes
                    };
                    alarmArr.Add(alarmItem);
                }
                else
                {
                    alarmArr.FirstOrDefault(alarm => alarm.time.Contains(MonthNumber)).alarmtimes += it.alarmtimes;
                    alarmArr.FirstOrDefault(alarm => alarm.time.Contains(MonthNumber)).mfgtimes += it.mfgtimes;

                }
            });
            return alarmArr;
        }

        
        public static List<StatusTimePoint> CalculateHourlyDownTimeold(List<AlarmItem> alarms,DateTime date)
        {
            var results = new List<StatusTimePoint>();
            
            DateTime now = DateTime.Now;
            DateTime dayStart = now.Date;
            if (date != DateTime.Today)
            {
                now = date.AddHours(23);
                dayStart = date;
            }

            for (int hour = 0; hour <= now.Hour; hour++)
            {
                DateTime periodStart = dayStart.AddHours(hour);
                DateTime periodEnd = periodStart.AddHours(1);

                // 筛选出该小时内活跃的报警
                var periodAlarms = alarms
                    .Where(a => a.StartTime < periodEnd && (a.EndTime != null || a.EndTime > periodStart))
                    .ToList();

                // 生成时间点（+1 报警开始，-1 报警结束）
                var events = new List<(DateTime Time, int Change)>();

                foreach (var alarm in periodAlarms)
                {
                    DateTime start = alarm.StartTime < periodStart ? periodStart : alarm.StartTime;
                    DateTime end = alarm.EndTime.GetValueOrDefault(periodEnd);

                    if (start < periodEnd)
                        events.Add((start, +1));
                    if (end < periodEnd)
                        events.Add((end, -1));
                }

                events.Sort((a, b) => a.Time.CompareTo(b.Time));

                int active = 0;
                DateTime? downStart = null;
                double downMinutes = 0;

                foreach (var e in events)
                {
                    active += e.Change;

                    if (active >= 2 && !downStart.HasValue)
                    {
                        downStart = e.Time;
                    }
                    else if (active < 2 && downStart.HasValue)
                    {
                        downMinutes += (e.Time - downStart.Value).TotalMinutes;
                        downStart = null;
                    }
                }

                if (downStart.HasValue)
                    downMinutes += (periodEnd - downStart.Value).TotalMinutes;

                results.Add(new StatusTimePoint
                {
                    TimeLabel = periodStart.ToString("HH:00"),
                    Minutes = Math.Round(downMinutes, 2)
                });
            }

            return results;
        }
        public static List<StatusTimePoint> CalculateDailyDownTime_old(List<AlarmItem> alarms,DateTime startTime,DateTime endTime)
        {
            var results = new List<StatusTimePoint>();

            int dateSpan = (int)(endTime - startTime).TotalDays;

            for (int i = dateSpan; i >= 0; i--)  // 近 7 天，含今天
            {
                DateTime dayStart = endTime.AddDays(-i);
                DateTime dayEnd = dayStart.AddDays(1);

                var dayAlarms = alarms
                    .Where(a => a.StartTime < dayEnd && (a.EndTime ?? dayEnd) > dayStart)
                    .ToList();

                var events = new List<(DateTime Time, int Change)>();

                foreach (var alarm in dayAlarms)
                {
                    DateTime start = alarm.StartTime < dayStart ? dayStart : alarm.StartTime;
                    DateTime end = (alarm.EndTime ?? dayEnd) > dayEnd ? dayEnd : (alarm.EndTime ?? dayEnd);

                    events.Add((start, +1));
                    events.Add((end, -1));
                }

                events.Sort((a, b) => a.Time.CompareTo(b.Time));

                int active = 0;
                DateTime? downStart = null;
                double downMinutes = 0;

                foreach (var e in events)
                {
                    active += e.Change;

                    if (active >= 2 && !downStart.HasValue)
                    {
                        downStart = e.Time;
                    }
                    else if (active < 2 && downStart.HasValue)
                    {
                        downMinutes += (e.Time - downStart.Value).TotalMinutes;
                        downStart = null;
                    }
                }

                if (downStart.HasValue)
                {
                    downMinutes += (dayEnd - downStart.Value).TotalMinutes;
                }

                results.Add(new StatusTimePoint
                {
                    TimeLabel = dayStart.ToString("yyyy-MM-dd"),
                    Minutes = Math.Round(downMinutes, 2)
                });
            }

            return results;
        }
        public static List<StatusTimePoint> CalculateRunIdlePerDay(DataTable statusData, DateTime periodStart, DateTime periodEnd)
        {
            var results = new List<StatusTimePoint>();

            // 整理数据
            var records = statusData.AsEnumerable()
                .Select(row => new
                {
                    Status = row.Field<string>("STATUS"),
                    Time = row.Field<DateTime>("DATETIME")
                })
                .OrderBy(r => r.Time)
                .ToList();

            // 补充开头或结尾空白（如果有）
            if (records.Count == 0)
                return results;

            // 按天统计
            for (int i = 0; i < 7; i++)
            {
                DateTime dayStart = periodEnd.Date.AddDays(-i);
                DateTime dayEnd = dayStart.AddDays(1);

                double totalMinutes = 0;
                string lastStatus = null;
                DateTime lastTime = dayStart;

                foreach (var rec in records)
                {
                    if (rec.Time >= dayEnd)
                        break;

                    if (rec.Time < dayStart)
                    {
                        lastStatus = rec.Status;
                        lastTime = dayStart;
                        continue;
                    }

                    if (lastStatus == "Run" || lastStatus == "Idle")
                    {
                        totalMinutes += (rec.Time - lastTime).TotalMinutes;
                    }

                    lastStatus = rec.Status;
                    lastTime = rec.Time;
                }

                // 补充当天最后一段状态
                if (lastStatus == "Run" || lastStatus == "Idle")
                {
                    totalMinutes += (dayEnd - lastTime).TotalMinutes;
                }

                results.Add(new StatusTimePoint
                {
                    TimeLabel = dayStart.ToString("yyyy-MM-dd"),
                    Minutes = Math.Round(totalMinutes, 2)
                });
            }

            // 结果倒序输出，今天在最后
            return results.OrderBy(r => r.TimeLabel).ToList();
        }


        public static bool CalculateAlarmRate(List<Equipment> eqids, DateTime starttime, DateTime endtime, string week, int idleduration)
        {
            var db = DbFactory.GetSqlSugarClient();
            try
            {
                for (int i = 0; i < eqids.Count; i++)
                {
                    var eqid = eqids[i].EQID.ToString();
                    var eqguid = eqids[i].ID;


                    TimeSpan mfgtime = endtime - starttime;
                    var alarmspan = new TimeSpan();
                    decimal alarmTotalSecond = 0;
                    var alarmcodeList = db.Queryable<EquipmentConfiguration>().Where(it => it.EQIDId == eqguid && it.ConfigurationItem == "Equipment.AlarmCode").Select(it => it.ConfigurationValue).First();
                    String[] alarmcodes = alarmcodeList != null ? alarmcodeList.Split(',') : new String[0];


                    var sql_total = string.Format(@"WITH AlarmTimes AS (
                            SELECT 
                                EQID,
                                ALARMCODE,
		                        ALARMTEXT,
                                ALARMSET,
                                ALARMTIME,
                                ROW_NUMBER() OVER (PARTITION BY EQID, ALARMCODE ORDER BY ALARMTIME) AS rn
                            FROM EQUIPMENTALARM
                            WHERE  ALARMTIME >= to_date( '{0}', 'yyyy-MM-DD HH24:MI:SS' )
		                        AND ALARMTIME <= to_date( '{1}', 'yyyy-MM-DD HH24:MI:SS' )
		                        AND UPPER(EQID) = '{2}' 
                        ),
                        FilteredAlarm AS (SELECT 
                            AT1.EQID,
                            AT1.ALARMCODE,
		                        AT1.ALARMTEXT,
                            AT1.ALARMTIME AS START_TIME,
		                        AT2.ALARMTIME AS END_TIME,
		                        ROW_NUMBER ( ) OVER ( PARTITION BY AT1.ALARMCODE ORDER BY AT1.ALARMTIME ) AS rn 
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
                        a1.START_TIME,
                        a1.END_TIME,
                        EXTRACT(SECOND FROM (a1.END_TIME -a1.START_TIME) )AS DURATION_SECONDS,
	                        CASE
		                        WHEN
			                        TO_CHAR( a1.START_TIME, 'HH24:MI:SS' ) >= '09:00:00' THEN
				                        TRUNC( a1.START_TIME ) + 9 / 24 ELSE TRUNC( a1.START_TIME - 1 ) + 9 / 24 
				                        END AS ALARMDATE
                        FROM FilteredAlarm a1
                        LEFT JOIN FilteredAlarm a2 ON a1.ALARMCODE = a2.ALARMCODE 
                        AND a1.rn = a2.rn + 1 
                        AND a1.START_TIME <= a2.START_TIME + INTERVAL '2' MINUTE 
                        WHERE a2.START_TIME IS NULL 
                        ORDER BY START_TIME", starttime, endtime, eqid);
                    var alarmDetailTable = OracleHelper.GetDTFromCommand(sql_total);

                    var alarmTotalSecondList = alarmDetailTable.AsEnumerable().Select(it => new
                    {
                        AlarmEqp = it.Field<string>("EQID"),
                        AlarmCode = it.Field<string>("ALARMCODE"),
                        AlarmDate = it.Field<DateTime>("ALARMDATE").ToShortDateString(),
                        AlarmText = it.Field<string>("ALARMTEXT"),

                        AlarmDuration = it.Field<decimal>("DURATION_SECONDS")
                    }).ToList();

                    if (alarmcodes.Length > 0) alarmTotalSecondList = alarmTotalSecondList.Where(it => alarmcodes.Contains(it.AlarmCode)).ToList();

                    if (alarmTotalSecondList.Count != 0)
                        alarmTotalSecond = alarmTotalSecondList.GroupBy(group => group.AlarmEqp).Select(it => new { totalSeconds = it.Sum(g => (decimal)g.AlarmDuration) }).FirstOrDefault().totalSeconds;

                    //.GroupBy(group => group.AlarmEqp).Select(it => new { totalSeconds = it.Sum(g => (decimal)g.AlarmDuration) }).FirstOrDefault()?.totalSeconds:(decimal)0;

                    if (eqid.Contains("FCT"))
                    {
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
ORDER BY EQID, DATETIME", eqid, starttime.ToString("yyyy-MM-dd HH:mm:ss"), endtime.ToString("yyyy-MM-dd HH:mm:ss"));

                        var statusdata = OracleHelper.GetDTFromCommand(sql);
                        if (statusdata.Rows.Count > 1)
                        {
                            for (int k = 0; k < statusdata.Rows.Count; k++)
                            {
                                if (statusdata.Rows[0][0].ToString() == "Alarm")
                                {
                                    alarmspan += endtime - (DateTime)statusdata.Rows[0][1];

                                }
                                else
                                {
                                    if (i != 0 && statusdata.Rows[i][0].ToString() == "Alarm")
                                    {
                                        alarmspan += (DateTime)statusdata.Rows[i][0] - (DateTime)statusdata.Rows[i - 1][0];
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }
                            }
                        }
                        else if (statusdata.Rows.Count <= 1 && statusdata.Rows.Count != 0)
                        {
                            if (statusdata.Rows[0][0].ToString() == "Run")
                            {
                                alarmspan += (DateTime)statusdata.Rows[0][1] - (DateTime)statusdata.Rows[0][1];

                            }
                            else if (statusdata.Rows[0][0].ToString() == "Alarm")
                            {
                                alarmspan += endtime - (DateTime)statusdata.Rows[0][1];
                            }

                        }
                        else if (statusdata.Rows.Count == 0)
                        {
                            alarmspan = endtime - endtime;
                        }
                        alarmTotalSecond = (decimal)alarmspan.TotalSeconds;
                    }


                    var alarmrate = ((alarmTotalSecond / (decimal)(mfgtime.TotalSeconds - idleduration * 60)) * 100).ToString("0.00");
                    var alarmtime = (alarmTotalSecond / 60).ToString("0.00");

                    var sqlrate = String.Format(@"insert into EQUIPMENTPARAMSHISCAL(EQID,NAME,VALUE,UPDATETIME,REMARK) 
                                                values('{0}','{1}','{2}',to_timestamp('{3}','yyyy-mm-dd hh24:mi:ss'),'{4}')"
                                                , eqid, "alarmrate", alarmrate,
                                                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), week);
                    var sqltime = String.Format(@"insert into EQUIPMENTPARAMSHISCAL(EQID,NAME,VALUE,UPDATETIME,REMARK) 
                                                values('{0}','{1}','{2}',to_timestamp('{3}','yyyy-mm-dd hh24:mi:ss'),'{4}')"
                                                , eqid, "alarmspan", alarmtime,
                                                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), week);


                    db.Ado.ExecuteCommand(sqlrate);
                    db.Ado.ExecuteCommand(sqltime);

                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return false;
            }

        }

        public static bool CalculateAlarmRateOld(List<Equipment> eqids, DateTime starttime, DateTime endtime, string week, int idleduration)
        {
            var db = DbFactory.GetSqlSugarClient();
            string sqlrate = string.Empty;
            string sqltime = string.Empty;
            try
            {
                for (int i = 0; i < eqids.Count; i++)
                {
                    var eqid = eqids[i].EQID.ToString();
                    var eqguid = eqids[i].ID;


                    TimeSpan mfgtime = endtime - starttime;
                    var alarmspan = new TimeSpan();
                    var alarmcodeList = db.Queryable<EquipmentConfiguration>().Where(it => it.EQIDId == eqguid && it.ConfigurationItem == "Equipment.AlarmCode").Select(it => it.ConfigurationValue).First();

                    var eqpalarm = db.Queryable<EquipmentAlarm>().Where(it => it.AlarmEqp.ToUpper() == eqid
                            && it.AlarmTime > starttime && it.AlarmTime <= endtime
                            ).Select(it => new { ALARMTIME = it.AlarmTime, ALARMCODE = it.AlarmCode, ALARMTEXT = it.AlarmText, ALARMSET = it.AlarmSet }).OrderByDescending(it => it.ALARMTIME).ToList();


                    if (alarmcodeList != null)
                    {
                        String[] alarmcodes = alarmcodeList.Split(',');
                        //var eqpalarm = db.EquipmentAlarm.Where(it => it.AlarmEqp.ToUpper() == eqid && it.AlarmTime > startdate && it.AlarmTime <= enddate && alarmcodes.Contains(it.AlarmCode)).OrderByDescending(it => it.AlarmTime)
                        //    .Select(it => new { ALARMTIME = it.AlarmTime, ALARMCODE = it.AlarmCode, ALARMTEXT = it.AlarmText, ALARMSET = it.AlarmSet }).ToList();

                        if (eqid.Contains("FCT"))
                        {
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
ORDER BY EQID, DATETIME", eqid, starttime.ToString("yyyy-MM-dd HH:mm:ss"), endtime.ToString("yyyy-MM-dd HH:mm:ss"));

                            var statusdata = OracleHelper.GetDTFromCommand(sql);
                            if (statusdata.Rows.Count > 1)
                            {
                                for (int k = 0; k < statusdata.Rows.Count; k++)
                                {
                                    if (statusdata.Rows[0][0].ToString() == "Alarm")
                                    {
                                        alarmspan += endtime - (DateTime)statusdata.Rows[0][1];

                                    }
                                    else
                                    {
                                        if (i != 0 && statusdata.Rows[i][0].ToString() == "Alarm")
                                        {
                                            alarmspan += (DateTime)statusdata.Rows[i][0] - (DateTime)statusdata.Rows[i - 1][0];
                                        }
                                        else
                                        {
                                            continue;
                                        }
                                    }
                                }
                            }
                            else if (statusdata.Rows.Count <= 1 && statusdata.Rows.Count != 0)
                            {
                                if (statusdata.Rows[0][0].ToString() == "Run")
                                {
                                    alarmspan += (DateTime)statusdata.Rows[0][1] - (DateTime)statusdata.Rows[0][1];

                                }
                                else if (statusdata.Rows[0][0].ToString() == "Alarm")
                                {
                                    alarmspan += endtime - (DateTime)statusdata.Rows[0][1];
                                }

                            }
                            else if (statusdata.Rows.Count == 0)
                            {
                                alarmspan = endtime - endtime;
                            }


                        }
                        else
                        {
                            eqpalarm = eqpalarm.Where(it => alarmcodes.Contains(it.ALARMCODE)).OrderByDescending(it => it.ALARMTIME)
                            .ToList();

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

                        //sqlrate = String.Format(@"insert into EQUIPMENTPARAMSHISCAL(EQID,NAME,VALUE,UPDATETIME,REMARK) 
                        //                        values('{0}','{1}','{2}',to_timestamp('{3}','yyyy-mm-dd hh24:mi:ss'),'{4}')"
                        //                            , eqid, "alarmrate", "0.00",
                        //                            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), week);
                        //sqltime = String.Format(@"insert into EQUIPMENTPARAMSHISCAL(EQID,NAME,VALUE,UPDATETIME,REMARK) 
                        //                        values('{0}','{1}','{2}',to_timestamp('{3}','yyyy-mm-dd hh24:mi:ss'),'{4}')"
                        //                            , eqid, "alarmspan", "0.00",
                        //                            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), week);
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

                    db.Ado.ExecuteCommand(sqlrate);
                    db.Ado.ExecuteCommand(sqltime);


                    //GetHistoryAlarmRate();
                }
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }

        }

        public static void GenerateReport(string EQID, DateTime datetime, int duration, string reportType, string targetPath,int hourOffset)
        {
            try
            {
                var db = DbFactory.GetSqlSugarClient();

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                ExcelPackage ep = new ExcelPackage();
                ExcelWorkbook wb = ep.Workbook;

                switch (reportType)
                {
                    case "fail_report":
                        wb = GenerateFailReport(EQID, datetime, duration, wb);
                        break;
                    case "alarm_code_report":
                        wb = GenerateAlarmCodeReport(EQID, datetime, duration, wb);
                        break;
                    case "output_report":
                        for (int i = 0; i <= duration; i++)
                        {
                            var targetDate = datetime.AddDays(-i);
                            var startTime = targetDate.Date.AddHours(5);
                            var endTime = targetDate.Date.AddDays(1).AddHours(5);
                            var OutputList = db.Queryable<EquipmentParamsHistoryRaw>()
                            .Where(it => it.EQID == EQID && it.Name == "output" && it.UpdateTime >= startTime && it.UpdateTime <= endTime).ToList()
                            .Select(item =>
                            {

                                var values = item.Value.Split(',')
                                .Select(v => v.Split('='))
                                .ToDictionary(k => k[0], v => v[1]);

                                return new
                                {
                                    CT = values.ContainsKey("CT") ? values["CT"] : "0",
                                    PN = values.ContainsKey("PN") ? values["PN"] : "null",
                                    ReelID = values.ContainsKey("reelID") ? values["reelID"] : "null",
                                    FirstReel = values.ContainsKey("IsFirst") ? values["IsFirst"] : "N",
                                    UpdateTime = item.UpdateTime

                                };
                            });

                            ExcelWorksheet ws = wb.Worksheets.Add(targetDate.ToShortDateString());

                            ws.Cells["A1"].LoadFromDataTable(ToDataTable(OutputList), true);
                            ws.Cells["A1"].Value = "CT (sec)";
                            ws.Cells["D1"].Value = "首盘";
                            //ws.Column(2).Style.Numberformat.Format = "yyyy/m/d h:mm:ss";
                            ws.Column(5).Style.Numberformat.Format = "yyyy/m/d h:mm:ss";


                        }

                        break;
                    case "alarm_report":
                        for (int i = 0; i <= duration; i++)
                        {
                            var targetDate = datetime.AddDays(-i);
                            var startTime = targetDate.Date.AddHours(hourOffset);
                            var endTime = targetDate.Date.AddDays(1).AddHours(hourOffset);
                            var alarmList = GetAlarmDetails(startTime, endTime, EQID, hourOffset);

                            ExcelWorksheet ws = wb.Worksheets.Add(targetDate.ToShortDateString());

                            ws.Cells["A1"].LoadFromDataTable(ToDataTable(alarmList), true);
                            //ws.Column(2).Style.Numberformat.Format = "yyyy/m/d h:mm:ss";

                            ws.Column(5).Style.Numberformat.Format = "yyyy/m/d h:mm:ss";
                            ws.Column(6).Style.Numberformat.Format = "yyyy/m/d h:mm:ss";
                            ws.Cells["G1"].Value = "Duration (min)";
                        }
                        break;

                    case "status_report":
                        wb = GenerateStatusReport(EQID, datetime, duration, wb, hourOffset);
                        break;
                    case "trends_report":
                        wb = GenerateTrendsReport(EQID, datetime, duration, wb);
                        break;

                }

                var filename = $"{EQID}_{reportType}_{datetime.AddDays(-duration).ToString("yyMMdd")}~{datetime.ToString("yyMMdd")}.xlsx";
                var datastring = ep.GetAsByteArray();
                if (!Directory.Exists(targetPath))
                {
                    Directory.CreateDirectory(targetPath);
                }

                var filePath = Path.Combine(targetPath, filename);
                File.WriteAllBytes(filePath, datastring);

            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }

        }

        public static ExcelWorkbook GenerateFailReport(string EQID, DateTime datetime, int duration, ExcelWorkbook wb)
        {
            using (var db = DbFactory.GetSqlSugarClient())
            {
                var start = datetime.Date.AddHours(5);
                var sql_Count = string.Format(@"SELECT 
    TRUNC(CASE 
        WHEN TO_CHAR(UPDATETIME, 'HH24') < 5 THEN UPDATETIME - INTERVAL '1' DAY 
        ELSE  UPDATETIME
    END) + INTERVAL '5' HOUR AS period_start,
    SUM(CASE WHEN name = 'output' THEN 1 ELSE 0 END) AS success_count,
    SUM(CASE WHEN name = 'FailCount' THEN 1 ELSE 0 END) AS fail_count,
    COUNT(*) AS total_count
FROM EQUIPMENTPARAMSHISRAW
WHERE UPDATETIME >= to_date( '{0}', 'yyyy-MM-DD HH24:MI:SS' )  - INTERVAL '{2}' DAY
AND UPDATETIME < TRUNC( to_date( '{0}', 'yyyy-MM-DD HH24:MI:SS' ) +1) + 5 / 24 
AND EQID = '{1}'
GROUP BY 
    TRUNC(CASE 
        WHEN TO_CHAR(UPDATETIME, 'HH24') < 5 THEN UPDATETIME - INTERVAL '1' DAY 
        ELSE UPDATETIME 
    END) + INTERVAL '5' HOUR
ORDER BY period_start;", start, EQID, duration == 1 ? 7 : duration);
                var success_fail_result = db.SqlQueryable<SuccessFail>(sql_Count).ToList();

                var success_fail_list = success_fail_result.Select(it => new
                {

                    Date = it.period_start.ToString("yyyy/MM/dd"),
                    EQID = EQID,

                    Fail = it.fail_count,
                    Total = it.total_count,
                    SuccessRatio = (1 - it.fail_count / it.total_count) * 100

                }).ToList();

                if(success_fail_list.Count == 0)
                {
                    Log.Info($"{EQID} NO FAIL DATA.");
                    return wb;
                }
                ExcelWorksheet ws1 = wb.Worksheets.Add("拆料成功率");
                ws1.Cells["A1"].LoadFromDataTable(ToDataTable(success_fail_list), true);
                ws1.Cells["A1"].Value = "日期";
                ws1.Cells["B1"].Value = "设备编号";
                ws1.Cells["C1"].Value = "失败盘数";
                ws1.Cells["D1"].Value = "总盘数";
                ws1.Cells["E1"].Value = "拆料成功率";


                ws1.Column(1).Style.Numberformat.Format = "yyyy/m/d";
                ws1.Column(5).Style.Numberformat.Format = "0.00";

                // 生成图表1
                var rate_chart = ws1.Drawings.AddLineChart("RateChart", eLineChartType.LineMarkers);
                rate_chart.Title.Text = "拆料成功率";
                //chart.ShowDataLabelsOverMaximum = true;

                var rate_row = success_fail_list.Count() + 1;
                // 设置数据范围
                var rate_dataRange = ws1.Cells[1, 1, rate_row, 5]; // 动态计算范围
                var rate_series = rate_chart.Series.Add(rate_dataRange);

                rate_series.XSeries = ws1.Cells[2, 1, rate_row, 1].ToString(); // X轴数据
                rate_series.Series = ws1.Cells[2, 5, rate_row, 5].ToString();  // Y轴数据
                rate_series.Header = "拆料成功率";


                // 设置图表位置
                rate_chart.SetPosition(0, 0, 7, 0);
                rate_chart.SetSize(800, 400);
                rate_chart.DataLabel.ShowValue = true;
                rate_chart.DataLabel.ShowPercent = true;
                rate_chart.DataLabel.Position = eLabelPosition.Top;

                rate_chart.Legend.Add();

                List<dynamic> allData = new List<dynamic>();
                ExcelWorksheet ws2 = wb.Worksheets.Add("累计总数");

                for (int i = 0; i <= duration; i++)
                {
                    var targetDate = datetime.AddDays(-i);
                    var startTime = targetDate.Date.AddHours(5);
                    var endTime = targetDate.Date.AddDays(1).AddHours(5);
                    var FailList = db.Queryable<EquipmentParamsHistoryRaw>()
                    .Where(it => it.EQID == EQID && it.Name == "FailCount" && it.UpdateTime >= startTime && it.UpdateTime <= endTime).ToList().Select(item =>
                    {

                        var values = item.Value.Split(',')
                        .Select(v => v.Split('='))
                        .ToDictionary(k => k[0], v => v[1]);
                        return new
                        {
                            EQID = item.EQID,
                            FailCode = values.ContainsKey("FailCode") ? values["FailCode"] : "null",
                            UUT = values.ContainsKey("UUT") ? (values["UUT"] == "null" ? "" : values["UUT"]) : "null",
                            Station = values.ContainsKey("Station") ? (values["Station"] == "null" ? "" : (values["Station"] == "Station_1" ? "1号工位" : "2号工位")) : "null",
                            PN = values.ContainsKey("PN") ? values["PN"] : "",
                            ReelID = values.ContainsKey("reelID") ? (values["reelID"] == "null" ? "" : values["reelID"]) : "null",
                            MagazineCode = values.ContainsKey("MagazineCode") ? (values["MagazineCode"] == "null" ? "" : values["MagazineCode"]) : "null",
                            MagazineNumber = values.ContainsKey("MagazineNumber") ? (values["MagazineNumber"] == "null" ? "" : values["MagazineNumber"]) : "null",
                            UpdateTime = item.UpdateTime,
                        };
                    }).OrderBy(it => it.UpdateTime);

                    allData.AddRange(FailList);

                    var failtotal = FailList.GroupBy(
                       it => new
                       {
                           //Date = it.Date.Split(' ')[0],
                           it.EQID,
                           it.FailCode,
                           //AlarmText = it.AlarmText
                       }).Select(it => new
                       {
                           FailCode = it.Key.FailCode,
                           Counts = it.Count(),
                           Ratio = it.Count() / (double)FailList.Count() * 100

                       }).OrderByDescending(it => it.Counts).ToList();
                    if (FailList.Count() == 0 || failtotal.Count == 0) continue;
                    ExcelWorksheet ws = wb.Worksheets.Add(targetDate.ToShortDateString());

                    ws.Cells["A1"].LoadFromDataTable(ToDataTable(FailList), true);
                    ws.Cells["A1"].Value = "设备编号";
                    ws.Cells["B1"].Value = "异常分类";
                    ws.Cells["C1"].Value = "工位(UUT)";
                    ws.Cells["D1"].Value = "拆料工位";
                    ws.Cells["E1"].Value = "料号";
                    ws.Cells["F1"].Value = "ReelID";
                    ws.Cells["G1"].Value = "弹夹底座码";
                    ws.Cells["H1"].Value = "弹夹号";

                    ws.Column(9).Style.Numberformat.Format = "yyyy/m/d h:mm:ss";


                    ws.Cells["K1"].LoadFromDataTable(ToDataTable(failtotal), true);
                    ws.Cells["K1"].Value = "异常分类";
                    ws.Cells["L1"].Value = "数量";
                    ws.Cells["M1"].Value = "占比";
                    // 生成图表1
                    var chart = ws.Drawings.AddBarChart("DynamicChart", eBarChartType.ColumnClustered);
                    chart.Title.Text = "拆料失败异常分类";
                    ws.Column(13).Style.Numberformat.Format = "0.00";
                    //chart.ShowDataLabelsOverMaximum = true;

                    var row = failtotal.Count() + 1;
                    // 设置数据范围
                    var dataRange = ws.Cells[2, 11, row, 12]; // 动态计算范围
                    var series = chart.Series.Add(dataRange);

                    series.XSeries = ws.Cells[2, 11, row, 11].ToString(); // X轴数据
                    series.Series = ws.Cells[2, 12, row, 12].ToString();  // Y轴数据

                    // 设置图表位置
                    chart.SetPosition(0, 0, 17, 0);
                    chart.SetSize(900, 600);
                    chart.DataLabel.ShowValue = true;

                    // 生成饼图
                    var chart2 = ws.Drawings.AddPieChart("PieChart", ePieChartType.Pie);
                    chart2.Title.Text = "拆料失败异常分类";
                    //chart.ShowDataLabelsOverMaximum = true;

                    var row2 = failtotal.Count() + 1;
                    // 设置数据范围
                    var dataRange2 = ws.Cells[2, 11, row2, 13]; // 动态计算范围
                    var series2 = chart2.Series.Add(dataRange2.TakeSingleColumn(2), dataRange2.TakeSingleColumn(0));

                    // 设置图表位置
                    chart2.SetPosition(row2 + 1, 0, 10, 0);
                    chart2.SetSize(400, 400);
                    //chart2.DataLabel.ShowValue = true;
                    chart2.DataLabel.ShowCategory = true;
                    chart2.DataLabel.ShowPercent = true;
                }


                var allCode = allData.GroupBy(
                       it => new
                       {
                           //Date = it.Date.Split(' ')[0],
                           it.EQID,
                           it.FailCode,
                           //AlarmText = it.AlarmText
                       }).Select(it => new
                       {
                           FailCode = it.Key.FailCode,
                           Counts = it.Count(),
                           Ratio = it.Count() / (double)allData.Count() * 100

                       }).OrderByDescending(it => it.Counts).ToList();

                ws2.Cells["A1"].LoadFromDataTable(ToDataTable(allCode), true);
                ws2.Cells["A1"].Value = "异常分类";
                ws2.Cells["B1"].Value = "数量";
                ws2.Cells["C1"].Value = "占比";

                ws2.Column(3).Style.Numberformat.Format = "0.00";

                // 生成图表1
                var chart_total_bar = ws2.Drawings.AddBarChart("TotalBarChart", eBarChartType.ColumnClustered);
                chart_total_bar.Title.Text = "拆料失败异常分类";
                var row_total_bar = allCode.Count() + 1;
                // 设置数据范围
                var dataRange_total_bar = ws2.Cells[2, 1, row_total_bar, 3]; // 动态计算范围
                var series_total_bar = chart_total_bar.Series.Add(dataRange_total_bar);

                series_total_bar.XSeries = ws2.Cells[2, 1, row_total_bar, 1].ToString(); // X轴数据
                series_total_bar.Series = ws2.Cells[2, 2, row_total_bar, 2].ToString();  // Y轴数据

                // 设置图表位置
                chart_total_bar.SetPosition(0, 0, 8, 0);
                chart_total_bar.SetSize(900, 600);
                chart_total_bar.DataLabel.ShowValue = true;

                // 生成饼图
                var chart_total_pie = ws2.Drawings.AddPieChart("PieChart", ePieChartType.Pie);
                chart_total_pie.Title.Text = "拆料失败异常分类";
                //chart.ShowDataLabelsOverMaximum = true;

                var row_total_pie = allCode.Count() + 1;
                // 设置数据范围
                var dataRange_total_pie = ws2.Cells[2, 1, row_total_pie, 3]; // 动态计算范围
                var series_total_pie = chart_total_pie.Series.Add(dataRange_total_pie.TakeSingleColumn(2), dataRange_total_pie.TakeSingleColumn(0));

                // 设置图表位置
                chart_total_pie.SetPosition(row_total_pie + 1, 0, 0, 0);
                chart_total_pie.SetSize(400, 400);
                //chart2.DataLabel.ShowValue = true;
                chart_total_pie.DataLabel.ShowCategory = true;
                chart_total_pie.DataLabel.ShowPercent = true;
                return wb;
            }
        }

        public static ExcelWorkbook GenerateAlarmCodeReport(string EQID, DateTime datetime, int duration, ExcelWorkbook wb)
        {
            using (var db = DbFactory.GetSqlSugarClient())
            {
                ExcelWorksheet ws_total = wb.Worksheets.Add("Total");
                ExcelWorksheet ws_daily = wb.Worksheets.Add("Daily");
                var alarmTotalList = new List<dynamic>();
                var sql_old = string.Format(@"WITH AlarmTimes AS (
    SELECT 
        EQID,
        ALARMCODE,
        ALARMTEXT,
        ALARMSET,
        ALARMTIME,
        ROW_NUMBER() OVER (PARTITION BY EQID, ALARMCODE ORDER BY ALARMTIME) AS rn
    FROM EQUIPMENTALARM
    WHERE  
        ALARMTIME >=  TRUNC(TO_DATE('{0}', 'yyyy-MM-DD HH24:MI:SS')- INTERVAL '{2}' DAY)+5/24
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
a1.ALARMDATE,
--a1.ALARMCODE,
a1.ALARMTEXT,
a1.STARTTIME,
a1.ENDTIME,
a1.DURATION AS DURATION_SECONDS

FROM FinalAlarmData a1
LEFT JOIN FinalAlarmData a2 ON a1.ALARMCODE = a2.ALARMCODE
AND a1.rn = a2.rn + 1  
AND a1.STARTTIME <= a2.STARTTIME + INTERVAL '2' MINUTE 
WHERE a2.STARTTIME IS NULL
AND a1.ALARMCODE NOT IN ('900002', '900006', '900007') 
ORDER BY a1.STARTTIME
", datetime, EQID,duration);
                var sql_new = string.Format(@"SELECT 
EQID,
ALARMDATE,
--a1.ALARMCODE,
ALARMTEXT,
STARTTIME,
ENDTIME,
DURATION AS DURATION_SECONDS

FROM ALARMRESULT 
WHERE TO_DATE(ALARMDATE,'yyyy-MM-DD HH24:MI:SS') >=  TRUNC(TO_DATE('{0}', 'yyyy-MM-DD HH24:MI:SS')- INTERVAL '{2}' DAY)
AND TO_DATE(ALARMDATE,'yyyy-MM-DD HH24:MI:SS') <= TRUNC(TO_DATE('{0}', 'yyyy-MM-DD HH24:MI:SS'))
AND UPPER(EQID) = '{1}'
ORDER BY STARTTIME
", datetime.Date, EQID, duration);
                var alarmCodeDetailTable = OracleHelper.GetDTFromCommand(sql_new);

                alarmTotalList.AddRange(alarmCodeDetailTable.AsEnumerable().Select(it => new
                {
                    AlarmEqp = it.Field<string>("EQID"),
                    //AlarmCode = it.Field<string>("ALARMCODE"),
                    AlarmDate = it.Field<string>("ALARMDATE"),
                    AlarmText = it.Field<string>("ALARMTEXT"),
                    //AlarmTime = it.Field<DateTime>("START_TIME"),
                    AlarmDuration = it.Field<decimal>("DURATION_SECONDS")
                }));

                var alarmtotal = alarmCodeDetailTable.AsEnumerable().GroupBy(
                   it => new
                   {

                       //AlarmCode = it.Field<string>("ALARMCODE"),
                       AlarmDate = it.Field<string>("ALARMDATE"),//.ToShortDateString(),
                       AlarmText = it.Field<string>("ALARMTEXT"),

                   }).Select(group => new
                   {
                       //AlarmCode = group.Key.AlarmCode,
                       AlarmText = group.Key.AlarmText,
                       AlarmDate = group.Key.AlarmDate,
                       Counts = group.Count(),
                       Duration_Seconds = Math.Round(group.Sum(it => it.Field<decimal>("DURATION_SECONDS")), 2)

                   }).OrderByDescending(it => it.Counts).ToList();


                var existingDates = alarmtotal.Select(item => DateTime.Parse(item.AlarmDate)).OrderByDescending( d=> d).ToHashSet();
                foreach(var date in existingDates)
                {
                    var alarmCodeTable = ToDataTable(alarmtotal.Where(it => DateTime.Parse(it.AlarmDate) == date).Select(it => new {it.AlarmText,it.Counts,it.Duration_Seconds}));
                    var alarmCodeDetailTableDate = alarmCodeDetailTable.AsEnumerable()
                        .Where(it => it.Field<string>("ALARMDATE") == date.ToString("yyyy-MM-dd")).Select(it=> new
                        {
                           
                            AlarmText = it.Field<string>("ALARMTEXT"),
                            StartTime = it.Field<DateTime>("STARTTIME"),
                            EndTime = it.Field<DateTime>("ENDTIME"),
                            AlarmDuration = it.Field<decimal>("DURATION_SECONDS")
                        }).ToList();//ToDataTable(alarmTotalList.Where(it => it.AlarmDate == date.ToShortDateString()).Select(it => new { it.AlarmEqp,it.AlarmText, it.Counts, it.Duration_Seconds }));
                    if (alarmCodeDetailTableDate.Count() == 0 || alarmCodeTable.Rows.Count == 0) continue;
                    ExcelWorksheet ws = wb.Worksheets.Add(date.ToShortDateString());

                    ws.Cells["AD"].LoadFromDataTable(ToDataTable(alarmCodeDetailTableDate), true);
                    ws.Column(31).Style.Numberformat.Format = "yyyy/m/d h:mm:ss";
                    ws.Column(32).Style.Numberformat.Format = "yyyy/m/d h:mm:ss";
                    //ws.Column(31).Style.Numberformat.Format = "yyyy/m/d";

                    var row = alarmCodeTable.Rows.Count + 1;

                    ws.Cells["A1"].LoadFromDataTable(alarmCodeTable, true);


                    // 生成图表
                    var chart = ws.Drawings.AddBarChart("CountChart", eBarChartType.ColumnClustered);

                    chart.Title.Text = "Alarm Code Count";
                    //chart.ShowDataLabelsOverMaximum = true;



                    // 设置数据范围
                    var dataRange = ws.Cells[2, 1, row, 3]; // 动态计算范围
                    var series = chart.Series.Add(dataRange.TakeSingleColumn(1), dataRange.TakeSingleColumn(0));
                    series.Header = "Count";
                    // 设置图表位置
                    chart.SetPosition(0, 0, 5, 0);
                    chart.SetSize(1500, 500);
                    chart.DataLabel.ShowValue = true;

                    var chart2 = ws.Drawings.AddBarChart("DurationChart", eBarChartType.ColumnClustered);
                    chart2.Title.Text = "Alarm Code Duration(sec)";
                    // 设置第二个系列（按累计时间排序）
                    var series2 = chart2.Series.Add(dataRange.TakeSingleColumn(2), dataRange.TakeSingleColumn(0));  // 数据区域

                    series2.Fill.Color = System.Drawing.Color.Orange;
                    series2.Header = "Duration(sec)";  // 设置系列名称
                    chart2.SetPosition(25, 0, 5, 0);
                    chart2.SetSize(1500, 500);
                    chart2.DataLabel.ShowValue = true;
                }
                //var alarmCodeTable = ToDataTable(alarmtotal);

               
                var totalcode = alarmTotalList
                    .GroupBy(
                       it => new
                       {
                           //Date = it.AlarmDate,
                           EQID = it.AlarmEqp,
                           //AlarmCode = it.AlarmCode,
                           AlarmText = it.AlarmText
                       }).Select(it => new
                       {
                           //Date = it.Key.Date,
                           //AlarmCode = it.Key.AlarmCode,
                           AlarmText = it.Key.AlarmText,

                           Counts = it.Count(),
                           Duration_Seconds = Math.Round(it.Sum(g => (decimal)g.AlarmDuration), 2)
                       }).OrderByDescending(it => it.Counts).ToList();
                //var alarmTotalTable = ToDataTable(alarmTotalList);

                var alarmCodeTotalTable = ToDataTable(totalcode);

                ws_total.Cells["A1"].LoadFromDataTable(alarmCodeTotalTable, true);



                // 生成图表
                var chart_total = ws_total.Drawings.AddBarChart("TotalChart", eBarChartType.ColumnClustered);
                chart_total.Title.Text = "Alarm Code Count";
                //chart.ShowDataLabelsOverMaximum = true;

                var row_total = totalcode.Count() + 1;

                // 设置数据范围
                var dataRange_total = ws_total.Cells[2, 1, row_total, 3]; // 动态计算范围

                var series_total = chart_total.Series.Add(dataRange_total.TakeSingleColumn(1), dataRange_total.TakeSingleColumn(0));
                series_total.Header = "Count";


                //series_total.XSeries = ws_total.Cells[2, 2, row_total, 2].ToString(); // X轴数据
                //series_total.Series = ws_total.Cells[2, 3, row_total, 3].ToString();  // Y轴数据



                // 设置图表位置
                chart_total.SetPosition(0, 0, 5, 0);
                chart_total.SetSize(2800, 500);
                chart_total.DataLabel.ShowValue = true;


                var chart_total_duration = ws_total.Drawings.AddBarChart("TotalDurationChart", eBarChartType.ColumnClustered);
                chart_total_duration.Title.Text = "Alarm Code Duration(sec)";
                // 设置第二个系列（按累计时间排序）
                var series_total_duration = chart_total_duration.Series.Add(dataRange_total.TakeSingleColumn(2), dataRange_total.TakeSingleColumn(0));  // 数据区域

                series_total_duration.Header = "Duration(sec)";  // 设置系列名称
                series_total_duration.Fill.Color = System.Drawing.Color.Orange;
                // 设置图表位置
                chart_total_duration.SetPosition(25, 0, 5, 0);
                chart_total_duration.SetSize(2800, 500);
                chart_total_duration.DataLabel.ShowValue = true;
                chart_total_duration.DataLabel.Font.Size = 8;
                //chart_total_duration.DataLabel.Position = eLabelPosition.BestFit;




                // 按日期和 AlarmCode 汇总数据
                var result = alarmTotalList
             .GroupBy(a => new { a.AlarmText, a.AlarmDate })
             .Select(g => new
             {
                 AlarmText = g.Key.AlarmText,
                 AlarmDate = g.Key.AlarmDate,
                 Count = g.Count(),
                 TotalDuration = Math.Round(g.Sum(a => (decimal)a.AlarmDuration), 2)
             })
             .OrderBy(r => r.AlarmDate)
             .ThenBy(r => r.AlarmText)
             .ToList();

                // 获取所有不同的 AlarmCode
                var alarmTexts = alarmTotalList.Select(a => a.AlarmText).Distinct().ToList();

                // 获取所有日期
                var dates = alarmTotalList.Select(a => a.AlarmDate).Distinct().OrderBy(d => Convert.ToDateTime(d)).ToList();
                ws_daily.Cells[1, 1].Value = "AlarmText";
                int colIndex = 2;  // 从第2列开始填充

                // 填充日期列头
                foreach (var date in dates)
                {
                    ws_daily.Cells[1, colIndex++].Value = date + " - Count";
                    ws_daily.Cells[1, colIndex++].Value = date + " - Duration(sec)";
                }

                // 填充 AlarmText 行头和对应的统计数据
                int rowIndex = 2;  // 从第2行开始填充
                foreach (var alarmText in alarmTexts)
                {
                    ws_daily.Cells[rowIndex, 1].Value = alarmText;

                    colIndex = 2;  // 从第2列开始填充每个日期的数据
                    foreach (var date in dates)
                    {
                        // 获取指定日期和 AlarmText 的数据
                        var data = result
                            .FirstOrDefault(r => r.AlarmDate == date && r.AlarmText == alarmText);

                        // 填充 Count 和 Duration，如果没有数据则填充为 0
                        ws_daily.Cells[rowIndex, colIndex++].Value = data?.Count ?? 0;
                        ws_daily.Cells[rowIndex, colIndex++].Value = data?.TotalDuration ?? 0;
                    }

                    rowIndex++;
                }
                // 设置列宽自适应
                ws_daily.Cells[ws_daily.Dimension.Address].AutoFitColumns();
                // 设置第一行第一列加粗
                ws_daily.Cells[1, 1].Style.Font.Bold = true;
                for (int col = 1; col <= ws_daily.Dimension.Columns; col++)
                {
                    ws_daily.Cells[1, col].Style.Font.Bold = true;
                }


                return wb;
            }
        }
       
        public static ExcelWorkbook GenerateStatusReport(string EQID, DateTime datetime, int duration, ExcelWorkbook wb, int houroffset)
        {
            using (var db = DbFactory.GetSqlSugarClient())
            {
              
                for (int i = 0; i <= duration; i++)
                {
                    var targetDate = datetime.AddDays(-i);
                    var chartstarttime = targetDate.Date.AddHours(houroffset);
                    var endTime = targetDate.Date.AddDays(1).AddHours(houroffset);
                    var startTime = chartstarttime.AddDays(-1);

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

                    List<DetailChartDataForTrip> chardata = new List<DetailChartDataForTrip>();
                    Dictionary<string, int> statusInt = new Dictionary<string, int> { { "Run", 1 }, { "Alarm", 2 }, { "Idle", 3 }, { "Offline", 4 }, { "Down", 5 }, { "Unknow", 6 } };
                    for (int k = 0; k < statusdata.Rows.Count; k++)
                    {
                        DateTime periodStartTime = Convert.ToDateTime(statusdata.Rows[k][1].ToString());
                        DateTime periodEndTime = Convert.ToDateTime(statusdata.Rows[k][1].ToString());

                        string periodStatus = string.Empty;
                        string periodColor = string.Empty;
                        int periodStatusInt = 0;
                        TimeSpan timeSpan = new TimeSpan();


                        if (k == 0)
                        {
                            periodStartTime = Convert.ToDateTime(statusdata.Rows[k][1].ToString());
                            if (periodStartTime < chartstarttime)
                            {
                                if (statusdata.Rows.Count > (k + 1))
                                {
                                    if (Convert.ToDateTime(statusdata.Rows[k + 1][1]) > chartstarttime)
                                    {
                                        periodStartTime = chartstarttime;
                                        periodEndTime = Convert.ToDateTime(statusdata.Rows[k + 1][1]);
                                    }
                                    else
                                    {
                                        continue;
                                    }

                                }
                                else
                                {
                                    periodStartTime = chartstarttime;
                                    periodEndTime = endTime;


                                }
                            }
                            else
                            {
                                if (statusdata.Rows.Count > (k + 1))
                                {
                                    periodEndTime = Convert.ToDateTime(statusdata.Rows[k + 1][1]);
                                    periodStartTime = chartstarttime;
                                }
                                else
                                {
                                    periodStartTime = chartstarttime;
                                    periodEndTime = endTime;


                                }


                            }

                        }
                        else if (k == statusdata.Rows.Count - 1)
                        {
                            periodStartTime = Convert.ToDateTime(statusdata.Rows[k][1].ToString());
                            periodEndTime = endTime;
                            if (periodStartTime < chartstarttime) periodStartTime = chartstarttime;

                        }
                        else
                        {
                            periodStartTime = Convert.ToDateTime(statusdata.Rows[k][1].ToString());
                            if (periodStartTime < chartstarttime)
                            {
                                if (Convert.ToDateTime(statusdata.Rows[k + 1][1]) > chartstarttime)
                                {
                                    periodStartTime = chartstarttime;
                                    periodEndTime = Convert.ToDateTime(statusdata.Rows[k + 1][1]);

                                }
                                else
                                {
                                    continue;
                                }


                            }
                            else
                            {
                                periodEndTime = Convert.ToDateTime(statusdata.Rows[k + 1][1]);

                            }
                        }

                        periodStatus = statusdata.Rows[k][0].ToString();

                        timeSpan = periodEndTime - periodStartTime;

                        chardata.Add(new DetailChartDataForTrip
                        {
                            name = periodStatus,
                            start = periodStartTime.ToString("yyyy-MM-dd HH:mm:ss"),
                            end = periodEndTime.ToString("yyyy-MM-dd HH:mm:ss"),
                            duration = timeSpan.TotalMinutes.ToString("0.0"),
                        });

                    }
                    ExcelWorksheet ws = wb.Worksheets.Add(targetDate.ToShortDateString());

                    ws.Cells["A1"].LoadFromDataTable(ToDataTable(chardata), true);
                    //ws.Column(2).Style.Numberformat.Format = "yyyy/m/d h:mm:ss";
                    ws.Cells["D1"].Value = "duration (min)";


                    ws.Column(2).Style.Numberformat.Format = "yyyy/m/d h:mm:ss";
                    ws.Column(3).Style.Numberformat.Format = "yyyy/m/d h:mm:ss";
                    ws.Column(4).Style.Numberformat.Format = "0.0";


                }

                return wb;
            }
        }

        public static ExcelWorkbook GenerateTrendsReport(string EQID, DateTime datetime, int duration, ExcelWorkbook wb)
        {
            using (var db = DbFactory.GetSqlSugarClient())
            {
                var mtbadata = db.Queryable<HistoryMTBAResult>()
                    .Where(it => it.EQID == EQID && 
                    it.DataTime.Date >= datetime.AddDays(-duration) && 
                    it.DataTime.Date <= datetime).OrderBy(it => it.DataTime)
                    .Select(it => new { it.DataTime, it.MTBAValue }).ToList();

                var runratedata = db.Queryable<HistoryRunrateResult>()
                    .Where(it => it.EQID == EQID && 
                    it.DataTime.Date >= datetime.AddDays(-duration) && 
                    it.DataTime.Date <= datetime).OrderBy(it => it.DataTime)
                    .Select(it=>new {it.DataTime,it.RunrateValue}).ToList();

                ExcelWorksheet ws_runrate = wb.Worksheets.Add("Run Rate");
                ExcelWorksheet ws_mtba = wb.Worksheets.Add("MTBA");
                //ExcelWorksheet ws_downrate= wb.Worksheets.Add("Down Rate");


                ws_runrate.Cells["A1"].LoadFromDataTable(ToDataTable(runratedata), true);

                ws_runrate.Column(1).Style.Numberformat.Format = "yyyy/m/d";
                ws_runrate.Column(2).Style.Numberformat.Format = "0.00%";

                ws_mtba.Cells["A1"].LoadFromDataTable(ToDataTable(mtbadata), true);
                ws_mtba.Column(1).Style.Numberformat.Format = "yyyy/m/d";
                ws_mtba.Column(2).Style.Numberformat.Format = "0.00";


                // 生成图表1
                var rate_chart = ws_runrate.Drawings.AddLineChart("Run Rate", eLineChartType.LineMarkers);
                rate_chart.Title.Text = "Run Rate";
                //chart.ShowDataLabelsOverMaximum = true;

                // 设置X轴分类
                var xRange = ws_runrate.Cells[2, 1, 1 + runratedata.Count(), 1];

                // 添加系列 OEE
                var series1 = rate_chart.Series.Add(ws_runrate.Cells[2, 2, 1 + runratedata.Count(), 2], xRange);
                series1.Header = "Run Rate";



                // 设置图表位置
                rate_chart.SetPosition(3 + runratedata.Count(), 0, 0, 0);
                rate_chart.SetSize(1200, 400);
                series1.DataLabel.ShowValue = true;
                series1.DataLabel.ShowPercent = true;
                series1.DataLabel.Position = eLabelPosition.Top;
         
                rate_chart.Legend.Add();

                // 生成图表2
                var rate_chart_2 = ws_mtba.Drawings.AddLineChart("MTBA", eLineChartType.LineMarkers);
                rate_chart_2.Title.Text = "MTBA";
                //chart.ShowDataLabelsOverMaximum = true;

                // 设置X轴分类
                var xRange_2 = ws_mtba.Cells[2, 1, 1 + mtbadata.Count(), 1];

                // 添加系列 OEE
                var series2 = rate_chart_2.Series.Add(ws_mtba.Cells[2, 2, 1 + mtbadata.Count(), 2], xRange_2);
                series2.Header = "MTBA";



                // 设置图表位置
                rate_chart_2.SetPosition(3 + mtbadata.Count(), 0, 0, 0);
                rate_chart_2.SetSize(1200, 400);
                series2.DataLabel.ShowValue = true;
                series2.DataLabel.ShowPercent = true;
                series2.DataLabel.Position = eLabelPosition.Top;

                rate_chart_2.Legend.Add();

                return wb;
            }
        }
        private static List<WMS_REEL_INTEGRATE> ReadCsvFile(string filePath)
        {


            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,  // 表明CSV文件有标题行
                MissingFieldFound = null, // 忽略缺失字段
                Delimiter = ",",
            }))
            {
                //跳过第一行
                csv.Read();
                csv.ReadHeader();

                var records = new List<WMS_REEL_INTEGRATE>();
                while (csv.Read())
                {
                    try
                    {
                        if (csv.GetField<string>(14) == "Y")
                        {
                            var record = new WMS_REEL_INTEGRATE
                            {
                                OperateTime = Convert.ToDateTime(csv.GetField<string>(0)),
                                IntegratedType = csv.GetField<int>(3).ToString(),
                                ReelSize = csv.GetField<int>(11) / csv.GetField<int>(3),
                                WorkOrder = csv.GetField<string>(4),
                                ModelName = csv.GetField<string>(5),
                                PartNumber = csv.GetField<string>(6),
                                LotNo = csv.GetField<string>(10),
                                QTY = csv.GetField<int>(11),
                                DcNo = csv.GetField<string>(12),
                                CT = csv.GetField<double>(15),
                                //isIntegratedReel = csv.GetField<string>(14)
                            };
                            records.Add(record);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex.Message);
                        continue;
                    }
                }
                return records;
            }


        }
        public List<OEERawData> GetOEERawData(DateTime date)
        {
            try
            {
                var db = DbFactory.GetSqlSugarClient();
                //db.BeginTran();
                //从DB获取设备EQID, 设备日志存储路径、用户名、密码, targets等。
                var eqpType = db.Queryable<EquipmentType>().Where(it => it.Name == "UERE22-U-V1").Select(it => it.ID).ToList();
                var targetDictionary = db.Queryable<EquipmentTypeConfiguration>().Where(it => eqpType.Contains(it.TypeNameId)).ToList()
                    .GroupBy(t => t.ConfigurationName)
                    .ToDictionary(g => g.Key, g => g.FirstOrDefault()?.ConfigurationValue);
                var eqps = db.Queryable<Equipment>().Where(it => eqpType.Contains(it.EquipmentTypeId)).Select(it => new { it.EQID, it.ID }).ToList();


                var totalRawList = new List<WMS_REEL_INTEGRATE>();
                var exceptionDictionary = new Dictionary<string, Dictionary<string, decimal>>();

                var db_wms = DbFactory.GetWMSClient();
                var integrate_reels = db_wms.Queryable<WMS_REEL_INTEGRATE>()
                       .Where(a => a.OperateTime.Date == date.Date).ToList();
                if (integrate_reels.Count != 0)
                {
                    totalRawList.AddRange(integrate_reels);
                    foreach (var eqp in eqps)
                    {
                        if (eqp.EQID == "EQAMS00005") continue;
                        var exception = CalculateExceptionTime(eqp.EQID, date, date.AddDays(1));
                        exceptionDictionary.Add(eqp.EQID, exception);
                    }

                }
                else
                {

                    foreach (var eqp in eqps)
                    {
                        if (eqp.EQID == "EQAMS00005") continue;
                        var EQIDid = eqp.ID;
                        var configDictionary = db.Queryable<EquipmentConfiguration>().Where(it => it.EQIDId == EQIDid && it.ConfigurationItem.StartsWith("IP.")).ToList()
                            .GroupBy(t => t.ConfigurationName)
                            .ToDictionary(g => g.Key, g => g.FirstOrDefault()?.ConfigurationValue);
                        //Task.Run(() =>
                        //{
                        var exception = CalculateExceptionTime(eqp.EQID, date, date.AddDays(1));
                        exceptionDictionary.Add(eqp.EQID, exception);


                        //db.Insertable(exception).ExecuteCommand();
                        //});

                        var ipStr = configDictionary["IP.Address"];
                        var accountStr = configDictionary["IP.Account"].ToString();
                        var pswStr = configDictionary["IP.Password"].ToString();

                        if (ipStr == null || accountStr == null || pswStr == null) continue;
                        var url = $"\\\\{ipStr}\\d$";//Path.Combine(ipStr.ConfigurationValue.ToString(),"d$");//$"\\\\192.168.53.174\\e$";
                                                     //var url = $"\\\\{ipStr.ConfigurationValue}\\d$";//Path.Combine(ipStr.ConfigurationValue.ToString(),"d$");//$"\\\\192.168.53.174\\e$";
                                                     //if (eqp.EQID == "EQAMS00005") url = $"\\\\{ipStr}\\d";
                                                     //url = $"\\\\127.0.0.1\\d$\\test\\合料output\\{eqp.EQID}";

                        if (!Directory.Exists(url))
                        {
                            NetworkCredential networkCredential = new NetworkCredential(accountStr, pswStr);
                            try
                            {
                                var connection = new Utils.NetworkConnection(url, networkCredential);
                            }
                            catch (Exception ex)
                            {
                                Log.Error(ex.ToString());
                                ConnectLan(url, accountStr, pswStr);
                            }

                            if (!Directory.Exists(url))
                            {
                                Log.Error($"{eqp.EQID}-{url}:路径不存在！");
                                continue;
                            }
                            //continue;
                        }
                        var paths = new List<string>()
                    {
                        url+"\\Report\\AlloyPlate\\Labe"
                    };
                        var files = new List<string>();
                        foreach (var path in paths)
                        {
                            DirectoryInfo directory = new DirectoryInfo(path);


                            var datePreFix = Convert.ToDateTime(date).ToString("yyyyMMdd");
                            // 获取文件夹中所有以指定前缀开头的文件
                            var isExist = Directory.GetFiles(path, $"{datePreFix}*").FirstOrDefault();
                            if (isExist == null)
                            {
                                Log.Error($"以 {Path.Combine(path, datePreFix)} 开头的文件不存在！");
                                continue;
                            }

                            //files.Add(isExist);
                            var result = ReadCsvFile(isExist);
                            DateTime lastOperateTime = date; // 初始时间，假设第一条记录前的时间是零点
                                                             //decimal lastReelSize = 0;
                            foreach (var record in result.OrderBy(r => r.OperateTime))
                            {
                                TimeSpan workTime = record.OperateTime - lastOperateTime;
                                //record.OperateTime - Convert.ToDateTime(record.OperateTime).Date
                                // 给每条记录加上计算出的工作时间
                                record.WorkTime = (record.CT / 60 > workTime.TotalMinutes) ? (record.CT / 60) : workTime.TotalMinutes; // 按分钟计时
                                record.ReelSize = record.ReelSize / 1000;
                                record.EQID = eqp.EQID;
                                record.DateTime = date.ToString("MM/dd");

                                // 更新lastOperateTime为当前记录的OperateTime
                                lastOperateTime = record.OperateTime;

                            }
                            totalRawList.AddRange(result);
                        }




                    }
                }


                var arrangedList = totalRawList
                   .GroupBy(w => new { w.EQID, w.DateTime, w.IntegratedType, w.ReelSize })
                   .Where(group =>
                       // 检查是否存在目标配置，如果不存在则排除该记录
                       targetDictionary.ContainsKey($"CT_{group.Key.ReelSize}_{group.Key.IntegratedType}") &&
                       targetDictionary.ContainsKey($"UPH_{group.Key.ReelSize}_{group.Key.IntegratedType}") &&
                       targetDictionary.ContainsKey("OEETarget"))
                   .Select(group => new OEERawData
                   {
                       EQID = group.Key.EQID,
                       IntegratedType = group.Key.IntegratedType,
                       ReelSize = group.Key.ReelSize,
                       DateTime = group.Key.DateTime,

                       // 盘数
                       Output = group.Count(),

                       // 计算 WorkTime
                       WorkTime = Math.Round(group.Sum(it => (decimal)it.WorkTime) / 60, 4),

                       // 获取 CT Target，如果没有，则排除该记录
                       CT = Convert.ToDecimal(targetDictionary[$"CT_{group.Key.ReelSize}_{group.Key.IntegratedType}"]),

                       // 获取 UPH Target
                       UPH = Convert.ToDecimal(targetDictionary[$"UPH_{group.Key.ReelSize}_{group.Key.IntegratedType}"]),

                       // 获取 OEE Target
                       OEETarget = Convert.ToDecimal(targetDictionary["OEETarget"]),

                       //OEE = group.Count() / Math.Round(Convert.ToDecimal(targetDictionary[$"UPH_{group.Key.ReelSize}_{group.Key.IntegratedType}"]) * Math.Round(group.Sum(it => (decimal)it.WorkTime) / 60, 2), 2),
                       // 计算 Output Target
                       OutputTarget = Math.Round(Convert.ToDecimal(targetDictionary[$"UPH_{group.Key.ReelSize}_{group.Key.IntegratedType}"]) * Math.Round(group.Sum(it => (decimal)it.WorkTime) / 60, 2)),

                       //DownTime
                       DownTime = exceptionDictionary[group.Key.EQID]["DownTime"],

                       //IdleTime
                       IdleTime = exceptionDictionary[group.Key.EQID]["IdleTime"],

                       //FAITime
                       FAITime = exceptionDictionary[group.Key.EQID]["FAITime"]
                   })
                   .OrderBy(group => group.DateTime)
                   .ToList();
                //db.CommitTran();
                return arrangedList;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return new List<OEERawData>();
                //db.RollbackTran();
            }
        }
        private static Dictionary<string, decimal> CalculateExceptionTime(string EQID, DateTime starttime, DateTime endtime)
        {
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
EQID, starttime.ToString("yyyy-MM-dd HH:mm:ss"), endtime.ToString("yyyy-MM-dd HH:mm:ss"));
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


            var FAICodes = new List<string>()
            {
                "DC_5255","LotNo_5254","MPN_5253","QTY_5251"
            };
            var alarmData = DashBoardService.GetAlarmDetails(starttime, endtime, EQID, 0);

            var FAIData = alarmData.Where(it => FAICodes.Contains(it.AlarmCode)).ToList();
            var downData = alarmData.Except(FAIData).ToList();
            // 合并同一天的报警时间段
            TimeSpan totalDownDuration = DashBoardService.MergeAndCalculateDuration(downData);
            TimeSpan totalFAIDuration = DashBoardService.MergeAndCalculateDuration(FAIData);
            var dict = new Dictionary<string, decimal>()
            {
                { "DownTime",(decimal)totalDownDuration.TotalMinutes },
                 { "FAITime",(decimal)totalFAIDuration.TotalMinutes },
                { "IdleTime",(decimal)totalIdleTime.TotalMinutes}
            };
            return dict;
        }
        public void UpdateReport(ExcelPackage package, List<OEERawData> newData, DateTime datetime)
        {
            try
            {

                var summarySheet = package.Workbook.Worksheets[0];
                // 获取最后一列（避免空列的影响）
                int lastColumn = GetLastColumnWithData(summarySheet);
                // 添加新数据到新的列
                int columnOffset = lastColumn + 1; // 下一列的索引

                summarySheet.Cells[1, columnOffset].Value = datetime.ToString("MM/dd");
                var cellrange = summarySheet.Cells[1, 1, 4, columnOffset];
                cellrange.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                cellrange.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                cellrange.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                cellrange.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;


                summarySheet.Cells[2, 2].Value = 0.8797;
                summarySheet.Cells[2, 3].FormulaR1C1 = "(1-RC[-1])*0.05+RC[-1]";

                summarySheet.Cells[3, 2].Value = 0.5784;
                summarySheet.Cells[3, 3].FormulaR1C1 = "(1-RC[-1])*0.05+RC[-1]";

                summarySheet.Cells[2, 4].FormulaR1C1 = $"AVERAGE(RC[3]:RC[{columnOffset - 4}])";
                summarySheet.Cells[2, 5].FormulaR1C1 = $"AVERAGE(RC[2]:RC[{columnOffset - 5}])";
                summarySheet.Cells[2, 6].FormulaR1C1 = $"AVERAGE(RC[1]:RC[{columnOffset - 6}])";

                summarySheet.Cells[3, 4].FormulaR1C1 = $"AVERAGE(RC[3]:RC[{columnOffset - 4}])";
                summarySheet.Cells[3, 5].FormulaR1C1 = $"AVERAGE(RC[2]:RC[{columnOffset - 5}])";
                summarySheet.Cells[3, 6].FormulaR1C1 = $"AVERAGE(RC[1]:RC[{columnOffset - 6}])";
                summarySheet.Cells[4, 6].FormulaR1C1 = $"SUM(RC[1]:RC[{columnOffset - 6}])";

                summarySheet.Cells[2, 2, 3, columnOffset].Style.Numberformat.Format = "0.00%";


                summarySheet.Drawings.Clear();
                // 生成图表1
                var rate_chart = summarySheet.Drawings.AddLineChart("OEEChart", eLineChartType.LineMarkers);
                rate_chart.Title.Text = "合料机OEE&LOADING RATE";
                //chart.ShowDataLabelsOverMaximum = true;

                // 设置X轴分类
                var xRange = summarySheet.Cells[1, 7, 1, columnOffset];

                // 添加系列 OEE
                var series1 = rate_chart.Series.Add(summarySheet.Cells[2, 7, 2, columnOffset], xRange);
                series1.Header = "OEE";

                // 添加系列 Loading Rate
                var series2 = rate_chart.Series.Add(summarySheet.Cells[3, 7, 3, columnOffset], xRange);
                series2.Header = "Loading Rate";


                // 设置图表位置
                rate_chart.SetPosition(6, 0, 0, 0);
                rate_chart.SetSize(1200, 400);
                series1.DataLabel.ShowValue = true;
                series1.DataLabel.ShowPercent = true;
                series1.DataLabel.Position = eLabelPosition.Top;
                series2.DataLabel.ShowValue = true;
                series2.DataLabel.ShowPercent = true;
                series2.DataLabel.Position = eLabelPosition.Bottom;
                rate_chart.Legend.Add();

                var worksheet = package.Workbook.Worksheets[1];
                HandleOeeSheet(summarySheet, columnOffset, worksheet, newData, datetime);

                var loadingSheet = package.Workbook.Worksheets[2];
                HandleLoadingRateSheet(summarySheet, columnOffset, loadingSheet, newData, datetime);
                // 保存合并后的文件


                package.Save();
                Log.Info("合料机OEE数据计算完毕!");



            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

        }

        private static void HandleOeeSheet(ExcelWorksheet summaryWs, int summaryCol, ExcelWorksheet worksheet, List<OEERawData> newData, DateTime datetime)
        {
            // 获取最后一列（避免空列的影响）
            int lastColumn = GetLastColumnWithData(worksheet);
            // 添加新数据到新的列
            int columnOffset = lastColumn; // 下一列的索引
            int row = 2; // 假设第一行是标题，数据从第二行开始
            worksheet.Cells[row + 1, columnOffset, 31, columnOffset].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            worksheet.Cells[row + 1, columnOffset, 31, columnOffset].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);
            worksheet.Cells[row, columnOffset].Value = datetime.ToString("MM/dd");

            var datetimeColLen = columnOffset + newData.Count;// col + totalOutputByReelSize.Count
            worksheet.Cells[2, columnOffset, 2, datetimeColLen].Merge = true;
            worksheet.Cells[2, columnOffset, 2, datetimeColLen].Style.Font.Bold = true;
            worksheet.Cells[2, columnOffset, 2, datetimeColLen].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

            worksheet.Cells[row + 1, datetimeColLen].Value = "Overall";
            worksheet.Cells[row + 1, datetimeColLen, 4, datetimeColLen].Merge = true;
            worksheet.Cells[row + 1, datetimeColLen, 4, datetimeColLen].Style.Font.Bold = true;
            worksheet.Cells[row + 1, datetimeColLen, 4, datetimeColLen].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            worksheet.Cells[row + 1, datetimeColLen, 4, datetimeColLen].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            worksheet.Cells[row + 1, datetimeColLen, 31, datetimeColLen].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            worksheet.Cells[row + 1, datetimeColLen, 31, datetimeColLen].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Orange);

            worksheet.Cells[2, datetimeColLen + 1].Value = $"{datetime.Month}月";

            worksheet.Cells[8, datetimeColLen].FormulaR1C1 = $"AVERAGE(RC[{-newData.Count}]:RC[-1])";
            worksheet.Cells[9, datetimeColLen].FormulaR1C1 = $"AVERAGE(RC[{-newData.Count}]:RC[-1])";
            worksheet.Cells[10, datetimeColLen].FormulaR1C1 = $"AVERAGE(RC[{-newData.Count}]:RC[-1])";
            //worksheet.Cells[9, datetimeColLen+1].FormulaR1C1 = $"AVERAGE(RC[{-newData.Count}]:RC[-1])";
            worksheet.Cells[row + 1, datetimeColLen + 1, 31, datetimeColLen + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            worksheet.Cells[row + 1, datetimeColLen + 1, 31, datetimeColLen + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);

            SetFormulaByColor(worksheet, 8, datetimeColLen, "AVERAGE", "0.00%");
            SetFormulaByColor(worksheet, 9, datetimeColLen, "AVERAGE", "0.00%");
            SetFormulaByColor(worksheet, 10, datetimeColLen, "AVERAGE", "0.00%");
            SetFormulaByColor(worksheet, 12, datetimeColLen, "SUM", string.Empty);
            SetFormulaByColor(worksheet, 13, datetimeColLen, "SUM", string.Empty);



            //worksheet.Cells[9, datetimeColLen].FormulaR1C1 = $"SUM(R[1]C[{-dailyOutputList.Count}]:R[1]C[-1])/R[4]C";

            worksheet.Cells[8, datetimeColLen, 10, datetimeColLen].Style.Numberformat.Format = "0.00%";


            worksheet.Cells[12, datetimeColLen].FormulaR1C1 = $"SUM(RC[{-newData.Count}]:RC[-1])";
            worksheet.Cells[13, datetimeColLen].FormulaR1C1 = $"SUM(RC[{-newData.Count}]:RC[-1])";

            worksheet.Cells[15, datetimeColLen].FormulaR1C1 = $"SUM(RC[{-newData.Count}]:RC[-1])";
            SetFormulaByColor(worksheet, 15, datetimeColLen, "SUM", string.Empty);

            worksheet.Cells[17, datetimeColLen].FormulaR1C1 = $"AVERAGE(RC[{-newData.Count}]:RC[-1])";
            worksheet.Cells[17, datetimeColLen].Style.Numberformat.Format = "0.00%";
            SetFormulaByColor(worksheet, 17, datetimeColLen, "AVERAGE", "0.00%");

            worksheet.Cells[18, datetimeColLen].FormulaR1C1 = $"SUM(RC[{-newData.Count}]:RC[-1])";
            worksheet.Cells[20, datetimeColLen].FormulaR1C1 = $"SUM(RC[{-newData.Count}]:RC[-1])";
            worksheet.Cells[22, datetimeColLen].FormulaR1C1 = $"SUM(RC[{-newData.Count}]:RC[-1])";
            worksheet.Cells[23, datetimeColLen].FormulaR1C1 = $"SUM(RC[{-newData.Count}]:RC[-1])";
            worksheet.Cells[24, datetimeColLen].FormulaR1C1 = $"SUM(RC[{-newData.Count}]:RC[-1])";
            SetFormulaByColor(worksheet, 18, datetimeColLen, "SUM", string.Empty);
            SetFormulaByColor(worksheet, 20, datetimeColLen, "SUM", string.Empty);
            SetFormulaByColor(worksheet, 22, datetimeColLen, "SUM", string.Empty);
            SetFormulaByColor(worksheet, 23, datetimeColLen, "SUM", string.Empty);
            SetFormulaByColor(worksheet, 24, datetimeColLen, "SUM", string.Empty);

            worksheet.Cells[25, datetimeColLen].FormulaR1C1 = $"AVERAGE(RC[{-newData.Count}]:RC[-1])";
            worksheet.Cells[25, datetimeColLen].Style.Numberformat.Format = "0.00%";
            SetFormulaByColor(worksheet, 25, datetimeColLen, "AVERAGE", "0.00%");

            worksheet.Cells[26, datetimeColLen].FormulaR1C1 = $"SUM(RC[{-newData.Count}]:RC[-1])";
            worksheet.Cells[28, datetimeColLen].FormulaR1C1 = $"SUM(RC[{-newData.Count}]:RC[-1])";
            worksheet.Cells[29, datetimeColLen].FormulaR1C1 = $"AVERAGE(RC[{-newData.Count}]:RC[-1])";
            worksheet.Cells[29, datetimeColLen, 31, datetimeColLen].Style.Numberformat.Format = "0.00%";
            SetFormulaByColor(worksheet, 26, datetimeColLen, "SUM", string.Empty);
            SetFormulaByColor(worksheet, 28, datetimeColLen, "SUM", string.Empty);
            SetFormulaByColor(worksheet, 29, datetimeColLen, "AVERAGE", "0.00%");
            SetFormulaByColor(worksheet, 30, datetimeColLen, "AVERAGE", "0.00%");
            SetFormulaByColor(worksheet, 31, datetimeColLen, "AVERAGE", "0.00%");

            worksheet.Cells[30, datetimeColLen].FormulaR1C1 = $"AVERAGE(RC[{-newData.Count}]:RC[-1])";
            worksheet.Cells[31, datetimeColLen].FormulaR1C1 = $"AVERAGE(RC[{-newData.Count}]:RC[-1])";

            //by eqp 生成详细的数据
            var eqps = newData.OrderBy(it => it.EQID).Select(it => it.EQID).Distinct().ToList();
            var subcol = columnOffset;
            foreach (var eqp in eqps)
            {
                worksheet.Cells[3, subcol].Value = eqp.Substring(9) + "#";
                var reelSizeCnt = newData.Where(it => it.EQID == eqp).Select(it => it.ReelSize).Distinct().ToList();

                var totalWorkTimeByEQID = newData.Where(it => it.EQID == eqp).GroupBy(g => g.EQID).Select(it => it.Sum(s => s.WorkTime)).FirstOrDefault();
                var totalOutputByEQID = newData.Where(it => it.EQID == eqp).GroupBy(g => g.EQID).Select(it => it.Sum(s => s.Output)).FirstOrDefault();

                var outputByReelSize = newData.Where(it => it.EQID == eqp).GroupBy(g => new { g.EQID, g.ReelSize }).Select(it => it.Sum(s => s.Output)).FirstOrDefault();
                var maxOutput = newData.Where(it => it.EQID == eqp).GroupBy(g => g.EQID).Select(it => it.Max(s => s.Output)).FirstOrDefault();
                worksheet.Cells[3, subcol, 3, subcol + newData.Where(it => it.EQID == eqp).Count() - 1].Merge = true;
                worksheet.Cells[3, subcol, 3, subcol + newData.Where(it => it.EQID == eqp).Count() - 1].Style.Font.Bold = true;
                worksheet.Cells[3, subcol, 3, subcol + newData.Where(it => it.EQID == eqp).Count() - 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                var integratedTypeCol = subcol;

                for (int k = 0; k < reelSizeCnt.Count(); k++)
                {

                    var targetType = reelSizeCnt[k];
                    var IntegratedTypes = newData.Where(it => it.EQID == eqp && it.ReelSize == targetType).OrderBy(it => it.IntegratedType).Select(it => it.IntegratedType).Distinct().ToList();//.FirstOrDefault();
                    worksheet.Cells[4, integratedTypeCol].Value = targetType + "K/卷";
                    worksheet.Cells[4, integratedTypeCol, 4, integratedTypeCol + IntegratedTypes.Count() - 1].Merge = true;
                    worksheet.Cells[4, integratedTypeCol, 4, integratedTypeCol + IntegratedTypes.Count() - 1].Style.Font.Bold = true;
                    worksheet.Cells[4, integratedTypeCol, 4, integratedTypeCol + IntegratedTypes.Count() - 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    foreach (var integratedType in IntegratedTypes)
                    {
                        //类型

                        var tgtRcd = newData.Where(it => it.EQID == eqp && it.ReelSize == targetType && it.IntegratedType == integratedType).FirstOrDefault();
                        var targetWorkTime = tgtRcd == null ? 0 : tgtRcd.WorkTime;
                        var targetCount = tgtRcd == null ? 0 : tgtRcd.Output;
                        var targetCT = tgtRcd == null ? 0 : tgtRcd.CT;
                        var targetUPH = tgtRcd == null ? 0 : tgtRcd.UPH;
                        var targetOEE = tgtRcd == null ? 0 : tgtRcd.OEETarget;

                        var DownTime = Math.Round(tgtRcd.DownTime * (targetCount / (decimal)totalOutputByEQID), 2);
                        var IdleTime = Math.Round(tgtRcd.IdleTime * (targetCount / (decimal)totalOutputByEQID), 2);
                        var FAITime = Math.Round(tgtRcd.FAITime * (targetCount / (decimal)totalOutputByEQID), 2);
                        //var FAITime = targetCount == (decimal)maxOutput ? 40 : 0;// Math.Round(_faitime * (targetCount / (decimal)totalOutputByEQID), 2);


                        if (Math.Round(targetWorkTime - IdleTime / 60, 2) >= 0)
                        {
                            targetWorkTime = Math.Round(Math.Round(targetWorkTime - IdleTime / 60, 2) * targetUPH) < targetCount ? Math.Round(targetCount / targetUPH, 2) : Math.Round(targetWorkTime - IdleTime / 60, 2);
                        }
                        else
                        {
                            targetWorkTime = Math.Round(targetCount / targetUPH, 2);
                        }
                        //Math.Round(targetWorkTime - IdleTime / 60, 2) <0?targetWorkTime: Math.Round(targetWorkTime - IdleTime / 60, 2);
                        var outputTarget = Math.Round(targetUPH * targetWorkTime);
                        var outputGap = outputTarget < targetCount ? 0 : outputTarget - targetCount;

                        var totalImpactOutput = Math.Floor((DownTime + IdleTime + FAITime) * 60 / targetCT);
                        var ImpactOutputGap = outputGap - totalImpactOutput;//outputGap < totalImpactOutput? 0:
                        var downTimeGap = Math.Round(ImpactOutputGap * targetCT / 60, 2);
                        var amendDownTime = (DownTime + downTimeGap) < 0 ? DownTime : Math.Round(DownTime + downTimeGap, 2);


                        //decimal OEEActual = (targetCount / (targetUPH * targetWorkTime));
                        //decimal exceptOEE = targetCount / ((targetWorkTime* 60 - IdleTime) / 60 * targetUPH);


                        worksheet.Cells[4, integratedTypeCol].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        //工时（S/卷)
                        worksheet.Cells[5, integratedTypeCol].Value = $"{integratedType}合1";
                        //工时（S/卷)
                        worksheet.Cells[6, integratedTypeCol].Value = targetCT;
                        //UPH
                        worksheet.Cells[7, integratedTypeCol].Value = targetUPH;
                        //OEE Target
                        worksheet.Cells[8, integratedTypeCol].Value = targetOEE / 100;
                        // 设置单元格格式为百分数
                        worksheet.Cells[8, integratedTypeCol].Style.Numberformat.Format = "0.00%";


                        //OEE Actual
                        //worksheet.Cells[8, k + col].Value = OEEActual;
                        worksheet.Cells[9, integratedTypeCol].FormulaR1C1 = "IF((R[4]C/R[3]C)>1,1,R[4]C/R[3]C)";
                        // 设置单元格格式为百分数
                        worksheet.Cells[9, integratedTypeCol].Style.Numberformat.Format = "0.00%";

                        //Except Schedule OEE Actual
                        //worksheet.Cells[9, k + col].Value = exceptOEE;
                        worksheet.Cells[10, integratedTypeCol].FormulaR1C1 = "IF((R[3]C/((R[1]C*60-R[13]C)/60*R[-3]C))>1,1,R[3]C/((R[1]C*60-R[13]C)/60*R[-3]C))";
                        worksheet.Cells[10, integratedTypeCol].Style.Numberformat.Format = "0.00%";
                        //worksheet.Cells[10, integratedTypeCol].FormulaR1C1 = $"R[-1]C*R[3]C";
                        //worksheet.Cells[10, integratedTypeCol].Style.Numberformat.Format = "0.00";

                        //WorkTime
                        worksheet.Cells[11, integratedTypeCol].Value = targetWorkTime; //(targetWorkTime - Math.Round(DownTime / 60, 2)) < 0 ? targetWorkTime : (targetWorkTime - Math.Round(DownTime / 60, 2));

                        //Output Target
                        worksheet.Cells[12, integratedTypeCol].FormulaR1C1 = "ROUND(R[-5]C*R[-1]C,0)";//Math.Round(targetUPH * targetWorkTime);
                        worksheet.Cells[12, integratedTypeCol].Style.Numberformat.Format = "0";

                        //Output Actual
                        worksheet.Cells[13, integratedTypeCol].Value = targetCount;

                        //Machine Down Time(Mins)
                        worksheet.Cells[15, integratedTypeCol].Value = amendDownTime;
                        //worksheet.Cells[14, k + col].FormulaR1C1 = $"{_downtime}*(R[-3]C/(R[-3]C+R[-3]C[-1]))";
                        worksheet.Cells[18, integratedTypeCol].Style.Numberformat.Format = "0";

                        //Others(Mins)
                        worksheet.Cells[16, integratedTypeCol].Value = 0;
                        //Total Rate
                        //worksheet.Cells[16, k + col].Value = DownTime / (targetWorkTime * 60);
                        worksheet.Cells[17, integratedTypeCol].FormulaR1C1 = "(R[-2]C+R[-1]C)/(R[-6]C*60+R[-2]C+R[-1]C)";
                        worksheet.Cells[17, integratedTypeCol].Style.Numberformat.Format = "0.00%";

                        //Impact Output
                        //worksheet.Cells[17, k + col].Value = Math.Round(DownTime * 60 / targetCT);
                        worksheet.Cells[18, integratedTypeCol].FormulaR1C1 = "ROUND((R[-3]C+R[-2]C)*60/R[-12]C,0)";
                        worksheet.Cells[18, integratedTypeCol].Style.Numberformat.Format = "0";
                        //首件/校验机台（Mins）①
                        worksheet.Cells[20, integratedTypeCol].Value = FAITime;

                        // "Total(Mins)"
                        worksheet.Cells[22, integratedTypeCol].FormulaR1C1 = $"SUM(R[-2]C:R[-1]C)";
                        //Impact Output
                        worksheet.Cells[23, integratedTypeCol].FormulaR1C1 = "ROUND(R[-1]C*60/R[-17]C,0)";
                        worksheet.Cells[23, integratedTypeCol].Style.Numberformat.Format = "0";
                        //排停(Mins)(idle)
                        worksheet.Cells[24, integratedTypeCol].Value = IdleTime;

                        //排停百分比
                        worksheet.Cells[25, integratedTypeCol].FormulaR1C1 = "R[-1]C/(R[-14]C*60+R[-1]C)";
                        worksheet.Cells[25, integratedTypeCol].Style.Numberformat.Format = "0.00%";

                        //排停Impact Output
                        worksheet.Cells[26, integratedTypeCol].FormulaR1C1 = "ROUND(R[-2]C*60/R[-20]C,0)";
                        worksheet.Cells[26, integratedTypeCol].Style.Numberformat.Format = "0";

                        //Total(Mins)
                        worksheet.Cells[28, integratedTypeCol].FormulaR1C1 = "R[-1]C+R[-4]C+R[-6]C";//1,4,6

                        //百分比
                        worksheet.Cells[29, integratedTypeCol].FormulaR1C1 = "R[-1]C/(R[-18]C*60+R[-1]C)";
                        worksheet.Cells[29, integratedTypeCol].Style.Numberformat.Format = "0.00%";

                        //Total OEE
                        worksheet.Cells[30, integratedTypeCol].FormulaR1C1 = "IF((R[-4]C+R[-7]C+R[-17]C+R[-12]C)/R[-18]C>1,1,(R[-4]C+R[-7]C+R[-17]C+R[-12]C)/R[-18]C)";//"IF((R[-1]C+R[-13]C+R[-21]C)>1,1,R[-1]C+R[-13]C+R[-21]C)";
                        worksheet.Cells[30, integratedTypeCol].Style.Numberformat.Format = "0.00%";

                        worksheet.Cells[31, integratedTypeCol].FormulaR1C1 = "1-R[-1]C";
                        worksheet.Cells[31, integratedTypeCol].Style.Numberformat.Format = "0.00%";

                        integratedTypeCol++;
                    }
                    //integratedTypeCol += IntegratedTypes.Count();
                    //targetOutputByReelSize


                }
                subcol += newData.Where(it => it.EQID == eqp).Count();
            }

            var cellrange = worksheet.Cells[2, 1, 31, columnOffset + newData.Count + 1];
            cellrange.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            cellrange.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            cellrange.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            cellrange.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;


            var hightlightRange = worksheet.Cells[8, 1, 9, columnOffset + newData.Count + 1];
            hightlightRange.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thick, System.Drawing.Color.Blue);

            // 强制计算公式
            worksheet.Calculate();
            var subLastCol = 1;
            for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
            {

                if (worksheet.Cells[33, col].Value != null)
                {
                    subLastCol = Math.Max(subLastCol, col); // 获取最大列
                }

            }
            subLastCol++;
            worksheet.Cells[33, subLastCol].Value = datetime.ToString("MM/dd");
            worksheet.Cells[34, subLastCol].Value = worksheet.Cells[8, datetimeColLen].Value;//OEE Target
            worksheet.Cells[35, subLastCol].Value = worksheet.Cells[9, datetimeColLen].Value;//OEE Act.
            worksheet.Cells[36, subLastCol].Value = worksheet.Cells[10, datetimeColLen].Value; //Except.
            worksheet.Cells[37, subLastCol].Value = worksheet.Cells[17, datetimeColLen].Value;//Down
            worksheet.Cells[38, subLastCol].Value = worksheet.Cells[25, datetimeColLen].Value;//排停
                                                                                              //List<object> OEESummaryDateList = SetOEESummaryTable(worksheet, 2, "");
                                                                                              //List<object> OEESummaryOEETargetList = SetOEESummaryTable(worksheet, 8, "FFFFA500");
                                                                                              //List<object> OEESummaryOEEActualList = SetOEESummaryTable(worksheet, 9, "FFFFA500");
                                                                                              //List<object> OEESummaryExceptOEEList = SetOEESummaryTable(worksheet, 10, "FFFFA500");
                                                                                              //List<object> OEESummaryDownList = SetOEESummaryTable(worksheet, 17, "FFFFA500");
                                                                                              //List<object> OEESummaryIdleList = SetOEESummaryTable(worksheet, 25, "FFFFA500");
                                                                                              //worksheet.Cells[33, 2].FormulaR1C1 = $"AVERAGE(RC[{subLastCol - 2}]:RC[1])";
            worksheet.Cells[34, 2].FormulaR1C1 = $"AVERAGE(RC[{subLastCol - 2}]:RC[1])";
            worksheet.Cells[35, 2].FormulaR1C1 = $"AVERAGE(RC[{subLastCol - 2}]:RC[1])";
            worksheet.Cells[36, 2].FormulaR1C1 = $"AVERAGE(RC[{subLastCol - 2}]:RC[1])";
            worksheet.Cells[37, 2].FormulaR1C1 = $"AVERAGE(RC[{subLastCol - 2}]:RC[1])";
            worksheet.Cells[38, 2].FormulaR1C1 = $"AVERAGE(RC[{subLastCol - 2}]:RC[1])";

            worksheet.Cells[34, 2, 38, subLastCol].Style.Numberformat.Format = "0.00%";

            var summaryrange = worksheet.Cells[33, 1, 38, subLastCol];
            summaryrange.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thick;
            summaryrange.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thick;
            summaryrange.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thick;
            summaryrange.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thick;

            worksheet.Drawings.Clear();
            // 生成图表1
            var rate_chart = worksheet.Drawings.AddLineChart("OEEChart", eLineChartType.LineMarkers);
            //rate_chart.Title.Text = "拆料成功率";
            //chart.ShowDataLabelsOverMaximum = true;

            // 设置X轴分类
            var xRange = worksheet.Cells[33, 3, 33, subLastCol];

            // 添加系列 OEE Target
            var series1 = rate_chart.Series.Add(worksheet.Cells[34, 3, 34, subLastCol], xRange);
            series1.Header = "OEE Target";

            // 添加系列2
            var series2 = rate_chart.Series.Add(worksheet.Cells[35, 3, 35, subLastCol], xRange);
            series2.Header = "OEE Actual";

            // 添加系列3
            var series3 = rate_chart.Series.Add(worksheet.Cells[36, 3, 36, subLastCol], xRange);
            series3.Header = "Except Schedule OEE";

            // 添加系列3
            var series4 = rate_chart.Series.Add(worksheet.Cells[37, 3, 37, subLastCol], xRange);
            series4.Header = "Machine DownRate";

            // 添加系列3
            var series5 = rate_chart.Series.Add(worksheet.Cells[38, 3, 38, subLastCol], xRange);
            series5.Header = "Schedule DownRate";
            // 设置图表位置
            rate_chart.SetPosition(33, 0, subLastCol + 1, 0);
            rate_chart.SetSize(1200, 400);
            series2.DataLabel.ShowValue = true;
            series2.DataLabel.ShowPercent = true;
            series2.DataLabel.Position = eLabelPosition.Top;
            series4.DataLabel.ShowValue = true;
            series4.DataLabel.ShowPercent = true;
            series4.DataLabel.Position = eLabelPosition.Top;
            series5.DataLabel.ShowValue = true;
            series5.DataLabel.ShowPercent = true;
            series5.DataLabel.Position = eLabelPosition.Top;

            rate_chart.Legend.Add();

            summaryWs.Cells[2, summaryCol].Value = worksheet.Cells[9, datetimeColLen].Value;
            summaryWs.Calculate();
            if (Convert.ToDecimal(worksheet.Cells[9, datetimeColLen].Value) < Convert.ToDecimal(summaryWs.Cells[2, 3].Value))
            {
                summaryWs.Cells[2, summaryCol].Style.Font.Color.SetColor(System.Drawing.Color.Red);
            }
            else
            {
                summaryWs.Cells[2, summaryCol].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
            }

            summaryWs.Cells[4, summaryCol].Value = worksheet.Cells[13, datetimeColLen].Value;
            summaryWs.Cells[2, summaryCol].Style.Numberformat.Format = "0.00%";

        }
        private static void HandleLoadingRateSheet(ExcelWorksheet summaryWs, int summaryCol, ExcelWorksheet worksheet, List<OEERawData> newData, DateTime datetime)
        {
            // 定义所有 9 种情况
            var machineCnt = newData.Select(it => it.EQID).Distinct().Count();
            var workTimeSum = newData.GroupBy(it => it.EQID).Select(it => it.Sum(x => x.WorkTime)).Average();//.Select(it => new { avgWorkTime = it.Sum(x => x.WorkTime)/ machineCnt }).FirstOrDefault().avgWorkTime;
            var reelSizes = new decimal[] { 10, 15, 20 };
            var integratedTypes = new[] { "2", "3", "4" };
            var allCombinations = (
               from reelSize in reelSizes
               from type in integratedTypes
               select new { ReelSize = reelSize, IntegratedType = type }
           ).ToList();

            // 左连接填充缺失值为 0
            // 使用 GroupJoin 进行连接，确保每个组合都有一个对应的记录
            var result = (from record in allCombinations
                          join count in newData
                          on new { record.ReelSize, record.IntegratedType }
                          equals new { count.ReelSize, count.IntegratedType } into joined
                          from j in joined.DefaultIfEmpty()  // 如果没有匹配项，使用默认值
                          group j by new { record.ReelSize, record.IntegratedType } into g
                          select new
                          {

                              g.Key.ReelSize,
                              g.Key.IntegratedType,
                              TotalOutput = g.Sum(x => x?.Output ?? 0),  // 计算 Output 总和
                              TotalWorkTime = g.Sum(x => x?.WorkTime ?? 0),
                              TotalMachine = machineCnt,
                              UPH = g.FirstOrDefault(x => x != null)?.UPH ?? 11, // 获取 UPH，如果没有匹配则使用默认值
                              OEETarget = g.FirstOrDefault(x => x != null)?.OEETarget / 100 ?? 0
                          }).ToList();


            // 获取最后一列（避免空列的影响）
            int lastColumn = GetLastColumnWithData(worksheet);
            // 添加新数据到新的列
            int columnOffset = lastColumn + 1; // 下一列的索引
            int row = 2; // 假设第一行是标题，数据从第二行开始
            worksheet.Cells[1, columnOffset].Value = datetime.ToString("MM/dd");
            decimal loadingRate = 0;
            for (int i = 0; i < result.Count; i++)
            {

                worksheet.Cells[row, columnOffset].Value = result[i].TotalOutput;
                loadingRate += result[i].TotalOutput == 0 ? 0 :
                    result[i].TotalOutput / (result[i].UPH * workTimeSum * result[i].OEETarget * result[i].TotalMachine);
                worksheet.Cells[row, 4].FormulaR1C1 = $"SUM(RC[1]:RC[{columnOffset - 4}])";
                //result[i].TotalOutput / (result[i].UPH * result[i].TotalWorkTime * result[i].OEETarget * result[i].TotalMachine);
                row++;
            }


            worksheet.Cells[11, 4].FormulaR1C1 = $"SUM(RC[1]:RC[{columnOffset - 4}])";
            worksheet.Cells[13, 4].FormulaR1C1 = $"AVERAGE(RC[1]:RC[{columnOffset - 4}])";
            worksheet.Cells[13, 4].Style.Numberformat.Format = "0.00%";
            worksheet.Cells[11, columnOffset].FormulaR1C1 = $"SUM(R[-9]C:R[-1]C)";
            worksheet.Cells[11, 2, 11, columnOffset].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            worksheet.Cells[11, 2, 11, columnOffset].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Orange);

            worksheet.Cells[12, columnOffset].Value = machineCnt;
            worksheet.Cells[13, columnOffset].Value = loadingRate > 1 ? 1 : loadingRate;
            worksheet.Cells[13, columnOffset].Style.Numberformat.Format = "0.00%";
            summaryWs.Cells[3, summaryCol].Value = worksheet.Cells[13, columnOffset].Value;
            summaryWs.Cells[3, summaryCol].Style.Numberformat.Format = "0.00%";
            if (Convert.ToDecimal(worksheet.Cells[13, columnOffset].Value) < Convert.ToDecimal(summaryWs.Cells[3, 3].Value))
            {
                summaryWs.Cells[3, summaryCol].Style.Font.Color.SetColor(System.Drawing.Color.Red);
            }
            else
            {
                summaryWs.Cells[3, summaryCol].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
            }


            worksheet.Cells[13, 2, 13, columnOffset].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            worksheet.Cells[13, 2, 13, columnOffset].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGreen);


            var cellrange = worksheet.Cells[1, 1, 13, columnOffset];
            cellrange.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            cellrange.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            cellrange.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            cellrange.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;


            worksheet.Drawings.Clear();
            // 生成图表1
            var rate_chart = worksheet.Drawings.AddLineChart("OEEChart", eLineChartType.LineMarkers);
            rate_chart.Title.Text = "Daily Loading Rate";
            //chart.ShowDataLabelsOverMaximum = true;

            // 设置X轴分类
            var xRange = worksheet.Cells[1, 5, 1, columnOffset];

            // 添加系列 OEE Target
            var series1 = rate_chart.Series.Add(worksheet.Cells[13, 5, 13, columnOffset], xRange);
            series1.Header = "Daily Loading Rate";

            // 设置图表位置
            rate_chart.SetPosition(15, 0, 1, 0);
            rate_chart.SetSize(1200, 400);
            series1.DataLabel.ShowValue = true;
            series1.DataLabel.ShowPercent = true;
            series1.DataLabel.Position = eLabelPosition.Top;
            rate_chart.Legend.Add();



            return;


        }

        public void GenerateFileTemplate(ExcelPackage package, int month)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage ep = package;
            ExcelWorkbook wb = ep.Workbook;
            var summarySheet = ep.Workbook.Worksheets.Add("Summary");
            var oeeSheet = ep.Workbook.Worksheets.Add("OEE");
            var loadingRateSheet = ep.Workbook.Worksheets.Add("Loading Rate");

            summarySheet.Cells["A1"].Value = "合料机";
            summarySheet.Cells["A2"].Value = "OEE";
            summarySheet.Cells["A3"].Value = "Loading Rate";
            summarySheet.Cells["A4"].Value = "每日生产盘数";
            summarySheet.Cells["B1"].Value = "2024 \r\nYTD";
            summarySheet.Cells["C1"].Value = "2025 \r\nTarget";
            summarySheet.Cells["D1"].Value = "2025 \r\nYTD";
            summarySheet.Cells["E1"].Value = "Jan'25 Actual";
            summarySheet.Cells["F1"].Value = $"{month}月";

            summarySheet.Cells[1, 1, 4, 5].Style.Font.Bold = true;
            summarySheet.Cells[1, 1, 4, 5].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            summarySheet.Cells[1, 1, 4, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;


            oeeSheet.Cells[2, 1].Value = "日期";
            oeeSheet.Cells[3, 1].Value = "机台编号";
            oeeSheet.Cells[4, 1].Value = "类型";
            oeeSheet.Cells[5, 1].Value = "合盘种类";

            oeeSheet.Cells[6, 1].Value = "工时（S/卷)";
            oeeSheet.Cells[7, 1].Value = "UPH (1H/卷)";
            oeeSheet.Cells[8, 1].Value = "OEE Target";
            oeeSheet.Cells[9, 1].Value = "OEE Actual";
            oeeSheet.Cells[10, 1].Value = "Except Schedule OEE Actual";
            oeeSheet.Cells[11, 1].Value = "工作时间(H)";
            oeeSheet.Cells[12, 1].Value = "Output Target(H)";
            oeeSheet.Cells[13, 1].Value = "Output Actual";

            oeeSheet.Cells[15, 1].Value = "Machine Down Time(Mins)";
            oeeSheet.Cells[16, 1].Value = "Others(Mins)";
            oeeSheet.Cells[17, 1].Value = "Total Down Rate";
            oeeSheet.Cells[18, 1].Value = "Impact Output";


            oeeSheet.Cells[20, 1].Value = "首件/校验机台（Mins）①";
            oeeSheet.Cells[21, 1].Value = "复位②";
            oeeSheet.Cells[22, 1].Value = "Total(Mins)";
            oeeSheet.Cells[23, 1].Value = "Impact Output";
            oeeSheet.Cells[24, 1].Value = "排停(Mins)(idle)";
            oeeSheet.Cells[25, 1].Value = "排停百分比";
            oeeSheet.Cells[26, 1].Value = "排停Impact Output";
            oeeSheet.Cells[27, 1].Value = "Service";
            oeeSheet.Cells[28, 1].Value = "Total(Mins)";
            oeeSheet.Cells[29, 1].Value = "百分比";
            oeeSheet.Cells[30, 1].Value = "Total OEE";
            oeeSheet.Cells[31, 1].Value = "OEE Loss";

            oeeSheet.Cells[2, 2].Value = $"{month}月";

            oeeSheet.Cells[33, 1].Value = "Date";
            oeeSheet.Cells[33, 2].Value = $"{month}月";
            oeeSheet.Cells[34, 1].Value = "OEE Target";
            oeeSheet.Cells[35, 1].Value = "OEE Actual";

            oeeSheet.Cells[36, 1].Value = "Except Schedule OEE Actual";
            oeeSheet.Cells[37, 1].Value = "Machine DownTime";
            oeeSheet.Cells[38, 1].Value = "Schedule DownTime";

            var colTitleRange = oeeSheet.Cells[2, 1, 38, 1];
            colTitleRange.Style.Font.Bold = true;

            loadingRateSheet.Cells[1, 2].Value = "Date";

            loadingRateSheet.Cells[1, 1, 1, 3].Merge = true;
            loadingRateSheet.Cells[1, 4].Value = $"{month}月";
            loadingRateSheet.Cells[2, 1].Value = "Ave.Actual Output (盘)";
            loadingRateSheet.Cells[2, 2].Value = "10k";
            loadingRateSheet.Cells[2, 2, 4, 2].Merge = true;
            loadingRateSheet.Cells[2, 3].Value = "2合1";
            loadingRateSheet.Cells[3, 3].Value = "3合1";
            loadingRateSheet.Cells[4, 3].Value = "4合1";

            loadingRateSheet.Cells[5, 2].Value = "15k";
            loadingRateSheet.Cells[5, 2, 7, 2].Merge = true;
            loadingRateSheet.Cells[5, 3].Value = "2合1";
            loadingRateSheet.Cells[6, 3].Value = "3合1";
            loadingRateSheet.Cells[7, 3].Value = "4合1";

            loadingRateSheet.Cells[8, 2].Value = "20k";
            loadingRateSheet.Cells[8, 2, 10, 2].Merge = true;
            loadingRateSheet.Cells[8, 3].Value = "2合1";
            loadingRateSheet.Cells[9, 3].Value = "3合1";
            loadingRateSheet.Cells[10, 3].Value = "4合1";

            loadingRateSheet.Cells[11, 2].Value = "Total";

            loadingRateSheet.Cells[2, 1, 11, 1].Merge = true;

            loadingRateSheet.Cells[12, 1].Value = "Daily";
            loadingRateSheet.Cells[12, 2].Value = "Machine Run";
            loadingRateSheet.Cells[13, 2].Value = "Loading Rate";
            loadingRateSheet.Cells[12, 1, 13, 1].Merge = true;
            loadingRateSheet.Cells[1, 1, 13, 3].Style.Font.Bold = true;
            loadingRateSheet.Cells[1, 1, 13, 3].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            loadingRateSheet.Cells[1, 1, 13, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;


            ep.Save();


        }
        // 安全获取最后一列有数据的列
        private static int GetLastColumnWithData(ExcelWorksheet worksheet)
        {
            int lastColumn = 1;
            // 遍历行
            for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
            {
                for (int row = 1; row <= worksheet.Dimension.End.Row; row++)
                {
                    if (worksheet.Cells[row, col].Value != null)
                    {
                        lastColumn = Math.Max(lastColumn, col); // 获取最大列
                    }
                }
            }
            return lastColumn;
        }

        // 获取带颜色的列并设置公式
        private static void SetFormulaByColor(ExcelWorksheet worksheet, int targetCol, int finalCol, string formulaType, string format)
        {
            List<ExcelRange> orangeCells = new List<ExcelRange>();

            // 遍历目标行的所有单元格
            for (int col = 1; col <= finalCol + 1; col++)
            {
                var cell = worksheet.Cells[targetCol, col];

                // 检查该单元格的背景色是否为橙色
                if (cell.Style.Fill.BackgroundColor.Rgb == "FFFFA500") // Orange的RGB值
                {
                    orangeCells.Add(cell);
                }
            }

            // 如果找到了橙色单元格，则可以计算它们的平均值
            if (orangeCells.Count > 0)
            {
                // 使用 Excel 公式设置计算平均值
                var range = worksheet.Cells[targetCol, finalCol + 1];
                string formula = $"{formulaType}(";

                foreach (var cell in orangeCells)
                {
                    formula += cell.Address + ",";
                }

                formula = formula.TrimEnd(',') + ")";
                range.Formula = formula;
                if (!String.IsNullOrEmpty(format))
                    range.Style.Numberformat.Format = format;
            }
        }
        protected void ConnectLan(string p_Path, string p_UserName, string p_PassWord)
        {
            //Match m = Regex.Match(p_Path, @"\d{1,3}.\d{1,3}.\d{1,3}.\d{1,3}");
            //SharedTool stToIP = new SharedTool(p_UserName, p_PassWord, m.Value);
            System.Diagnostics.Process _Process = new System.Diagnostics.Process();
            _Process.StartInfo.FileName = "cmd.exe";
            _Process.StartInfo.UseShellExecute = false;
            _Process.StartInfo.RedirectStandardInput = true;
            _Process.StartInfo.RedirectStandardOutput = true;
            _Process.StartInfo.CreateNoWindow = true;
            _Process.Start();
            //NET USE //192.168.0.1 PASSWORD /USER:UserName
            _Process.StandardInput.WriteLine("net use " + p_Path + " " + p_PassWord + " /user:" + p_UserName);
            _Process.StandardInput.WriteLine("exit");
            _Process.WaitForExit(5000);
            Thread.Sleep(200);
            //  string _ReturnText = _Process.StandardOutput.ReadToEnd();// 得到cmd.exe的输出 
            _Process.Close();
            // return _ReturnText;
        }
        public class SuccessFail
        {
            public DateTime period_start { get; set; }
            public decimal success_count { get; set; }
            public decimal fail_count { get; set; }
            public decimal total_count { get; set; }
        }
        public class OEERawData
        {
            public string EQID { get; set; }
            public string IntegratedType { get; set; }
            public decimal ReelSize { get; set; }
            public string DateTime { get; set; }
            public int Output { get; set; }

            public decimal WorkTime { get; set; }

            public decimal CT { get; set; }

            public decimal UPH { get; set; }

            public decimal OEETarget { get; set; }

            public decimal OutputTarget { get; set; }

            public decimal DownTime { get; set; }

            public decimal IdleTime { get; set; }

            public decimal FAITime { get; set; }


        }
        public class StatusTimePoint
        {
            public string TimeLabel { get; set; } // 如 "08:00"
            public double Minutes { get; set; }
        }
        public class DownTimeResult
        {
            public string Date { get; set; } // yyyy-MM-dd
            public double DownMinutes { get; set; }
        }

    }
}
