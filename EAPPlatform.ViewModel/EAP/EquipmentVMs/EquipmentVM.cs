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
namespace EAPPlatform.ViewModel.EAP.EquipmentVMs
{
    public partial class EquipmentVM : BaseCRUDVM<Equipment>
    {
        
        public List<string> EAPEquipmentFTempSelected { get; set; }

        public EquipmentVM()
        {
            
            SetInclude(x => x.EquipmentType);

        }

        protected override void InitVM()
        {
            

        }

        public override DuplicatedInfo<Equipment> SetDuplicatedCheck()
        {
            var rv = CreateFieldsInfo(SimpleField(x => x.EQID));
            return rv;

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
