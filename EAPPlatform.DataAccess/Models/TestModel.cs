using System;
using System.Collections.Generic;

namespace EAPPlatform.DataAccess.Models
{
    public partial class TestModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime? CreateTime { get; set; }
        public string CreateBy { get; set; }
        public DateTime? UpdateTime { get; set; }
        public string UpdateBy { get; set; }
        public string Name1 { get; set; }
    }
}
