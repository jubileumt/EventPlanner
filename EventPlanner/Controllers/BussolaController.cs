using Microsoft.AspNetCore.Mvc;

namespace EventPlanner.Controllers
{
    public class BussolaController : Controller
    {

        public IActionResult VerificarCliente(string User, int Senha)
        {
            return View("~/Views/Registro/RegistroUsuario.cshtml");
        }
        public IActionResult RetornaIndex()
        {
            return View("~/Views/Home/Index.cshtml");
        }
        public IActionResult RegistroUsuario(string Nome, string Email, string Senha)
        {
            return View("~/Views/Registro/RegistroUsuario.cshtml");
        }

        public IActionResult MudarSenha()
        {
            return View("~/Views/Registro/RecuperarSenha.cshtml");
        }

        public IActionResult RegistroEventoInfantil1()
        {
            return View("~/Views/Registro/RegistroEvento.cshtml");
        }

        public IActionResult RegistroUsuarioPremium()
        {
            return View("~/Views/Registro/RegistroUsuarioPremium.cshtml");
        }

        public IActionResult DeletarEvento()
        {
            return RedirectToAction("Deletar", "Evento");
        }

    }
}