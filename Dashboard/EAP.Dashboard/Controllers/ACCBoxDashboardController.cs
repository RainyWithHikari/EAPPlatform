using EAP.Dashboard.Models;
using EAP.Dashboard.Utils;
using EAP.Models.Database;
using OfficeOpenXml;
using OfficeOpenXml.Core.ExcelPackage;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using ExcelPackage = OfficeOpenXml.ExcelPackage;
using ExcelWorkbook = OfficeOpenXml.ExcelWorkbook;
using ExcelWorksheet = OfficeOpenXml.ExcelWorksheet;

namespace EAP.Dashboard.Controllers
{
    public class ACCBoxDashboardController : BaseController
    {
        
        public ActionResult Index()
        {
            var isDebugMode = ConfigurationManager.AppSettings["IsDebugMode"].ToUpper().ToString() == "TRUE";
            if (isDebugMode) return View();
            return View();
        }
        public ActionResult stationdetails()
        {
            return View();
        }

        public ActionResult alarmdetails()
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
        public JsonResult GetEQP()
        {
            var db = DbFactory.GetSqlSugarClient();
            var eqtype = db.Queryable<EquipmentType>().Where(it => it.Name == "ACCBOX").Select(it => it.ID).ToList();
            var eqpidids = db.Queryable<Equipment>().Where(it => eqtype.Contains(it.EquipmentTypeId)).OrderBy(it => it.Sort).ToList();


            string option = "<option value=\"" + eqpidids.ElementAt(0).EQID + "\">" + eqpidids.ElementAt(0).Name + "</option>";
            foreach (var item in eqpidids)
            {
                option += "<option value=\"" + item.EQID + "\" >" + item.Name + "</option>";
            }
            return Json(new { option = option, defaultoption = eqpidids.ElementAt(0).EQID });
        }
        public JsonResult GetStatus()
        {
            var db = DbFactory.GetSqlSugarClient();
            var eqtype = db.Queryable<EquipmentType>().Where(it => it.Name == "ACCBOX").Select(it => it.ID).ToList();
            var eqpidids = db.Queryable<Equipment>().Where(it => eqtype.Contains(it.EquipmentTypeId)).OrderBy(it => it.Sort).Select(it => it.EQID).ToList();
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


            var output = db.Queryable<EquipmentParamsRealtime>().Where(it => eqpidids.Contains(it.EQID) && it.Name == "output").ToList();
            var yield = db.Queryable<EquipmentParamsRealtime>().Where(it => eqpidids.Contains(it.EQID) && it.Name == "yield").ToList();

            //var eqpProfile = new EquipmentProfile();
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
            for (int i = 0; i < eqpProfiles.Count; i++)
            {
                for (int j = 0; j < output.Count; j++)
                {
                    if (eqpProfiles[i].EQID == output[j].EQID)
                    {
                        eqpProfiles[i].output = output[j].Value;
                        break;
                    }

                }
                for (int j = 0; j < yield.Count; j++)
                {
                    if (eqpProfiles[i].EQID == yield[j].EQID)
                    {
                        eqpProfiles[i].yield = yield[j].Value;
                        break;
                    }


                }
            }
            return Json(new {  eqpProfiles });
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
               
                var data = db.Queryable< EquipmentParamsHistoryCalculate>().Where(it => it.EQID == eqid).ToList();
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
                var yieldrtdata = rtdata.Count > 0 ? rtdata.Where(it => it.Name.Equals("yield")).ToList():new List<EquipmentParamsRealtime>();
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

                var target = db.Queryable<EquipmentTypeConfiguration>().Where(it => it.TypeNameId == eqtype.First() &&  it.ConfigurationItem == "UPD").Select(it => it.ConfigurationValue).First();
                return Json(new { trenddata, alarmdata, status, yieldrtdata, outputrtdata, target, alarmduration = date.AddHours(9) + "~" + date.AddDays(1).AddHours(9) });
            }

            return Json(new { });
        }

        public JsonResult GetStationProfile(string EQID)
        {
            var db = DbFactory.GetSqlSugarClient();
            var eqtype = db.Queryable<EquipmentType>().Where(it => it.Name == "ACCBOX").Select(it => it.ID).First();

            var rtdata = db.Queryable<EquipmentParamsRealtime>().Where(it => it.EQID == EQID).ToList();
            var target = db.Queryable<EquipmentTypeConfiguration>().Where(it => it.TypeNameId == eqtype && it.ConfigurationItem == "UPD").Select(it => it.ConfigurationValue).First();
            var status = db.Queryable<V_EquipmentStatus>().Where(it => it.EQID == EQID).Single();

            return Json(new { rtdata, target, status });

        }

        public JsonResult GetStationAlarms(string EQID, string datetime)
        {
            var date = DateTime.Now;
            var db = DbFactory.GetSqlSugarClient();
            var eqtype = db.Queryable<EquipmentType>().Where(it => it.Name == "ACCBOX").Select(it => it.ID).ToList();
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
       

        
        public JsonResult GetRealTimeAlarms()
        {
            var db = DbFactory.GetSqlSugarClient();
            var eqtype = db.Queryable<EquipmentType>().Where(it => it.Name == "ACCBOX").Select(it => it.ID).ToList();
            var eqpidids = db.Queryable<Equipment>().Where(it => eqtype.Contains(it.EquipmentTypeId)).OrderBy(it => it.Sort).Select(it => it.EQID).ToList();

            var date = DateTime.Now;
            var alarmcodes = new List<string>();

            for (int i = 0; i < eqpidids.Count; i++)
            {
                var eqid = eqpidids[i];
                var eqguid = db.Queryable<Equipment>().Where(it => it.EQID == eqid).Select(it => it.ID).Single();

                var alarmcodeList = db.Queryable<EquipmentConfiguration>().Where(it => it.EQIDId == eqguid && it.ConfigurationItem == "Equipment.AlarmCode").Select(it => it.ConfigurationValue).First();
                var tmpList = alarmcodeList.Split(',');
                foreach (var item in tmpList)
                {

                    alarmcodes.Add(item);
                }
                //alarmcodes.Append()
                //alarmcodes.Add(alarmcodeList.Split(","));

            }


            var alarmdata = db.Queryable<EquipmentAlarm>()
                .Where(it => it.AlarmSource == "ACCBOX"
                && it.AlarmTime > date.AddDays(-1)
                && it.AlarmTime < date && it.AlarmSet == true && alarmcodes.Contains(it.AlarmCode))
                .OrderByDescending(it => it.AlarmTime).Take(10)
                .Select(it => new { EQID = it.AlarmEqp,
                    ALARMTIME =it.AlarmTime,
                    ALARMCODE = it.AlarmCode,
                    //Carrier = it.AlarmText.Split(';').Length == 3? it.AlarmText.Split(';')[0]:" ",
                    //SN = it.AlarmText.Split(';').Length == 3 ? it.AlarmText.Split(';')[1] : " ",
                    ALARMTEXT = it.AlarmText //it.AlarmText.Split(';').Length == 3 ? it.AlarmText.Split(';')[2]: 
                }).ToList();

            

            return Json(new { alarmdata });
        }
       

        public FileContentResult ExportAlarmDataByWeek(string week)
        {
            var db = DbFactory.GetSqlSugarClient();
            var eqtype = db.Queryable<EquipmentType>().Where(it => it.Name == "ACCBOX").Select(it => it.ID).ToList();
            var eqpidids = db.Queryable<Equipment>().Where(it => eqtype.Contains(it.EquipmentTypeId)).OrderBy(it => it.Sort).Select(it => it.EQID).ToList();
            
            var alarmList = new List<AlarmItem>();
            DateTime startdate = db.Queryable<EquipmentParamsHistoryCalculate>()
               .Where(it => it.Name == "starttime"
               && it.Remark == week
               && it.EQID == "ACCBOX").ToList()
               .OrderBy(it => Convert.ToDateTime(it.Value)).Select(it => Convert.ToDateTime(it.Value)).First();
            DateTime enddate = db.Queryable<EquipmentParamsHistoryCalculate>()
                .Where(it => it.Name == "endtime"
                && it.Remark == week
                && it.EQID == "ACCBOX").ToList()
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
                ws.Column(7).Style.Numberformat.Format = "yyyy/m/d h:mm:ss";
                ws.Column(8).Style.Numberformat.Format = "yyyy/m/d h:mm:ss";

            }
            var filename = "ACCBOX_" + week + "_" + $"AlarmData.xlsx";
            var datastring = ep.GetAsByteArray();
            return new FileContentResult(datastring, "application/x-xls")
            {
                FileDownloadName = filename
            };
        }

        public FileContentResult ExportAlarmDataByStation(string EQID, string datetime)
        {
            var db = DbFactory.GetSqlSugarClient();

            var alarmList = new List<AlarmItem>();
            DateTime startdate = Convert.ToDateTime(datetime).Date.AddHours(9);
            DateTime enddate = Convert.ToDateTime(datetime).Date.AddDays(1).AddHours(9);
            string route = string.Empty;

            alarmList = DashBoardService.GetAlarmDetails(startdate, enddate, EQID, 9);
            foreach (var item in alarmList)
            {
                item.AlarmText = item.AlarmText.Split(';').Length == 3 ? item.AlarmText.Split(';')[2] : item.AlarmText;
            }

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

            DataTable total = DashBoardService.ToDataTable(alarmtotal);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage ep = new ExcelPackage();
            ExcelWorkbook wb = ep.Workbook;
            ExcelWorksheet ws = wb.Worksheets.Add("Details");
            ws.Cells["A1"].LoadFromDataTable(details, true);
            ws.Column(1).Style.Numberformat.Format = "yyyy/m/d";
            ws.Column(7).Style.Numberformat.Format = "yyyy/m/d h:mm:ss";
            ws.Column(8).Style.Numberformat.Format = "yyyy/m/d h:mm:ss";
            ws.Column(9).Style.Numberformat.Format = "0.00";

            ExcelWorksheet ws2 = wb.Worksheets.Add("Overall");
            ws2.Cells["A1"].LoadFromDataTable(total, true);
            ws2.Column(1).Style.Numberformat.Format = "yyyy/m/d";
            ws2.Column(5).Style.Numberformat.Format = "0.00";

            var filename = EQID + "_" + datetime + "_" + $"AlarmData.xlsx";
            var datastring = ep.GetAsByteArray();
          
            return new FileContentResult(datastring, "application/x-xls")
            {
                FileDownloadName = filename
            };
        }
        public FileContentResult ExportYieldData(DateTime start, DateTime end)
        {
            var db = DbFactory.GetSqlSugarClient();

            start = start.Date;
            end = end.Date;

            var eqtype = db.Queryable<EquipmentType>().Where(it => it.Name == "ACCBOX").Select(it => it.ID).ToList();
            var eqpidids = db.Queryable<Equipment>().Where(it => eqtype.Contains(it.EquipmentTypeId)).OrderBy(it => it.Sort).Select(it => it.EQID).ToList();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage ep = new ExcelPackage();
            ExcelWorkbook wb = ep.Workbook;
            foreach (var eqp in eqpidids)
            {
                var list = db.Queryable<EquipmentParamsHistoryCalculate>().Where(it => it.EQID == eqp
                && (it.Name == "output" || it.Name == "yield")
                && it.UpdateTime >= start
                && it.UpdateTime <= end).OrderBy(it => it.UpdateTime).ToList();

                var query = from row1 in list
                            join row2 in list
                            on row1.UpdateTime.ToShortDateString() equals row2.UpdateTime.ToShortDateString()
                            where row1.Name == "output" && row2.Name == "yield"

                            select new DashBoardService.ParamsList
                            {
                                EQID = row1.EQID,
                                OUTPUT = row1.Value,
                                YIELD = list.FirstOrDefault(d => d.UpdateTime.Date == row1.UpdateTime.Date && d.Name == "yield")?.Value,
                                DATE = row1.UpdateTime,
                            };
                var resultList = new List<DashBoardService.ParamsList>();
                resultList.AddRange(query.Cast<DashBoardService.ParamsList>().ToList());
                ExcelWorksheet ws = wb.Worksheets.Add(eqp);
                ws.Cells["A1"].LoadFromDataTable(DashBoardService.ToDataTable(resultList), true);
                //ws.Column(2).Style.Numberformat.Format = "yyyy/m/d h:mm:ss";
                ws.Column(4).Style.Numberformat.Format = "yyyy/m/d h:mm:ss";
            }
            var filename = $"Total.xlsx";
            //$"ZJ_ACCBOX_{start.ToShortDateString()}~{end.ToShortDateString()}_生产情况.xlsx";
            var datastring = ep.GetAsByteArray();

            Dictionary<byte[],string> fileBytesArr = DashBoardService.GenerateFiles(start, end,"yield");
            Dictionary<byte[], string> fileBytesArr2 = DashBoardService.GenerateFiles(start, end, "output");
            //Dictionary<byte[], string> fileBytesArr2 = DashBoardService.GenerateFiles("output", start, end);
            fileBytesArr.Add(datastring, filename);
            foreach(var file in fileBytesArr2)
            {
                fileBytesArr.Add(file.Key,file.Value);
            }
            

            using (var memoryStream = new MemoryStream())
            {
                using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var dict in fileBytesArr)
                    {
                        var zipEntry = zipArchive.CreateEntry(dict.Value);

                        using (var entryStream = zipEntry.Open())
                        {
                            entryStream.Write(dict.Key, 0, dict.Key.Length);
                        }
                    }
                }

                memoryStream.Seek(0, SeekOrigin.Begin);
                return new FileContentResult(memoryStream.ToArray(), "application/zip")
                {
                    FileDownloadName = $"ZJ_ACCBOX_{start.ToShortDateString()}~{end.ToShortDateString()}_生产情况.zip"
                };
            }
        }
       
    }
}