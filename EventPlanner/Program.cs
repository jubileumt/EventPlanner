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

            // Adicione serviços ao contêiner.
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
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Defina o tempo limite da sessão conforme necessário.
                options.Cookie.HttpOnly = true; // Define o cookie como HttpOnly para segurança
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
                // Rota "verificarCliente" para ação "VerificarCliente" no controlador "Cliente"
                endpoints.MapControllerRoute(
                    name: "verificarCliente",
                    pattern: "cliente/verificar",
                    defaults: new { controller = "Cliente", action = "VerificarCliente" }
                );

                // Rota "registrarUsuario" para ação "RegistrarUsuario" no controlador "Cliente"
                endpoints.MapControllerRoute(
                    name: "registrarUsuario",
                    pattern: "cliente/registrar",
                    defaults: new { controller = "Cliente", action = "RegistroUsuario" }
                );

                // Rota padrão (mantenha-a após a configuração das rotas personalizadas)
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