using System.ComponentModel.DataAnnotations;

namespace ADOSMELHORES.Data.Empresa
{
    public class Diretor : Funcionario
    {
        public bool IsencaoHorario { get; set; }

        [DataType(DataType.Currency)]

        public decimal salario { get; set; }

        public decimal BonusMensal { get; set; }

        public bool CarroEmpresa { get; set; }

        public ICollection<Secretaria> Secretarias { get; set; } = new List<Secretaria>();
    }
}