using System;
using System.Collections.Generic;

namespace EAPPlatform.DataAccess.Models
{
    public partial class FrameworkRole
    {
        public Guid Id { get; set; }
        public string RoleCode { get; set; }
        public string RoleName { get; set; }
        public string RoleRemark { get; set; }
        public string TenantCode { get; set; }
        public DateTime? CreateTime { get; set; }
        public string CreateBy { get; set; }
        public DateTime? UpdateTime { get; set; }
        public string UpdateBy { get; set; }
    }
}
