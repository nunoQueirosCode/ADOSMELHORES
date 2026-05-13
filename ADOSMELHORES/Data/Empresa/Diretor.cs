using System.ComponentModel.DataAnnotations;

namespace ADOSMELHORES.Data.Empresa
{
    public class Diretor : Funcionario
    {
        public bool IsencaoHorario { get; set; }

        public decimal Salario { get; set; }

        public decimal? BonusMensal { get; set; }

        public bool CarroEmpresa { get; set; }

        public ICollection<Secretaria> Secretarias { get; set; } = new List<Secretaria>();
    }
}