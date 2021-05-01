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

            var eventosDoUsuario = context.Eventos.
                Where(e => e.AppIdentityUserId == usuario.Id.ToString()).AsEnumerable();

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
            var usuario = await userManager.GetUserAsync(this.User);
            if (ModelState.IsValid)
            {
                var ultimoRegistro = context.Eventos.AsNoTracking().AsEnumerable();
                int idUltimoRegistro = 1;
                if (ultimoRegistro.Count() > 0)
                    idUltimoRegistro = ultimoRegistro.Last().Id + 1;

                if (evento.ListaDeParticipantes != null)
                {
                    foreach (var participante in evento.ListaDeParticipantes)
                    {
                        //context.ParticipantesEmEventos.Add(new Participante
                        //{ EventoId = evento.Id, IdParticipante = participante.IdParticipante });
                    }
                    evento.Participantes = evento.ListaDeParticipantes.Count();
                }

                evento.Participantes += 1;
                evento.AppIdentityUserId = usuario.Id;
                evento.DataCriado = DateTime.Now;
                evento.DataAlterado = DateTime.Now;
                context.Eventos.Add(evento);
                //context.ParticipantesEmEventos.Add(new Participante
                //{ EventoId = idUltimoRegistro, IdParticipante = usuario.Id });
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
                var dbContextParticipantesEmEventos = context.ParticipantesEmEventos;

                if (evento.ListaDeParticipantes != null)
                {
                    foreach (var participante in evento.ListaDeParticipantes)
                    {
                        if (antigoEvento.ListaDeParticipantes.Contains(participante))
                            dbContextParticipantesEmEventos.Update(participante);
                        else
                            dbContextParticipantesEmEventos.Remove(participante);
                    }
                    evento.Participantes = evento.ListaDeParticipantes.Count();
                }

                evento.Participantes += 1;
                evento.AppIdentityUserId = usuario.Id;
                evento.DataCriado = antigoEvento.DataCriado;
                evento.DataAlterado = DateTime.Now;
                context.Eventos.Update(evento);
                context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(evento);
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

            var participantesEmEventos = context.ParticipantesEmEventos.
                Where(x => x.EventoId == evento.Id);

            foreach (var participante in participantesEmEventos)
            {
                context.ParticipantesEmEventos.Remove(participante);
            }

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
