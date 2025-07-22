
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using EAPPlatform.Model.EAP;
using EAPPlatform.Model;
namespace EAPPlatform.ViewModel.EAP.EquipmentVMs
{
    public partial class EquipmentSearcher : BaseSearcher
    {
        
        public List<string> EAPEquipmentSTempSelected { get; set; }
        [Display(Name = "_Model._Equipment._EQID")]
        public string EQID { get; set; }
        [Display(Name = "_Model._Equipment._EquipmentType")]
        public Guid? EquipmentTypeId { get; set; }
        public List<ComboSelectListItem> AllEquipmentTypes { get; set; }
        [Display(Name = "_Model._Equipment._Name")]
        public string Name { get; set; }

        protected override void InitVM()
        {
            

        }
    }

}