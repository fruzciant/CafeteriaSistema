using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using CafeteriaUNAL.Models;
using CafeteriaUNAL.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CafeteriaUNAL.Forms
{
    public partial class FormUsuarios : Form
    {
        private readonly IUsuarioService _usuarioService;
        private DataGridView dgvUsuarios = null!;
        private TextBox txtBuscar = null!;
        private Button btnNuevo = null!;
        private Button btnEditar = null!;
        private Button btnEliminar = null!;
        private Button btnBuscar = null!;
        private Label lblBuscar = null!;

        public FormUsuarios() : this(Program.ServiceProvider.GetRequiredService<IUsuarioService>())
        {
        }

        public FormUsuarios(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
            InitializeComponent();
            ConfigurarFormulario();
            ConfigurarComponentes();
            _ = CargarUsuariosAsync();
        }

        private void ConfigurarFormulario()
        {
            this.Text = "Gestión de Usuarios";
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
        }

        private void ConfigurarComponentes()
        {
            // Panel superior para búsqueda
            var panelSuperior = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                Padding = new Padding(10)
            };

            lblBuscar = new Label
            {
                Text = "Buscar:",
                Location = new Point(10, 20),
                Size = new Size(50, 23),
                TextAlign = ContentAlignment.MiddleRight
            };

            txtBuscar = new TextBox
            {
                Location = new Point(65, 20),
                Size = new Size(300, 23),
                PlaceholderText = "Buscar por nombre, apellido, documento o email..."
            };

            btnBuscar = new Button
            {
                Text = "Buscar",
                Location = new Point(370, 19),
                Size = new Size(75, 25),
                UseVisualStyleBackColor = true
            };
            btnBuscar.Click += BtnBuscar_Click;

            panelSuperior.Controls.AddRange(new Control[] { lblBuscar, txtBuscar, btnBuscar });

            // Panel lateral para botones
            var panelLateral = new Panel
            {
                Dock = DockStyle.Right,
                Width = 120,
                Padding = new Padding(10)
            };

            btnNuevo = new Button
            {
                Text = "Nuevo",
                Location = new Point(10, 10),
                Size = new Size(100, 30),
                UseVisualStyleBackColor = true,
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnNuevo.Click += BtnNuevo_Click;

            btnEditar = new Button
            {
                Text = "Editar",
                Location = new Point(10, 50),
                Size = new Size(100, 30),
                UseVisualStyleBackColor = true,
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnEditar.Click += BtnEditar_Click;

            btnEliminar = new Button
            {
                Text = "Eliminar",
                Location = new Point(10, 90),
                Size = new Size(100, 30),
                UseVisualStyleBackColor = true,
                BackColor = Color.FromArgb(244, 67, 54),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnEliminar.Click += BtnEliminar_Click;

            panelLateral.Controls.AddRange(new Control[] { btnNuevo, btnEditar, btnEliminar });

            // DataGridView
            dgvUsuarios = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                GridColor = Color.FromArgb(224, 224, 224),
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    SelectionBackColor = Color.FromArgb(33, 150, 243),
                    SelectionForeColor = Color.White,
                    Padding = new Padding(5)
                },
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(55, 71, 79),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                },
                RowTemplate = { Height = 35 }
            };

            // Agregar controles al formulario
            this.Controls.Add(dgvUsuarios);
            this.Controls.Add(panelLateral);
            this.Controls.Add(panelSuperior);
        }

        private async Task CargarUsuariosAsync()
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                var usuarios = await _usuarioService.ObtenerTodosAsync();
                MostrarUsuariosEnGrid(usuarios);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar usuarios: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void MostrarUsuariosEnGrid(List<Usuario> usuarios)
        {
            dgvUsuarios.DataSource = null;
            dgvUsuarios.DataSource = usuarios.Select(u => new
            {
                u.Id,
                u.Documento,
                u.Nombre,
                u.Apellido,
                u.Email,
                u.Telefono,
                Tipo = u.TipoUsuario.ToString(),
                Modalidad = u.EsEstudiante ? u.ModalidadPago?.ToString() ?? "N/A" : "N/A",
                Estado = u.Activo ? "Activo" : "Inactivo"
            }).ToList();

            // Ocultar columna Id
            if (dgvUsuarios.Columns["Id"] != null)
                dgvUsuarios.Columns["Id"].Visible = false;

            // Configurar anchos de columna de forma segura
            if (dgvUsuarios.Columns.Count > 0)
            {
                foreach (DataGridViewColumn column in dgvUsuarios.Columns)
                {
                    switch (column.Name)
                    {
                        case "Documento":
                            column.Width = 100;
                            break;
                        case "Nombre":
                            column.Width = 120;
                            break;
                        case "Apellido":
                            column.Width = 120;
                            break;
                        case "Email":
                            column.Width = 180;
                            break;
                        case "Telefono":
                            column.Width = 100;
                            break;
                        case "Tipo":
                            column.Width = 100;
                            break;
                        case "Modalidad":
                            column.Width = 100;
                            break;
                        case "Estado":
                            column.Width = 80;
                            break;
                    }
                }
            }
        }

        private async void BtnBuscar_Click(object? sender, EventArgs e)
        {
            try
            {
                var termino = txtBuscar.Text.Trim();
                var usuarios = await _usuarioService.BuscarAsync(termino);
                MostrarUsuariosEnGrid(usuarios);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnNuevo_Click(object? sender, EventArgs e)
        {
            var formEditar = new FormEditarUsuario();
            if (formEditar.ShowDialog() == DialogResult.OK)
            {
                _ = CargarUsuariosAsync();
            }
        }

        private void BtnEditar_Click(object? sender, EventArgs e)
        {
            if (dgvUsuarios.SelectedRows.Count == 0)
            {
                MessageBox.Show("Por favor seleccione un usuario para editar", "Información",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var id = Convert.ToInt32(dgvUsuarios.SelectedRows[0].Cells["Id"].Value);
            var formEditar = new FormEditarUsuario(id);
            if (formEditar.ShowDialog() == DialogResult.OK)
            {
                _ = CargarUsuariosAsync();
            }
        }

        private async void BtnEliminar_Click(object? sender, EventArgs e)
        {
            if (dgvUsuarios.SelectedRows.Count == 0)
            {
                MessageBox.Show("Por favor seleccione un usuario para eliminar", "Información",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var id = Convert.ToInt32(dgvUsuarios.SelectedRows[0].Cells["Id"].Value);
            var nombre = dgvUsuarios.SelectedRows[0].Cells["Nombre"].Value?.ToString();
            var apellido = dgvUsuarios.SelectedRows[0].Cells["Apellido"].Value?.ToString();

            var resultado = MessageBox.Show(
                $"¿Está seguro de eliminar al usuario {nombre} {apellido}?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (resultado == DialogResult.Yes)
            {
                try
                {
                    await _usuarioService.EliminarAsync(id);
                    MessageBox.Show("Usuario eliminado correctamente", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await CargarUsuariosAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al eliminar: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }
    }
}