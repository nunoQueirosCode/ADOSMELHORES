using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADOSMELHORES.Migrations
{
    /// <inheritdoc />
    public partial class DbSeeds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Funcionarios",
                columns: new[] { "Id", "BonusMensal", "CarroEmpresa", "Contacto", "DataFimContrato", "DataRegistoCriminal", "IsencaoHorario", "Morada", "Nome", "Salario", "TipoFuncionario" },
                values: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), null, true, "912345678", new DateTime(2028, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 5, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "", "Mário Oliveira", 3500.00m, "Diretor" });

            migrationBuilder.InsertData(
                table: "Funcionarios",
                columns: new[] { "Id", "Contacto", "DataFimContrato", "DataRegistoCriminal", "Morada", "Nome", "Coordenador_Salario", "TipoFuncionario" },
                values: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), "962345678", new DateTime(2026, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "Nuno Queirós", 2200.00m, "Coordenador" });

            migrationBuilder.InsertData(
                table: "Funcionarios",
                columns: new[] { "Id", "Area", "Contacto", "DataFimContrato", "DataRegistoCriminal", "DiretorId", "Morada", "Nome", "Secretaria_Salario", "TipoFuncionario" },
                values: new object[] { new Guid("33333333-3333-3333-3333-333333333333"), "Recursos Humanos", "932345678", new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("11111111-1111-1111-1111-111111111111"), "", "Bruna Buss", 1100.00m, "Secretaria" });

            migrationBuilder.InsertData(
                table: "Funcionarios",
                columns: new[] { "Id", "AreaLecionada", "Contacto", "CoordenadorId", "DataFimContrato", "DataRegistoCriminal", "Morada", "Nome", "TipoDisponibilidade", "TipoFuncionario", "ValorHora" },
                values: new object[] { new Guid("44444444-4444-4444-4444-444444444444"), "Programação C#", "919876543", new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "Jorge Dias", 2, "Formador", 25.50m });

            migrationBuilder.InsertData(
                table: "Alocacoes",
                columns: new[] { "Id", "DataFim", "DataInicio", "DescricaoFormacao", "FormadorId" },
                values: new object[] { new Guid("55555555-5555-5555-5555-555555555555"), new DateTime(2026, 5, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Curso Intensivo de ASP.NET MVC", new Guid("44444444-4444-4444-4444-444444444444") });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Alocacoes",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"));

            migrationBuilder.DeleteData(
                table: "Funcionarios",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "Funcionarios",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "Funcionarios",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"));

            migrationBuilder.DeleteData(
                table: "Funcionarios",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));
        }
    }
}
