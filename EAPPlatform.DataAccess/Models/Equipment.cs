using System;
using System.Collections.Generic;

namespace EAPPlatform.DataAccess.Models
{
    public partial class Equipment
    {
        public Equipment()
        {
            Equipmentconfigurations = new HashSet<Equipmentconfiguration>();
        }

        public Guid Id { get; set; }
        public string Eqid { get; set; }
        public Guid? Equipmenttypeid { get; set; }
        public string Name { get; set; }
        public bool IsValid { get; set; }
        public DateTime? CreateTime { get; set; }
        public string CreateBy { get; set; }
        public DateTime? UpdateTime { get; set; }
        public string UpdateBy { get; set; }

        public virtual Equipmenttype Equipmenttype { get; set; }
        public virtual ICollection<Equipmentconfiguration> Equipmentconfigurations { get; set; }
    }
}
