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
            modelBuilder.Entity<Funcionario>()
                .HasDiscriminator<string>("TipoFuncionario")   
                .HasValue<Diretor>("Diretor")                  
                .HasValue<Secretaria>("Secretaria")
                .HasValue<Formador>("Formador")
                .HasValue<Coordenador>("Coordenador");

            foreach (var property in modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                property.SetColumnType("decimal(18,2)");
            }


            modelBuilder.Entity<Secretaria>()
                .HasOne(secretaria => secretaria.DiretorQueReporta)     // Uma secretaria tem um diretor
                .WithMany(diretor => diretor.Secretarias)               // Um diretor tem muitas secretarias
                .HasForeignKey(secretaria => secretaria.DiretorId)      // Esse DiretorId é a chave estrangeira na tabela Secretaria
                .OnDelete(DeleteBehavior.Restrict);                     // Esse Restrict evita apagar a secretaria em cascata se o diretor for apagado

            modelBuilder.Entity<Formador>()
                .HasOne(formador => formador.Coordenador)                   // Um formador tem um coordenador
                .WithMany(coordenador => coordenador.FormadoresAlocados)    // O coordenador tem uma lista de Formadores associada
                .HasForeignKey(formador => formador.CoordenadorId)          // A FK na tabela formador
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Alocacao>()
                .HasOne(alocacao => alocacao.Formador)
                .WithMany(formador => formador.Alocacoes)
                .HasForeignKey(alocacao => alocacao.FormadorId)
                .OnDelete(DeleteBehavior.SetNull); // <---  apenas limpa o ID
        }
    }
}
