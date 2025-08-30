using MiPOSCSharpMySQL.Formularios;
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

namespace MiPOSCSharpMySQL
{
    public partial class Form1 : Form
    {

        private string usuarioLogueado;
        private string rolLogueado;

        public Form1(string usuario, string rol)
        {
            InitializeComponent();

            usuarioLogueado = usuario;
            rolLogueado = rol;

            Configuracion.CConexion objetoConexcion = new Configuracion.CConexion();
            objetoConexcion.estableceConexion();
            

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            AplicarPermisos();
        }

        private void SetAllMenuItemsEnabled(ToolStripItemCollection items, bool enabled)
        {
            foreach (ToolStripItem it in items)
            {
                if (it is ToolStripMenuItem mi)
                {
                    mi.Enabled = enabled;
                    if (mi.HasDropDownItems)
                        SetAllMenuItemsEnabled(mi.DropDownItems, enabled);
                }
            }
        }

        private void AplicarPermisos()
        {
            // 1) Apaga todo
            venderToolStripMenuItem.Enabled = false;
            clientesToolStripMenuItem.Enabled = false;
            productoToolStripMenuItem.Enabled = false;
            caducidadToolStripMenuItem.Enabled = false;
            buscarComprobanteToolStripMenuItem.Enabled = false;
            cuadreToolStripMenuItem.Enabled = false;
            registroToolStripMenuItem.Enabled = false;
            almacenToolStripMenuItem.Enabled = false;
            adminToolStripMenuItem.Enabled = false;

            // 2) Si es ADMIN, enciende todo y sal
            if (rolLogueado == "ADMIN")
            {
                SetAllMenuItemsEnabled(menuStrip1.Items, true);
                return;
            }

            // 3) Cargar permisos del usuario desde BD
            var permisos = PermisosUsuarioPorClave(usuarioLogueado); // HashSet<string> con 'formVentas', etc.

            // 4) Mapa clave -> item
            var mapa = new Dictionary<string, ToolStripMenuItem>()
    {
        { "formVentas", venderToolStripMenuItem },
        { "formClientes", clientesToolStripMenuItem },
        { "formProductos", productoToolStripMenuItem },
        { "formCaducidad", caducidadToolStripMenuItem },
        { "formBuscarComprobante", buscarComprobanteToolStripMenuItem },
        { "formCuadre", cuadreToolStripMenuItem },
        { "formRegistroPorFecha", registroToolStripMenuItem },
        { "formAlmacen", almacenToolStripMenuItem },
        { "formAdminMode", adminToolStripMenuItem },
    };

            foreach (var clave in permisos)
                if (mapa.TryGetValue(clave, out var item))
                    item.Enabled = true;
        }

        private HashSet<string> PermisosUsuarioPorClave(string nombreUsuario)
        {
            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            try
            {
                var con = new Configuracion.CConexion();
                using (var cx = con.estableceConexion())
                {
                    string sql = @"
                    SELECT p.claveForm
                    FROM usuario u
                    JOIN usuario_permiso up ON up.fkUsuario = u.idUsuario
                    JOIN permiso p ON p.idPermiso = up.fkPermiso
                    WHERE u.nombreUsuario = @u";
                    var cmd = new MySqlCommand(sql, cx);
                    cmd.Parameters.AddWithValue("@u", nombreUsuario);
                    using (var rd = cmd.ExecuteReader())
                        while (rd.Read()) set.Add(rd.GetString(0));
                }
            }
            catch { /* log opcional */ }
            return set;
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
        private void adminToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormAdminMode mode = new FormAdminMode();
            mode.Show();

        }
    }
}
