using ADOSMELHORES.Data;
using ADOSMELHORES.Data.Empresa;
using ADOSMELHORES.Models;
using ADOSMELHORES.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using static ADOSMELHORES.Data.Empresa.Formador;

namespace ADOSMELHORES.Controllers
{
    public class FuncionariosController : BaseController
    {
        public FuncionariosController(EmpresaContext context, IMemoryCache cache) : base(context, cache) { }

        public async Task<IActionResult> Index()
        {
            var funcionarios = await ObterFuncionariosDaCache();
            return View(funcionarios);
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof (Index));
            }
            var funcionarios = await ObterFuncionariosDaCache();

            var funcionario = funcionarios.FirstOrDefault(a => a.Id == id);

            if (funcionario == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(funcionario);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new FuncionarioViewModel();

            DateTime dataAtualSistema = ObterDataDoSistema();

            var funcionarios = await ObterFuncionariosDaCache();

            model.ListaDiretores = funcionarios.OfType<Diretor>()
                .Where(d => d.DataFimContrato > dataAtualSistema)
                .Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.Nome })
                .ToList();

            model.ListaCoordenadores = funcionarios.OfType<Coordenador>()
                .Where(c => c.DataFimContrato > dataAtualSistema)
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
                            DiretorId = model.DiretorId == null ? null : model.DiretorId
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

            DateTime dataAtualSistema = ObterDataDoSistema();

            model.ListaDiretores = funcionarios.OfType<Diretor>()
               .Where(d => d.DataFimContrato > dataAtualSistema)
               .Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.Nome })
               .ToList();

            model.ListaCoordenadores = funcionarios.OfType<Coordenador>()
                .Where(c => c.DataFimContrato > dataAtualSistema)
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Nome })
                .ToList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRegistoCriminal(Guid id, DateTime novaDataRegisto)
        {
            DateTime dataAtualSistema = ObterDataDoSistema();

            if (novaDataRegisto <= dataAtualSistema)
            {
                return Json(new { sucesso = false, mensagem = "A nova data de registo criminal não pode ser inferior à data atual do sistema." });
            }

            var funcionarios = await ObterFuncionariosDaCache();

            var funcionario = funcionarios.FirstOrDefault(a => a.Id == id);

            if (funcionario == null) return RedirectToAction(nameof(Index));


            funcionario.DataRegistoCriminal = novaDataRegisto;

            _context.Update(funcionario);
            await _context.SaveChangesAsync();

            _cache.Remove(CacheKeys.ListaFuncionarios);

            return Json(new { sucesso = true});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateContrato(Guid id, DateTime novaDataContrato)
        {
            DateTime dataAtualSistema = ObterDataDoSistema();

            if (novaDataContrato <= dataAtualSistema)
            {
                return Json(new { sucesso = false, mensagem = "A nova data de fim de contrato não pode ser inferior à data atual do sistema." });
            }

            var funcionarios = await ObterFuncionariosDaCache();

            var funcionario = funcionarios.FirstOrDefault(a => a.Id == id);

            if (funcionario == null) return RedirectToAction(nameof(Index));

            funcionario.DataFimContrato = novaDataContrato;

            _context.Update(funcionario);
            await _context.SaveChangesAsync();

            _cache.Remove(CacheKeys.ListaFuncionarios);

            return Json(new { sucesso = true });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return RedirectToAction(nameof(Index));

            var funcionarios = await ObterFuncionariosDaCache();

            var funcionario = funcionarios.FirstOrDefault(a => a.Id == id);

            DateTime dataAtualSistema = ObterDataDoSistema();

            if (funcionario == null) return RedirectToAction(nameof(Index));

            var model = new FuncionarioViewModel
            {
                Id = funcionario.Id,
                Nome = funcionario.Nome,
                Morada = funcionario.Morada,
                Contacto = funcionario.Contacto,
                DataFimContrato = funcionario.DataFimContrato,
                DataRegistoCriminal = funcionario.DataRegistoCriminal,
                ListaDiretores = funcionarios.OfType<Diretor>().Where(d => d.DataFimContrato > dataAtualSistema).Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.Nome }).ToList(),
                ListaCoordenadores = funcionarios.OfType<Coordenador>().Where(d => d.DataFimContrato > dataAtualSistema).Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Nome }).ToList()
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
            if (id != model.Id) return RedirectToAction(nameof(Index));

            if (ModelState.IsValid)
            {
                var funcionarios = await ObterFuncionariosDaCache();

                var funcionarioExistente = funcionarios.FirstOrDefault(a => a.Id == id);

                if (funcionarioExistente == null) return RedirectToAction(nameof(Index));

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
                    secretaria.DiretorId = model.DiretorId == null ? null : model.DiretorId;
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
                        return RedirectToAction(nameof(Index));
                    else
                        throw;
                }

                return RedirectToAction(nameof(Details), new { id = funcionarioExistente.Id });
            }
            var listaFuncionarios = await ObterFuncionariosDaCache();

            DateTime dataAtualSistema = ObterDataDoSistema();

            model.ListaDiretores = listaFuncionarios.OfType<Diretor>().Where(d => d.DataFimContrato > dataAtualSistema).Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.Nome }).ToList();
            model.ListaCoordenadores = listaFuncionarios.OfType<Coordenador>().Where(d => d.DataFimContrato > dataAtualSistema).Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Nome }).ToList();

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
            DateTime dataAtualSistema = ObterDataDoSistema();

            if (descricao.IsNullOrEmpty())
            {
                return Json(new { sucesso = false, mensagem = "A descrição é obrigatória." });
            }


            if (dataFim <= dataInicio)
            {
                return Json(new { sucesso = false, mensagem = "A Data de Fim não pode ser anterior à Data de Início." });
            }

            if (dataInicio < dataAtualSistema)
            {
                return Json(new { sucesso = false, mensagem = "A Data de Início não pode ser inferior à data atual do sistema." });
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

            return Json(new { sucesso = true });
        }

        [HttpPost]
        public async Task<IActionResult> CalcularVencimento(Guid? id, DateTime dataInicio, DateTime dataFim)
        {
            if (dataFim < dataInicio)
            {
                return Json(new { sucesso = false, mensagem = "A Data de Fim não pode ser anterior à Data de Início." });
            }

            if (id == null) return RedirectToAction(nameof(Index));

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
                if (formadorId == null || coordenadorId == null) return RedirectToAction(nameof(Index));

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
    }
}
