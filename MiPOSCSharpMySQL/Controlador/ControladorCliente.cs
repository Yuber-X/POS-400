using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace MiPOSCSharpMySQL.Controlador
{
    internal class ControladorCliente
    {
        public void MostrarClientes(DataGridView tablaTotalClientes)
        {

            Configuracion.CConexion objetoConexion = new Configuracion.CConexion();
            Modelos.ModeloCliente objetoCliente = new Modelos.ModeloCliente();

            DataTable modelo = new DataTable();

            modelo.Columns.Add("ID", typeof(long));
            modelo.Columns.Add("Nombres", typeof(string));
            modelo.Columns.Add("Ap.Paterno", typeof(string));
            modelo.Columns.Add("Ap.Materno", typeof(string));

            tablaTotalClientes.DataSource = modelo;

            string sql = "select idcliente, nombres, appaterno, appmaterno from cliente;";

            try
            {
                MySqlConnection conexion = objetoConexion.estableceConexion();

                MySqlCommand comando = new MySqlCommand(sql, conexion);

                MySqlDataAdapter adaptador = new MySqlDataAdapter(comando);

                DataSet ds = new DataSet();

                adaptador.Fill(ds);

                DataTable dt = ds.Tables[0];

                foreach (DataRow row in dt.Rows)
                {
                    objetoCliente.IdCliente = Convert.ToInt64(row["idCliente"]);
                    objetoCliente.Nombre = row["nombres"].ToString();
                    objetoCliente.ApPaterno = row["appaterno"].ToString();
                    objetoCliente.ApMaterno = row["appmaterno"].ToString();

                    modelo.Rows.Add(objetoCliente.IdCliente,objetoCliente.Nombre,objetoCliente.ApPaterno, objetoCliente.ApMaterno);
                }

                tablaTotalClientes.DataSource= modelo;
            }
            catch (Exception e)
            {
                MessageBox.Show("Error al mostrar Datos" + e.ToString());
            }
            finally
            {
                objetoConexion.CerrarConexion();
            }
        }
  
        public void AgregarCliente(TextBox nombre, TextBox appaterno, TextBox apmaterno)
        {
            Configuracion.CConexion objetoConexion = new Configuracion.CConexion();
            Modelos.ModeloCliente objetoCliente = new Modelos.ModeloCliente();

            string consulta = "insert into cliente (nombres, appaterno, appmaterno) values (@nombres, @appaterno, @appmaterno);";

            try
            {
                objetoCliente.Nombre = nombre.Text;
                objetoCliente.ApPaterno = appaterno.Text;
                objetoCliente.ApMaterno = apmaterno.Text;

                MySqlConnection conexion = objetoConexion.estableceConexion();

                MySqlCommand comando = new MySqlCommand(consulta, conexion);

                comando.Parameters.AddWithValue("@nombres", objetoCliente.Nombre);
                comando.Parameters.AddWithValue("@appaterno", objetoCliente.ApPaterno);
                comando.Parameters.AddWithValue("@appmaterno", objetoCliente.ApMaterno);

                comando.ExecuteNonQuery();

                MessageBox.Show("Sé Guardo Correctamente");

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar | Error: " + ex.ToString());
            }
            finally
            {
                objetoConexion.CerrarConexion();
            }
        }

        public void Seleccionar(DataGridView totalClientes , TextBox id, TextBox nombre, TextBox appaterno, TextBox apmaterno)
        {
            int fila = totalClientes.CurrentRow.Index;

            try
            {
                if (fila >= 0)
                {
                    id.Text = totalClientes.Rows[fila].Cells[0].Value.ToString();
                    nombre.Text = totalClientes.Rows[fila].Cells[1].Value.ToString();
                    appaterno.Text = totalClientes.Rows[fila].Cells[2].Value.ToString();
                    apmaterno.Text = totalClientes.Rows[fila].Cells[3].Value.ToString();


                }
            }
            catch(Exception e)
            {
                MessageBox.Show("Error inesperado: " + e.ToString());
            }
        }

        public void ModificarCliente(TextBox id, TextBox nombre, TextBox appaterno, TextBox apmaterno)
        {
            Configuracion.CConexion objetoConexion = new Configuracion.CConexion();
            Modelos.ModeloCliente objetoCliente = new Modelos.ModeloCliente();

            string consulta = "UPDATE cliente SET cliente.nombres = @nombres, cliente.appaterno = @appaterno, cliente.appmaterno = @appmaterno where cliente.idcliente = @id ;";

            try
            {
                objetoCliente.IdCliente = long.Parse(id.Text);
                objetoCliente.Nombre = nombre.Text;
                objetoCliente.ApPaterno = appaterno.Text;
                objetoCliente.ApMaterno = apmaterno.Text;

                MySqlConnection conexion = objetoConexion.estableceConexion();

                MySqlCommand comando = new MySqlCommand(consulta, conexion);

                comando.Parameters.AddWithValue("@id", objetoCliente.IdCliente);
                comando.Parameters.AddWithValue("@nombres", objetoCliente.Nombre);
                comando.Parameters.AddWithValue("@appaterno", objetoCliente.ApPaterno);
                comando.Parameters.AddWithValue("@appmaterno", objetoCliente.ApMaterno);

                comando.ExecuteNonQuery();

                MessageBox.Show("Se Modifico correctamente");

            }
            catch(Exception e)
            {
                MessageBox.Show("Error al Modificar los datos: " + e);
            }
            finally
            {
                objetoConexion.CerrarConexion();
            }

        }

        public void LimpiarCampos(TextBox id, TextBox nombre , TextBox appaterno, TextBox apmaterno)
        {
            id.Text = "";
            nombre.Text = "";
            appaterno.Text = "";
            apmaterno.Text = "";
        }

        public void EliminarCliente(TextBox id)
        {
            Configuracion.CConexion objetoConexion = new Configuracion.CConexion();
            Modelos.ModeloCliente objetoCliente = new Modelos.ModeloCliente();

            string consulta = "delete from cliente where cliente.idCliente = @id;";

            try
            {
                objetoCliente.IdCliente = long.Parse(id.Text);


                MySqlConnection conexion = objetoConexion.estableceConexion();

                MySqlCommand comando = new MySqlCommand(consulta, conexion);



                comando.Parameters.AddWithValue("@id", objetoCliente.IdCliente);

                comando.ExecuteNonQuery();

                MessageBox.Show("Se eliminó Correctamente");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eleminar con los datos: " + ex.Message);
            }
            finally
            {
                objetoConexion.CerrarConexion();
            }


        }
    }
}


