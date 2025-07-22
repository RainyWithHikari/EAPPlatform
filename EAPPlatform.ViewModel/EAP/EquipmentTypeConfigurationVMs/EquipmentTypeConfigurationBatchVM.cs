
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

namespace EAPPlatform.ViewModel.EAP.EquipmentTypeConfigurationVMs
{
    public partial class EquipmentTypeConfigurationBatchVM : BaseBatchVM<EquipmentTypeConfiguration, EquipmentTypeConfiguration_BatchEdit>
    {
        public EquipmentTypeConfigurationBatchVM()
        {
            ListVM = new EquipmentTypeConfigurationListVM();
            LinkedVM = new EquipmentTypeConfiguration_BatchEdit();
        }

        public override bool DoBatchEdit()
        {
            
            return base.DoBatchEdit();
        }
    }

	/// <summary>
    /// Class to define batch edit fields
    /// </summary>
    public class EquipmentTypeConfiguration_BatchEdit : BaseVM
    {

        
        public List<string> EAPEquipmentTypeConfigurationBTempSelected { get; set; }
        [Display(Name = "_Model._EquipmentTypeConfiguration._TypeName")]
        public Guid? TypeNameId { get; set; }
        [Display(Name = "_Model._EquipmentTypeConfiguration._ConfigurationItem")]
        public string ConfigurationItem { get; set; }
        [Display(Name = "_Model._EquipmentTypeConfiguration._ConfigurationValue")]
        public string ConfigurationValue { get; set; }

        protected override void InitVM()
        {
           
        }
    }

}