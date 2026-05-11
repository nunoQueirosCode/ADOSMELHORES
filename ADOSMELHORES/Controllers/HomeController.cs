using ADOSMELHORES.Models;
using ADOSMELHORES.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ADOSMELHORES.Controllers
{
    public class HomeController : BaseController
    {
        [HttpGet]
        public IActionResult Index()
        {
            var model = new HomeSimuladorViewModel
            {
                DataSimulacao = this.DataSimulacao
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult GuardarDataSimulacao([FromBody] string dataString)
        {
            // Parse da string ISO para DateTime
            if (!DateTime.TryParse(dataString, out DateTime novaData) || novaData == default(DateTime))
            {
                return BadRequest(new { sucesso = false, mensagem = "Data inválida" });
            }

            var options = new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddDays(30),
                HttpOnly = true,
                IsEssential = true,
                SameSite = SameSiteMode.Lax
            };

            Response.Cookies.Append("DataSimulacaoCookie", 
                novaData.ToString("yyyy-MM-ddTHH:mm:ss"), options);

            return Json(new { sucesso = true, mensagem = "Data alterada com sucesso", data = novaData });
        }

        [HttpPost]
        public IActionResult ResetarData()
        {
            Response.Cookies.Delete("DataSimulacaoCookie");
            return Json(new { sucesso = true, mensagem = "Data resetada para hoje", data = DateTime.Now });
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
