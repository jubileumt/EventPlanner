using EventPlanner.Models;
using EventPlanner.Validadores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using EventPlanner.Data;
using ViaCepConsumer;

namespace TesteMVC2.Controllers
{
    public class EventoController : Controller
    {
        private readonly EventosBD _context;
        private readonly UsuarioPremiumBD _contextPremium;
        private readonly UsuarioBD _contextNormal;
        private readonly AvaliacaoBD _contextAvaliacao;
        public EventoController(EventosBD context, UsuarioPremiumBD contextPremium, UsuarioBD contextNormal, AvaliacaoBD contextAvaliacao)
        {
            _context = context;
            _contextPremium = contextPremium;
            _contextNormal = contextNormal;
            _contextAvaliacao = contextAvaliacao;
        }
        public async Task<IActionResult> Index()
        {
            return _context.Eventos != null ?
                        View(await _context.Eventos.ToListAsync()) :
                        Problem("Entity set 'EventosBD.Eventos'  is null.");
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Eventos == null)
            {
                return NotFound();
            }

            var evento = await _context.Eventos
                .FirstOrDefaultAsync(m => m.ID == id);
            if (evento == null)
            {
                return NotFound();
            }

            return View(evento);
        }
        public IActionResult Create()
        {
            return View();
        }
        public async Task<byte[]> ProcessarImagemEvento()
        {
            var imagemEvento = Request.Form.Files["FotoDoEvento"];
            if (imagemEvento != null && imagemEvento.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    ModelState.AddModelError("FotoDoEvento", "Foto enviada");
                    await imagemEvento.CopyToAsync(memoryStream);
                    return memoryStream.ToArray();
                }
            }
            else
            {
                ModelState.AddModelError("FotoDoEvento", "Foto nâo enviada");
                return null;
            }
        }

        public string GerarIdentificador()
        {
            Random IdentificadorAleatorioParaOEvento = new Random();
            char letra = (char)('A' + IdentificadorAleatorioParaOEvento.Next(0, 26));
            int numero = IdentificadorAleatorioParaOEvento.Next(1000, 10000);
            return letra.ToString() + numero.ToString();
        }

        public void DefinirAtributosEvento(Evento evento)
        {
            evento.TipoEvento = "Evento infantil";
            evento.DataCriacao = DateTime.Now;

            evento.TempoAteEvento = evento.DataInicio.Subtract(DateTime.Now);
            evento.Duracao = evento.DataFinal - evento.DataInicio;
        }

        public void ValidarDataEvento(Evento evento)
        {
            bool dataInicioInvalida = DateTime.Now > evento.DataInicio;
            bool dataFinalInvalida = DateTime.Now > evento.DataFinal;
            bool dataInicioMaiorQueDataFinal = evento.DataInicio > evento.DataFinal;
            bool duracaoExcede24Horas = (evento.DataFinal - evento.DataInicio).TotalHours > 24;
            bool QuantMaxPessoasInvalida = evento.QuantMaxPessoas == null || evento.QuantMaxPessoas <= 0;
            bool QuantCriancasInvalida = evento.QuantCriancas == null || evento.QuantCriancas <= 0;

            if (dataInicioInvalida)
            {
                ModelState.AddModelError("DataInicio", "Data inicial não pode ser antes da data atual!");
            }

            if (dataFinalInvalida)
            {
                ModelState.AddModelError("DataFinal", "Data final não pode ser antes da data atual!");
            }

            if (dataInicioMaiorQueDataFinal)
            {
                ModelState.AddModelError("DataFinal", "Data final não pode ser antes da data inicial!");
            }

            if (duracaoExcede24Horas)
            {
                ModelState.AddModelError("DataFinal", "A duração do evento não pode exceder 24 horas!");
            }

            if (QuantMaxPessoasInvalida)
            {
                ModelState.AddModelError("QuantMaxPessoas", "Quantidade máxima de pessoas não pode ser menor ou igual a 0!");
            }

            if (QuantCriancasInvalida)
            {
                ModelState.AddModelError("QuantCriancas", "Quantidade de crianças não pode ser menor ou igual a 0!");
            }
        }

        private async Task EnviarEmailEventoCriadoPremium(UsuarioPremium UsuarioAtual, Evento NovoEvento)
        {
            var ServicoEmail = new Email("smtp.gmail.com", "contatoeventplanner@gmail.com", "ipmbosrsubhctwkk");
            var ListaEmails = new List<string> { UsuarioAtual.Email };
            string AssuntoEmail = "Evento registrado com sucesso";
            string CorpoEmail = UsuarioAtual.Nome + " Seu registro para o evento " + NovoEvento.NomeEvento + " concluido com sucesso, esse é o codigo do seu evento: " + NovoEvento.Identificador;

            ServicoEmail.SendEmail(ListaEmails, AssuntoEmail, CorpoEmail);
        }
        private async Task EnviarEmailEventoCriado(Usuario UsuarioAtual, Evento NovoEvento)
        {
            var ServicoEmail = new Email("smtp.gmail.com", "contatoeventplanner@gmail.com", "ipmbosrsubhctwkk");
            var ListaEmails = new List<string> { UsuarioAtual.Email };
            string AssuntoEmail = "Evento registrado com sucesso";
            string CorpoEmail = UsuarioAtual.Nome + " Seu registro para o evento " + NovoEvento.NomeEvento + " concluido com sucesso, esse é o codigo do seu evento: " + NovoEvento.Identificador;

            ServicoEmail.SendEmail(ListaEmails, AssuntoEmail, CorpoEmail);
        }
        public async Task EnvioEmail(Evento NovoEvento)
        {
            HttpContext.Session.SetString("EventoId", NovoEvento.ID.ToString());

            await AtualizarChavesUsuario(NovoEvento);

            var UsuarioRetornado = await GetUsuarioPorId();
            var ResultadoOk = UsuarioRetornado as OkObjectResult;

            var UsuarioEncontrado = ResultadoOk.Value as Usuario;
            var UsuarioPremiumEncontrado = ResultadoOk.Value as UsuarioPremium;

            if (UsuarioEncontrado != null)
            {
                await EnviarEmailEventoCriado(UsuarioEncontrado, NovoEvento);
            }
            else if (UsuarioPremiumEncontrado != null)
            {
                await EnviarEmailEventoCriadoPremium(UsuarioPremiumEncontrado, NovoEvento);
            }
        }

        private async Task<bool> AtualizarChavesUsuario(Evento NovoEvento)
        {
            var IdUsuarioString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(IdUsuarioString))
            {
                return false;
            }

            var IdUsuario = int.Parse(IdUsuarioString);
            var ResultadoAcaoUsuario = await GetUsuarioPorId();
            var ResultadoOk = ResultadoAcaoUsuario as OkObjectResult;

            if (ResultadoOk == null)
            {
                return false;
            }

            var UsuarioEncontrado = ResultadoOk.Value as Usuario;
            var UsuarioPremiumEncontrado = ResultadoOk.Value as UsuarioPremium;

            if (UsuarioEncontrado != null)
            {
                UsuarioEncontrado.eventoID = NovoEvento.ID;
                NovoEvento.UsuarioID = UsuarioEncontrado.ID;
                NovoEvento.Organizador = UsuarioEncontrado.Nome;
                _contextNormal.Update(UsuarioEncontrado);
                await _contextNormal.SaveChangesAsync();
            }
            else if (UsuarioPremiumEncontrado != null)
            {
                UsuarioPremiumEncontrado.eventoID = NovoEvento.ID;
                NovoEvento.UsuarioPremiumID = UsuarioPremiumEncontrado.ID;
                NovoEvento.Organizador = UsuarioPremiumEncontrado.Nome;
                _contextPremium.Update(UsuarioPremiumEncontrado);
                await _contextPremium.SaveChangesAsync();
            }
            else
            {
                return false;
            }

            return true;
        }
        public async Task<IActionResult> GetUsuarioPorId()
        {
            var UsuarioIdString = HttpContext.Session.GetString("UserId");
            if (UsuarioIdString == null)
            {
                return RedirectToAction("Login");
            }

            var UsuarioId = int.Parse(UsuarioIdString);

            var Usuario = await _contextNormal.Usuarios.FindAsync(UsuarioId);
            if (Usuario != null)
            {
                return Ok(Usuario);
            }

            var UsuarioPremium = await _contextPremium.UsuariosPremium.FindAsync(UsuarioId);
            if (UsuarioPremium != null)
            {
                return Ok(UsuarioPremium);
            }

            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CriarEventoInfantil([Bind("ID,NomeEvento,Descricao,DataInicio,DataFinal,QuantMetrosQuadrados,QuantMaxPessoas,QuantCriancas,QuantRefri,QuantAlcool,QuantCarne,QuantDoces,QuantSalgados,QuantCadeiras,CEP,Estado,Cidade,Bairro,FotoDoEvento,TipoEvento,Organizador,UsuarioPremiumID")] Evento NovoEvento)
        {
            ValidarDataEvento(NovoEvento);

            if (!ModelState.IsValid)
            {
                var UsuarioIdString = HttpContext.Session.GetString("UserId");
                var UsuarioId = int.Parse(UsuarioIdString);
                var Usuario = await _contextNormal.Usuarios.FindAsync(UsuarioId);
                if (Usuario != null)
                {
                    return View("~/Views/Registro/RegistroEventoInfantilNormal.cshtml", NovoEvento);
                }
                else
                {
                    return View("~/Views/Registro/RegistroEventoInfantil.cshtml", NovoEvento);
                }
            }

            NovoEvento.FotoDoEvento = await ProcessarImagemEvento();
            NovoEvento.Identificador = GerarIdentificador();

            DefinirAtributosEvento(NovoEvento);

            _context.Add(NovoEvento);

            await _context.SaveChangesAsync();

            await EnvioEmail(NovoEvento);

            _context.Update(NovoEvento);

            await _context.SaveChangesAsync();

            return View("~/Views/Home/MenuEvento.cshtml", NovoEvento);
        }


        public async Task<IActionResult> CriarEventoAniversario([Bind("ID,NomeEvento,Descricao,DataInicio,DataFinal,QuantMetrosQuadrados,QuantMaxPessoas,QuantRefri,QuantAlcool,QuantCarne,QuantDoces,QuantSalgados,QuantCadeiras,CEP,Estado,Cidade,Bairro,FotoDoEvento,TipoEvento,Organizador,UsuarioPremiumID")] Evento evento)
        {
            if (ModelState.IsValid)
            {
                ValidarDataEvento(evento);

                if (!ModelState.IsValid)
                {
                    return View("~/Views/Registro/RegistroEvento.cshtml", evento);
                }

                evento.FotoDoEvento = await ProcessarImagemEvento();

                evento.Identificador = GerarIdentificador();

                evento.TipoEvento = "Aniversário";

                var UsuarioIdString = HttpContext.Session.GetString("UserId");
                var userId = int.Parse(UsuarioIdString);

                HttpContext.Session.SetString("EventoId", evento.ID.ToString());

                var usuarioPremium = await _contextPremium.UsuariosPremium.FirstOrDefaultAsync(Usuario => Usuario.ID == userId);

                DefinirAtributosEvento(evento);

                _context.Add(evento);
                await _context.SaveChangesAsync();

                usuarioPremium.eventoID = evento.ID;

                var ServicoEmail = new Email("smtp.gmail.com", "contatoeventplanner@gmail.com", "ipmbosrsubhctwkk");
                var ListaEmail = new List<string> { usuarioPremium.Email };
                string Assunto = "Evento registrado com sucesso";
                string Corpo = usuarioPremium.Nome + " Seu registro para o evento " + evento.NomeEvento + " concluido com sucesso, esse é o codigo do seu evento: " + evento.Identificador;

                ServicoEmail.SendEmail(ListaEmail, Assunto, Corpo);

                _contextPremium.Update(usuarioPremium);
                await _contextPremium.SaveChangesAsync();

                return View("~/Views/Registro/AtualizarEvento.cshtml", evento);
            }
            return View("~/Views/Registro/RegistroEvento.cshtml");
        }
        public async Task<IActionResult> EstatisticasEventoInfantil()
        {
            var EventoId = HttpContext.Session.GetString("EventoId");
            var EventoExistente = await _context.Eventos.FindAsync(int.Parse(EventoId));
            if (EventoExistente == null)
            {
                return View("~/Views/Registro/RegistroEvento.cshtml");
            }

            EventoExistente.MediaSalgadosPorPessoa = EventoExistente.QuantSalgados.HasValue && EventoExistente.QuantMaxPessoas.HasValue
                ? (double)EventoExistente.QuantSalgados.Value / EventoExistente.QuantMaxPessoas.Value
                : 0;

            EventoExistente.MediaDocesPorPessoa = EventoExistente.QuantDoces.HasValue && EventoExistente.QuantMaxPessoas.HasValue
                ? (double)EventoExistente.QuantDoces.Value / EventoExistente.QuantMaxPessoas.Value
                : 0;

            EventoExistente.MediaRefriPorPessoa = EventoExistente.QuantRefri.HasValue && EventoExistente.QuantMaxPessoas.HasValue
                ? (double)EventoExistente.QuantRefri.Value / EventoExistente.QuantMaxPessoas.Value
                : 0;

            EventoExistente.MediaCadeiraPorPessoa = EventoExistente.QuantCadeiras.HasValue && EventoExistente.QuantMaxPessoas.HasValue
                ? (double)EventoExistente.QuantCadeiras.Value / EventoExistente.QuantMaxPessoas.Value
                : 0;

            EventoExistente.MediaMetrosPorPessoa = EventoExistente.QuantMetrosQuadrados.HasValue && EventoExistente.QuantMaxPessoas.HasValue && EventoExistente.QuantMaxPessoas.Value != 0
                ? (double)EventoExistente.QuantMetrosQuadrados.Value / EventoExistente.QuantMaxPessoas.Value
                : 0;

            double margemSeguranca = 0.2;

            EventoExistente.QuantidadeDeCopos = EventoExistente.QuantMaxPessoas.HasValue
                ? EventoExistente.QuantMaxPessoas.Value * 4 * (1 + margemSeguranca)
                : 0;

            EventoExistente.QuantidadeDePratos = EventoExistente.QuantMaxPessoas.HasValue
                ? EventoExistente.QuantMaxPessoas.Value * (1 + margemSeguranca)
                : 0;

            EventoExistente.QuantidadeTalheres = EventoExistente.QuantMaxPessoas.HasValue
                ? (int)Math.Round(EventoExistente.QuantMaxPessoas.Value * (1 + margemSeguranca))
                : 0;

            if (EventoExistente.QuantRefri.HasValue)
            {
                double coposPorRefri = EventoExistente.QuantRefri.Value * 2 * 10;
                EventoExistente.QuantidadeDeCopos = Math.Max(EventoExistente.QuantidadeDeCopos.Value, coposPorRefri * (1 + margemSeguranca));
            }

            EventoExistente.QuantidadeQuardanapos = EventoExistente.QuantSalgados.HasValue
                ? EventoExistente.QuantSalgados.Value * (1 + margemSeguranca)
                : 0;

            if (EventoExistente.QuantMaxPessoas.HasValue)
            {
                double guardanaposPorPessoa = EventoExistente.QuantMaxPessoas.Value * 12.5;
                EventoExistente.QuantidadeQuardanapos = Math.Max(EventoExistente.QuantidadeQuardanapos.Value, guardanaposPorPessoa * (1 + margemSeguranca));
            }

            double? TotalAdulto = EventoExistente.QuantMaxPessoas.HasValue
                ? EventoExistente.QuantMaxPessoas - EventoExistente.QuantCriancas
                : 0;

            EventoExistente.PercentualDeCrianças = EventoExistente.QuantCriancas.HasValue && EventoExistente.QuantMaxPessoas.HasValue
                ? (double)EventoExistente.QuantCriancas / (double)EventoExistente.QuantMaxPessoas * 100
                : 0;

            EventoExistente.PercentualDeAdultos = TotalAdulto.HasValue && EventoExistente.QuantMaxPessoas.HasValue
                ? TotalAdulto / EventoExistente.QuantMaxPessoas * 100
                : 0;

            return View("~/Views/Detalhes/EstatisticasEventoInfantil.cshtml", EventoExistente);
        }
        public async Task<IActionResult> EstatisticasEventoAdulto()
        {
            var EventoId = HttpContext.Session.GetString("EventoId");
            var EventoExistente = await _context.Eventos.FindAsync(int.Parse(EventoId));

            if (EventoExistente == null)
            {
                return View("~/Views/Registro/RegistroEvento.cshtml", EventoExistente);
            }

            if (EventoExistente.QuantAlcool.HasValue && EventoExistente.QuantMaxPessoas.HasValue)
            {
                var Media = (double)EventoExistente.QuantAlcool.Value / EventoExistente.QuantMaxPessoas.Value;
                EventoExistente.MediaAlcoolPorPessoa = Media;
            }
            else
            {
                EventoExistente.MediaAlcoolPorPessoa = 0;
            }


            if (EventoExistente.QuantSalgados.HasValue && EventoExistente.QuantMaxPessoas.HasValue)
            {
                var Media = (double)EventoExistente.QuantSalgados.Value / EventoExistente.QuantMaxPessoas.Value;
                EventoExistente.MediaSalgadosPorPessoa = Media;
            }
            else
            {
                EventoExistente.MediaSalgadosPorPessoa = 0;
            }

            if (EventoExistente.QuantDoces.HasValue && EventoExistente.QuantMaxPessoas.HasValue)
            {
                var Media = (double)EventoExistente.QuantDoces.Value / EventoExistente.QuantMaxPessoas.Value;
                EventoExistente.MediaDocesPorPessoa = Media;
            }
            else
            {
                EventoExistente.MediaDocesPorPessoa = 0;
            }

            if (EventoExistente.QuantRefri.HasValue && EventoExistente.QuantMaxPessoas.HasValue)
            {
                var Media = (double)EventoExistente.QuantRefri.Value / EventoExistente.QuantMaxPessoas.Value;
                EventoExistente.MediaRefriPorPessoa = Media;
            }
            else
            {
                EventoExistente.MediaRefriPorPessoa = 0;
            }

            if (EventoExistente.QuantAlcool.HasValue && EventoExistente.QuantMaxPessoas.HasValue)
            {
                var mediaAlcool = (double)EventoExistente.QuantAlcool.Value / EventoExistente.QuantMaxPessoas.Value;
                EventoExistente.MediaAlcoolPorPessoa = mediaAlcool;
            }
            else
            {
                EventoExistente.MediaAlcoolPorPessoa = 0;
            }


            if (EventoExistente.QuantCadeiras.HasValue && EventoExistente.QuantMaxPessoas.HasValue)
            {
                var Media = (double)EventoExistente.QuantCadeiras.Value / EventoExistente.QuantMaxPessoas.Value;
                EventoExistente.MediaCadeiraPorPessoa = Media;
            }
            else
            {
                EventoExistente.MediaCadeiraPorPessoa = 0;
            }

            if (EventoExistente.QuantMetrosQuadrados.HasValue && EventoExistente.QuantMaxPessoas.HasValue && EventoExistente.QuantMaxPessoas.Value != 0)
            {
                var Media = (double)EventoExistente.QuantMetrosQuadrados.Value / EventoExistente.QuantMaxPessoas.Value;
                EventoExistente.MediaMetrosPorPessoa = Media;
            }
            else
            {
                EventoExistente.MediaMetrosPorPessoa = 0;
            }

            double margemSeguranca = 0.2;

            if (EventoExistente.QuantMaxPessoas.HasValue)
            {
                EventoExistente.QuantidadeDeCopos = EventoExistente.QuantMaxPessoas.Value * 4 * (1 + margemSeguranca);

                EventoExistente.QuantidadeDePratos = EventoExistente.QuantMaxPessoas.Value * (1 + margemSeguranca);
                EventoExistente.QuantidadeTalheres = (int)Math.Round(EventoExistente.QuantMaxPessoas.Value * (1 + margemSeguranca));
            }

            if (EventoExistente.QuantRefri.HasValue || EventoExistente.QuantAlcool.HasValue)
            {
                double coposPorRefri = EventoExistente.QuantRefri.GetValueOrDefault(0) * 2 * 10;
                double coposPorAlcool = EventoExistente.QuantAlcool.GetValueOrDefault(0) * 2 * 10;

                double coposNecessarios = Math.Max(coposPorRefri, coposPorAlcool);
                EventoExistente.QuantidadeDeCopos = Math.Max(EventoExistente.QuantidadeDeCopos.GetValueOrDefault(0), coposNecessarios * (1 + margemSeguranca));
            }


            if (EventoExistente.QuantSalgados.HasValue)
            {
                EventoExistente.QuantidadeQuardanapos = EventoExistente.QuantSalgados.Value * (1 + margemSeguranca);
            }

            double guardanaposPorPessoa = EventoExistente.QuantMaxPessoas.Value * 12.5;
            EventoExistente.QuantidadeQuardanapos = Math.Max(EventoExistente.QuantidadeQuardanapos.Value, guardanaposPorPessoa * (1 + margemSeguranca));

            double? TotalAdulto = EventoExistente.QuantMaxPessoas - EventoExistente.QuantCriancas;

            EventoExistente.PercentualDeCrianças = (double)EventoExistente.QuantCriancas / (double)EventoExistente.QuantMaxPessoas * 100;
            EventoExistente.PercentualDeAdultos = TotalAdulto / EventoExistente.QuantMaxPessoas * 100;




            return View("~/Views/Detalhes/EstatisticasEventoInfantil.cshtml", EventoExistente);
        }
        public async Task<IActionResult> EventoEncontrado(int id, string buscador)
        {
            var model = await _context.Eventos.FindAsync(id);

            if (string.IsNullOrEmpty(buscador))
            {
                ModelState.AddModelError("Buscador", "Buscador não pode estar vazio");
                return View("~/Views/Home/MenuEvento.cshtml", model);
            }

            var EventoResgatado = await _context.Eventos.FirstOrDefaultAsync(e => e.NomeEvento.ToLower() == buscador.ToLower());

            if (EventoResgatado == null)
            {
                EventoResgatado = await _context.Eventos.FirstOrDefaultAsync(e => e.Identificador == buscador);
            }

            if (EventoResgatado == null)
            {
                ModelState.AddModelError("Buscador", "Evento não registrado");
                return View("~/Views/Home/MenuEvento.cshtml", model);
            }

            EventoResgatado.Avaliacoes = await _contextAvaliacao.Avaliacao.Where(a => a.EventoID == EventoResgatado.ID).ToListAsync();

            return View("~/Views/Detalhes/DetalhesEventoAchado.cshtml", EventoResgatado);
        }
        public ActionResult DisplayImage(int id)
        {
            var evento = _context.Eventos.FirstOrDefault(m => m.ID == id);
            if (evento == null || evento.FotoDoEvento == null)
            {
                return NotFound();
            }

            return File(evento.FotoDoEvento, "image/jpeg");
        }
        public async Task<IActionResult> EditarParaView(int? id)
        {

            if (id == null || _context.Eventos == null)
            {
                return NotFound();
            }

            var evento = await _context.Eventos.FindAsync(id);
            if (evento == null)
            {
                return NotFound();
            }
            return View("~/Views/Registro/AtualizarEvento.cshtml", evento);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("ID,NomeEvento,Descricao,DataInicio,DataFinal,QuantMetrosQuadrados,QuantMaxPessoas,QuantRefri,QuantCarne,QuantDoces,QuantSalgados,QuantCadeiras,CEP,Estado,Cidade,Bairro,FotoDoEvento,TipoEvento,Organizador,UsuarioPremiumID")] Evento EventoNovo)
        {
            var EventoId = HttpContext.Session.GetString("EventoId");
            var UsuarioIdString = HttpContext.Session.GetString("UserId");
            var IDUsuario = int.Parse(UsuarioIdString);

            if (EventoId == null)
            {
                return View("~/Views/Registro/RegistroEvento.cshtml", EventoNovo);
            }

            var EventoExistente = await _context.Eventos.FindAsync(int.Parse(EventoId));
            if (EventoExistente == null)
            {
                return View("~/Views/Registro/RegistroEvento.cshtml", EventoNovo);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    EventoExistente.NomeEvento = EventoNovo.NomeEvento;
                    EventoExistente.Descricao = EventoNovo.Descricao;
                    EventoExistente.QuantMaxPessoas = EventoNovo.QuantMaxPessoas;
                    EventoExistente.QuantCriancas = EventoNovo.QuantCriancas;
                    EventoExistente.QuantMetrosQuadrados = EventoNovo.QuantMetrosQuadrados;
                    EventoExistente.QuantMaxPessoas = EventoNovo.QuantMaxPessoas;
                    EventoExistente.QuantRefri = EventoNovo.QuantRefri;
                    EventoExistente.QuantCarne = EventoNovo.QuantCarne;
                    EventoExistente.QuantDoces = EventoNovo.QuantDoces;
                    EventoExistente.QuantSalgados = EventoNovo.QuantSalgados;
                    EventoExistente.QuantCadeiras = EventoNovo.QuantCadeiras;

                    if (EventoNovo.FotoDoEvento != null)
                    {
                        EventoExistente.FotoDoEvento = EventoNovo.FotoDoEvento;
                    }

                    var UsuarioPremium = await _contextPremium.UsuariosPremium.FindAsync(IDUsuario);

                    var EmailDeEnvio = new Email("smtp.gmail.com", "contatoeventplanner@gmail.com", "ipmbosrsubhctwkk");
                    var ListaEmail = new List<string> { UsuarioPremium.Email };
                    string Assunto = "Evento Atualizado";
                    string Corpo = UsuarioPremium.Nome + ", seu evento " + EventoExistente.NomeEvento + " de código " + EventoExistente.Identificador + " foi atualizado com sucesso!";
                    EmailDeEnvio.SendEmail(ListaEmail, Assunto, Corpo);

                    _context.Update(EventoExistente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventoExists(EventoExistente.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return View("~/Views/Registro/AtualizarEvento.cshtml", EventoExistente);
            }
            return View("~/Views/Registro/AtualizarEvento.cshtml", EventoNovo);
        }
        public async Task<IActionResult> Delete(int? ID)
        {
            if (ID == null || _context.Eventos == null)
            {
                return NotFound();
            }

            var Evento = await _context.Eventos.FirstOrDefaultAsync(m => m.ID == ID);
            if (Evento == null)
            {
                return NotFound();
            }

            var EventoId = HttpContext.Session.GetString("EventoId");
            var EventoExistente = await _context.Eventos.FindAsync(int.Parse(EventoId));

            if (EventoExistente == null)
            {
                return Problem("Entity set 'EventosBD.Eventos'  is null.");
            }

            var UsuarioIdString = HttpContext.Session.GetString("UserId");
            if (int.TryParse(UsuarioIdString, out var UserId))
            {
                var UsuarioPremium = await _contextPremium.UsuariosPremium.FindAsync(UserId);
                if (UsuarioPremium != null)
                {
                    UsuarioPremium.eventoID = null;
                    _contextPremium.Update(UsuarioPremium);
                    await _contextPremium.SaveChangesAsync();

                    var EmailDeEnvio = new Email("smtp.gmail.com", "contatoeventplanner@gmail.com", "ipmbosrsubhctwkk");
                    var ListaEmail = new List<string> { UsuarioPremium.Email };
                    string Asssunto = "Evento excluido";
                    string Corpo = UsuarioPremium.Nome + " seu evento" + EventoExistente.NomeEvento + " de codigo" + EventoExistente.Identificador + "foi excluido com sucesso";
                    EmailDeEnvio.SendEmail(ListaEmail, Asssunto, Corpo);
                }
            }

            _context.Eventos.Remove(EventoExistente);

            await _context.SaveChangesAsync();
            return View("~/Views/Home/MenuEvento.cshtml");
        }

        [HttpPost]
        public async Task<ActionResult> AdicionarComentario(int id, string comentario)
        {
            var EventoResgatado = await _context.Eventos.FindAsync(id);
            if (EventoResgatado == null)
            {
                return NotFound();
            }

            var UsuarioIdString = HttpContext.Session.GetString("UserId");
            var IDusuario = int.Parse(UsuarioIdString);

            var UsuarioResgatado = await _contextPremium.UsuariosPremium.FindAsync(IDusuario);

            Avaliacao avaliacao = new Avaliacao();
            avaliacao.Comentario = comentario;
            avaliacao.DataComentario = DateOnly.FromDateTime(DateTime.Now);
            avaliacao.UsuarioPremiumID = UsuarioResgatado.ID;
            avaliacao.EventoID = EventoResgatado.ID;
            avaliacao.UsuarioID = 0;

            _contextAvaliacao.Add(avaliacao);
            await _contextAvaliacao.SaveChangesAsync();

            EventoResgatado.Avaliacoes = await _contextAvaliacao.Avaliacao.Where(a => a.EventoID == EventoResgatado.ID).ToListAsync();

            return View("~/Views/Detalhes/DetalhesEventoAchado.cshtml", EventoResgatado);
        }

        [HttpPost]
        public async Task<IActionResult> GetEventos(string term)
        {

            var eventos = await _context.Eventos
                .Where(e => e.NomeEvento.Contains(term))
                .ToListAsync();

            var nomesEventos = eventos.Select(e => e.NomeEvento).ToList();

            return Json(nomesEventos);
        }
        private bool EventoExists(int id)
        {
            return (_context.Eventos?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}