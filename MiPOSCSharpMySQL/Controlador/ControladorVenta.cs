using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Collections;



namespace MiPOSCSharpMySQL.Controlador
{
    internal class ControladorVenta
    {
        /*LOGICA DE PRODUCTOS*/
        public void BuscarProductos(TextBox nombreProducto, DataGridView tablaTotalProductos)
        {

            Configuracion.CConexion objetoConexion = new Configuracion.CConexion();
            Modelos.ModeloProducto objetoProducto = new Modelos.ModeloProducto();

            DataTable modelo = new DataTable();

            modelo.Columns.Add("Id", typeof(long));
            modelo.Columns.Add("Nombre", typeof(string));
            modelo.Columns.Add("Precio", typeof(double));
            modelo.Columns.Add("Cantidad", typeof(int));


            tablaTotalProductos.DataSource = modelo;


            try
            {
                // Uso Trim() para eliminar cada espacio del string y luego convertirlo en texto.
                string filtro = nombreProducto.Text.Trim();

                if (string.IsNullOrEmpty(filtro))
                {
                    tablaTotalProductos.DataSource = null;
                    return;
                }
                else
                {

                    string sql;

                    // Reviso si el filtro es numerico o textual, asi buscara el producto por nombre o por ID
                    if (long.TryParse(filtro, out long  idBuscado))
                    {
                        sql = "SELECT * FROM producto WHERE CAST(IdProducto AS CHAR) LIKE CONCAT('%', @filtro, '%');"; 
                    }
                    else
                    {
                        sql = "select * from producto where producto.nombre LIKE concat('%', @filtro, '%');";
                    }

                    MySqlConnection conexion = objetoConexion.estableceConexion();

                    MySqlCommand comando = new MySqlCommand(sql, conexion);
                    comando.Parameters.AddWithValue("@filtro", filtro);

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

                        modelo.Rows.Add(objetoProducto.IdProducto, objetoProducto.NombreProducto, objetoProducto.PrecioProducto, objetoProducto.StockProducto);
                    }
                    tablaTotalProductos.DataSource = modelo;

                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error al mostrar Datos: " + e.ToString());
            }
            finally
            {
                objetoConexion.CerrarConexion();
            }
        }
        public void SeleccionarProductoVenta(DataGridView totalProducto, TextBox id, TextBox nombre, TextBox precio, TextBox stock, TextBox precioFinal)
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
                    precioFinal.Text = totalProducto.Rows[fila].Cells[2].Value.ToString();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error inesperado: " + e.ToString());
            }
        }


        /*LOGICA DE CLIENTES*/
        public void BuscarClientes(TextBox nombreCliente, DataGridView tablaTotalClientes)
        {

            Configuracion.CConexion objetoConexion = new Configuracion.CConexion();
            Modelos.ModeloCliente objetoCliente = new Modelos.ModeloCliente();

            DataTable modelo = new DataTable();

            modelo.Columns.Add("Id", typeof(long));
            modelo.Columns.Add("Nombre", typeof(string));
            modelo.Columns.Add("Ap.Paterno", typeof(string));
            modelo.Columns.Add("Ap.Materno", typeof(string));


            tablaTotalClientes.DataSource = modelo;


            try
            {
                if (nombreCliente.Text == "")
                {
                    tablaTotalClientes.DataSource = null;
                }
                else
                {
                    string sql = "select * from cliente where cliente.nombres LIKE concat('%', @nombre, '%');";

                    MySqlConnection conexion = objetoConexion.estableceConexion();

                    MySqlCommand comando = new MySqlCommand(sql, conexion);
                    comando.Parameters.AddWithValue("@nombre", nombreCliente.Text);

                    MySqlDataAdapter adaptador = new MySqlDataAdapter(comando);

                    DataSet ds = new DataSet();

                    adaptador.Fill(ds);

                    DataTable dt = ds.Tables[0];

                    foreach (DataRow row in dt.Rows)
                    {
                        objetoCliente.IdCliente = Convert.ToInt64(row["IdCliente"]);
                        objetoCliente.Nombre = row["nombres"].ToString();
                        objetoCliente.ApPaterno = row["appaterno"].ToString();
                        objetoCliente.ApMaterno = row["appmaterno"].ToString();

                        modelo.Rows.Add(objetoCliente.IdCliente, objetoCliente.Nombre, objetoCliente.ApPaterno, objetoCliente.ApMaterno);
                    }
                    tablaTotalClientes.DataSource = modelo;
                }
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
        public void SeleccionarClienteVenta(DataGridView totalCliente, TextBox id, TextBox nombre, TextBox appaterno, TextBox apmaterno)
        {
            int fila = totalCliente.CurrentRow.Index;

            try
            {
                if (fila >= 0)
                {
                    id.Text = totalCliente.Rows[fila].Cells[0].Value.ToString();
                    nombre.Text = totalCliente.Rows[fila].Cells[1].Value.ToString();
                    appaterno.Text = totalCliente.Rows[fila].Cells[2].Value.ToString();
                    apmaterno.Text = totalCliente.Rows[fila].Cells[3].Value.ToString();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error inesperado: " + e.ToString());
            }
        }


        /*LOGICA ADICIONAL*/
        public void LimpiarCamposVenta(TextBox BuscarCliente, DataGridView tablaCliente , TextBox buscarProducto, DataGridView tablaProducto, 
                                       TextBox selectIdCliente, TextBox selectNombreCliente, TextBox selectAppaterno, TextBox selectApmaterno,
                                       TextBox selectIdProducto, TextBox selectNombreP, TextBox selectPrecioP, TextBox selectStock, 
                                       TextBox precioVentaFinal, TextBox cantidadVenta, DataGridView tablaResumen, Label iva, Label totalPagar){
            BuscarCliente.Text = "";
            tablaCliente.DataSource = null;

            buscarProducto.Text = "";
            tablaProducto.DataSource = null;

            selectIdCliente.Text = "";
            selectNombreCliente.Text = "";
            selectAppaterno.Text = "";
            selectApmaterno.Text = "";

            selectIdProducto.Text = "";
            selectNombreP.Text = "";
            selectPrecioP.Text = "";
            selectStock.Text = "";
            precioVentaFinal.Text = "";
            precioVentaFinal.ReadOnly = true;
            cantidadVenta.Text = "";

            tablaResumen.DataSource = null;
            iva.Text = "------------";
            totalPagar.Text = "------------";

        }


        /*LOGICA DE VENTA*/
        public void PasarProductosVenta(DataGridView tablaResumen, TextBox idProducto, TextBox nombreProducto, TextBox precioProducto, TextBox cantidadProducto, TextBox stock)
        {
            try
            {
                DataTable modelo = (DataTable)tablaResumen.DataSource;

                if (modelo == null)
                {
                    modelo = new DataTable();
                    modelo.Columns.Add("Id", typeof(string));
                    modelo.Columns.Add("NombreProducto", typeof(string));
                    modelo.Columns.Add("PrecioProducto", typeof(double));
                    modelo.Columns.Add("Cantidad", typeof(int));
                    modelo.Columns.Add("Subtotal", typeof(double));

                    tablaResumen.DataSource = modelo;
                }

                int stockDisponible = int.Parse(stock.Text);

                string idProductoTexto = idProducto.Text;

                foreach (DataRow row in modelo.Rows)
                {
                    string idExistente = (string)row["id"];

                    if (idExistente.Equals(idProductoTexto))
                    {
                        MessageBox.Show("El producto ya fue registrado");
                        return;
                    }
                }






                string nProducto = nombreProducto.Text;
                double precioUnitario = double.Parse(precioProducto.Text);
                int cantidad = int.Parse(cantidadProducto.Text);

                if (cantidad > stockDisponible)
                {
                    MessageBox.Show("La cantidad es mayor el stock disponible");
                    return;
                }
                double subtotal = precioUnitario * cantidad;

                modelo.Rows.Add(idProductoTexto, nProducto, precioUnitario, cantidad, subtotal);

            }
            catch (Exception e)
            {
                MessageBox.Show("Error al mostrar Datos: " + e.ToString());
            }
        }
        public void CalcularTotal(DataGridView tablaResumen, Label IVA, Label totalPagar)
        {
            double totalSubtotal = 0;
            double iva = 0.18;
            double totalIva = 0;

            NumberFormatInfo formato = new NumberFormatInfo();
            formato.NumberDecimalDigits = 2;

            foreach (DataGridViewRow row in tablaResumen.Rows)
            {
                if (row.Cells[4].Value != null)
                {
                    totalSubtotal += Convert.ToDouble(row.Cells[4].Value);
                }
            }
            totalIva = iva * totalSubtotal;

            totalPagar.Text = totalSubtotal.ToString("N",formato);
            IVA.Text = totalIva.ToString("N",formato);
        }
        public void EliminarSeleccion(DataGridView tablaResumen)
        {

            try
            {
                int indiceSeleccion = tablaResumen.CurrentRow.Index;

                if (indiceSeleccion >= 0)
                {
                    tablaResumen.Rows.RemoveAt(indiceSeleccion);
                }
                else
                {
                    MessageBox.Show("Seleccione una fila para eliminar");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Hubo un error al eliminar: " + e);
            }
        }

        //public void CrearFactura(TextBox codCliente) 
        //{
        //    Configuracion.CConexion objetoConexion = new Configuracion.CConexion();
        //    Modelos.ModeloCliente objetoCliente = new Modelos.ModeloCliente();

        //    string consulta = "insert into factura (fechaFactura, fkCliente) values (curdate(),@fkCliente);";

        //    try
        //    {
        //        objetoCliente.IdCliente = long.Parse(codCliente.Text);

        //        MySqlConnection conexion = objetoConexion.estableceConexion();

        //        MySqlCommand comando = new MySqlCommand(consulta, conexion);

        //        comando.Parameters.AddWithValue("@fkCliente", objetoCliente.IdCliente);

        //        comando.ExecuteNonQuery();

        //        MessageBox.Show("Factura Creada");
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Error al guardar | Error: " + ex.ToString());
        //    }
        //    finally
        //    {
        //        objetoConexion.CerrarConexion();
        //    }
        //}
        public long CrearFacturaV2(TextBox codCliente)
        {
            Configuracion.CConexion objetoConexion = new Configuracion.CConexion();
            Modelos.ModeloCliente objetoCliente = new Modelos.ModeloCliente();

            string consulta = "INSERT INTO factura (fechaFactura, fkCliente) VALUES (CURDATE(), @fkCliente); SELECT LAST_INSERT_ID();";

            try
            {
                objetoCliente.IdCliente = long.Parse(codCliente.Text);

                using (MySqlConnection conexion = objetoConexion.estableceConexion())
                using (MySqlCommand comando = new MySqlCommand(consulta, conexion))
                {
                    comando.Parameters.AddWithValue("@fkCliente", objetoCliente.IdCliente);
                    long idFactura = Convert.ToInt64(comando.ExecuteScalar()); // Obtenemos el ID de la factura creada
                    return idFactura;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al crear factura: " + ex.Message);
                return -1;
            }
        }

        //public void RealizarVenta(DataGridView tablaResumenVenta)
        //{
        //    Configuracion.CConexion objetoConexion = new Configuracion.CConexion();
            
        //    string consultaDetalle = "insert into detalle (fkfactura, fkproducto, cantidad, precioVenta) values ((select max(idfactura) from factura),@fkproducto,@cantidad,@precioVenta);";
        //    string consultaStock = "update producto set stock = stock - @cantidad where idproducto = @idproducto;";


        //    try
        //    {

        //        MySqlConnection conexion = objetoConexion.estableceConexion();

        //        MySqlCommand comandoDetalle = new MySqlCommand(consultaDetalle, conexion);
        //        MySqlCommand comandoStock = new MySqlCommand(consultaStock, conexion);


        //        foreach (DataGridViewRow row in tablaResumenVenta.Rows)
        //        {
        //            if (row.Cells[0].Value != null)
        //            {
        //                long idProducto = Convert.ToInt64(row.Cells[0].Value);
        //                int cantidad = Convert.ToInt32(row.Cells[3].Value);
        //                double precioVenta = Convert.ToDouble(row.Cells[2].Value);

        //                comandoDetalle.Parameters.Clear();
        //                comandoStock.Parameters.Clear();

        //                comandoDetalle.Parameters.AddWithValue("@fkProducto", idProducto);
        //                comandoDetalle.Parameters.AddWithValue("@cantidad", cantidad);
        //                comandoDetalle.Parameters.AddWithValue("@precioVenta", precioVenta);

        //                comandoDetalle.ExecuteNonQuery();

        //                comandoStock.Parameters.AddWithValue("@cantidad", cantidad);
        //                comandoStock.Parameters.AddWithValue("@idProducto", idProducto);

        //                comandoStock.ExecuteNonQuery();

        //            }
        //        }

        //        MessageBox.Show("Venta Realizada");

        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Error al Vender | Error: " + ex.ToString());
        //    }
        //    finally
        //    {
        //        objetoConexion.CerrarConexion();
        //    }
        //}
        public void RealizarVentaV2(DataGridView tablaResumenVenta, long idFactura)
        {
            if (idFactura == -1) return; // Si hubo un error en la factura, no continuamos

            Configuracion.CConexion objetoConexion = new Configuracion.CConexion();

            string consultaDetalle = "INSERT INTO detalle (fkfactura, fkproducto, cantidad, precioVenta) VALUES (@fkfactura, @fkproducto, @cantidad, @precioVenta);";
            string consultaStock = "UPDATE producto SET stock = stock - @cantidad WHERE idproducto = @idproducto;";

            try
            {
                using (MySqlConnection conexion = objetoConexion.estableceConexion())
                {
                    foreach (DataGridViewRow row in tablaResumenVenta.Rows)
                    {
                        if (row.Cells[0].Value != null)
                        {
                            long idProducto = Convert.ToInt64(row.Cells[0].Value);
                            int cantidad = Convert.ToInt32(row.Cells[3].Value);
                            double precioVenta = Convert.ToDouble(row.Cells[2].Value);

                            using (MySqlCommand comandoDetalle = new MySqlCommand(consultaDetalle, conexion))
                            {
                                comandoDetalle.Parameters.AddWithValue("@fkfactura", idFactura);
                                comandoDetalle.Parameters.AddWithValue("@fkproducto", idProducto);
                                comandoDetalle.Parameters.AddWithValue("@cantidad", cantidad);
                                comandoDetalle.Parameters.AddWithValue("@precioVenta", precioVenta);
                                comandoDetalle.ExecuteNonQuery();
                            }

                            using (MySqlCommand comandoStock = new MySqlCommand(consultaStock, conexion))
                            {
                                comandoStock.Parameters.AddWithValue("@cantidad", cantidad);
                                comandoStock.Parameters.AddWithValue("@idproducto", idProducto);
                                comandoStock.ExecuteNonQuery();
                            }
                        }
                    }
                }

                MessageBox.Show("Venta realizada con éxito.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al realizar la venta: " + ex.Message);
            }
        }
        public void MostrarUltimaFactura(Label ultimaFactura)
        {

            Configuracion.CConexion objetoConexion = new Configuracion.CConexion();

            string consulta = "Select max(idfactura) as ultimaFactura from factura;";

            try
            {
                MySqlConnection conexion = objetoConexion.estableceConexion();

                MySqlCommand comando = new MySqlCommand(consulta, conexion);

                MySqlDataReader reader = comando.ExecuteReader();

                if (reader.Read())
                {
                    ultimaFactura.Text = reader["UltimaFactura"].ToString();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error al mostrar Datos de la Ultima Factura: " + e.ToString());
            }
            finally
            {
                objetoConexion.CerrarConexion();
            }
        }

    }
}
