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
                .ToListAsync();


            return View(alocacoes);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var alocacao = await _context.Alocacoes.FindAsync(id);
            if (alocacao != null)
            {
                _context.Alocacoes.Remove(alocacao);
                await _context.SaveChangesAsync();

                return Json(new { sucesso = true });
            }

            return Json(new { sucesso = false, mensagem = "Não encontrada" });
        }
    }
}
