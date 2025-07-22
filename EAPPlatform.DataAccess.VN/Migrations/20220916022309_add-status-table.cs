using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAPPlatform.DataAccess.VN.Migrations
{
    public partial class addstatustable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }
    }
}
