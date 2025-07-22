using EAP.Dashboard.Models;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace EAP.Dashboard.Controllers
{
    public class SMTStockController : BaseController
    {
        // GET: SMTStock
        private static log4net.ILog Log = log4net.LogManager.GetLogger("Logger");
        public ActionResult Index()
        {
            //var controllerName = ControllerContext.RouteData.Values["controller"].ToString();
            //if (controllers == null)
            //{
            //    return RedirectToAction("login", "account");
            //}
            //if (controllers.Contains(controllerName))
            //{
            //    return View();
            //}
            //else
            //{
            //    // 设置错误信息
            //    TempData["ErrorMessage"] = "您暂时没有权限访问该看板，请联系管理员申请访问！";

            //    return RedirectToAction("index", "home");
            //}
            return View();
        }

        public JsonResult GetQuantityOfTrayData(string enddate)
        {
            var now = DateTime.Now;
            var endtime = string.IsNullOrEmpty(enddate)? DateTime.Today.AddHours(5): Convert.ToDateTime(enddate).AddHours(5);
            if (now > endtime)
                endtime = endtime.AddDays(1);
            var starttime = endtime.AddDays(-6);
            var db = AutoReel_DbFactory.GetSqlSugarClient_AutoReel();
            #region 旧sql
            //            string sql = string.Format(@"SELECT TO_CHAR(TRUNC(CREATETIME - INTERVAL '5' HOUR, 'DD'),'yyyy-mm-dd') as DAY, 
            //DECODE(COMMANDTYPE, '1','人工入库','2','机器入库','3','人工出库','4','机器出库','6','人工出库') COMMANDNAME, 
            //COUNT(*) COMMANDQTY
            //FROM AR_A_COMMAND
            //WHERE CREATETIME >= TO_DATE('{0}', 'YYYY-MM-dd HH24:mi:ss')
            //AND CREATETIME < TO_DATE('{1}', 'YYYY-MM-dd HH24:mi:ss')
            //GROUP BY TRUNC(CREATETIME - INTERVAL '5' HOUR, 'DD'),COMMANDTYPE
            //ORDER BY DAY", starttime.ToString("yyyy-MM-dd HH:mm:ss"), endtime.ToString("yyyy-MM-dd HH:mm:ss"));
            #endregion

            string sql = string.Format(@"SELECT DAY,COMMANDNAME,SUM(COMMANDQTY) COMMANDQTY FROM
(SELECT TO_CHAR(TRUNC(CREATETIME - INTERVAL '5' HOUR, 'DD'),'yyyy-mm-dd') as DAY, 
DECODE(TO_NUMBER(COMMANDTYPE) * 2 + TO_NUMBER(MATERIALSOURCE), 2,'人工入库',4,'机器入库',5,'人工入库',6,'人工出库',8,'机器出库',12,'人工出库', '未知') COMMANDNAME, 
COUNT(*) COMMANDQTY
FROM USI_AUTO_REEL.AR_A_COMMAND
WHERE CREATETIME >= TO_DATE('{0}', 'YYYY-MM-dd HH24:mi:ss')
AND CREATETIME < TO_DATE('{1}', 'YYYY-MM-dd HH24:mi:ss')
AND COMMANDTYPE IN ('1','2','3','4','6')
GROUP BY TRUNC(CREATETIME - INTERVAL '5' HOUR, 'DD'),COMMANDTYPE, MATERIALSOURCE)
GROUP BY DAY,COMMANDNAME
ORDER BY DAY", starttime.ToString("yyyy-MM-dd HH:mm:ss"), endtime.ToString("yyyy-MM-dd HH:mm:ss"));

            var data = db.SqlQueryable<QuantityOfTray>(sql).ToList();
            return Json(new { data });
        }

        public JsonResult GetCTData(string enddate, string line, string orderno, string pn)
        {
            var now = DateTime.Now;
            var endtime = string.IsNullOrEmpty(enddate) ? DateTime.Today.AddHours(5) : Convert.ToDateTime(enddate).AddHours(5);
            if (now > endtime)
                endtime = endtime.AddDays(1);
            var starttime = endtime.AddDays(-1);
            var db = AutoReel_DbFactory.GetSqlSugarClient_AutoReel();
            #region 旧sql
            //            string sql = string.Format(@"SELECT LINE_1 LINE, MONUMBER_1 MONUMBER, REELID_1 REELID, COMP_PN_1 PN, 
            //FLOOR((LASTUPDATETIME - CREATETIME) *24 *60 *60) AS CT, 
            //DECODE(COMMANDTYPE, '1','人工入库','2','机器入库','3','人工出库','4','机器出库','6','人工出库') COMMANDNAME,
            //CREATETIME
            //FROM AR_A_COMMAND
            //WHERE CREATETIME >= TO_DATE('{0}', 'YYYY-MM-dd HH24:mi:ss')
            //AND CREATETIME < TO_DATE('{1}', 'YYYY-MM-dd HH24:mi:ss')
            //ORDER BY CREATETIME", starttime.ToString("yyyy-MM-dd HH:mm:ss"), endtime.ToString("yyyy-MM-dd HH:mm:ss"));
            #endregion

            string sql = string.Format(@"SELECT LINE_1 LINE, MONUMBER_1 MONUMBER, REELID_1 REELID, COMP_PN_1 PN, 
FLOOR((LASTUPDATETIME - CREATETIME) *24 *60 *60) AS CT, 
DECODE(TO_NUMBER(COMMANDTYPE) * 2 + TO_NUMBER(MATERIALSOURCE), 2,'人工入库',4,'机器入库',5,'人工入库',6,'人工出库',8,'机器出库',12,'人工出库', '未知') COMMANDNAME,
CREATETIME
FROM USI_AUTO_REEL.AR_A_COMMAND
WHERE CREATETIME >= TO_DATE('{0}', 'YYYY-MM-dd HH24:mi:ss')
AND CREATETIME < TO_DATE('{1}', 'YYYY-MM-dd HH24:mi:ss')
AND COMMANDTYPE IN ('1','2','3','4','6')
ORDER BY CREATETIME", starttime.ToString("yyyy-MM-dd HH:mm:ss"), endtime.ToString("yyyy-MM-dd HH:mm:ss"));
            var data = db.SqlQueryable<CTModel>(sql).ToList();

            if (!string.IsNullOrEmpty(line))
                data = data.Where(it => it.LINE.Contains(line)).ToList();

            if (!string.IsNullOrEmpty(orderno))
                data = data.Where(it => it.MONUMBER.Contains(orderno)).ToList();

            if (!string.IsNullOrEmpty(pn))
                data = data.Where(it => it.PN.Contains(pn)).ToList();

            return Json(new { data });
        }

        #region 旧的计算DownTime方法
        //        public JsonResult GetDownTimeData(string enddate)
        //        {
        //            var now = DateTime.Now;
        //            var endtime = string.IsNullOrEmpty(enddate) ? DateTime.Today.AddHours(5) : Convert.ToDateTime(enddate).AddHours(5);
        //            if (now > endtime)
        //                endtime = endtime.AddDays(1);
        //            var starttime = endtime.AddDays(-6);
        //            var db = DbFactory.GetSqlSugarClient();
        //            List<LineChartModel> chartdata = new List<LineChartModel>();
        //            for (int i = 0; starttime.AddDays(i) < endtime; i++)
        //            {
        //                var startday = starttime.AddDays(i);
        //                var endday = startday.AddDays(1);
        //                string sql = string.Format(@"SELECT STATUS, DATETIME, NVL(LEAD(DATETIME) OVER (ORDER BY DATETIME), TO_DATE('{1}', 'yyyy-mm-dd hh24:mi:ss')) AS NEXT_STSTUS_DATETIME
        //FROM (SELECT EQID, EQTYPE, STATUS, DATETIME, 
        //LAG(STATUS) OVER (PARTITION BY EQID ORDER BY DATETIME) AS PRIOR_STSTUS
        //FROM (SELECT * FROM EQUIPMENTSTATUS
        //WHERE EQID = 'EQSTK00002'
        //AND DATETIME >= to_date('{0}','yyyy-mm-dd hh24:mi:ss')
        //AND DATETIME <= to_date('{1}','yyyy-mm-dd hh24:mi:ss')
        //ORDER BY DATETIME)
        //)
        //WHERE STATUS <> PRIOR_STSTUS OR PRIOR_STSTUS IS NULL
        //ORDER BY DATETIME", startday.ToString("yyyy-MM-dd HH:mm:ss"), endday.ToString("yyyy-MM-dd HH:mm:ss"));
        //                var data = db.SqlQueryable<StatusDuration>(sql).ToList();

        //                LineChartModel model = new LineChartModel()
        //                {
        //                    DATE = startday.ToString("yy-MM-dd"),
        //                    RATE = 0
        //                };

        //                if (data.Count > 0)
        //                {
        //                    //如果日期是今天
        //                    if(endday > now)
        //                    {
        //                        data[data.Count - 1].NEXT_STSTUS_DATETIME = now;
        //                        //var totalWorkedTime = (now - startday).TotalMilliseconds - data.Where(it => it.STATUS == "Idle").Sum(it => it.DURATION) - data.Where(it => it.STATUS == "Offline").Sum(it => it.DURATION);
        //                        //if (totalWorkedTime > 0)
        //                        model.RATE = data.Where(it => it.STATUS == "Alarm").Sum(it => it.DURATION) / (now - startday).TotalMilliseconds * 100;
        //                    }
        //                    else
        //                    {
        //                        //var totalWorkedTime = 24 * 60 * 60 * 1000 - data.Where(it => it.STATUS == "Idle").Sum(it => it.DURATION) - data.Where(it => it.STATUS == "Offline").Sum(it => it.DURATION);
        //                        //if (totalWorkedTime > 0)
        //                        model.RATE = data.Where(it => it.STATUS == "Alarm").Sum(it => it.DURATION) / (24 * 60 * 60 * 10);
        //                    }

        //                }
        //                chartdata.Add(model);
        //            }
        //            return Json(new { data = chartdata });

        //        }
        #endregion

        public JsonResult GetDownTimeData(string enddate)
        {
            var now = DateTime.Now;
            var endtime = string.IsNullOrEmpty(enddate) ? DateTime.Today.AddHours(5) : Convert.ToDateTime(enddate).AddHours(5);
            if (now > endtime)
                endtime = endtime.AddDays(1);
            var starttime = endtime.AddDays(-6);
            var db = DbFactory.GetSqlSugarClient();
            string sql = string.Format(@"SELECT * FROM ALARMRESULTCAL
WHERE EQID = 'EQSTK00002'
AND NAME = 'downrate'
AND DATETIME >= TO_DATE('{0}', 'YYYY-MM-dd HH24:mi:ss')
AND DATETIME < TO_DATE('{1}', 'YYYY-MM-dd HH24:mi:ss')
ORDER BY DATETIME", starttime.ToString("yyyy-MM-dd HH:mm:ss"), endtime.ToString("yyyy-MM-dd HH:mm:ss"));
            var data = db.SqlQueryable<ALARMRESULTCAL>(sql).ToList();

            List<LineChartModel> chartdata = new List<LineChartModel>();
            for (int i = 0; starttime.AddDays(i) < endtime; i++)
            {
                var startday = starttime.AddDays(i);

                LineChartModel model = new LineChartModel()
                {
                    DATE = startday.ToString("yy-MM-dd"),
                    RATE = 0
                };

                var query = data.Where(it => it.DATETIME == startday).FirstOrDefault();
                if (query != null)
                {
                    model.RATE = Convert.ToDouble(query.VALUE);
                }
                chartdata.Add(model);
            }
            return Json(new { data = chartdata });

        }

        public JsonResult LoadingRateData(string enddate)
        {
            var now = DateTime.Now;
            var endtime = string.IsNullOrEmpty(enddate) ? DateTime.Today.AddHours(5) : Convert.ToDateTime(enddate).AddHours(5);
            if (now > endtime)
                endtime = endtime.AddDays(1);
            var starttime = endtime.AddDays(-6);
            var db = DbFactory.GetSqlSugarClient();

            List<LineChartModel> chartdata = new List<LineChartModel>();
            for (int i = 0; starttime.AddDays(i) < endtime; i++)
            {
                var startday = starttime.AddDays(i);
                var endday = startday.AddDays(1);
                string sql = string.Format(@"SELECT STATUS, DATETIME, NVL(LEAD(DATETIME) OVER (ORDER BY DATETIME), TO_DATE('{1}', 'yyyy-mm-dd hh24:mi:ss')) AS NEXT_STSTUS_DATETIME
FROM (SELECT EQID, EQTYPE, STATUS, DATETIME, 
LAG(STATUS) OVER (PARTITION BY EQID ORDER BY DATETIME) AS PRIOR_STSTUS
FROM (SELECT * FROM EQUIPMENTSTATUS
WHERE EQID = 'EQSTK00002'
AND DATETIME >= to_date('{0}','yyyy-mm-dd hh24:mi:ss')
AND DATETIME <= to_date('{1}','yyyy-mm-dd hh24:mi:ss')
ORDER BY DATETIME)
)
WHERE STATUS <> PRIOR_STSTUS OR PRIOR_STSTUS IS NULL
ORDER BY DATETIME", startday.ToString("yyyy-MM-dd HH:mm:ss"), endday.ToString("yyyy-MM-dd HH:mm:ss"));
                var data = db.SqlQueryable<StatusDuration>(sql).ToList();

                LineChartModel model = new LineChartModel()
                {
                    DATE = startday.ToString("yy-MM-dd"),
                    RATE = 0
                };

                if (data.Count > 0)
                {
                    //如果日期是今天，就是指结束时间比现在晚
                    if (endday > now)
                    {
                        data[data.Count - 1].NEXT_STSTUS_DATETIME = now;
                        model.RATE = data.Where(it => it.STATUS == "Run").Sum(it => it.DURATION) / (now - startday).TotalMilliseconds * 100;
                    }
                    else
                    {
                        model.RATE = data.Where(it => it.STATUS == "Run").Sum(it => it.DURATION) / (24 * 60 * 60 * 10);
                    }
                }
                chartdata.Add(model);
            }
            return Json(new { data = chartdata });
        }

        public JsonResult MtrlStockTableData(string line, string orderno, string pn)//实时的
        {
            var data = GetMaterialStockData(line, orderno, pn);
            return Json(new { data});
        }

        private List<MaterialStock> GetMaterialStockData(string line, string orderno, string pn)
        {
            var db_Ar = AutoReel_DbFactory.GetSqlSugarClient_AutoReel();
            #region 旧sql
            //            string sql_Ar = @"SELECT a.LINE, a.MONUMBER, a.COMPPN, a.TRAYQTY, a.MATERIALQTY, NVL(b.TARY_TYPE1, 0) MANUALINTARY, NVL(b.QTY_TYPE1, 0) MANUALINQTY, NVL(b.TARY_TYPE2, 0) MACHINEINTARY, NVL(b.QTY_TYPE2, 0) MACHINEINQTY, NVL(b.TARY_TYPE36, 0) MANUALOUTTARY, NVL(b.QTY_TYPE36, 0) MANUALOUTQTY, NVL(b.TARY_TYPE4, 0) MACHINEOUTTARY, NVL(b.QTY_TYPE4, 0) MACHINEOUTQTY FROM
            //(SELECT LINE, MONUMBER, COMPPN, COUNT(*) TRAYQTY, SUM(QTY) MATERIALQTY FROM AR_A_LOCATION_REEL
            //WHERE STATUS IN ('2', '3', '4', '5')
            //GROUP BY LINE, MONUMBER, COMPPN) a
            //LEFT JOIN
            //(SELECT LINE_1, MONUMBER_1, COMP_PN_1, SUM(CASE WHEN COMMANDTYPE = '1' THEN 1 ELSE 0 END) TARY_TYPE1, SUM(CASE WHEN COMMANDTYPE = '1' THEN TO_NUMBER(QTY_1) ELSE 0 END) QTY_TYPE1, SUM(CASE WHEN COMMANDTYPE = '2' THEN 1 ELSE 0 END) TARY_TYPE2, SUM(CASE WHEN COMMANDTYPE = '2' THEN TO_NUMBER(QTY_1) ELSE 0 END) QTY_TYPE2, SUM(CASE WHEN COMMANDTYPE in ('3', '6') THEN 1 ELSE 0 END) TARY_TYPE36, SUM(CASE WHEN COMMANDTYPE in ('3', '6') THEN TO_NUMBER(QTY_1) ELSE 0 END) QTY_TYPE36, SUM(CASE WHEN COMMANDTYPE = '4' THEN 1 ELSE 0 END) TARY_TYPE4, SUM(CASE WHEN COMMANDTYPE = '4' THEN TO_NUMBER(QTY_1) ELSE 0 END) QTY_TYPE4 FROM AR_A_COMMAND
            //WHERE QTY_1 IS NOT NULL AND STATUS = 2
            //GROUP BY LINE_1, MONUMBER_1, COMP_PN_1) b
            //ON a.LINE = b.LINE_1 AND a.MONUMBER = b.MONUMBER_1 AND a.COMPPN = b.COMP_PN_1";


            //            string sql_Ar = @"SELECT a.LINE, a.MONUMBER, a.COMPPN, a.TRAYQTY, a.MATERIALQTY, a.OCCUPIED_TRAY, a.OCCUPIED_QTY, NVL(b.TARY_TYPE1, 0) MANUALINTARY, NVL(b.QTY_TYPE1, 0) MANUALINQTY, NVL(b.TARY_TYPE2, 0) MACHINEINTARY, NVL(b.QTY_TYPE2, 0) MACHINEINQTY, NVL(b.TARY_TYPE36, 0) MANUALOUTTARY, NVL(b.QTY_TYPE36, 0) MANUALOUTQTY, NVL(b.TARY_TYPE4, 0) MACHINEOUTTARY, NVL(b.QTY_TYPE4, 0) MACHINEOUTQTY FROM
            //(SELECT LINE, MONUMBER, COMPPN, COUNT(*) TRAYQTY, SUM(QTY) MATERIALQTY, SUM(CASE WHEN STATUS = '3' THEN 1 ELSE 0 END) OCCUPIED_TRAY, SUM(CASE WHEN STATUS = '3' THEN TO_NUMBER(QTY) ELSE 0 END) OCCUPIED_QTY FROM AR_A_LOCATION_REEL
            //WHERE STATUS IN ('2', '3', '4', '5')
            //GROUP BY LINE, MONUMBER, COMPPN) a
            //LEFT JOIN
            //(SELECT LINE_1, MONUMBER_1, COMP_PN_1, SUM(CASE WHEN COMMANDTYPE = '1' THEN 1 ELSE 0 END) TARY_TYPE1, SUM(CASE WHEN COMMANDTYPE = '1' THEN TO_NUMBER(QTY_1) ELSE 0 END) QTY_TYPE1, SUM(CASE WHEN COMMANDTYPE = '2' THEN 1 ELSE 0 END) TARY_TYPE2, SUM(CASE WHEN COMMANDTYPE = '2' THEN TO_NUMBER(QTY_1) ELSE 0 END) QTY_TYPE2, SUM(CASE WHEN COMMANDTYPE in ('3', '6') THEN 1 ELSE 0 END) TARY_TYPE36, SUM(CASE WHEN COMMANDTYPE in ('3', '6') THEN TO_NUMBER(QTY_1) ELSE 0 END) QTY_TYPE36, SUM(CASE WHEN COMMANDTYPE = '4' THEN 1 ELSE 0 END) TARY_TYPE4, SUM(CASE WHEN COMMANDTYPE = '4' THEN TO_NUMBER(QTY_1) ELSE 0 END) QTY_TYPE4 FROM AR_A_COMMAND
            //WHERE QTY_1 IS NOT NULL AND STATUS = 2
            //GROUP BY LINE_1, MONUMBER_1, COMP_PN_1) b
            //ON a.LINE = b.LINE_1 AND a.MONUMBER = b.MONUMBER_1 AND a.COMPPN = b.COMP_PN_1";


            //            string sql_Ar = string.Format(@"SELECT b.LINE_1 LINE, b.MONUMBER_1 MONUMBER, b.COMP_PN_1 COMPPN, NVL(a.TRAYQTY, 0) TRAYQTY, NVL(a.MATERIALQTY, 0) MATERIALQTY, NVL(a.OCCUPIED_TRAY, 0) OCCUPIED_TRAY, NVL(a.OCCUPIED_QTY, 0) OCCUPIED_QTY, NVL(b.TARY_TYPE1, 0) MANUALINTARY, NVL(b.QTY_TYPE1, 0) MANUALINQTY, NVL(b.TARY_TYPE2, 0) MACHINEINTARY, NVL(b.QTY_TYPE2, 0) MACHINEINQTY, NVL(b.TARY_TYPE36, 0) MANUALOUTTARY, NVL(b.QTY_TYPE36, 0) MANUALOUTQTY, NVL(b.TARY_TYPE4, 0) MACHINEOUTTARY, NVL(b.QTY_TYPE4, 0) MACHINEOUTQTY FROM
            //(SELECT LINE, MONUMBER, COMPPN, COUNT(*) TRAYQTY, SUM(QTY) MATERIALQTY, SUM(CASE WHEN STATUS = '3' THEN 1 ELSE 0 END) OCCUPIED_TRAY, SUM(CASE WHEN STATUS = '3' THEN TO_NUMBER(QTY) ELSE 0 END) OCCUPIED_QTY FROM AR_A_LOCATION_REEL
            //WHERE STATUS IN ('2', '3', '4', '5')
            //GROUP BY LINE, MONUMBER, COMPPN) a
            //RIGHT JOIN
            //(SELECT LINE_1, MONUMBER_1, COMP_PN_1, SUM(CASE WHEN COMMANDTYPE = '1' THEN 1 ELSE 0 END) TARY_TYPE1, SUM(CASE WHEN COMMANDTYPE = '1' THEN TO_NUMBER(QTY_1) ELSE 0 END) QTY_TYPE1, SUM(CASE WHEN COMMANDTYPE = '2' THEN 1 ELSE 0 END) TARY_TYPE2, SUM(CASE WHEN COMMANDTYPE = '2' THEN TO_NUMBER(QTY_1) ELSE 0 END) QTY_TYPE2, SUM(CASE WHEN COMMANDTYPE in ('3', '6') THEN 1 ELSE 0 END) TARY_TYPE36, SUM(CASE WHEN COMMANDTYPE in ('3', '6') THEN TO_NUMBER(QTY_1) ELSE 0 END) QTY_TYPE36, SUM(CASE WHEN COMMANDTYPE = '4' THEN 1 ELSE 0 END) TARY_TYPE4, SUM(CASE WHEN COMMANDTYPE = '4' THEN TO_NUMBER(QTY_1) ELSE 0 END) QTY_TYPE4 FROM AR_A_COMMAND
            //WHERE QTY_1 IS NOT NULL AND STATUS = 2 
            //GROUP BY LINE_1, MONUMBER_1, COMP_PN_1) b
            //ON a.LINE = b.LINE_1 AND a.MONUMBER = b.MONUMBER_1 AND a.COMPPN = b.COMP_PN_1");
            #endregion

            string sql_Ar = @"SELECT b.LINE_1 LINE, b.MONUMBER_1 MONUMBER, b.COMP_PN_1 COMPPN, NVL(a.TRAYQTY, 0) TRAYQTY, NVL(a.MATERIALQTY, 0) MATERIALQTY, NVL(a.OCCUPIED_TRAY, 0) OCCUPIED_TRAY, NVL(a.OCCUPIED_QTY, 0) OCCUPIED_QTY, NVL(b.TARY_TYPE1, 0) + NVL(b.TARY_TYPE21, 0) MANUALINTARY, NVL(b.QTY_TYPE1, 0) + NVL(b.QTY_TYPE21, 0)  MANUALINQTY, NVL(b.TARY_TYPE20, 0) MACHINEINTARY, NVL(b.QTY_TYPE20, 0) MACHINEINQTY, NVL(b.TARY_TYPE36, 0) MANUALOUTTARY, NVL(b.QTY_TYPE36, 0) MANUALOUTQTY, NVL(b.TARY_TYPE4, 0) MACHINEOUTTARY, NVL(b.QTY_TYPE4, 0) MACHINEOUTQTY 
FROM
(SELECT LINE, MONUMBER, COMPPN, COUNT(*) TRAYQTY, SUM(QTY) MATERIALQTY, SUM(CASE WHEN STATUS = '3' THEN 1 ELSE 0 END) OCCUPIED_TRAY, SUM(CASE WHEN STATUS = '3' THEN TO_NUMBER(QTY) ELSE 0 END) OCCUPIED_QTY 
FROM AR_A_LOCATION_REEL
WHERE STATUS IN ('2', '3', '4', '5')
GROUP BY LINE, MONUMBER, COMPPN) a
RIGHT JOIN
(SELECT LINE_1, MONUMBER_1, COMP_PN_1, SUM(CASE WHEN COMMANDTYPE = '1' THEN 1 ELSE 0 END) TARY_TYPE1, SUM(CASE WHEN COMMANDTYPE = '1' THEN TO_NUMBER(QTY_1) ELSE 0 END) QTY_TYPE1, SUM(CASE WHEN COMMANDTYPE = '2' AND MATERIALSOURCE = '0' THEN 1 ELSE 0 END) TARY_TYPE20, SUM(CASE WHEN COMMANDTYPE = '2' AND MATERIALSOURCE = '0' THEN TO_NUMBER(QTY_1) ELSE 0 END) QTY_TYPE20, SUM(CASE WHEN COMMANDTYPE = '2' AND MATERIALSOURCE = '1' THEN 1 ELSE 0 END) TARY_TYPE21, SUM(CASE WHEN COMMANDTYPE = '2' AND MATERIALSOURCE = '1' THEN TO_NUMBER(QTY_1) ELSE 0 END) QTY_TYPE21, SUM(CASE WHEN COMMANDTYPE in ('3', '6') THEN 1 ELSE 0 END) TARY_TYPE36, SUM(CASE WHEN COMMANDTYPE in ('3', '6') THEN TO_NUMBER(QTY_1) ELSE 0 END) QTY_TYPE36, SUM(CASE WHEN COMMANDTYPE = '4' THEN 1 ELSE 0 END) TARY_TYPE4, SUM(CASE WHEN COMMANDTYPE = '4' THEN TO_NUMBER(QTY_1) ELSE 0 END) QTY_TYPE4 
FROM AR_A_COMMAND
WHERE QTY_1 IS NOT NULL AND STATUS = 2 
GROUP BY LINE_1, MONUMBER_1, COMP_PN_1) b
ON a.LINE = b.LINE_1 AND a.MONUMBER = b.MONUMBER_1 AND a.COMPPN = b.COMP_PN_1";
            var data_Ar = db_Ar.SqlQueryable<AutoReelStockModel>(sql_Ar).ToList();
            
            var data_Imcp = db_Ar.Queryable<IMCPStockModel>().ToList();

            var data_left = from a in data_Imcp
                            join b in data_Ar on new { orderno = a.ORDER, line = a.LINE, pn = a.PN } equals new { orderno = b.MONUMBER, line = b.LINE, pn = b.COMPPN }
                                       into result
                                       from c in result.DefaultIfEmpty()
                                       select new MaterialStock()
                                       {
                                           Line = a.LINE,
                                           OrderNo =a.ORDER,
                                           PN = a.PN,
                                           ReceivedQuantity = a.QTY_TYPE3,
                                           ReceivedTray = a.TARY_TYPE3,
                                           DeliveringQuantity = a.QTY_TYPE2,
                                           DeliveringTray = a.TARY_TYPE2,
                                           StockQuantity = a.QTY_TYPE1,
                                           StockTray = a.TARY_TYPE1,
                                           UsedQuantity = a.PVS_QTY,
                                           UsedTray = a.PVS_TRAY,
                                           SMTStockQuantity = c== null ? 0 : c.MATERIALQTY,
                                           SMTStockTray = c==null ? 0 : c.TRAYQTY,
                                           OccupiedQuantity = c==null ? 0 : c.OCCUPIED_QTY,
                                           OccupiedTray = c==null ? 0 : c.OCCUPIED_TRAY,
                                           AutoOutQuantity = c == null ? 0 : c.MACHINEOUTQTY,
                                           AutoOutTray = c == null ? 0 : c.MACHINEOUTTARY,
                                           ManualOutQuantity = c == null ? 0 : c.MANUALOUTQTY,
                                           ManualOutTray = c == null ? 0 : c.MANUALOUTTARY,
                                           AutoInQuantity = c == null ? 0 : c.MACHINEINQTY,
                                           AutoInTray = c == null ? 0 : c.MACHINEINTARY,
                                           ManualInQuantity = c == null ? 0 : c.MANUALINQTY,
                                           ManualInTray = c == null ? 0 : c.MANUALINTARY
                                       };


            var data_right = from a in data_Ar.Where(it=>it.TRAYQTY != 0).ToList()
                             join b in data_Imcp on new { orderno = a.MONUMBER, line = a.LINE, pn = a.COMPPN } equals new { orderno = b.ORDER, line = b.LINE, pn = b.PN }
                             into result
                             from c in result.DefaultIfEmpty()
                             where c is null
                             select new MaterialStock()
                             {
                                 Line = a.LINE,
                                 OrderNo = a.MONUMBER,
                                 PN = a.COMPPN,
                                 SMTStockQuantity = a.MATERIALQTY,
                                 SMTStockTray = a.TRAYQTY,
                                 OccupiedQuantity = a.OCCUPIED_QTY,
                                 OccupiedTray = a.OCCUPIED_TRAY,
                                 AutoOutQuantity = a.MACHINEOUTQTY,
                                 AutoOutTray = a.MACHINEOUTTARY,
                                 ManualOutQuantity = a.MANUALOUTQTY,
                                 ManualOutTray = a.MANUALOUTTARY,
                                 AutoInQuantity = a.MACHINEINQTY,
                                 AutoInTray = a.MACHINEINTARY,
                                 ManualInQuantity = a.MANUALINQTY,
                                 ManualInTray = a.MANUALINTARY
                             };

            List<MaterialStock> data = new List<MaterialStock>();
            data.AddRange(data_left.ToList());
            data.AddRange(data_right.ToList());

            //List<MaterialStock> data = data_Ar.Select(it => new MaterialStock()
            //{
            //    Line = it.LINE,
            //    OrderNo = it.MONUMBER,
            //    PN = it.COMPPN,
            //    SMTStockQuantity = it.MATERIALQTY,
            //    SMTStockTray = it.TRAYQTY,
            //    AutoOutQuantity = it.MACHINEOUTQTY,
            //    AutoOutTray = it.MACHINEOUTTARY,
            //    ManualOutQuantity = it.MANUALOUTQTY,
            //    ManualOutTray = it.MANUALOUTTARY,
            //    AutoInQuantity = it.MACHINEINQTY,
            //    AutoInTray = it.MACHINEINTARY,
            //    ManualInQuantity = it.MANUALINQTY,
            //    ManualInTray = it.MANUALINTARY
            //}).ToList();

            if (!string.IsNullOrEmpty(line))
                data = data.Where(it => it.Line.Contains(line)).ToList();

            if (!string.IsNullOrEmpty(orderno))
                data = data.Where(it => it.OrderNo.Contains(orderno)).ToList();

            if (!string.IsNullOrEmpty(pn))
                data = data.Where(it => it.PN.Contains(pn)).ToList();

            return data;
        }

        public JsonResult GetAlarmData(string enddate)
        {
            var now = DateTime.Now;
            var endtime = string.IsNullOrEmpty(enddate) ? DateTime.Today.AddHours(5) : Convert.ToDateTime(enddate).AddHours(5);
            if (now > endtime)
                endtime = endtime.AddDays(1);
            var starttime = endtime.AddDays(-1);
            var db = DbFactory.GetSqlSugarClient();
            string sql_alarm = string.Format(@"SELECT ALARMCODE,ALARMTEXT,STARTTIME AS ALARMTIME FROM ALARMRESULT
WHERE EQID = 'EQSTK00002'
AND ENDTIME IS NOT NULL
AND STARTTIME >= TO_DATE('{0}', 'YYYY-MM-dd HH24:mi:ss')
AND STARTTIME < TO_DATE('{1}', 'YYYY-MM-dd HH24:mi:ss')
ORDER BY STARTTIME DESC", starttime.ToString("yyyy-MM-dd HH:mm:ss"), endtime.ToString("yyyy-MM-dd HH:mm:ss"));
            var data = db.SqlQueryable<AlarmModel>(sql_alarm).ToList();
            return Json(new { data });
        }

        public FileResult DownloadReport(string startdate, string enddate, string line, string orderno, string pn)
        {
            var now = DateTime.Now;
            var endtime = string.IsNullOrEmpty(enddate) ? DateTime.Today.AddHours(5) : Convert.ToDateTime(enddate).AddHours(5);
            if (now > endtime)
                endtime = endtime.AddDays(1);

            var starttime_rate = endtime.AddDays(-6);
            var starttime_ct = endtime.AddDays(-1);
            if (!string.IsNullOrEmpty(startdate))
            {
                starttime_rate = Convert.ToDateTime(startdate).AddHours(5);
                starttime_ct = Convert.ToDateTime(startdate).AddHours(5);
            }

            var db_Ar = AutoReel_DbFactory.GetSqlSugarClient_AutoReel();
            string sql_trayquantity = string.Format(@"SELECT TO_CHAR(TRUNC(CREATETIME - INTERVAL '5' HOUR, 'DD'),'yyyy-mm-dd') as DAY, 
DECODE(COMMANDTYPE, '1','人工入库','2','机器入库','3','人工出库','4','机器出库','6','人工出库') COMMANDNAME, 
COUNT(*) COMMANDQTY
FROM AR_A_COMMAND
WHERE CREATETIME >= TO_DATE('{0}', 'YYYY-MM-dd HH24:mi:ss')
AND CREATETIME < TO_DATE('{1}', 'YYYY-MM-dd HH24:mi:ss')
GROUP BY TRUNC(CREATETIME - INTERVAL '5' HOUR, 'DD'),COMMANDTYPE
ORDER BY DAY", starttime_rate.ToString("yyyy-MM-dd HH:mm:ss"), endtime.ToString("yyyy-MM-dd HH:mm:ss"));
            var data_trayquantity = db_Ar.SqlQueryable<QuantityOfTray>(sql_trayquantity).ToList();

            string sql_ct = string.Format(@"SELECT LINE_1 LINE, MONUMBER_1 MONUMBER, REELID_1 REELID, COMP_PN_1 PN, 
FLOOR((LASTUPDATETIME - CREATETIME) *24 *60 *60) AS CT, 
DECODE(COMMANDTYPE, '1','人工入库','2','机器入库','3','人工出库','4','机器出库','6','人工出库') COMMANDNAME,
CREATETIME
FROM AR_A_COMMAND
WHERE CREATETIME >= TO_DATE('{0}', 'YYYY-MM-dd HH24:mi:ss')
AND CREATETIME < TO_DATE('{1}', 'YYYY-MM-dd HH24:mi:ss')
ORDER BY CREATETIME", starttime_ct.ToString("yyyy-MM-dd HH:mm:ss"), endtime.ToString("yyyy-MM-dd HH:mm:ss"));
            var data_ct = db_Ar.SqlQueryable<CTModel>(sql_ct).ToList();
            if (!string.IsNullOrEmpty(line))
                data_ct = data_ct.Where(it => it.LINE.Contains(line)).ToList();

            if (!string.IsNullOrEmpty(orderno))
                data_ct = data_ct.Where(it => it.MONUMBER.Contains(orderno)).ToList();
            if (!string.IsNullOrEmpty(pn))
                data_ct = data_ct.Where(it => it.PN.Contains(pn)).ToList();


            var db = DbFactory.GetSqlSugarClient();

            #region 旧设备状态统计
            //            List<List<StatusDuration>> data_status = new List<List<StatusDuration>>();
            //            List<LineChartModel> data_downtime = new List<LineChartModel>();
            //            List<LineChartModel> data_loadingrate = new List<LineChartModel>();
            //            for (int i = 0; starttime_rate.AddDays(i) < endtime; i++)
            //            {
            //                var startday = starttime_rate.AddDays(i);
            //                var endday = startday.AddDays(1);
            //                string sql = string.Format(@"SELECT STATUS, DATETIME, NVL(LEAD(DATETIME) OVER (ORDER BY DATETIME), TO_DATE('{1}', 'yyyy-mm-dd hh24:mi:ss')) AS NEXT_STSTUS_DATETIME
            //FROM (SELECT EQID, EQTYPE, STATUS, DATETIME, 
            //LAG(STATUS) OVER (PARTITION BY EQID ORDER BY DATETIME) AS PRIOR_STSTUS
            //FROM (SELECT * FROM EQUIPMENTSTATUS
            //WHERE EQID = 'EQSTK00002'
            //AND DATETIME >= to_date('{0}','yyyy-mm-dd hh24:mi:ss')
            //AND DATETIME <= to_date('{1}','yyyy-mm-dd hh24:mi:ss')
            //ORDER BY DATETIME)
            //)
            //WHERE STATUS <> PRIOR_STSTUS OR PRIOR_STSTUS IS NULL
            //ORDER BY DATETIME", startday.ToString("yyyy-MM-dd HH:mm:ss"), endday.ToString("yyyy-MM-dd HH:mm:ss"));
            //                var data = db.SqlQueryable<StatusDuration>(sql).ToList();

            //                LineChartModel model__downtime = new LineChartModel()
            //                {
            //                    DATE = startday.ToString("yy-MM-dd"),
            //                    RATE = 0
            //                };

            //                LineChartModel model__loadingrate = new LineChartModel()
            //                {
            //                    DATE = startday.ToString("yy-MM-dd"),
            //                    RATE = 0
            //                };

            //                if (data.Count > 0)
            //                {
            //                    //如果日期是今天，就是指结束时间比现在晚
            //                    if (endday > now)
            //                    {
            //                        data[data.Count - 1].NEXT_STSTUS_DATETIME = now;
            //                        model__downtime.RATE = data.Where(it => it.STATUS == "Alarm").Sum(it => it.DURATION) / (now - startday).TotalMilliseconds * 100;
            //                        model__loadingrate.RATE = data.Where(it => it.STATUS == "Run").Sum(it => it.DURATION) / (now - startday).TotalMilliseconds * 100;
            //                    }
            //                    else
            //                    {
            //                        model__downtime.RATE = data.Where(it => it.STATUS == "Alarm").Sum(it => it.DURATION) / (24 * 60 * 60 * 10);
            //                        model__loadingrate.RATE = data.Where(it => it.STATUS == "Run").Sum(it => it.DURATION) / (24 * 60 * 60 * 10);
            //                    }
            //                }
            //                data_status.Add(data);
            //                data_downtime.Add(model__downtime);
            //                data_loadingrate.Add(model__loadingrate);
            //            }
            #endregion

            //alarm data
//            string sql_alarm = string.Format(@"SELECT ALARMCODE,ALARMTEXT,STARTTIME,DURATION FROM ""ALARMRESULT""
//WHERE EQID = 'EQSTK00002'
//AND ENDTIME IS NOT NULL
//AND STARTTIME >= TO_DATE('{0}', 'YYYY-MM-dd HH24:mi:ss')
//AND STARTTIME < TO_DATE('{1}', 'YYYY-MM-dd HH24:mi:ss')
//ORDER BY STARTTIME", starttime_ct.ToString("yyyy-MM-dd HH:mm:ss"), endtime.ToString("yyyy-MM-dd HH:mm:ss"));
//            var data_alarm = db.SqlQueryable<ALARMRESULT>(sql_alarm).ToList();

            //StockTableData
            //var data_stock = GetMaterialStockData(line, orderno, pn);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage package = new ExcelPackage())
            {
                //ExcelWorksheet worksheet1 = package.Workbook.Worksheets.Add("工单物料数据(实时)");
                ExcelWorksheet worksheet2 = package.Workbook.Worksheets.Add("出入库盘数数据");
                ExcelWorksheet worksheet3 = package.Workbook.Worksheets.Add("CT数据");
                //ExcelWorksheet worksheet4 = package.Workbook.Worksheets.Add("Status数据");
                //ExcelWorksheet worksheet4 = package.Workbook.Worksheets.Add("Down Time数据");
                //ExcelWorksheet worksheet5 = package.Workbook.Worksheets.Add("Loading Rate数据");
                //ExcelWorksheet worksheet5 = package.Workbook.Worksheets.Add("Alarm原数据");
                //ExcelWorksheet worksheet6 = package.Workbook.Worksheets.Add("Alarm统计数据");

                worksheet2.Cells[1, 1].Value = "日期";
                worksheet2.Cells[1, 2].Value = "类型";
                worksheet2.Cells[1, 3].Value = "盘数";

                int i = 2;
                data_trayquantity.ForEach(it =>
                {
                    worksheet2.Cells[i, 1].Value = it.DAY;
                    worksheet2.Cells[i, 2].Value = it.COMMANDNAME;
                    worksheet2.Cells[i, 3].Value = it.COMMANDQTY;
                    i++;
                });

                worksheet3.Cells[1, 1].Value = "ReelId";
                worksheet3.Cells[1, 2].Value = "PN";
                worksheet3.Cells[1, 3].Value = "类型";
                worksheet3.Cells[1, 4].Value = "CT";
                worksheet3.Cells[1, 5].Value = "时间";

                int j = 2;
                data_ct.ForEach(it => 
                {
                    worksheet3.Cells[j, 1].Value = it.LINE;
                    worksheet3.Cells[j, 2].Value = it.MONUMBER;
                    worksheet3.Cells[j, 3].Value = it.REELID;
                    worksheet3.Cells[j, 4].Value = it.PN;
                    worksheet3.Cells[j, 5].Value = it.COMMANDNAME;
                    worksheet3.Cells[j, 6].Value = it.CT;
                    worksheet3.Cells[j, 7].Value = it.DATESTR;
                    j++;
                });

                //worksheet4.Cells[1, 1].Value = "状态";
                //worksheet4.Cells[1, 2].Value = "开始时间";
                //worksheet4.Cells[1, 3].Value = "结束时间";
                //worksheet4.Cells[1, 4].Value = "持续时间(秒)";
                //int k = 2;
                //data_status.ForEach(list =>
                //{
                //    list.ForEach(it => 
                //    {
                //        worksheet4.Cells[k, 1].Value = it.STATUS;
                //        worksheet4.Cells[k, 2].Value = it.DATETIME.ToString("yyyy-MM-dd yyyy-MM-dd HH:mm:ss");
                //        worksheet4.Cells[k, 3].Value = it.NEXT_STSTUS_DATETIME.ToString("yyyy-MM-dd yyyy-MM-dd HH:mm:ss");
                //        worksheet4.Cells[k, 4].Value = Math.Round(it.DURATION / 1000);
                //        k++;
                //    });
                //});

                using (MemoryStream ms = new MemoryStream())
                {
                    package.SaveAs(ms);
                    var result = ms.ToArray();
                    ms.Dispose();
                    var filename = $"{startdate.Replace("-", "")}-{enddate.Replace("-", "")}CT相关统计报表.xlsx";
                    return File(result, "application/mx-excel", filename);
                }

            }

        }

        public FileResult DownloadAlarmReport(string startdate, string enddate)
        {
            var now = DateTime.Now;
            var endtime = string.IsNullOrEmpty(enddate) ? DateTime.Today.AddHours(5) : Convert.ToDateTime(enddate).AddHours(5);
            if (now > endtime)
                endtime = endtime.AddDays(1);

            var starttime_rate = endtime.AddDays(-6);
            var starttime_ct = endtime.AddDays(-1);
            if (!string.IsNullOrEmpty(startdate))
            {
                starttime_rate = Convert.ToDateTime(startdate).AddHours(5);
                starttime_ct = Convert.ToDateTime(startdate).AddHours(5);
            }

            var db = DbFactory.GetSqlSugarClient();

            //downtimerate
            string sql_downtime = string.Format(@"SELECT TO_CHAR(DATETIME, 'YYYY-MM-dd') DATESTR, VALUE RATE FROM ALARMRESULTCAL
WHERE EQID = 'EQSTK00002'
AND NAME = 'downrate'
AND DATETIME >= TO_DATE('{0}', 'YYYY-MM-dd HH24:mi:ss')
AND DATETIME < TO_DATE('{1}', 'YYYY-MM-dd HH24:mi:ss')
ORDER BY DATETIME", starttime_rate.ToString("yyyy-MM-dd HH:mm:ss"), endtime.ToString("yyyy-MM-dd HH:mm:ss"));
            var data_downtime = db.SqlQueryable<DownTimeRateModel>(sql_downtime).ToList();

            //alarm data
            string sql_alarm = string.Format(@"SELECT ALARMCODE,ALARMTEXT,STARTTIME,DURATION FROM ALARMRESULT
WHERE EQID = 'EQSTK00002'
AND ENDTIME IS NOT NULL
AND STARTTIME >= TO_DATE('{0}', 'YYYY-MM-dd HH24:mi:ss')
AND STARTTIME < TO_DATE('{1}', 'YYYY-MM-dd HH24:mi:ss')
ORDER BY STARTTIME", starttime_ct.ToString("yyyy-MM-dd HH:mm:ss"), endtime.ToString("yyyy-MM-dd HH:mm:ss"));
            var data_alarm = db.SqlQueryable<ALARMRESULT>(sql_alarm).ToList();

            //loading rate--还没更新
            List<LineChartModel> chartdata = new List<LineChartModel>();
            for (int i = 0; starttime_rate.AddDays(i) < endtime; i++)
            {
                var startday = starttime_rate.AddDays(i);
                var endday = startday.AddDays(1);
                string sql = string.Format(@"SELECT STATUS, DATETIME, NVL(LEAD(DATETIME) OVER (ORDER BY DATETIME), TO_DATE('{1}', 'yyyy-mm-dd hh24:mi:ss')) AS NEXT_STSTUS_DATETIME
FROM (SELECT EQID, EQTYPE, STATUS, DATETIME, 
LAG(STATUS) OVER (PARTITION BY EQID ORDER BY DATETIME) AS PRIOR_STSTUS
FROM (SELECT * FROM EQUIPMENTSTATUS
WHERE EQID = 'EQSTK00002'
AND DATETIME >= to_date('{0}','yyyy-mm-dd hh24:mi:ss')
AND DATETIME <= to_date('{1}','yyyy-mm-dd hh24:mi:ss')
ORDER BY DATETIME)
)
WHERE STATUS <> PRIOR_STSTUS OR PRIOR_STSTUS IS NULL
ORDER BY DATETIME", startday.ToString("yyyy-MM-dd HH:mm:ss"), endday.ToString("yyyy-MM-dd HH:mm:ss"));
                var data = db.SqlQueryable<StatusDuration>(sql).ToList();

                LineChartModel model = new LineChartModel()
                {
                    DATE = startday.ToString("yyyy-MM-dd"),
                    RATE = 0
                };

                if (data.Count > 0)
                {
                    //如果日期是今天，就是指结束时间比现在晚
                    if (endday > now)
                    {
                        data[data.Count - 1].NEXT_STSTUS_DATETIME = now;
                        model.RATE = data.Where(it => it.STATUS == "Run").Sum(it => it.DURATION) / (now - startday).TotalMilliseconds * 100;
                    }
                    else
                    {
                        model.RATE = data.Where(it => it.STATUS == "Run").Sum(it => it.DURATION) / (24 * 60 * 60 * 10);
                    }
                }
                chartdata.Add(model);
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet worksheet3 = package.Workbook.Worksheets.Add("Loading Rate数据");
                ExcelWorksheet worksheet4 = package.Workbook.Worksheets.Add("DownTime Rate数据");
                ExcelWorksheet worksheet5 = package.Workbook.Worksheets.Add("Alarm原数据");
                ExcelWorksheet worksheet6 = package.Workbook.Worksheets.Add("Alarm统计数据");

                //Loading数据
                worksheet3.Cells[1, 1].Value = "日期";
                worksheet3.Cells[1, 2].Value = "Loading Rate(%)";
                int l = 2;
                chartdata.ForEach(it =>
                {
                    worksheet3.Cells[l, 1].Value = it.DATE;
                    worksheet3.Cells[l, 2].Value = it.RATE;
                    l++;
                });


                //downtime数据
                worksheet4.Cells[1, 1].Value = "日期";
                worksheet4.Cells[1, 2].Value = "DownTime Rate(%)";
                int a = 2;
                data_downtime.ForEach(it =>
                {
                    worksheet4.Cells[a, 1].Value = it.DATESTR;
                    worksheet4.Cells[a, 2].Value = it.RATE;
                    a++;
                });

                //alarm数据统计
                worksheet5.Cells[1, 1].Value = "Alarm Code";
                worksheet5.Cells[1, 2].Value = "Alarm Text";
                worksheet5.Cells[1, 3].Value = "Alarm Time";
                worksheet5.Cells[1, 4].Value = "Duration(sec)";
                int n = 2;
                data_alarm.ForEach(it =>
                {
                    worksheet5.Cells[n, 1].Value = it.ALARMCODE;
                    worksheet5.Cells[n, 2].Value = it.ALARMTEXT;
                    worksheet5.Cells[n, 3].Value = it.DATESTR;
                    worksheet5.Cells[n, 4].Value = it.DURATION;
                    n++;
                });

                var data_statistic = data_alarm.GroupBy(it => new { it.ALARMCODE, it.ALARMTEXT })
                    .Select(group => new
                    {
                        group.Key.ALARMCODE,
                        group.Key.ALARMTEXT,
                        Count = group.Count(),
                        Duration = group.Sum(it => it.DURATION)
                    }).OrderByDescending(it => it.Count).ToList();

                worksheet6.Cells[1, 1].Value = "Alarm Code";
                worksheet6.Cells[1, 2].Value = "Alarm Text";
                worksheet6.Cells[1, 3].Value = "Count";
                worksheet6.Cells[1, 4].Value = "Duration(sec)";

                int m = 2;
                data_statistic.ForEach(it =>
                {
                    worksheet6.Cells[m, 1].Value = it.ALARMCODE;
                    worksheet6.Cells[m, 2].Value = it.ALARMTEXT;
                    worksheet6.Cells[m, 3].Value = it.Count;
                    worksheet6.Cells[m, 4].Value = it.Duration;
                    m++;
                });

                ExcelBarChart chart = worksheet6.Drawings.AddChart("chart1", eChartType.ColumnClustered) as ExcelBarChart;
                chart.SetPosition(0, 0, 6, 0);
                chart.SetSize(800, 800);//设置大小
                // 设置图表标题
                chart.Title.Text = "Alarm 统计";
                chart.Title.Font.Size = 14;//标题的大小
                chart.Title.Font.Bold = true;//标题的粗体
                var series1 = chart.Series.Add(worksheet6.Cells[2, 3, m - 1, 3], worksheet6.Cells[2, 2, m - 1, 2]);
                series1.Header = "Alarm Count";
                //var series2 = chart.Series.Add(worksheet6.Cells[2, 4, m - 1, 4], worksheet6.Cells[2, 2, m - 1, 2]);
                //series2.Header = "Alarm Duration";
                chart.Legend.Position = eLegendPosition.Top;
                chart.DataLabel.ShowValue = true;

                using (MemoryStream ms = new MemoryStream())
                {
                    package.SaveAs(ms);
                    var result = ms.ToArray();
                    ms.Dispose();
                    var filename = $"{startdate.Replace("-", "")}-{enddate.Replace("-", "")}报警相关统计报表.xlsx";
                    return File(result, "application/mx-excel", filename);
                }

            }

        }

        public FileResult DownloadItemsData(string line, string orderno, string pn)
        {
            //StockTableData
            var data_stock = GetMaterialStockData(line, orderno, pn);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet worksheet1 = package.Workbook.Worksheets.Add("工单物料数据(实时)");

                //StockTableData
                worksheet1.Cells["A1:A2"].Merge = true;
                worksheet1.Cells[1, 1].Value = "线体";
                worksheet1.Cells["B1:B2"].Merge = true;
                worksheet1.Cells[1, 2].Value = "工单号";
                worksheet1.Cells["C1:C2"].Merge = true;
                worksheet1.Cells[1, 3].Value = "料号";
                worksheet1.Cells["D1:E1"].Merge = true;
                worksheet1.Cells[1, 4].Value = "工单";
                worksheet1.Cells[2, 4].Value = "总数量";
                worksheet1.Cells[2, 5].Value = "总盘数";
                worksheet1.Cells["F1:G1"].Merge = true;
                worksheet1.Cells[1, 6].Value = "已扫PVS";
                worksheet1.Cells[2, 6].Value = "数量";
                worksheet1.Cells[2, 7].Value = "盘数";
                worksheet1.Cells["H1:I1"].Merge = true;
                worksheet1.Cells[1, 8].Value = "已接收";
                worksheet1.Cells[2, 8].Value = "数量";
                worksheet1.Cells[2, 9].Value = "盘数";
                worksheet1.Cells["J1:K1"].Merge = true;
                worksheet1.Cells[1, 10].Value = "运送中";
                worksheet1.Cells[2, 10].Value = "数量";
                worksheet1.Cells[2, 11].Value = "盘数";
                worksheet1.Cells["L1:M1"].Merge = true;
                worksheet1.Cells[1, 12].Value = "方仓库存";
                worksheet1.Cells[2, 12].Value = "数量";
                worksheet1.Cells[2, 13].Value = "盘数";
                worksheet1.Cells["N1:O1"].Merge = true;
                worksheet1.Cells[1, 14].Value = "线边仓库存";
                worksheet1.Cells[2, 14].Value = "数量";
                worksheet1.Cells[2, 15].Value = "盘数";
                worksheet1.Cells["P1:Q1"].Merge = true;
                worksheet1.Cells[1, 16].Value = "叫料占用";
                worksheet1.Cells[2, 16].Value = "数量";
                worksheet1.Cells[2, 17].Value = "盘数";
                worksheet1.Cells["R1:S1"].Merge = true;
                worksheet1.Cells[1, 18].Value = "机器出库";
                worksheet1.Cells[2, 18].Value = "数量";
                worksheet1.Cells[2, 19].Value = "盘数";
                worksheet1.Cells["T1:U1"].Merge = true;
                worksheet1.Cells[1, 20].Value = "人工出库";
                worksheet1.Cells[2, 20].Value = "数量";
                worksheet1.Cells[2, 21].Value = "盘数";
                worksheet1.Cells["V1:W1"].Merge = true;
                worksheet1.Cells[1, 22].Value = "机器入库";
                worksheet1.Cells[2, 22].Value = "数量";
                worksheet1.Cells[2, 23].Value = "盘数";
                worksheet1.Cells["X1:Y1"].Merge = true;
                worksheet1.Cells[1, 24].Value = "人工入库";
                worksheet1.Cells[2, 24].Value = "数量";
                worksheet1.Cells[2, 25].Value = "盘数";

                int a = 3;
                data_stock.ForEach(it =>
                {
                    worksheet1.Cells[a, 1].Value = it.Line;
                    worksheet1.Cells[a, 2].Value = it.OrderNo;
                    worksheet1.Cells[a, 3].Value = it.PN;
                    worksheet1.Cells[a, 4].Value = it.QuantityInOrder;
                    worksheet1.Cells[a, 5].Value = it.TrayInOrder;
                    worksheet1.Cells[a, 6].Value = it.UsedQuantity;
                    worksheet1.Cells[a, 7].Value = it.UsedTray;
                    worksheet1.Cells[a, 8].Value = it.ReceivedQuantity;
                    worksheet1.Cells[a, 9].Value = it.ReceivedTray;
                    worksheet1.Cells[a, 10].Value = it.DeliveringQuantity;
                    worksheet1.Cells[a, 11].Value = it.DeliveringTray;
                    worksheet1.Cells[a, 12].Value = it.StockQuantity;
                    worksheet1.Cells[a, 13].Value = it.StockTray;
                    worksheet1.Cells[a, 14].Value = it.SMTStockQuantity;
                    worksheet1.Cells[a, 15].Value = it.SMTStockTray;
                    worksheet1.Cells[a, 16].Value = it.OccupiedQuantity;
                    worksheet1.Cells[a, 17].Value = it.OccupiedTray;
                    worksheet1.Cells[a, 18].Value = it.AutoOutQuantity;
                    worksheet1.Cells[a, 19].Value = it.AutoOutTray;
                    worksheet1.Cells[a, 20].Value = it.ManualOutQuantity;
                    worksheet1.Cells[a, 21].Value = it.ManualOutTray;
                    worksheet1.Cells[a, 22].Value = it.AutoInQuantity;
                    worksheet1.Cells[a, 23].Value = it.AutoInTray;
                    worksheet1.Cells[a, 24].Value = it.ManualInQuantity;
                    worksheet1.Cells[a, 25].Value = it.ManualInTray;
                    a++;
                });

                using (MemoryStream ms = new MemoryStream())
                {
                    package.SaveAs(ms);
                    var result = ms.ToArray();
                    ms.Dispose();
                    var filename = $"物料库存报表.xlsx";
                    return File(result, "application/mx-excel", filename);
                }
            }
        }

        public FileResult DownloadHistoryReport(string date, string hour, string minute, string line, string orderno, string pn)
        {
            int hour_int = Convert.ToInt32(hour);
            int minute_int = Convert.ToInt32(minute);
            DateTime dateTime = Convert.ToDateTime(date).AddHours(hour_int).AddMinutes(minute_int);
            DateTime startTime = dateTime.AddMinutes(-5);
            DateTime endTime = dateTime.AddMinutes(5);
            //查到最近的历史数据
            var db_Ar = AutoReel_DbFactory.GetSqlSugarClient_AutoReel();
            var data_stock = db_Ar.Queryable<MaterialStockHistory>().Where(it => it.CreateTime > startTime && it.CreateTime < endTime).ToList();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet worksheet1 = package.Workbook.Worksheets.Add("工单物料数据");

                //StockTableData
                worksheet1.Cells["A1:A2"].Merge = true;
                worksheet1.Cells[1, 1].Value = "线体";
                worksheet1.Cells["B1:B2"].Merge = true;
                worksheet1.Cells[1, 2].Value = "工单号";
                worksheet1.Cells["C1:C2"].Merge = true;
                worksheet1.Cells[1, 3].Value = "料号";
                worksheet1.Cells["D1:E1"].Merge = true;
                worksheet1.Cells[1, 4].Value = "工单";
                worksheet1.Cells[2, 4].Value = "总数量";
                worksheet1.Cells[2, 5].Value = "总盘数";
                worksheet1.Cells["F1:G1"].Merge = true;
                worksheet1.Cells[1, 6].Value = "已扫PVS";
                worksheet1.Cells[2, 6].Value = "数量";
                worksheet1.Cells[2, 7].Value = "盘数";
                worksheet1.Cells["H1:I1"].Merge = true;
                worksheet1.Cells[1, 8].Value = "已接收";
                worksheet1.Cells[2, 8].Value = "数量";
                worksheet1.Cells[2, 9].Value = "盘数";
                worksheet1.Cells["J1:K1"].Merge = true;
                worksheet1.Cells[1, 10].Value = "运送中";
                worksheet1.Cells[2, 10].Value = "数量";
                worksheet1.Cells[2, 11].Value = "盘数";
                worksheet1.Cells["L1:M1"].Merge = true;
                worksheet1.Cells[1, 12].Value = "方仓库存";
                worksheet1.Cells[2, 12].Value = "数量";
                worksheet1.Cells[2, 13].Value = "盘数";
                worksheet1.Cells["N1:O1"].Merge = true;
                worksheet1.Cells[1, 14].Value = "线边仓库存";
                worksheet1.Cells[2, 14].Value = "数量";
                worksheet1.Cells[2, 15].Value = "盘数";
                worksheet1.Cells["P1:Q1"].Merge = true;
                worksheet1.Cells[1, 16].Value = "叫料占用";
                worksheet1.Cells[2, 16].Value = "数量";
                worksheet1.Cells[2, 17].Value = "盘数";
                worksheet1.Cells["R1:S1"].Merge = true;
                worksheet1.Cells[1, 18].Value = "机器出库";
                worksheet1.Cells[2, 18].Value = "数量";
                worksheet1.Cells[2, 19].Value = "盘数";
                worksheet1.Cells["T1:U1"].Merge = true;
                worksheet1.Cells[1, 20].Value = "人工出库";
                worksheet1.Cells[2, 20].Value = "数量";
                worksheet1.Cells[2, 21].Value = "盘数";
                worksheet1.Cells["V1:W1"].Merge = true;
                worksheet1.Cells[1, 22].Value = "机器入库";
                worksheet1.Cells[2, 22].Value = "数量";
                worksheet1.Cells[2, 23].Value = "盘数";
                worksheet1.Cells["X1:Y1"].Merge = true;
                worksheet1.Cells[1, 24].Value = "人工入库";
                worksheet1.Cells[2, 24].Value = "数量";
                worksheet1.Cells[2, 25].Value = "盘数";

                int a = 3;
                data_stock.ForEach(it =>
                {
                    worksheet1.Cells[a, 1].Value = it.Line;
                    worksheet1.Cells[a, 2].Value = it.OrderNo;
                    worksheet1.Cells[a, 3].Value = it.PN;
                    worksheet1.Cells[a, 4].Value = it.QuantityInOrder;
                    worksheet1.Cells[a, 5].Value = it.TrayInOrder;
                    worksheet1.Cells[a, 6].Value = it.UsedQuantity;
                    worksheet1.Cells[a, 7].Value = it.UsedTray;
                    worksheet1.Cells[a, 8].Value = it.ReceivedQuantity;
                    worksheet1.Cells[a, 9].Value = it.ReceivedTray;
                    worksheet1.Cells[a, 10].Value = it.DeliveringQuantity;
                    worksheet1.Cells[a, 11].Value = it.DeliveringTray;
                    worksheet1.Cells[a, 12].Value = it.StockQuantity;
                    worksheet1.Cells[a, 13].Value = it.StockTray;
                    worksheet1.Cells[a, 14].Value = it.SMTStockQuantity;
                    worksheet1.Cells[a, 15].Value = it.SMTStockTray;
                    worksheet1.Cells[a, 16].Value = it.OccupiedQuantity;
                    worksheet1.Cells[a, 17].Value = it.OccupiedTray;
                    worksheet1.Cells[a, 18].Value = it.AutoOutQuantity;
                    worksheet1.Cells[a, 19].Value = it.AutoOutTray;
                    worksheet1.Cells[a, 20].Value = it.ManualOutQuantity;
                    worksheet1.Cells[a, 21].Value = it.ManualOutTray;
                    worksheet1.Cells[a, 22].Value = it.AutoInQuantity;
                    worksheet1.Cells[a, 23].Value = it.AutoInTray;
                    worksheet1.Cells[a, 24].Value = it.ManualInQuantity;
                    worksheet1.Cells[a, 25].Value = it.ManualInTray;
                    a++;
                });

                using (MemoryStream ms = new MemoryStream())
                {
                    package.SaveAs(ms);
                    var result = ms.ToArray();
                    ms.Dispose();
                    var filename = $"{date.Replace("-","")}{hour.PadLeft(2, '0')}{minute.PadLeft(2, '0')}物料库存报表.xlsx";
                    return File(result, "application/mx-excel", filename);
                }
            }
        }
    }

    public class AutoReel_DbFactory
    {
        public static SqlSugarClient GetSqlSugarClient_AutoReel()
        {
            var db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = ConfigurationManager.ConnectionStrings["AutoReel_DB"].ToString(),
                DbType = DbType.Oracle,
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute,//从实体特性中读取主键自增列信息
                MoreSettings = new ConnMoreSettings()
                {
                    IsAutoToUpper = false // 是否转大写，默认是转大写的可以禁止转大写
                }
            });
            return db;
        }

        //public static SqlSugarClient GetSqlSugarClient_IMCP()
        //{
        //    var db = new SqlSugarClient(new ConnectionConfig()
        //    {
        //        ConnectionString = "server=192.168.53.249;database=imcp;uid=root;pwd=haishen&%$2111",//10.5.1.249
        //        DbType = DbType.MySql,
        //        IsAutoCloseConnection = true,
        //        InitKeyType = InitKeyType.Attribute//从实体特性中读取主键自增列信息
        //    });
        //    return db;
        //}
    }
}