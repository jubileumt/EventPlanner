using Microsoft.AspNetCore.Mvc;

namespace EventPlanner.Controllers
{
    public class BussolaController : Controller
    {

        public IActionResult MudarSenha()
        {
            return View("~/Views/Registro/RecuperarSenha.cshtml");
        }

        public IActionResult RegistroEventoInfantilPremium()
        {
            return View("~/Views/Registro/RegistroEventoInfantil.cshtml");
        }

        public IActionResult RegistroEventoInfantilNormal()
        {
            return View("~/Views/Registro/RegistroEventoInfantilNormal.cshtml");
        }

        public IActionResult RegistroUsuarioPremium()
        {
            return View("~/Views/Registro/RegistroUsuarioPremium.cshtml");
        }
        public IActionResult RegistroUsuario()
        {
            return View("~/Views/Registro/RegistroUsuario.cshtml");
        }

        public IActionResult DeletarEvento()
        {
            return RedirectToAction("Delete", "Evento");
        }
        public IActionResult PlanosDeUsuarios()
        {
            return View("~/Views/Home/PlanosDeUsuarios.cshtml");
        } 
        public IActionResult FiltrosEventos()
        {
            return View("~/Views/Home/FiltroEvento.cshtml");
        }
        public IActionResult TiposDeEventos()
        {
            return View("~/Views/Home/TiposDeEventos.cshtml");
        }

    }
}