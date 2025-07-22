using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAPPlatform.Model.EAP
{
    [Table("EQUIPMENTSTATUS")]

    public class EquipmentStatus
    {
        [StringLength(100)]
        [Column("EQID")]
        public string EQID { get; set; }

        [StringLength(100)]
        [Column("EQTYPE")]
        public string EQTYPE { get; set; }


        [StringLength(50)]
        [Column("STATUS")]
        public string STATUS { get; set; }

        [Column("DATETIME")]
        public DateTime DATETIME { get; set; }

    }
    public class OTMSStatus
    {
        public string EQID { get; set; }
        public string CHAMBERID { get; set; }
        public string STATUS {  get; set; }

        public string TEMPERTURE { get; set; }
        public string LOCATION { get; set; }
    }

}
