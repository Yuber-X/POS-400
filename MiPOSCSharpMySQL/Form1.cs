using MiPOSCSharpMySQL.Formularios;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiPOSCSharpMySQL
{
    public partial class Form1 : Form
    {
        private string usuarioLogueado;
        private string rolLogueado;

        public Form1(string usuario, string rol)
        {
            InitializeComponent();

            Configuracion.CConexion objetoConexcion = new Configuracion.CConexion();
            objetoConexcion.estableceConexion();
            
            usuarioLogueado = usuario;
            rolLogueado = rol;


        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = $"POS-400 - Usuario: {usuarioLogueado} ({rolLogueado})";

            // Restringir accesos según rol
            if (rolLogueado != "ADMIN")
            {
                // Desactivar menús que no son de usuario normal
                usuariosToolStripMenuItem.Enabled = false;
                almacenToolStripMenuItem.Enabled = false;
                registroToolStripMenuItem.Enabled = false;
                caducidadToolStripMenuItem.Enabled = false;
                adminToolStripMenuItem.Enabled=false;
                productoToolStripMenuItem.Enabled = false;
                cuadreToolStripMenuItem.Enabled=false;
            }
        }
        private void almacenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Formularios.FormProductos objetoFormProductos = new Formularios.FormProductos();
            AbrirFormulario(new FormProductos());
            //objetoFormProductos.Show();
        }
        private void venderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Formularios.FormVentas objetoFormVentas = new Formularios.FormVentas();
            AbrirFormulario(new FormVentas());
             //objetoFormVentas.Show();
        }
        private void registroToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Formularios.FormReportePorFechas objetoFormReportePorFechas = new Formularios.FormReportePorFechas();
            AbrirFormulario(new FormReportePorFechas());
            //objetoFormReportePorFechas.Show();
        }
        private void clientesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Formularios.FormClientes objetoFormClientes = new Formularios.FormClientes();
            AbrirFormulario(new FormClientes());
            //objetoFormClientes.Show();
        }
        private void almacenToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Formularios.FormAlmacen objetoFormAlmacen = new Formularios.FormAlmacen();
            AbrirFormulario(new FormAlmacen());
            //objetoFormAlmacen.Show();
        }
        private void buscarComprobanteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Formularios.FormBuscarComprobante objetoFormBuscarComprobante = new Formularios.FormBuscarComprobante();
            AbrirFormulario(new FormBuscarComprobante());
            //objetoFormBuscarComprobante.Show();
        }
        private void ayudaToolStripMenuItem_Click(object sender, EventArgs e)
        {

            MessageBox.Show("Llama a los siguientes encargados del sistema de ventas: Yuber Ern. Santana Polanco Whatsapp: Yuber Ern. Santana Lizardo Whatsapp: 849-438-0242");

        }
        private void AbrirFormulario(Form formularioHijo)
        {
            // Cerrar el formulario actual si hay uno abierto
                            
            foreach (Form form in this.MdiChildren)
            {
               form.Close();
            }

            // Configurar el formulario hijo

            formularioHijo.MdiParent = this;
         // formularioHijo.Dock = DockStyle.Fill; // Hace que ocupe todo el espacio disponible
            formularioHijo.Show();
        }
        private void caducidadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Formularios.FormCaducidad objetoFormCaducidad = new Formularios.FormCaducidad();
            AbrirFormulario(new FormCaducidad());
            //objetoFormProductos.Show();
        }

        private void usuariosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //RegistrarTiempoSesion();

            // Volver al login
            FormInicioSesion login = new FormInicioSesion();
            login.Show();
            this.Hide();
        }
    }
}
