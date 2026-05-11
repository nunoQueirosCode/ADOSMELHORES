using System.ComponentModel.DataAnnotations;

namespace ADOSMELHORES.Data.Empresa
{
    public abstract class Funcionario
    {
        [Key]
        public Guid Id { get; set; }= Guid.NewGuid();

        [Required]
        public string Nome { get; set; } = string.Empty;

        public string Morada { get; set; } = string.Empty;

        [Phone]
        public string Contacto { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime DataFimContrato { get; set; }

        [DataType(DataType.Date)]
        public DateTime DataRegistoCriminal { get; set; }
    }
}
