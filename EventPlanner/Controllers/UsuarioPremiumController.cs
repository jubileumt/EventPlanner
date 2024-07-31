using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventPlanner.Models;
using EventPlanner.Validadores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventPlanner.Data;
using ViaCepConsumer;
using ViaCepConsumer.Models;

namespace EventPlanner.Controllers
{
    public class UsuarioPremiumController : Controller
    {
        private readonly UsuarioPremiumBD _context;
        private readonly UsuarioBD _contextNormal;
        private readonly ViaCepClient viaCepClient;
        private readonly EventosBD _contextEvento;

        public UsuarioPremiumController(UsuarioPremiumBD context, UsuarioBD contextNormal, ViaCepClient viaCepClient, EventosBD contextEvento)
        {
            _context = context;
            _contextNormal = contextNormal;
            this.viaCepClient = viaCepClient;
            _contextEvento = contextEvento;
        }

        public async Task<IActionResult> Login([Bind("Email,Senha")] UsuarioPremium UsuarioPremium)
        {
            var UsuarioRegistrado = await _context.UsuariosPremium.FirstOrDefaultAsync(UsuarioExistente => UsuarioExistente.Email == UsuarioPremium.Email);

            if (UsuarioRegistrado != null)
            {
                bool SenhaCorreta = BCrypt.Net.BCrypt.Verify(UsuarioPremium.Senha, UsuarioRegistrado.Senha);

                if (SenhaCorreta)
                {
                    HttpContext.Session.SetString("UsuarioPremiumID", UsuarioRegistrado.ID.ToString());

                    var Evento = await _contextEvento.Eventos.FirstOrDefaultAsync(e => e.ID == UsuarioRegistrado.eventoID);

                    if (Evento != null)
                    {
                        HttpContext.Session.SetString("EventoId", Evento.ID.ToString());
                    }

                    return RedirectToAction("LoginEfetivo");
                }
            }

            ModelState.AddModelError("Senha", "Email ou senha incorretos.");
            return View("~/Views/Home/Index.cshtml");
        }
        public async Task<IActionResult> LoginEfetivo()
        {
            var UsuarioIdString = HttpContext.Session.GetString("UsuarioPremiumID");
            if (UsuarioIdString == null)
            {
                return RedirectToAction("Login");
            }

            var UsuarioId = int.Parse(UsuarioIdString);

            var Usuario = await _context.UsuariosPremium.FirstOrDefaultAsync(u => u.ID == UsuarioId);
            if (Usuario == null)
            {
                return RedirectToAction("Login");
            }

            var Evento = await _contextEvento.Eventos.FirstOrDefaultAsync(e => e.ID == Usuario.eventoID);
            return RedirectToAction("EventosAleatorios","Evento");
        }

        public async Task<IActionResult> MeusEventosPremium()
        {
            var UsuarioIdString = HttpContext.Session.GetString("UsuarioPremiumID");
            if (UsuarioIdString == null)
            {
                return RedirectToAction("Login");
            }

            var UsuarioID = int.Parse(UsuarioIdString);

            var UsuarioPremium = await _context.UsuariosPremium.FirstOrDefaultAsync(u => u.ID == UsuarioID);
            if (UsuarioPremium == null)
            {
                return RedirectToAction("Login");
            }

            var Evento = await _contextEvento.Eventos.FirstOrDefaultAsync(e => e.ID == UsuarioPremium.eventoID);
            return View("~/Views/Home/MenuEvento.cshtml", Evento);
        }

        public async Task<IActionResult> Index()
        {
            return _context.UsuariosPremium != null ?
                        View(await _context.UsuariosPremium.ToListAsync()) :
                        Problem("Entity set 'UsuarioPremiumBD.UsuariosPremium'  is null.");
        }

        // GET: UsuarioP/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.UsuariosPremium == null)
            {
                return NotFound();
            }

            var usuarioPremium = await _context.UsuariosPremium
                .FirstOrDefaultAsync(m => m.ID == id);
            if (usuarioPremium == null)
            {
                return NotFound();
            }

            return View(usuarioPremium);
        }

        // GET: UsuarioP/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: UsuarioP/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create([Bind("UsuPremiumID,Nome,Email,CPF,Telefone,Idade,Senha,CEP,Bairro,Cidade,Estado,NumeroCartao,TitularCartao,DataValidade,CodigoSeguranca")] UsuarioPremium usuarioPremium)
        {
            if (ModelState.IsValid)
            {
                bool CpfValido = ValidadorCpf.ValidaCPF(usuarioPremium.CPF);
                if (!CpfValido)
                {
                    ModelState.AddModelError("CPF", "CPF inválido");
                    return View("~/Views/Registro/RegistroUsuarioPremium.cshtml");
                }

                var CpfEmUsoPremium = await _context.UsuariosPremium.FirstOrDefaultAsync(u => u.CPF == usuarioPremium.CPF);
                if (CpfEmUsoPremium != null)
                {
                    ModelState.AddModelError("CPF", "Este CPF já está registrado.");
                    return View("~/Views/Registro/RegistroUsuarioPremium.cshtml");
                }

                var EmailEmUsoPremium = await _context.UsuariosPremium.FirstOrDefaultAsync(u => u.Email == usuarioPremium.Email);
                if (EmailEmUsoPremium != null)
                {
                    ModelState.AddModelError("Email", "Este e-mail já está registrado.");
                    return View("~/Views/Registro/RegistroUsuarioPremium.cshtml");
                }

                var CpfEmUsoPadrao = await _contextNormal.Usuarios.FirstOrDefaultAsync(u => u.CPF == usuarioPremium.CPF);
                if (CpfEmUsoPadrao != null)
                {
                    ModelState.AddModelError("CPF", "Este CPF já está registrado.");
                    return View("~/Views/Registro/RegistroUsuarioPremium.cshtml");
                }

                var EmailEmUsoPadrao = await _contextNormal.Usuarios.FirstOrDefaultAsync(u => u.Email == usuarioPremium.Email);
                if (EmailEmUsoPadrao != null)
                {
                    ModelState.AddModelError("Email", "Este email já está registrado.");
                    return View("~/Views/Registro/RegistroUsuarioPremium.cshtml");
                }

                bool TelefoneValido = ValidarTelefone.ValidarTeledone(usuarioPremium.Telefone);
                if (TelefoneValido == false)
                {
                    ModelState.AddModelError("Telefone", "Telefone invalido.");
                    return View("~/Views/Registro/RegistroUsuarioPremium.cshtml");
                }

                bool IdadeValida = ValidaIdade.ValidarIdade(usuarioPremium.Idade);
                if (IdadeValida == false)
                {
                    ModelState.AddModelError("Idade", "Idade invalida.");
                    return View("~/Views/Registro/RegistroUsuarioPremium.cshtml");
                }


                ViaCepClient viaCepClient = new ViaCepClient();
                string cep2 = usuarioPremium.CEP;
                var ResgatarCep = viaCepClient.Search(cep2);

                if (ResgatarCep == null)
                {
                    ModelState.AddModelError("CEP", "Cep invalido.");
                    return View("~/Views/Registro/RegistroUsuarioPremium.cshtml");
                }

                usuarioPremium.Tipo = 2;

                string senha = usuarioPremium.Senha;
                usuarioPremium.Senha = Criptografia.GerarHash(senha);

                usuarioPremium.Estado = ResgatarCep.EstadoUf;
                usuarioPremium.Cidade = ResgatarCep.Cidade;
                usuarioPremium.Bairro = ResgatarCep.Bairro;

                _context.Add(usuarioPremium);
                await _context.SaveChangesAsync();

                var EmailDeEnvio = new Email("smtp.gmail.com", "contatoeventplanner@gmail.com", "ipmbosrsubhctwkk");
                var ListaEmail = new List<string> { usuarioPremium.Email };
                string Assunto = "Registro Concluído";
                string Corpo = usuarioPremium.Nome + " Seu registro como usuario premium foi  concluído com sucesso!";

                EmailDeEnvio.SendEmail(ListaEmail, Assunto, Corpo);

                return View("~/Views/Home/Index.cshtml");
            }

            return View("~/Views/Registro/RegistroUsuarioPremium.cshtml");
        }
        // GET: UsuarioP/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null || _context.UsuariosPremium == null)
        //    {
        //        return NotFound();
        //    }

        //    var usuarioPremium = await _context.UsuariosPremium.FindAsync(id);
        //    if (usuarioPremium == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(usuarioPremium);
        //}

        // POST: UsuarioP/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("ID,Nome,Email,CPF,Telefone,Idade,Senha,CEP,Rua,Cidade,Estado,NumeroCartao,DataValidade,CodigoSeguranca,Tipo")] UsuarioPremium UsuarioPremiumCopia)
        {
            var UsuarioPremiumIdString = HttpContext.Session.GetString("UsuarioPremiumID");
            if (UsuarioPremiumIdString == null)
            {
                return RedirectToAction("Login");
            }
                try
                {
                int UsuarioPremiumId = int.Parse(UsuarioPremiumIdString);
                var UsuarioPremium = await _context.UsuariosPremium.FirstOrDefaultAsync(u => u.ID == UsuarioPremiumId);
                UsuarioPremium.Nome = UsuarioPremiumCopia.Nome;
                UsuarioPremium.Telefone = UsuarioPremiumCopia.Telefone;
                UsuarioPremium.Email = UsuarioPremiumCopia.Email;
                UsuarioPremium.Idade = UsuarioPremiumCopia.Idade;
                UsuarioPremium.CEP = UsuarioPremiumCopia.CEP;
                UsuarioPremium.Estado = UsuarioPremiumCopia.Estado;
                UsuarioPremium.Cidade = UsuarioPremiumCopia.Cidade;
                UsuarioPremium.Bairro = UsuarioPremiumCopia.Bairro;
                UsuarioPremium.NumeroCartao = UsuarioPremiumCopia.NumeroCartao;
                UsuarioPremium.TitularCartao = UsuarioPremiumCopia.TitularCartao;
                UsuarioPremium.DataValidade = UsuarioPremiumCopia.DataValidade;
                UsuarioPremium.CodigoSeguranca = UsuarioPremiumCopia.CodigoSeguranca;
                _context.Update(UsuarioPremium);
                await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioPremiumExists(UsuarioPremiumCopia.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                
                return RedirectToAction(nameof(Index));
            }
            return View("~/Views/Detalhes/DetalhesUsuarioPremium.cshtml", UsuarioPremiumCopia);
        }

        // GET: UsuarioP/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.UsuariosPremium == null)
            {
                return NotFound();
            }

            var usuarioPremium = await _context.UsuariosPremium
                .FirstOrDefaultAsync(m => m.ID == id);
            if (usuarioPremium == null)
            {
                return NotFound();
            }

            return View(usuarioPremium);
        }

        // POST: UsuarioP/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.UsuariosPremium == null)
            {
                return Problem("Entity set 'UsuarioPremiumBD.UsuariosPremium'  is null.");
            }
            var usuarioPremium = await _context.UsuariosPremium.FindAsync(id);
            if (usuarioPremium != null)
            {
                _context.UsuariosPremium.Remove(usuarioPremium);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioPremiumExists(int id)
        {
            return (_context.UsuariosPremium?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}