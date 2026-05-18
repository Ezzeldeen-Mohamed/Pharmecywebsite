using Microsoft.EntityFrameworkCore.Migrations;

namespace ePharma_asp_mvc.Migrations
{
    public partial class Prescpectionaddress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Prescriptions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Prescriptions",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Prescriptions");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Prescriptions");
        }
    }
}
