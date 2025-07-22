using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EAP.Dashboard.Models
{
    public class QuantityOfTray
    {
        public string DAY { get; set; }
        public string COMMANDNAME { get; set; }
        public string COMMANDQTY { get; set; }
    }

    public class CTModel
    {
        public string LINE { get; set; }
        public string MONUMBER { get; set; }
        public string REELID { get; set; }
        public string PN { get; set; }
        public int CT { get; set; }
        public string COMMANDNAME { get; set; }
        public DateTime CREATETIME { get; set; }
        public string DATESTR
        {
            get
            {
                return CREATETIME.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
    }

    public class AlarmModel
    {
        public string ALARMCODE { get; set; }
        public string ALARMTEXT { get; set; }
        public DateTime ALARMTIME { get; set; }
        public string DATESTR
        {
            get
            {
                return ALARMTIME.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
    }

    public class StatusDuration
    {
        public string STATUS { get; set; }
        public DateTime DATETIME { get; set; }
        public DateTime NEXT_STSTUS_DATETIME { get; set; }
        public double DURATION
        {
            get
            {
                return (NEXT_STSTUS_DATETIME - DATETIME).TotalMilliseconds;
            }
        }
    }

    public class ALARMRESULT
    {
        public string ALARMCODE { get; set; }
        public string ALARMTEXT { get; set; }
        public DateTime STARTTIME { get; set; }
        public string DATESTR
        {
            get
            {
                return STARTTIME.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
        public double DURATION { get; set; }
    }

    public class ALARMRESULTCAL
    {
        public string EQID { get; set; }
        public string NAME { get; set; }
        public string VALUE { get; set; }
        public DateTime DATETIME { get; set; }
    }

    public class LineChartModel
    {
        public string DATE { get; set; }
        public double RATE { get; set; }
    }

    public class MaterialStock
    {
        public string Line { get; set; }
        public string OrderNo { get; set; }
        public string PN { get; set; }
        public int QuantityInOrder 
        { 
            get 
            {
                return ReceivedQuantity + DeliveringQuantity + StockQuantity;
            } 
        }
        public int TrayInOrder
        {
            get
            {
                return ReceivedTray + DeliveringTray + StockTray;
            }
        }
        public int UsedQuantity { get; set; }
        public int UsedTray { get; set; }
        public int ReceivedQuantity { get; set; }
        public int ReceivedTray { get; set; }
        public int DeliveringQuantity { get; set; }
        public int DeliveringTray { get; set; }
        public int StockQuantity { get; set; }
        public int StockTray { get; set; }
        public int SMTStockQuantity { get; set; }
        public int SMTStockTray { get; set; }
        public int OccupiedQuantity { get; set; }
        public int OccupiedTray {  get; set; }
        public int AutoOutQuantity { get; set; }
        public int AutoOutTray { get; set; }
        public int ManualOutQuantity { get; set; }
        public int ManualOutTray { get; set; }
        public int AutoInQuantity { get; set; }
        public int AutoInTray { get; set; }
        public int ManualInQuantity { get; set; }
        public int ManualInTray { get; set; }
    }

    [SugarTable("USI_AMHS.MATERIALSTOCK")]
    public class MaterialStockHistory : MaterialStock
    {
        public DateTime CreateTime { get; set; }
    }
    public class AutoReelStockModel
    {
        public string LINE { get; set; }
        public string MONUMBER { get; set; }
        public string COMPPN { get; set; }
        public int TRAYQTY { get; set; }
        public int MATERIALQTY { get; set; }
        public int OCCUPIED_TRAY { get; set; }
        public int OCCUPIED_QTY { get; set; }
        public int MANUALINTARY {  get; set; }
        public int MANUALINQTY { get; set; }
        public int MACHINEINTARY { get; set; }
        public int MACHINEINQTY { get; set; }
        public int MANUALOUTTARY { get; set; }
        public int MANUALOUTQTY { get; set; }
        public int MACHINEOUTTARY { get; set; }
        public int MACHINEOUTQTY { get; set; }
    }

    [SugarTable("USI_AMHS.IMCP_STOCK")]
    public class IMCPStockModel
    {
        public string ORDER { get; set; }
        public string LINE { get; set; }
        public string PN { get; set; }
        public int TARY_TYPE1 { get; set; }
        public int QTY_TYPE1 { get; set; }
        public int TARY_TYPE2 { get; set; }
        public int QTY_TYPE2 { get; set; }
        public int TARY_TYPE3 { get; set; }
        public int QTY_TYPE3 { get; set; }
        public int PVS_TRAY { get; set; }
        public int PVS_QTY { get; set; }
    }

    public class DownTimeRateModel
    {
        public string DATESTR { get; set; }
        public string RATE {  get; set; }
    }
}