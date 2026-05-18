using ADOSMELHORES.Data;
using ADOSMELHORES.Data.Empresa;
using ADOSMELHORES.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Text;

namespace ADOSMELHORES.Controllers
{
    public abstract class BaseController : Controller
    {
        protected readonly EmpresaContext _context;
        protected readonly IMemoryCache _cache;

        public BaseController(EmpresaContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        protected async Task<List<Funcionario>> ObterFuncionariosDaCache()
        {
            if (!_cache.TryGetValue(CacheKeys.ListaFuncionarios, out List<Funcionario> funcionarios))  // Tenta obter a lista do cache
            {
                funcionarios = await _context.Funcionarios.Include("Alocacoes").ToListAsync();
                _cache.Set(CacheKeys.ListaFuncionarios, funcionarios);  // Armazena a lista no cache para futuras requisições
            }
            return funcionarios;
        }

        protected DateTime ObterDataDoSistema()
        {
            string dataCookie = Request.Cookies["DataSistema"];
            DateTime dataAtualDoSistema;

            if (string.IsNullOrEmpty(dataCookie) || !DateTime.TryParse(dataCookie, out dataAtualDoSistema))  // Se o cookie não existir ou for inválido, define a data atual do sistema
            {
                dataAtualDoSistema = DateTime.Today;

                CookieOptions options = new CookieOptions
                {
                    Expires = DateTime.Now.AddMinutes(30),  // Define o tempo de expiração do cookie para 30 minutos
                    HttpOnly = true
                };

                Response.Cookies.Append("DataSistema", dataAtualDoSistema.ToString("yyyy-MM-dd"), options);  // Armazena a data atual do sistema no cookie para futuras requisições
            }
            return dataAtualDoSistema;
        }

        [HttpGet]
        public async Task<IActionResult> ExportarCSV()
        {
            var funcionarios = await _context.Funcionarios.ToListAsync();

            DateTime dataAtualSistema = ObterDataDoSistema();  // Obtém a data atual do sistema, considerando o cookie "DataSistema"

            if (!funcionarios.Any())
            {
                return BadRequest("Não há funcionários para exportar.");
            }

            var csv = new StringBuilder();

            csv.AppendLine("Id,Nome,Morada,Contacto,Tipo,DataFimContrato,DataRegistoCriminal,Salario,Area,AreaLecionada,ValorHora,IsencaoHorario,BonusMensal,CarroEmpresa,TipoDisponibilidade,DiretorId,CoordenadorId");

            foreach (var funcionario in funcionarios)  
            {
                var tipo = funcionario.GetType().Name;
                var salario = "null";  // Inicializa as variáveis com "null" para os campos que podem não ser aplicáveis a todos os tipos de funcionários, garantindo que o CSV tenha uma estrutura consistente mesmo quando algumas informações não estiverem disponíveis
                var area = "null";
                var areaLecionada = "null";
                var valorHora = "null";
                var isencaoHorario = "null";
                var bonusMensal = "null";
                var carroEmpresa = "null";
                var tipoDisponibilidade = "null";
                var diretorId = "null";
                var coordenadorId = "null";

                if (funcionario is Diretor d)
                {
                    salario = d.Salario.ToString("F2");  // Formata o salário com duas casas decimais para melhor legibilidade no CSV
                    isencaoHorario = d.IsencaoHorario.ToString();
                    bonusMensal = d.BonusMensal.HasValue ? d.BonusMensal.Value.ToString("F2") : "null";
                    carroEmpresa = d.CarroEmpresa.ToString();
                }
                else if (funcionario is Secretaria s)
                {
                    salario = s.Salario.ToString("F2"); 
                    area = s.Area;
                    diretorId = s.DiretorId.ToString() ?? "null";
                }
                else if (funcionario is Formador f)
                {
                    areaLecionada = f.AreaLecionada;
                    valorHora = f.ValorHora.ToString("F2");
                    tipoDisponibilidade = f.TipoDisponibilidade.ToString();
                    coordenadorId = f.CoordenadorId?.ToString() ?? "null";
                }
                else if (funcionario is Coordenador c)
                {
                    salario = c.Salario.ToString("F2");
                }

                var linha = $"\"{funcionario.Id}\",\"{funcionario.Nome}\",\"{funcionario.Morada}\"," + 
                    $"\"{funcionario.Contacto}\",\"{tipo}\",\"{funcionario.DataFimContrato:yyyy-MM-dd}\"," +
                    $"\"{funcionario.DataRegistoCriminal:yyyy-MM-dd}\",\"{salario}\",\"{area}\"," +
                    $"\"{areaLecionada}\",\"{valorHora}\",\"{isencaoHorario}\",\"{bonusMensal}\"," +
                    $"\"{carroEmpresa}\",\"{tipoDisponibilidade}\",\"{diretorId}\",\"{coordenadorId}\"";

                csv.AppendLine(linha);
            }

            var conteudo = Encoding.UTF8.GetBytes(csv.ToString());  
            return File(conteudo, "text/csv", $"Funcionarios_{dataAtualSistema:yyyyMMdd}.csv");  // Retorna o arquivo CSV para download, utilizando a data atual do sistema no nome do arquivo para facilitar a identificação da data de exportação dos dados
        }
    }
}
