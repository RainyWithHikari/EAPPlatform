using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using Microsoft.EntityFrameworkCore;

using EAPPlatform.Model.EAP;
using EAPPlatform.Model;
namespace EAPPlatform.ViewModel.EAP.EquipmentTypeVMs
{
    public partial class EquipmentTypeVM : BaseCRUDVM<EquipmentType>
    {

        public List<string> EAPEquipmentTypeFTempSelected { get; set; }

        public List<string> SelectedEquipmentTypeRole_IDs { get; set; }
        public EquipmentTypeVM()
        {

        }

        protected override void InitVM()
        {
            SelectedEquipmentTypeRole_IDs = DC.Set<EquipmentTypeRole>().Where(y => y.EquipmentTypeId == Entity.ID).Join(DC.Set<FrameworkRole>(), ur => ur.FrameworkRoleId, role => role.ID, (ur, role) => role.GetID().ToString()).ToList();
        }

        public override DuplicatedInfo<EquipmentType> SetDuplicatedCheck()
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

        public async Task DoEditPrivilegeAsync()
        {
            //TODO 修改设备类型角色权限
            var etid = Entity.ID;
            var deleteitems = DC.Set<EquipmentTypeRole>().AsNoTracking().Where(it => it.EquipmentTypeId == Entity.ID);
            foreach (var item in deleteitems)
            {
                DC.Set<EquipmentTypeRole>().Remove(item);
            }
            foreach (var roleid in SelectedEquipmentTypeRole_IDs)
            {
                try
                {
                    DC.AddEntity(new EquipmentTypeRole
                    {
                        EquipmentTypeId = Entity.ID,
                        FrameworkRoleId = new Guid(roleid)
                    });
                }
                catch (Exception ex)
                {

                    throw;
                }
            }
            DC.SaveChanges();
        }
    }
}
