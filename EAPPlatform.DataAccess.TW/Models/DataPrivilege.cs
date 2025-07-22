using System;
using System.Collections.Generic;

namespace EAPPlatform.DataAccess.Models
{
    public partial class DataPrivilege
    {
        public Guid Id { get; set; }
        public string UserCode { get; set; }
        public string GroupCode { get; set; }
        public string TableName { get; set; }
        public string RelateId { get; set; }
        public string Domain { get; set; }
        public DateTime? CreateTime { get; set; }
        public string CreateBy { get; set; }
        public DateTime? UpdateTime { get; set; }
        public string UpdateBy { get; set; }
    }
}
