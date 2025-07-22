using System;
using System.Collections.Generic;

namespace EAPPlatform.DataAccess.Models
{
    public partial class FrameworkUser
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public int? Gender { get; set; }
        public string CellPhone { get; set; }
        public string HomePhone { get; set; }
        public string Address { get; set; }
        public string ZipCode { get; set; }
        public DateTime? CreateTime { get; set; }
        public string CreateBy { get; set; }
        public DateTime? UpdateTime { get; set; }
        public string UpdateBy { get; set; }
        public string Itcode { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public bool IsValid { get; set; }
        public Guid? PhotoId { get; set; }
        public string TenantCode { get; set; }

        public virtual FileAttachment Photo { get; set; }
    }
}
