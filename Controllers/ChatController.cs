using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebSuportePim.Services; 

namespace WebSuportePim.Controllers
{
    public class ChatController : Controller
    {
        private readonly GeminiService _geminiService;

        public ChatController(GeminiService geminiService)
        {
            _geminiService = geminiService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            // ADICIONAR VERIFICAÇÃO DE SESSÃO
            if (HttpContext.Session.GetInt32("UsuarioId") == null)
            {
                ViewBag.Mensagem = "Sua sessão expirou. Faça o login.";
                return View("~/Views/Home/Login.cshtml");
            }
            return View(); // Carrega a view /Views/Chat/Index.cshtml
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(string message)
        {
            // ADICIONAR VERIFICAÇÃO DE SESSÃO
            if (HttpContext.Session.GetInt32("UsuarioId") == null)
            {
                return Json(new { response = "Erro: Sessão expirada. Faça o login novamente." });
            }

            if (string.IsNullOrWhiteSpace(message))
                return Json(new { response = "Digite uma mensagem primeiro." });

            var response = await _geminiService.SendMessageAsync(message);
            return Json(new { response });
        }
    }
}