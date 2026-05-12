using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADOSMELHORES.Migrations
{
    /// <inheritdoc />
    public partial class first : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Funcionarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Morada = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Contacto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataFimContrato = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataRegistoCriminal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TipoFuncionario = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    Coordenador_Salario = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IsencaoHorario = table.Column<bool>(type: "bit", nullable: true),
                    Salario = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    BonusMensal = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CarroEmpresa = table.Column<bool>(type: "bit", nullable: true),
                    AreaLecionada = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TipoDisponibilidade = table.Column<int>(type: "int", nullable: true),
                    ValorHora = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CoordenadorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Secretaria_Salario = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Area = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiretorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Funcionarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Funcionarios_Funcionarios_CoordenadorId",
                        column: x => x.CoordenadorId,
                        principalTable: "Funcionarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Funcionarios_Funcionarios_DiretorId",
                        column: x => x.DiretorId,
                        principalTable: "Funcionarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Alocacoes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FormadorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ValorHoraNoMomento = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DescricaoFormacao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataFim = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alocacoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Alocacoes_Funcionarios_FormadorId",
                        column: x => x.FormadorId,
                        principalTable: "Funcionarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alocacoes_FormadorId",
                table: "Alocacoes",
                column: "FormadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Funcionarios_CoordenadorId",
                table: "Funcionarios",
                column: "CoordenadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Funcionarios_DiretorId",
                table: "Funcionarios",
                column: "DiretorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alocacoes");

            migrationBuilder.DropTable(
                name: "Funcionarios");
        }
    }
}
