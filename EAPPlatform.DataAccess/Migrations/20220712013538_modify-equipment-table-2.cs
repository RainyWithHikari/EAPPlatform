using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAPPlatform.DataAccess.Migrations
{
    public partial class modifyequipmenttable2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Equipment_EquipmentType_Eq~",
                table: "Equipment");

            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentConfiguration_Equ~",
                table: "EquipmentConfiguration");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EquipmentConfiguration",
                table: "EquipmentConfiguration");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Equipment",
                table: "Equipment");

            migrationBuilder.RenameTable(
                name: "EquipmentConfiguration",
                newName: "EQUIPMENTCONFIGURATION");

            migrationBuilder.RenameTable(
                name: "Equipment",
                newName: "EQUIPMENT");

            migrationBuilder.RenameColumn(
                name: "EQIDId",
                table: "EQUIPMENTCONFIGURATION",
                newName: "EQIDID");

            migrationBuilder.RenameColumn(
                name: "ConfigurationValue",
                table: "EQUIPMENTCONFIGURATION",
                newName: "CONFIGURATIONVALUE");

            migrationBuilder.RenameColumn(
                name: "ConfigurationName",
                table: "EQUIPMENTCONFIGURATION",
                newName: "CONFIGURATIONNAME");

            migrationBuilder.RenameColumn(
                name: "ConfigurationItem",
                table: "EQUIPMENTCONFIGURATION",
                newName: "CONFIGURATIONITEM");

            migrationBuilder.RenameIndex(
                name: "IX_EquipmentConfiguration_EQI~",
                table: "EQUIPMENTCONFIGURATION",
                newName: "IX_EQUIPMENTCONFIGURATION_EQI~");

            migrationBuilder.RenameIndex(
                name: "IX_Equipment_EquipmentTypeId",
                table: "EQUIPMENT",
                newName: "IX_EQUIPMENT_EquipmentTypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EQUIPMENTCONFIGURATION",
                table: "EQUIPMENTCONFIGURATION",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EQUIPMENT",
                table: "EQUIPMENT",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_EQUIPMENT_EquipmentType_Eq~",
                table: "EQUIPMENT",
                column: "EquipmentTypeId",
                principalTable: "EquipmentType",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_EQUIPMENTCONFIGURATION_EQU~",
                table: "EQUIPMENTCONFIGURATION",
                column: "EQIDID",
                principalTable: "EQUIPMENT",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EQUIPMENT_EquipmentType_Eq~",
                table: "EQUIPMENT");

            migrationBuilder.DropForeignKey(
                name: "FK_EQUIPMENTCONFIGURATION_EQU~",
                table: "EQUIPMENTCONFIGURATION");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EQUIPMENTCONFIGURATION",
                table: "EQUIPMENTCONFIGURATION");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EQUIPMENT",
                table: "EQUIPMENT");

            migrationBuilder.RenameTable(
                name: "EQUIPMENTCONFIGURATION",
                newName: "EquipmentConfiguration");

            migrationBuilder.RenameTable(
                name: "EQUIPMENT",
                newName: "Equipment");

            migrationBuilder.RenameColumn(
                name: "EQIDID",
                table: "EquipmentConfiguration",
                newName: "EQIDId");

            migrationBuilder.RenameColumn(
                name: "CONFIGURATIONVALUE",
                table: "EquipmentConfiguration",
                newName: "ConfigurationValue");

            migrationBuilder.RenameColumn(
                name: "CONFIGURATIONNAME",
                table: "EquipmentConfiguration",
                newName: "ConfigurationName");

            migrationBuilder.RenameColumn(
                name: "CONFIGURATIONITEM",
                table: "EquipmentConfiguration",
                newName: "ConfigurationItem");

            migrationBuilder.RenameIndex(
                name: "IX_EQUIPMENTCONFIGURATION_EQI~",
                table: "EquipmentConfiguration",
                newName: "IX_EquipmentConfiguration_EQI~");

            migrationBuilder.RenameIndex(
                name: "IX_EQUIPMENT_EquipmentTypeId",
                table: "Equipment",
                newName: "IX_Equipment_EquipmentTypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EquipmentConfiguration",
                table: "EquipmentConfiguration",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Equipment",
                table: "Equipment",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Equipment_EquipmentType_Eq~",
                table: "Equipment",
                column: "EquipmentTypeId",
                principalTable: "EquipmentType",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentConfiguration_Equ~",
                table: "EquipmentConfiguration",
                column: "EQIDId",
                principalTable: "Equipment",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
