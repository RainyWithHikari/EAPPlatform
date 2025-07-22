using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace EAP.Dashboard.Models
{
    [SugarTable("FrameworkRoles")]

    public class FrameworkRoles
    {
        [SugarColumn(ColumnName = "ID")]
        public byte[] ID { get; set; }


        [SugarColumn(ColumnName = "RoleCode")]
        public string RoleCode { get; set; }

        [SugarColumn(ColumnName = "RoleName")]
     
        public string RoleName { get; set; }

        [SugarColumn(ColumnName = "CreateTime")]
        public DateTime CreateTime { get; set; }
        [SugarColumn(ColumnName = "CreateBy")]
        public string CreateBy { get; set; }
    }


}