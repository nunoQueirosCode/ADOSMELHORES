using System.ComponentModel.DataAnnotations;

namespace ADOSMELHORES.Data.Empresa
{
    public class Alocacao
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid? FormadorId { get; set; }
        public Formador? Formador { get; set; }

        public decimal ValorHoraNoMomento { get; set; } // Importante para histórico de preços

        [Required]
        public string DescricaoFormacao { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime DataInicio { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DataFim { get; set; }

    }
}
