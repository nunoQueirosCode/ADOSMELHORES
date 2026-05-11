using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

public abstract class BaseController : Controller
{
    protected DateTime DataSimulacao { get; private set; }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        DataSimulacao = DateTime.Now;

        if (Request.Cookies.TryGetValue("DataSimulacaoCookie", out string cookieValue))
        {
            if (DateTime.TryParse(cookieValue, out DateTime dataConvertida))
            {
                DataSimulacao = dataConvertida;
            }
        }

        // Opcional: Passar a data para a ViewBag para usares diretamente nas tuas Views HTML
        ViewBag.DataSimulacao = DataSimulacao;

        base.OnActionExecuting(context);
    }
}