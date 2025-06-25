using CafeteriaUNAL.Data;
using CafeteriaUNAL.Forms;
using CafeteriaUNAL.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CafeteriaUNAL
{
    internal static class Program
    {
        public static IServiceProvider ServiceProvider { get; private set; } = null!;
        public static IConfiguration Configuration { get; private set; } = null!;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            // Cargar configuración desde appsettings.json
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();

            // Configurar servicios (Dependency Injection)
            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();

            // Aplicar migraciones y crear base de datos si no existe
            AplicarMigraciones(ServiceProvider);

            // Configurar cultura para Colombia
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("es-CO");
            System.Threading.Thread.CurrentThread.CurrentCulture = cultura;
            System.Threading.Thread.CurrentThread.CurrentUICulture = cultura;

            // Iniciar la aplicación con el formulario principal
            Application.Run(ServiceProvider.GetRequiredService<MainForm>());
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // Registrar la configuración
            services.AddSingleton<IConfiguration>(Configuration);

            // Registrar DbContext
            services.AddDbContext<CafeteriaContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));

            // Registrar servicios
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<IProductoService, ProductoService>();
            services.AddScoped<ITransaccionService, TransaccionService>();
            // TODO: Registrar otros servicios cuando los creemos
            // services.AddScoped<IFichoService, FichoService>();

            // Registrar formularios
            services.AddTransient<MainForm>();
            services.AddTransient<FormUsuarios>();
            services.AddTransient<FormEditarUsuario>();
            services.AddTransient<FormProductos>();
            services.AddTransient<FormEditarProducto>();
            services.AddTransient<FormNuevaVenta>();
            // TODO: Registrar otros formularios cuando los creemos
        }

        private static void AplicarMigraciones(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<CafeteriaContext>();
                try
                {
                    context.Database.EnsureCreated(); // Crea la BD si no existe
                    // O si prefieres usar migraciones:
                    // context.Database.Migrate();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al crear/actualizar la base de datos: {ex.Message}",
                        "Error de Base de Datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}