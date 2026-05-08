using System.ComponentModel.DataAnnotations;

namespace ADOSMELHORES.Data.Empresa
{
    public class Alocacao
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int? FormadorId { get; set; }
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
