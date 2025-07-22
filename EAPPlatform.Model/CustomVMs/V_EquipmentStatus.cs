using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAPPlatform.Model.CustomVMs
{
    public class V_EquipmentStatus
    {
        [Column("EQID")]
        public string EQID { get; set; }
        //[Column("PD")]
        //public string PD { get; set; }
        //[Column("LINE")]
        //public string LINE { get; set; }

        [Column("TYPE")]
        public string Type { get; set; }

        [Column("NAME")]
        public string Name { get; set; }

        [Column("MTBA")]
        public decimal Mtba { get; set; }

        [Column("RUNRATE")]
        public decimal RunRate { get; set; }

        [Column("STATUS")]
        public string Status { get; set; }
    }

    public class EquipmentProfile
    {
        public string EQID { get; set; }
        
        public string Status { get; set; }
        public string output { get; set; }
        public string yield { get; set; }

    }
}
