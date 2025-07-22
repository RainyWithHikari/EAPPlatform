using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EAP.Dashboard.Models
{
    public class XmSelectModel
    {
        public string name {  get; set; }
        public string value { get; set; }
    }

    public class DeviceStatusModel{
        public string EQID { get; set; }
        public string STATUS { get; set; }
        public DateTime DATETIME { get; set; }
    }

    public class StatusDurationModel
    {
        public string EQID { get; set; }
        public string STATUS { get; set; }
        public DateTime DATETIME { get; set; }
        public DateTime NEXT_STSTUS_DATETIME { get; set; }
        public double DURATION
        {
            get
            {
                return (NEXT_STSTUS_DATETIME - DATETIME).TotalMinutes;
            }
        }
    }

    public class StatusRateModel
    {
        public string EQID { get; set; }
        public string STATUSNAME { get; set; }
        public double RATE { get; set; }
        public DateTime DATE { get; set; }
    }

    public class OutputRequestModel
    {
        public RequestInfoModel Condition { get; set; }
    }

    public class RequestInfoModel
    {
        public string FUNCTION { get; set; }
        public string GROUP_NAME { get; set; }
        public string FROM_TIME { get; set; }
        public string END_TIME { get; set; }
    }

    public class RequestResultModel
    {
        public ResultInfoModel RESULT { get; set; }
    }

    public class ResultInfoModel
    {
        public string RESULT { get; set; }
        public string RESULT_MESSAGE { get; set; }
        public string GROUP_NAME { get; set; }
        public List<ResDataModel> RES_DATA { get; set; }
    }

    public class ResDataModel
    {
        public string NO {  get; set; }
        public string STATION_NAME { get; set; }
        public string MODEL_NAME { get; set; }
        public string FAMILY_CODE { get; set; }
        public string OEE_CNT { get; set; }
    }

    public class DailyOutputDataModel: ResDataModel
    {
        public DateTime DATE { get; set; }
    }
}