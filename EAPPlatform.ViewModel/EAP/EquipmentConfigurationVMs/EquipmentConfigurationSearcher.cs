
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using EAPPlatform.Model.EAP;
using EAPPlatform.Model;
namespace EAPPlatform.ViewModel.EAP.EquipmentConfigurationVMs
{
    public partial class EquipmentConfigurationSearcher : BaseSearcher
    {
        
        public List<string> EAPEquipmentConfigurationSTempSelected { get; set; }
        [Display(Name = "_Model._EquipmentConfiguration._EQID")]
        public Guid? EQIDId { get; set; }
        public List<ComboSelectListItem> AllEQIDs { get; set; }
        [Display(Name = "_Model._EquipmentConfiguration._ConfigurationItem")]
        public string ConfigurationItem { get; set; }
        [Display(Name = "_Model._EquipmentConfiguration._ConfigurationValue")]
        public string ConfigurationValue { get; set; }

        protected override void InitVM()
        {
            

        }
    }

}