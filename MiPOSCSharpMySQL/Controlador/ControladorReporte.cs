using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace MiPOSCSharpMySQL.Controlador
{
    internal class ControladorReporte
    {
        Configuracion.CConexion objetoConexion = new Configuracion.CConexion();
        private Bitmap facturaImagen;

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
            modelo.Columns.Add("Efectivo", typeof(string));  // ← nuevo
            modelo.Columns.Add("Cambio", typeof(string));    // ← nuevo



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

        public DataTable ObtenerFactura(long numeroFactura)
        {
            Configuracion.CConexion objetoConexion = new Configuracion.CConexion();

            string consulta = "SELECT factura.idfactura, factura.fechaFactura, factura.metodoPago , cliente.nombres, cliente.telefono, cliente.direccion " +
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
                string query = @"SELECT f.idFactura, c.nombres, c.telefono, c.direccion, p.nombre, d.cantidad, d.precioVenta, 
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
            int width = 300; // ancho estándar ticket (80mm)
            int y = 10;
            facturaImagen = new Bitmap(width, 600 + (datosFactura.Rows.Count * 20));

            using (Graphics g = Graphics.FromImage(facturaImagen))
            {
                g.Clear(Color.White);
                Font font = new Font("Consolas", 9);
                Font fontBold = new Font("Consolas", 9, FontStyle.Bold);
                Brush brush = Brushes.Black;

                // ---------- LOGO COMO MARCA DE AGUA EN EL CENTRO ----------
                try
                {
                    Image logo = Image.FromFile("C:\\Users\\lizar\\Desktop\\Yu\\Code\\PT-Venta-V2\\MiPOSCSharpMySQL\\Resources\\logo.jpgC:\\Users\\lizar\\Desktop\\Yu\\Code\\PT-Venta-V2\\MiPOSCSharpMySQL\\Resources\\logo.jpg"); // ⚠️ pon aquí la ruta correcta del logo

                    // Hacemos el logo transparente
                    ColorMatrix cm = new ColorMatrix();
                    cm.Matrix33 = 0.08f; // Transparencia: 0.05 = muy tenue, 0.2 = más visible
                    ImageAttributes ia = new ImageAttributes();
                    ia.SetColorMatrix(cm, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                    // Posición: centro de la factura
                    int logoSize = 200;
                    int logoX = (width - logoSize) / 2;
                    int logoY = (facturaImagen.Height - logoSize) / 2;

                    g.DrawImage(logo, new Rectangle(logoX, logoY, logoSize, logoSize),
                                0, 0, logo.Width, logo.Height,
                                GraphicsUnit.Pixel, ia);
                }
                catch
                {
                    // si no hay logo, seguimos sin errores
                }

                // ---------- ENCABEZADO ----------
                g.DrawString("BOTI FARMA EL MAMON", fontBold, brush, 10, y); y += 20;
                g.DrawString("RNC: 123456789", font, brush, 10, y); y += 15;
                g.DrawString("Tel: +1 809-353-4924", font, brush, 10, y); y += 15;
                g.DrawString("Direccion: Santo Domingo, RD", font, brush, 10, y); y += 20;

                g.DrawLine(Pens.Black, 0, y, width, y); y += 10;

                // ---------- INFO FACTURA ----------
                g.DrawString($"Factura: {datosFactura.Rows[0]["idFactura"]}", font, brush, 10, y); y += 15;
                g.DrawString($"Cliente: {datosFactura.Rows[0]["nombres"]} {datosFactura.Rows[0]["telefono"]} {datosFactura.Rows[0]["direccion"]}", font, brush, 10, y); y += 15;
                g.DrawString($"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}", font, brush, 10, y); y += 20;

                g.DrawLine(Pens.Black, 0, y, width, y); y += 10;

                // ---------- TABLA PRODUCTOS ----------
                g.DrawString("Cant  Desc         Precio   Subtot", fontBold, brush, 10, y); y += 20;

                double subtotal = 0;
                foreach (DataRow row in datosFactura.Rows)
                {
                    int cantidad = Convert.ToInt32(row["cantidad"]);
                    string nombre = row["nombre"].ToString();
                    double precio = Convert.ToDouble(row["precioVenta"]);
                    double lineaSubtotal = cantidad * precio;
                    subtotal += lineaSubtotal;

                    string linea = $"{cantidad,-4} {nombre,-10} {precio,6:N2} {lineaSubtotal,7:N2}";
                    g.DrawString(linea, font, brush, 10, y);
                    y += 20;
                }

                g.DrawLine(Pens.Black, 0, y, width, y); y += 10;

                // ---------- TOTALES ----------
                double itbis = subtotal * 0.18;
                double total = subtotal + itbis;

                g.DrawString($"Subtotal:   RD$ {subtotal:N2}", font, brush, 10, y); y += 20;
                g.DrawString($"ITBIS 18%:  RD$ {itbis:N2}", font, brush, 10, y); y += 20;
                g.DrawString($"TOTAL:      RD$ {total:N2}", fontBold, brush, 10, y); y += 30;

                g.DrawLine(Pens.Black, 0, y, width, y); y += 15;

                // ---------- PIE ----------
                g.DrawString("¡Gracias por su compra!", font, brush, 50, y); y += 20;
                g.DrawString("Software POS v1.0", font, brush, 60, y);
            }
        }

        public void VistaPreviaFactura(long idFactura)
        {
            DataTable datosFactura = ObtenerDatosFactura(idFactura);
            if (datosFactura.Rows.Count > 0)
            {
                GenerarImagenFactura(datosFactura);
                PrintDocument pd = new PrintDocument();
                pd.PrintPage += new PrintPageEventHandler(PrintFactura);

                // Vista previa en vez de imprimir directo
                PrintPreviewDialog preview = new PrintPreviewDialog();
                preview.Document = pd;
                preview.Width = 800;
                preview.Height = 600;

                preview.ShowDialog(); 
            }
            else
            {
                MessageBox.Show("No se encontraron datos para la factura.");
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
