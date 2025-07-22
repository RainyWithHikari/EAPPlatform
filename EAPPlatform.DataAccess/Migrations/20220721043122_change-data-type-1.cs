using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAPPlatform.DataAccess.Migrations
{
    public partial class changedatatype1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "MTBAVALUE",
                table: "MTBARESULT",
                type: "NUMBER",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "BINARY_DOUBLE")
                .Annotation("Relational:ColumnOrder", 2);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "MTBAVALUE",
                table: "MTBARESULT",
                type: "BINARY_DOUBLE",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "NUMBER")
                .OldAnnotation("Relational:ColumnOrder", 2);
        }
    }
}
