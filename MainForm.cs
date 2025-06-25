using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using CafeteriaUNAL.Forms;

namespace CafeteriaUNAL
{
    public partial class MainForm : Form
    {
        private readonly IServiceProvider _serviceProvider;

        public MainForm()
        {
            InitializeComponent();
            _serviceProvider = Program.ServiceProvider;
            ConfigurarFormulario();
        }

        public MainForm(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _serviceProvider = serviceProvider;
            ConfigurarFormulario();
        }

        private void ConfigurarFormulario()
        {
            // Configurar el formulario principal
            this.Text = "Sistema de Cafetería - Universidad Nacional de Colombia";
            this.Size = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.IsMdiContainer = true;

            // Crear el menú
            CrearMenu();

            // Crear la barra de estado
            CrearBarraEstado();

            // Configurar el color de fondo del MDI
            foreach (Control control in this.Controls)
            {
                if (control is MdiClient)
                {
                    control.BackColor = Color.FromArgb(240, 240, 240);
                }
            }
        }

        private void CrearMenu()
        {
            var menuStrip = new MenuStrip();

            // Menú Archivo
            var menuArchivo = new ToolStripMenuItem("&Archivo");
            menuArchivo.DropDownItems.Add("Cerrar Sesión", null, (s, e) => MessageBox.Show("Cerrar sesión - Por implementar"));
            menuArchivo.DropDownItems.Add(new ToolStripSeparator());
            menuArchivo.DropDownItems.Add("Salir", null, (s, e) => Application.Exit());

            // Menú Gestión
            var menuGestion = new ToolStripMenuItem("&Gestión");

            var menuUsuarios = new ToolStripMenuItem("Usuarios", null, MenuUsuarios_Click);
            menuUsuarios.ShortcutKeys = Keys.Control | Keys.U;
            menuGestion.DropDownItems.Add(menuUsuarios);

            var menuProductos = new ToolStripMenuItem("Productos", null, MenuProductos_Click);
            menuProductos.ShortcutKeys = Keys.Control | Keys.P;
            menuGestion.DropDownItems.Add(menuProductos);

            menuGestion.DropDownItems.Add(new ToolStripSeparator());
            menuGestion.DropDownItems.Add("Fichos del Día", null, (s, e) => MessageBox.Show("Gestión de Fichos - Por implementar"));

            // Menú Ventas
            var menuVentas = new ToolStripMenuItem("&Ventas");

            var menuNuevaVenta = new ToolStripMenuItem("Nueva Venta", null, MenuNuevaVenta_Click);
            menuNuevaVenta.ShortcutKeys = Keys.Control | Keys.N;
            menuVentas.DropDownItems.Add(menuNuevaVenta);

            menuVentas.DropDownItems.Add("Historial de Ventas", null, (s, e) => MessageBox.Show("Historial - Por implementar"));

            // Menú Reportes
            var menuReportes = new ToolStripMenuItem("&Reportes");
            menuReportes.DropDownItems.Add("Ventas del Día", null, (s, e) => MessageBox.Show("Reporte del Día - Por implementar"));
            menuReportes.DropDownItems.Add("Subsidios", null, (s, e) => MessageBox.Show("Reporte de Subsidios - Por implementar"));
            menuReportes.DropDownItems.Add("Productos más Vendidos", null, (s, e) => MessageBox.Show("Productos más Vendidos - Por implementar"));

            // Menú Ayuda
            var menuAyuda = new ToolStripMenuItem("A&yuda");
            menuAyuda.DropDownItems.Add("Acerca de", null, (s, e) =>
                MessageBox.Show("Sistema de Cafetería UNAL v1.0\n\nUniversidad Nacional de Colombia\nSede La Paz",
                "Acerca de", MessageBoxButtons.OK, MessageBoxIcon.Information));

            // Agregar menús al MenuStrip
            menuStrip.Items.Add(menuArchivo);
            menuStrip.Items.Add(menuGestion);
            menuStrip.Items.Add(menuVentas);
            menuStrip.Items.Add(menuReportes);
            menuStrip.Items.Add(menuAyuda);

            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);
        }

        private void CrearBarraEstado()
        {
            var statusStrip = new StatusStrip();

            var lblUsuario = new ToolStripStatusLabel("Usuario: Admin");
            var lblFecha = new ToolStripStatusLabel($"Fecha: {DateTime.Now:dd/MM/yyyy}");
            var lblHora = new ToolStripStatusLabel($"Hora: {DateTime.Now:HH:mm}");

            statusStrip.Items.Add(lblUsuario);
            statusStrip.Items.Add(new ToolStripStatusLabel() { Spring = true }); // Espaciador
            statusStrip.Items.Add(lblFecha);
            statusStrip.Items.Add(lblHora);

            // Timer para actualizar la hora
            var timer = new System.Windows.Forms.Timer();
            timer.Interval = 1000;
            timer.Tick += (s, e) => lblHora.Text = $"Hora: {DateTime.Now:HH:mm:ss}";
            timer.Start();

            this.Controls.Add(statusStrip);
        }

        private void MenuUsuarios_Click(object? sender, EventArgs e)
        {
            var formUsuarios = _serviceProvider.GetRequiredService<FormUsuarios>();
            formUsuarios.MdiParent = this;
            formUsuarios.Show();
        }

        private void MenuProductos_Click(object? sender, EventArgs e)
        {
            var formProductos = _serviceProvider.GetRequiredService<FormProductos>();
            formProductos.MdiParent = this;
            formProductos.Show();
        }

        private void MenuNuevaVenta_Click(object? sender, EventArgs e)
        {
            var formNuevaVenta = _serviceProvider.GetRequiredService<FormNuevaVenta>();
            formNuevaVenta.MdiParent = this;
            formNuevaVenta.Show();
        }
    }
}