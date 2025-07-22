using System;
using System.Collections.Generic;

namespace EAPPlatform.DataAccess.Models
{
    public partial class Equipmenttype
    {
        public Equipmenttype()
        {
            Equipment = new HashSet<Equipment>();
            Equipmenttypeconfigurations = new HashSet<Equipmenttypeconfiguration>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsValid { get; set; }
        public DateTime? CreateTime { get; set; }
        public string CreateBy { get; set; }
        public DateTime? UpdateTime { get; set; }
        public string UpdateBy { get; set; }

        public virtual ICollection<Equipment> Equipment { get; set; }
        public virtual ICollection<Equipmenttypeconfiguration> Equipmenttypeconfigurations { get; set; }
    }
}
