using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EAP.Dashboard.Models
{
    [SugarTable("V_EQUIPMENTSTATUS")]
    public class V_EquipmentStatus
    {
      
        public string EQID { get; set; }
        //[Column("PD")]
        //public string PD { get; set; }
        //[Column("LINE")]
        //public string LINE { get; set; }

        [SugarColumn(ColumnName = "TYPE")]
        public string Type { get; set; }

        [SugarColumn(ColumnName = "NAME")]
        public string Name { get; set; }

        [SugarColumn(ColumnName = "MTBA")]
        public decimal Mtba { get; set; }

        [SugarColumn(ColumnName = "RUNRATE")]
        public decimal RunRate { get; set; }

        [SugarColumn(ColumnName = "STATUS")]
        public string Status { get; set; }
    }

    public class EquipmentProfile
    {
        public string EQID { get; set; }

        public string EQName { get; set; }

        public string Status { get; set; }
        public string output { get; set; }
        public string yield { get; set; }

    }
}