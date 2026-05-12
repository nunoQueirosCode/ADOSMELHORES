using ADOSMELHORES.Data;
using ADOSMELHORES.Data.Empresa;
using ADOSMELHORES.Models;
using ADOSMELHORES.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Text;
using static ADOSMELHORES.Data.Empresa.Formador;

namespace ADOSMELHORES.Controllers
{
    public class FuncionariosController : Controller
    {
        private readonly EmpresaContext _context;
        private readonly IMemoryCache _cache;

        public FuncionariosController(EmpresaContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<IActionResult> Index()
        {
            var funcionarios = await ObterFuncionariosDaCache();
            return View(funcionarios);
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var funcionarios = await ObterFuncionariosDaCache();

            var funcionario = funcionarios.FirstOrDefault(a => a.Id == id);

            if (funcionario == null)
            {
                return NotFound();
            }

            return View(funcionario);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new FuncionarioViewModel();

            var funcionarios = await ObterFuncionariosDaCache();

            model.ListaDiretores = funcionarios.OfType<Diretor>()
                .Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.Nome })
                .ToList();

            model.ListaCoordenadores = funcionarios.OfType<Coordenador>()
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Nome })
                .ToList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FuncionarioViewModel model)
        {
            if (ModelState.IsValid)
            {
                Funcionario novoFuncionario = null;

                switch (model.TipoFuncionario)
                {
                    case "Diretor":
                        novoFuncionario = new Diretor
                        {
                            IsencaoHorario = model.IsencaoHorario,
                            Salario = model.Salario,
                            BonusMensal = model.BonusMensal,
                            CarroEmpresa = model.CarroEmpresa
                        };
                        break;
                    case "Secretaria":
                        novoFuncionario = new Secretaria
                        {
                            Salario = model.Salario,
                            Area = model.Area,
                            DiretorId = (Guid)model.DiretorId
                        };
                        break;
                    case "Formador":
                        novoFuncionario = new Formador
                        {
                            AreaLecionada = model.AreaLecionada,
                            TipoDisponibilidade = (Disponibilidade)model.TipoDisponibilidade,
                            ValorHora = model.ValorHora,
                            CoordenadorId = model.CoordenadorId
                        };
                        break;
                    case "Coordenador":
                        novoFuncionario = new Coordenador
                        {
                            Salario = model.Salario
                        };
                        break;
                }

                if (novoFuncionario != null)
                {
                    novoFuncionario.Id = Guid.NewGuid();
                    novoFuncionario.Nome = model.Nome;
                    novoFuncionario.Morada = model.Morada;
                    novoFuncionario.Contacto = model.Contacto;
                    novoFuncionario.DataFimContrato = model.DataFimContrato;
                    novoFuncionario.DataRegistoCriminal = model.DataRegistoCriminal;

                    _context.Add(novoFuncionario);
                    await _context.SaveChangesAsync();

                    _cache.Remove(CacheKeys.ListaFuncionarios);

                    return RedirectToAction(nameof(Details), new { id = novoFuncionario.Id });
                }
              
            }

            var funcionarios = await ObterFuncionariosDaCache();

            model.ListaDiretores = funcionarios.OfType<Diretor>()
                .Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.Nome })
                .ToList();

            model.ListaCoordenadores = funcionarios.OfType<Coordenador>()
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Nome })
                .ToList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRegistoCriminal(Guid id, DateTime novaDataRegisto)
        {
            var funcionarios = await ObterFuncionariosDaCache();

            var funcionario = funcionarios.FirstOrDefault(a => a.Id == id);

            if (funcionario == null) return NotFound();

            funcionario.DataRegistoCriminal = novaDataRegisto;

            _context.Update(funcionario);
            await _context.SaveChangesAsync();

            _cache.Remove(CacheKeys.ListaFuncionarios);

            return RedirectToAction(nameof(Details), new {id = funcionario.Id}); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateContrato(Guid id, DateTime novaDataContrato)
        {
            var funcionarios = await ObterFuncionariosDaCache();

            var funcionario = funcionarios.FirstOrDefault(a => a.Id == id);

            if (funcionario == null) return NotFound();

            funcionario.DataFimContrato = novaDataContrato;

            _context.Update(funcionario);
            await _context.SaveChangesAsync();

            _cache.Remove(CacheKeys.ListaFuncionarios);

            return RedirectToAction(nameof(Details), new { id = funcionario.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();

            var funcionarios = await ObterFuncionariosDaCache();

            var funcionario = funcionarios.FirstOrDefault(a => a.Id == id);

            if (funcionario == null) return NotFound();

            var model = new FuncionarioViewModel
            {
                Id = funcionario.Id,
                Nome = funcionario.Nome,
                Morada = funcionario.Morada,
                Contacto = funcionario.Contacto,
                DataFimContrato = funcionario.DataFimContrato,
                DataRegistoCriminal = funcionario.DataRegistoCriminal,
                ListaDiretores = _context.Diretores.Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.Nome }).ToList(),
                ListaCoordenadores = _context.Coordenadores.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Nome }).ToList()
            };

            if (funcionario is Diretor diretor)
            {
                model.TipoFuncionario = "Diretor";
                model.IsencaoHorario = diretor.IsencaoHorario;
                model.BonusMensal = (decimal)diretor.BonusMensal;
                model.CarroEmpresa = diretor.CarroEmpresa;
                model.Salario = diretor.Salario;
            }
            else if (funcionario is Secretaria secretaria)
            {
                model.TipoFuncionario = "Secretaria";
                model.Area = secretaria.Area;
                model.DiretorId = secretaria.DiretorId;
                model.Salario = secretaria.Salario;
            }
            else if (funcionario is Formador formador)
            {
                model.TipoFuncionario = "Formador";
                model.AreaLecionada = formador.AreaLecionada;
                model.TipoDisponibilidade = (Disponibilidade)formador.TipoDisponibilidade;
                model.ValorHora = formador.ValorHora;
                model.CoordenadorId = formador.CoordenadorId;
            }
            else if (funcionario is Coordenador coordenador)
            {
                model.TipoFuncionario = "Coordenador";
                model.Salario = coordenador.Salario;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, FuncionarioViewModel model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var funcionarios = await ObterFuncionariosDaCache();

                var funcionarioExistente = funcionarios.FirstOrDefault(a => a.Id == id);

                if (funcionarioExistente == null) return NotFound();

                funcionarioExistente.Nome = model.Nome;
                funcionarioExistente.Morada = model.Morada;
                funcionarioExistente.Contacto = model.Contacto;
                funcionarioExistente.DataFimContrato = model.DataFimContrato;
                funcionarioExistente.DataRegistoCriminal = model.DataRegistoCriminal;

                if (funcionarioExistente is Diretor diretor)
                {
                    diretor.IsencaoHorario = model.IsencaoHorario;
                    diretor.BonusMensal = model.BonusMensal;
                    diretor.CarroEmpresa = model.CarroEmpresa;
                    diretor.Salario = model.Salario;
                }
                else if (funcionarioExistente is Secretaria secretaria)
                {
                    secretaria.Area = model.Area;
                    secretaria.DiretorId = (Guid)model.DiretorId;
                    secretaria.Salario = model.Salario;
                }
                else if (funcionarioExistente is Formador formador)
                {
                    formador.AreaLecionada = model.AreaLecionada;
                    formador.TipoDisponibilidade = (Disponibilidade)model.TipoDisponibilidade;
                    formador.ValorHora = model.ValorHora;
                    formador.CoordenadorId = model.CoordenadorId;
                }
                else if (funcionarioExistente is Coordenador coordenador)
                {
                    coordenador.Salario = model.Salario;
                }

                try
                {
                    _context.Update(funcionarioExistente);
                    await _context.SaveChangesAsync();

                    _cache.Remove(CacheKeys.ListaFuncionarios);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!funcionarios.Any(e => e.Id == id))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Details), new { id = funcionarioExistente.Id });
            }
            var listaFuncionarios = await ObterFuncionariosDaCache();

            model.ListaDiretores = listaFuncionarios.OfType<Diretor>().Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.Nome }).ToList();
            model.ListaCoordenadores = listaFuncionarios.OfType<Coordenador>().Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Nome }).ToList();

            return View(model);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var funcionarios = await ObterFuncionariosDaCache();

            var funcionario = funcionarios.FirstOrDefault(a => a.Id == id);

            if (funcionario != null)
            {
                if (funcionario is Diretor)
                {
                    var secretariasAfetadas = funcionarios.OfType<Secretaria>().Where(s => s.DiretorId == id);
                    foreach (var sec in secretariasAfetadas)
                    {
                        sec.DiretorId = null;
                    }
                }
                if (funcionario is Coordenador)
                {
                    var formadoresAfetados = funcionarios.OfType<Formador>().Where(f => f.CoordenadorId == id);
                    foreach (var form in formadoresAfetados)
                    {
                        form.CoordenadorId = null;
                    }
                }
                _context.Funcionarios.Remove(funcionario);
                await _context.SaveChangesAsync();

                _cache.Remove(CacheKeys.ListaFuncionarios);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> AlocacaoFuncionario(Guid idFormador, DateTime dataInicio, DateTime dataFim, string descricao)
        {
            if (dataFim < dataInicio)
            {
                return Json(new { sucesso = false, mensagem = "A Data de Fim não pode ser anterior à Data de Início." });
            }

            var novaAlocacao = new Alocacao
            {
                Id = Guid.NewGuid(),
                FormadorId = idFormador,
                DataInicio = dataInicio,
                DataFim = dataFim,
                DescricaoFormacao = descricao
            };

            _context.Add(novaAlocacao);
            await _context.SaveChangesAsync();

            _cache.Remove(CacheKeys.ListaFuncionarios);

            return RedirectToAction(nameof(Details), new { id = idFormador });
        }

        [HttpPost]
        public async Task<IActionResult> CalcularVencimento(Guid? id, DateTime dataInicio, DateTime dataFim)
        {
            if (dataFim < dataInicio)
            {
                return Json(new { sucesso = false, mensagem = "A Data de Fim não pode ser anterior à Data de Início." });
            }

            if (id == null) return NotFound();

            var funcionarios = await ObterFuncionariosDaCache();

            var formador = funcionarios.OfType<Formador>().FirstOrDefault(a => a.Id == id);

            if (formador == null) return Json(new { sucesso = false, mensagem = "Formador não encontrado." });

            decimal valorHora = formador.ValorHora;

            int totalDias = (dataFim - dataInicio).Days + 1;
            int horasPorDia = 6;

            decimal valorTotal = totalDias * horasPorDia * valorHora;

            return Json(new
            {
                sucesso = true,
                valorTotalFormatado = valorTotal.ToString("C"),
                detalhesCalculo = $"{totalDias} dias x {horasPorDia}h x {valorHora.ToString("C")}/h"
            });
        }

        [HttpPost]
        public async Task<IActionResult> RemoverAssociacao(Guid? formadorId, Guid? coordenadorId)
        {
            try
            {
                if (formadorId == null || coordenadorId == null) return NotFound();

                var funcionarios = await ObterFuncionariosDaCache();

                var formador = funcionarios.OfType<Formador>().FirstOrDefault(a => a.Id == formadorId);

                if (formador == null) return Json(new { sucesso = false, mensagem = "Formador não encontrado." });

                formador.CoordenadorId = null;

                _context.Update(formador);
                await _context.SaveChangesAsync();

                _cache.Remove(CacheKeys.ListaFuncionarios);

                return Json(new { sucesso = true });
            }
            catch (Exception ex)
            {
                return Json(new { sucesso = false, mensagem = "Erro interno do servidor: " + ex.Message });
            }
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
    }
}
