using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiPOSCSharpMySQL.Configuracion
{
    internal class CConexion
    {
        private MySqlConnection conectar = null;

        private static string usuario = "root";
        private static string contrasenia = "root";
        private static string bd = "bdposv2";
        private static string ip = "localhost";
        private static string puerto = "3306";

        private string cadena = $"Server={ip};Port={puerto};Database={bd};Uid={usuario};Pwd={contrasenia};";

        public MySqlConnection estableceConexion()
        {
            try
            {
                conectar = new MySqlConnection(cadena);
                conectar.Open();
                //MessageBox.Show("Se Conecto Correctamente");
            }
            catch(Exception e) 
            {

                MessageBox.Show("No se Conecto Correctamente FRACASO: " + e.ToString());
            
            }
            return conectar;
        }

        public void CerrarConexion()
        {
            try
            {
                if (conectar != null && conectar.State == System.Data.ConnectionState.Open)
                    
                    conectar.Close();

                //MessageBox.Show("Se cerro Correctamente la Conexion");

            }
            catch (Exception e)
            {
                MessageBox.Show(" No se cerror Correctamente la conexion FRACASO: " + e.ToString());

            }

        }

    }


}
