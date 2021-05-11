using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Agenda.Desktop.Migrations
{
    public partial class CreateDbSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Eventos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(maxLength: 70, nullable: false),
                    Descricao = table.Column<string>(maxLength: 500, nullable: true),
                    DataInicial = table.Column<DateTime>(nullable: false),
                    DataFinal = table.Column<DateTime>(nullable: false),
                    Local = table.Column<string>(maxLength: 40, nullable: false),
                    Participantes = table.Column<int>(nullable: false),
                    Tipo = table.Column<string>(nullable: false),
                    DataCriado = table.Column<DateTime>(nullable: false),
                    DataAlterado = table.Column<DateTime>(nullable: false),
                    AppIdentityUserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Eventos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ParticipantesEmEventos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdParticipante = table.Column<string>(nullable: true),
                    EventoId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParticipantesEmEventos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParticipantesEmEventos_Eventos_EventoId",
                        column: x => x.EventoId,
                        principalTable: "Eventos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantesEmEventos_EventoId",
                table: "ParticipantesEmEventos",
                column: "EventoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ParticipantesEmEventos");

            migrationBuilder.DropTable(
                name: "Eventos");
        }
    }
}
