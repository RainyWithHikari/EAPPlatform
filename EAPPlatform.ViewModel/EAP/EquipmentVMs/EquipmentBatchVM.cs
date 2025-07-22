
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

namespace EAPPlatform.ViewModel.EAP.EquipmentVMs
{
    public partial class EquipmentBatchVM : BaseBatchVM<Equipment, Equipment_BatchEdit>
    {
        public EquipmentBatchVM()
        {
            ListVM = new EquipmentListVM();
            LinkedVM = new Equipment_BatchEdit();
        }

        public override bool DoBatchEdit()
        {
            
            return base.DoBatchEdit();
        }
    }

	/// <summary>
    /// Class to define batch edit fields
    /// </summary>
    public class Equipment_BatchEdit : BaseVM
    {

        
        public List<string> EAPEquipmentBTempSelected { get; set; }
        [Display(Name = "_Model._Equipment._EquipmentType")]
        public Guid? EquipmentTypeId { get; set; }
        [Display(Name = "_Model._Equipment._Name")]
        public string Name { get; set; }

        protected override void InitVM()
        {
           
        }
    }

}