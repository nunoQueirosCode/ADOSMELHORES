using ADOSMELHORES.Data;
using ADOSMELHORES.Data.Empresa;
using ADOSMELHORES.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ADOSMELHORES.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        
        private readonly EmpresaContext _context;

        public static DateTime DataSimulada = DateTime.Now;

        public HomeController(ILogger<HomeController> logger, EmpresaContext context)
        {
            _logger = logger;
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
            var hoje = DateTime.Now;

            ViewBag.TotalDiretores = funcionarios.OfType<Diretor>().Sum(d => d.Salario + d.BonusMensal);
            ViewBag.TotalSecretarias = funcionarios.OfType<Secretaria>().Sum(s => s.Salario);
            ViewBag.TotalCoordenadores = funcionarios.OfType<Coordenador>().Sum(c => c.Salario);

            // Formadores (alocações do mês atual dentro da soma)
            ViewBag.TotalFormadores = funcionarios.OfType<Formador>().Sum(f => f.Alocacoes?
                        .Where(a => a.DataInicio.Month == hoje.Month && a.DataInicio.Year == hoje.Year)
                        .Sum(a => ContarDiasUteis(a.DataInicio, a.DataFim) * 6 * f.ValorHora) ?? 0
                );

            // Totais
            ViewBag.TotalGeral = (decimal)ViewBag.TotalDiretores + (decimal)ViewBag.TotalSecretarias +
                                 (decimal)ViewBag.TotalCoordenadores + (decimal)ViewBag.TotalFormadores;
            ViewBag.QtdFuncionarios = funcionarios.Count;

            return View();
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
    }
}
