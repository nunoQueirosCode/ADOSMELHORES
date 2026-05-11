using ADOSMELHORES.Data;
using ADOSMELHORES.Data.Empresa;
using ADOSMELHORES.Models;
using ADOSMELHORES.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text;

namespace ADOSMELHORES.Controllers
{
    public class HomeController : Controller
    {
        private readonly EmpresaContext _context;

        public static DateTime DataSimulada = DateTime.Now;

        public HomeController(EmpresaContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View(DataSimulada);
        }

        [HttpPost]
        public IActionResult AlterarData(DateTime novaData)
        {
            DataSimulada = novaData;
            return Json(DataSimulada);
        }

        [HttpPost]
        public IActionResult ResetarData()
        {
            DataSimulada = DateTime.Now;
            return Json(DataSimulada);
        }

        public async Task<IActionResult> CalcularDespesaMensal()
        {
            var funcionarios = await _context.Funcionarios.Include("Alocacoes").ToListAsync();
            var hoje = DateTime.Now;//dataSistema

            var model = new HomeDashboardViewModel();

            model.TotalDiretores = funcionarios.OfType<Diretor>().Sum(d => d.Salario + d.BonusMensal);
            model.TotalSecretarias = funcionarios.OfType<Secretaria>().Sum(s => s.Salario);
            model.TotalCoordenadores = funcionarios.OfType<Coordenador>().Sum(c => c.Salario);

            model.TotalFormadores = funcionarios.OfType<Formador>().Sum(f => f.Alocacoes?
                        .Where(a => a.DataInicio.Month == hoje.Month && a.DataInicio.Year == hoje.Year)
                        .Sum(a => ContarDiasUteis(a.DataInicio, a.DataFim) * 6 * f.ValorHora) ?? 0
                );

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
                    bonusMensal = d.BonusMensal.ToString("F2");
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
    }
}
