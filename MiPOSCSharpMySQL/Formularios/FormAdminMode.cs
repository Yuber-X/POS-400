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
    public partial class FormAdminMode: Form
    {
        public FormAdminMode()
        {
            InitializeComponent();
        }

        private void btneliminar_Click(object sender, EventArgs e)
        {

        }

        private void btnmodificar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtId.Text)) return;

            Configuracion.CConexion conexion = new Configuracion.CConexion();
            using (MySqlConnection con = conexion.estableceConexion())
            {
                string sql = @"UPDATE usuario 
                       SET nombreUsuario=@nombre, password=@contra, fkRol=@rol 
                       WHERE idUsuario=@id";
                MySqlCommand cmd = new MySqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@nombre", txtNombres.Text.Trim());
                cmd.Parameters.AddWithValue("@contra", txtContra.Text.Trim());
                cmd.Parameters.AddWithValue("@rol", textRol.Text.Trim()); 
                cmd.Parameters.AddWithValue("@id", txtId.Text.Trim());

                cmd.ExecuteNonQuery();
                MessageBox.Show("Usuario modificado correctamente.");
                CargarUsuarios();
            }
        }


        private void btnguardar_Click(object sender, EventArgs e)
        {

        }

        private void btnlimpiar_Click(object sender, EventArgs e)
        {

        }

        private void CargarUsuarios()
        {
            Configuracion.CConexion conexion = new Configuracion.CConexion();
            using (MySqlConnection con = conexion.estableceConexion())
            {
                string sql = @"SELECT u.idUsuario, u.nombreUsuario, r.nombreRol 
                       FROM usuario u
                       JOIN rol r ON u.fkRol = r.idRol";
                MySqlDataAdapter da = new MySqlDataAdapter(sql, con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvUsuarios.DataSource = dt;
            }
        }

    }
}
