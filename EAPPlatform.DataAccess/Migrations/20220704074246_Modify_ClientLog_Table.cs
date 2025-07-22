using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAPPlatform.DataAccess.Migrations
{
    public partial class Modify_ClientLog_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ClientLog",
                table: "ClientLog");

            migrationBuilder.RenameTable(
                name: "ClientLog",
                newName: "CLIENTLOG");

            migrationBuilder.RenameColumn(
                name: "LogType",
                table: "CLIENTLOG",
                newName: "LOGTYPE");

            migrationBuilder.RenameColumn(
                name: "LogLevel",
                table: "CLIENTLOG",
                newName: "LOGLEVEL");

            migrationBuilder.RenameColumn(
                name: "LogContent",
                table: "CLIENTLOG",
                newName: "LOGCONTENT");

            migrationBuilder.RenameColumn(
                name: "DateTime",
                table: "CLIENTLOG",
                newName: "DATETIME");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CLIENTLOG",
                table: "CLIENTLOG",
                column: "ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CLIENTLOG",
                table: "CLIENTLOG");

            migrationBuilder.RenameTable(
                name: "CLIENTLOG",
                newName: "ClientLog");

            migrationBuilder.RenameColumn(
                name: "LOGTYPE",
                table: "ClientLog",
                newName: "LogType");

            migrationBuilder.RenameColumn(
                name: "LOGLEVEL",
                table: "ClientLog",
                newName: "LogLevel");

            migrationBuilder.RenameColumn(
                name: "LOGCONTENT",
                table: "ClientLog",
                newName: "LogContent");

            migrationBuilder.RenameColumn(
                name: "DATETIME",
                table: "ClientLog",
                newName: "DateTime");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClientLog",
                table: "ClientLog",
                column: "ID");
        }
    }
}
