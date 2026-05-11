namespace ADOSMELHORES.Data.Empresa
{
    public class Coordenador : Funcionario
    {
        public decimal Salario { get; set; }

        public ICollection<Formador> FormadoresAlocados { get; set; } = new List<Formador>();
    }
}
