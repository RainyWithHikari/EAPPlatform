using System;
using System.Collections.Generic;

namespace EAPPlatform.DataAccess.Models
{
    public partial class VEquipmentstatus
    {
        public string Eqid { get; set; }
        public decimal Mtba { get; set; }
        public decimal Runrate { get; set; }
        public string Status { get; set; }
    }
}
