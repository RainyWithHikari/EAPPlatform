using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAPPlatform.Model.EAP
{
    [Table("EQUIPMENTREALTIMESTATUS")]
    public class EquipmentRealtimeStatus
    {
        [Key]
        [StringLength(100)]
        [Column("EQID")]
        public string EQID { get; set; }

        [StringLength(50)]
        [Column("STATUS")]
        public string STATUS { get; set; }

        [Column("UPDATETIME")]
        public DateTime UPDATETIME { get; set; }

    }
}
