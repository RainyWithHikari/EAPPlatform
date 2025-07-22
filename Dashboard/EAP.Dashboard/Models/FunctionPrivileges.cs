using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EAP.Dashboard.Models
{
    [SugarTable("FunctionPrivileges")]
    public class FunctionPrivileges
    {
        [SugarColumn(ColumnName = "ID")]
        public byte[] ID { get; set; }
        [SugarColumn(ColumnName = "RoleCode")]
        public string RoleCode { get; set; }

        [SugarColumn(ColumnName = "MenuItemId")]
        public byte[] MenuItemId { get; set; }
       
    }
}