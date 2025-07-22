using EAP.Dashboard.Models;
using EAP.Dashboard.Utils;
using EAP.Models.Database;
using Microsoft.Ajax.Utilities;
using Microsoft.Kiota.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Web.Http;

namespace EAP.Dashboard.Controllers
{

    public class RequestFormat
    {
        public string EqTypeId { get; set; }
        public string EqType { get; set; }
        public string EQID { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }

    public class ResponseStatus
    {
        public bool success { get; set; }
        public string message { get; set; }
        public string eqid { get; set; }
        public string eqType { get; set; }
        public string query_start_time { get; set; }
        public string query_endtime { get; set; }
        public List<DetailChartDataForTrip> data { get; set; }

    }
    public class ResponseAlarm
    {
        public bool success { get; set; }
        public string message { get; set; }
        public string eqid { get; set; }
        public string eqType { get; set; }
        public string query_start_time { get; set; }
        public string query_endtime { get; set; }
        public List<AlarmItem> data { get; set; }

    }

    public class ResponseEqpType
    {
        public bool success { get; set; }
        public string message { get; set; }

        public List<EquipmentType> data { get; set; }

    }
    public class ResponseEqp
    {
        public bool success { get; set; }
        public string message { get; set; }

        public List<string> data { get; set; }

    }
    [RoutePrefix("api/data")]
    public class DataController : ApiController
    {
        private static log4net.ILog Log = log4net.LogManager.GetLogger("Logger");


        [HttpPost]
        [Route("eqtype")]
        public IHttpActionResult GetEqTypes()
        {
            var db = DbFactory.GetSqlSugarClient();
            var eqTypes = db.Queryable<EquipmentType>().ToList();
            var result = new ResponseEqpType
            {

                success = true,
                message = "Query successful",

                data = eqTypes
            };


            return Ok(result);



        }

        [HttpPost]
        [Route("eqp")]
        public IHttpActionResult GetEqps([FromBody] RequestFormat request)
        {
            if (request.EqTypeId == null)
            {
                return BadRequest("Invalid or missing JSON body.");
            }
            var test = Encoding.UTF8.GetBytes(request.EqTypeId);
            var db = DbFactory.GetSqlSugarClient();
            var eqTypes = db.Queryable<Equipment>().ToList().Where(it => Convert.ToBase64String(it.EquipmentTypeId) == request.EqTypeId).Select(it=>it.EQID).ToList();
            var result = new ResponseEqp
            {

                success = true,
                message = "Query successful",

                data = eqTypes
            };


            return Ok(result);



        }


        [HttpPost]
        [Route("status")]
        public IHttpActionResult GetStatus([FromBody] RequestFormat request)
        {
            if (request.EQID == null || request.StartTime == null || request.EndTime == null)
            {
                return BadRequest("Invalid or missing JSON body.");
            }
            try
            {
                var EQType = request.EqType;
                var EQID = request.EQID;
                var starttime = Convert.ToDateTime(request.StartTime);
                var endtime = Convert.ToDateTime(request.EndTime);

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
                        if (periodStartTime < starttime)
                        {
                            if (statusdata.Rows.Count > (i + 1))
                            {
                                if (Convert.ToDateTime(statusdata.Rows[i + 1][1]) > starttime)
                                {
                                    periodStartTime = starttime;
                                    periodEndTime = Convert.ToDateTime(statusdata.Rows[i + 1][1]);
                                }
                                else
                                {
                                    continue;
                                }

                            }
                            else
                            {
                                periodStartTime = starttime;
                                periodEndTime = endtime;


                            }
                        }
                        else
                        {
                            if (statusdata.Rows.Count > (i + 1))
                            {
                                periodEndTime = Convert.ToDateTime(statusdata.Rows[i + 1][1]);
                                periodStartTime = starttime;
                            }
                            else
                            {
                                periodStartTime = starttime;
                                periodEndTime = endtime;


                            }


                        }

                    }
                    else if (i == statusdata.Rows.Count - 1)
                    {
                        periodStartTime = Convert.ToDateTime(statusdata.Rows[i][1].ToString());
                        periodEndTime = endtime;
                        if (periodStartTime < starttime) periodStartTime = starttime;

                    }
                    else
                    {
                        periodStartTime = Convert.ToDateTime(statusdata.Rows[i][1].ToString());
                        if (periodStartTime < starttime)
                        {
                            if (Convert.ToDateTime(statusdata.Rows[i + 1][1]) > starttime)
                            {
                                periodStartTime = starttime;
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
                var result = new ResponseStatus
                {
                    eqid = EQID,
                    eqType = request.EqType,
                    success = true,
                    message = "Query successful",
                    query_start_time = starttime.ToString("yyyy-MM-dd HH:mm:ss"),
                    query_endtime = endtime.ToString("yyyy-MM-dd HH:mm:ss"),
                    data = chardata
                };
                Log.Info($"[DataController API] GET Status - EQID = {EQID}; StartTime = {starttime}; EndTime = {endtime};");

                return Ok(result);
            }
            catch (Exception ex)
            {
                Log.Error("[DataController API] " + ex.Message);
                return BadRequest(ex.Message);
            }

        }
        [HttpPost]
        [Route("alarm")]
        public IHttpActionResult GetAlarm([FromBody] RequestFormat request)
        {
            if (request.EQID == null || request.StartTime == null || request.EndTime == null)
            {
                return BadRequest("Invalid or missing JSON body.");
            }
            var EQID = request.EQID;
            var starttime = Convert.ToDateTime(request.StartTime);
            var endtime = Convert.ToDateTime(request.EndTime);
            try
            {


                var db = DbFactory.GetSqlSugarClient();
                var startstr = starttime.ToString("yyyy-MM-dd HH:mm:ss");
                var endstr = endtime.ToString("yyyy-MM-dd HH:mm:ss");

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
    WHERE ALARMTIME >= TO_DATE('{0}', 'yyyy-mm-dd hh24:mi:ss')
      AND ALARMTIME <= TO_DATE('{1}', 'yyyy-mm-dd hh24:mi:ss')
      AND UPPER(EQID) = '{2}'
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
   
)
SELECT
    TO_CHAR(a1.START_TIME, 'YYYY-MM-DD') AS AlarmDate,
    a1.EQID,
    a1.ALARMCODE AS AlarmCode,
    a1.ALARMTEXT AS AlarmText,
    a1.START_TIME AS StartTime,
    a1.END_TIME AS EndTime,
    CASE 
        WHEN a1.END_TIME IS NULL THEN 0
        ELSE ROUND((CAST(a1.END_TIME AS DATE) - CAST(a1.START_TIME AS DATE)) * 86400 / 60, 2)
    END AS Duration
FROM FilteredAlarm a1
LEFT JOIN FilteredAlarm a2 
    ON a1.ALARMCODE = a2.ALARMCODE 
    AND a1.rn = a2.rn + 1 
    AND a1.START_TIME <= a2.START_TIME + INTERVAL '2' MINUTE 
WHERE a2.START_TIME IS NULL
ORDER BY StartTime;", startstr, endstr, EQID.ToUpper());
                var alarmList = db.SqlQueryable<AlarmItem>(sql_total).ToList();
                var result = new ResponseAlarm
                {
                    eqid = EQID,
                    eqType = request.EqType,
                    success = true,
                    message = "Query successful",
                    query_start_time = starttime.ToString("yyyy-MM-dd HH:mm:ss"),
                    query_endtime = endtime.ToString("yyyy-MM-dd HH:mm:ss"),
                    data = alarmList
                };
                Log.Info($"[DataController API] GET Alarm - EQID = {EQID}; StartTime = {starttime}; EndTime = {endtime}; AlarmCounts = {alarmList.Count}");
                return Ok(result);

            }
            catch (Exception ex)
            {

                Log.Error("[DataController API] " + ex.Message);
                return BadRequest(ex.Message);
            }

        }


    }

}

