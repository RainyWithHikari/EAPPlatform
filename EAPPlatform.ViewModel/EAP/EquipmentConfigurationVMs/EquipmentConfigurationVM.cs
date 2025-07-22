using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using Microsoft.EntityFrameworkCore;

using EAPPlatform.ViewModel.EAP.EquipmentVMs;
using EAPPlatform.Model.EAP;
using EAPPlatform.Model;
namespace EAPPlatform.ViewModel.EAP.EquipmentConfigurationVMs
{
    public partial class EquipmentConfigurationVM : BaseCRUDVM<EquipmentConfiguration>
    {
        
        public List<string> EAPEquipmentConfigurationFTempSelected { get; set; }

        public EquipmentConfigurationVM()
        {
            
            SetInclude(x => x.EQID);

        }

        protected override void InitVM()
        {
            

        }

        public override DuplicatedInfo<EquipmentConfiguration> SetDuplicatedCheck()
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
