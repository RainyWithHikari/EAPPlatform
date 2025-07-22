using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using Microsoft.EntityFrameworkCore;

using EAPPlatform.ViewModel.EAP.EquipmentTypeVMs;
using EAPPlatform.Model.EAP;
using EAPPlatform.Model;
namespace EAPPlatform.ViewModel.EAP.EquipmentTypeConfigurationVMs
{
    public partial class EquipmentTypeConfigurationVM : BaseCRUDVM<EquipmentTypeConfiguration>
    {
        
        public List<string> EAPEquipmentTypeConfigurationFTempSelected { get; set; }

        public EquipmentTypeConfigurationVM()
        {
            
            SetInclude(x => x.TypeName);

        }

        protected override void InitVM()
        {
            

        }

        public override DuplicatedInfo<EquipmentTypeConfiguration> SetDuplicatedCheck()
        {
            return null;

        }

        public override async Task DoAddAsync()        
        {
            
            await base.DoAddAsync();

        }

        public override async Task DoEditAsync(bool updateAllFields = false)
        {
            
            await base.DoEditAsync();

        }

        public override async Task DoDeleteAsync()
        {
            await base.DoDeleteAsync();

        }
    }
}
