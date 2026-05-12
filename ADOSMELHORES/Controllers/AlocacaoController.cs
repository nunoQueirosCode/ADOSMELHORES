using ADOSMELHORES.Data;
using ADOSMELHORES.Data.Empresa;
using ADOSMELHORES.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace ADOSMELHORES.Controllers
{
    public class AlocacaoController : Controller
    {
        private readonly EmpresaContext _context;
        private readonly IMemoryCache _cache;
        
        public AlocacaoController(EmpresaContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }
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
            if (alocacao != null)
            {
            try { 
                var funcionarios = await ObterFuncionariosDaCache();

                var alocacaoExisteNaCache = funcionarios
                    .OfType<Formador>()
                    .Where(f => f.Alocacoes != null)
                    .SelectMany(f => f.Alocacoes)
                    .FirstOrDefault(a => a.Id == id);

                if (alocacaoExisteNaCache == null)
                {
                    return Json(new { sucesso = false, mensagem = "Formação não encontrada." });
                }

                var alocacaoParaApagar = new Alocacao { Id = id };

                _context.Alocacoes.Remove(alocacaoParaApagar);
                await _context.SaveChangesAsync();

                _cache.Remove(CacheKeys.ListaFuncionarios);

                return Json(new { sucesso = true, mensagem = "Alocação eliminada com sucesso." });
            }
            catch (Exception ex)
            {
                return Json(new { sucesso = false, mensagem = "Erro ao eliminar alocação: " + ex.Message });
            }
        }

            return Json(new { sucesso = false, mensagem = "Não encontrada" });
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
