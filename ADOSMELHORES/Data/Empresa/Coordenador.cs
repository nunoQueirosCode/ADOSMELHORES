namespace ADOSMELHORES.Data.Empresa
{
    public class Coordenador : Funcionario
    {
        public int Salario { get; set; }

        public List <Formador> FormadoresAlocados { get; set; } = new List<Formador>();
    }
}
