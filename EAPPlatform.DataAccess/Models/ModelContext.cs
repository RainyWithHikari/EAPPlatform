using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EAPPlatform.DataAccess.Models
{
    public partial class ModelContext : DbContext
    {
        public ModelContext()
        {
        }

        public ModelContext(DbContextOptions<ModelContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ActionLog> ActionLogs { get; set; }
        public virtual DbSet<Clientlog> Clientlogs { get; set; }
        public virtual DbSet<DataPrivilege> DataPrivileges { get; set; }
        public virtual DbSet<Equipment> Equipment { get; set; }
        public virtual DbSet<Equipmentalarm> Equipmentalarms { get; set; }
        public virtual DbSet<Equipmentconfiguration> Equipmentconfigurations { get; set; }
        public virtual DbSet<Equipmentrealtimestatus> Equipmentrealtimestatuses { get; set; }
        public virtual DbSet<Equipmentstatus> Equipmentstatuses { get; set; }
        public virtual DbSet<Equipmenttype> Equipmenttypes { get; set; }
        public virtual DbSet<Equipmenttypeconfiguration> Equipmenttypeconfigurations { get; set; }
        public virtual DbSet<FileAttachment> FileAttachments { get; set; }
        public virtual DbSet<FrameworkGroup> FrameworkGroups { get; set; }
        public virtual DbSet<FrameworkMenu> FrameworkMenus { get; set; }
        public virtual DbSet<FrameworkRole> FrameworkRoles { get; set; }
        public virtual DbSet<FrameworkUser> FrameworkUsers { get; set; }
        public virtual DbSet<FrameworkUserGroup> FrameworkUserGroups { get; set; }
        public virtual DbSet<FrameworkUserRole> FrameworkUserRoles { get; set; }
        public virtual DbSet<FunctionPrivilege> FunctionPrivileges { get; set; }
        public virtual DbSet<Historymtbaresult> Historymtbaresults { get; set; }
        public virtual DbSet<Historyrunrateresult> Historyrunrateresults { get; set; }
        public virtual DbSet<Mtbaresult> Mtbaresults { get; set; }
        public virtual DbSet<PersistedGrant> PersistedGrants { get; set; }
        public virtual DbSet<Runrateresult> Runrateresults { get; set; }
        public virtual DbSet<TestModel> TestModels { get; set; }
        public virtual DbSet<VEquipmentstatus> VEquipmentstatuses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseOracle("User Id=EAP;Password=EAP123;Pooling = True;Max Pool Size = 10;Min Pool Size = 1;Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=192.168.40.168)(PORT=1521)))(CONNECT_DATA=(SERVICE_NAME=SMDADC)))");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("EAP");

            modelBuilder.Entity<ActionLog>(entity =>
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.ActionName).HasMaxLength(255);

                entity.Property(e => e.ActionTime).HasPrecision(7);

                entity.Property(e => e.ActionUrl).HasMaxLength(250);

                entity.Property(e => e.CreateBy).HasMaxLength(50);

                entity.Property(e => e.CreateTime).HasPrecision(7);

                entity.Property(e => e.Ip)
                    .HasMaxLength(50)
                    .HasColumnName("IP");

                entity.Property(e => e.Itcode)
                    .HasMaxLength(50)
                    .HasColumnName("ITCode");

                entity.Property(e => e.LogType).HasPrecision(10);

                entity.Property(e => e.ModuleName).HasMaxLength(255);

                entity.Property(e => e.UpdateBy).HasMaxLength(50);

                entity.Property(e => e.UpdateTime).HasPrecision(7);
            });

            modelBuilder.Entity<Clientlog>(entity =>
            {
                entity.ToTable("CLIENTLOG");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.Datetime)
                    .HasPrecision(7)
                    .HasColumnName("DATETIME");

                entity.Property(e => e.Eqid)
                    .HasMaxLength(100)
                    .HasColumnName("EQID");

                entity.Property(e => e.Logcontent).HasColumnName("LOGCONTENT");

                entity.Property(e => e.Loglevel)
                    .HasMaxLength(100)
                    .HasColumnName("LOGLEVEL");

                entity.Property(e => e.Logtype)
                    .HasMaxLength(100)
                    .HasColumnName("LOGTYPE");
            });

            modelBuilder.Entity<DataPrivilege>(entity =>
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.CreateBy).HasMaxLength(50);

                entity.Property(e => e.CreateTime).HasPrecision(7);

                entity.Property(e => e.TableName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.UpdateBy).HasMaxLength(50);

                entity.Property(e => e.UpdateTime).HasPrecision(7);
            });

            modelBuilder.Entity<Equipment>(entity =>
            {
                entity.ToTable("EQUIPMENT");

                entity.HasIndex(e => e.Equipmenttypeid, "IX_EQUIPMENT_EQUIPMENTTYPEID");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.CreateBy).HasMaxLength(50);

                entity.Property(e => e.CreateTime).HasPrecision(7);

                entity.Property(e => e.Eqid)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("EQID");

                entity.Property(e => e.Equipmenttypeid).HasColumnName("EQUIPMENTTYPEID");

                entity.Property(e => e.IsValid).HasPrecision(1);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("NAME");

                entity.Property(e => e.UpdateBy).HasMaxLength(50);

                entity.Property(e => e.UpdateTime).HasPrecision(7);

                entity.HasOne(d => d.Equipmenttype)
                    .WithMany(p => p.Equipment)
                    .HasForeignKey(d => d.Equipmenttypeid)
                    .HasConstraintName("FK_EQUIPMENT_EQUIPMENTTYPE_EQ~");
            });

            modelBuilder.Entity<Equipmentalarm>(entity =>
            {
                entity.ToTable("EQUIPMENTALARM");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.Alarmcode)
                    .HasMaxLength(100)
                    .HasColumnName("ALARMCODE");

                entity.Property(e => e.Alarmset)
                    .HasPrecision(1)
                    .HasColumnName("ALARMSET");

                entity.Property(e => e.Alarmsource)
                    .HasMaxLength(100)
                    .HasColumnName("ALARMSOURCE");

                entity.Property(e => e.Alarmtext)
                    .HasMaxLength(1000)
                    .HasColumnName("ALARMTEXT");

                entity.Property(e => e.Alarmtime)
                    .HasPrecision(7)
                    .HasColumnName("ALARMTIME");

                entity.Property(e => e.Eqid)
                    .HasMaxLength(100)
                    .HasColumnName("EQID");
            });

            modelBuilder.Entity<Equipmentconfiguration>(entity =>
            {
                entity.ToTable("EQUIPMENTCONFIGURATION");

                entity.HasIndex(e => e.Eqidid, "IX_EQUIPMENTCONFIGURATION_EQI~");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.Configurationitem)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("CONFIGURATIONITEM");

                entity.Property(e => e.Configurationname)
                    .HasMaxLength(300)
                    .HasColumnName("CONFIGURATIONNAME");

                entity.Property(e => e.Configurationvalue)
                    .HasMaxLength(300)
                    .HasColumnName("CONFIGURATIONVALUE");

                entity.Property(e => e.CreateBy).HasMaxLength(50);

                entity.Property(e => e.CreateTime).HasPrecision(7);

                entity.Property(e => e.DisplayOrder).HasPrecision(10);

                entity.Property(e => e.Eqidid).HasColumnName("EQIDID");

                entity.Property(e => e.IsValid).HasPrecision(1);

                entity.Property(e => e.UpdateBy).HasMaxLength(50);

                entity.Property(e => e.UpdateTime).HasPrecision(7);

                entity.HasOne(d => d.Eqid)
                    .WithMany(p => p.Equipmentconfigurations)
                    .HasForeignKey(d => d.Eqidid)
                    .HasConstraintName("FK_EQUIPMENTCONFIGURATION_EQU~");
            });

            modelBuilder.Entity<Equipmentrealtimestatus>(entity =>
            {
                entity.HasKey(e => e.Eqid)
                    .HasName("EQUIPMENTREALTIMESTATUS_PK");

                entity.ToTable("EQUIPMENTREALTIMESTATUS");

                entity.Property(e => e.Eqid)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("EQID");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("STATUS");

                entity.Property(e => e.Updatetime)
                    .HasPrecision(6)
                    .HasColumnName("UPDATETIME");
            });

            modelBuilder.Entity<Equipmentstatus>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("EQUIPMENTSTATUS");

                entity.HasIndex(e => e.Eqid, "EQUIPMENTSTATUS_INDEX1");

                entity.Property(e => e.Datetime)
                    .HasPrecision(6)
                    .HasColumnName("DATETIME");

                entity.Property(e => e.Eqid)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("EQID");

                entity.Property(e => e.Eqtype)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("EQTYPE");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("STATUS");
            });

            modelBuilder.Entity<Equipmenttype>(entity =>
            {
                entity.ToTable("EQUIPMENTTYPE");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.CreateBy).HasMaxLength(50);

                entity.Property(e => e.CreateTime).HasPrecision(7);

                entity.Property(e => e.IsValid).HasPrecision(1);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("NAME");

                entity.Property(e => e.UpdateBy).HasMaxLength(50);

                entity.Property(e => e.UpdateTime).HasPrecision(7);
            });

            modelBuilder.Entity<Equipmenttypeconfiguration>(entity =>
            {
                entity.ToTable("EQUIPMENTTYPECONFIGURATION");

                entity.HasIndex(e => e.Typenameid, "IX_EQUIPMENTTYPECONFIGURATION~");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.Configurationitem)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("CONFIGURATIONITEM");

                entity.Property(e => e.Configurationname)
                    .HasMaxLength(300)
                    .HasColumnName("CONFIGURATIONNAME");

                entity.Property(e => e.Configurationvalue)
                    .HasMaxLength(300)
                    .HasColumnName("CONFIGURATIONVALUE");

                entity.Property(e => e.CreateBy).HasMaxLength(50);

                entity.Property(e => e.CreateTime).HasPrecision(7);

                entity.Property(e => e.DisplayOrder).HasPrecision(10);

                entity.Property(e => e.IsValid).HasPrecision(1);

                entity.Property(e => e.Typenameid).HasColumnName("TYPENAMEID");

                entity.Property(e => e.UpdateBy).HasMaxLength(50);

                entity.Property(e => e.UpdateTime).HasPrecision(7);

                entity.HasOne(d => d.Typename)
                    .WithMany(p => p.Equipmenttypeconfigurations)
                    .HasForeignKey(d => d.Typenameid)
                    .HasConstraintName("FK_EQUIPMENTTYPECONFIGURATION~");
            });

            modelBuilder.Entity<FileAttachment>(entity =>
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.FileExt)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.FileName).IsRequired();

                entity.Property(e => e.Length).HasPrecision(19);

                entity.Property(e => e.UploadTime).HasPrecision(7);
            });

            modelBuilder.Entity<FrameworkGroup>(entity =>
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.CreateBy).HasMaxLength(50);

                entity.Property(e => e.CreateTime).HasPrecision(7);

                entity.Property(e => e.GroupCode)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.GroupName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.UpdateBy).HasMaxLength(50);

                entity.Property(e => e.UpdateTime).HasPrecision(7);
            });

            modelBuilder.Entity<FrameworkMenu>(entity =>
            {
                entity.HasIndex(e => e.ParentId, "IX_FrameworkMenus_ParentId");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.DisplayOrder).HasPrecision(10);

                entity.Property(e => e.FolderOnly).HasPrecision(1);

                entity.Property(e => e.Icon).HasMaxLength(50);

                entity.Property(e => e.IsInherit).HasPrecision(1);

                entity.Property(e => e.IsInside).HasPrecision(1);

                entity.Property(e => e.IsPublic).HasPrecision(1);

                entity.Property(e => e.PageName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.ShowOnMenu).HasPrecision(1);

                entity.HasOne(d => d.Parent)
                    .WithMany(p => p.InverseParent)
                    .HasForeignKey(d => d.ParentId)
                    .HasConstraintName("FK_FrameworkMenus_FrameworkMe~");
            });

            modelBuilder.Entity<FrameworkRole>(entity =>
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.CreateBy).HasMaxLength(50);

                entity.Property(e => e.CreateTime).HasPrecision(7);

                entity.Property(e => e.RoleCode)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.RoleName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.UpdateBy).HasMaxLength(50);

                entity.Property(e => e.UpdateTime).HasPrecision(7);
            });

            modelBuilder.Entity<FrameworkUser>(entity =>
            {
                entity.HasIndex(e => e.PhotoId, "IX_FrameworkUsers_PhotoId");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.Address).HasMaxLength(200);

                entity.Property(e => e.CreateBy).HasMaxLength(50);

                entity.Property(e => e.CreateTime).HasPrecision(7);

                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.Gender).HasPrecision(10);

                entity.Property(e => e.HomePhone).HasMaxLength(30);

                entity.Property(e => e.IsValid).HasPrecision(1);

                entity.Property(e => e.Itcode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("ITCode");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(32);

                entity.Property(e => e.UpdateBy).HasMaxLength(50);

                entity.Property(e => e.UpdateTime).HasPrecision(7);

                entity.HasOne(d => d.Photo)
                    .WithMany(p => p.FrameworkUsers)
                    .HasForeignKey(d => d.PhotoId)
                    .HasConstraintName("FK_FrameworkUsers_FileAttachm~");
            });

            modelBuilder.Entity<FrameworkUserGroup>(entity =>
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.CreateBy).HasMaxLength(50);

                entity.Property(e => e.CreateTime).HasPrecision(7);

                entity.Property(e => e.GroupCode).IsRequired();

                entity.Property(e => e.UpdateBy).HasMaxLength(50);

                entity.Property(e => e.UpdateTime).HasPrecision(7);

                entity.Property(e => e.UserCode).IsRequired();
            });

            modelBuilder.Entity<FrameworkUserRole>(entity =>
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.CreateBy).HasMaxLength(50);

                entity.Property(e => e.CreateTime).HasPrecision(7);

                entity.Property(e => e.RoleCode).IsRequired();

                entity.Property(e => e.UpdateBy).HasMaxLength(50);

                entity.Property(e => e.UpdateTime).HasPrecision(7);

                entity.Property(e => e.UserCode).IsRequired();
            });

            modelBuilder.Entity<FunctionPrivilege>(entity =>
            {
                entity.HasIndex(e => e.MenuItemId, "IX_FunctionPrivileges_MenuIte~");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.Allowed).HasPrecision(1);

                entity.Property(e => e.CreateBy).HasMaxLength(50);

                entity.Property(e => e.CreateTime).HasPrecision(7);

                entity.Property(e => e.UpdateBy).HasMaxLength(50);

                entity.Property(e => e.UpdateTime).HasPrecision(7);

                entity.HasOne(d => d.MenuItem)
                    .WithMany(p => p.FunctionPrivileges)
                    .HasForeignKey(d => d.MenuItemId)
                    .HasConstraintName("FK_FunctionPrivileges_Framewo~");
            });

            modelBuilder.Entity<Historymtbaresult>(entity =>
            {
                entity.ToTable("HISTORYMTBARESULT");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.Checktime)
                    .HasPrecision(7)
                    .HasColumnName("CHECKTIME");

                entity.Property(e => e.Eqid).HasColumnName("EQID");

                entity.Property(e => e.Mtbavalue)
                    .HasColumnType("NUMBER")
                    .HasColumnName("MTBAVALUE");

                entity.Property(e => e.Shift).HasColumnName("SHIFT");
            });

            modelBuilder.Entity<Historyrunrateresult>(entity =>
            {
                entity.ToTable("HISTORYRUNRATERESULT");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.Checktime)
                    .HasPrecision(7)
                    .HasColumnName("CHECKTIME");

                entity.Property(e => e.Eqid)
                    .HasMaxLength(100)
                    .HasColumnName("EQID");

                entity.Property(e => e.Runratevalue)
                    .HasColumnType("NUMBER")
                    .HasColumnName("RUNRATEVALUE");

                entity.Property(e => e.Shift).HasColumnName("SHIFT");
            });

            modelBuilder.Entity<Mtbaresult>(entity =>
            {
                entity.HasKey(e => e.Eqid);

                entity.ToTable("MTBARESULT");

                entity.Property(e => e.Eqid).HasColumnName("EQID");

                entity.Property(e => e.Checktime)
                    .HasPrecision(7)
                    .HasColumnName("CHECKTIME");

                entity.Property(e => e.Mtbavalue)
                    .HasColumnType("NUMBER")
                    .HasColumnName("MTBAVALUE");
            });

            modelBuilder.Entity<PersistedGrant>(entity =>
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.CreationTime).HasPrecision(7);

                entity.Property(e => e.Expiration).HasPrecision(7);

                entity.Property(e => e.RefreshToken).HasMaxLength(50);

                entity.Property(e => e.Type).HasMaxLength(50);
            });

            modelBuilder.Entity<Runrateresult>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("RUNRATERESULT");

                entity.Property(e => e.Checktime)
                    .HasPrecision(7)
                    .HasColumnName("CHECKTIME");

                entity.Property(e => e.Eqid)
                    .HasMaxLength(100)
                    .HasColumnName("EQID");

                entity.Property(e => e.Runratevalue)
                    .HasColumnType("NUMBER")
                    .HasColumnName("RUNRATEVALUE");
            });

            modelBuilder.Entity<TestModel>(entity =>
            {
                entity.ToTable("TestModel");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.CreateBy).HasMaxLength(50);

                entity.Property(e => e.CreateTime).HasPrecision(7);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.UpdateBy).HasMaxLength(50);

                entity.Property(e => e.UpdateTime).HasPrecision(7);
            });

            modelBuilder.Entity<VEquipmentstatus>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("V_EQUIPMENTSTATUS");

                entity.Property(e => e.Eqid)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("EQID");

                entity.Property(e => e.Mtba)
                    .HasColumnType("NUMBER")
                    .HasColumnName("MTBA");

                entity.Property(e => e.Runrate)
                    .HasColumnType("NUMBER")
                    .HasColumnName("RUNRATE");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("STATUS");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
