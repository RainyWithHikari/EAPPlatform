using System;
using System.Collections.Generic;

namespace EAPPlatform.DataAccess.Models
{
    public partial class ActionLog
    {
        public Guid Id { get; set; }
        public string ModuleName { get; set; }
        public string ActionName { get; set; }
        public string Itcode { get; set; }
        public string ActionUrl { get; set; }
        public DateTime ActionTime { get; set; }
        public double Duration { get; set; }
        public string Remark { get; set; }
        public string Ip { get; set; }
        public int LogType { get; set; }
        public DateTime? CreateTime { get; set; }
        public string CreateBy { get; set; }
        public DateTime? UpdateTime { get; set; }
        public string UpdateBy { get; set; }
    }
}
