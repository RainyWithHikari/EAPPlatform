using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace EAP.Models.Database
{
    /// <summary>
    /// 设备配置
    /// </summary>
    [SugarTable("EQUIPMENTCONFIGURATION")]

    public class EquipmentConfiguration
    {
        [SugarColumn(IsPrimaryKey =true)]
        public byte[] ID { get; set; }

        [SugarColumn(ColumnName = "EQIDID")]
        public byte[] EQIDId { get; set; }

        [SugarColumn(ColumnName = "CONFIGURATIONITEM")]
        public string ConfigurationItem { get; set; }

        [SugarColumn(ColumnName = "CONFIGURATIONVALUE")]
        public string ConfigurationValue { get; set; }

        [SugarColumn(ColumnName = "CONFIGURATIONNAME")]
        public string ConfigurationName { get; set; }
        [SugarColumn(ColumnName = "ISVALID")]
        public bool IsValid { get; set; } = true;
        [SugarColumn(ColumnName = "ISREADONLY")]
        public bool IsReadOnly { get; set; }
    }

}
