using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SGI_JMC.Api.Migrations
{
    /// <inheritdoc />
    public partial class AdvertenciasSuspensoes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Advertencias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NomeAluno = table.Column<string>(type: "text", nullable: false),
                    NomePai = table.Column<string>(type: "text", nullable: true),
                    NomeMae = table.Column<string>(type: "text", nullable: false),
                    DataNascimento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AnoSerie = table.Column<string>(type: "text", nullable: false),
                    Turma = table.Column<string>(type: "text", nullable: false),
                    Turno = table.Column<string>(type: "text", nullable: false),
                    CodigoSeed = table.Column<string>(type: "text", nullable: false),
                    DescricaoDoFato = table.Column<string>(type: "text", nullable: false),
                    Numero = table.Column<int>(type: "integer", nullable: false),
                    DataDeEmissao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EmitidoPor = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Advertencias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Suspensoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NomeAluno = table.Column<string>(type: "text", nullable: false),
                    NomePai = table.Column<string>(type: "text", nullable: true),
                    NomeMae = table.Column<string>(type: "text", nullable: false),
                    DataNascimento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AnoSerie = table.Column<string>(type: "text", nullable: false),
                    Turma = table.Column<string>(type: "text", nullable: false),
                    Turno = table.Column<string>(type: "text", nullable: false),
                    CodigoSeed = table.Column<string>(type: "text", nullable: false),
                    DescricaoDoFato = table.Column<string>(type: "text", nullable: false),
                    Dias = table.Column<int>(type: "integer", nullable: false),
                    NumeroSuspensao = table.Column<int>(type: "integer", nullable: false),
                    DataDeEmissao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EmitidoPor = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suspensoes", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Advertencias");

            migrationBuilder.DropTable(
                name: "Suspensoes");
        }
    }
}
