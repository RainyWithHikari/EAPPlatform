using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAPPlatform.DataAccess.Migrations
{
    public partial class modify_col_name : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsValid",
                table: "EQUIPMENTTYPECONFIGURATION",
                newName: "ISVALID");

            migrationBuilder.RenameColumn(
                name: "IsValid",
                table: "EQUIPMENTTYPE",
                newName: "ISVALID");

            migrationBuilder.RenameColumn(
                name: "IsValid",
                table: "EQUIPMENTCONFIGURATION",
                newName: "ISVALID");

            migrationBuilder.RenameColumn(
                name: "IsValid",
                table: "EQUIPMENT",
                newName: "ISVALID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ISVALID",
                table: "EQUIPMENTTYPECONFIGURATION",
                newName: "IsValid");

            migrationBuilder.RenameColumn(
                name: "ISVALID",
                table: "EQUIPMENTTYPE",
                newName: "IsValid");

            migrationBuilder.RenameColumn(
                name: "ISVALID",
                table: "EQUIPMENTCONFIGURATION",
                newName: "IsValid");

            migrationBuilder.RenameColumn(
                name: "ISVALID",
                table: "EQUIPMENT",
                newName: "IsValid");
        }
    }
}
