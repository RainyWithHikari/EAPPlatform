using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAPPlatform.DataAccess.JQ.Migrations
{
    public partial class addtable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_RUNRATERESULT",
                table: "RUNRATERESULT");

            migrationBuilder.DropColumn(
                name: "ID",
                table: "RUNRATERESULT");

            migrationBuilder.AlterColumn<string>(
                name: "EQID",
                table: "RUNRATERESULT",
                type: "NVARCHAR2(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_RUNRATERESULT",
                table: "RUNRATERESULT",
                column: "EQID");

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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EQUIPMENTREALTIMESTATUS");

            migrationBuilder.DropTable(
                name: "EQUIPMENTSTATUS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RUNRATERESULT",
                table: "RUNRATERESULT");

            migrationBuilder.AlterColumn<string>(
                name: "EQID",
                table: "RUNRATERESULT",
                type: "NVARCHAR2(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<Guid>(
                name: "ID",
                table: "RUNRATERESULT",
                type: "RAW(16)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_RUNRATERESULT",
                table: "RUNRATERESULT",
                column: "ID");
        }
    }
}
