using Microsoft.EntityFrameworkCore.Migrations;

namespace ExemploMeetingHangfire.Migrations
{
    public partial class AdicionadoColunaProcessando : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Processando",
                table: "Postos",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Processando",
                table: "Postos");
        }
    }
}
