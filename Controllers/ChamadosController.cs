using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http; // Para IFormFile (upload)
using System;
using System.IO;                 // Para MemoryStream (processar imagem)
using System.Threading.Tasks;    // Para métodos Async
using WebSuportePim.Models;      // Para os models Chamado e ChamadoDAO

namespace WebSuportePim.Controllers
{
    public class ChamadosController : Controller
    {
        // Este é o método [HttpPost] que a URL /Chamados/Novo estava procurando
        [HttpPost]
        public async Task<IActionResult> Novo(string titulo, string descricao, IFormFile arquivo)
        {
            // 1. Verificar se o usuário está logado (pelo ID salvo no Passo 1)
            int? idUsuario = HttpContext.Session.GetInt32("UsuarioId");

            if (idUsuario == null)
            {
                // Se não achou o ID, a sessão expirou. Manda para o Login.
                ViewBag.Mensagem = "Sua sessão expirou. Por favor, faça o login novamente.";
                return View("~/Views/Home/Login.cshtml");
            }

            // 2. Criar o objeto Chamado (exatamente como você pediu)
            var chamado = new Chamado
            {
                Titulo = titulo,
                Descricao = descricao,
                Status = "Aberto",
                Data_Abertura = DateTime.Now,
                Id_Usuario = idUsuario.Value // Vincula o chamado ao usuário logado
            };

            // 3. Processar a imagem (se ela foi enviada)
            if (arquivo != null && arquivo.Length > 0)
            {
                // Converte o arquivo (IFormFile) para um array de bytes (byte[])
                using (var memoryStream = new MemoryStream())
                {
                    await arquivo.CopyToAsync(memoryStream);
                    chamado.Imagem = memoryStream.ToArray(); // Salva os bytes no Model
                }
            }

            // 4. Salvar no Banco de Dados
            ChamadoDAO chamadoDAO = new ChamadoDAO();
            bool sucesso = chamadoDAO.Inserir(chamado);

            // 5. Redirecionar para a página de "Chamados Abertos"
            if (sucesso)
            {
                return RedirectToAction("ChamadosAbertos", "Home");
            }
            else
            {
                // Se deu erro ao salvar, volta para o formulário
                ViewBag.Erro = "Houve um problema ao salvar seu chamado. Tente novamente.";
                return View("~/Views/Home/AbrirNovoChamado.cshtml");
            }
        }
    }
}