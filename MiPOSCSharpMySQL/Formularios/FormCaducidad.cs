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
        }

        private void FormCaducidad_Load(object sender, EventArgs e)
        {

        }
        private void dgvproductos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

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
    }
}
