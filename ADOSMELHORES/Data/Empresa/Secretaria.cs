using System.ComponentModel.DataAnnotations;

namespace ADOSMELHORES.Data.Empresa
{
    public class Secretaria : Funcionario
    {
        [Required]

        public decimal Salario { get; set; }

        public string Area { get; set; } = string.Empty;

        public int DiretorId { get; set; }

        public Diretor? DiretorQueReporta { get; set; }
    }
}