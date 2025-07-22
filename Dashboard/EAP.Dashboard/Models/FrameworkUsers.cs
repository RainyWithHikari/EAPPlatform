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
    [SugarTable("FrameworkUsers")]

    public class FrameworkUsers
    {
        [SugarColumn(ColumnName = "ID")]
        public byte[] ID { get; set; }

        public string Email { get; set; }
        [SugarColumn(ColumnName = "ITCode")]
        public string ITCode { get; set; }

        [SugarColumn(ColumnName = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [SugarColumn(ColumnName = "Name")]
        public string Name { get; set; }

        [SugarColumn(ColumnName = "CreateTime")]
        public DateTime CreateTime { get; set; }
        [SugarColumn(ColumnName = "CreateBy")]
        public string CreateBy { get; set; }
        [SugarColumn(ColumnName = "IsValid")]
        public bool IsValid { get; set; }
    }


}