using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAPPlatform.DataAccess.Migrations
{
    public partial class changedatatype2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "RUNRATEVALUE",
                table: "RUNRATERESULT",
                type: "NUMBER",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "BINARY_DOUBLE");

            migrationBuilder.AlterColumn<decimal>(
                name: "MTBAVALUE",
                table: "MTBARESULT",
                type: "NUMBER",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "NUMBER")
                .OldAnnotation("Relational:ColumnOrder", 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "RUNRATEVALUE",
                table: "HISTORYRUNRATERESULT",
                type: "NUMBER",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "BINARY_DOUBLE");

            migrationBuilder.AlterColumn<decimal>(
                name: "MTBAVALUE",
                table: "HISTORYMTBARESULT",
                type: "NUMBER",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "BINARY_DOUBLE");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "RUNRATEVALUE",
                table: "RUNRATERESULT",
                type: "BINARY_DOUBLE",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "NUMBER");

            migrationBuilder.AlterColumn<decimal>(
                name: "MTBAVALUE",
                table: "MTBARESULT",
                type: "NUMBER",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "NUMBER")
                .Annotation("Relational:ColumnOrder", 2);

            migrationBuilder.AlterColumn<double>(
                name: "RUNRATEVALUE",
                table: "HISTORYRUNRATERESULT",
                type: "BINARY_DOUBLE",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "NUMBER");

            migrationBuilder.AlterColumn<double>(
                name: "MTBAVALUE",
                table: "HISTORYMTBARESULT",
                type: "BINARY_DOUBLE",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "NUMBER");
        }
    }
}
