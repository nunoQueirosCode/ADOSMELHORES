using ADOSMELHORES.Data.Empresa;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.ComponentModel.DataAnnotations;

namespace ADOSMELHORES.ViewModels
{
    public class FuncionarioViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tem que selecionar o tipo de funcionário.")]
        public string TipoFuncionario { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tem que inserir o nome do funcionário.")]
        public string Nome { get; set; } = string.Empty;
        public string Morada { get; set; } = string.Empty;
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
        public string Area { get; set; } = string.Empty;
        public int? DiretorId { get; set; }
        public List<SelectListItem> ListaDiretores { get; set; } = new List<SelectListItem>();

        //Formador
        public string AreaLecionada { get; set; } = string.Empty;
        public Formador.Disponibilidade TipoDisponibilidade { get; set; } 
        public decimal ValorHora { get; set; }
        public int? CoordenadorId { get; set; }
        public List<SelectListItem> ListaCoordenadores { get; set; } = new List<SelectListItem>();



    }
}
