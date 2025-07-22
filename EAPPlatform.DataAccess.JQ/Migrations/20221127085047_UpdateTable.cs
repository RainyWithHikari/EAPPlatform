using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAPPlatform.DataAccess.JQ.Migrations
{
    public partial class UpdateTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EQUIPMENTPARAMSHISCAL",
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EQUIPMENTPARAMSHISCAL");

            migrationBuilder.DropTable(
                name: "EQUIPMENTPARAMSHISRAW");

            migrationBuilder.DropTable(
                name: "EQUIPMENTPARAMSREALTIME");
        }
    }
}
