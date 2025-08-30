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
    public partial class FormInicioSesion: Form
    {


        public FormInicioSesion()
        {
            InitializeComponent();
        }

        private void btnCrear_Click(object sender, EventArgs e)
        {

        }
        private void InicioSesion_Load(object sender, EventArgs e)
        {

        }
        // -------------------------------------------------------------------------------------------

        private void btnAcceder_Click(object sender, EventArgs e)
        {
            string usuario = txtUsuario.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Por favor complete usuario y contraseña.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Configuracion.CConexion conexion = new Configuracion.CConexion();
                using (MySqlConnection con = conexion.estableceConexion())
                {
                    string sql = @"SELECT u.idUsuario, u.nombreUsuario, u.password, r.nombreRol
                           FROM usuario u
                           JOIN rol r ON u.fkRol = r.idRol
                           WHERE u.nombreUsuario = @usuario
                             AND u.password = @password;";

                    MySqlCommand cmd = new MySqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@usuario", usuario);
                    cmd.Parameters.AddWithValue("@password", password);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string nombreUsuario = reader["nombreUsuario"].ToString();
                            string rol = reader["nombreRol"].ToString();

                            // Abrimos el POS con el usuario
                            Form1 formPrincipal = new Form1(nombreUsuario, rol);
                            formPrincipal.Show();
                            this.Hide();
                        }
                        else
                        {
                            MessageBox.Show("Usuario o contraseña incorrectos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error en login: " + ex.Message);
            }
        }

        private void lblAyuda_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("Debe llamar a su administrador. \nSi no se puede, contacte al desarrollador del POS: Yuber Santana Lizardo - 849-438-0242",
                   "Ayuda", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
             MessageBox.Show("La creación de usuarios solo puede realizarla el Administrador. \n" +
             "Por favor contacte a su supervisor o administrador.",
             "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
