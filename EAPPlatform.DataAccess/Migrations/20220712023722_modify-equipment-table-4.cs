using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAPPlatform.DataAccess.Migrations
{
    public partial class modifyequipmenttable4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EQUIPMENT_EQUIPMENTTYPE_Eq~",
                table: "EQUIPMENT");

            migrationBuilder.RenameColumn(
                name: "TypeNameId",
                table: "EQUIPMENTTYPECONFIGURATION",
                newName: "TYPENAMEID");

            migrationBuilder.RenameColumn(
                name: "EquipmentTypeId",
                table: "EQUIPMENT",
                newName: "EQUIPMENTTYPEID");

            migrationBuilder.RenameIndex(
                name: "IX_EQUIPMENT_EquipmentTypeId",
                table: "EQUIPMENT",
                newName: "IX_EQUIPMENT_EQUIPMENTTYPEID");

            migrationBuilder.AddForeignKey(
                name: "FK_EQUIPMENT_EQUIPMENTTYPE_EQ~",
                table: "EQUIPMENT",
                column: "EQUIPMENTTYPEID",
                principalTable: "EQUIPMENTTYPE",
                principalColumn: "ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EQUIPMENT_EQUIPMENTTYPE_EQ~",
                table: "EQUIPMENT");

            migrationBuilder.RenameColumn(
                name: "TYPENAMEID",
                table: "EQUIPMENTTYPECONFIGURATION",
                newName: "TypeNameId");

            migrationBuilder.RenameColumn(
                name: "EQUIPMENTTYPEID",
                table: "EQUIPMENT",
                newName: "EquipmentTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_EQUIPMENT_EQUIPMENTTYPEID",
                table: "EQUIPMENT",
                newName: "IX_EQUIPMENT_EquipmentTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_EQUIPMENT_EQUIPMENTTYPE_Eq~",
                table: "EQUIPMENT",
                column: "EquipmentTypeId",
                principalTable: "EQUIPMENTTYPE",
                principalColumn: "ID");
        }
    }
}
