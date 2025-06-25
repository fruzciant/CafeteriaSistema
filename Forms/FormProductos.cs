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
    public partial class FormProductos : Form
    {
        private readonly IProductoService _productoService;
        private DataGridView dgvProductos = null!;
        private TextBox txtBuscar = null!;
        private ComboBox cboFiltroCategoria = null!;
        private CheckBox chkSoloMenuDelDia = null!;
        private CheckBox chkSoloBajoStock = null!;
        private Button btnNuevo = null!;
        private Button btnEditar = null!;
        private Button btnEliminar = null!;
        private Button btnBuscar = null!;
        private Button btnActualizarStock = null!;
        private Label lblBuscar = null!;
        private Label lblCategoria = null!;
        private StatusStrip statusStrip = null!;
        private ToolStripStatusLabel lblTotalProductos = null!;
        private ToolStripStatusLabel lblProductosBajoStock = null!;

        public FormProductos() : this(Program.ServiceProvider.GetRequiredService<IProductoService>())
        {
        }

        public FormProductos(IProductoService productoService)
        {
            _productoService = productoService;
            InitializeComponent();
            ConfigurarFormulario();
            ConfigurarComponentes();
            _ = CargarProductosAsync();
        }

        private void ConfigurarFormulario()
        {
            this.Text = "Gestión de Productos";
            this.Size = new Size(1000, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
        }

        private void ConfigurarComponentes()
        {
            // Panel superior para búsqueda y filtros
            var panelSuperior = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                Padding = new Padding(10)
            };

            // Primera fila - Búsqueda
            lblBuscar = new Label
            {
                Text = "Buscar:",
                Location = new Point(10, 15),
                Size = new Size(50, 23),
                TextAlign = ContentAlignment.MiddleRight
            };

            txtBuscar = new TextBox
            {
                Location = new Point(65, 15),
                Size = new Size(250, 23),
                PlaceholderText = "Buscar por código, nombre o descripción..."
            };

            btnBuscar = new Button
            {
                Text = "Buscar",
                Location = new Point(320, 14),
                Size = new Size(75, 25),
                UseVisualStyleBackColor = true
            };
            btnBuscar.Click += BtnBuscar_Click;

            // Segunda fila - Filtros
            lblCategoria = new Label
            {
                Text = "Categoría:",
                Location = new Point(10, 45),
                Size = new Size(60, 23),
                TextAlign = ContentAlignment.MiddleRight
            };

            cboFiltroCategoria = new ComboBox
            {
                Location = new Point(75, 45),
                Size = new Size(150, 23),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboFiltroCategoria.Items.Add("Todas");
            cboFiltroCategoria.Items.AddRange(Enum.GetNames(typeof(CategoriaProducto)));
            cboFiltroCategoria.SelectedIndex = 0;
            cboFiltroCategoria.SelectedIndexChanged += FiltrosChanged;

            chkSoloMenuDelDia = new CheckBox
            {
                Text = "Solo Menú del Día",
                Location = new Point(240, 47),
                Size = new Size(130, 23)
            };
            chkSoloMenuDelDia.CheckedChanged += FiltrosChanged;

            chkSoloBajoStock = new CheckBox
            {
                Text = "Solo Bajo Stock",
                Location = new Point(380, 47),
                Size = new Size(120, 23)
            };
            chkSoloBajoStock.CheckedChanged += FiltrosChanged;

            panelSuperior.Controls.AddRange(new Control[] {
                lblBuscar, txtBuscar, btnBuscar,
                lblCategoria, cboFiltroCategoria, chkSoloMenuDelDia, chkSoloBajoStock
            });

            // Panel lateral para botones
            var panelLateral = new Panel
            {
                Dock = DockStyle.Right,
                Width = 140,
                Padding = new Padding(10)
            };

            btnNuevo = new Button
            {
                Text = "Nuevo",
                Location = new Point(10, 10),
                Size = new Size(120, 30),
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
                Size = new Size(120, 30),
                UseVisualStyleBackColor = true,
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnEditar.Click += BtnEditar_Click;

            btnActualizarStock = new Button
            {
                Text = "Actualizar Stock",
                Location = new Point(10, 90),
                Size = new Size(120, 30),
                UseVisualStyleBackColor = true,
                BackColor = Color.FromArgb(255, 152, 0),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnActualizarStock.Click += BtnActualizarStock_Click;

            btnEliminar = new Button
            {
                Text = "Eliminar",
                Location = new Point(10, 130),
                Size = new Size(120, 30),
                UseVisualStyleBackColor = true,
                BackColor = Color.FromArgb(244, 67, 54),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnEliminar.Click += BtnEliminar_Click;

            panelLateral.Controls.AddRange(new Control[] { btnNuevo, btnEditar, btnActualizarStock, btnEliminar });

            // DataGridView
            dgvProductos = new DataGridView
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

            // Barra de estado
            statusStrip = new StatusStrip();
            lblTotalProductos = new ToolStripStatusLabel("Total: 0 productos");
            lblProductosBajoStock = new ToolStripStatusLabel("Bajo stock: 0");
            statusStrip.Items.AddRange(new ToolStripItem[] {
                lblTotalProductos,
                new ToolStripStatusLabel() { Spring = true },
                lblProductosBajoStock
            });

            // Agregar controles al formulario
            this.Controls.Add(dgvProductos);
            this.Controls.Add(panelLateral);
            this.Controls.Add(panelSuperior);
            this.Controls.Add(statusStrip);
        }

        private async Task CargarProductosAsync()
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                var productos = await _productoService.ObtenerTodosAsync();

                // Aplicar filtros
                if (cboFiltroCategoria.SelectedIndex > 0)
                {
                    var categoria = (CategoriaProducto)Enum.Parse(typeof(CategoriaProducto),
                        cboFiltroCategoria.SelectedItem!.ToString()!);
                    productos = productos.Where(p => p.Categoria == categoria).ToList();
                }

                if (chkSoloMenuDelDia.Checked)
                    productos = productos.Where(p => p.EsMenuDelDia).ToList();

                if (chkSoloBajoStock.Checked)
                    productos = productos.Where(p => p.BajoStock).ToList();

                MostrarProductosEnGrid(productos);
                ActualizarBarraEstado(productos);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar productos: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void MostrarProductosEnGrid(List<Producto> productos)
        {
            dgvProductos.DataSource = null;
            dgvProductos.DataSource = productos.Select(p => new
            {
                p.Id,
                p.Codigo,
                p.Nombre,
                Categoria = p.Categoria.ToString(),
                Precio = p.Precio.ToString("C"),
                Stock = p.StockDisponible,
                StockMinimo = p.StockMinimo,
                MenuDelDia = p.EsMenuDelDia ? "Sí" : "No",
                Estado = p.Activo ? "Activo" : "Inactivo",
                EstadoStock = p.EstadoStock
            }).ToList();

            // Ocultar columna Id
            if (dgvProductos.Columns["Id"] != null)
                dgvProductos.Columns["Id"].Visible = false;

            // Configurar anchos de columna de forma segura
            if (dgvProductos.Columns.Count > 0)
            {
                foreach (DataGridViewColumn column in dgvProductos.Columns)
                {
                    switch (column.Name)
                    {
                        case "Codigo":
                            column.Width = 80;
                            break;
                        case "Nombre":
                            column.Width = 200;
                            break;
                        case "Categoria":
                            column.Width = 100;
                            break;
                        case "Precio":
                            column.Width = 80;
                            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                            break;
                        case "Stock":
                            column.Width = 60;
                            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            break;
                        case "StockMinimo":
                            column.HeaderText = "Stock Mín.";
                            column.Width = 80;
                            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            break;
                        case "MenuDelDia":
                            column.HeaderText = "Menú Día";
                            column.Width = 80;
                            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            break;
                        case "Estado":
                            column.Width = 70;
                            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            break;
                        case "EstadoStock":
                            column.HeaderText = "Estado Stock";
                            column.Width = 100;
                            break;
                    }
                }

                // Colorear filas según estado del stock
                foreach (DataGridViewRow row in dgvProductos.Rows)
                {
                    var estadoStock = row.Cells["EstadoStock"]?.Value?.ToString();
                    if (estadoStock == "Sin Stock")
                        row.DefaultCellStyle.BackColor = Color.FromArgb(255, 235, 238);
                    else if (estadoStock == "Stock Bajo")
                        row.DefaultCellStyle.BackColor = Color.FromArgb(255, 245, 230);
                }
            }
        }

        private void ActualizarBarraEstado(List<Producto> productos)
        {
            lblTotalProductos.Text = $"Total: {productos.Count} productos";
            var bajoStock = productos.Count(p => p.BajoStock);
            lblProductosBajoStock.Text = $"Bajo stock: {bajoStock}";
            if (bajoStock > 0)
                lblProductosBajoStock.ForeColor = Color.Red;
            else
                lblProductosBajoStock.ForeColor = SystemColors.ControlText;
        }

        private async void BtnBuscar_Click(object? sender, EventArgs e)
        {
            try
            {
                var termino = txtBuscar.Text.Trim();
                var productos = await _productoService.BuscarAsync(termino);
                MostrarProductosEnGrid(productos);
                ActualizarBarraEstado(productos);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FiltrosChanged(object? sender, EventArgs e)
        {
            _ = CargarProductosAsync();
        }

        private void BtnNuevo_Click(object? sender, EventArgs e)
        {
            var formEditar = new FormEditarProducto();
            if (formEditar.ShowDialog() == DialogResult.OK)
            {
                _ = CargarProductosAsync();
            }
        }

        private void BtnEditar_Click(object? sender, EventArgs e)
        {
            if (dgvProductos.SelectedRows.Count == 0)
            {
                MessageBox.Show("Por favor seleccione un producto para editar", "Información",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var id = Convert.ToInt32(dgvProductos.SelectedRows[0].Cells["Id"].Value);
            var formEditar = new FormEditarProducto(id);
            if (formEditar.ShowDialog() == DialogResult.OK)
            {
                _ = CargarProductosAsync();
            }
        }

        private async void BtnActualizarStock_Click(object? sender, EventArgs e)
        {
            if (dgvProductos.SelectedRows.Count == 0)
            {
                MessageBox.Show("Por favor seleccione un producto para actualizar stock", "Información",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var id = Convert.ToInt32(dgvProductos.SelectedRows[0].Cells["Id"].Value);
            var nombre = dgvProductos.SelectedRows[0].Cells["Nombre"].Value?.ToString();
            var stockActual = Convert.ToInt32(dgvProductos.SelectedRows[0].Cells["Stock"].Value);

            using (var form = new Form())
            {
                form.Text = "Actualizar Stock";
                form.Size = new Size(400, 200);
                form.StartPosition = FormStartPosition.CenterParent;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.MaximizeBox = false;
                form.MinimizeBox = false;

                var lblProducto = new Label
                {
                    Text = $"Producto: {nombre}",
                    Location = new Point(20, 20),
                    Size = new Size(350, 23)
                };

                var lblStockActual = new Label
                {
                    Text = $"Stock Actual: {stockActual}",
                    Location = new Point(20, 50),
                    Size = new Size(150, 23)
                };

                var lblCantidad = new Label
                {
                    Text = "Cantidad:",
                    Location = new Point(20, 80),
                    Size = new Size(80, 23),
                    TextAlign = ContentAlignment.MiddleRight
                };

                var nudCantidad = new NumericUpDown
                {
                    Location = new Point(105, 80),
                    Size = new Size(100, 23),
                    Maximum = 9999,
                    Minimum = -9999,
                    Value = 0
                };

                var lblNota = new Label
                {
                    Text = "(Positivo para agregar, negativo para reducir)",
                    Location = new Point(210, 82),
                    Size = new Size(170, 23),
                    Font = new Font("Segoe UI", 8F),
                    ForeColor = Color.Gray
                };

                var btnAceptar = new Button
                {
                    Text = "Aceptar",
                    Location = new Point(120, 120),
                    Size = new Size(75, 30),
                    DialogResult = DialogResult.OK
                };

                var btnCancelar = new Button
                {
                    Text = "Cancelar",
                    Location = new Point(205, 120),
                    Size = new Size(75, 30),
                    DialogResult = DialogResult.Cancel
                };

                form.Controls.AddRange(new Control[] {
                    lblProducto, lblStockActual, lblCantidad, nudCantidad,
                    lblNota, btnAceptar, btnCancelar
                });

                if (form.ShowDialog() == DialogResult.OK)
                {
                    var cantidad = (int)nudCantidad.Value;
                    if (cantidad != 0)
                    {
                        try
                        {
                            var esVenta = cantidad < 0;
                            await _productoService.ActualizarStockAsync(id, Math.Abs(cantidad), esVenta);
                            MessageBox.Show("Stock actualizado correctamente", "Éxito",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            await CargarProductosAsync();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error al actualizar stock: {ex.Message}", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        private async void BtnEliminar_Click(object? sender, EventArgs e)
        {
            if (dgvProductos.SelectedRows.Count == 0)
            {
                MessageBox.Show("Por favor seleccione un producto para eliminar", "Información",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var id = Convert.ToInt32(dgvProductos.SelectedRows[0].Cells["Id"].Value);
            var nombre = dgvProductos.SelectedRows[0].Cells["Nombre"].Value?.ToString();

            var resultado = MessageBox.Show(
                $"¿Está seguro de eliminar el producto {nombre}?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (resultado == DialogResult.Yes)
            {
                try
                {
                    await _productoService.EliminarAsync(id);
                    MessageBox.Show("Producto eliminado correctamente", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await CargarProductosAsync();
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