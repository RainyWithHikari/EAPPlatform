
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using EAPPlatform.Model.EAP;
using EAPPlatform.Model;

namespace EAPPlatform.ViewModel.EAP.EquipmentTypeVMs
{
    public partial class EquipmentTypeBatchVM : BaseBatchVM<EquipmentType, EquipmentType_BatchEdit>
    {
        public EquipmentTypeBatchVM()
        {
            ListVM = new EquipmentTypeListVM();
            LinkedVM = new EquipmentType_BatchEdit();
        }

        public override bool DoBatchEdit()
        {
            
            return base.DoBatchEdit();
        }
    }

	/// <summary>
    /// Class to define batch edit fields
    /// </summary>
    public class EquipmentType_BatchEdit : BaseVM
    {

        
        public List<string> EAPEquipmentTypeBTempSelected { get; set; }
        [Display(Name = "_Model._EquipmentType._Name")]
        public string Name { get; set; }

        protected override void InitVM()
        {
           
        }
    }

}