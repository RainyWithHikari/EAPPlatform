using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAPPlatform.DataAccess.GDL.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActionLogs",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ModuleName = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true),
                    ActionName = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true),
                    ITCode = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    ActionUrl = table.Column<string>(type: "NVARCHAR2(250)", maxLength: 250, nullable: true),
                    ActionTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Duration = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    Remark = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    IP = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    LogType = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    CreateBy = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    UpdateBy = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActionLogs", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ALARMPOSITIONS",
                columns: table => new
                {
                    EQID = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    ALARMCODE = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    POSITIONID = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "CLIENTLOG",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    DATETIME = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    EQID = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    LOGTYPE = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    LOGLEVEL = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    LOGCONTENT = table.Column<string>(type: "NVARCHAR2(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CLIENTLOG", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "DataPrivileges",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    UserCode = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    GroupCode = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    TableName = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    RelateId = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Domain = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    CreateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    CreateBy = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    UpdateBy = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataPrivileges", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "EQUIPMENTALARM",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    EQID = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    ALARMCODE = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    ALARMTEXT = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: true),
                    ALARMSOURCE = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    ALARMTIME = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    ALARMSET = table.Column<bool>(type: "NUMBER(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EQUIPMENTALARM", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "EQUIPMENTPARAMSHISCAL",
                columns: table => new
                {
                    EQID = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    NAME = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    VALUE = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    UPDATETIME = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    REMARK = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "EQUIPMENTPARAMSHISRAW",
                columns: table => new
                {
                    EQID = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    NAME = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    VALUE = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    UPDATETIME = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "EQUIPMENTPARAMSREALTIME",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    EQID = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    SVID = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    NAME = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    VALUE = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    UPDATETIME = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EQUIPMENTPARAMSREALTIME", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "EQUIPMENTREALTIMESTATUS",
                columns: table => new
                {
                    EQID = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    STATUS = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    UPDATETIME = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EQUIPMENTREALTIMESTATUS", x => x.EQID);
                });

            migrationBuilder.CreateTable(
                name: "EQUIPMENTSTATUS",
                columns: table => new
                {
                    EQID = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    EQTYPE = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    STATUS = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    DATETIME = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "EQUIPMENTTYPE",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    NAME = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    ISVALID = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    CreateBy = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    UpdateBy = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EQUIPMENTTYPE", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "FileAttachments",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    FileName = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    FileExt = table.Column<string>(type: "NVARCHAR2(10)", maxLength: 10, nullable: false),
                    Path = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Length = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    UploadTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    SaveMode = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    FileData = table.Column<byte[]>(type: "RAW(2000)", nullable: true),
                    ExtraInfo = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    HandlerInfo = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileAttachments", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "FrameworkGroups",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    GroupCode = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    GroupName = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    GroupRemark = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    TenantCode = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    CreateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    CreateBy = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    UpdateBy = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FrameworkGroups", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "FrameworkMenus",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    PageName = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    ActionName = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    ModuleName = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    FolderOnly = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    IsInherit = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    ClassName = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    MethodName = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Domain = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    ShowOnMenu = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    IsPublic = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    DisplayOrder = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    IsInside = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    Url = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Icon = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    ParentId = table.Column<Guid>(type: "RAW(16)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FrameworkMenus", x => x.ID);
                    table.ForeignKey(
                        name: "FK_FrameworkMenus_FrameworkMe~",
                        column: x => x.ParentId,
                        principalTable: "FrameworkMenus",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "FrameworkRoles",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    RoleCode = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    RoleName = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    RoleRemark = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    TenantCode = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    CreateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    CreateBy = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    UpdateBy = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FrameworkRoles", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "FrameworkUserGroups",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    UserCode = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    GroupCode = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    CreateBy = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    UpdateBy = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FrameworkUserGroups", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "FrameworkUserRoles",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    UserCode = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    RoleCode = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    CreateBy = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    UpdateBy = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FrameworkUserRoles", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "HISTORYMTBARESULT",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    EQID = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    MTBAVALUE = table.Column<decimal>(type: "NUMBER", nullable: false),
                    CHECKTIME = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DATATIME = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    SHIFT = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HISTORYMTBARESULT", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "HISTORYRUNRATERESULT",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    EQID = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    RUNRATEVALUE = table.Column<decimal>(type: "NUMBER", nullable: false),
                    CHECKTIME = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DATATIME = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    SHIFT = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HISTORYRUNRATERESULT", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "MTBARESULT",
                columns: table => new
                {
                    EQID = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    MTBAVALUE = table.Column<decimal>(type: "NUMBER", nullable: false),
                    CHECKTIME = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MTBARESULT", x => x.EQID);
                });

            migrationBuilder.CreateTable(
                name: "PersistedGrants",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Type = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    UserCode = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Expiration = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    RefreshToken = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersistedGrants", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "RUNRATERESULT",
                columns: table => new
                {
                    EQID = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    RUNRATEVALUE = table.Column<decimal>(type: "NUMBER", nullable: false),
                    CHECKTIME = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RUNRATERESULT", x => x.EQID);
                });

            migrationBuilder.CreateTable(
                name: "TestModel",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Name1 = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    CreateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    CreateBy = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    UpdateBy = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestModel", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "EQUIPMENT",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    EQID = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    EQUIPMENTTYPEID = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    NAME = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    ISVALID = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    SITE = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    PD = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    LINE = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    SORT = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    CreateBy = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    UpdateBy = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EQUIPMENT", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EQUIPMENT_EQUIPMENTTYPE_EQ~",
                        column: x => x.EQUIPMENTTYPEID,
                        principalTable: "EQUIPMENTTYPE",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "EQUIPMENTTYPECONFIGURATION",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    TYPENAMEID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    CONFIGURATIONITEM = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    CONFIGURATIONVALUE = table.Column<string>(type: "NVARCHAR2(300)", maxLength: 300, nullable: true),
                    CONFIGURATIONNAME = table.Column<string>(type: "NVARCHAR2(300)", maxLength: 300, nullable: true),
                    DisplayOrder = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    ISVALID = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    CreateBy = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    UpdateBy = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EQUIPMENTTYPECONFIGURATION", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EQUIPMENTTYPECONFIGURATION~",
                        column: x => x.TYPENAMEID,
                        principalTable: "EQUIPMENTTYPE",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FrameworkUsers",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Email = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    Gender = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    CellPhone = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    HomePhone = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: true),
                    Address = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true),
                    ZipCode = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    CreateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    CreateBy = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    UpdateBy = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    ITCode = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "NVARCHAR2(32)", maxLength: 32, nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    IsValid = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    PhotoId = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    TenantCode = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FrameworkUsers", x => x.ID);
                    table.ForeignKey(
                        name: "FK_FrameworkUsers_FileAttachm~",
                        column: x => x.PhotoId,
                        principalTable: "FileAttachments",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FunctionPrivileges",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    RoleCode = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    MenuItemId = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Allowed = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    CreateBy = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    UpdateBy = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FunctionPrivileges", x => x.ID);
                    table.ForeignKey(
                        name: "FK_FunctionPrivileges_Framewo~",
                        column: x => x.MenuItemId,
                        principalTable: "FrameworkMenus",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EQUIPMENTTYPEROLE",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    EQUIPMENTTYPEID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    FRAMEWORKROLEID = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EQUIPMENTTYPEROLE", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EQUIPMENTTYPEROLE_EQUIPMEN~",
                        column: x => x.EQUIPMENTTYPEID,
                        principalTable: "EQUIPMENTTYPE",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EQUIPMENTTYPEROLE_Framewor~",
                        column: x => x.FRAMEWORKROLEID,
                        principalTable: "FrameworkRoles",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EQUIPMENTCONFIGURATION",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    EQIDID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    CONFIGURATIONITEM = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    CONFIGURATIONVALUE = table.Column<string>(type: "NVARCHAR2(2000)", maxLength: 2000, nullable: true),
                    CONFIGURATIONNAME = table.Column<string>(type: "NVARCHAR2(300)", maxLength: 300, nullable: true),
                    DisplayOrder = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    ISVALID = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    ISREADONLY = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    CreateBy = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    UpdateBy = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EQUIPMENTCONFIGURATION", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EQUIPMENTCONFIGURATION_EQU~",
                        column: x => x.EQIDID,
                        principalTable: "EQUIPMENT",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EQUIPMENT_EQUIPMENTTYPEID",
                table: "EQUIPMENT",
                column: "EQUIPMENTTYPEID");

            migrationBuilder.CreateIndex(
                name: "IX_EQUIPMENTCONFIGURATION_EQI~",
                table: "EQUIPMENTCONFIGURATION",
                column: "EQIDID");

            migrationBuilder.CreateIndex(
                name: "IX_EQUIPMENTTYPECONFIGURATION~",
                table: "EQUIPMENTTYPECONFIGURATION",
                column: "TYPENAMEID");

            migrationBuilder.CreateIndex(
                name: "IX_EQUIPMENTTYPEROLE_EQUIPMEN~",
                table: "EQUIPMENTTYPEROLE",
                column: "EQUIPMENTTYPEID");

            migrationBuilder.CreateIndex(
                name: "IX_EQUIPMENTTYPEROLE_FRAMEWOR~",
                table: "EQUIPMENTTYPEROLE",
                column: "FRAMEWORKROLEID");

            migrationBuilder.CreateIndex(
                name: "IX_FrameworkMenus_ParentId",
                table: "FrameworkMenus",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_FrameworkUsers_PhotoId",
                table: "FrameworkUsers",
                column: "PhotoId");

            migrationBuilder.CreateIndex(
                name: "IX_FunctionPrivileges_MenuIte~",
                table: "FunctionPrivileges",
                column: "MenuItemId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActionLogs");

            migrationBuilder.DropTable(
                name: "ALARMPOSITIONS");

            migrationBuilder.DropTable(
                name: "CLIENTLOG");

            migrationBuilder.DropTable(
                name: "DataPrivileges");

            migrationBuilder.DropTable(
                name: "EQUIPMENTALARM");

            migrationBuilder.DropTable(
                name: "EQUIPMENTCONFIGURATION");

            migrationBuilder.DropTable(
                name: "EQUIPMENTPARAMSHISCAL");

            migrationBuilder.DropTable(
                name: "EQUIPMENTPARAMSHISRAW");

            migrationBuilder.DropTable(
                name: "EQUIPMENTPARAMSREALTIME");

            migrationBuilder.DropTable(
                name: "EQUIPMENTREALTIMESTATUS");

            migrationBuilder.DropTable(
                name: "EQUIPMENTSTATUS");

            migrationBuilder.DropTable(
                name: "EQUIPMENTTYPECONFIGURATION");

            migrationBuilder.DropTable(
                name: "EQUIPMENTTYPEROLE");

            migrationBuilder.DropTable(
                name: "FrameworkGroups");

            migrationBuilder.DropTable(
                name: "FrameworkUserGroups");

            migrationBuilder.DropTable(
                name: "FrameworkUserRoles");

            migrationBuilder.DropTable(
                name: "FrameworkUsers");

            migrationBuilder.DropTable(
                name: "FunctionPrivileges");

            migrationBuilder.DropTable(
                name: "HISTORYMTBARESULT");

            migrationBuilder.DropTable(
                name: "HISTORYRUNRATERESULT");

            migrationBuilder.DropTable(
                name: "MTBARESULT");

            migrationBuilder.DropTable(
                name: "PersistedGrants");

            migrationBuilder.DropTable(
                name: "RUNRATERESULT");

            migrationBuilder.DropTable(
                name: "TestModel");

            migrationBuilder.DropTable(
                name: "EQUIPMENT");

            migrationBuilder.DropTable(
                name: "FrameworkRoles");

            migrationBuilder.DropTable(
                name: "FileAttachments");

            migrationBuilder.DropTable(
                name: "FrameworkMenus");

            migrationBuilder.DropTable(
                name: "EQUIPMENTTYPE");
        }
    }
}
