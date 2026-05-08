using ADOSMELHORES.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ADOSMELHORES.Controllers
{
    public class HomeController : Controller
    {
        public static DateTime DataSimulada = DateTime.Now;

        public IActionResult Index()
        {
            return View(DataSimulada);
        }

        [HttpPost]
        public IActionResult AlterarData(DateTime novaData)
        {
            DataSimulada = novaData;
            return Json(DataSimulada);
        }

        [HttpPost]
        public IActionResult ResetarData()
        {
            DataSimulada = DateTime.Now;
            return Json(DataSimulada);
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
