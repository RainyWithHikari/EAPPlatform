using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAPPlatform.DataAccess.Migrations
{
    public partial class addtable4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HISTORYRUNRATERESULT",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    EQID = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    RUNRATEVALUE = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    CHECKTIME = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    SHIFT = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HISTORYRUNRATERESULT", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HISTORYRUNRATERESULT");
        }
    }
}
