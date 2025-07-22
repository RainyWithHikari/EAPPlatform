
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

namespace EAPPlatform.ViewModel.EAP.EquipmentConfigurationVMs
{
    public partial class EquipmentConfigurationBatchVM : BaseBatchVM<EquipmentConfiguration, EquipmentConfiguration_BatchEdit>
    {
        public EquipmentConfigurationBatchVM()
        {
            ListVM = new EquipmentConfigurationListVM();
            LinkedVM = new EquipmentConfiguration_BatchEdit();
        }

        public override bool DoBatchEdit()
        {
            
            return base.DoBatchEdit();
        }
    }

	/// <summary>
    /// Class to define batch edit fields
    /// </summary>
    public class EquipmentConfiguration_BatchEdit : BaseVM
    {

        
        public List<string> EAPEquipmentConfigurationBTempSelected { get; set; }
        [Display(Name = "_Model._EquipmentConfiguration._EQID")]
        public Guid? EQIDId { get; set; }

        protected override void InitVM()
        {
           
        }
    }

}