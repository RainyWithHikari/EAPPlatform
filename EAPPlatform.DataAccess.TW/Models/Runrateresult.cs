using System;
using System.Collections.Generic;

namespace EAPPlatform.DataAccess.Models
{
    public partial class Runrateresult
    {
        public decimal Runratevalue { get; set; }
        public DateTime Checktime { get; set; }
        public string Eqid { get; set; }
    }
}
