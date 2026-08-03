using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SGI_JMC.Api.Migrations
{
    /// <inheritdoc />
    public partial class AdicionaTipoDeclaracaoENotificacoes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EscolaDestino",
                table: "Declaracoes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MotivoTransferencia",
                table: "Declaracoes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tipo",
                table: "Declaracoes",
                type: "text",
                nullable: false,
                defaultValue: "frequencia");

            migrationBuilder.CreateTable(
                name: "ConfiguracoesNotificacoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmailNotificacaoTransferencia = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracoesNotificacoes", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfiguracoesNotificacoes");

            migrationBuilder.DropColumn(
                name: "EscolaDestino",
                table: "Declaracoes");

            migrationBuilder.DropColumn(
                name: "MotivoTransferencia",
                table: "Declaracoes");

            migrationBuilder.DropColumn(
                name: "Tipo",
                table: "Declaracoes");
        }
    }
}
