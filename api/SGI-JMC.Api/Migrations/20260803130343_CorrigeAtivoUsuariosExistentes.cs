using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGI_JMC.Api.Migrations
{
    /// <inheritdoc />
    public partial class CorrigeAtivoUsuariosExistentes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Corrige usuários que ficaram com Ativo = false por causa do defaultValue
            // incorreto da migration anterior (AdicionaAtivoUsuario), que chegou a rodar
            // antes da correção para defaultValue: true.
            migrationBuilder.Sql("UPDATE \"AspNetUsers\" SET \"Ativo\" = true WHERE \"Ativo\" = false;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
