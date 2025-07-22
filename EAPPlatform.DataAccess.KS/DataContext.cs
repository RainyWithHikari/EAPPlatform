using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using WalkingTec.Mvvm.Core;
using EAPPlatform.Model;
using System.Collections.Generic;
using WalkingTec.Mvvm.Core.Extensions;

using EAPPlatform.Model.EAP;
using EAPPlatform.Model.CustomVMs;

namespace EAPPlatform.DataAccess.KS
{
    public class DataContext : FrameworkContext
    {
        public DbSet<FrameworkUser> FrameworkUsers { get; set; }
        public DbSet<Equipment> Equipments { get; set; }
        public DbSet<EquipmentType> EquipmentTypes { get; set; }
        public DbSet<EquipmentConfiguration> EquipmentConfigurations { get; set; }
        public DbSet<EquipmentTypeConfiguration> EquipmentTypeConfigurations { get; set; }
        public DbSet<TestModel> TestModels { get; set; }
        public DbSet<ClientLog> ClientLog { get; set; }
        public DbSet<EquipmentAlarm> EquipmentAlarm { get; set; }

        public DbSet<EquipmentRunrate> EquipmentRunrate { get; set; }
        public DbSet<EquipmentRunrateHistory> EquipmentRunrateHistory { get; set; }
        public DbSet<EquipmentMTBA> EquipmentMTBA { get; set; }
        public DbSet<EquipmentMTBAHistory> EquipmentMTBAHistory { get; set; }
        public DbSet<EquipmentStatus> EquipmentStatuses { get; set; }
        public DbSet<EquipmentRealtimeStatus> EquipmentRealtimeStatus { get; set; }
        public DbSet<EquipmentParamsHistoryCalculate> EquipmentParamsHistoryCalculate { get; set; }
        public DbSet<EquipmentParamsHistoryRaw> EquipmentParamsHistoryRaw { get; set; }
        public DbSet<EquipmentParamsRealtime> EquipmentParamsRealtime { get; set; }
        public DbSet<AlarmPositions> AlarmPositions { get; set; }
        // Views
        public DbSet<V_EquipmentStatus> V_EquipmentStatus { get; set; }

        public DbSet<EquipmentTypeRole> EquipmentTypeRoles { get; set; }

        public DataContext(CS cs)
             : base(cs)
        {
        }

        public DataContext(string cs, DBTypeEnum dbtype) : base(cs, dbtype)
        {

        }

        public DataContext(string cs, DBTypeEnum dbtype, string version = null) : base(cs, dbtype, version)
        {

        }
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public override async Task<bool> DataInit(object allModules, bool IsSpa)
        {
            var state = await base.DataInit(allModules, IsSpa);
            bool emptydb = false;
            try
            {
                emptydb = Set<FrameworkUser>().Count() == 0 && Set<FrameworkUserRole>().Count() == 0;
            }
            catch { }
            if (state == true || emptydb == true)
            {
                //when state is true, means it's the first time EF create database, do data init here
                //当state是true的时候，表示这是第一次创建数据库，可以在这里进行数据初始化
                var user = new FrameworkUser
                {
                    ITCode = "admin",
                    Password = Utils.GetMD5String("000000"),
                    IsValid = true,
                    Name = "Admin",
                                        
                };

                var userrole = new FrameworkUserRole
                {
                    UserCode = user.ITCode,
                    RoleCode = "001"
                };
                await SaveChangesAsync();
                Set<FrameworkUser>().Add(user);
                Set<FrameworkUserRole>().Add(userrole);
                await SaveChangesAsync();
                try{
                    Dictionary<string, List<object>> data = new Dictionary<string, List<object>>();
                    }catch{}
            }
            return state;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<V_EquipmentStatus>();//忽略数据迁移
            modelBuilder.Entity<V_EquipmentStatus>(entity =>
            {
                entity.HasNoKey();
                entity.ToView("V_EQUIPMENTSTATUS");
            });
            modelBuilder.Entity<EquipmentStatus>(entity =>
            {
                entity.HasNoKey();
            });
            modelBuilder.Entity<EquipmentParamsHistoryCalculate>(entity =>
            {
                entity.HasNoKey();
                entity.ToTable("EQUIPMENTPARAMSHISCAL");
            });
            modelBuilder.Entity<EquipmentParamsHistoryRaw>(entity =>
            {
                entity.HasNoKey();
            });
            modelBuilder.Entity<AlarmPositions>(entity =>
            {
                entity.HasNoKey();
            });
            base.OnModelCreating(modelBuilder);

        }

        private void SetTestData(Type modelType, Dictionary<string, List<object>> data, int count = 100)
        {
            if (data.ContainsKey(modelType.FullName) && data[modelType.FullName].Count>=count)
            {
                return;
            }
            using (var dc = this.CreateNew())
            {
                Random r = new Random();
                data[modelType.FullName] = new List<object>();
                int retry = 0;
                List<string> ids = new List<string>();
                for (int i = 0; i < count; i++)
                {
                    var modelprops = modelType.GetRandomValuesForTestData();
                    var newobj = modelType.GetConstructor(Type.EmptyTypes).Invoke(null);
                    var idvalue = modelprops.Where(x => x.Key == "ID").Select(x=>x.Value).SingleOrDefault();
                    if (idvalue != null )
                    {
                        if (ids.Contains(idvalue.ToLower()) == false)
                        {
                            ids.Add(idvalue.ToLower());
                        }
                        else
                        {
                            retry++;
                            i--;
                            if (retry > count)
                            {
                                break;
                            }
                            continue;
                        }
                    }
                    foreach (var pro in modelprops)
                    {
                        if (pro.Value == "$fk$")
                        {
                            var fktype = modelType.GetSingleProperty(pro.Key[0..^2])?.PropertyType;
                            if (fktype != modelType && typeof(TopBasePoco).IsAssignableFrom(fktype)==true)
                            {
                                try
                                {
                                    SetTestData(fktype, data, count);
                                    newobj.SetPropertyValue(pro.Key, (data[fktype.FullName][r.Next(0, data[fktype.FullName].Count)] as TopBasePoco).GetID());
                                }
                                catch { }
                            }
                        }
                        else
                        {
                            var v = pro.Value;
                            if (v.StartsWith("\""))
                            {
                                v = v[1..];
                            }
                            if (v.EndsWith("\""))
                            {
                                v = v[..^1];
                            }
                            newobj.SetPropertyValue(pro.Key, v);
                        }
                    }
                    if(modelType == typeof(FileAttachment))
                    {
                        newobj.SetPropertyValue("Path", "./wwwroot/logo.png");
                        newobj.SetPropertyValue("SaveMode", "local");
                        newobj.SetPropertyValue("Length", 16728);
                    }
                    if (typeof(IBasePoco).IsAssignableFrom(modelType))
                    {
                        newobj.SetPropertyValue("CreateTime", DateTime.Now);
                        newobj.SetPropertyValue("CreateBy", "admin");
                    }
                    if (typeof(IPersistPoco).IsAssignableFrom(modelType))
                    {
                        newobj.SetPropertyValue("IsValid",true);
                    }
                    try
                    {
                        (dc as DbContext).Add(newobj);
                        data[modelType.FullName].Add(newobj);
                    }
                    catch
                    {
                        retry++;
                        i--;
                        if(retry > count)
                        {
                            break;
                        }
                    }
                }
                try
                {
                    dc.SaveChanges();
                }
                catch { }
            }
        }


    }


  


    /// <summary>
    /// DesignTimeFactory for EF Migration, use your full connection string,
    /// EF will find this class and use the connection defined here to run Add-Migration and Update-Database
    /// </summary>
    public class DataContextFactory : IDesignTimeDbContextFactory<DataContext>
    {
        public DataContext CreateDbContext(string[] args)
        {
            return new DataContext("User Id=EAP;Password=EAP123;Pooling = True;Max Pool Size = 10;Min Pool Size = 1;Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=192.168.110.180)(PORT=1521)))(CONNECT_DATA=(SERVICE_NAME=SPC)))", DBTypeEnum.Oracle);
        }
    }

}