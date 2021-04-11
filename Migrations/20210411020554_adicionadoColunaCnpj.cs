using Microsoft.EntityFrameworkCore.Migrations;

namespace ExemploMeetingHangfire.Migrations
{
    public partial class adicionadoColunaCnpj : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Cnpj",
                table: "PostosParaAtualizar",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cnpj",
                table: "Postos",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cnpj",
                table: "PostosParaAtualizar");

            migrationBuilder.DropColumn(
                name: "Cnpj",
                table: "Postos");
        }
    }
}
