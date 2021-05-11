using Agenda.Desktop.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Agenda.Desktop.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Evento> Eventos { get; set; }
        public DbSet<ParticipantesEmEventos> ParticipantesEmEventos { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Evento>().HasKey(t => t.Id);
            builder.Entity<ParticipantesEmEventos>().HasKey(t => t.Id);
        }
    }
}
