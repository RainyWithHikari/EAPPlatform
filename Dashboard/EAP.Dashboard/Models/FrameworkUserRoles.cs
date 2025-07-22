using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EAP.Dashboard.Models
{
    [SugarTable("FrameworkUserRoles")]
    public class FrameworkUserRoles
    {
        public byte[] ID { get; set; }
        [SugarColumn(ColumnName = "UserCode")]
        public string UserCode { get; set; }

        [SugarColumn(ColumnName = "RoleCode")]
        public string RoleCode { get; set; }

        [SugarColumn(ColumnName = "CreateTime")]
        public DateTime CreateTime { get; set; }
        [SugarColumn(ColumnName = "CreateBy")]
        public string CreateBy { get; set; }

    }
}