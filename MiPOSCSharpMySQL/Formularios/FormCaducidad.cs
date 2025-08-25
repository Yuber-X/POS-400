using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiPOSCSharpMySQL.Formularios
{
    public partial class FormCaducidad: Form
    {
        public FormCaducidad()
        {
            InitializeComponent();
            txtid.ReadOnly = true;
            txtnombreproducto.ReadOnly = true;
            txtprecio.ReadOnly = true;
            txtstock.ReadOnly = true;
            txtdescripcion.ReadOnly = true;
            dtpFechaCaducidad.Enabled = false;

            Controlador.ControladorProducto objetoProducto = new Controlador.ControladorProducto();
            objetoProducto.MostrarProductos(dgvproductos);

            CargarProductosCaducidad(dgvproductos);

            dgvproductos.Columns["idProducto"].HeaderText = "ID";
            dgvproductos.Columns["nombre"].HeaderText = "Producto";
            dgvproductos.Columns["precioProducto"].HeaderText = "Precio";
            dgvproductos.Columns["stock"].HeaderText = "Cantidad";
            dgvproductos.Columns["descripcionProducto"].HeaderText = "Descripcion";
            dgvproductos.Columns["fechaCaducidad"].HeaderText = "Caducidad";
            dgvproductos.Columns["mesesRestantes"].HeaderText = "Meses Restantes";



        }

        private void FormCaducidad_Load(object sender, EventArgs e)
        {

        }
        private void dgvproductos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

  //---------------------------------------------------------------------------------------------------
        private void dgvproductos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            Controlador.ControladorProducto objetoProducto = new Controlador.ControladorProducto();
            objetoProducto.SeleccionarProducto(dgvproductos, txtid, txtnombreproducto, txtprecio, txtstock, txtdescripcion, dtpFechaCaducidad);
        }

        private void btnlimpiar_Click(object sender, EventArgs e)
        {
            Controlador.ControladorProducto objetoProducto = new Controlador.ControladorProducto();
            objetoProducto.LimpiarCampos(txtid, txtnombreproducto, txtprecio, txtstock, txtdescripcion, dtpFechaCaducidad);
        }

        private void btneliminar_Click(object sender, EventArgs e)
        {
            Controlador.ControladorProducto objetoProducto = new Controlador.ControladorProducto();
            objetoProducto.EliminarProducto(txtid);
            objetoProducto.MostrarProductos(dgvproductos);
            objetoProducto.LimpiarCampos(txtid, txtnombreproducto, txtprecio, txtstock, txtdescripcion, dtpFechaCaducidad);
        }

        public void CargarProductosCaducidad(DataGridView dgvproductos, int ? filtroMeses = null)
        {
            Configuracion.CConexion objetoConexion = new Configuracion.CConexion();
            DataTable modelo = new DataTable();

            try
            {
                using (MySqlConnection conexion = objetoConexion.estableceConexion())
                {
                    string sql = @"SELECT idProducto, nombre, precioProducto, stock, descripcionProducto, fechaCaducidad,TIMESTAMPDIFF(MONTH, CURDATE(), fechaCaducidad) AS mesesRestantes FROM producto WHERE fechaCaducidad IS NOT NULL ORDER BY fechaCaducidad ASC;";

                    MySqlCommand comando = new MySqlCommand(sql, conexion);
                    MySqlDataAdapter adaptador = new MySqlDataAdapter(comando);
                    adaptador.Fill(modelo);

                    // Si hay filtro (ejemplo 3 meses), solo mostramos esos
                    if (filtroMeses.HasValue)
                    {
                        DataView vista = new DataView(modelo);
                        vista.RowFilter = $"mesesRestantes <= {filtroMeses.Value}";
                        dgvproductos.DataSource = vista;
                    }
                    else
                    {
                        dgvproductos.DataSource = modelo;
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error al cargar productos próximos a caducar: " + e.Message);
            }
            finally
            {
                objetoConexion.CerrarConexion();
            }
        }

        private void dgvproductos_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvproductos.Columns[e.ColumnIndex].Name == "mesesRestantes" && e.Value != null)
            {
                
                long meses = Convert.ToInt64(e.Value);

                if (meses >= 9)
                    e.CellStyle.BackColor = Color.Green;
                else if (meses == 8 || meses == 7)
                    e.CellStyle.BackColor = Color.YellowGreen;
                else if (meses == 6 || meses == 5 || meses == 4)
                    e.CellStyle.BackColor = Color.Yellow;
                else if (meses == 3)
                    e.CellStyle.BackColor = Color.Orange;
                else if (meses == 2)
                    e.CellStyle.BackColor = Color.OrangeRed;
                else if (meses <= 1)
                    e.CellStyle.BackColor = Color.Red;

                // Cambiar color de texto si no se distingue
                if (e.CellStyle.BackColor == Color.Red || e.CellStyle.BackColor == Color.OrangeRed)
                {
                    e.CellStyle.ForeColor = Color.White;
                    e.CellStyle.Font = new Font(dgvproductos.Font, FontStyle.Bold);
                }
            }

        }
    }
}
