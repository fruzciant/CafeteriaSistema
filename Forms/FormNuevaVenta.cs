using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using CafeteriaUNAL.Models;
using CafeteriaUNAL.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CafeteriaUNAL.Forms
{
    public partial class FormNuevaVenta : Form
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IProductoService _productoService;
        private readonly ITransaccionService _transaccionService;

        private Usuario? _usuarioSeleccionado;
        private List<DetalleVenta> _detallesVenta = new();

        // Controles principales
        private GroupBox grpUsuario = null!;
        private TextBox txtBuscarUsuario = null!;
        private Button btnBuscarUsuario = null!;
        private Label lblUsuarioSeleccionado = null!;
        private Label lblTipoUsuario = null!;
        private Label lblDescuento = null!;

        private GroupBox grpProductos = null!;
        private ComboBox cboCategoria = null!;
        private ComboBox cboProducto = null!;
        private NumericUpDown nudCantidad = null!;
        private Button btnAgregar = null!;
        private Label lblPrecioUnitario = null!;
        private Label lblStock = null!;

        private GroupBox grpDetalles = null!;
        private DataGridView dgvDetalles = null!;
        private Button btnEliminarItem = null!;
        private GroupBox grpTotales = null!;
        private Label lblSubtotal = null!;
        private Label lblDescuentoTotal = null!;
        private Label lblTotal = null!;

        private GroupBox grpPago = null!;
        private RadioButton rbEfectivo = null!;
        private RadioButton rbTarjeta = null!;
        private RadioButton rbTransferencia = null!;

        private Button btnGuardarVenta = null!;
        private Button btnCancelar = null!;
        private Button btnNuevaVenta = null!;

        public FormNuevaVenta()
        {
            _usuarioService = Program.ServiceProvider.GetRequiredService<IUsuarioService>();
            _productoService = Program.ServiceProvider.GetRequiredService<IProductoService>();
            _transaccionService = Program.ServiceProvider.GetRequiredService<ITransaccionService>();

            InitializeComponent();
            ConfigurarFormulario();
            CrearControles();
            _ = CargarDatosInicialesAsync();
        }

        private void ConfigurarFormulario()
        {
            this.Text = "Nueva Venta";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
        }
        private void CrearControles()
        {
            // Grupo Usuario
            grpUsuario = new GroupBox
            {
                Text = "Seleccionar Usuario",
                Location = new Point(20, 20),
                Size = new Size(460, 120)
            };

            var lblBuscar = new Label
            {
                Text = "Buscar:",
                Location = new Point(20, 30),
                Size = new Size(50, 23),
                TextAlign = ContentAlignment.MiddleRight
            };
            grpUsuario.Controls.Add(lblBuscar);

            txtBuscarUsuario = new TextBox
            {
                Location = new Point(75, 30),
                Size = new Size(200, 23),
                PlaceholderText = "Documento o nombre..."
            };
            txtBuscarUsuario.KeyPress += TxtBuscarUsuario_KeyPress;
            grpUsuario.Controls.Add(txtBuscarUsuario);

            btnBuscarUsuario = new Button
            {
                Text = "Buscar",
                Location = new Point(280, 29),
                Size = new Size(75, 25)
            };
            btnBuscarUsuario.Click += BtnBuscarUsuario_Click;
            grpUsuario.Controls.Add(btnBuscarUsuario);

            lblUsuarioSeleccionado = new Label
            {
                Text = "Usuario: No seleccionado",
                Location = new Point(20, 60),
                Size = new Size(420, 23),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };
            grpUsuario.Controls.Add(lblUsuarioSeleccionado);

            lblTipoUsuario = new Label
            {
                Text = "Tipo: -",
                Location = new Point(20, 85),
                Size = new Size(200, 23)
            };
            grpUsuario.Controls.Add(lblTipoUsuario);

            lblDescuento = new Label
            {
                Text = "Descuento: 0%",
                Location = new Point(230, 85),
                Size = new Size(200, 23),
                ForeColor = Color.Green,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            grpUsuario.Controls.Add(lblDescuento);

            this.Controls.Add(grpUsuario);

            // Grupo Productos
            grpProductos = new GroupBox
            {
                Text = "Agregar Productos",
                Location = new Point(500, 20),
                Size = new Size(460, 120)
            };

            var lblCategoria = new Label
            {
                Text = "Categoría:",
                Location = new Point(20, 30),
                Size = new Size(70, 23),
                TextAlign = ContentAlignment.MiddleRight
            };
            grpProductos.Controls.Add(lblCategoria);

            cboCategoria = new ComboBox
            {
                Location = new Point(95, 30),
                Size = new Size(150, 23),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboCategoria.SelectedIndexChanged += CboCategoria_SelectedIndexChanged;
            grpProductos.Controls.Add(cboCategoria);

            var lblProducto = new Label
            {
                Text = "Producto:",
                Location = new Point(20, 60),
                Size = new Size(70, 23),
                TextAlign = ContentAlignment.MiddleRight
            };
            grpProductos.Controls.Add(lblProducto);

            cboProducto = new ComboBox
            {
                Location = new Point(95, 60),
                Size = new Size(250, 23),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboProducto.SelectedIndexChanged += CboProducto_SelectedIndexChanged;
            grpProductos.Controls.Add(cboProducto);

            var lblCantidad = new Label
            {
                Text = "Cantidad:",
                Location = new Point(20, 90),
                Size = new Size(70, 23),
                TextAlign = ContentAlignment.MiddleRight
            };
            grpProductos.Controls.Add(lblCantidad);

            nudCantidad = new NumericUpDown
            {
                Location = new Point(95, 90),
                Size = new Size(60, 23),
                Minimum = 1,
                Maximum = 100,
                Value = 1
            };
            grpProductos.Controls.Add(nudCantidad);

            lblPrecioUnitario = new Label
            {
                Text = "Precio: $0",
                Location = new Point(165, 90),
                Size = new Size(100, 23),
                ForeColor = Color.Blue
            };
            grpProductos.Controls.Add(lblPrecioUnitario);

            lblStock = new Label
            {
                Text = "Stock: 0",
                Location = new Point(270, 90),
                Size = new Size(80, 23)
            };
            grpProductos.Controls.Add(lblStock);

            btnAgregar = new Button
            {
                Text = "Agregar",
                Location = new Point(360, 88),
                Size = new Size(80, 27),
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnAgregar.Click += BtnAgregar_Click;
            grpProductos.Controls.Add(btnAgregar);

            this.Controls.Add(grpProductos);

            // Grupo Detalles
            grpDetalles = new GroupBox
            {
                Text = "Detalle de la Venta",
                Location = new Point(20, 150),
                Size = new Size(620, 350)
            };

            dgvDetalles = new DataGridView
            {
                Location = new Point(10, 25),
                Size = new Size(600, 280),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false
            };
            grpDetalles.Controls.Add(dgvDetalles);

            btnEliminarItem = new Button
            {
                Text = "Eliminar Item",
                Location = new Point(10, 310),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(244, 67, 54),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnEliminarItem.Click += BtnEliminarItem_Click;
            grpDetalles.Controls.Add(btnEliminarItem);

            this.Controls.Add(grpDetalles);

            // Grupo Totales
            grpTotales = new GroupBox
            {
                Text = "Resumen",
                Location = new Point(650, 150),
                Size = new Size(310, 200)
            };

            lblSubtotal = new Label
            {
                Text = "Subtotal: $0.00",
                Location = new Point(20, 30),
                Size = new Size(270, 30),
                Font = new Font("Segoe UI", 12F),
                TextAlign = ContentAlignment.MiddleRight
            };
            grpTotales.Controls.Add(lblSubtotal);

            lblDescuentoTotal = new Label
            {
                Text = "Descuento: $0.00",
                Location = new Point(20, 70),
                Size = new Size(270, 30),
                Font = new Font("Segoe UI", 12F),
                TextAlign = ContentAlignment.MiddleRight,
                ForeColor = Color.Green
            };
            grpTotales.Controls.Add(lblDescuentoTotal);

            lblTotal = new Label
            {
                Text = "TOTAL: $0.00",
                Location = new Point(20, 120),
                Size = new Size(270, 40),
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleRight,
                ForeColor = Color.Navy
            };
            grpTotales.Controls.Add(lblTotal);

            this.Controls.Add(grpTotales);

            // Grupo Tipo de Pago
            grpPago = new GroupBox
            {
                Text = "Tipo de Pago",
                Location = new Point(650, 360),
                Size = new Size(310, 140)
            };

            rbEfectivo = new RadioButton
            {
                Text = "Efectivo",
                Location = new Point(30, 30),
                Size = new Size(100, 25),
                Checked = true
            };
            grpPago.Controls.Add(rbEfectivo);

            rbTarjeta = new RadioButton
            {
                Text = "Tarjeta",
                Location = new Point(30, 60),
                Size = new Size(100, 25)
            };
            grpPago.Controls.Add(rbTarjeta);

            rbTransferencia = new RadioButton
            {
                Text = "Transferencia",
                Location = new Point(30, 90),
                Size = new Size(120, 25)
            };
            grpPago.Controls.Add(rbTransferencia);

            this.Controls.Add(grpPago);

            // Botones principales
            btnGuardarVenta = new Button
            {
                Text = "Guardar Venta",
                Location = new Point(650, 520),
                Size = new Size(150, 40),
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };
            btnGuardarVenta.Click += BtnGuardarVenta_Click;
            this.Controls.Add(btnGuardarVenta);

            btnNuevaVenta = new Button
            {
                Text = "Nueva Venta",
                Location = new Point(810, 520),
                Size = new Size(150, 40),
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Visible = false
            };
            btnNuevaVenta.Click += BtnNuevaVenta_Click;
            this.Controls.Add(btnNuevaVenta);

            btnCancelar = new Button
            {
                Text = "Cancelar",
                Location = new Point(20, 520),
                Size = new Size(100, 40),
                BackColor = Color.FromArgb(158, 158, 158),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnCancelar.Click += (s, e) => this.Close();
            this.Controls.Add(btnCancelar);
        }

        private async Task CargarDatosInicialesAsync()
        {
            try
            {
                cboCategoria.Items.Clear();
                cboCategoria.Items.Add("Todas");
                cboCategoria.Items.AddRange(Enum.GetNames(typeof(CategoriaProducto)));
                cboCategoria.SelectedIndex = 0;

                ConfigurarGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar datos: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ConfigurarGrid()
        {
            dgvDetalles.Columns.Clear();
            dgvDetalles.Columns.Add("ProductoId", "ID");
            dgvDetalles.Columns.Add("Codigo", "Código");
            dgvDetalles.Columns.Add("Nombre", "Producto");
            dgvDetalles.Columns.Add("Cantidad", "Cantidad");
            dgvDetalles.Columns.Add("PrecioUnitario", "P. Unitario");
            dgvDetalles.Columns.Add("Subtotal", "Subtotal");

            dgvDetalles.Columns["ProductoId"].Visible = false;
            dgvDetalles.Columns["Codigo"].Width = 80;
            dgvDetalles.Columns["Nombre"].Width = 200;
            dgvDetalles.Columns["Cantidad"].Width = 80;
            dgvDetalles.Columns["PrecioUnitario"].Width = 100;
            dgvDetalles.Columns["Subtotal"].Width = 100;

            dgvDetalles.Columns["PrecioUnitario"].DefaultCellStyle.Format = "C2";
            dgvDetalles.Columns["Subtotal"].DefaultCellStyle.Format = "C2";
            dgvDetalles.Columns["PrecioUnitario"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvDetalles.Columns["Subtotal"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        private async void BtnBuscarUsuario_Click(object? sender, EventArgs e)
        {
            await BuscarUsuarioAsync();
        }

        private async void TxtBuscarUsuario_KeyPress(object? sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                await BuscarUsuarioAsync();
            }
        }

        private async Task BuscarUsuarioAsync()
        {
            try
            {
                var termino = txtBuscarUsuario.Text.Trim();
                if (string.IsNullOrEmpty(termino))
                {
                    MessageBox.Show("Ingrese un documento o nombre para buscar", "Información",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                var usuarios = await _usuarioService.BuscarAsync(termino);

                if (usuarios.Count == 0)
                {
                    MessageBox.Show("No se encontró ningún usuario", "Información",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                Usuario usuarioSeleccionado;

                if (usuarios.Count == 1)
                {
                    usuarioSeleccionado = usuarios[0];
                }
                else
                {
                    using var form = new Form
                    {
                        Text = "Seleccionar Usuario",
                        Size = new Size(600, 400),
                        StartPosition = FormStartPosition.CenterParent
                    };

                    var dgv = new DataGridView
                    {
                        Dock = DockStyle.Fill,
                        DataSource = usuarios.Select(u => new
                        {
                            u.Id,
                            u.Documento,
                            u.NombreCompleto,
                            Tipo = u.TipoUsuario.ToString()
                        }).ToList(),
                        AllowUserToAddRows = false,
                        AllowUserToDeleteRows = false,
                        ReadOnly = true,
                        SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                        MultiSelect = false
                    };
                    dgv.Columns["Id"].Visible = false;

                    var panel = new Panel { Dock = DockStyle.Bottom, Height = 50 };
                    var btnSeleccionar = new Button
                    {
                        Text = "Seleccionar",
                        DialogResult = DialogResult.OK,
                        Location = new Point(400, 10),
                        Size = new Size(80, 30)
                    };
                    var btnCancelar = new Button
                    {
                        Text = "Cancelar",
                        DialogResult = DialogResult.Cancel,
                        Location = new Point(490, 10),
                        Size = new Size(80, 30)
                    };
                    panel.Controls.AddRange(new Control[] { btnSeleccionar, btnCancelar });

                    form.Controls.Add(dgv);
                    form.Controls.Add(panel);

                    if (form.ShowDialog() != DialogResult.OK || dgv.SelectedRows.Count == 0)
                        return;

                    var id = Convert.ToInt32(dgv.SelectedRows[0].Cells["Id"].Value);
                    usuarioSeleccionado = usuarios.First(u => u.Id == id);
                }

                SeleccionarUsuario(usuarioSeleccionado);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar usuario: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SeleccionarUsuario(Usuario usuario)
        {
            _usuarioSeleccionado = usuario;
            lblUsuarioSeleccionado.Text = $"Usuario: {usuario.NombreCompleto} - {usuario.Documento}";
            lblTipoUsuario.Text = $"Tipo: {usuario.TipoUsuario}";
            if (usuario.EsEstudiante)
            {
                var descuento = usuario.ObtenerPorcentajeDescuento();
                lblDescuento.Text = $"Descuento: {descuento}%";

                if (usuario.ModalidadPago == ModalidadPagoEstudiante.Subsidiado)
                {
                    lblDescuento.Text += " (SUBSIDIADO)";
                    lblDescuento.ForeColor = Color.Red;
                }
                else
                {
                    lblDescuento.ForeColor = Color.Green;
                }
            }
            else
            {
                lblDescuento.Text = "Descuento: 0%";
                lblDescuento.ForeColor = Color.Black;
            }

            ActualizarTotales();
        }

        private async void CboCategoria_SelectedIndexChanged(object? sender, EventArgs e)
        {
            await CargarProductosAsync();
        }

        private async Task CargarProductosAsync()
        {
            try
            {
                cboProducto.Items.Clear();
                cboProducto.Text = "";
                lblPrecioUnitario.Text = "Precio: $0";
                lblStock.Text = "Stock: 0";

                List<Producto> productos;

                if (cboCategoria.SelectedIndex <= 0)
                {
                    productos = await _productoService.ObtenerActivosAsync();
                }
                else
                {
                    var categoria = (CategoriaProducto)Enum.Parse(typeof(CategoriaProducto),
                        cboCategoria.SelectedItem!.ToString()!);
                    productos = await _productoService.ObtenerPorCategoriaAsync(categoria);
                }
                productos = productos.Where(p => p.StockDisponible > 0).ToList();

                foreach (var producto in productos)
                {
                    cboProducto.Items.Add(new ProductoItem
                    {
                        Id = producto.Id,
                        Codigo = producto.Codigo,
                        Nombre = producto.Nombre,
                        Precio = producto.Precio,
                        Stock = producto.StockDisponible
                    });
                }

                if (cboProducto.Items.Count > 0)
                    cboProducto.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar productos: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CboProducto_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (cboProducto.SelectedItem is ProductoItem producto)
            {
                lblPrecioUnitario.Text = $"Precio: {producto.Precio:C}";
                lblStock.Text = $"Stock: {producto.Stock}";
                nudCantidad.Maximum = producto.Stock;
            }
        }

        private void BtnAgregar_Click(object? sender, EventArgs e)
        {
            if (_usuarioSeleccionado == null)
            {
                MessageBox.Show("Debe seleccionar un usuario primero", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBuscarUsuario.Focus();
                return;
            }
            if (cboProducto.SelectedItem == null)
            {
                MessageBox.Show("Debe seleccionar un producto", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var producto = (ProductoItem)cboProducto.SelectedItem;
            var cantidad = (int)nudCantidad.Value;

            var detalleExistente = _detallesVenta.FirstOrDefault(d => d.ProductoId == producto.Id);
            if (detalleExistente != null)
            {
                if (detalleExistente.Cantidad + cantidad > producto.Stock)
                {
                    MessageBox.Show("No hay suficiente stock disponible", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                detalleExistente.Cantidad += cantidad;
                detalleExistente.Subtotal = detalleExistente.Cantidad * detalleExistente.PrecioUnitario;
            }
            else
            {
                _detallesVenta.Add(new DetalleVenta
                {
                    ProductoId = producto.Id,
                    Codigo = producto.Codigo,
                    Nombre = producto.Nombre,
                    Cantidad = cantidad,
                    PrecioUnitario = producto.Precio,
                    Subtotal = cantidad * producto.Precio
                });
            }

            ActualizarGrid();
            ActualizarTotales();
            nudCantidad.Value = 1;
            cboProducto.Focus();
        }
        private void BtnEliminarItem_Click(object? sender, EventArgs e)
        {
            if (dgvDetalles.SelectedRows.Count == 0)
                return;

            var productoId = Convert.ToInt32(dgvDetalles.SelectedRows[0].Cells["ProductoId"].Value);
            _detallesVenta.RemoveAll(d => d.ProductoId == productoId);

            ActualizarGrid();
            ActualizarTotales();
        }

        private void ActualizarGrid()
        {
            dgvDetalles.Rows.Clear();

            foreach (var detalle in _detallesVenta)
            {
                dgvDetalles.Rows.Add(
                    detalle.ProductoId,
                    detalle.Codigo,
                    detalle.Nombre,
                    detalle.Cantidad,
                    detalle.PrecioUnitario,
                    detalle.Subtotal
                );
            }
        }

        private void ActualizarTotales()
        {
            var subtotal = _detallesVenta.Sum(d => d.Subtotal);
            var porcentajeDescuento = _usuarioSeleccionado?.ObtenerPorcentajeDescuento() ?? 0;
            var montoDescuento = subtotal * (porcentajeDescuento / 100);
            var total = subtotal - montoDescuento;

            if (_usuarioSeleccionado?.EsEstudiante == true &&
                _usuarioSeleccionado.ModalidadPago == ModalidadPagoEstudiante.Subsidiado)
            {
                total = 0;
            }

            lblSubtotal.Text = $"Subtotal: {subtotal:C}";
            lblDescuentoTotal.Text = $"Descuento: {montoDescuento:C}";
            lblTotal.Text = $"TOTAL: {total:C}";
        }
        private async void BtnGuardarVenta_Click(object? sender, EventArgs e)
        {
            if (!ValidarVenta())
                return;

            try
            {
                this.Enabled = false;
                this.Cursor = Cursors.WaitCursor;

                var tipoPago = TipoPago.Efectivo;
                if (rbTarjeta.Checked) tipoPago = TipoPago.Tarjeta;
                if (rbTransferencia.Checked) tipoPago = TipoPago.Transferencia;

                var items = _detallesVenta.Select(d => (d.ProductoId, d.Cantidad)).ToList();

                var transaccion = await _transaccionService.CrearTransaccionAsync(
                    _usuarioSeleccionado!.Id,
                    items,
                    tipoPago
                );

                MessageBox.Show($"Venta registrada exitosamente\n\nNúmero: {transaccion.NumeroTransaccion}\n" +
                    $"Total: {transaccion.Total:C}", "Éxito",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                btnGuardarVenta.Visible = false;
                btnNuevaVenta.Visible = true;
                grpProductos.Enabled = false;
                grpPago.Enabled = false;
                btnEliminarItem.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar la venta: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Enabled = true;
                this.Cursor = Cursors.Default;
            }
        }
        private bool ValidarVenta()
        {
            if (_usuarioSeleccionado == null)
            {
                MessageBox.Show("Debe seleccionar un usuario", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (_detallesVenta.Count == 0)
            {
                MessageBox.Show("Debe agregar al menos un producto", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void BtnNuevaVenta_Click(object? sender, EventArgs e)
        {
            LimpiarFormulario();
        }

        private void LimpiarFormulario()
        {
            _usuarioSeleccionado = null;
            _detallesVenta.Clear();

            txtBuscarUsuario.Clear();
            lblUsuarioSeleccionado.Text = "Usuario: No seleccionado";
            lblTipoUsuario.Text = "Tipo: -";
            lblDescuento.Text = "Descuento: 0%";
            lblDescuento.ForeColor = Color.Black;

            cboCategoria.SelectedIndex = 0;
            nudCantidad.Value = 1;

            dgvDetalles.Rows.Clear();
            ActualizarTotales();

            rbEfectivo.Checked = true;

            btnGuardarVenta.Visible = true;
            btnNuevaVenta.Visible = false;
            grpProductos.Enabled = true;
            grpPago.Enabled = true;
            btnEliminarItem.Enabled = true;

            txtBuscarUsuario.Focus();
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.F5)
            {
                LimpiarFormulario();
            }
            else if (e.KeyCode == Keys.F12 && btnGuardarVenta.Visible)
            {
                BtnGuardarVenta_Click(this, EventArgs.Empty);
            }
        }

        // Clases auxiliares
        private class ProductoItem
        {
            public int Id { get; set; }
            public string Codigo { get; set; } = string.Empty;
            public string Nombre { get; set; } = string.Empty;
            public decimal Precio { get; set; }
            public int Stock { get; set; }

            public override string ToString()
            {
                return $"{Codigo} - {Nombre}";
            }
        }

        private class DetalleVenta
        {
            public int ProductoId { get; set; }
            public string Codigo { get; set; } = string.Empty;
            public string Nombre { get; set; } = string.Empty;
            public int Cantidad { get; set; }
            public decimal PrecioUnitario { get; set; }
            public decimal Subtotal { get; set; }
        }
    }
}
        
