using ADOSMELHORES.Data.Empresa;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.ComponentModel.DataAnnotations;

namespace ADOSMELHORES.ViewModels
{
    public class FuncionarioViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Campo obrigatório.")]
        public string TipoFuncionario { get; set; } = string.Empty;

        [Required(ErrorMessage = "Campo obrigatório.")]
        public string Nome { get; set; } = string.Empty;
        public string? Morada { get; set; } = string.Empty;
        [Required(ErrorMessage = "Campo obrigatório.")]
        [RegularExpression(@"^(9[1236]\d{7}|2\d{8})$", ErrorMessage = "Insira um número válido com 9 dígitos (ex: 912345678 ou 212345678).")]
        public string Contacto { get; set; } = string.Empty;
        public decimal Salario { get; set; }

        [DataType(DataType.Date)]
        public DateTime DataFimContrato { get; set; }

        [DataType(DataType.Date)]
        public DateTime DataRegistoCriminal { get; set; }
        //Diretor
        public bool IsencaoHorario { get; set; }
        public decimal BonusMensal { get; set; }
        public bool CarroEmpresa { get; set; }
        //Secretaria
        public string? Area { get; set; } = string.Empty;
        public Guid? DiretorId { get; set; }
        public List<SelectListItem> ListaDiretores { get; set; } = new List<SelectListItem>();

        //Formador
        public string? AreaLecionada { get; set; } = string.Empty;
        public Formador.Disponibilidade TipoDisponibilidade { get; set; } 
        public decimal ValorHora { get; set; }
        public Guid? CoordenadorId { get; set; }
        public List<SelectListItem> ListaCoordenadores { get; set; } = new List<SelectListItem>();



    }
}
