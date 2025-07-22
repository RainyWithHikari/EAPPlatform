using System;
using System.Collections.Generic;

namespace EAPPlatform.DataAccess.Models
{
    public partial class FrameworkGroup
    {
        public Guid Id { get; set; }
        public string GroupCode { get; set; }
        public string GroupName { get; set; }
        public string GroupRemark { get; set; }
        public string TenantCode { get; set; }
        public DateTime? CreateTime { get; set; }
        public string CreateBy { get; set; }
        public DateTime? UpdateTime { get; set; }
        public string UpdateBy { get; set; }
    }
}
