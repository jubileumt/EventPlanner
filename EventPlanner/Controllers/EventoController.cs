using EventPlanner.Data;
using EventPlanner.Models;
using EventPlanner.Validadores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TesteMVC2.Controllers;

public class EventoController : Controller
{
    readonly EventosBD _context;
    readonly UsuarioPremiumBD _contextPremium;
    readonly UsuarioBD _contextNormal;
    readonly AvaliacaoBD _contextAvaliacao;

    public EventoController(EventosBD context, UsuarioPremiumBD contextPremium, UsuarioBD contextNormal,
        AvaliacaoBD contextAvaliacao)
    {
        _context = context;
        _contextPremium = contextPremium;
        _contextNormal = contextNormal;
        _contextAvaliacao = contextAvaliacao;
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || _context.Eventos == null) return NotFound();

        var evento = await _context.Eventos
            .FirstOrDefaultAsync(m => m.ID == id);

        if (evento == null) return NotFound();

        return View(evento);
    }

    public async Task<byte[]> ProcessarImagemEvento()
    {
        var imagemEvento = Request.Form.Files["FotoDoEvento"];

        if (imagemEvento != null && imagemEvento.Length > 0)
            using (var memoryStream = new MemoryStream())
            {
                ModelState.AddModelError("FotoDoEvento", "Foto enviada");
                await imagemEvento.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }

        ModelState.AddModelError("FotoDoEvento", "Foto nâo enviada");
        return null;
    }

    public string GerarIdentificador()
    {
        var IdentificadorAleatorioParaOEvento = new Random();
        var letra = (char)('A' + IdentificadorAleatorioParaOEvento.Next(0, 26));
        var numero = IdentificadorAleatorioParaOEvento.Next(1000, 10000);
        return letra + numero.ToString();
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
        var dataInicioInvalida = DateTime.Now > evento.DataInicio;
        var dataFinalInvalida = DateTime.Now > evento.DataFinal;
        var dataInicioMaiorQueDataFinal = evento.DataInicio > evento.DataFinal;
        var duracaoExcede24Horas = (evento.DataFinal - evento.DataInicio).TotalHours > 24;
        var QuantMaxPessoasInvalida = evento.QuantMaxPessoas == null || evento.QuantMaxPessoas <= 0;
        var QuantCriancasInvalida = evento.QuantCriancas == null || evento.QuantCriancas <= 0;

        if (dataInicioInvalida)
            ModelState.AddModelError("DataInicio", "Data inicial não pode ser antes da data atual!");

        if (dataFinalInvalida) ModelState.AddModelError("DataFinal", "Data final não pode ser antes da data atual!");

        if (dataInicioMaiorQueDataFinal)
            ModelState.AddModelError("DataFinal", "Data final não pode ser antes da data inicial!");

        if (duracaoExcede24Horas)
            ModelState.AddModelError("DataFinal", "A duração do evento não pode exceder 24 horas!");

        if (QuantMaxPessoasInvalida)
            ModelState.AddModelError("QuantMaxPessoas",
                "Quantidade máxima de pessoas não pode ser menor ou igual a 0!");

        if (QuantCriancasInvalida)
            ModelState.AddModelError("QuantCriancas", "Quantidade de crianças não pode ser menor ou igual a 0!");
    }

    async Task EnviarEmailEventoCriadoPremium(UsuarioPremium UsuarioAtual, Evento NovoEvento)
    {
        var ServicoEmail = new Email("smtp.gmail.com", "contatoeventplanner@gmail.com", "ipmbosrsubhctwkk");
        var ListaEmails = new List<string> { UsuarioAtual.Email };
        var AssuntoEmail = "Evento registrado com sucesso";

        var CorpoEmail = UsuarioAtual.Nome + " Seu registro para o evento " + NovoEvento.NomeEvento +
                         " concluido com sucesso, esse é o codigo do seu evento: " + NovoEvento.Identificador;

        ServicoEmail.SendEmail(ListaEmails, AssuntoEmail, CorpoEmail);
    }

    async Task EnviarEmailEventoCriado(Usuario UsuarioAtual, Evento NovoEvento)
    {
        var ServicoEmail = new Email("smtp.gmail.com", "contatoeventplanner@gmail.com", "ipmbosrsubhctwkk");
        var ListaEmails = new List<string> { UsuarioAtual.Email };
        var AssuntoEmail = "Evento registrado com sucesso";

        var CorpoEmail = UsuarioAtual.Nome + " Seu registro para o evento " + NovoEvento.NomeEvento +
                         " concluido com sucesso, esse é o codigo do seu evento: " + NovoEvento.Identificador;

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
            await EnviarEmailEventoCriado(UsuarioEncontrado, NovoEvento);
        else if (UsuarioPremiumEncontrado != null)
            await EnviarEmailEventoCriadoPremium(UsuarioPremiumEncontrado, NovoEvento);
    }

    public async Task<IActionResult> RedirecionarParaMeusEventos()
    {
        var usuarioResult = await GetUsuarioPorId();

        if (usuarioResult is OkObjectResult okObjectResult)
        {
            var usuario = okObjectResult.Value;

            if (usuario is Usuario)
                return RedirectToAction("MeusEventosNormal", "Usuario");

            if (usuario is UsuarioPremium)
                return RedirectToAction("MeusEventosPremium", "UsuarioPremium");

            return NotFound();
        }

        return NotFound();
    }

    async Task<bool> AtualizarChavesUsuario(Evento NovoEvento)
    {
        var IdUsuarioStringNormal = HttpContext.Session.GetString("UsuarioID");
        var IdUsuarioStringPremium = HttpContext.Session.GetString("UsuarioPremiumID");

        if (!string.IsNullOrEmpty(IdUsuarioStringNormal))
        {
            var IdUsuarioNormal = int.Parse(IdUsuarioStringNormal);
            var UsuarioNormal = await _contextNormal.Usuarios.FindAsync(IdUsuarioNormal);

            if (UsuarioNormal != null)
            {
                UsuarioNormal.eventoID = NovoEvento.ID;
                NovoEvento.UsuarioID = UsuarioNormal.ID;
                NovoEvento.Organizador = UsuarioNormal.Nome;
                _contextNormal.Update(UsuarioNormal);
                await _contextNormal.SaveChangesAsync();
                return true;
            }
        }
        else if (!string.IsNullOrEmpty(IdUsuarioStringPremium))
        {
            var IdUsuarioPremium = int.Parse(IdUsuarioStringPremium);
            var UsuarioPremium = await _contextPremium.UsuariosPremium.FindAsync(IdUsuarioPremium);

            if (UsuarioPremium != null)
            {
                UsuarioPremium.eventoID = NovoEvento.ID;
                NovoEvento.UsuarioPremiumID = UsuarioPremium.ID;
                NovoEvento.Organizador = UsuarioPremium.Nome;
                _contextPremium.Update(UsuarioPremium);
                await _contextPremium.SaveChangesAsync();
                return true;
            }
        }

        return false;
    }

    public async Task<IActionResult> EventosAleatorios()
    {
        var Eventos = _context.Eventos;
        if (Eventos == null) return NotFound();
        var EventosAleatorios = Eventos.OrderBy(e => Guid.NewGuid()).Take(5);
        return View("~/Views/Home/MenuPrincipal.cshtml", EventosAleatorios);
    }

    public async Task<IActionResult> CriarEventoInfantil()
    {
        IActionResult result;

        var usuarioResult = await GetUsuarioPorId();

        if (usuarioResult is OkObjectResult okObjectResult)
        {
            var usuario = okObjectResult.Value;

            if (usuario is Usuario)
                return RedirectToAction("RegistroEventoInfantilNormal", "Bussola");

            if (usuario is UsuarioPremium)
                return RedirectToAction("RegistroEventoInfantilPremium", "Bussola");

            result = NotFound();
        }
        else
        {
            result = NotFound();
        }

        return result;
    }

    async Task<IActionResult> EscolherViewParaCriarEvento(Evento novoEvento)
    {
        var UsuarioIdStringNormal = HttpContext.Session.GetString("UsuarioID");
        var UsuarioIdStringPremium = HttpContext.Session.GetString("UsuarioPremiumID");

        if (UsuarioIdStringNormal != null)
        {
            var UsuarioId = int.Parse(UsuarioIdStringNormal);
            var Usuario = await _contextNormal.Usuarios.FindAsync(UsuarioId);

            if (Usuario != null) return View("~/Views/Registro/RegistroEventoInfantilNormal.cshtml", novoEvento);
        }

        if (UsuarioIdStringPremium != null)
        {
            var UsuarioId = int.Parse(UsuarioIdStringPremium);
            var Usuario = await _contextPremium.UsuariosPremium.FindAsync(UsuarioId);

            if (Usuario != null) return View("~/Views/Registro/RegistroEventoInfantil.cshtml", novoEvento);
        }

        return View("~/Views/Shared/Erro.cshtml");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CriarEventoInfantil(
        [Bind(
            "ID,NomeEvento,Descricao,DataInicio,DataFinal,QuantMetrosQuadrados,QuantMaxPessoas,QuantCriancas,QuantRefri,QuantAlcool,QuantCarne,QuantDoces,QuantSalgados,QuantCadeiras,CEP,Estado,Cidade,Bairro,FotoDoEvento,TipoEvento,Organizador,UsuarioPremiumID")]
        Evento NovoEvento)
    {
        ValidarDataEvento(NovoEvento);

        if (!ModelState.IsValid) return await EscolherViewParaCriarEvento(NovoEvento);

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

    public async Task<IActionResult> CriarEventoAniversario(
        [Bind(
            "ID,NomeEvento,Descricao,DataInicio,DataFinal,QuantMetrosQuadrados,QuantMaxPessoas,QuantRefri,QuantAlcool,QuantCarne,QuantDoces,QuantSalgados,QuantCadeiras,CEP,Estado,Cidade,Bairro,FotoDoEvento,TipoEvento,Organizador,UsuarioPremiumID")]
        Evento evento)
    {
        if (ModelState.IsValid)
        {
            ValidarDataEvento(evento);

            if (!ModelState.IsValid) return await EscolherViewParaCriarEvento(evento);

            evento.FotoDoEvento = await ProcessarImagemEvento();

            evento.Identificador = GerarIdentificador();

            evento.TipoEvento = "Aniversário";

            var UsuarioIdString = HttpContext.Session.GetString("UserId");
            var userId = int.Parse(UsuarioIdString);

            HttpContext.Session.SetString("EventoId", evento.ID.ToString());

            var usuarioPremium =
                await _contextPremium.UsuariosPremium.FirstOrDefaultAsync(Usuario => Usuario.ID == userId);

            DefinirAtributosEvento(evento);

            _context.Add(evento);
            await _context.SaveChangesAsync();

            usuarioPremium.eventoID = evento.ID;

            var ServicoEmail = new Email("smtp.gmail.com", "contatoeventplanner@gmail.com", "ipmbosrsubhctwkk");
            var ListaEmail = new List<string> { usuarioPremium.Email };
            var Assunto = "Evento registrado com sucesso";

            var Corpo = usuarioPremium.Nome + " Seu registro para o evento " + evento.NomeEvento +
                        " concluido com sucesso, esse é o codigo do seu evento: " + evento.Identificador;

            ServicoEmail.SendEmail(ListaEmail, Assunto, Corpo);

            _contextPremium.Update(usuarioPremium);
            await _contextPremium.SaveChangesAsync();

            return View("~/Views/Home/MenuEvento.cshtml", evento);
        }

        return View("~/Views/Registro/RegistroEventoAdulto.cshtml");
    }

    public decimal CalcularOrcamentoEventoInfantil(Evento evento)
    {
        if (evento == null)
            return 0;

        var CustoPorSalgado = 1.5;
        var CustoPorDoce = 1.0;
        var CustoPorRefri = 2.0;
        var CustoPorCadeira = 5.0;
        var CustoPorMetroQuadrado = 10.0;

        var QuantidadeSalgados = evento.QuantSalgados ?? 0;
        var QuantidadeDoces = evento.QuantDoces ?? 0;
        var QuantidadeRefri = evento.QuantRefri ?? 0;
        var QuantidadeCadeiras = evento.QuantCadeiras ?? 0;
        var QuantidadeMetrosQuadrados = evento.QuantMetrosQuadrados ?? 0;

        var CustoSalgados = CustoPorSalgado * QuantidadeSalgados;
        var CustoDoces = CustoPorDoce * QuantidadeDoces;
        var CustoRefri = CustoPorRefri * QuantidadeRefri;
        var CustoCadeiras = CustoPorCadeira * QuantidadeCadeiras;
        var CustoMetroQuadrado = CustoPorMetroQuadrado * QuantidadeMetrosQuadrados;

        var CustoTotal = CustoSalgados + CustoDoces + CustoRefri + CustoCadeiras + CustoMetroQuadrado;

        var MargemSeguranca = 0.2;
        var OrcamentoFinal = CustoTotal * (1 + MargemSeguranca);

        return (decimal)OrcamentoFinal;
    }

    public async Task<IActionResult> EstatisticasEventoInfantil()
    {
        var EventoId = HttpContext.Session.GetString("EventoId");
        var EventoExistente = await _context.Eventos.FindAsync(int.Parse(EventoId));

        if (EventoExistente == null) return View("~/Views/Home/index.cshtml");

        EventoExistente.MediaSalgadosPorPessoa =
            EventoExistente.QuantSalgados.HasValue && EventoExistente.QuantMaxPessoas.HasValue
                ? (double)EventoExistente.QuantSalgados.Value / EventoExistente.QuantMaxPessoas.Value
                : 0;

        EventoExistente.MediaDocesPorPessoa =
            EventoExistente.QuantDoces.HasValue && EventoExistente.QuantMaxPessoas.HasValue
                ? (double)EventoExistente.QuantDoces.Value / EventoExistente.QuantMaxPessoas.Value
                : 0;

        EventoExistente.MediaRefriPorPessoa =
            EventoExistente.QuantRefri.HasValue && EventoExistente.QuantMaxPessoas.HasValue
                ? (double)EventoExistente.QuantRefri.Value / EventoExistente.QuantMaxPessoas.Value
                : 0;

        EventoExistente.MediaCadeiraPorPessoa =
            EventoExistente.QuantCadeiras.HasValue && EventoExistente.QuantMaxPessoas.HasValue
                ? (double)EventoExistente.QuantCadeiras.Value / EventoExistente.QuantMaxPessoas.Value
                : 0;

        EventoExistente.MediaMetrosPorPessoa = EventoExistente.QuantMetrosQuadrados.HasValue &&
                                               EventoExistente.QuantMaxPessoas.HasValue &&
                                               EventoExistente.QuantMaxPessoas.Value != 0
            ? (double)EventoExistente.QuantMetrosQuadrados.Value / EventoExistente.QuantMaxPessoas.Value
            : 0;

        var margemSeguranca = 0.2;

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

            EventoExistente.QuantidadeDeCopos = Math.Max(EventoExistente.QuantidadeDeCopos.Value,
                coposPorRefri * (1 + margemSeguranca));
        }

        EventoExistente.QuantidadeQuardanapos = EventoExistente.QuantSalgados.HasValue
            ? EventoExistente.QuantSalgados.Value * (1 + margemSeguranca)
            : 0;

        if (EventoExistente.QuantMaxPessoas.HasValue)
        {
            var guardanaposPorPessoa = EventoExistente.QuantMaxPessoas.Value * 12.5;

            EventoExistente.QuantidadeQuardanapos = Math.Max(EventoExistente.QuantidadeQuardanapos.Value,
                guardanaposPorPessoa * (1 + margemSeguranca));
        }

        var TotalAdulto = EventoExistente.QuantMaxPessoas.HasValue
            ? EventoExistente.QuantMaxPessoas - EventoExistente.QuantCriancas
            : 0;

        EventoExistente.PercentualDeCrianças =
            EventoExistente.QuantCriancas.HasValue && EventoExistente.QuantMaxPessoas.HasValue
                ? (double)EventoExistente.QuantCriancas / (double)EventoExistente.QuantMaxPessoas * 100
                : 0;

        EventoExistente.PercentualDeAdultos = TotalAdulto.HasValue && EventoExistente.QuantMaxPessoas.HasValue
            ? TotalAdulto / EventoExistente.QuantMaxPessoas * 100
            : 0;

        // EventoExistente.PercentualDeCrianças = (EventoExistente.QuantCriancas / EventoExistente.QuantMaxPessoas) * 100;

        // EventoExistente.PercentualDeAdultos = (EventoExistente.QuantMaxPessoas - EventoExistente.QuantCriancas) / EventoExistente.QuantMaxPessoas * 100;

        EventoExistente.Orcamento = CalcularOrcamentoEventoInfantil(EventoExistente);

        return View("~/Views/Detalhes/EstatisticasEventoInfantil.cshtml", EventoExistente);
    }

    public async Task<IActionResult> EstatisticasEventoAdulto()
    {
        var EventoId = HttpContext.Session.GetString("EventoId");
        var EventoExistente = await _context.Eventos.FindAsync(int.Parse(EventoId));

        if (EventoExistente == null) return View("~/Views/Home/index.cshtml", EventoExistente);

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

        if (EventoExistente.QuantMetrosQuadrados.HasValue && EventoExistente.QuantMaxPessoas.HasValue &&
            EventoExistente.QuantMaxPessoas.Value != 0)
        {
            var Media = (double)EventoExistente.QuantMetrosQuadrados.Value / EventoExistente.QuantMaxPessoas.Value;
            EventoExistente.MediaMetrosPorPessoa = Media;
        }
        else
        {
            EventoExistente.MediaMetrosPorPessoa = 0;
        }

        var margemSeguranca = 0.2;

        if (EventoExistente.QuantMaxPessoas.HasValue)
        {
            EventoExistente.QuantidadeDeCopos = EventoExistente.QuantMaxPessoas.Value * 4 * (1 + margemSeguranca);

            EventoExistente.QuantidadeDePratos = EventoExistente.QuantMaxPessoas.Value * (1 + margemSeguranca);

            EventoExistente.QuantidadeTalheres =
                (int)Math.Round(EventoExistente.QuantMaxPessoas.Value * (1 + margemSeguranca));
        }

        if (EventoExistente.QuantRefri.HasValue || EventoExistente.QuantAlcool.HasValue)
        {
            double coposPorRefri = EventoExistente.QuantRefri.GetValueOrDefault(0) * 2 * 10;
            double coposPorAlcool = EventoExistente.QuantAlcool.GetValueOrDefault(0) * 2 * 10;

            var coposNecessarios = Math.Max(coposPorRefri, coposPorAlcool);

            EventoExistente.QuantidadeDeCopos = Math.Max(EventoExistente.QuantidadeDeCopos.GetValueOrDefault(0),
                coposNecessarios * (1 + margemSeguranca));
        }

        if (EventoExistente.QuantSalgados.HasValue)
            EventoExistente.QuantidadeQuardanapos = EventoExistente.QuantSalgados.Value * (1 + margemSeguranca);

        var guardanaposPorPessoa = EventoExistente.QuantMaxPessoas.Value * 12.5;

        EventoExistente.QuantidadeQuardanapos = Math.Max(EventoExistente.QuantidadeQuardanapos.Value,
            guardanaposPorPessoa * (1 + margemSeguranca));

        var TotalAdulto = EventoExistente.QuantMaxPessoas - EventoExistente.QuantCriancas;

        EventoExistente.PercentualDeCrianças =
            (double)EventoExistente.QuantCriancas / (double)EventoExistente.QuantMaxPessoas * 100;

        EventoExistente.PercentualDeAdultos = TotalAdulto / EventoExistente.QuantMaxPessoas * 100;

        return View("~/Views/Detalhes/EstatisticasEventoInfantil.cshtml", EventoExistente);
    }

    public ActionResult DisplayImage(int id)
    {
        var evento = _context.Eventos.FirstOrDefault(m => m.ID == id);
        if (evento == null || evento.FotoDoEvento == null) return NotFound();

        return File(evento.FotoDoEvento, "image/jpeg");
    }

    public async Task<IActionResult> EditarParaView(int? id)
    {
        if (id == null || _context.Eventos == null) return NotFound();

        var evento = await _context.Eventos.FindAsync(id);
        if (evento == null) return NotFound();
        return View("~/Views/Registro/AtualizarEvento.cshtml", evento);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        [Bind(
            "ID,NomeEvento,Descricao,DataInicio,DataFinal,QuantMetrosQuadrados,QuantMaxPessoas,QuantRefri,QuantCarne,QuantDoces,QuantSalgados,QuantCadeiras,CEP,Estado,Cidade,Bairro,FotoDoEvento,TipoEvento,Organizador,UsuarioPremiumID")]
        Evento EventoNovo)
    {
        var EventoId = HttpContext.Session.GetString("EventoId");
        var UsuarioIdString = HttpContext.Session.GetString("UserId");
        var IDUsuario = int.Parse(UsuarioIdString);

        if (EventoId == null) return View("~/Views/Home/Index.cshtml");

        var EventoExistente = await _context.Eventos.FindAsync(int.Parse(EventoId));
        if (EventoExistente == null) return View("~/Views/Registro/RegistroEvento.cshtml", EventoNovo);

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

                if (EventoNovo.FotoDoEvento != null) EventoExistente.FotoDoEvento = EventoNovo.FotoDoEvento;

                var UsuarioPremium = await _contextPremium.UsuariosPremium.FindAsync(IDUsuario);

                var EmailDeEnvio = new Email("smtp.gmail.com", "contatoeventplanner@gmail.com", "ipmbosrsubhctwkk");
                var ListaEmail = new List<string> { UsuarioPremium.Email };
                var Assunto = "Evento Atualizado";

                var Corpo = UsuarioPremium.Nome + ", seu evento " + EventoExistente.NomeEvento + " de código " +
                            EventoExistente.Identificador + " foi atualizado com sucesso!";

                EmailDeEnvio.SendEmail(ListaEmail, Assunto, Corpo);

                _context.Update(EventoExistente);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventoExists(EventoExistente.ID))
                    return NotFound();

                throw;
            }

            return View("~/Views/Registro/AtualizarEvento.cshtml", EventoExistente);
        }

        return View("~/Views/Registro/AtualizarEvento.cshtml", EventoNovo);
    }

    public async Task<IActionResult> EnviarEmailEventoExcluido(Evento eventoObjt, Usuario? usuario1, UsuarioPremium? usuario2)
    {
        var emailDeEnvio = new Email("smtp.gmail.com", "contatoeventplanner@gmail.com", "ipmbosrsubhctwkk");
        var listaEmail = new List<string>();

        if (usuario1 != null)
        {
            listaEmail.Add(usuario1.Email);
        }

        if (usuario2 != null)
        {
            listaEmail.Add(usuario2.Email);
        }

        var assunto = "Evento excluido";
        var corpo = "Seu evento " + eventoObjt.NomeEvento + " de código " + eventoObjt.Identificador + " foi excluído com sucesso";

        emailDeEnvio.SendEmail(listaEmail, assunto, corpo);

        return Ok();
    }

    public async Task<IActionResult> GetUsuarioPorId()
    {
        var UsuarioNormalIdString = HttpContext.Session.GetString("UsuarioID");
        var UsuarioPremiumIdString = HttpContext.Session.GetString("UsuarioPremiumID");

        if (UsuarioNormalIdString != null)
        {
            var UsuarioNormalId = int.Parse(UsuarioNormalIdString);
            var UsuarioNormal = await _contextNormal.Usuarios.FindAsync(UsuarioNormalId);

            if (UsuarioNormal != null) return Ok(UsuarioNormal);
        }
        else if (UsuarioPremiumIdString != null)
        {
            var UsuarioPremiumId = int.Parse(UsuarioPremiumIdString);
            var UsuarioPremium = await _contextPremium.UsuariosPremium.FindAsync(UsuarioPremiumId);

            if (UsuarioPremium != null) return Ok(UsuarioPremium);
        }

        return NotFound();
    }

    public async Task<IActionResult> ExcluirEventoUsuario(Evento EventoObjt, Usuario UsuarioObjt)
    {
        UsuarioObjt.eventoID = null;
        _contextNormal.Update(UsuarioObjt);
        await _contextPremium.SaveChangesAsync();

        _context.Eventos.Remove(EventoObjt);
        await _context.SaveChangesAsync();

        await EnviarEmailEventoExcluido(EventoObjt, UsuarioObjt, null);

        return View("~/Views/Home/MenuEvento.cshtml");
    }

    public async Task<IActionResult> ExcluirEventoUsuarioPremium(Evento EventoObjt, UsuarioPremium UsuarioPremiumObjt)
    {
        UsuarioPremiumObjt.eventoID = null;
        _contextPremium.Update(UsuarioPremiumObjt);
        await _contextPremium.SaveChangesAsync();

        _context.Eventos.Remove(EventoObjt);
        await _context.SaveChangesAsync();

        EnviarEmailEventoExcluido(EventoObjt, null, UsuarioPremiumObjt);

        return View("~/Views/Home/MenuEvento.cshtml");
    }

    public async Task EnvioEmaila(Evento NovoEvento)
    {
        HttpContext.Session.SetString("EventoId", NovoEvento.ID.ToString());

        await AtualizarChavesUsuario(NovoEvento);

        var UsuarioRetornado = await GetUsuarioPorId();
        var ResultadoOk = UsuarioRetornado as OkObjectResult;

        var UsuarioEncontrado = ResultadoOk.Value as Usuario;
        var UsuarioPremiumEncontrado = ResultadoOk.Value as UsuarioPremium;

        if (UsuarioEncontrado != null)
            await EnviarEmailEventoCriado(UsuarioEncontrado, NovoEvento);
        else if (UsuarioPremiumEncontrado != null)
            await EnviarEmailEventoCriadoPremium(UsuarioPremiumEncontrado, NovoEvento);
    }

    public async Task<IActionResult> Delete(int? ID)
    {
        if (ID == null || _context.Eventos == null) return NotFound();

        var Evento = await _context.Eventos.FirstOrDefaultAsync(m => m.ID == ID);
        if (Evento == null) return NotFound();

        var EventoId = HttpContext.Session.GetString("EventoId");
        var EventoExistente = await _context.Eventos.FindAsync(int.Parse(EventoId));

        if (EventoExistente == null) return Problem("Entity set 'EventosBD.Eventos' is null.");

        var usuario = await GetUsuarioPorId();
        var ResultadoOk = usuario as OkObjectResult;

        var UsuarioNormalEncontrado = ResultadoOk.Value as Usuario;
        var UsuarioPremiumEncontrado = ResultadoOk.Value as UsuarioPremium;
        Console.Write("");

        if (UsuarioNormalEncontrado is Usuario)
        {
            await ExcluirEventoUsuario(EventoExistente, (Usuario)UsuarioNormalEncontrado);
        }
        else if (UsuarioPremiumEncontrado is UsuarioPremium)
        {
            await ExcluirEventoUsuarioPremium(EventoExistente, (UsuarioPremium)UsuarioPremiumEncontrado);
        }
        // else
        // {
        //    // Lidar com o caso em que o usuário não é de nenhum dos tipos esperados
        //    // Por exemplo, retornar um erro ou redirecionar para uma página de erro
        //    return BadRequest("Usuário não reconhecido");
        // }

        return View("~/Views/Home/MenuEvento.cshtml");
    }

    public async Task<IActionResult> EventosPorPeriodo(DateTime DataInicio, DateTime DataFinal)
    {
        var Eventos = await _context.Eventos.ToListAsync();
        var EventosPorPeriodo = new List<Evento>();
        foreach (var evento in Eventos)
            if (evento.DataInicio >= DataInicio &&
                evento.DataFinal <= DataFinal)
                EventosPorPeriodo.Add(evento);
        if (EventosPorPeriodo.Count == 0)
            return NotFound("Não existe nenhum evento nesse período!");
        return View("~/Views/Detalhes/EventosPorPeriodo.cshtml", EventosPorPeriodo);
    }
  
    public async Task<IActionResult> EventosPorTipo(string TipoEvento)
    {
        var Eventos = await _context.Eventos.ToListAsync();
        var EventosPorTipo = new List<Evento>();
        foreach (var evento in Eventos)
            if (evento.TipoEvento == TipoEvento)
                EventosPorTipo.Add(evento);
        if (EventosPorTipo.Count == 0)
            return NotFound("Não existe nenhum evento desse tipo!");
        return View("~/Views/Detalhes/EventosPorTipo.cshtml", EventosPorTipo);
    }

    public async Task<IActionResult> EventosPorData(DateTime DataInicio)
    {
        var Eventos = await _context.Eventos.ToListAsync();
        var EventosPorData = new List<Evento>();
        foreach (var evento in Eventos)
            if (evento.DataInicio.Date == DataInicio.Date)
                EventosPorData.Add(evento);
        if (EventosPorData.Count == 0)
            return NotFound("Não existe nenhum evento nessa data!");
        return View("~/Views/Detalhes/FiltroEvento.cshtml", EventosPorData);
    }


    //faca um metodo que busque eventos de uma data e horario especifico
    public async Task<IActionResult> EventosPorDataEHorario(DateTime DataInicio, DateTime DataFinal)
    {
        var Eventos = await _context.Eventos.ToListAsync();
        var EventosPorDataEHorario = new List<Evento>();
        foreach (var evento in Eventos)
            if (evento.DataInicio == DataInicio &&
                evento.DataFinal == DataFinal)
                EventosPorDataEHorario.Add(evento);
        if (EventosPorDataEHorario.Count == 0)
            return NotFound("Não existe nenhum evento nessa data e horário!");
        return View("~/Views/Detalhes/EventosPorDataEHorario.cshtml", EventosPorDataEHorario);
    }

    public async Task<IActionResult> EventosComMesmaData(int id)
    {
        var EventoResgatado = await _context.Eventos.FindAsync(id);
        if (EventoResgatado == null) return NotFound();
        var EventosComMesmaData = new List<Evento>();
        var Eventos = await _context.Eventos.ToListAsync();

        foreach (var evento in Eventos)
            if (evento.DataInicio == EventoResgatado.DataInicio &&
                evento.DataFinal == EventoResgatado.DataFinal)
                EventosComMesmaData.Add(evento);

        if (EventosComMesmaData.Count == 0)
            return NotFound("Não existe nenhum evento com a mesma data!");

        return View("~/Views/Detalhes/EventosComMesmaData.cshtml", EventosComMesmaData);
    }

    public async Task<IActionResult> EventosComMesmaDataEHorario(int id)
    {
        var EventoResgatado = await _context.Eventos.FindAsync(id);
        if (EventoResgatado == null) return NotFound();
        var EventosComMesmaDataEHorario = new List<Evento>();
        var Eventos = await _context.Eventos.ToListAsync();

        foreach (var evento in Eventos)
            if (evento.DataInicio == EventoResgatado.DataInicio &&
                evento.DataFinal == EventoResgatado.DataFinal &&
                evento.DataInicio == EventoResgatado.DataInicio &&
                evento.DataFinal == EventoResgatado.DataFinal)
                EventosComMesmaDataEHorario.Add(evento);

        if (EventosComMesmaDataEHorario.Count == 0)
            return NotFound("Não existe nenhum evento com a mesma data e horário!");

        return View("~/Views/Detalhes/DetalhesEventoAchado.cshtml" /*EventosComMesmaDataEHorario*/);
    }

    [HttpPost]
    public async Task<ActionResult> AdicionarComentario(int id, string comentario)
    {
        var EventoResgatado = await _context.Eventos.FindAsync(id);
        if (EventoResgatado == null) return NotFound();

        var UsuarioIdString = HttpContext.Session.GetString("UserId");
        var IDusuario = int.Parse(UsuarioIdString);

        var UsuarioResgatado = await _contextPremium.UsuariosPremium.FindAsync(IDusuario);

        var avaliacao = new Avaliacao();
        avaliacao.Comentario = comentario;
        avaliacao.DataComentario = DateOnly.FromDateTime(DateTime.Now);
        avaliacao.UsuarioPremiumID = UsuarioResgatado.ID;
        avaliacao.EventoID = EventoResgatado.ID;
        avaliacao.UsuarioID = 0;

        _contextAvaliacao.Add(avaliacao);
        await _contextAvaliacao.SaveChangesAsync();

        EventoResgatado.Avaliacoes =
            await _contextAvaliacao.Avaliacao.Where(a => a.EventoID == EventoResgatado.ID).ToListAsync();

        return View("~/Views/Detalhes/DetalhesEventoAchado.cshtml", EventoResgatado);
    }

    public async Task<IActionResult> EventoEncontrado(string nomeEvento)
    {
        Evento eventoResgatado = null;

        if (!string.IsNullOrEmpty(nomeEvento))
        {
            eventoResgatado = await _context.Eventos
                .FirstOrDefaultAsync(e => e.NomeEvento.ToLower() == nomeEvento.ToLower());
        }

        if (eventoResgatado == null)
        {
            ModelState.AddModelError("Buscador", "Evento não registrado");
            return View("~/Views/Home/MenuEvento.cshtml");
        }

        eventoResgatado.Avaliacoes =
            await _contextAvaliacao.Avaliacao.Where(a => a.EventoID == eventoResgatado.ID).ToListAsync();

        return View("~/Views/Detalhes/DetalhesEventoAchado.cshtml", eventoResgatado);
    }


    //[HttpGet("GetEventos")]
    //public async Task<IActionResult> GetEventos(string term)
    //{
    //    var eventos = await _context.Eventos.Where(e => e.NomeEvento.ToLower().Contains(term.ToLower())).ToListAsync();
    //    return Json(eventos);
    //}

    [HttpGet("GetEventos")]
    public async Task<IActionResult> GetEventos(string term)
    {
        if (string.IsNullOrWhiteSpace(term))
        {
            return Json(new List<Evento>());
        }

        var eventos = await _context.Eventos
            .Where(e => e.NomeEvento.ToLower().Contains(term.ToLower()))
            .ToListAsync();
        return Json(eventos);
    }

    bool EventoExists(int id) => (_context.Eventos?.Any(e => e.ID == id)).GetValueOrDefault();
}