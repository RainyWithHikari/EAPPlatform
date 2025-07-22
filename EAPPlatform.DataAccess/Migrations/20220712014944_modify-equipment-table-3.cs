using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAPPlatform.DataAccess.Migrations
{
    public partial class modifyequipmenttable3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EQUIPMENT_EquipmentType_Eq~",
                table: "EQUIPMENT");

            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentTypeConfiguration~",
                table: "EquipmentTypeConfiguration");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EquipmentTypeConfiguration",
                table: "EquipmentTypeConfiguration");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EquipmentType",
                table: "EquipmentType");

            migrationBuilder.RenameTable(
                name: "EquipmentTypeConfiguration",
                newName: "EQUIPMENTTYPECONFIGURATION");

            migrationBuilder.RenameTable(
                name: "EquipmentType",
                newName: "EQUIPMENTTYPE");

            migrationBuilder.RenameColumn(
                name: "ConfigurationValue",
                table: "EQUIPMENTTYPECONFIGURATION",
                newName: "CONFIGURATIONVALUE");

            migrationBuilder.RenameColumn(
                name: "ConfigurationName",
                table: "EQUIPMENTTYPECONFIGURATION",
                newName: "CONFIGURATIONNAME");

            migrationBuilder.RenameColumn(
                name: "ConfigurationItem",
                table: "EQUIPMENTTYPECONFIGURATION",
                newName: "CONFIGURATIONITEM");

            migrationBuilder.RenameIndex(
                name: "IX_EquipmentTypeConfiguration~",
                table: "EQUIPMENTTYPECONFIGURATION",
                newName: "IX_EQUIPMENTTYPECONFIGURATION~");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "EQUIPMENTTYPE",
                newName: "NAME");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EQUIPMENTTYPECONFIGURATION",
                table: "EQUIPMENTTYPECONFIGURATION",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EQUIPMENTTYPE",
                table: "EQUIPMENTTYPE",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_EQUIPMENT_EQUIPMENTTYPE_Eq~",
                table: "EQUIPMENT",
                column: "EquipmentTypeId",
                principalTable: "EQUIPMENTTYPE",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_EQUIPMENTTYPECONFIGURATION~",
                table: "EQUIPMENTTYPECONFIGURATION",
                column: "TypeNameId",
                principalTable: "EQUIPMENTTYPE",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EQUIPMENT_EQUIPMENTTYPE_Eq~",
                table: "EQUIPMENT");

            migrationBuilder.DropForeignKey(
                name: "FK_EQUIPMENTTYPECONFIGURATION~",
                table: "EQUIPMENTTYPECONFIGURATION");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EQUIPMENTTYPECONFIGURATION",
                table: "EQUIPMENTTYPECONFIGURATION");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EQUIPMENTTYPE",
                table: "EQUIPMENTTYPE");

            migrationBuilder.RenameTable(
                name: "EQUIPMENTTYPECONFIGURATION",
                newName: "EquipmentTypeConfiguration");

            migrationBuilder.RenameTable(
                name: "EQUIPMENTTYPE",
                newName: "EquipmentType");

            migrationBuilder.RenameColumn(
                name: "CONFIGURATIONVALUE",
                table: "EquipmentTypeConfiguration",
                newName: "ConfigurationValue");

            migrationBuilder.RenameColumn(
                name: "CONFIGURATIONNAME",
                table: "EquipmentTypeConfiguration",
                newName: "ConfigurationName");

            migrationBuilder.RenameColumn(
                name: "CONFIGURATIONITEM",
                table: "EquipmentTypeConfiguration",
                newName: "ConfigurationItem");

            migrationBuilder.RenameIndex(
                name: "IX_EQUIPMENTTYPECONFIGURATION~",
                table: "EquipmentTypeConfiguration",
                newName: "IX_EquipmentTypeConfiguration~");

            migrationBuilder.RenameColumn(
                name: "NAME",
                table: "EquipmentType",
                newName: "Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EquipmentTypeConfiguration",
                table: "EquipmentTypeConfiguration",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EquipmentType",
                table: "EquipmentType",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_EQUIPMENT_EquipmentType_Eq~",
                table: "EQUIPMENT",
                column: "EquipmentTypeId",
                principalTable: "EquipmentType",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentTypeConfiguration~",
                table: "EquipmentTypeConfiguration",
                column: "TypeNameId",
                principalTable: "EquipmentType",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
