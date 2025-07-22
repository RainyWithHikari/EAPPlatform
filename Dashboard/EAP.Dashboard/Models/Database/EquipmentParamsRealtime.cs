using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace EAP.Models.Database
{
    [SugarTable("EQUIPMENTPARAMSREALTIME")]
    public class EquipmentParamsRealtime
    {
        [SugarColumn(ColumnName = "ID", IsPrimaryKey =true)]
        public byte[] ID { get; set; }
        [SugarColumn(ColumnName = "EQID")]
        public string EQID { get; set; }
        [SugarColumn(ColumnName = "SVID")]
        public string SVID { get; set; }

        [SugarColumn(ColumnName = "NAME")]
        public string Name { get; set; }

        [SugarColumn(ColumnName = "VALUE")]
        public string Value { get; set; }

        [SugarColumn(ColumnName = "UPDATETIME")]
        public DateTime UpdateTime { get; set; }
    }
}
