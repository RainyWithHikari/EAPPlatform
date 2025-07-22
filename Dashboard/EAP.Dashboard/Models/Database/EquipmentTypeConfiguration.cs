using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace EAP.Models.Database
{
    /// <summary>
    /// 设备类型配置
    /// </summary>
	[SugarTable("EQUIPMENTTYPECONFIGURATION")]

    public class EquipmentTypeConfiguration
    {


        public byte[] ID { get; set; }

        [SugarColumn(ColumnName = "TYPENAMEID")]
        public byte[] TypeNameId { get; set; }

        [SugarColumn(ColumnName = "CONFIGURATIONITEM")]
        public string ConfigurationItem { get; set; }

        [SugarColumn(ColumnName = "CONFIGURATIONVALUE")]
        public string ConfigurationValue { get; set; }

        [SugarColumn(ColumnName = "CONFIGURATIONNAME")]
        public string ConfigurationName { get; set; }

	}

}
