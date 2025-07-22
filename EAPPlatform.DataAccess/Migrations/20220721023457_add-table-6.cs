using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAPPlatform.DataAccess.Migrations
{
    public partial class addtable6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HISTORYMTBARESULT",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    EQID = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    MTBAVALUE = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    CHECKTIME = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    SHIFT = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HISTORYMTBARESULT", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "MTBARESULT",
                columns: table => new
                {
                    EQID = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    MTBAVALUE = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    CHECKTIME = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MTBARESULT", x => x.EQID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HISTORYMTBARESULT");

            migrationBuilder.DropTable(
                name: "MTBARESULT");
        }
    }
}
