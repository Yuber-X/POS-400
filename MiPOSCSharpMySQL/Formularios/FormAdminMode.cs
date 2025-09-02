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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace MiPOSCSharpMySQL.Formularios
{
    public partial class FormAdminMode : Form
    {
        private bool _cargandoPermisos = false;

        public FormAdminMode()
        {
            InitializeComponent();
            txtId.ReadOnly = true;
        }


        //--------------------------------------------------------------------------------------

        private void FormAdminMode_Load(object sender, EventArgs e)
        {
            Controlador.ControladorUsuario ctrlUsuario = new Controlador.ControladorUsuario();
            ctrlUsuario.MostrarUsuarios(dgvUsuarios);

            // Cargar permisos
            DataTable cat = ctrlUsuario.ListarPermisos();
            clPermisos.DataSource = cat;
            clPermisos.DisplayMember = "nombrePermiso";
            clPermisos.ValueMember = "idPermiso";

            // 🔹 Cargar roles
            var con = new Configuracion.CConexion();
            using (var cx = con.estableceConexion())
            {
                string sql = "SELECT idRol, nombreRol FROM rol";
                var da = new MySqlDataAdapter(sql, cx);
                var dt = new DataTable();
                da.Fill(dt);

                cbRol.DataSource = dt;
                cbRol.DisplayMember = "nombreRol";
                cbRol.ValueMember = "idRol";
            }

            cbRol.SelectedIndex = -1;
        }

        private void btnguardar_Click(object sender, EventArgs e)
        {
            Controlador.ControladorUsuario ctrlUsuario = new Controlador.ControladorUsuario();
            ctrlUsuario.AgregarUsuario(
                txtNombres.Text,
                txtApellido.Text,
                txtUser.Text,
                txtContra.Text,
                Convert.ToInt32(cbRol.SelectedValue)
            );
            ctrlUsuario.MostrarUsuarios(dgvUsuarios);
            LimpiarCampos();
        }

        private void btnmodificar_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(txtId.Text)) return;

            int rol = Convert.ToInt32(cbRol.SelectedValue);
            int id = Convert.ToInt32(txtId.Text);


            Controlador.ControladorUsuario ctrlUsuario = new Controlador.ControladorUsuario();
            ctrlUsuario.ModificarUsuario(
                id,
                txtNombres.Text,
                txtApellido.Text,
                txtUser.Text,
                txtContra.Text,
                rol
            );

            // Guardar permisos seleccionados
            List<int> permisosSeleccionados = new List<int>();
            foreach (var item in clPermisos.CheckedItems)
            {
                var drv = (DataRowView)item;
                permisosSeleccionados.Add(Convert.ToInt32(drv["idPermiso"]));
            }
             ctrlUsuario.GuardarPermisos(id, permisosSeleccionados);

            ctrlUsuario.MostrarUsuarios(dgvUsuarios);
            LimpiarCampos();
        }

        private void btneliminar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtId.Text)) return;

            Controlador.ControladorUsuario ctrlUsuario = new Controlador.ControladorUsuario();
            ctrlUsuario.EliminarUsuario(Convert.ToInt64(txtId.Text));
            ctrlUsuario.MostrarUsuarios(dgvUsuarios);
            LimpiarCampos();
        }

        private void btnlimpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }

        private void dgvUsuarios_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            Controlador.ControladorUsuario ctrlUsuario = new Controlador.ControladorUsuario();

            if (e.RowIndex < 0) return;

            var fila = dgvUsuarios.Rows[e.RowIndex];

            txtId.Text = fila.Cells["ID"].Value.ToString();
            txtUser.Text = fila.Cells["Usuario"].Value.ToString();
            txtNombres.Text = fila.Cells["Nombre"].Value.ToString();
            txtApellido.Text = fila.Cells["Apellido"].Value.ToString();
            txtContra.Text = fila.Cells["Contraseña"].Value.ToString();
            cbRol.SelectedValue = fila.Cells["idRol"].Value.ToString();

            btnguardar.Enabled = false;

            PintarPermisosUsuario();

        }

        // METODOS 
        private void LimpiarCampos()
        {
            txtId.Clear();
            txtUser.Clear();
            txtNombres.Clear();
            txtApellido.Clear();
            cbRol.SelectedIndex = -1;
            txtContra.Clear();

            btnguardar.Enabled = true;

            for (int i = 0; i < clPermisos.Items.Count; i++)
            {
                clPermisos.SetItemChecked(i, false);
            }
        }

        private void PintarPermisosUsuario()
        {
            if (string.IsNullOrWhiteSpace(txtId.Text)) return;

            _cargandoPermisos = true;
            try
            {
                long idUser = Convert.ToInt64(txtId.Text);
                var ctrl = new Controlador.ControladorUsuario();
                var setIds = ctrl.PermisosDeUsuarioIds(idUser);

                for (int i = 0; i < clPermisos.Items.Count; i++)
                {
                    var drv = (DataRowView)clPermisos.Items[i];
                    int idPermiso = Convert.ToInt32(drv["idPermiso"]);
                    clPermisos.SetItemChecked(i, setIds.Contains(idPermiso));
                }
            }
            finally
            {
                _cargandoPermisos = false;
            }
        }

        //private void clPermisos_ItemCheck(object sender, ItemCheckEventArgs e)
        //{
        //    if (_cargandoPermisos) return;
        //    if (string.IsNullOrWhiteSpace(txtId.Text)) return;

        //    // Ejecutar después de que el check se aplique visualmente
        //    BeginInvoke(new Action(() =>
        //    {
        //        long idUser = Convert.ToInt64(txtId.Text);
        //        var drv = (DataRowView)clPermisos.Items[e.Index];
        //        int idPermiso = Convert.ToInt32(drv["idPermiso"]);

        //        var ctrl = new Controlador.ControladorUsuario();
        //        if (clPermisos.GetItemChecked(e.Index)) // quedó chequeado
        //            ctrl.AsignarPermiso(idUser, idPermiso);
        //        else // quedó deschequeado
        //            ctrl.RevocarPermiso(idUser, idPermiso);
        //    }));
        //}

    }
}
