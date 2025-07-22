using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAPPlatform.DataAccess.Migrations
{
    public partial class add_equipmentalarm_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EQUIPMENTALARM",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    EQID = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    ALARMCODE = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    ALARMTEXT = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: true),
                    ALARMSOURCE = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    ALARMTIME = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    ALARMSET = table.Column<bool>(type: "NUMBER(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EQUIPMENTALARM", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EQUIPMENTALARM");
        }
    }
}
