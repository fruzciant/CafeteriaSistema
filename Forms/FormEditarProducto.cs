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
    public partial class FormEditarProducto : Form
    {
        private readonly IProductoService _productoService;
        private Producto? _productoActual;
        private readonly bool _esNuevo;

        // Controles del formulario
        private TextBox txtCodigo = null!;
        private TextBox txtNombre = null!;
        private TextBox txtDescripcion = null!;
        private ComboBox cboCategoria = null!;
        private NumericUpDown nudPrecio = null!;
        private NumericUpDown nudStock = null!;
        private NumericUpDown nudStockMinimo = null!;
        private CheckBox chkMenuDelDia = null!;
        private CheckBox chkActivo = null!;
        private Button btnGuardar = null!;
        private Button btnCancelar = null!;
        private GroupBox grpInformacion = null!;
        private GroupBox grpInventario = null!;

        public FormEditarProducto() : this(null)
        {
        }

        public FormEditarProducto(int? productoId)
        {
            _productoService = Program.ServiceProvider.GetRequiredService<IProductoService>();
            _esNuevo = !productoId.HasValue;
            InitializeComponent();
            ConfigurarFormulario();
            CrearControles();

            if (!_esNuevo)
            {
                _ = CargarProductoAsync(productoId!.Value);
            }
            else
            {
                ConfigurarFormularioNuevo();
            }
        }

        private void ConfigurarFormulario()
        {
            this.Text = _esNuevo ? "Nuevo Producto" : "Editar Producto";
            this.Size = new Size(550, 580);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
        }

        private void CrearControles()
        {
            // Grupo de Información General
            grpInformacion = new GroupBox
            {
                Text = "Información General",
                Location = new Point(20, 20),
                Size = new Size(490, 250)
            };

            int yPos = 30;
            int espaciado = 35;

            // Código
            CrearLabel("Código:", 20, yPos, grpInformacion);
            txtCodigo = CrearTextBox(120, yPos, 150, grpInformacion);
            txtCodigo.CharacterCasing = CharacterCasing.Upper;
            yPos += espaciado;

            // Nombre
            CrearLabel("Nombre:", 20, yPos, grpInformacion);
            txtNombre = CrearTextBox(120, yPos, 300, grpInformacion);
            yPos += espaciado;

            // Descripción
            CrearLabel("Descripción:", 20, yPos, grpInformacion);
            txtDescripcion = new TextBox
            {
                Location = new Point(120, yPos),
                Size = new Size(350, 60),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };
            grpInformacion.Controls.Add(txtDescripcion);
            yPos += 70;

            // Categoría
            CrearLabel("Categoría:", 20, yPos, grpInformacion);
            cboCategoria = new ComboBox
            {
                Location = new Point(120, yPos),
                Size = new Size(200, 23),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboCategoria.Items.AddRange(Enum.GetNames(typeof(CategoriaProducto)));
            grpInformacion.Controls.Add(cboCategoria);
            yPos += espaciado;

            // Precio
            CrearLabel("Precio:", 20, yPos, grpInformacion);
            nudPrecio = new NumericUpDown
            {
                Location = new Point(120, yPos),
                Size = new Size(120, 23),
                DecimalPlaces = 2,
                Maximum = 999999.99M,
                Minimum = 0.01M,
                Value = 0.01M,
                ThousandsSeparator = true
            };
            grpInformacion.Controls.Add(nudPrecio);

            this.Controls.Add(grpInformacion);

            // Grupo de Inventario
            grpInventario = new GroupBox
            {
                Text = "Control de Inventario",
                Location = new Point(20, 280),
                Size = new Size(490, 150)
            };

            yPos = 30;

            // Stock Disponible
            CrearLabel("Stock Disponible:", 20, yPos, grpInventario);
            nudStock = new NumericUpDown
            {
                Location = new Point(150, yPos),
                Size = new Size(100, 23),
                Maximum = 9999,
                Minimum = 0,
                Value = 0
            };
            grpInventario.Controls.Add(nudStock);
            yPos += espaciado;

            // Stock Mínimo
            CrearLabel("Stock Mínimo:", 20, yPos, grpInventario);
            nudStockMinimo = new NumericUpDown
            {
                Location = new Point(150, yPos),
                Size = new Size(100, 23),
                Maximum = 9999,
                Minimum = 0,
                Value = 5
            };
            grpInventario.Controls.Add(nudStockMinimo);
            yPos += espaciado;

            // Checkboxes
            chkMenuDelDia = new CheckBox
            {
                Text = "Es Menú del Día",
                Location = new Point(150, yPos),
                Size = new Size(150, 23)
            };
            grpInventario.Controls.Add(chkMenuDelDia);

            chkActivo = new CheckBox
            {
                Text = "Producto Activo",
                Location = new Point(310, yPos),
                Size = new Size(150, 23),
                Checked = true
            };
            grpInventario.Controls.Add(chkActivo);

            this.Controls.Add(grpInventario);

            // Botones
            btnGuardar = new Button
            {
                Text = "Guardar",
                Location = new Point(200, 450),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                UseVisualStyleBackColor = false
            };
            btnGuardar.Click += BtnGuardar_Click;
            this.Controls.Add(btnGuardar);

            btnCancelar = new Button
            {
                Text = "Cancelar",
                Location = new Point(310, 450),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(158, 158, 158),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                UseVisualStyleBackColor = false
            };
            btnCancelar.Click += (s, e) => this.Close();
            this.Controls.Add(btnCancelar);
        }

        private Label CrearLabel(string texto, int x, int y, Control parent)
        {
            var label = new Label
            {
                Text = texto,
                Location = new Point(x, y),
                Size = new Size(95, 23),
                TextAlign = ContentAlignment.MiddleRight
            };
            parent.Controls.Add(label);
            return label;
        }

        private TextBox CrearTextBox(int x, int y, int width, Control parent)
        {
            var textBox = new TextBox
            {
                Location = new Point(x, y),
                Size = new Size(width, 23)
            };
            parent.Controls.Add(textBox);
            return textBox;
        }

        private async Task CargarProductoAsync(int productoId)
        {
            try
            {
                _productoActual = await _productoService.ObtenerPorIdAsync(productoId);
                if (_productoActual == null)
                {
                    MessageBox.Show("No se encontró el producto", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
                }

                // Cargar datos en los controles
                txtCodigo.Text = _productoActual.Codigo;
                txtNombre.Text = _productoActual.Nombre;
                txtDescripcion.Text = _productoActual.Descripcion ?? "";
                cboCategoria.SelectedItem = _productoActual.Categoria.ToString();
                nudPrecio.Value = _productoActual.Precio;
                nudStock.Value = _productoActual.StockDisponible;
                nudStockMinimo.Value = _productoActual.StockMinimo;
                chkMenuDelDia.Checked = _productoActual.EsMenuDelDia;
                chkActivo.Checked = _productoActual.Activo;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar producto: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void ConfigurarFormularioNuevo()
        {
            cboCategoria.SelectedIndex = 0;
            chkActivo.Checked = true;
            txtCodigo.Focus();
        }

        private async void BtnGuardar_Click(object? sender, EventArgs e)
        {
            if (!ValidarFormulario())
                return;

            try
            {
                var producto = _esNuevo ? new Producto() : _productoActual!;

                producto.Codigo = txtCodigo.Text.Trim().ToUpper();
                producto.Nombre = txtNombre.Text.Trim();
                producto.Descripcion = string.IsNullOrWhiteSpace(txtDescripcion.Text) ?
                    null : txtDescripcion.Text.Trim();
                producto.Categoria = (CategoriaProducto)Enum.Parse(typeof(CategoriaProducto),
                    cboCategoria.SelectedItem!.ToString()!);
                producto.Precio = nudPrecio.Value;
                producto.StockDisponible = (int)nudStock.Value;
                producto.StockMinimo = (int)nudStockMinimo.Value;
                producto.EsMenuDelDia = chkMenuDelDia.Checked;
                producto.Activo = chkActivo.Checked;

                if (_esNuevo)
                {
                    await _productoService.CrearAsync(producto);
                    MessageBox.Show("Producto creado exitosamente", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    await _productoService.ActualizarAsync(producto);
                    MessageBox.Show("Producto actualizado exitosamente", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidarFormulario()
        {
            if (string.IsNullOrWhiteSpace(txtCodigo.Text))
            {
                MessageBox.Show("El código es obligatorio", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCodigo.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("El nombre es obligatorio", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombre.Focus();
                return false;
            }

            if (cboCategoria.SelectedIndex == -1)
            {
                MessageBox.Show("Debe seleccionar una categoría", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboCategoria.Focus();
                return false;
            }

            if (nudStockMinimo.Value > nudStock.Value)
            {
                MessageBox.Show("El stock mínimo no puede ser mayor al stock disponible", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                nudStockMinimo.Focus();
                return false;
            }

            return true;
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