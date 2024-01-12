using EventPlanner.Models;
using Microsoft.EntityFrameworkCore;


namespace EventPlanner.Data
{
    public class UsuarioPremiumBD : DbContext
    {
        public UsuarioPremiumBD(DbContextOptions<UsuarioPremiumBD> options)
            : base(options)
        { }

        public DbSet<UsuarioPremium> UsuariosPremium { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configurar um índice único para o CPF
            modelBuilder.Entity<UsuarioPremium>()
                .HasIndex(up => up.CPF)
                .IsUnique();

            // Configurar um índice único para o Email
            modelBuilder.Entity<UsuarioPremium>()
                .HasIndex(up => up.Email)
                .IsUnique();
        }
    }
}