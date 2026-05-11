namespace ADOSMELHORES.ViewModels
{
    public class ItemAlocacaoViewModel
    {
        public Guid AlocacaoId { get; set; }
        public string NomeFuncionario { get; set; } = string.Empty;
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
    }
    public class HomeIndexViewModel
    {
        public decimal TotalGeral { get; set; }
        public int QtdFuncionarios { get; set; }
        public int QtdFuncionariosContratos { get; set; }
        public int QtdFuncionariosRegistoCriminal { get; set; }
        public int QtdDiretores { get; set; }

        public int QtdCoordenadores { get; set; }
        public int QtdSecretarias { get; set; }
        public int QtdFormadores { get; set; }
        public List<ItemAlocacaoViewModel>? TabelaAlocacoes { get; set; }

    }
}
