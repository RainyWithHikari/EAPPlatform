using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EAP.Dashboard.Models
{
    [SugarTable("EQUIPMENTTYPEROLE")]
    public class EquipmentTypeRole
    {
        public byte[] ID { get; set; }
        public byte[] EQUIPMENTTYPEID { get; set; }
        public byte[] FRAMEWORKROLEID { get; set; }
    }
}