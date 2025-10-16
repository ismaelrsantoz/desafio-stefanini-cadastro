using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CadastroPessoas.Api.Migrations
{
    /// <inheritdoc />
    public partial class CriacaoInicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Pessoas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", nullable: false),
                    Sexo = table.Column<string>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    DataNascimento = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Naturalidade = table.Column<string>(type: "TEXT", nullable: true),
                    Nacionalidade = table.Column<string>(type: "TEXT", nullable: true),
                    Cpf = table.Column<string>(type: "TEXT", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pessoas", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Pessoas");
        }
    }
}
