using EventPlanner.Models;
using Microsoft.EntityFrameworkCore;


namespace EventPlanner.Data
{
    public class AvaliacaoBD : DbContext
    {
        public AvaliacaoBD(DbContextOptions<AvaliacaoBD> options)
            : base(options)
        { }

        public DbSet<Avaliacao> Avaliacao { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Avaliacao>()
                .HasIndex(a => a.UsuarioPremiumID)
                .IsUnique(false);
        }
    }
}