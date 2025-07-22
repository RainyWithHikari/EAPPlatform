using System;
using System.Collections.Generic;

namespace EAPPlatform.DataAccess.Models
{
    public partial class Clientlog
    {
        public Guid Id { get; set; }
        public DateTime Datetime { get; set; }
        public string Eqid { get; set; }
        public string Logtype { get; set; }
        public string Loglevel { get; set; }
        public string Logcontent { get; set; }
    }
}
