using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAPPlatform.DataAccess.Migrations
{
    public partial class addtableAlarmPositions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ALARMPOSITIONS",
                columns: table => new
                {
                    EQID = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    ALARMCODE = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    POSITIONID = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ALARMPOSITIONS");
        }
    }
}
