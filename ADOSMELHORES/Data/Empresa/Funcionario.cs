using System.ComponentModel.DataAnnotations;

namespace ADOSMELHORES.Data.Empresa
{
    public abstract class Funcionario
    {
        [Key]
        public int Id { get; set; }

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
