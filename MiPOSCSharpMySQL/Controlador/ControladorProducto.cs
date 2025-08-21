using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiPOSCSharpMySQL.Controlador
{
    internal class ControladorProducto
    {
        public void MostrarProductos(DataGridView tablaTotalProductos)
        {

            Configuracion.CConexion objetoConexion = new Configuracion.CConexion();
            Modelos.ModeloProducto objetoProducto = new Modelos.ModeloProducto();

            DataTable modelo = new DataTable();

            modelo.Columns.Add("id", typeof(long));
            modelo.Columns.Add("Nombre", typeof(string));
            modelo.Columns.Add("Precio", typeof(double));
            modelo.Columns.Add("Cantidad", typeof(int));
            modelo.Columns.Add("Descripcion", typeof(string));
            modelo.Columns.Add("FechaCaducidad", typeof(DateTime));


            tablaTotalProductos.DataSource = modelo;

            string sql = "select idProducto, nombre, precioProducto, stock, descripcionProducto, fechaCaducidad from producto;";

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
                    objetoProducto.IdProducto = Convert.ToInt64(row["IdProducto"]);
                    objetoProducto.NombreProducto = row["Nombre"].ToString();
                    objetoProducto.PrecioProducto = Convert.ToDouble(row["precioProducto"].ToString());
                    objetoProducto.StockProducto = Convert.ToInt32(row["Stock"].ToString());
                    objetoProducto.Descripcion = row["descripcionProducto"].ToString();

                    if (row.IsNull("fechaCaducidad"))
                        objetoProducto.FechaCaducidad = Convert.ToDateTime()

                    modelo.Rows.Add(
                        objetoProducto.IdProducto, 
                        objetoProducto.NombreProducto, 
                        objetoProducto.PrecioProducto, 
                        objetoProducto.StockProducto, 
                        objetoProducto.Descripcion,
                        fechaParaTabla
                        );
                }

                tablaTotalProductos.DataSource = modelo;
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

        public void AgregarProducto(TextBox idProducto, TextBox nombre, TextBox precioProducto, TextBox stock, TextBox descripcion)
        {
            Configuracion.CConexion objetoConexion = new Configuracion.CConexion();
            Modelos.ModeloProducto objetoProducto = new Modelos.ModeloProducto();

            string consulta = "insert into producto( idProducto, nombre, precioProducto, stock, descripcionProducto) values (@idProducto,@nombre, @precioProducto, @stock,@descripcionProducto);";

            try
            {
                objetoProducto.IdProducto = long.Parse(idProducto.Text);
                objetoProducto.NombreProducto = nombre.Text;
                objetoProducto.PrecioProducto = Double.Parse(precioProducto.Text);
                objetoProducto.StockProducto = int.Parse(stock.Text);
                objetoProducto.Descripcion = descripcion.Text;

                MySqlConnection conexion = objetoConexion.estableceConexion();

                MySqlCommand comando = new MySqlCommand(consulta, conexion);

                comando.Parameters.AddWithValue("@idProducto", objetoProducto.IdProducto);
                comando.Parameters.AddWithValue("@nombre", objetoProducto.NombreProducto);
                comando.Parameters.AddWithValue("@precioProducto", objetoProducto.PrecioProducto);
                comando.Parameters.AddWithValue("@stock", objetoProducto.StockProducto);
                comando.Parameters.AddWithValue("@descripcionProducto", objetoProducto.Descripcion);

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

        public void LimpiarCampos(TextBox id, TextBox nombre, TextBox precio, TextBox stock, TextBox descripcion)
        {
            id.Text = "";
            nombre.Text = "";
            precio.Text = "";
            stock.Text = "";
            descripcion.Text = "";
        }

        public void EliminarProducto(TextBox id)
        {
            Configuracion.CConexion objetoConexion = new Configuracion.CConexion();
            Modelos.ModeloProducto objetoProducto = new Modelos.ModeloProducto();

            string consulta = "delete from producto where producto.idProducto = @id;";

            try
            {
                objetoProducto.IdProducto = long.Parse(id.Text);


                MySqlConnection conexion = objetoConexion.estableceConexion();

                MySqlCommand comando = new MySqlCommand(consulta, conexion);



                comando.Parameters.AddWithValue("@id", objetoProducto.IdProducto);

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

        public void ModificarProducto(TextBox id, TextBox nombre, TextBox precio, TextBox stock, TextBox descripcion)
        {
            Configuracion.CConexion objetoConexion = new Configuracion.CConexion();
            Modelos.ModeloProducto objetoProducto = new Modelos.ModeloProducto();

            string consulta = "UPDATE producto SET producto.idProducto = @idProducto , producto.nombre = @nombre, producto.precioProducto = @precioProducto, producto.stock = @stock, producto.descripcionProducto = @descripcionProducto where producto.idProducto = @idProducto";

            try
            {
                objetoProducto.IdProducto = long.Parse(id.Text);
                objetoProducto.NombreProducto = nombre.Text;
                objetoProducto.PrecioProducto = Double.Parse(precio.Text);
                objetoProducto.StockProducto = int.Parse(stock.Text);
                objetoProducto.Descripcion = descripcion.Text;

                MySqlConnection conexion = objetoConexion.estableceConexion();

                MySqlCommand comando = new MySqlCommand(consulta, conexion);


                comando.Parameters.AddWithValue("@idProducto", objetoProducto.IdProducto);
                comando.Parameters.AddWithValue("@nombre", objetoProducto.NombreProducto);
                comando.Parameters.AddWithValue("@precioProducto", objetoProducto.PrecioProducto);
                comando.Parameters.AddWithValue("@stock", objetoProducto.StockProducto);
                comando.Parameters.AddWithValue("@descripcionProducto", objetoProducto.Descripcion);

                comando.ExecuteNonQuery();

                MessageBox.Show("Se Modifico correctamente");

            }
            catch (Exception e)
            {
                MessageBox.Show("Error al Modificar los datos: " + e);
            }
            finally
            {
                objetoConexion.CerrarConexion();
            }

        } 

        public void SeleccionarProducto(DataGridView totalProducto, TextBox id, TextBox nombre, TextBox precio, TextBox stock, TextBox descripcion)
        {
            int fila = totalProducto.CurrentRow.Index;

            try
            {
                if (fila >= 0)
                {
                    id.Text = totalProducto.Rows[fila].Cells[0].Value.ToString();
                    nombre.Text = totalProducto.Rows[fila].Cells[1].Value.ToString();
                    precio.Text = totalProducto.Rows[fila].Cells[2].Value.ToString();
                    stock.Text = totalProducto.Rows[fila].Cells[3].Value.ToString();
                    descripcion.Text = totalProducto.Rows[fila].Cells[4].Value.ToString();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error inesperado: " + e.ToString());
            }
        }

    }
}
