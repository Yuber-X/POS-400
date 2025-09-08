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
    public partial class FormClientes : Form
    {
        public FormClientes()
        {
            InitializeComponent();

             Controlador.ControladorCliente objetoCliente = new Controlador.ControladorCliente();
             objetoCliente.MostrarClientes(dgvclientes);

             txtid.ReadOnly = true;
        }

        private void btnguardar_Click(object sender, EventArgs e)
        {
            Controlador.ControladorCliente objetoCliente = new Controlador.ControladorCliente();
            objetoCliente.AgregarCliente(txtnombres,txtappaterno,txtapmaterno);
            objetoCliente.MostrarClientes(dgvclientes);
            objetoCliente.LimpiarCampos(txtid, txtnombres, txtappaterno, txtapmaterno);
        }

        private void dgvclientes_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            Controlador.ControladorCliente objetoCliente = new Controlador.ControladorCliente();
            objetoCliente.Seleccionar(dgvclientes, txtid,txtnombres,txtappaterno,txtapmaterno);
            btnguardar.Enabled = false;
        }

        private void btnmodificar_Click(object sender, EventArgs e)
        {
            Controlador.ControladorCliente objetoCliente = new Controlador.ControladorCliente();
            objetoCliente.ModificarCliente(txtid, txtnombres, txtappaterno, txtapmaterno);
            objetoCliente.MostrarClientes(dgvclientes);
            objetoCliente.LimpiarCampos(txtid, txtnombres,txtapmaterno, txtappaterno);
            btnguardar.Enabled = true;
        }

        private void btnlimpiar_Click(object sender, EventArgs e)
        {
            Controlador.ControladorCliente objetoCliente = new Controlador.ControladorCliente();
            objetoCliente.LimpiarCampos(txtid, txtnombres, txtappaterno, txtapmaterno);
            btnguardar.Enabled = true;
        }

        private void btneliminar_Click(object sender, EventArgs e)
        {
            Controlador.ControladorCliente objetoCliente = new Controlador.ControladorCliente();
            objetoCliente.EliminarCliente(txtid);
            objetoCliente.MostrarClientes(dgvclientes);
            objetoCliente.LimpiarCampos(txtid, txtnombres, txtappaterno, txtapmaterno);
            btnguardar.Enabled = true;
        }

        private void txtid_Click(object sender, EventArgs e)
        {
            MessageBox.Show("El ID se Auto-Rellena no es posible modificar.");
        }

        private void txtid_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
