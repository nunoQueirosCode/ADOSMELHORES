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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var alocacao = await _context.Alocacoes.FindAsync(id);

                if (alocacao == null)
                    return Json(new { sucesso = false, mensagem = "Alocação não encontrada." });

                _context.Alocacoes.Remove(alocacao);
                await _context.SaveChangesAsync();

                return Json(new { sucesso = true, mensagem = "Alocação eliminada com sucesso." });
            }
            catch (Exception ex)
            {
                return Json(new { sucesso = false, mensagem = "Erro ao eliminar alocação: " + ex.Message });
            }
        }
    }
}
