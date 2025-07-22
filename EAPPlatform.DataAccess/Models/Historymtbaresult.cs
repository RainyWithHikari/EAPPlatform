using System;
using System.Collections.Generic;

namespace EAPPlatform.DataAccess.Models
{
    public partial class Historymtbaresult
    {
        public Guid Id { get; set; }
        public string Eqid { get; set; }
        public decimal Mtbavalue { get; set; }
        public DateTime Checktime { get; set; }
        public string Shift { get; set; }
    }
}
