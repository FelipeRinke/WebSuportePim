using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebSuportePim.Models;

namespace WebSuportePim.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string senha)
        {
            UsuarioDAO usuarioDAO = new UsuarioDAO();
            Usuario usuario = usuarioDAO.ValidarLogin(email, senha);

            if (usuario != null)
            {
                // Armazena informações do usuário na sessão
                HttpContext.Session.SetInt32("UsuarioId", usuario.Id);
                HttpContext.Session.SetString("UsuarioNome", usuario.Nome);
                HttpContext.Session.SetString("UsuarioDepartamento", usuario.Departamento);
                HttpContext.Session.SetString("UsuarioEmail", usuario.Email);

                return RedirectToAction("Dashboard", "Home");
            }
            else
            {
                ViewBag.Mensagem = "Usuário ou senha inválidos.";
                return View();
            }
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult ChamadosAbertos()
        {
            // 1. Verificar se o usuário está logado
            int? idUsuario = HttpContext.Session.GetInt32("UsuarioId");

            if (idUsuario == null)
            {
                // Se não está logado, manda para a tela de Login
                ViewBag.Mensagem = "Sua sessão expirou. Por favor, faça o login novamente.";
                return View("Login"); // Redireciona para a View de Login
            }

            // 2. Se está logado, buscar os chamados dele
            ChamadoDAO chamadoDAO = new ChamadoDAO();
            List<Chamado> meusChamados = chamadoDAO.ListarPorUsuario(idUsuario.Value);

            // 3. Enviar a lista de chamados (o Model) para a View
            return View(meusChamados); // 'meusChamados' será o "Model" na sua view
        }

        public IActionResult AbrirNovoChamado()
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