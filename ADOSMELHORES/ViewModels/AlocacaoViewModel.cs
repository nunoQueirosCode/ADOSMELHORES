using System.ComponentModel.DataAnnotations;

namespace ADOSMELHORES.ViewModels
{
    public class AlocacaoViewModel
    {
        public Guid Id { get; set; }

        public string NomeFormador { get; set; } = string.Empty;

        public string DescricaoFormacao { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime DataInicio { get; set; }

        [DataType(DataType.Date)]
        public DateTime DataFim { get; set; }
    }
}