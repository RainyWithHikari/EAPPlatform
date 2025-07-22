using System;
using SqlSugar;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace EAP.Models.Database
{
    [SugarTable("EQUIPMENT")]
    public class Equipment
    {
        public byte[] ID { get; set; }

        [SugarColumn(ColumnName = "EQID")]
        public string EQID { get; set; }

        [SugarColumn(ColumnName = "EQUIPMENTTYPEID")]
        public byte[] EquipmentTypeId { get; set; }

        [SugarColumn(ColumnName = "NAME")]
        public string Name { get; set; }

       

        [SugarColumn(ColumnName = "SORT")]
        public int Sort { get; set; }

    }
}
