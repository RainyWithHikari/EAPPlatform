
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using EAPPlatform.Model.EAP;
using EAPPlatform.Model;
namespace EAPPlatform.ViewModel.EAP.EquipmentTypeVMs
{
    public partial class EquipmentTypeSearcher : BaseSearcher
    {
        
        public List<string> EAPEquipmentTypeSTempSelected { get; set; }
        [Display(Name = "_Model._EquipmentType._Name")]
        public string Name { get; set; }

        protected override void InitVM()
        {
            
        }
    }

}