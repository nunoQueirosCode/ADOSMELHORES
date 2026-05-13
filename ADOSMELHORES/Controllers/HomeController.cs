using ADOSMELHORES.Data;
using ADOSMELHORES.Data.Empresa;
using ADOSMELHORES.Models;
using ADOSMELHORES.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;
using System.Text;

namespace ADOSMELHORES.Controllers
{
    public class HomeController : Controller
    {
        private readonly EmpresaContext _context;
        private readonly IMemoryCache _cache;

        public HomeController(EmpresaContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<IActionResult> Index()
        {
            DateTime dataAtualDoSistema = ObterDataDoSistema();

            var funcionarios = await ObterFuncionariosDaCache();

            var model = new HomeIndexViewModel();

            decimal TotalDiretores = funcionarios.OfType<Diretor>().Sum(d => d.Salario + (d.BonusMensal ?? 0));
            model.TotalDiretores = TotalDiretores;
            decimal TotalSecretarias = funcionarios.OfType<Secretaria>().Sum(s => s.Salario);
            model.TotalSecretarias = TotalSecretarias;
            decimal TotalCoordenadores = funcionarios.OfType<Coordenador>().Sum(c => c.Salario);
            model.TotalCoordenadores = TotalCoordenadores;

            DateTime inicioDoMes = new DateTime(dataAtualDoSistema.Year, dataAtualDoSistema.Month, 1);
            DateTime fimDoMes = inicioDoMes.AddMonths(1).AddDays(-1);

            decimal TotalFormadores = funcionarios.OfType<Formador>().Sum(f => f.Alocacoes?
                .Where(a => a.DataInicio <= fimDoMes && a.DataFim >= inicioDoMes)
                .Sum(a => {
                    DateTime dataCalculoInicio = a.DataInicio < inicioDoMes ? inicioDoMes : a.DataInicio;
                    DateTime dataCalculoFim = a.DataFim > fimDoMes ? fimDoMes : a.DataFim;
                    return ContarDiasUteis(dataCalculoInicio, dataCalculoFim) * 6 * f.ValorHora;
                }) ?? 0);

            model.TotalFormadores = TotalFormadores;    

            model.TotalGeral = TotalDiretores + TotalSecretarias + TotalCoordenadores + TotalFormadores;

            model.QtdFuncionarios = funcionarios.Count();

            model.QtdFuncionariosContratos = funcionarios.Count(f =>
                f.DataFimContrato >= dataAtualDoSistema &&
                f.DataFimContrato <= dataAtualDoSistema.AddDays(30));

            model.QtdFuncionariosRegistoCriminal = funcionarios.Count(f => f.DataRegistoCriminal < dataAtualDoSistema);

            model.QtdDiretores = funcionarios.OfType<Diretor>().Count();
            model.QtdCoordenadores = funcionarios.OfType<Coordenador>().Count();
            model.QtdSecretarias = funcionarios.OfType<Secretaria>().Count();
            model.QtdFormadores = funcionarios.OfType<Formador>().Count();

            model.TabelaAlocacoes = funcionarios.OfType<Formador>()
                    .SelectMany(f => f.Alocacoes, (f, a) => new { Funcionario = f, Alocacao = a })
                    .Where(x => x.Alocacao.DataInicio <= dataAtualDoSistema && x.Alocacao.DataFim >= dataAtualDoSistema)
                    .OrderBy(x => x.Alocacao.DataFim)
                    .Take(6)
                    .Select(x => new ItemAlocacaoViewModel
                    {
                        AlocacaoId = x.Alocacao.Id,
                        Descricao = x.Alocacao.DescricaoFormacao,
                        NomeFuncionario = x.Funcionario.Nome, // Pegamos no nome do funcionário
                        DataInicio = x.Alocacao.DataInicio,
                        DataFim = x.Alocacao.DataFim
                    })
                    .ToList();

            return View(model);
        }

        [HttpPost]
        public IActionResult DefinirDataSistema(DateTime novaDataSistema)
        {
            CookieOptions options = new CookieOptions
            {
                Expires = DateTime.Now.AddMinutes(30),
                HttpOnly = true
            };

            Response.Cookies.Append("DataSistema", novaDataSistema.ToString("yyyy-MM-dd"), options);

            return RedirectToAction(nameof(Index)); 
        }

        public IActionResult ResetData()
        {
            Response.Cookies.Delete("DataSistema");

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> CalcularDespesaMensal()
        {
   
            DateTime dataAtualDoSistema = ObterDataDoSistema();

            var funcionarios = await ObterFuncionariosDaCache();

            var model = new HomeDashboardViewModel();

            model.TotalDiretores = funcionarios.OfType<Diretor>().Sum(d => d.Salario + (d.BonusMensal ?? 0));
            model.TotalSecretarias = funcionarios.OfType<Secretaria>().Sum(s => s.Salario);
            model.TotalCoordenadores = funcionarios.OfType<Coordenador>().Sum(c => c.Salario);

            DateTime inicioDoMes = new DateTime(dataAtualDoSistema.Year, dataAtualDoSistema.Month, 1);
            DateTime fimDoMes = inicioDoMes.AddMonths(1).AddDays(-1);

            model.TotalFormadores = funcionarios.OfType<Formador>().Sum(f => f.Alocacoes?
                .Where(a => a.DataInicio <= fimDoMes && a.DataFim >= inicioDoMes)
                .Sum(a => {DateTime dataCalculoInicio = a.DataInicio < inicioDoMes ? inicioDoMes : a.DataInicio;
                           DateTime dataCalculoFim = a.DataFim > fimDoMes ? fimDoMes : a.DataFim;
                           return ContarDiasUteis(dataCalculoInicio, dataCalculoFim) * 6 * f.ValorHora;
                           }) ?? 0);

            model.TotalGeral = model.TotalDiretores + model.TotalSecretarias + model.TotalCoordenadores + model.TotalFormadores;

            model.QtdFuncionarios = funcionarios.Count;

            return View(model);
        }

        private int ContarDiasUteis(DateTime inicio, DateTime fim)
        {
            int diasUteis = 0;
            for (var data = inicio; data <= fim; data = data.AddDays(1))
            {
                if (data.DayOfWeek != DayOfWeek.Saturday && data.DayOfWeek != DayOfWeek.Sunday)
                {
                    diasUteis++;
                }
            }
            return diasUteis;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public async Task<IActionResult> ExportarCSV()
        {
            var funcionarios = await _context.Funcionarios.ToListAsync();

            if (!funcionarios.Any())
            {
                return BadRequest("Não há funcionários para exportar.");
            }

            var csv = new StringBuilder();

            // Cabeçalho do CSV
            csv.AppendLine("Id,Nome,Morada,Contacto,Tipo,DataFimContrato,DataRegistoCriminal,Salario,Area,AreaLecionada,ValorHora,IsencaoHorario,BonusMensal,CarroEmpresa,TipoDisponibilidade,DiretorId,CoordenadorId");

            // Linhas de dados
            foreach (var funcionario in funcionarios)
            {
                var tipo = funcionario.GetType().Name;
                var salario = "null";
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
                    salario = d.Salario.ToString("F2");
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
            return File(conteudo, "text/csv", $"Funcionarios_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
        }
        private async Task<List<Funcionario>> ObterFuncionariosDaCache()
        {
            if (!_cache.TryGetValue(CacheKeys.ListaFuncionarios, out List<Funcionario> funcionarios))
            {
                funcionarios = await _context.Funcionarios.Include("Alocacoes").ToListAsync();

                _cache.Set(CacheKeys.ListaFuncionarios, funcionarios);
            }

            return funcionarios;
        }
        protected DateTime ObterDataDoSistema()
        {
            string dataCookie = Request.Cookies["DataSistema"];
            DateTime dataAtualDoSistema;

            if (string.IsNullOrEmpty(dataCookie) || !DateTime.TryParse(dataCookie, out dataAtualDoSistema))
            {
                dataAtualDoSistema = DateTime.Today;

                CookieOptions options = new CookieOptions
                {
                    Expires = DateTime.Now.AddMinutes(30),
                    HttpOnly = true
                };

                Response.Cookies.Append("DataSistema", dataAtualDoSistema.ToString("yyyy-MM-dd"), options);
            }

            return dataAtualDoSistema;
        }
    }
}
