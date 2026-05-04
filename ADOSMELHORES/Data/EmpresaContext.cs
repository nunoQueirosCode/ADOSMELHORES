using Microsoft.EntityFrameworkCore;

namespace ADOSMELHORES.Data
{
    public class EmpresaContext : DbContext
    {
        public EmpresaContext(DbContextOptions<EmpresaContext> options) : base(options)

        {

        }

    }
}
