using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventPlanner.Migrations
{
    /// <inheritdoc />
    public partial class CriacaoInicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Avaliacao",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Comentario = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    DataComentario = table.Column<DateOnly>(type: "date", nullable: false),
                    EventoID = table.Column<int>(type: "int", nullable: false),
                    UsuarioPremiumID = table.Column<int>(type: "int", nullable: true),
                    UsuarioID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Avaliacao", x => x.ID);
                });

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

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CPF = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    Telefone = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    Idade = table.Column<int>(type: "int", nullable: false),
                    Senha = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    eventoID = table.Column<int>(type: "int", nullable: true),
                    Tipo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Usuario_Evento_eventoID",
                        column: x => x.eventoID,
                        principalTable: "Evento",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "UsuarioPremium",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CPF = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Telefone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Idade = table.Column<int>(type: "int", nullable: false),
                    Senha = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CEP = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Bairro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cidade = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumeroCartao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TitularCartao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataValidade = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CodigoSeguranca = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    eventoID = table.Column<int>(type: "int", nullable: true),
                    usuarioID = table.Column<int>(type: "int", nullable: true),
                    AvaliacaoID = table.Column<int>(type: "int", nullable: true),
                    Tipo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioPremium", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UsuarioPremium_Avaliacao_AvaliacaoID",
                        column: x => x.AvaliacaoID,
                        principalTable: "Avaliacao",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_UsuarioPremium_Evento_eventoID",
                        column: x => x.eventoID,
                        principalTable: "Evento",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_UsuarioPremium_Usuario_usuarioID",
                        column: x => x.usuarioID,
                        principalTable: "Usuario",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_eventoID",
                table: "Usuario",
                column: "eventoID");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioPremium_AvaliacaoID",
                table: "UsuarioPremium",
                column: "AvaliacaoID");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioPremium_CPF",
                table: "UsuarioPremium",
                column: "CPF",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioPremium_Email",
                table: "UsuarioPremium",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioPremium_eventoID",
                table: "UsuarioPremium",
                column: "eventoID");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioPremium_usuarioID",
                table: "UsuarioPremium",
                column: "usuarioID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UsuarioPremium");

            migrationBuilder.DropTable(
                name: "Avaliacao");

            migrationBuilder.DropTable(
                name: "Usuario");

            migrationBuilder.DropTable(
                name: "Evento");
        }
    }
}
