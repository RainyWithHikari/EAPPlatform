using System;
using System.Collections.Generic;

namespace EAPPlatform.DataAccess.Models
{
    public partial class FrameworkUserRole
    {
        public Guid Id { get; set; }
        public string UserCode { get; set; }
        public string RoleCode { get; set; }
        public DateTime? CreateTime { get; set; }
        public string CreateBy { get; set; }
        public DateTime? UpdateTime { get; set; }
        public string UpdateBy { get; set; }
    }
}
