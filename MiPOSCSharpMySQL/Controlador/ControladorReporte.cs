using MySql.Data.MySqlClient;
using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using System.Data.SqlClient;

namespace MiPOSCSharpMySQL.Controlador
{
    internal class ControladorReporte
    {
        Configuracion.CConexion objetoConexion = new Configuracion.CConexion();
        private Bitmap facturaImagen;

     /* public void MostrarDatosFactura(TextBox numeroFactura, Label numeroFacturaEncontrado, Label fechaFacturaEncontrado,
                                        Label nombreClienteEncontrado, Label appaternoEncontrado, Label apmaternoEncontrado) 
        {

            Configuracion.CConexion objetoConexion = new Configuracion.CConexion();

            string consulta = "select factura.idfactura, factura.fechaFactura, cliente.nombres, cliente.appaterno, cliente.appmaterno from factura INNER JOIN cliente ON cliente.idCliente = factura.fkcliente WHERE factura.idFactura = @idfactura;";

            try
            {
                MySqlConnection conexion = objetoConexion.estableceConexion();

                MySqlCommand comando = new MySqlCommand(consulta, conexion);
                comando.Parameters.AddWithValue("@idFactura", long.Parse(numeroFactura.Text));

                MySqlDataReader reader = comando.ExecuteReader();

                if (reader.Read())
                {
                    numeroFacturaEncontrado.Text = reader["idfactura"].ToString();
                    fechaFacturaEncontrado.Text = DateTime.Parse(reader["fechaFactura"].ToString()).ToString("dd-MM-yyyy");
                    nombreClienteEncontrado.Text = reader["nombres"].ToString();
                    appaternoEncontrado.Text = reader["appaterno"].ToString();
                    apmaternoEncontrado.Text = reader["appmaterno"].ToString();
                }
                else
                {
                    numeroFacturaEncontrado.Text = "-------";
                    fechaFacturaEncontrado.Text = "-------";
                    nombreClienteEncontrado.Text = "-------";
                    appaternoEncontrado.Text = "-------";
                    apmaternoEncontrado.Text = "-------";

                    MessageBox.Show("No se encontró la factura");
                }
            }
            catch (Exception e)
            {

                MessageBox.Show("Error al Mostrar Factura: " + e.ToString());

            }
            finally
            {
                objetoConexion.CerrarConexion();
            }
        } */
        public void MostrarVentaFactura(TextBox numeroFactura, DataGridView tablaTotalProductos, Label iva, Label total)
        {

            Configuracion.CConexion objetoConexion = new Configuracion.CConexion();


            DataTable modelo = new DataTable();

            modelo.Columns.Add("Producto", typeof(string));
            modelo.Columns.Add("Cantidad", typeof(string));
            modelo.Columns.Add("Precio Venta", typeof(double));
            modelo.Columns.Add("Subtotal", typeof(int));

            NumberFormatInfo formato = new NumberFormatInfo();
            formato.NumberDecimalDigits = 2;

            try
            {
                string sql = "select producto.nombre, detalle.cantidad, detalle.precioventa from detalle INNER JOIN factura ON factura.idfactura = detalle.fkfactura INNER JOIN producto ON producto.idproducto = detalle.fkproducto WHERE factura.idfactura = @idfactura;";
                
                MySqlConnection conexion = objetoConexion.estableceConexion();

                MySqlCommand comando = new MySqlCommand(sql, conexion);
                comando.Parameters.AddWithValue("@idfactura", int.Parse(numeroFactura.Text));

                MySqlDataReader rs = comando.ExecuteReader();

                double totalFactura = 0.0;
                double valorIVA = 0.18;

                while (rs.Read())
                {
                    string nombreProducto = rs["nombre"].ToString();
                    int cantidad = rs.GetInt32("cantidad");
                    double precioVenta = rs.GetDouble("precioVenta");

                    double subtotal = cantidad * precioVenta; 

                    totalFactura += subtotal;

                    modelo.Rows.Add(nombreProducto, cantidad, precioVenta, subtotal);
                }

                tablaTotalProductos.DataSource = modelo;


                double totalIVA = totalFactura * valorIVA;

                iva.Text = totalIVA.ToString("N", formato);
                total.Text = totalFactura.ToString("N", formato);

            }
            catch (Exception e)
            {
                MessageBox.Show ("Error al mostrar Datos: " + e.ToString());
            }
            finally
            {
                objetoConexion.CerrarConexion();
            }
        }

        /*-----------------------------------------------------------------------------------------------------------------------------------*/

        public void MostrarVentaPorFecha(DateTimePicker desde, DateTimePicker hasta, DataGridView tablaVenta, Label totalGenaral)
        {

            Configuracion.CConexion objetoConexion = new Configuracion.CConexion();


            DataTable modelo = new DataTable();

            modelo.Columns.Add("ID.Factura", typeof(long));
            modelo.Columns.Add("FechaFactura", typeof(DateTime));
            modelo.Columns.Add("N.Producto", typeof(string));
            modelo.Columns.Add("Cantidad", typeof(int));
            modelo.Columns.Add("PrecioVenta", typeof(double));
            modelo.Columns.Add("Subtotal", typeof(double));


            tablaVenta.DataSource = modelo;

            // \r\n

            NumberFormatInfo formato = new NumberFormatInfo();
            formato.NumberDecimalDigits = 2;

            try
            {
                string sql = "SELECT factura.idfactura, factura.fechaFactura, producto.nombre, detalle.cantidad, detalle.precioventa FROM detalle INNER JOIN factura ON factura.idfactura = detalle.fkfactura INNER JOIN producto ON producto.idproducto = detalle.fkproducto WHERE factura.fechaFactura BETWEEN @fechadesde AND @fechahasta;";

                MySqlConnection conexion = objetoConexion.estableceConexion();

                // Verificar conexión
                if (conexion.State == ConnectionState.Closed)
                {
                    MessageBox.Show("Error: La conexión no se estableció correctamente.");
                    return;
                }

                MySqlCommand comando = new MySqlCommand(sql, conexion);

                comando.Parameters.AddWithValue("@fechadesde", desde.Value.ToString("yyyy-MM-dd"));
                comando.Parameters.AddWithValue("@fechahasta", hasta.Value.ToString("yyyy-MM-dd"));


                MySqlDataReader rs = comando.ExecuteReader();

                double totalFactura = 0.0;
 
                while (rs.Read())
                {

                    long idFactura = rs.GetInt64("idfactura");
                    DateTime fechaFactura = rs.GetDateTime("fechaFactura");
                    string nombreProducto = rs.GetString("nombre");
                    int cantidad = rs.GetInt32("cantidad");
                    double precioVenta = rs.GetDouble("precioVenta");
                    double subtotal = cantidad * precioVenta;

                    totalFactura += subtotal;

                    modelo.Rows.Add(idFactura, fechaFactura,nombreProducto, cantidad, precioVenta, subtotal);
                }

                rs.Close();

                tablaVenta.DataSource = null;
                tablaVenta.DataSource = modelo;
                tablaVenta.Refresh();

                totalGenaral.Text = totalFactura.ToString("N", formato);

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

        /*-----------------------------------------------------------------------------------------------------------------------------------*/

        public DataTable ObtenerFactura(long numeroFactura)
        {
            Configuracion.CConexion objetoConexion = new Configuracion.CConexion();

            string consulta = "SELECT factura.idfactura, factura.fechaFactura, factura.metodoPago , cliente.nombres, cliente.appaterno, cliente.appmaterno " +
                              "FROM factura INNER JOIN cliente ON cliente.idcliente = factura.fkCliente " +
                              "WHERE factura.idfactura = @idFactura;";

            try
            {
                using (MySqlConnection conexion = objetoConexion.estableceConexion())
                using (MySqlCommand comando = new MySqlCommand(consulta, conexion))
                {
                    comando.Parameters.AddWithValue("@idFactura", numeroFactura);
                    using (MySqlDataAdapter adaptador = new MySqlDataAdapter(comando))
                    {
                        DataTable tablaFactura = new DataTable();
                        adaptador.Fill(tablaFactura);
                        return tablaFactura;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener factura: " + ex.Message);
                return null;
            }
        }

        public void ImprimirFactura(long idFactura)
        {
            DataTable datosFactura = ObtenerDatosFactura(idFactura);
            if (datosFactura.Rows.Count > 0)
            {
                GenerarImagenFactura(datosFactura);
                PrintDocument pd = new PrintDocument();
                pd.PrintPage += new PrintPageEventHandler(PrintFactura);
                pd.Print();
            }
            else
            {
                Console.WriteLine("No se encontraron datos para la factura.");
            }
        }

        private DataTable ObtenerDatosFactura(long idFactura)
        {
            DataTable dt = new DataTable();
            using (MySqlConnection con = objetoConexion.estableceConexion()) 
            {
                string query = @"SELECT f.idFactura, c.nombres, c.appaterno, c.appmaterno, p.nombre, d.cantidad, d.precioVenta, 
                            (SELECT SUM(d.cantidad * d.precioVenta) FROM detalle d WHERE d.fkFactura = f.idFactura) AS TotalFinal
                            FROM factura f
                            JOIN cliente c ON f.fkCliente = c.idCliente
                            JOIN detalle d ON f.idFactura = d.fkFactura
                            JOIN producto p ON d.fkProducto = p.idProducto
                            WHERE f.idFactura = @idFactura";
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@idFactura", idFactura);
                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            return dt;
        }

        private void GenerarImagenFactura(DataTable datosFactura)
        {
            int width = 400;
            int height = 200 + (datosFactura.Rows.Count * 20);
            facturaImagen = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(facturaImagen))
            {
                g.Clear(Color.White);
                Font font = new Font("Arial", 10);
                Brush brush = Brushes.Black;

                g.DrawString("Factura ID: " + datosFactura.Rows[0]["idFactura"].ToString(), font, brush, 10, 10);
                g.DrawString("Cliente: " + datosFactura.Rows[0]["nombres"] + " " + datosFactura.Rows[0]["appaterno"] + " " + datosFactura.Rows[0]["appmaterno"], font, brush, 10, 30);

                int y = 50;
                g.DrawString("Productos:", font, brush, 10, y);
                y += 20;
                foreach (DataRow row in datosFactura.Rows)
                {
                    g.DrawString(row["nombre"] + " - Cant: " + row["cantidad"] + " - Precio: " + row["precioVenta"], font, brush, 10, y);
                    y += 20;
                }
                g.DrawString("Total: $" + datosFactura.Rows[0]["TotalFinal"].ToString(), font, brush, 10, y + 10);
            }
        }

        private void PrintFactura(object sender, PrintPageEventArgs e)
        {
            if (facturaImagen != null)
            {
                e.Graphics.DrawImage(facturaImagen, 0, 0);
            }
        }
    }
}
