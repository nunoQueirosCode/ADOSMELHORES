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

    }
}
