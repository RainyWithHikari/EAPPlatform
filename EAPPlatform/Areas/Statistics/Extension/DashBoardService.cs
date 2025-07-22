using EAPPlatform.DataAccess;
using EAPPlatform.Model.EAP;
using EAPPlatform.Util;
using Microsoft.AspNet.SignalR.Client.Http;
using NPOI.HPSF;
using NPOI.SS.Formula.Functions;
using OfficeOpenXml;
using SQLitePCL;
using SqlSugar;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;

namespace EAPPlatform.Areas.Statistics.Extension
{
    public class DashBoardService
    {
        public static List<DetailChartData> GetChartData(DataContext db, string equipment, string datetime)
        {
 
            DateTime starttime = new();
            DateTime chartstarttime = new();
            DateTime endtime = new();
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
                var startdateList = db.EquipmentParamsHistoryCalculate
              .Where(it => it.Name == "starttime"
              && it.Remark == datetime
              && it.EQID == "ACCBOX").ToList();
                var enddateList = db.EquipmentParamsHistoryCalculate
                    .Where(it => it.Name == "endtime"
                    && it.Remark == datetime
                    && it.EQID == "ACCBOX").ToList();
                starttime = Convert.ToDateTime(startdateList.OrderByDescending(it => Convert.ToDateTime(it.Value)).First().Value);
                endtime = Convert.ToDateTime(enddateList.OrderByDescending(it => Convert.ToDateTime(it.Value)).First().Value);
                chartstarttime = starttime;
            }

            var rawalarmdata = db.EquipmentAlarm.Where(it => it.AlarmEqp == equipment && it.AlarmTime > starttime && it.AlarmTime <= endtime).OrderBy(it => it.AlarmTime).ToList();
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
                periodColor = color[statusdata.Rows[i][0].ToString()];
                periodStatusInt = statusInt[statusdata.Rows[i][0].ToString()];
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
            var alarmdata = rawalarm.Where(it => it.AlarmEqp == equipment && it.AlarmTime > periodStartTime && it.AlarmTime <= periodEndTime).OrderBy(it => it.AlarmTime)
                .Select(it => new { ALARMTIME = it.AlarmTime, ALARMTEXT = it.AlarmText }).ToList();
            foreach (var item in alarmdata)
            {
                alarmtext += Convert.ToDateTime(item.ALARMTIME).ToString("yyyy-MM-dd HH:mm:ss") + " " + item.ALARMTEXT;
                alarmtext += "<br>";
            }
            return alarmtext;
        }

        public static bool CalculateAlarmRate(DataContext db, List<Equipment> eqids, DateTime starttime, DateTime endtime, string week, int idleduration)
        {
            string sqlrate = string.Empty;
            string sqltime = string.Empty;
            try
            {
                for (int i = 0; i < eqids.Count; i++)
                {
                    var eqid = eqids[i].EQID.ToString();
                    var eqguid = eqids[i].ID;

                    //var startdateList = db.EquipmentParamsHistoryCalculate.Where(it => it.EQID == eqid && it.Name == "starttime").ToList();
                    //starttime = Convert.ToDateTime(startdateList.OrderByDescending(it => Convert.ToDateTime(it.Value)).First().Value);
                    //var enddateList = db.EquipmentParamsHistoryCalculate.Where(it => it.EQID == eqid && it.Name == "endtime").ToList();
                    //endtime = Convert.ToDateTime(enddateList.OrderByDescending(it => Convert.ToDateTime(it.Value)).First().Value);
                    TimeSpan mfgtime = endtime - starttime;
                    TimeSpan alarmspan = new();
                    var alarmcodeList = db.EquipmentConfigurations.Where(it => it.EQIDId == eqguid).Select(it => it.ConfigurationValue).First();
                    if (alarmcodeList != null)
                    {
                        String[] alarmcodes = alarmcodeList.Split(",");
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
                            var eqpalarm = db.EquipmentAlarm.Where(it => it.AlarmEqp.ToUpper() == eqid
                            && it.AlarmTime > starttime && it.AlarmTime <= endtime
                            && alarmcodes.Contains(it.AlarmCode)).OrderByDescending(it => it.AlarmTime)
                            .Select(it => new { ALARMTIME = it.AlarmTime, ALARMCODE = it.AlarmCode, ALARMTEXT = it.AlarmText, ALARMSET = it.AlarmSet }).ToList();

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
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }

        }
        public static DataTable ToDataTable<T>(IEnumerable<T> list)
        {
            //Transfer List to DataTable
            PropertyInfo[] modelItemType = typeof(T).GetProperties();
            DataTable dataTable = new DataTable();
            dataTable.Columns.AddRange(modelItemType.Select(Columns => new DataColumn(Columns.Name, Columns.PropertyType)).ToArray());
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
        public static List<AlarmItem> GetAlarmDetails(DataContext db, DateTime startdate, DateTime enddate, string EQID)
        {
            List<AlarmItem> alarmList = new();
            TimeSpan alarmspan = new();
            var date = Convert.ToDateTime(startdate).Date;
            var alarmcodeList = db.EquipmentConfigurations.Where(it => it.EQID.EQID == EQID).Select(it => it.ConfigurationValue).First();
            String[] alarmcodes = alarmcodeList.Split(",");

            var eqpalarm = db.EquipmentAlarm.Where(it => it.AlarmEqp.ToUpper() == EQID
                    && it.AlarmTime > startdate && it.AlarmTime <= enddate
                        && alarmcodes.Contains(it.AlarmCode)).OrderByDescending(it => it.AlarmTime)
                        .Select(it => new { ALARMTIME = it.AlarmTime, ALARMCODE = it.AlarmCode, ALARMTEXT = it.AlarmText, ALARMSET = it.AlarmSet }).ToList();
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
                                        Date = eqpalarm[j].ALARMTIME.ToString().Split(' ')[0],
                                        EQID = EQID,
                                        AlarmCode = eqpalarm[k].ALARMCODE,
                                        AlarmText = eqpalarm[j].ALARMTEXT,
                                        StartTime = (DateTime)eqpalarm[k].ALARMTIME,
                                        EndTime = (DateTime)eqpalarm[j].ALARMTIME,
                                        Duration = double.Parse(alarmspan.TotalMinutes.ToString("0.00"))

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
                                break;
                            }
                        }


                    }
                }





            }
            return alarmList;

        }

        public static void Tmp(DataContext db)
        {
            string sql = string.Format(@"SELECT EQID FROM EQUIPMENT WHERE (EQID = 'EQLAS00185' OR EQID='EQDIC00007' OR EQID = 'EQMLD00003')");
            var eqlist = OracleHelper.GetDTFromCommand(sql);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage ep = new();
            ExcelWorkbook wb = ep.Workbook;
            for (int i = 0; i < eqlist.Rows.Count; i++)
            {
                var chartdata = GetChartDataTEST(db, eqlist.Rows[i][0].ToString(), "2022-09-22");
                var charttable = ToDataTable(chartdata);
                DataTable details = DashBoardService.ToDataTable(chartdata);
                //DataTable total = DashBoardService.ToDataTable(alarmtotal);

                ExcelWorksheet ws = wb.Worksheets.Add(eqlist.Rows[i][0].ToString());
                ws.Cells["A1"].LoadFromDataTable(details, true);
                ws.Column(2).Style.Numberformat.Format = "yyyy/m/d h:mm:ss";
                ws.Column(3).Style.Numberformat.Format = "yyyy/m/d h:mm:ss";
                //ws.Column(4).Style.Numberformat.Format = "yyyy/m/d h:mm:ss";
                //ws.Column(4).Style.Numberformat.Format = "0.00";

            }
            var filename = "TripLamp" + $"StatusData.xlsx";
            Directory.CreateDirectory(System.AppDomain.CurrentDomain.BaseDirectory + $"ExcelReport");
            string route = string.Format(System.AppDomain.CurrentDomain.BaseDirectory + $"ExcelReport\\{filename}");
            ep.SaveAs(new FileInfo(route));
        }

        public static List<DetailChartDataForTrip> GetChartDataTEST(DataContext db, string equipment, string datetime)
        {
            DateTime starttime = new();
            DateTime chartstarttime = new();
            DateTime endtime = new();
            DateTime nowtime = DateTime.Now;
            if (!datetime.StartsWith("W"))
            {
                starttime = Convert.ToDateTime(datetime + " 00:00:00");
                chartstarttime = Convert.ToDateTime(datetime + " 00:00:00");
                endtime = Convert.ToDateTime(datetime + " 00:00:00").AddDays(1);
                nowtime = DateTime.Now;
            }
            else
            {
                var startdateList = db.EquipmentParamsHistoryCalculate
              .Where(it => it.Name == "starttime"
              && it.Remark == datetime
              && it.EQID == "ACCBOX").ToList();
                var enddateList = db.EquipmentParamsHistoryCalculate
                    .Where(it => it.Name == "endtime"
                    && it.Remark == datetime
                    && it.EQID == "ACCBOX").ToList();
                starttime = Convert.ToDateTime(startdateList.OrderByDescending(it => Convert.ToDateTime(it.Value)).First().Value);
                endtime = Convert.ToDateTime(enddateList.OrderByDescending(it => Convert.ToDateTime(it.Value)).First().Value);
                chartstarttime = starttime;
            }

            var rawalarmdata = db.EquipmentAlarm.Where(it => it.AlarmEqp == equipment && it.AlarmTime > starttime && it.AlarmTime <= endtime).OrderBy(it => it.AlarmTime).ToList();
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
ORDER BY EQID, DATETIME", equipment, starttime.ToString("yyyy-MM-dd HH:mm:ss"), endtime.ToString("yyyy-MM-dd HH:mm:ss"));
            var statusdata = OracleHelper.GetDTFromCommand(sql);
            List<DetailChartDataForTrip> chardata = new List<DetailChartDataForTrip>();
            Dictionary<string, string> color = new Dictionary<string, string> { { "Run", "#75d874" }, { "Alarm", "#e3170d" }, { "Idle", "#ff9912" }, { "Down", "#cccccc" }, { "Unknow", "#082e54" } };
            Dictionary<string, int> statusInt = new Dictionary<string, int> { { "Run", 1 }, { "Alarm", 2 }, { "Idle", 3 }, { "Down", 4 }, { "Unknow", 5 } };
            for (int i = 0; i < statusdata.Rows.Count; i++)
            {
                DateTime periodStartTime = new DateTime();
                DateTime periodEndTime = Convert.ToDateTime(statusdata.Rows[i][1].ToString());
                if (periodEndTime < chartstarttime) continue;//筛选5点之前的事件
                string periodStatus = string.Empty;
                string periodColor = string.Empty;
                int periodStatusInt = 0;
                string alarmtext = string.Empty;
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
                    periodStatus = statusdata.Rows[i - 1][0].ToString();
                    periodColor = color[statusdata.Rows[i - 1][0].ToString()];
                    periodStatusInt = statusInt[statusdata.Rows[i - 1][0].ToString()];
                }
                if (periodStartTime < chartstarttime) periodStartTime = chartstarttime;
                TimeSpan timeSpan = periodEndTime - periodStartTime;
                alarmtext = GetAlarmText(rawalarmdata, equipment, periodStartTime, periodEndTime);
                chardata.Add(new DetailChartDataForTrip
                {
                    name = periodStatus,
                    start = periodStartTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    end = periodEndTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    duration = timeSpan.TotalMinutes.ToString("0.00")
                });
                if (i == statusdata.Rows.Count - 1)
                {
                    if (endtime < nowtime)
                        periodEndTime = endtime;
                    else
                        periodEndTime = nowtime;
                    periodStartTime = Convert.ToDateTime(statusdata.Rows[i][1].ToString());
                    periodStatus = statusdata.Rows[i][0].ToString();
                    periodColor = color[statusdata.Rows[i][0].ToString()];
                    periodStatusInt = statusInt[statusdata.Rows[i][0].ToString()];
                    timeSpan = periodEndTime - periodStartTime;
                    alarmtext = GetAlarmText(rawalarmdata, equipment, periodStartTime, periodEndTime);
                    chardata.Add(new DetailChartDataForTrip
                    {
                        name = periodStatus,
                        start = periodStartTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        end = periodEndTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        duration = timeSpan.TotalMinutes.ToString("0.00")
                    });
                }
            }
            return chardata;
        }

        public static List<EquipmentAlarmTimes> GetAlarmTimeList(DataContext db)
        {
            List<EquipmentAlarmTimes> alarmArr = new();

            var eqptypeids = db.EquipmentTypes.Where(it => it.Name == "ACCBOX").Select(it => it.ID).ToList();
            var eqpidids = db.Equipments.Where(it => eqptypeids.Contains((Guid)it.EquipmentTypeId)).OrderBy(it => it.Sort).Select(it => it.EQID).ToList();
            var weeks = db.EquipmentParamsHistoryCalculate
                .Where(it => it.EQID == "ACCBOX" && it.Remark != null && it.Name == "starttime")
                .Select(it => it.Remark)
                .Distinct()
                .ToList()
                .OrderBy(it => Convert.ToDouble(it.Replace("W", ""))).TakeLast(24);
            //.OrderBy(it => Convert.ToDouble(it.Split("W")[1]));//.TakeLast(8);
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


                //List<double> alarmrates = new();
                List<double> alarmtimes = new();

                double alarmtime = 0;
                //alarmrates = DC.Set<EquipmentParamsHistoryCalculate>()
                //.Where(it => eqpidids.Contains(it.EQID) && it.Remark == item && it.Name == "alarmrate").Select(it => Convert.ToDouble(it.Value)).ToList();
                alarmtimes = db.EquipmentParamsHistoryCalculate
                    .Where(it => eqpidids.Contains(it.EQID) && it.Remark == item && it.Name == "alarmspan")
                    .Select(it => Convert.ToDouble(it.Value)).ToList();
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
                EquipmentAlarmTimes alarmItem = new() { time = item, alarmtimes = alarmtime, mfgtimes = mfgspan.TotalMinutes - idleTime };
                alarmArr.Add(alarmItem);
                //totalrate.Add(double.Parse(alarmrate.ToString("0.00")));
                //totaltime.Add(double.Parse(alarmtime.ToString("0.00")));
                //totalmfg.Add(double.Parse(restTime.ToString("0.00")));

            }

            return alarmArr;
        }

        public static List<EquipmentAlarmTimes> GetAlarmMonthlyList(List<EquipmentAlarmTimes> arr)
        {
            List<EquipmentAlarmTimes> alarmArr = new();

            arr.ForEach(it =>
            {
                int year = Convert.ToInt32(it.time.Split('W')[0]);

                DateTime startOfYear = new DateTime(year, 1, 1);
                int offset = startOfYear.DayOfWeek == DayOfWeek.Sunday ? 6 : ((int)startOfYear.DayOfWeek - 1);

                DateTime targetWeek = startOfYear.AddDays(offset + (Convert.ToDouble(it.time.Split("W")[1]) - 1) * 7);
                int targetMonth = targetWeek.Month;
                string MonthNumber = year.ToString() + 'M' + targetMonth.ToString();
                if (!alarmArr.Any(alarm => alarm.time.Contains(MonthNumber)))
                {
                    EquipmentAlarmTimes alarmItem = new() { time = MonthNumber, alarmtimes = it.alarmtimes, mfgtimes = it.mfgtimes };
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

        public static Dictionary<byte[], string> GenerateFiles(DataContext db, DateTime start, DateTime end)
        {
            var eqptypeids = db.EquipmentTypes.Where(it => it.Name == "ACCBOX").Select(it => it.ID).ToList();
            var eqpidids = db.Equipments.Where(it => eqptypeids.Contains((Guid)it.EquipmentTypeId)).OrderBy(it => it.Sort).ToList();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            Dictionary<byte[], string> filesDict = new();
            List<byte[]> fileBytesArr = new List<byte[]>();
            foreach (var eqp in eqpidids)
            {
                ExcelPackage ep = new();
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    ExcelWorkbook wb = ep.Workbook;
                    var list = db.EquipmentParamsHistoryRaw.Where(it => it.EQID == eqp.EQID
                    && it.Name == "yield"
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
                    var filename = $"{eqp.EQID}_Yield.xlsx";
                    var datastring = ep.GetAsByteArray();
                    filesDict.Add(datastring, filename);


                }


            }
            return filesDict;
        }
        public class ParamsList
        {
            public string EQID { get; set; }
            public string OUTPUT { get; set; }
            public string YIELD { get; set; }
            public DateTime DATE { get; set; }

        }
    }
}
