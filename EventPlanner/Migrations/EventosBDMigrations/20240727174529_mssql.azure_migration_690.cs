using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventPlanner.Migrations.EventosBDMigrations
{
    /// <inheritdoc />
    public partial class mssqlazure_migration_690 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Evento",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomeEvento = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DataInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataFinal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TempoAteEvento = table.Column<TimeSpan>(type: "time", nullable: false),
                    Duracao = table.Column<TimeSpan>(type: "time", nullable: false),
                    QuantMetrosQuadrados = table.Column<int>(type: "int", nullable: true),
                    QuantMaxPessoas = table.Column<int>(type: "int", nullable: false),
                    QuantCriancas = table.Column<int>(type: "int", nullable: true),
                    QuantRefri = table.Column<int>(type: "int", nullable: true),
                    QuantAlcool = table.Column<int>(type: "int", nullable: true),
                    QuantCarne = table.Column<int>(type: "int", nullable: true),
                    QuantDoces = table.Column<int>(type: "int", nullable: true),
                    QuantSalgados = table.Column<int>(type: "int", nullable: true),
                    QuantCadeiras = table.Column<int>(type: "int", nullable: true),
                    CEP = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cidade = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Bairro = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FotoDoEvento = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    Identificador = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TipoEvento = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Organizador = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UsuarioID = table.Column<int>(type: "int", nullable: true),
                    UsuarioPremiumID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Evento", x => x.ID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Evento");
        }
    }
}
