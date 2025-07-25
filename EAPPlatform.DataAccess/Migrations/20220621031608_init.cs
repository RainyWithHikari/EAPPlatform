﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAPPlatform.DataAccess.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.CreateTable(
            //    name: "ActionLogs",
            //    columns: table => new
            //    {
            //        ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
            //        ModuleName = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true),
            //        ActionName = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true),
            //        ITCode = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
            //        ActionUrl = table.Column<string>(type: "NVARCHAR2(250)", maxLength: 250, nullable: true),
            //        ActionTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
            //        Duration = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
            //        Remark = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
            //        IP = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
            //        LogType = table.Column<int>(type: "NUMBER(10)", nullable: false),
            //        CreateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
            //        CreateBy = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
            //        UpdateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
            //        UpdateBy = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_ActionLogs", x => x.ID);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "DataPrivileges",
            //    columns: table => new
            //    {
            //        ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
            //        UserCode = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
            //        GroupCode = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
            //        TableName = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
            //        RelateId = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
            //        Domain = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
            //        CreateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
            //        CreateBy = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
            //        UpdateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
            //        UpdateBy = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_DataPrivileges", x => x.ID);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "EquipmentType",
            //    columns: table => new
            //    {
            //        ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
            //        Name = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
            //        IsValid = table.Column<bool>(type: "NUMBER(1)", nullable: false),
            //        CreateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
            //        CreateBy = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
            //        UpdateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
            //        UpdateBy = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_EquipmentType", x => x.ID);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "FileAttachments",
            //    columns: table => new
            //    {
            //        ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
            //        FileName = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
            //        FileExt = table.Column<string>(type: "NVARCHAR2(10)", maxLength: 10, nullable: false),
            //        Path = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
            //        Length = table.Column<long>(type: "NUMBER(19)", nullable: false),
            //        UploadTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
            //        SaveMode = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
            //        FileData = table.Column<byte[]>(type: "RAW(2000)", nullable: true),
            //        ExtraInfo = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
            //        HandlerInfo = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_FileAttachments", x => x.ID);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "FrameworkGroups",
            //    columns: table => new
            //    {
            //        ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
            //        GroupCode = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
            //        GroupName = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
            //        GroupRemark = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
            //        TenantCode = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
            //        CreateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
            //        CreateBy = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
            //        UpdateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
            //        UpdateBy = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_FrameworkGroups", x => x.ID);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "FrameworkMenus",
            //    columns: table => new
            //    {
            //        ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
            //        PageName = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
            //        ActionName = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
            //        ModuleName = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
            //        FolderOnly = table.Column<bool>(type: "NUMBER(1)", nullable: false),
            //        IsInherit = table.Column<bool>(type: "NUMBER(1)", nullable: false),
            //        ClassName = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
            //        MethodName = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
            //        Domain = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
            //        ShowOnMenu = table.Column<bool>(type: "NUMBER(1)", nullable: false),
            //        IsPublic = table.Column<bool>(type: "NUMBER(1)", nullable: false),
            //        DisplayOrder = table.Column<int>(type: "NUMBER(10)", nullable: false),
            //        IsInside = table.Column<bool>(type: "NUMBER(1)", nullable: false),
            //        Url = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
            //        Icon = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
            //        ParentId = table.Column<Guid>(type: "RAW(16)", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_FrameworkMenus", x => x.ID);
            //        table.ForeignKey(
            //            name: "FK_FrameworkMenus_FrameworkMe~",
            //            column: x => x.ParentId,
            //            principalTable: "FrameworkMenus",
            //            principalColumn: "ID");
            //    });

            //migrationBuilder.CreateTable(
            //    name: "FrameworkRoles",
            //    columns: table => new
            //    {
            //        ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
            //        RoleCode = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
            //        RoleName = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
            //        RoleRemark = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
            //        TenantCode = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
            //        CreateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
            //        CreateBy = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
            //        UpdateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
            //        UpdateBy = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_FrameworkRoles", x => x.ID);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "FrameworkUserGroups",
            //    columns: table => new
            //    {
            //        ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
            //        UserCode = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
            //        GroupCode = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
            //        CreateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
            //        CreateBy = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
            //        UpdateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
            //        UpdateBy = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_FrameworkUserGroups", x => x.ID);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "FrameworkUserRoles",
            //    columns: table => new
            //    {
            //        ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
            //        UserCode = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
            //        RoleCode = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
            //        CreateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
            //        CreateBy = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
            //        UpdateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
            //        UpdateBy = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_FrameworkUserRoles", x => x.ID);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "PersistedGrants",
            //    columns: table => new
            //    {
            //        ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
            //        Type = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
            //        UserCode = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
            //        CreationTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
            //        Expiration = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
            //        RefreshToken = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_PersistedGrants", x => x.ID);
            //    });

            ////migrationBuilder.CreateTable(
            ////    name: "TestModel",
            ////    columns: table => new
            ////    {
            ////        ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
            ////        Name = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
            ////        CreateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
            ////        CreateBy = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
            ////        UpdateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
            ////        UpdateBy = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true)
            ////    },
            ////    constraints: table =>
            ////    {
            ////        table.PrimaryKey("PK_TestModel", x => x.ID);
            ////    });

            //migrationBuilder.CreateTable(
            //    name: "Equipment",
            //    columns: table => new
            //    {
            //        ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
            //        EQID = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
            //        EquipmentTypeId = table.Column<Guid>(type: "RAW(16)", nullable: true),
            //        Name = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
            //        IsValid = table.Column<bool>(type: "NUMBER(1)", nullable: false),
            //        CreateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
            //        CreateBy = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
            //        UpdateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
            //        UpdateBy = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Equipment", x => x.ID);
            //        table.ForeignKey(
            //            name: "FK_Equipment_EquipmentType_Eq~",
            //            column: x => x.EquipmentTypeId,
            //            principalTable: "EquipmentType",
            //            principalColumn: "ID");
            //    });

            //migrationBuilder.CreateTable(
            //    name: "EquipmentTypeConfiguration",
            //    columns: table => new
            //    {
            //        ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
            //        TypeNameId = table.Column<Guid>(type: "RAW(16)", nullable: false),
            //        ConfigurationItem = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
            //        ConfigurationValue = table.Column<string>(type: "NVARCHAR2(300)", maxLength: 300, nullable: true),
            //        ConfigurationName = table.Column<string>(type: "NVARCHAR2(300)", maxLength: 300, nullable: true),
            //        DisplayOrder = table.Column<int>(type: "NUMBER(10)", nullable: true),
            //        IsValid = table.Column<bool>(type: "NUMBER(1)", nullable: false),
            //        CreateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
            //        CreateBy = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
            //        UpdateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
            //        UpdateBy = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_EquipmentTypeConfiguration", x => x.ID);
            //        table.ForeignKey(
            //            name: "FK_EquipmentTypeConfiguration~",
            //            column: x => x.TypeNameId,
            //            principalTable: "EquipmentType",
            //            principalColumn: "ID",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "FrameworkUsers",
            //    columns: table => new
            //    {
            //        ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
            //        Email = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
            //        Gender = table.Column<int>(type: "NUMBER(10)", nullable: true),
            //        CellPhone = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
            //        HomePhone = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: true),
            //        Address = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true),
            //        ZipCode = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
            //        CreateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
            //        CreateBy = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
            //        UpdateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
            //        UpdateBy = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
            //        ITCode = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
            //        Password = table.Column<string>(type: "NVARCHAR2(32)", maxLength: 32, nullable: false),
            //        Name = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
            //        IsValid = table.Column<bool>(type: "NUMBER(1)", nullable: false),
            //        PhotoId = table.Column<Guid>(type: "RAW(16)", nullable: true),
            //        TenantCode = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_FrameworkUsers", x => x.ID);
            //        table.ForeignKey(
            //            name: "FK_FrameworkUsers_FileAttachm~",
            //            column: x => x.PhotoId,
            //            principalTable: "FileAttachments",
            //            principalColumn: "ID",
            //            onDelete: ReferentialAction.Restrict);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "FunctionPrivileges",
            //    columns: table => new
            //    {
            //        ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
            //        RoleCode = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
            //        MenuItemId = table.Column<Guid>(type: "RAW(16)", nullable: false),
            //        Allowed = table.Column<bool>(type: "NUMBER(1)", nullable: false),
            //        CreateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
            //        CreateBy = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
            //        UpdateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
            //        UpdateBy = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_FunctionPrivileges", x => x.ID);
            //        table.ForeignKey(
            //            name: "FK_FunctionPrivileges_Framewo~",
            //            column: x => x.MenuItemId,
            //            principalTable: "FrameworkMenus",
            //            principalColumn: "ID",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "EquipmentConfiguration",
            //    columns: table => new
            //    {
            //        ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
            //        EQIDId = table.Column<Guid>(type: "RAW(16)", nullable: false),
            //        ConfigurationItem = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
            //        ConfigurationValue = table.Column<string>(type: "NVARCHAR2(300)", maxLength: 300, nullable: true),
            //        ConfigurationName = table.Column<string>(type: "NVARCHAR2(300)", maxLength: 300, nullable: true),
            //        DisplayOrder = table.Column<int>(type: "NUMBER(10)", nullable: true),
            //        IsValid = table.Column<bool>(type: "NUMBER(1)", nullable: false),
            //        CreateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
            //        CreateBy = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
            //        UpdateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
            //        UpdateBy = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_EquipmentConfiguration", x => x.ID);
            //        table.ForeignKey(
            //            name: "FK_EquipmentConfiguration_Equ~",
            //            column: x => x.EQIDId,
            //            principalTable: "Equipment",
            //            principalColumn: "ID",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateIndex(
            //    name: "IX_Equipment_EquipmentTypeId",
            //    table: "Equipment",
            //    column: "EquipmentTypeId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_EquipmentConfiguration_EQI~",
            //    table: "EquipmentConfiguration",
            //    column: "EQIDId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_EquipmentTypeConfiguration~",
            //    table: "EquipmentTypeConfiguration",
            //    column: "TypeNameId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_FrameworkMenus_ParentId",
            //    table: "FrameworkMenus",
            //    column: "ParentId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_FrameworkUsers_PhotoId",
            //    table: "FrameworkUsers",
            //    column: "PhotoId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_FunctionPrivileges_MenuIte~",
            //    table: "FunctionPrivileges",
            //    column: "MenuItemId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "ActionLogs");

            //migrationBuilder.DropTable(
            //    name: "DataPrivileges");

            //migrationBuilder.DropTable(
            //    name: "EquipmentConfiguration");

            //migrationBuilder.DropTable(
            //    name: "EquipmentTypeConfiguration");

            //migrationBuilder.DropTable(
            //    name: "FrameworkGroups");

            //migrationBuilder.DropTable(
            //    name: "FrameworkRoles");

            //migrationBuilder.DropTable(
            //    name: "FrameworkUserGroups");

            //migrationBuilder.DropTable(
            //    name: "FrameworkUserRoles");

            //migrationBuilder.DropTable(
            //    name: "FrameworkUsers");

            //migrationBuilder.DropTable(
            //    name: "FunctionPrivileges");

            //migrationBuilder.DropTable(
            //    name: "PersistedGrants");

            ////migrationBuilder.DropTable(
            ////    name: "TestModel");

            //migrationBuilder.DropTable(
            //    name: "Equipment");

            //migrationBuilder.DropTable(
            //    name: "FileAttachments");

            //migrationBuilder.DropTable(
            //    name: "FrameworkMenus");

            //migrationBuilder.DropTable(
            //    name: "EquipmentType");
        }
    }
}
