using ADOSMELHORES.Data.Empresa;
using Microsoft.EntityFrameworkCore;

namespace ADOSMELHORES.Data
{
    public class EmpresaContext : DbContext
    {
        public EmpresaContext(DbContextOptions<EmpresaContext> options) : base(options)

        {

        }

        public DbSet<Funcionario> Funcionarios { get; set; }
        public DbSet<Diretor> Diretores { get; set; }
        public DbSet<Secretaria> Secretarias { get; set; }
        public DbSet<Formador> Formadores { get; set; }
        public DbSet<Coordenador> Coordenadores { get; set; }

        public DbSet<Alocacao> Alocacoes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configurações, Aqui estamos dizendo que vai ser criada apenas uma tabela chamada Funcionarios
            modelBuilder.Entity<Funcionario>()
                .HasDiscriminator<string>("TipoFuncionario")    // Coluna extra para descriminar o tipo de funcionario
                .HasValue<Diretor>("Diretor")                   // Esses HaValue para dizer que quando guardar o objeto Diretor, escrever Diretor na coluna
                .HasValue<Secretaria>("Secretaria")
                .HasValue<Formador>("Formador")
                .HasValue<Coordenador>("Coordenador");

            // Loop para ajustar todas as variaveis decimais
            foreach (var property in modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                property.SetColumnType("decimal(18,2)");
            }


            // Tabela Secretaria com a ligação da Secretaria e Diretor
            modelBuilder.Entity<Secretaria>()
                .HasOne(secretaria => secretaria.DiretorQueReporta)     // Uma secretaria tem um diretor
                .WithMany(diretor => diretor.Secretarias)               // Um diretor  tem muitas secretarias
                .HasForeignKey(secretaria => secretaria.DiretorId)      // Esse DiretorId é a chave estrangeira na tabela Secretaria
                .OnDelete(DeleteBehavior.Restrict);                     // Esse Restrict evita apagar a secretaria em cascata se o diretor for apagado

            // 3. Tabela Formador com
            modelBuilder.Entity<Formador>()
                .HasOne(formador => formador.Coordenador)                   // Um formador tem um coordenador
                .WithMany(coordenador => coordenador.FormadoresAlocados)    // O coordenador tem uma lista de Formadores associada
                .HasForeignKey(formador => formador.CoordenadorId)          // A FK na tabela formador
                .OnDelete(DeleteBehavior.Restrict);

            // Relação Formador e Alocação
            modelBuilder.Entity<Alocacao>()
                .HasOne(alocacao => alocacao.Formador)
                .WithMany(formador => formador.Alocacoes)
                .HasForeignKey(alocacao => alocacao.FormadorId)
                .OnDelete(DeleteBehavior.SetNull); // <---  apenas limpa o ID

            // DATA SEEDING

            //// Guids fixos para podermos usar nas chaves estrangeiras
            var diretorId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var coordenadorId = Guid.Parse("22222222-2222-2222-2222-222222222222");
            var secretariaId = Guid.Parse("33333333-3333-3333-3333-333333333333");
            var formadorId = Guid.Parse("44444444-4444-4444-4444-444444444444");
            var alocacaoId = Guid.Parse("55555555-5555-5555-5555-555555555555");

            modelBuilder.Entity<Diretor>().HasData(new Diretor
            {
                Id = diretorId,
                Nome = "Mário Oliveira",
                Contacto = "912345678",
                DataFimContrato = new DateTime(2028, 12, 31),
                DataRegistoCriminal = new DateTime(2026, 5, 10),
                IsencaoHorario = true,
                Salario = 3500.00m,
                CarroEmpresa = true
            });

            modelBuilder.Entity<Coordenador>().HasData(new Coordenador
            {
                Id = coordenadorId,
                Nome = "Nuno Queirós",
                Contacto = "962345678",
                DataFimContrato = new DateTime(2026, 6, 1),
                DataRegistoCriminal = new DateTime(2026, 4, 15),
                Salario = 2200.00m
            });

            modelBuilder.Entity<Secretaria>().HasData(new Secretaria
            {
                Id = secretariaId,
                DiretorId = diretorId,
                Nome = "Bruna Buss",
                Contacto = "932345678",
                DataFimContrato = new DateTime(2026, 12, 31),
                DataRegistoCriminal = new DateTime(2026, 8, 20),
                Salario = 1100.00m,
                Area = "Recursos Humanos"
            });

            modelBuilder.Entity<Formador>().HasData(new Formador
            {
                Id = formadorId,
                CoordenadorId = coordenadorId,
                Nome = "Jorge Dias",
                Contacto = "919876543",
                DataFimContrato = new DateTime(2026, 12, 31),
                DataRegistoCriminal = new DateTime(2026, 9, 10),
                AreaLecionada = "Programação C#",
                ValorHora = 25.50m,
                TipoDisponibilidade = Formador.Disponibilidade.Ambas
            });

            modelBuilder.Entity<Alocacao>().HasData(new Alocacao
            {
                Id = alocacaoId,
                FormadorId = formadorId,
                DescricaoFormacao = "Curso Intensivo de ASP.NET MVC",
                DataInicio = new DateTime(2026, 5, 1),
                DataFim = new DateTime(2026, 5, 31)
            });
        }
    }
}
