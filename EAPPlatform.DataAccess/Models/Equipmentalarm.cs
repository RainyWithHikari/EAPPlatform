using System;
using System.Collections.Generic;

namespace EAPPlatform.DataAccess.Models
{
    public partial class Equipmentalarm
    {
        public Guid Id { get; set; }
        public string Eqid { get; set; }
        public string Alarmcode { get; set; }
        public string Alarmtext { get; set; }
        public string Alarmsource { get; set; }
        public DateTime? Alarmtime { get; set; }
        public bool Alarmset { get; set; }
    }
}
