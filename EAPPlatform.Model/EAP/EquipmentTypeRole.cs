using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;

namespace EAPPlatform.Model.EAP
{
    [Table("EQUIPMENTTYPEROLE")]

    public class EquipmentTypeRole: TopBasePoco
    {
        public EquipmentType EquipmentType { get; set; }
        [Required]
        [Column("EQUIPMENTTYPEID")]
        public Guid? EquipmentTypeId { get; set; }

        public FrameworkRole FrameworkRole { get; set; }
        [Required]
        [Column("FRAMEWORKROLEID")]
        public Guid? FrameworkRoleId { get; set; }
    }
}
