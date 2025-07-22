using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using EAP.Dashboard.Models;
using EAP.Models.Database;
using EAP.Dashboard.Utils;
using System.Collections;

namespace EAP.Dashboard.Controllers
{
    public class BaseController : Controller
    {
        protected string roleCode => Session["RoleCode"] as string;

        protected List<string> controllers => Session["controllers"] as List<string>;

        protected string site = ConfigurationManager.AppSettings["site"].ToString();

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


        public JsonResult GetHistoryAlarmRate(string eqpType)
        {
            var db = DbFactory.GetSqlSugarClient();
            var eqptypeids = db.Queryable<EquipmentType>().Where(it => it.Name == eqpType).Select(it => it.ID).ToList();
            var eqpidids = db.Queryable<Equipment>().Where(it => eqptypeids.Contains(it.EquipmentTypeId)).OrderBy(it => it.Sort).Select(it => it.EQID).ToList();
            var weeks = db.Queryable<EquipmentParamsHistoryCalculate>()
                .Where(it => it.EQID == eqpType && it.Remark != null && it.Name == "starttime")
                .Select(it => it.Remark)
                .Distinct()
                .ToList()
                .OrderByDescending(it => Convert.ToDouble(it.Replace("W", ""))).Take(8)
                .ToList()
                .OrderBy(it => Convert.ToDouble(it.Replace("W", ""))).ToList();
            string weekOption = "";

            foreach (var weekItem in weeks)
            {
                weekOption += "<option value=\"" + weekItem + "\" >" + weekItem + "</option>";
                //checkbox += $"<input type=\"checkbox\" name=\"like[{item}]\" title=\"{item.Replace("EQAMS0000", string.Empty)}号机\">" ;
            }
            //.OrderBy(it => Convert.ToDouble(it.Split("W")[1])).TakeLast(8);
            var target = db.Queryable<EquipmentTypeConfiguration>().Where(it => it.TypeNameId == eqptypeids.First() && it.ConfigurationItem == "AlarmRateTarget").Select(it => it.ConfigurationValue).First();

            var totalrate = new List<double>();
            var totaltime = new List<double>();
            var totalmfg = new List<double>();
            List<EquipmentAlarmTimes> alarmArr = DashBoardService.GetAlarmTimeList(eqpType);
            List<EquipmentAlarmTimes> alarmMonArr = DashBoardService.GetAlarmMonthlyList(alarmArr);
            List<EquipmentAlarmTimes> alarmWkArr = alarmArr.Count > 8 ? alarmArr.GetRange(alarmArr.Count - 8, 8) : alarmArr;

            //alarmArr = alarmArr.GetRange(alarmArr.Count - 8, 8);
            double maxtime = 0;
            double maxrate = 0;


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


                //var alarmrates = new List<double>();
                var alarmtimes = db.Queryable<EquipmentParamsHistoryCalculate>()
                    .Where(it => eqpidids.Contains(it.EQID) && it.Remark == item && it.Name == "alarmspan").Select(it => Convert.ToDouble(it.Value)).ToList();

                double alarmtime = 0;


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

            return Json(new { alarmWkArr, alarmMonArr, weekdata = weeks, weekOption, totaltimes = totaltime, totalrates = totalrate, maxtime = maxtime, maxrate = maxrate, totalmfg = totalmfg, target });

        }
        public JsonResult SetStartEnd(DateTime starttime, DateTime endtime, int idleduration, string eqpType)
        {
            //User set the start time and end time firstly.
            var db = DbFactory.GetSqlSugarClient();
            var eqtype = db.Queryable<EquipmentType>().Where(it => it.Name == eqpType).Select(it => it.ID).ToList();
            var eqpidids = db.Queryable<Equipment>().Where(it => eqtype.Contains(it.EquipmentTypeId)).OrderBy(it => it.Sort).ToList();
            GregorianCalendar gregorianCalendar = new GregorianCalendar();

            int week = gregorianCalendar.GetWeekOfYear(starttime, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            string w = starttime.Year.ToString() + "W" + week.ToString().PadLeft(2, '0');

            //string w = "W" + week.ToString();
            string sqlstart = String.Format(@"insert into EQUIPMENTPARAMSHISCAL(EQID,NAME,VALUE,UPDATETIME,REMARK) values('{0}','{1}','{2}',to_timestamp('{3}','yyyy-mm-dd hh24:mi:ss'),'{4}')", eqpType, "starttime", starttime.ToString("yyyy-MM-dd HH:mm"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), w);
            string sqlend = String.Format(@"insert into EQUIPMENTPARAMSHISCAL(EQID,NAME,VALUE,UPDATETIME,REMARK) values('{0}','{1}','{2}',to_timestamp('{3}','yyyy-mm-dd hh24:mi:ss'),'{4}')", eqpType, "endtime", endtime.ToString("yyyy-MM-dd HH:mm"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), w);
            string sqlduration = String.Format(@"insert into EQUIPMENTPARAMSHISCAL(EQID,NAME,VALUE,UPDATETIME,REMARK) values('{0}','{1}','{2}',to_timestamp('{3}','yyyy-mm-dd hh24:mi:ss'),'{4}')", eqpType, "idleduration", idleduration, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), w);


            db.Ado.ExecuteCommand(sqlstart);
            db.Ado.ExecuteCommand(sqlend);
            db.Ado.ExecuteCommand(sqlduration);
            //Post Date to function and calculate the AlarmRate
            if (DashBoardService.CalculateAlarmRate(eqpidids, starttime, endtime, w, idleduration))
            {
                JsonResult result = GetAlarmRate(w, eqpType);
                return result;
            }
            else
            {
                return Json(new { });
            }
            //JsonResult result = CalculateAlarmRate(eqpidids, starttime, endtime, w, idleduration);



        }

        public JsonResult GetAlarmDetails(string EQID, string week, string eqpType)
        {
            var db = DbFactory.GetSqlSugarClient();
            var alarmList = new List<AlarmItem>();

            var startendList = db.Queryable<EquipmentParamsHistoryCalculate>()
               .Where(it => (it.Name == "starttime" || it.Name == "endtime")
               && it.Remark == week
               && it.EQID == eqpType).ToList();
            // 找出每个 week 的 starttime 和 endtime 数据
            var startTimes = startendList.Where(r => r.Name == "starttime").OrderBy(r => Convert.ToDateTime(r.Value)).ToList();
            var endTimes = startendList.Where(r => r.Name == "endtime").OrderBy(r => Convert.ToDateTime(r.Value)).ToList();

            // 将每对 starttime 和 endtime 配对，并进行处理
            for (int i = 0; i < startTimes.Count; i++)
            {
                DateTime start = Convert.ToDateTime(startTimes[i].Value);
                DateTime end = Convert.ToDateTime(endTimes[i].Value);// : start.AddDays(1).AddSeconds(-1); // 如果没有endtime，默认结束为第二天的23:59:59

                var periodData = DashBoardService.GetAlarmDetails(start, end, EQID,9);
                alarmList.AddRange(periodData);
            }
          

            foreach (var item in alarmList)
            {
                item.AlarmText = item.AlarmText.Split(';').Length == 3 ? item.AlarmText.Split(';')[2] : item.AlarmText;
            }

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

            var totalalarm = new { totalcodelist, totalcodetext, totalalarmtime, totalalarmpie };
            return Json(new { alarmList = alarmList.OrderBy(it => Convert.ToDateTime(it.AlarmDate)), alarmtotal = totalalarm });
        }
        public JsonResult GetAlarmDetailsOld(string EQID, string week, string eqpType)
        {
            var db = DbFactory.GetSqlSugarClient();

            var alarmList = new List<AlarmItem>();

            DateTime startdate = db.Queryable<EquipmentParamsHistoryCalculate>()
               .Where(it => it.Name == "starttime"
               && it.Remark == week
               && it.EQID == eqpType).ToList()
               .OrderBy(it => Convert.ToDateTime(it.Value)).Select(it => Convert.ToDateTime(it.Value)).First();
            DateTime enddate = db.Queryable<EquipmentParamsHistoryCalculate>()
                .Where(it => it.Name == "endtime"
                && it.Remark == week
                && it.EQID == eqpType).ToList()
                .OrderByDescending(it => Convert.ToDateTime(it.Value)).Select(it => Convert.ToDateTime(it.Value)).First();

            alarmList = DashBoardService.GetAlarmDetails(startdate, enddate, EQID, 9);

            foreach (var item in alarmList)
            {
                item.AlarmText = item.AlarmText.Split(';').Length == 3 ? item.AlarmText.Split(';')[2] : item.AlarmText;
            }

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

            var totalalarm = new { totalcodelist, totalcodetext, totalalarmtime, totalalarmpie };
            return Json(new { alarmList = alarmList.OrderBy(it => Convert.ToDateTime(it.AlarmDate)), alarmtotal = totalalarm });
        }
        public JsonResult GetAlarmRate(string selectweek, string eqpType)
        {
            var db = DbFactory.GetSqlSugarClient();
            var eqtype = db.Queryable<EquipmentType>().Where(it => it.Name == eqpType).Select(it => it.ID).ToList();
            var eqpidids = db.Queryable<Equipment>().Where(it => eqtype.Contains(it.EquipmentTypeId)).OrderBy(it => it.Sort).Select(it => it.EQID).ToList();

            var eqpNames = db.Queryable<Equipment>().Where(it => eqtype.Contains(it.EquipmentTypeId)).OrderBy(it => it.Sort).Select(it => it.Name).ToList();
            string alarmSQL = string.Format(@"
            SELECT EQID, NAME, to_char(sum(cast(VALUE as DECIMAL(6,2))),'fm9990.99') VALUE FROM EQUIPMENTPARAMSHISCAL
            WHERE REMARK = '{0}'
            AND NAME <> 'starttime'
            AND NAME <> 'endtime'
            AND NAME <> 'idleduration'
            GROUP BY EQID, NAME", selectweek);
            var alarmList = db.SqlQueryable<EquipmentParamsHistoryCalculate>(alarmSQL).Where(it=> eqpidids.Contains(it.EQID)).ToList();

            var alarmSpanByStation = alarmList.Where(it => it.Name == "alarmspan").Select(it => new { name = it.EQID, value = it.Value }).ToList();

            List<double> alarmrates = new List<double>();
            List<double> alarmtimes = new List<double>();
            List<string> eqdata = new List<string>();
            var mfgspan = new TimeSpan();
            List<DateTime> startdateList = db.Queryable<EquipmentParamsHistoryCalculate>()
               .Where(it => it.Name == "starttime"
               && it.Remark == selectweek
               && it.EQID == eqpType).ToList()
               .OrderBy(it => Convert.ToDateTime(it.Value)).Select(it => Convert.ToDateTime(it.Value)).ToList();
            List<DateTime> enddateList = db.Queryable<EquipmentParamsHistoryCalculate>()
                .Where(it => it.Name == "endtime"
                && it.Remark == selectweek
                && it.EQID == eqpType).ToList()
                .OrderBy(it => Convert.ToDateTime(it.Value)).Select(it => Convert.ToDateTime(it.Value)).ToList();
            var idleTime = db.Queryable<EquipmentParamsHistoryCalculate>()
                .Where(it => it.Name == "idleduration"
                && it.Remark == selectweek
                && it.EQID == eqpType).Sum(it => Convert.ToDouble(it.Value));


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

            return Json(new { eqdata = eqpNames, eqids = eqpidids, alarmtimes = alarmtimes, alarmrates = alarmrates, starttime = startdateList[0].ToString(), endtime = enddateList[enddateList.Count - 1].ToString(), maxtime = maxalarmtimes, maxrate = maxalarmrates, alarmSpanByStation });

        }

        public JsonResult SetUPD(string target, string eqpType)
        {
            //User set the start time and end time firstly.
            var db = DbFactory.GetSqlSugarClient();
            var eqtype = db.Queryable<EquipmentType>().Where(it => it.Name == eqpType).Select(it => it.ID).First();
            var UPDrecord = db.Queryable<EquipmentTypeConfiguration>().Where(it => it.TypeNameId == eqtype && it.ConfigurationItem == "UPD")?.First();
            if (UPDrecord != null)
            {
                UPDrecord.ConfigurationValue = target;
                //UPDrecord.UpdateTime = DateTime.Now;
                db.Updateable(UPDrecord).Where(it => it.ID == UPDrecord.ID).ExecuteCommand();
            }

            return Json(new { });


        }



        public JsonResult SetAlarmRateTarget(string target, string eqpType)
        {

            var db = DbFactory.GetSqlSugarClient();
            var eqtype = db.Queryable<EquipmentType>().Where(it => it.Name == eqpType).Select(it => it.ID).First();
            var alarmTarget = db.Queryable<EquipmentTypeConfiguration>().Where(it => it.TypeNameId == eqtype && it.ConfigurationItem == "AlarmRateTarget")?.First();
            if (alarmTarget != null)
            {
                alarmTarget.ConfigurationValue = target;
                //UPDrecord.UpdateTime = DateTime.Now;
                db.Updateable(alarmTarget).Where(it => it.ID == alarmTarget.ID).ExecuteCommand();
            }


            return Json(new { });
        }

        
        protected void AddDirectoryToZip(ZipArchive archive, string sourceDir, string entryName)
        {
            // 获取目录中的所有文件并将它们添加到zip存档中
            foreach (var filePath in Directory.GetFiles(sourceDir))
            {
                string entryPath = Path.Combine(entryName, Path.GetFileName(filePath));
                var entry = archive.CreateEntry(entryPath);

                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                using (var entryStream = entry.Open())
                {
                    fileStream.CopyTo(entryStream);
                }
            }

            // 递归添加子目录
            foreach (var directory in Directory.GetDirectories(sourceDir))
            {
                string directoryName = Path.GetFileName(directory);
                AddDirectoryToZip(archive, directory, Path.Combine(entryName, directoryName));
            }
        }
    }
}