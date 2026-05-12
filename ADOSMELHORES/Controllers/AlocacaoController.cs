using ADOSMELHORES.Data;
using ADOSMELHORES.Data.Empresa;
using ADOSMELHORES.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ADOSMELHORES.Controllers
{
    public class AlocacaoController : Controller
    {
        private readonly EmpresaContext _context;
        
        public AlocacaoController(EmpresaContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var alocacoes = await _context.Alocacoes
                .Include(a => a.Formador)
                .Include(a => a.DataInicio)
                .ToListAsync();


            return View(alocacoes);
        }

        public async Task<IActionResult> Details (Guid? id)
        {
           if (id == null) return NotFound();
            var alocacao = await _context.Alocacoes
                .Include(a => a.Formador)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (alocacao == null) return NotFound();
            return View(alocacao);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var alocacao = await _context.Alocacoes.FindAsync(id);
            if (alocacao != null)
            {
                _context.Alocacoes.Remove(alocacao);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }


    }
}
