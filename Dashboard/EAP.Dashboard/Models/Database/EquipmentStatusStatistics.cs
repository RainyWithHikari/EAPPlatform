using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAP.Models.Database
{ 
    [SugarTable("EQUIPMENTSTATUSSTATISTICS")]
    public class EquipmentStatusStatistics
    {
        [SugarColumn(ColumnName = "NAME")]
        public string name { get; set; }

        [SugarColumn(ColumnName = "VALUE")]
        public object value { get; set; }
        [SugarColumn(ColumnName = "ITEMSTYLE")]
        public string itemStyle { get; set; }

        [SugarColumn(ColumnName = "DURATION")]
        public TimeSpan duration { get; set; }
    }
}
