
using EventPlanner.Models;
using EventPlanner.Validadores;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventPlanner.Data;


namespace EventPlanner.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly UsuarioBD _context;
        private readonly UsuarioPremiumBD _contextPremium;
        private readonly EventosBD _contextEvento;

        public UsuarioController(UsuarioBD context, UsuarioPremiumBD contextPremium, EventosBD contextEvento)
        {
            _context = context;
            _contextPremium = contextPremium;
            _contextEvento = contextEvento;
        }

        [HttpPost]
        [HttpPost]


        public async Task<IActionResult> Index()
        {
            return _context.Usuarios != null ?
                        View(await _context.Usuarios.ToListAsync()) :
                        Problem("Entity set 'UsuarioBD.Usuarios'  is null.");
        }



        public async Task<IActionResult> TipoUsuario()
        {
            var UsuarioIdString = HttpContext.Session.GetString("UserId");
            if (UsuarioIdString == null)
            {
                return RedirectToAction("Login");
            }

            var UsuarioId = int.Parse(UsuarioIdString);

            var Usuario = await _context.Usuarios.FirstOrDefaultAsync(Usuario => Usuario.ID == UsuarioId);
            var UsuarioPremium = await _contextPremium.UsuariosPremium.FirstOrDefaultAsync(UsuarioPremium => UsuarioPremium.ID == UsuarioId);

            if (Usuario != null)
            {
                return View("~/Views/Registro/RegistroEventoInfantilNormal.cshtml");
            }
            else if (UsuarioPremium != null)
            {
                return View("~/Views/Registro/RegistroEventoInfantil.cshtml");
            }

            return View();
        }



        [HttpPost]
        public async Task<IActionResult> TipoLogin(Usuario usuario)
        {
            if (usuario == null || string.IsNullOrWhiteSpace(usuario.Email) || string.IsNullOrWhiteSpace(usuario.Senha))
            {
                ModelState.AddModelError("Senha", "Campos não podem estar vazios");
                return View("~/Views/Home/Index.cshtml");
            }

            var usuarioRegistrado = await _context.Usuarios.FirstOrDefaultAsync(usuarioexistente => usuarioexistente.Email == usuario.Email);
            var usuarioPremiumRegistrado = await _contextPremium.UsuariosPremium.FirstOrDefaultAsync(usuarioexistente => usuarioexistente.Email == usuario.Email);

            if (usuarioRegistrado != null && BCrypt.Net.BCrypt.Verify(usuario.Senha, usuarioRegistrado.Senha) && usuarioRegistrado.Tipo == 1)
            {
                return RedirectToAction("Login", "Usuario", usuario);
            }
            else if (usuarioPremiumRegistrado != null && BCrypt.Net.BCrypt.Verify(usuario.Senha, usuarioPremiumRegistrado.Senha) && usuarioPremiumRegistrado.Tipo == 2)
            {
                UsuarioPremium usuarioPremium = new UsuarioPremium();

                usuarioPremium.Email = usuario.Email;
                usuarioPremium.Senha = usuario.Senha;

                return RedirectToAction("Login", "UsuarioPremium", usuarioPremium);
            }

            ModelState.AddModelError("Senha", "As informações estão incorretas");

            return View("~/Views/Home/Index.cshtml");
        }

        public async Task<IActionResult> Login([Bind("Email,Senha")] Usuario Usuario)
        {
            //Saber se precisa procura o objeto no contexto ou usar o que recebeu por parametro
            var UsuarioRegistrado = await _context.Usuarios.FirstOrDefaultAsync(UsuarioExistente => UsuarioExistente.Email == Usuario.Email);

            if (UsuarioRegistrado != null)
            {
                bool SenhaCorreta = BCrypt.Net.BCrypt.Verify(Usuario.Senha, UsuarioRegistrado.Senha);

                if (SenhaCorreta)
                {
                    HttpContext.Session.SetString("UserId", UsuarioRegistrado.ID.ToString());

                    var Evento = await _contextEvento.Eventos.FirstOrDefaultAsync(e => e.ID == UsuarioRegistrado.eventoID);

                    if (Evento != null)
                    {
                        HttpContext.Session.SetString("EventoId", Evento.ID.ToString());
                    }

                    return RedirectToAction("UserProfile", "Usuario");
                }
            }

            ModelState.AddModelError("Senha", "Email ou senha incorretos.");
            return View("~/Views/Home/Index.cshtml");
        }


        public async Task<IActionResult> UserProfile()
        {
            var UsuarioIdString = HttpContext.Session.GetString("UserId");
            if (UsuarioIdString == null)
            {
                return RedirectToAction("Login");
            }

            var UsuarioID = int.Parse(UsuarioIdString);

            var Usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.ID == UsuarioID);
            if (Usuario == null)
            {
                return RedirectToAction("Login");
            }

            var Evento = await _contextEvento.Eventos.FirstOrDefaultAsync(e => e.ID == Usuario.eventoID);
            return View("~/Views/Home/MenuEvento.cshtml", Evento);
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Usuarios == null)
            {
                return NotFound();
            }
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.ID == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // GET: Usuarios1/Create
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Nome,Email,CPF,Telefone,Idade,Senha")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {

                bool CpfValido = ValidadorCpf.ValidaCPF(usuario.CPF);
                if (!CpfValido)
                {
                    ModelState.AddModelError("CPF", "CPF invalido");
                    return View("~/Views/Registro/RegistroUsuario.cshtml");
                }

                var CpfEmUsoPadrao = await _context.Usuarios.FirstOrDefaultAsync(u => u.CPF == usuario.CPF);
                if (CpfEmUsoPadrao != null)
                {
                    ModelState.AddModelError("CPF", "Este CPF já está registrado.");
                    return View("~/Views/Registro/RegistroUsuario.cshtml");
                }

                var EmailEmUsoPadrao = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == usuario.Email);
                if (EmailEmUsoPadrao != null)
                {
                    ModelState.AddModelError("Email", "Este email já está registrado.");
                    return View("~/Views/Registro/RegistroUsuario.cshtml");
                }

                var EmailEmUsoPremium = await _contextPremium.UsuariosPremium.FirstOrDefaultAsync(u => u.Email == usuario.Email);
                if (EmailEmUsoPremium != null)
                {
                    ModelState.AddModelError("Email", "Este email já está registrado.");
                    return View("~/Views/Registro/RegistroUsuario.cshtml");
                }

                var CpfEmUsoPremium = await _contextPremium.UsuariosPremium.FirstOrDefaultAsync(u => u.CPF == usuario.CPF);
                if (CpfEmUsoPremium != null)
                {
                    ModelState.AddModelError("CPF", "Este CPF já está registrado.");
                    return View("~/Views/Registro/RegistroUsuario.cshtml");
                }

                bool TelefoneValido = ValidarTelefone.ValidarTeledone(usuario.Telefone);
                if (TelefoneValido == false)
                {
                    ModelState.AddModelError("Telefone", "Telefone invalido.");
                    return View("~/Views/Registro/RegistroUsuario.cshtml");
                }

                bool IdadeValida = ValidaIdade.ValidarIdade(usuario.Idade);
                if (IdadeValida == false)
                {
                    ModelState.AddModelError("Idade", "Idade invalida.");
                    return View("~/Views/Registro/RegistroUsuario.cshtml");
                }

                string SenhaValida = ValidaSenha.ValidarSenha(usuario.Senha);
                if (SenhaValida != null)
                {
                    ModelState.AddModelError("Senha", SenhaValida);
                    return View("~/Views/Registro/RegistroUsuario.cshtml");
                }

                string senha = usuario.Senha;
                usuario.Senha = Criptografia.GerarHash(senha);

                //var maxIdNormal = await _context.Usuarios.MaxAsync(u => (int?)u.ID);

                //var maxIdPremium = await _contextPremium.UsuariosPremium.MaxAsync(u => (int?)u.ID);

                //int novoId = Math.Max(maxIdNormal ?? 0, maxIdPremium ?? 0) + 1;

                //usuario.ID = novoId;

                usuario.Tipo = 1;

                _context.Add(usuario);
                await _context.SaveChangesAsync();

                var emailService = new Email("smtp.gmail.com", "contatoeventplanner@gmail.com", "ipmbosrsubhctwkk");
                var emaillist = new List<string> { usuario.Email };
                string subject = "Registro Concluído";
                string body = usuario.Nome + " Seu registro como usuario padrão foi  concluído com sucesso!";

                emailService.SendEmail(emaillist, subject, body);

                int IDusuario = usuario.ID;
                string cpf = usuario.CPF;
                string email = usuario.Email;
                string nome = usuario.Nome;
                string telefone = usuario.Telefone;
                int idade = usuario.Idade;
                string senha2 = usuario.Senha;
                int tipo = usuario.Tipo;

                return View("~/Views/Home/Index.cshtml");
            }

            return View("~/Views/Registro/RegistroUsuario.cshtml");
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Usuarios == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Nome,Email,CPF,Senha")] Usuario usuario)
        {
            if (id != usuario.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        // GET: Usuarios1/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Usuarios == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.ID == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuarios1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Usuarios == null)
            {
                return Problem("Entity set 'UsuarioBD.Usuarios'  is null.");
            }
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(int id)
        {
            return (_context.Usuarios?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}