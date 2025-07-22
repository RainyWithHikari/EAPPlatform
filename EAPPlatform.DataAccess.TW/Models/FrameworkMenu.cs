using System;
using System.Collections.Generic;

namespace EAPPlatform.DataAccess.Models
{
    public partial class FrameworkMenu
    {
        public FrameworkMenu()
        {
            FunctionPrivileges = new HashSet<FunctionPrivilege>();
            InverseParent = new HashSet<FrameworkMenu>();
        }

        public Guid Id { get; set; }
        public string PageName { get; set; }
        public string ActionName { get; set; }
        public string ModuleName { get; set; }
        public bool FolderOnly { get; set; }
        public bool IsInherit { get; set; }
        public string ClassName { get; set; }
        public string MethodName { get; set; }
        public string Domain { get; set; }
        public bool ShowOnMenu { get; set; }
        public bool IsPublic { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsInside { get; set; }
        public string Url { get; set; }
        public string Icon { get; set; }
        public Guid? ParentId { get; set; }

        public virtual FrameworkMenu Parent { get; set; }
        public virtual ICollection<FunctionPrivilege> FunctionPrivileges { get; set; }
        public virtual ICollection<FrameworkMenu> InverseParent { get; set; }
    }
}
