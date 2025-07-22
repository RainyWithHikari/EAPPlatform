using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAPPlatform.DataAccess.Migrations
{
    public partial class addentity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "REMARK",
                table: "EQUIPMENTPARAMSHISCAL",
                type: "NVARCHAR2(2000)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "REMARK",
                table: "EQUIPMENTPARAMSHISCAL");
        }
    }
}
