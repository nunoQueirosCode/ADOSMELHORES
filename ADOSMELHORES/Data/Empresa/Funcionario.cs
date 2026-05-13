using System.ComponentModel.DataAnnotations;

namespace ADOSMELHORES.Data.Empresa
{
    public abstract class Funcionario
    {
        [Key]
        public Guid Id { get; set; }= Guid.NewGuid();

        [Required(ErrorMessage = "Campo obrigatório.")]
        public string Nome { get; set; } = string.Empty;

        public string? Morada { get; set; } = string.Empty;

        [Required(ErrorMessage = "Campo obrigatório.")]
        [RegularExpression(@"^(9[1236]\d{7}|2\d{8})$", ErrorMessage = "Insira um número válido com 9 dígitos (ex: 912345678 ou 212345678).")]
        public string Contacto { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime DataFimContrato { get; set; }

        [DataType(DataType.Date)]
        public DateTime DataRegistoCriminal { get; set; }


    }
}
