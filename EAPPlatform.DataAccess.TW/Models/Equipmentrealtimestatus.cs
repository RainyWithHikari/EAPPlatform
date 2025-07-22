using System;
using System.Collections.Generic;

namespace EAPPlatform.DataAccess.Models
{
    public partial class Equipmentrealtimestatus
    {
        public string Eqid { get; set; }
        public string Status { get; set; }
        public DateTime? Updatetime { get; set; }
    }
}
