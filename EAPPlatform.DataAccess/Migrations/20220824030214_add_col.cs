using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAPPlatform.DataAccess.Migrations
{
    public partial class add_col : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ISREADONLY",
                table: "EQUIPMENTCONFIGURATION",
                type: "NUMBER(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LINE",
                table: "EQUIPMENT",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PD",
                table: "EQUIPMENT",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SITE",
                table: "EQUIPMENT",
                type: "NVARCHAR2(2000)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ISREADONLY",
                table: "EQUIPMENTCONFIGURATION");

            migrationBuilder.DropColumn(
                name: "LINE",
                table: "EQUIPMENT");

            migrationBuilder.DropColumn(
                name: "PD",
                table: "EQUIPMENT");

            migrationBuilder.DropColumn(
                name: "SITE",
                table: "EQUIPMENT");
        }
    }
}
