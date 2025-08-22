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
    public partial class FormProductos : Form
    {
        public FormProductos()
        {
            InitializeComponent();
            Controlador.ControladorProducto objetoProducto = new Controlador.ControladorProducto();
            objetoProducto.MostrarProductos(dgvproductos);
        }

        private void dgvproductos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }
        private void txtid_TextChanged(object sender, EventArgs e)
        {

        }
        private void FormProductos_Load(object sender, EventArgs e)
        {

        }

        /*------------------------------------------------------------------------------------------------------*/

        private void btneliminar_Click(object sender, EventArgs e)
        {
            Controlador.ControladorProducto objetoProducto = new Controlador.ControladorProducto();
            objetoProducto.EliminarProducto(txtid);
            objetoProducto.MostrarProductos(dgvproductos);
            objetoProducto.LimpiarCampos(txtid, txtnombreproducto, txtprecio, txtstock,txtdescripcion, dtpFechaCaducidad);
        }

        private void btnmodificar_Click(object sender, EventArgs e)
        {
            Controlador.ControladorProducto objetoProducto = new Controlador.ControladorProducto();
            objetoProducto.ModificarProducto(txtid, txtnombreproducto, txtprecio, txtstock, txtdescripcion, dtpFechaCaducidad);
            objetoProducto.MostrarProductos(dgvproductos);
            objetoProducto.LimpiarCampos(txtid, txtnombreproducto, txtprecio,txtstock, txtdescripcion, dtpFechaCaducidad);
        }

        private void btnlimpiar_Click(object sender, EventArgs e)
        {
            Controlador.ControladorProducto objetoProducto = new Controlador.ControladorProducto();
            objetoProducto.LimpiarCampos(txtid, txtnombreproducto, txtprecio, txtstock, txtdescripcion, dtpFechaCaducidad);
        }

        private void btnguardar_Click(object sender, EventArgs e)
        {
            Controlador.ControladorProducto objetoProducto = new Controlador.ControladorProducto();
            objetoProducto.AgregarProducto(txtid,txtnombreproducto, txtprecio, txtstock,txtdescripcion, dtpFechaCaducidad);
            objetoProducto.MostrarProductos(dgvproductos);
            objetoProducto.LimpiarCampos(txtid, txtnombreproducto, txtprecio, txtstock, txtdescripcion, dtpFechaCaducidad);
        }

        private void dgvproductos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            Controlador.ControladorProducto objetoProducto = new Controlador.ControladorProducto();
            objetoProducto.SeleccionarProducto(dgvproductos, txtid, txtnombreproducto, txtprecio, txtstock,txtdescripcion, dtpFechaCaducidad);
        }

    }
}
