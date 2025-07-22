using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAPPlatform.DataAccess.Migrations
{
    public partial class add_ClientLog_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name1",
                table: "TestModel",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ClientLog",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    DateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    EQID = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    LogType = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    LogLevel = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    LogContent = table.Column<string>(type: "NVARCHAR2(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientLog", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientLog");

            migrationBuilder.DropColumn(
                name: "Name1",
                table: "TestModel");
        }
    }
}
