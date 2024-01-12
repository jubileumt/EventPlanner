using EventPlanner.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace EventPlanner.Data
{
    public class UsuarioBD : DbContext
    {
        public UsuarioBD(DbContextOptions<UsuarioBD> options)
            : base(options)
        { }

        public DbSet<Usuario> Usuarios { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configurar um índice único para o CPF
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.CPF)
                .IsUnique();

            // Configurar um índice único para o Email
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }

    }
}
