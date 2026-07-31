using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SGI_JMC.Api.Migrations
{
    /// <inheritdoc />
    public partial class OficioFuncao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OficiosFuncao",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Tipo = table.Column<string>(type: "text", nullable: false),
                    NumeroOficio = table.Column<int>(type: "integer", nullable: false),
                    Assunto = table.Column<string>(type: "text", nullable: false),
                    Destinatario = table.Column<string>(type: "text", nullable: false),
                    CargoDestinatario = table.Column<string>(type: "text", nullable: false),
                    CidadeDestinatario = table.Column<string>(type: "text", nullable: false),
                    SaudacaoGenero = table.Column<string>(type: "text", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    CPF = table.Column<string>(type: "text", nullable: false),
                    Vinculo = table.Column<string>(type: "text", nullable: false),
                    DataAssumiuFuncao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CargaHoraria = table.Column<int>(type: "integer", nullable: false),
                    Cargo = table.Column<string>(type: "text", nullable: true),
                    Disciplina = table.Column<string>(type: "text", nullable: true),
                    FonteRecursos = table.Column<int>(type: "integer", nullable: true),
                    DataEmissao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EmitidoPor = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OficiosFuncao", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OficiosFuncao");
        }
    }
}
