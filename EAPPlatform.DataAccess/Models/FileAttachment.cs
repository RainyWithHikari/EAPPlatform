using System;
using System.Collections.Generic;

namespace EAPPlatform.DataAccess.Models
{
    public partial class FileAttachment
    {
        public FileAttachment()
        {
            FrameworkUsers = new HashSet<FrameworkUser>();
        }

        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string FileExt { get; set; }
        public string Path { get; set; }
        public long Length { get; set; }
        public DateTime UploadTime { get; set; }
        public string SaveMode { get; set; }
        public byte[] FileData { get; set; }
        public string ExtraInfo { get; set; }
        public string HandlerInfo { get; set; }

        public virtual ICollection<FrameworkUser> FrameworkUsers { get; set; }
    }
}
