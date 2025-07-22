using System;
using System.Collections.Generic;

namespace EAPPlatform.DataAccess.Models
{
    public partial class Equipmentstatus
    {
        public string Eqid { get; set; }
        public string Eqtype { get; set; }
        public string Status { get; set; }
        public DateTime? Datetime { get; set; }
    }
}
