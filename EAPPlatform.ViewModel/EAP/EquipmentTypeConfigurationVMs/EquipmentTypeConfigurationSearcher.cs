
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using EAPPlatform.Model.EAP;
using EAPPlatform.Model;
namespace EAPPlatform.ViewModel.EAP.EquipmentTypeConfigurationVMs
{
    public partial class EquipmentTypeConfigurationSearcher : BaseSearcher
    {
        
        public List<string> EAPEquipmentTypeConfigurationSTempSelected { get; set; }
        [Display(Name = "_Model._EquipmentTypeConfiguration._TypeName")]
        public Guid? TypeNameId { get; set; }
        public List<ComboSelectListItem> AllTypeNames { get; set; }
        [Display(Name = "_Model._EquipmentTypeConfiguration._ConfigurationItem")]
        public string ConfigurationItem { get; set; }
        [Display(Name = "_Model._EquipmentTypeConfiguration._ConfigurationValue")]
        public string ConfigurationValue { get; set; }

        protected override void InitVM()
        {
            

        }
    }

}