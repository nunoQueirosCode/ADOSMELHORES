using ADOSMELHORES.Data;
using ADOSMELHORES.Data.Empresa;
using ADOSMELHORES.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static ADOSMELHORES.Data.Empresa.Formador;

namespace ADOSMELHORES.Controllers
{
    public class FuncionariosController : Controller
    {
        private readonly EmpresaContext _context;

        public FuncionariosController(EmpresaContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var funcionarios = await _context.Funcionarios.ToListAsync();
            return View(funcionarios);
        }

        public async Task<IActionResult> Detalhes(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var funcionario = await _context.Funcionarios.FirstOrDefaultAsync(a => a.Id == id);

            if (funcionario == null)
            {
                return NotFound();
            }

            return View(funcionario);
        }

        [HttpGet]
        public IActionResult Registo()
        {
            var model = new FuncionarioViewModel();

            model.ListaDiretores = _context.Diretores
                .Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.Nome })
                .ToList();

            model.ListaCoordenadores = _context.Coordenadores
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Nome })
                .ToList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registo(FuncionarioViewModel model)
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
                            DiretorId = model.DiretorId
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
                    novoFuncionario.Nome = model.Nome;
                    novoFuncionario.Morada = model.Morada;
                    novoFuncionario.Contacto = model.Contacto;
                    novoFuncionario.DataFimContrato = model.DataFimContrato;
                    novoFuncionario.DataRegistoCriminal = model.DataRegistoCriminal;

                    _context.Add(novoFuncionario);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            model.ListaDiretores = _context.Diretores
                .Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.Nome })
                .ToList();

            model.ListaCoordenadores = _context.Coordenadores
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Nome })
                .ToList();

            return View(model);

        }


    }
}
