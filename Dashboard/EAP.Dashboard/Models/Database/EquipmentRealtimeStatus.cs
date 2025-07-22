using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAP.Models.Database
{
    [SugarTable("EQUIPMENTREALTIMESTATUS")]
    public class EquipmentRealtimeStatus
    {
        [SugarColumn(ColumnName = "EQID",IsPrimaryKey =true)]
        public string EQID { get; set; }

        [SugarColumn(ColumnName = "STATUS")]
        public string Status { get; set; }

        [SugarColumn(ColumnName = "UPDATETIME")]
        public DateTime UpdateTime { get; set; }
    }
}
