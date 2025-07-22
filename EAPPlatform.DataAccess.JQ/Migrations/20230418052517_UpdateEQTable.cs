using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAPPlatform.DataAccess.JQ.Migrations
{
    public partial class UpdateEQTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
