using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EAP.Dashboard.Models
{
    [SugarTable("FrameworkMenus")]
    public class FrameworkMenus
    {
        [SugarColumn(ColumnName = "ID")]
        public byte[] ID { get; set; }
        [SugarColumn(ColumnName = "MethodName")]
        public string MethodName { get; set; }

        [SugarColumn(ColumnName = "ModuleName")]
        public string ModuleName { get; set; }
    }
}