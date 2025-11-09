using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;
using WebSuportePim.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Png;

namespace WebSuportePim.Controllers
{
    public class ChamadosController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> Novo(string titulo, string descricao, IFormFile arquivo)
        {
            // 1. Verificar se o usuário está logado
            int? idUsuario = HttpContext.Session.GetInt32("UsuarioId");

            if (idUsuario == null)
            {
                ViewBag.Mensagem = "Sua sessão expirou. Por favor, faça o login novamente.";
                return View("~/Views/Home/Login.cshtml"); // <-- Retorno 1
            }

            // 2. Criar o objeto Chamado
            var chamado = new Chamado
            {
                Titulo = titulo,
                Descricao = descricao,
                Status = "Aberto",
                Data_Abertura = DateTime.Now,
                Id_Usuario = idUsuario.Value
            };

            // 3. Processar a imagem (com conversão para PNG)
            if (arquivo != null && arquivo.Length > 0)
            {
                try
                {
                    using (var inputStream = arquivo.OpenReadStream())
                    {
                        using (var image = await Image.LoadAsync(inputStream))
                        {
                            // Opcional: Redimensionar
                            image.Mutate(x => x.Resize(new ResizeOptions
                            {
                                Size = new Size(1024, 1024),
                                Mode = ResizeMode.Max
                            }));

                            using (var memoryStream = new MemoryStream())
                            {
                                // Salvar como PNG
                                await image.SaveAsPngAsync(memoryStream);
                                chamado.Imagem = memoryStream.ToArray();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Se o upload falhar (ex: não é imagem), salva o chamado sem imagem.
                    chamado.Imagem = null;
                }
            }

            // 4. Salvar no Banco de Dados
            ChamadoDAO chamadoDAO = new ChamadoDAO();
            bool sucesso = chamadoDAO.Inserir(chamado); //

            // 5. Redirecionar (AQUI ESTÁ A CORREÇÃO)
            // Você precisa OBRIGATORIAMENTE de um 'else' aqui.
            if (sucesso)
            {
                return RedirectToAction("ChamadosAbertos", "Home"); // <-- Retorno 2
            }
            else
            {
                ViewBag.Erro = "Houve um problema ao salvar seu chamado. Tente novamente.";
                return View("~/Views/Home/AbrirNovoChamado.cshtml"); // <-- Retorno 3
            }


        }
    }
}