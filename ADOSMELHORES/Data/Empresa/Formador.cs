namespace ADOSMELHORES.Data.Empresa
{
    public class Formador : Funcionario
    {
        public enum Disponibilidade
        {
            Laboral,
            PosLaboral,
            Ambas
        }

        public string AreaLecionada { get; set; } = string.Empty;

        public Disponibilidade TipoDisponibilidade { get; set; }
        
        public decimal ValorHora { get; set; }

        public int? CoordenadorId { get; set; }
        public Coordenador? Coordenador { get; set; }
    }
}
