using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAPPlatform.Model.EAP
{
    public class DetailChartData
    {
        public string name { get; set; }

        public object[] value { get; set; }

        public itemStyle itemStyle { get; set; }

        public TimeSpan duration { get; set; }
        public string alarmtext { get; set; }
    }
    public class itemStyle
    {
        public normal normal { get; set; }
    }

    public class normal
    {
        public string color { get; set; }
    }

    public class DetailChartDataForTrip
    {
        public string name { get; set; }
        public string start { get; set; }
        public string end { get; set; }


        public string duration { get; set; }
    }
}
