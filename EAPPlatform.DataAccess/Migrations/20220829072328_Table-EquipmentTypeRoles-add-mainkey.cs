using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAPPlatform.DataAccess.Migrations
{
    public partial class TableEquipmentTypeRolesaddmainkey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ID",
                table: "EQUIPMENTTYPEROLE",
                type: "RAW(16)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_EQUIPMENTTYPEROLE",
                table: "EQUIPMENTTYPEROLE",
                column: "ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_EQUIPMENTTYPEROLE",
                table: "EQUIPMENTTYPEROLE");

            migrationBuilder.DropColumn(
                name: "ID",
                table: "EQUIPMENTTYPEROLE");
        }
    }
}
