using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace EAP.Models.Database
{
    /// <summary>
    /// 设备类型
    /// </summary>
	[SugarTable("EQUIPMENTTYPE")]
    public class EquipmentType
    {

        public byte[] ID { get; set; }

        [SugarColumn(ColumnName = "NAME")]
        public string Name { get; set; }

       
	}

}
