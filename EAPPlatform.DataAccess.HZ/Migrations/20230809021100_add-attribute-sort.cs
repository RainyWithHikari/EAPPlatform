using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAPPlatform.DataAccess.HZ.Migrations
{
    public partial class addattributesort : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "REMARK",
                table: "EQUIPMENTPARAMSHISCAL",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CONFIGURATIONVALUE",
                table: "EQUIPMENTCONFIGURATION",
                type: "NVARCHAR2(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(300)",
                oldMaxLength: 300,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SORT",
                table: "EQUIPMENT",
                type: "NUMBER(10)",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "REMARK",
                table: "EQUIPMENTPARAMSHISCAL");

            migrationBuilder.DropColumn(
                name: "SORT",
                table: "EQUIPMENT");

            migrationBuilder.AlterColumn<string>(
                name: "CONFIGURATIONVALUE",
                table: "EQUIPMENTCONFIGURATION",
                type: "NVARCHAR2(300)",
                maxLength: 300,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)",
                oldMaxLength: 2000,
                oldNullable: true);
        }
    }
}
