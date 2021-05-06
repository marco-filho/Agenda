using Agenda.Desktop.Areas.Identity.Data;
using Agenda.Desktop.Data;
using Agenda.Desktop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Agenda.Desktop.Controllers
{
    public class EventosController : Controller
    {
        private readonly ILogger<EventosController> logger;
        private readonly ApplicationDbContext context;
        private readonly UserManager<AppIdentityUser> userManager;

        public EventosController(ILogger<EventosController> logger,
            ApplicationDbContext context, UserManager<AppIdentityUser> userManager)
        {
            this.logger = logger;
            this.context = context;
            this.userManager = userManager;
        }

        [Authorize]
        public async Task<IActionResult> IndexAsync()
        {
            var usuario = await userManager.GetUserAsync(this.User);

            //var eventosDoUsuario = context.Eventos.First(e => e.AppIdentityUserId == usuario.Id);
            //var test = context.Participantes.First(p => p.EventoId == 1).AsEnumerable();
            var eventosDoUsuario = context.Eventos
                .Where(e => e.AppIdentityUserId == usuario.Id).AsEnumerable();
            //var test = Enumerable.Empty<Participante>();
            //foreach (var evento in eventosDoUsuario)
            //    //evento.ListaDeParticipantes =
            //    test = context.Participantes.Where(p => p.EventoId == evento.Id).AsEnumerable();

            return View(eventosDoUsuario);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IndexAsync(Evento evento)
        {
            var usuario = await userManager.GetUserAsync(this.User);

            var eventosDoUsuario = context.Eventos.
                Where(e => e.AppIdentityUserId == usuario.Id);

            var retornoPesquisa = eventosDoUsuario.
                Where(e => e.DataInicial <= evento.DataInicial && evento.DataInicial <= e.DataFinal ||
                            e.DataInicial <= evento.DataFinal && evento.DataFinal <= e.DataFinal).AsEnumerable();

            return View("Index", retornoPesquisa);
        }

        [Authorize]
        public IActionResult NewEvento()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewEventoAsync(Evento evento)
        {
            if (ModelState.IsValid)
            {
                var usuario = await userManager.GetUserAsync(this.User);
                if (evento.ListaDeParticipantes == null)
                    evento.ListaDeParticipantes = Enumerable.Empty<Participante>();

                evento.ListaDeParticipantes = evento.ListaDeParticipantes
                    .Prepend(new Participante { Username = usuario.UserName, Nome = usuario.Nome })
                    .ToList();

                foreach (var participante in evento.ListaDeParticipantes)
                {
                    var usuarioParticipante = userManager.Users
                        .FirstOrDefault(u => u.UserName == participante.Username);
                    if (usuarioParticipante != null)
                        participante.Nome = usuarioParticipante.Nome;
                    else
                        return BadRequest();
                }

                evento.Participantes = evento.ListaDeParticipantes.Count();
                evento.AppIdentityUserId = usuario.Id;
                evento.DataCriado = evento.DataAlterado = DateTime.Now;
                context.Eventos.Add(evento);
                context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(evento);
        }

        [Authorize]
        public IActionResult UpdateEvento(int? Id)
        {
            if (Id == null || Id == 0)
                return NotFound();

            var query = context.Eventos.Find(Id);

            if (query == null)
                return NotFound();

            return View(query);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateEventoAsync(Evento evento)
        {
            if (ModelState.IsValid)
            {
                var usuario = await userManager.GetUserAsync(this.User);
                var antigoEvento = context.Eventos.AsNoTracking().SingleOrDefault
                    (e => e.Id == evento.Id);
                //var dbContextParticipantesEmEventos = context.ParticipantesEmEventos;

                //if (evento.ListaDeParticipantes != null)
                //{
                //    foreach (var participante in evento.ListaDeParticipantes)
                //    {
                //        if (antigoEvento.ListaDeParticipantes.Contains(participante))
                //            dbContextParticipantesEmEventos.Update(participante);
                //        else
                //            dbContextParticipantesEmEventos.Remove(participante);
                //    }
                //    evento.Participantes = evento.ListaDeParticipantes.Count();
                //}

                evento.Participantes += 1;
                evento.AppIdentityUserId = usuario.Id;
                //evento.DataCriado = antigoEvento.DataCriado; really necessary?
                evento.DataAlterado = DateTime.Now;
                context.Eventos.Update(evento);
                context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(evento);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<List<Participante>> SearchUsersAsync([FromBody] string entrada)
        {
            if (entrada == null || entrada == "")
                return null;

            var usuario = await userManager.GetUserAsync(this.User);
            var query = userManager.Users
                .Where(p => (p.UserName.Contains(entrada) || p.Nome.Contains(entrada))
                && p.UserName != usuario.UserName)
                .OrderByDescending(p => p.Nome)
                .Take(10).ToList();
            query.TrimExcess();

            var match = new List<Participante>();

            foreach (var item in query)
                match.Add(
                    new Participante()
                    {
                        Username = item.UserName,
                        Nome = item.Nome
                    });

            return match;
        }

        [Authorize]
        public IActionResult DeleteEvento(int? Id)
        {
            if (Id == null || Id == 0)
                return NotFound();

            var query = context.Eventos.Find(Id);

            if (query == null)
                return NotFound();

            return View(query);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteEventoPost(Evento evento)
        {
            if (!ModelState.IsValid)
                return ValidationProblem();

            context.Eventos.Remove(evento);

            //var participantesEmEventos = context.Participantes.
            //    Where(x => x.EventoId == evento.Id);

            //foreach (var participante in participantesEmEventos)
            //{
            //    context.Participantes.Remove(participante);
            //}

            context.SaveChanges();

            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
