using ADOSMELHORES.Data;
using ADOSMELHORES.Data.Empresa;
using ADOSMELHORES.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace ADOSMELHORES.Controllers
{
    public class AlocacaoController : BaseController
    {
        public AlocacaoController(EmpresaContext context, IMemoryCache cache) : base(context, cache) { }
        public async Task<IActionResult> Index()
        {
            var funcionarios = await ObterFuncionariosDaCache();

            var alocacoes = funcionarios
                .OfType<Formador>()
                .Where(f => f.Alocacoes != null)
                .SelectMany(f => f.Alocacoes)
                .ToList();


            return View(alocacoes);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var alocacao = await _context.Alocacoes.FindAsync(id);

            if (alocacao == null)
            {
                return Json(new { sucesso = false, mensagem = "Formação não encontrada na base de dados." });
            }

            try
            {
                var funcionarios = await ObterFuncionariosDaCache();
                var existeNaCache = funcionarios
                    .OfType<Formador>()
                    .Any(f => f.Alocacoes != null && f.Alocacoes.Any(a => a.Id == id));

                _context.Alocacoes.Remove(alocacao);
                await _context.SaveChangesAsync();

                _cache.Remove(CacheKeys.ListaFuncionarios);

                return Json(new { sucesso = true, mensagem = "Alocação eliminada com sucesso." });
            }
            catch (Exception ex)
            {
                return Json(new { sucesso = false, mensagem = "Erro ao eliminar alocação: " + ex.Message });
            }
        }
    }
}
