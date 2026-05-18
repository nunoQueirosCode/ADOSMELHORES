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
    public class HomeController : BaseController
    {
        public HomeController(EmpresaContext context, IMemoryCache cache) : base(context, cache) { }

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

            model.QtdFuncionariosRegistoCriminal = funcionarios.Count(f => f.DataRegistoCriminal < dataAtualDoSistema && f.DataFimContrato >= dataAtualDoSistema);

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
                        NomeFuncionario = x.Funcionario.Nome, 
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
