using EAP.Dashboard.Models;
using EAP.Dashboard.Utils;
using EAP.Models.Database;
using Microsoft.IdentityModel.Tokens;
using Mysqlx.Datatypes;
using OfficeOpenXml;
using OfficeOpenXml.Core.ExcelPackage;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlTypes;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using static log4net.Appender.RollingFileAppender;
using ExcelPackage = OfficeOpenXml.ExcelPackage;
using ExcelWorkbook = OfficeOpenXml.ExcelWorkbook;
using ExcelWorksheet = OfficeOpenXml.ExcelWorksheet;

namespace EAP.Dashboard.Controllers
{
    public class APLPackingController : BaseController
    {

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

        public ActionResult setWeek()
        {
            return View();
        }

        public ActionResult setTarget()
        {
            return View();
        }

        public ActionResult weekSelector()
        {
            return View();
        }

        public JsonResult GetStatus()
        {
            var db = DbFactory.GetSqlSugarClient();

            // 获取设备类型
            var eqtypeIds = db.Queryable<EquipmentType>()
                .Where(it => it.Name == "APL Packing")
                .Select(it => it.ID)
                .ToList();

            // 获取设备信息（EQID + EQNAME + 排序字段）
            var eqps = db.Queryable<Equipment>()
                .Where(it => eqtypeIds.Contains(it.EquipmentTypeId))
                .OrderBy(it => it.Sort)
                .Select(it => new { it.EQID, it.Name })
                .ToList();

            var eqpids = eqps.Select(e => e.EQID).ToList();

            // 状态、产量、良率一次性查出
            var statusDict = db.Queryable<V_EquipmentStatus>()
                .Where(it => eqpids.Contains(it.EQID))
                .ToDictionary(it => it.EQID, it => it.Status);

            var outputDict = db.Queryable<EquipmentParamsRealtime>()
                .Where(it => eqpids.Contains(it.EQID) && it.Name.ToLower() == "output")
                .ToDictionary(it => it.EQID, it => it.Value);

            var yieldDict = db.Queryable<EquipmentParamsRealtime>()
                .Where(it => eqpids.Contains(it.EQID) && it.Name.ToLower() == "yield")
                .ToDictionary(it => it.EQID, it => it.Value);

            // 组装结果
            var eqpProfiles = eqps.Select(e => new EquipmentProfile
            {
                EQID = e.EQID,
                EQName = e.Name,
                Status = (string)(statusDict.ContainsKey(e.EQID) ? statusDict[e.EQID] : null),
                output = (string)(outputDict.ContainsKey(e.EQID) ? outputDict[e.EQID] : null),
                yield = (string)(yieldDict.ContainsKey(e.EQID) ? yieldDict[e.EQID] : null)
            }).ToList();

            return Json(new { eqpProfiles });
        }
        public JsonResult GetStation(string EQID, string datetime, string selecttype)
        {
            var db = DbFactory.GetSqlSugarClient();
            var date = Convert.ToDateTime(datetime).Date;
            var eqid = EQID;

            var eqguid = new byte[16];
            var output = new List<string>();
            var outputdates = new List<string>();
            var fpysdates = new List<string>();
            var fpy = new List<string>();
            var oee = new List<string>();
            var oeedates = new List<string>();
            var runrates = new List<string>();
            var runratedates = new List<string>();
            if (eqid != null)
            {
                //ExportDataByStation(EQID, datetime);

                var data = db.Queryable<EquipmentParamsHistoryCalculate>().Where(it => it.EQID == eqid).ToList();
                var eqtype = db.Queryable<EquipmentType>().Where(it => it.Name == "ACCBOX").Select(it => it.ID).ToList();
                var eqpidids = db.Queryable<Equipment>().Where(it => eqtype.Contains(it.EquipmentTypeId)).OrderBy(it => it.Sort).Select(it => it.EQID).ToList();
                //var runratedata = db.EquipmentRunrateHistory.Where(it => it.EQID == eqid && it.DataTime.Date >= date.AddDays(-7) && it.DataTime <= date).OrderBy(it => it.DataTime);
                if (eqpidids.Contains(eqid))
                    eqguid = db.Queryable<Equipment>().Where(it => it.EQID == eqid).Select(it => it.ID).Single();

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


                var rtdata = db.Queryable<EquipmentParamsRealtime>().Where(it => it.EQID == eqid).ToList();
                var yieldrtdata = rtdata.Count > 0 ? rtdata.Where(it => it.Name.Equals("yield")).ToList() : new List<EquipmentParamsRealtime>();
                var outputrtdata = rtdata.Count > 0 ? rtdata.Where(it => it.Name.Equals("output")).ToList() : new List<EquipmentParamsRealtime>();



                var trenddata = new { outputs = output, fpys = fpy, outputdates = outputdates, fpysdates = fpysdates, oee = oee, oeedates = oeedates };
                var alarmcodeList = db.Queryable<EquipmentConfiguration>().Where(it => it.EQIDId == eqguid && it.ConfigurationItem == "Equipment.AlarmCode").Select(it => it.ConfigurationValue).First();
                var alarmcodes = alarmcodeList.Split(',');

                var alarmdata = db.Queryable<EquipmentAlarm>()
                    .Where(it => it.AlarmEqp.ToUpper() == EQID
                    && it.AlarmTime > date.AddHours(9)
                    && it.AlarmTime < date.AddDays(1).AddHours(9) && it.AlarmSet == true && alarmcodes.Contains(it.AlarmCode))
                    .OrderByDescending(it => it.AlarmTime)
                    .Select(it => new { ALARMTIME = it.AlarmTime, ALARMCODE = it.AlarmCode, ALARMTEXT = it.AlarmText }).ToList();
                var status = db.Queryable<EquipmentRealtimeStatus>().Where(it => it.EQID == eqid).First();

                var target = db.Queryable<EquipmentTypeConfiguration>().Where(it => it.TypeNameId == eqtype.First() && it.ConfigurationItem == "UPD").Select(it => it.ConfigurationValue).First();
                return Json(new { trenddata, alarmdata, status, yieldrtdata, outputrtdata, target, alarmduration = date.AddHours(9) + "~" + date.AddDays(1).AddHours(9) });
            }

            return Json(new { });
        }

        public JsonResult GetStationProfile(string EQID)
        {
            var db = DbFactory.GetSqlSugarClient();
            var eqtype = db.Queryable<EquipmentType>().Where(it => it.Name == "APL Packing").Select(it => it.ID).First();

            var rtdata = db.Queryable<EquipmentParamsRealtime>().Where(it => it.EQID == EQID).ToList();
            var target = db.Queryable<EquipmentTypeConfiguration>().Where(it => it.TypeNameId == eqtype && it.ConfigurationItem == "UPD").Select(it => it.ConfigurationValue).First();
            var status = db.Queryable<V_EquipmentStatus>().Where(it => it.EQID == EQID).Single();

            return Json(new { rtdata, target, status });

        }

        public JsonResult GetStationAlarms(string EQID, string datetime)
        {
            var date = DateTime.Now;
            var db = DbFactory.GetSqlSugarClient();
            var eqtype = db.Queryable<EquipmentType>().Where(it => it.Name == "APL Packing").Select(it => it.ID).ToList();
            var eqpidids = db.Queryable<Equipment>().Where(it => eqtype.Contains(it.EquipmentTypeId)).OrderBy(it => it.Sort).Select(it => it.EQID).ToList();

            string sql = string.Format(@"SELECT a.EQID AS EQID,a.ALARMCODE AS AlarmCode,a.ALARMTIME AS AlarmTime,a.ALARMTEXT AS AlarmText , b.POSITIONID AS PositionID
FROM EQUIPMENTALARM a,ALARMPOSITIONS b
WHERE a.EQID = LOWER( '{0}')
AND a.ALARMTIME >= to_date('{1}','yyyy-mm-dd hh24:mi:ss')
AND a.EQID = LOWER(b.EQID)
AND a.ALARMCODE = b.ALARMCODE
AND ROWNUM <= 1
ORDER BY a.ALARMTIME DESC", EQID, date.AddDays(-1).AddHours(9).ToString("yyyy-MM-dd HH:mm:ss"));
            var result = db.Ado.SqlQuery<AlarmPositionsDetails>(sql).ToList();
            //var result = OracleHelper.GetDTFromCommand(sql);
            //var result = from alarm in alarmdata
            //join position in positiondata on alarm.ALARMCODE equals position.AlarmCode
            //select new { position.EQID, alarm.ALARMTEXT, alarm.ALARMCODE, alarm.ALARMTIME, position.PositionID };

            return Json(new { result }); //, result
        }

        public JsonResult GetLineOEE()
        {
            var db = DbFactory.GetSqlSugarClient();
            var output = db.Queryable<EquipmentParamsRealtime>().Where(it => it.EQID == "EQRSP00001" && it.Name.ToLower() == "output").First()?.Value;
            var yield = db.Queryable<EquipmentParamsRealtime>().Where(it => it.EQID == "EQRSP00001" && it.Name.ToLower() == "yield").First()?.Value;
            var eqtype = db.Queryable<EquipmentType>().Where(it => it.Name == "APL Packing").Select(it => it.ID).ToList();
            var target = db.Queryable<EquipmentTypeConfiguration>().Where(it => it.TypeNameId == eqtype.First() && it.ConfigurationItem == "UPD").First()?.ConfigurationValue;
            //var target = db.Queryable<EquipmentTypeConfiguration>().Where(it => it.TypeNameId == eqtype.First() && it.ConfigurationItem == "UPD").First()?.ConfigurationValue;
            var target_value = target ?? output;

            var now = DateTime.Now;
            var timespan = (now - DateTime.Today).TotalMinutes;
            var percent = target_value == output ? 100 : timespan / 1440;

            var oee_value = output == "0" ? 0 : (Convert.ToInt32(output) / (Convert.ToInt32(target_value) * percent)) * Convert.ToDouble(yield) * 100;

            return Json(new { data = oee_value, target = target_value });
        }

        public JsonResult GetRealTimeAlarms()
        {
            var db = DbFactory.GetSqlSugarClient();

            // 获取 APL Packing 类型下的所有设备，带 EQID 和 EQName
            var eqtype = db.Queryable<EquipmentType>().Where(it => it.Name == "APL Packing").Select(it => it.ID).ToList();
            var eqpList = db.Queryable<Equipment>()
                .Where(it => eqtype.Contains(it.EquipmentTypeId))
                .OrderBy(it => it.Sort)
                .Select(it => new { it.EQID, it.Name }) // EQID 和 EQName
                .ToList();

            // 构建字典映射，便于快速查找
            var eqpMap = eqpList.ToDictionary(x => x.EQID, x => x.Name);

            var date = DateTime.Now;

            var alarmdataRaw = db.Queryable<EquipmentAlarm>()
                .Where(it => it.AlarmSource == "APL Packing"
                    && it.AlarmTime > date.AddDays(-1)
                    && it.AlarmTime < date
                    && it.AlarmSet == true)
                .OrderByDescending(it => it.AlarmTime)
                .Select(it => new
                {
                    EQID = it.AlarmEqp,
                    ALARMTIME = it.AlarmTime,
                    ALARMCODE = it.AlarmCode,
                    ALARMTEXT = it.AlarmText
                })
                .ToList();

            // 拼接 EQName
            var alarmdata = alarmdataRaw.Select(a => new
            {
                a.EQID,
                EQName = eqpMap.ContainsKey(a.EQID) ? eqpMap[a.EQID] : "Unknown",
                a.ALARMTIME,
                a.ALARMCODE,
                a.ALARMTEXT
            }).ToList();

            return Json(new { alarmdata });

        }

        public JsonResult GetParamsStatus()
        {
            var db = DbFactory.GetSqlSugarClient();

            // 获取目标设备类型ID
            var eqtypeIds = db.Queryable<EquipmentType>()
                .Where(it => it.Name == "APL Packing")
                .Select(it => it.ID)
                .ToList();

            // 获取设备列表及映射
            var equipments = db.Queryable<Equipment>()
                .Where(it => eqtypeIds.Contains(it.EquipmentTypeId))
                .OrderBy(it => it.Sort)
                .Select(it => new { it.EQID, it.Name, it.ID, it.Sort })
                .ToList();

            var eqidToName = equipments.ToDictionary(x => x.EQID, x => x.Name);
            var eqidToGuid = equipments.ToDictionary(x => x.EQID, x => x.ID);
            var eqidToSort = equipments.ToDictionary(x => x.EQID, x => x.Sort);

            // 获取实时参数
            var parameters = db.Queryable<EquipmentParamsRealtime>()
                .Where(it => eqidToName.Keys.Contains(it.EQID)
                    && it.Name.ToLower() != "output"
                    && it.Name.ToLower() != "yield"
                    && it.Name.ToLower() != "status"
                     && !it.Name.ToLower().Contains("shortage"))
                .OrderBy(it => it.EQID)
                .OrderBy(it => it.SVID)
                .ToList();

            var resultList = new List<dynamic>();

            foreach (var param in parameters)
            {
                if (!eqidToGuid.TryGetValue(param.EQID, out byte[] eqGuid))
                    continue;

                var configList = db.Queryable<EquipmentConfiguration>()
                    .Where(it => it.EQIDId == eqGuid && (it.ConfigurationName == "UCL_" + param.Name ||
                    it.ConfigurationName == "LCL_" + param.Name ||
                                               it.ConfigurationName == "WarnUCL_" + param.Name ||
                                               it.ConfigurationName == "WarnLCL_" + param.Name
                    ))
                    .ToList();

                string targetUcl = configList.FirstOrDefault(x => x.ConfigurationName == "UCL_" + param.Name)?.ConfigurationValue;
                string targetLcl = configList.FirstOrDefault(x => x.ConfigurationName == "LCL_" + param.Name)?.ConfigurationValue;
                string warnUcl = configList.FirstOrDefault(x => x.ConfigurationName == "WarnUCL_" + param.Name)?.ConfigurationValue;
                string warnLcl = configList.FirstOrDefault(x => x.ConfigurationName == "WarnLCL_" + param.Name)?.ConfigurationValue;

                double? upperLimit = double.TryParse(targetUcl, out double ucl) ? ucl : (double?)null;
                double? lowerLimit = double.TryParse(targetLcl, out double lcl) ? lcl : (double?)null;
                double? warnUpperLimit = double.TryParse(warnUcl, out double wucl) ? wucl : (double?)null;
                double? warnLowerLimit = double.TryParse(warnLcl, out double wlcl) ? wlcl : (double?)null;

                bool isOverLimit = false;
                bool isWarning = false;

                if (double.TryParse(param.Value, out double realVal))
                {
                    // 超标判断
                    if (upperLimit.HasValue && realVal > upperLimit.Value)
                        isOverLimit = true;
                    if (lowerLimit.HasValue && realVal < lowerLimit.Value)
                        isOverLimit = true;

                    // 警戒判断（仅在未超标情况下判断）
                    if (!isOverLimit)
                    {
                        if (warnUpperLimit.HasValue && realVal > warnUpperLimit.Value)
                            isWarning = true;
                        if (warnLowerLimit.HasValue && realVal < warnLowerLimit.Value)
                            isWarning = true;
                    }
                }

                resultList.Add(new
                {
                    EQID = param.EQID,
                    EQName = eqidToName[param.EQID],
                    Name = param.Name,
                    Value = param.Value,
                    LCL = lowerLimit,
                    UCL = upperLimit,
                    WarnLCL = warnLowerLimit,
                    WarnUCL = warnUpperLimit,
                    IsOverLimit = isOverLimit,
                    IsWarning = isWarning,
                    Sort = eqidToSort[param.EQID],
                    SVID = param.SVID
                });
            }

            // 最终排序：设备Sort优先，设备内参数SVID排序
            var sortedResult = resultList
                .OrderBy(x => x.Sort)
                .ThenBy(x => x.SVID)
                .Select(x => new
                {
                    x.EQID,
                    x.EQName,
                    x.Name,
                    x.Value,
                    x.LCL,
                    x.UCL,
                    x.WarnLCL,
                    x.WarnUCL,
                    x.IsOverLimit,
                    x.IsWarning
                })
                .ToList();

            return Json(sortedResult);

        }

        public JsonResult GetHourlyDownTime(string datetime)
        {
            var date = datetime.IsNullOrEmpty() ? DateTime.Now : Convert.ToDateTime(datetime);
            var db = DbFactory.GetSqlSugarClient();
            var eqtype = db.Queryable<EquipmentType>().Where(it => it.Name == "APL Packing").Select(it => it.ID).ToList();
            var eqpids = db.Queryable<Equipment>().Where(it => eqtype.Contains(it.EquipmentTypeId)).OrderBy(it => it.Sort).Select(it => it.EQID).ToList();
            var alarms = new List<AlarmItem>();
            var startTime = date.Date.AddDays(-2);
            var endTime = date.Date.AddDays(2);
            var target = db.Queryable<EquipmentTypeConfiguration>().Where(it => it.TypeNameId == eqtype.First() && it.ConfigurationItem == "HourlyDownTimeTarget").First()?.ConfigurationValue;
            foreach (var eqid in eqpids)
            {
                var alarmList = DashBoardService.GetAlarmDetails(startTime, endTime, eqid, 0);
                alarms.AddRange(alarmList);
            }
            var data = DashBoardService.CalculateHourlyDownTime(alarms, date);
            return Json(new { downtimes = data, downtimetarget = target ?? "0" }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDailyDownTime(string datetime)
        {
            var date = datetime.IsNullOrEmpty() ? DateTime.Now : Convert.ToDateTime(datetime);
            var db = DbFactory.GetSqlSugarClient();
            var eqtype = db.Queryable<EquipmentType>().Where(it => it.Name == "APL Packing").Select(it => it.ID).ToList();
            var eqpids = db.Queryable<Equipment>().Where(it => eqtype.Contains(it.EquipmentTypeId)).OrderBy(it => it.Sort).Select(it => it.EQID).ToList();
            var alarms = new List<AlarmItem>();
            var startTime = date.Date.AddDays(-7);
            var endTime = date.Date.AddDays(-1);
            var target = db.Queryable<EquipmentTypeConfiguration>().Where(it => it.TypeNameId == eqtype.First() && it.ConfigurationItem == "DailyDownTimeTarget").First()?.ConfigurationValue;
            foreach (var eqid in eqpids)
            {
                var alarmList = DashBoardService.GetAlarmDetails(startTime, endTime.AddDays(1), eqid, 0);
                alarms.AddRange(alarmList);
            }

            var data = DashBoardService.AggregateDailyFromHourlyBatch(alarms, startTime, endTime);
            return Json(new { downtimes = data, downtimetarget = target ?? "0" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SetDownTimeTarget(string target, string eqpType, string dataType)
        {

            var db = DbFactory.GetSqlSugarClient();
            var eqtype = db.Queryable<EquipmentType>().Where(it => it.Name == eqpType).Select(it => it.ID).First();
            var alarmTarget = db.Queryable<EquipmentTypeConfiguration>().Where(it => it.TypeNameId == eqtype && it.ConfigurationItem == dataType)?.First();
            if (alarmTarget != null)
            {
                alarmTarget.ConfigurationValue = target;
                //UPDrecord.UpdateTime = DateTime.Now;
                db.Updateable(alarmTarget).Where(it => it.ID == alarmTarget.ID).ExecuteCommand();
            }


            return Json(new { });
        }

        public JsonResult UpdateParamLimits([FromBody] ParamLimitUpdateModel model)
        {
            try
            {
                var db = DbFactory.GetSqlSugarClient();

                // 使用事务确保操作原子性
                var result = db.Ado.UseTran(() =>
                {
                    // 一次性获取设备ID和配置
                    var eqGuid = db.Queryable<Equipment>()
                                  .Where(it => it.EQID == model.eqid)
                                  .Select(it => it.ID)
                                  .First();

                    if (eqGuid == null)
                    {
                        throw new Exception("更新失败，不存在该设备！");
                    }

                    // 一次性查询所有相关配置
                    var configs = db.Queryable<EquipmentConfiguration>()
                                  .Where(it => it.EQIDId == eqGuid &&
                                             (it.ConfigurationName == "LCL_" + model.ParamName ||
                                              it.ConfigurationName == "UCL_" + model.ParamName ||
                                               it.ConfigurationName == "WarnUCL_" + model.ParamName ||
                                               it.ConfigurationName == "WarnLCL_" + model.ParamName
                                               ))
                                  .ToList();

                    var lclConfig = configs.FirstOrDefault(it => it.ConfigurationName.StartsWith("LCL"));
                    var uclConfig = configs.FirstOrDefault(it => it.ConfigurationName.StartsWith("UCL"));
                    var warnlclConfig = configs.FirstOrDefault(it => it.ConfigurationName.StartsWith("WarnLCL_"));
                    var warnuclConfig = configs.FirstOrDefault(it => it.ConfigurationName.StartsWith("WarnUCL_"));

                    // 处理LCL配置
                    ProcessConfig(db, lclConfig, eqGuid, model.ParamName, "LCL", model.Lcl);

                    // 处理UCL配置
                    ProcessConfig(db, uclConfig, eqGuid, model.ParamName, "UCL", model.Ucl);

                    // 处理WarnUCL配置
                    ProcessConfig(db, warnuclConfig, eqGuid, model.ParamName, "WarnUCL", model.Warnucl);
                    // 处理WarnUCL配置
                    ProcessConfig(db, warnlclConfig, eqGuid, model.ParamName, "WarnLCL", model.Warnlcl);
                });

                if (result.IsSuccess)
                {
                    return Json(new { success = true, message = "更新成功" });
                }
                else
                {
                    return Json(new { success = false, message = result.ErrorMessage });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // 提取公共方法处理配置
        private void ProcessConfig(SqlSugarClient db, EquipmentConfiguration config, byte[] eqGuid, string paramName, string prefix, double? value)
        {
            if (config == null)
            {
                // 插入新配置
                var newConfig = new EquipmentConfiguration
                {
                    ID = Guid.NewGuid().ToByteArray(),
                    EQIDId = eqGuid,
                    ConfigurationItem = "ParameterSPEC",
                    ConfigurationName = $"{prefix}_{paramName}",
                    ConfigurationValue = value.ToString(),
                    IsReadOnly = false,
                    IsValid = true
                };
                db.Insertable(newConfig).ExecuteCommand();
            }
            else
            {
                // 更新现有配置
                config.ConfigurationValue = value.ToString();
                db.Updateable(config).ExecuteCommand();
            }
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
            string tempDirectory = Path.Combine("\\\\127.0.0.1\\e$\\tmp\\APLPacking", Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDirectory);

            foreach (var eqp in eqps)
            {
                string subDirectory = Path.Combine(tempDirectory, eqp);
                Directory.CreateDirectory(subDirectory);

                foreach (var report in reports)
                {
                    DashBoardService.GenerateReport(eqp, targetDate, cyclingTimes, report, subDirectory, 0);
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


        public class ParamLimitUpdateModel
        {
            public string eqid { get; set; }
            public string EqName { get; set; }
            public string ParamName { get; set; }
            public double? Lcl { get; set; }
            public double? Ucl { get; set; }
            public double? Warnlcl { get; set; }
            public double? Warnucl { get; set; }
        }
    }
}