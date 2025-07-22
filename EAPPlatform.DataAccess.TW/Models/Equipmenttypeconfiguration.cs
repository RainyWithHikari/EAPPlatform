using System;
using System.Collections.Generic;

namespace EAPPlatform.DataAccess.Models
{
    public partial class Equipmenttypeconfiguration
    {
        public Guid Id { get; set; }
        public Guid Typenameid { get; set; }
        public string Configurationitem { get; set; }
        public string Configurationvalue { get; set; }
        public string Configurationname { get; set; }
        public int? DisplayOrder { get; set; }
        public bool IsValid { get; set; }
        public DateTime? CreateTime { get; set; }
        public string CreateBy { get; set; }
        public DateTime? UpdateTime { get; set; }
        public string UpdateBy { get; set; }

        public virtual Equipmenttype Typename { get; set; }
    }
}
