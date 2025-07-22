using System;
using System.Collections.Generic;

namespace EAPPlatform.DataAccess.Models
{
    public partial class PersistedGrant
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public string UserCode { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime Expiration { get; set; }
        public string RefreshToken { get; set; }
    }
}
