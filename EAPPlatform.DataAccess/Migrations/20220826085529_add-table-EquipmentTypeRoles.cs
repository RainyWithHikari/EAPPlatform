using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAPPlatform.DataAccess.Migrations
{
    public partial class addtableEquipmentTypeRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EQUIPMENTTYPEROLE",
                columns: table => new
                {
                    EQUIPMENTTYPEID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    FRAMEWORKROLEID = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK_EQUIPMENTTYPEROLE_EQUIPMEN~",
                        column: x => x.EQUIPMENTTYPEID,
                        principalTable: "EQUIPMENTTYPE",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EQUIPMENTTYPEROLE_Framewor~",
                        column: x => x.FRAMEWORKROLEID,
                        principalTable: "FrameworkRoles",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EQUIPMENTTYPEROLE_EQUIPMEN~",
                table: "EQUIPMENTTYPEROLE",
                column: "EQUIPMENTTYPEID");

            migrationBuilder.CreateIndex(
                name: "IX_EQUIPMENTTYPEROLE_FRAMEWOR~",
                table: "EQUIPMENTTYPEROLE",
                column: "FRAMEWORKROLEID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EQUIPMENTTYPEROLE");
        }
    }
}
