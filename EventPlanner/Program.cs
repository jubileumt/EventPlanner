using Microsoft.AspNetCore.Localization;
using EventPlanner.Data;
using ViaCepConsumer;
using Microsoft.EntityFrameworkCore;

namespace TesteMVC2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Adicione servi�os ao cont�iner.
            builder.Services.AddControllersWithViews();

            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture("pt-BR");
            });

            builder.Services.AddScoped<ViaCepClient>();

            string connectionString = "Server=tcp:prototipo.database.windows.net,1433;Initial Catalog=teste;Persist Security Info=False;User ID=henrique;Password=pedro311268@;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

            builder.Services.AddDbContext<UsuarioBD>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            builder.Services.AddDbContext<UsuarioPremiumBD>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            builder.Services.AddDbContext<EventosBD>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            builder.Services.AddDbContext<AvaliacaoBD>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            builder.Services.AddDistributedMemoryCache();

            builder.Services.AddAuthorization();

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Defina o tempo limite da sess�o conforme necess�rio.
                options.Cookie.HttpOnly = true; // Define o cookie como HttpOnly para seguran�a
                options.Cookie.IsEssential = true;
            });

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseSession();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                // Rota "verificarCliente" para a��o "VerificarCliente" no controlador "Cliente"
                endpoints.MapControllerRoute(
                    name: "verificarCliente",
                    pattern: "cliente/verificar",
                    defaults: new { controller = "Cliente", action = "VerificarCliente" }
                );

                // Rota "registrarUsuario" para a��o "RegistrarUsuario" no controlador "Cliente"
                endpoints.MapControllerRoute(
                    name: "registrarUsuario",
                    pattern: "cliente/registrar",
                    defaults: new { controller = "Cliente", action = "RegistroUsuario" }
                );

                // Rota padr�o (mantenha-a ap�s a configura��o das rotas personalizadas)
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}"
                );

                endpoints.MapControllerRoute(
                   name: "registrarUsuario",
                   pattern: "cliente/registrar",
                   defaults: new { controller = "Cliente", action = "RegistroUsuario" }
               );



                app.Run();
            });
        }
    }
}