using System.ComponentModel.DataAnnotations;

namespace ADOSMELHORES.Data.Empresa
{
    public class Secretaria : Funcionario
    {
        [Required]

        public string Area { get; set; } = string.Empty;

        public int DiretorId { get; set; }

        public Diretor? DiretoQueReporta { get; set; }
    }
}
