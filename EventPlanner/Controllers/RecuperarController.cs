using EventPlanner.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventPlanner.Data;
using EventPlanner.Validadores;

namespace TesteMVC2.Controllers
{
    public class RecuperarController : Controller
    {
        private readonly UsuarioBD _context;
        private readonly UsuarioPremiumBD _contextPremium;

        public RecuperarController(UsuarioBD context, UsuarioPremiumBD contextPremium)
        {
            _context = context;
            _contextPremium = contextPremium;
        }

        public async Task<IActionResult> TipoLogin(Usuario usuario)
        {
            if (usuario == null || string.IsNullOrWhiteSpace(usuario.Email) || string.IsNullOrWhiteSpace(usuario.Senha))
            {
                ModelState.AddModelError("Senha", "Campos não podem estar vazios");
                return View("~/Views/Home/Index.cshtml");
            }

            var usuarioRegistrado = await _context.Usuarios.FirstOrDefaultAsync(usuarioexistente => usuarioexistente.Email == usuario.Email && usuarioexistente.Senha == usuario.Senha);
            var usuarioPremiumRegistrado = await _contextPremium.UsuariosPremium.FirstOrDefaultAsync(usuarioexistente => usuarioexistente.Email == usuario.Email && usuarioexistente.Senha == usuario.Senha);

            if (usuarioRegistrado != null && usuarioRegistrado.Tipo == 1)
            {
                return RedirectToAction("Login", usuario);

            }
            else if (usuarioPremiumRegistrado != null && usuarioPremiumRegistrado.Tipo == 2)
            {
                UsuarioPremium usuarioPremium = new UsuarioPremium();

                usuarioPremium.Email = usuario.Email;
                usuarioPremium.Senha = usuario.Senha;
                return RedirectToAction("Login", usuarioPremium);
            }
            return View("~/Views/Home/Index.cshtml");
        }


        public async Task<IActionResult> MudarUsuarioPadraoSenha([Bind("Email,CPF,Senha")] Usuario usuario)
        {
            int verifiacar = 0;

            var usuarioRegistrado = await _context.Usuarios.FirstOrDefaultAsync(usuarioexistente => usuarioexistente.Email == usuario.Email && usuarioexistente.CPF == usuario.CPF);

            if (usuarioRegistrado != null)
            {
                verifiacar++;
                usuarioRegistrado.Senha = usuario.Senha;
                await _context.SaveChangesAsync();
                return View("~/Views/Registro/RecuperarSenha.cshtml");
            }
            else if (verifiacar == 0)
            {
                ModelState.AddModelError("Senha", "Credenciais não registradas");
                verifiacar = 2;
            }
            else if (verifiacar == 2)
            {
                ModelState.AddModelError("Cpf", "Email ou CPF incorretos.");
            }

            return View("~/Views/Home/Index.cshtml");
        }

        public async Task<IActionResult> MudarUsarioPremiumSenha([Bind("Email,CPF,Senha")] UsuarioPremium UsuarioPremium)
        {
            int verifiacar = 0;

            var UsuarioPremiumRegistrado = await _context.Usuarios.FirstOrDefaultAsync(usuarioexistente => usuarioexistente.Email == UsuarioPremium.Email && usuarioexistente.CPF == UsuarioPremium.CPF);

            if (UsuarioPremiumRegistrado != null)
            {
                verifiacar++;
                UsuarioPremiumRegistrado.Senha = UsuarioPremium.Senha;
                await _contextPremium.SaveChangesAsync();
                return View("~/Views/Registro/RecuperarSenha.cshtml");
            }
            else if (verifiacar == 0)
            {
                ModelState.AddModelError("Senha", "Credenciais não registradas");
                verifiacar = 2;
            }
            else if (verifiacar == 2)
            {
                ModelState.AddModelError("Cpf", "Email ou CPF incorretos.");
            }

            return View("~/Views/Home/Index.cshtml");
        }



    }
}