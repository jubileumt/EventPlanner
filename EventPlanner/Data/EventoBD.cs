using EventPlanner.Models;
using Microsoft.EntityFrameworkCore;


namespace EventPlanner.Data
{
    public class EventosBD : DbContext
    {
        public EventosBD(DbContextOptions<EventosBD> options)
           : base(options)
        { }

        public DbSet<Evento> Eventos { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Você pode configurar o relacionamento entre Evento e Avaliacao aqui, se necessário
        }
    }
}