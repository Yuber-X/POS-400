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
using System.Drawing.Printing;

namespace MiPOSCSharpMySQL.Formularios
{
    public partial class FormCuadre : Form
    {
        private DataTable productosVendidosActual;
        private DataTable facturasDelDia;

        private DataTable resumenGeneralActual;
        private DataTable detalleUsuarioActual;
        private DataTable tiemposUsuariosActual;
        private string totalesActual;

        public FormCuadre()
        {
            InitializeComponent();
        }

        private void FormCuadre_Load(object sender, EventArgs e)
        {
            PrepararGrids();
            CargarUsuarios();
            cbUsuario.SelectedIndexChanged += cbUsuario_SelectedIndexChanged;
        }
        private void btnGenerar_Click(object sender, EventArgs e)
        {
            DateTime fecha = dtpFecha.Value.Date;

            resumenGeneralActual = CargarResumenGeneral(fecha);
            tiemposUsuariosActual = CargarTiemposUsuarios(fecha);
            totalesActual = CargarTotalesGenerales(fecha);

            productosVendidosActual = CargarProductosVendidos(fecha);
            facturasDelDia = CargarFacturasDelDia(fecha);

            detalleUsuarioActual = null;
            dgvDetalleUsuario.DataSource = null;

        }


        // ====================== CARGAR DATOS ======================
        private DataTable CargarResumenGeneral(DateTime fecha)
        {
            string sql = @"
                SELECT 
                u.idUsuario,
                u.nombreUsuario AS Usuario,
                COUNT(DISTINCT f.idFactura) AS CantidadFacturas,
                COALESCE(SUM(d.cantidad * d.precioVenta), 0) AS TotalVendido
                FROM usuario u
                LEFT JOIN factura f 
                ON f.fkUsuario = u.idUsuario
                AND DATE(f.fechaFactura) = @fecha
                LEFT JOIN detalle d 
                ON d.fkFactura = f.idFactura
                GROUP BY u.idUsuario, u.nombreUsuario
                HAVING CantidadFacturas > 0 OR TotalVendido > 0
                ORDER BY TotalVendido DESC;";

            var dt = EjecutarConsulta(sql, ("@fecha", fecha));
            dgvResumenGeneral.DataSource = dt;

            if (dgvResumenGeneral.Columns.Count > 0)
            {
                dgvResumenGeneral.Columns["idUsuario"].HeaderText = "ID Usuario";
                dgvResumenGeneral.Columns["Usuario"].HeaderText = "Usuario";
                dgvResumenGeneral.Columns["CantidadFacturas"].HeaderText = "Facturas";
                dgvResumenGeneral.Columns["TotalVendido"].HeaderText = "Total Vendido";
                dgvResumenGeneral.Columns["TotalVendido"].DefaultCellStyle.Format = "N2";

            }

            AjustarColumnas(dgvResumenGeneral);
            return dt; 
        }
        private DataTable CargarTiemposUsuarios(DateTime fecha)
        {
            string sql = @"
                SELECT 
                u.idUsuario,
                u.nombreUsuario,
                SEC_TO_TIME(
                COALESCE(SUM(
                GREATEST(0, TIMESTAMPDIFF(SECOND,
                GREATEST(s.loginTime, @desde),
                LEAST(COALESCE(s.logoutTime, NOW()), @hasta)
                ))
                ),0)
                ) AS TiempoActivo
                FROM usuario u
                LEFT JOIN userSession s
                ON u.idUsuario = s.fkUsuario
                AND s.loginTime < @hasta
                AND COALESCE(s.logoutTime, NOW()) > @desde
                GROUP BY u.idUsuario, u.nombreUsuario
                ORDER BY u.nombreUsuario;";

            DateTime desde = fecha.Date;
            DateTime hasta = fecha.Date.AddDays(1);

            var dt = EjecutarConsulta(sql, ("@desde", desde), ("@hasta", hasta));
            dgvTiempos.DataSource = dt;

            if (dgvTiempos.Columns.Count > 0)
            {
                dgvTiempos.Columns["idUsuario"].HeaderText = "ID Usuario";
                dgvTiempos.Columns["nombreUsuario"].HeaderText = "Usuario";
                dgvTiempos.Columns["TiempoActivo"].HeaderText = "Tiempo Activo";
            }
            AjustarColumnas(dgvTiempos);
            return dt;
        }
        private void CargarDetalleUsuario(int idUsuario, DateTime fecha)
        {
            string sql = @"
            SELECT 
            f.idFactura,
            f.fechaFactura,
            f.metodoPago,
            COALESCE(SUM(d.cantidad * d.precioVenta), 0) AS MontoFactura
            FROM factura f
            LEFT JOIN detalle d 
            ON d.fkFactura = f.idFactura
            WHERE f.fkUsuario = @idUsuario
            AND DATE(f.fechaFactura) = @fecha
            GROUP BY f.idFactura, f.fechaFactura, f.metodoPago
            ORDER BY f.fechaFactura;";

            var dt = EjecutarConsulta(sql, ("@idUsuario", idUsuario), ("@fecha", fecha));
            dgvDetalleUsuario.DataSource = dt;

            if (dgvDetalleUsuario.Columns.Count > 0)
            {
                dgvDetalleUsuario.Columns["idFactura"].HeaderText = "ID Factura";
                dgvDetalleUsuario.Columns["fechaFactura"].HeaderText = "Fecha";
                dgvDetalleUsuario.Columns["metodoPago"].HeaderText = "Método Pago";
                dgvDetalleUsuario.Columns["MontoFactura"].HeaderText = "Monto";
                dgvDetalleUsuario.Columns["MontoFactura"].DefaultCellStyle.Format = "N2";
            }

            AjustarColumnas(dgvDetalleUsuario);
        }
        private DataTable CargarProductosVendidos(DateTime fecha)
        {
            string sql = @"
        SELECT 
            p.nombre AS Producto,
            SUM(d.cantidad) AS Cantidad,
            SUM(d.cantidad * d.precioVenta) AS Subtotal
        FROM factura f
        JOIN detalle d ON f.idFactura = d.fkFactura
        JOIN producto p ON p.idProducto = d.fkProducto
        WHERE DATE(f.fechaFactura) = @fecha
        GROUP BY p.idProducto, p.nombre
        ORDER BY Cantidad DESC;";

            return EjecutarConsulta(sql, ("@fecha", fecha));
        }
        private DataTable CargarFacturasDelDia(DateTime fecha)
        {
            string sql = @"
        SELECT f.idFactura, f.fechaFactura, f.metodoPago
        FROM factura f
        WHERE DATE(f.fechaFactura) = @fecha
        ORDER BY f.idFactura;";

            return EjecutarConsulta(sql, ("@fecha", fecha));
        }
        private string CargarTotalesGenerales(DateTime fecha)
        {
            string sql = @"
                SELECT 
                COUNT(DISTINCT f.idFactura) AS TotalFacturas,
                COALESCE(SUM(d.cantidad * d.precioVenta), 0) AS TotalGeneral
                FROM factura f
                LEFT JOIN detalle d ON d.fkFactura = f.idFactura
                WHERE DATE(f.fechaFactura) = @fecha;";

            DataTable dt = EjecutarConsulta(sql, ("@fecha", fecha));
            if (dt.Rows.Count > 0)
            {
                int totalFacturas = Convert.ToInt32(dt.Rows[0]["TotalFacturas"]);
                decimal totalGeneral = Convert.ToDecimal(dt.Rows[0]["TotalGeneral"]);

                // Guardamos en la variable de clase
                totalesActual = $"Total Facturas: {totalFacturas}\nTotal Vendido: {totalGeneral:C}";
            }
            else
            {
                totalesActual = "Total Facturas: 0\nTotal Vendido: $0.00";
            }

            // Mostrar en el label
            lblTotales.Text = totalesActual;

            return totalesActual;
        }
        private void CargarUsuarios()
        {
            string sql = "SELECT idUsuario, nombreUsuario FROM usuario ORDER BY nombreUsuario";
            DataTable dt = EjecutarConsulta(sql);
            cbUsuario.DataSource = dt;
            cbUsuario.DisplayMember = "nombreUsuario";
            cbUsuario.ValueMember = "idUsuario";
            cbUsuario.SelectedIndex = -1;
        }
        private DataTable EjecutarConsulta(string sql, params (string, object)[] parametros)
        {
            var con = new Configuracion.CConexion();
            var dt = new DataTable();

            try
            {
                using (var cx = con.estableceConexion())
                using (var da = new MySqlDataAdapter(sql, cx))
                {
                    foreach (var p in parametros)
                        da.SelectCommand.Parameters.AddWithValue(p.Item1, p.Item2);
                    da.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error en consulta: " + ex.Message);
            }
            finally
            {
                con.CerrarConexion();
            }

            return dt;
        }


        // ====================== EVENTOS UI ======================
        private void AjustarColumnas(DataGridView dgv)
        {
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            dgv.ReadOnly = true;
        }
        private void cbUsuario_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbUsuario.SelectedIndex < 0) return;
            if (cbUsuario.SelectedValue == null || cbUsuario.SelectedValue == DBNull.Value) return;

            if (int.TryParse(cbUsuario.SelectedValue.ToString(), out int idUsuario))
            {
                DateTime fecha = dtpFecha.Value.Date;
                CargarDetalleUsuario(idUsuario, fecha);
            }
        }
        private void dgvResumenGeneral_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return; // cabecera
            if (dgvResumenGeneral.CurrentRow == null) return;

            var cell = dgvResumenGeneral.Rows[e.RowIndex].Cells["idUsuario"];
            if (cell == null) return;

            var val = cell.Value;
            if (val == null || val == DBNull.Value) return; // ignorar filas vacías

            int idUsuario;
            if (!int.TryParse(val.ToString(), out idUsuario)) return;

            DateTime fecha = dtpFecha.Value.Date;
            CargarDetalleUsuario(idUsuario, fecha);

            // sincroniza combo
            cbUsuario.SelectedValue = idUsuario;
        }
        private void PrepararGrids()
        {
            // Forzamos autogenerado y limpiamos columnas diseñadas a mano
            dgvResumenGeneral.AutoGenerateColumns = true;
            dgvDetalleUsuario.AutoGenerateColumns = true;
            dgvTiempos.AutoGenerateColumns = true;

            dgvResumenGeneral.Columns.Clear();
            dgvDetalleUsuario.Columns.Clear();
            dgvTiempos.Columns.Clear();

            AjustarColumnas(dgvResumenGeneral);
            AjustarColumnas(dgvDetalleUsuario);
            AjustarColumnas(dgvTiempos);
        }


        // ====================== IMPRESIÓN ======================
        private void btnImprimir_Click(object sender, EventArgs e)
        {
            ImprimirCuadre();
        }
        private void btnReimprimir_Click(object sender, EventArgs e)
        {
            ImprimirCuadre(true);
        }
        private void ImprimirCuadre(bool esReimpresion = false)
        {
            if (resumenGeneralActual == null || tiemposUsuariosActual == null)
            {
                MessageBox.Show("Debe generar el cuadre antes de imprimir.");
                return;
            }

            PrintDocument doc = new PrintDocument();
            doc.DocumentName = esReimpresion ? "Reimpresión de Cuadre" : "Cuadre del Día";
            doc.DefaultPageSettings.PaperSize = new PaperSize("Ticket80mm", 302, 1200);
            doc.DefaultPageSettings.Margins = new Margins(5, 5, 5, 5);

            doc.PrintPage += (s, ev) =>
            {
                float y = 10;
                float left = 5;
                float width = 290; // margen seguro dentro del ticket
                Font font = new Font("Consolas", 8);
                Font fontBold = new Font("Consolas", 9, FontStyle.Bold);
                Brush brush = Brushes.Black;

                // ENCABEZADO
                ev.Graphics.DrawString("BOTI FARMA EL MAMON", fontBold, brush, left, y); y += 20;
                ev.Graphics.DrawString("Cuadre del Día", fontBold, brush, left, y); y += 20;
                ev.Graphics.DrawString($"Fecha: {dtpFecha.Value:dd/MM/yyyy}", font, brush, left, y); y += 20;
                ev.Graphics.DrawLine(Pens.Black, left, y, width, y); y += 10;

                // RESUMEN GENERAL
                ev.Graphics.DrawString("=== RESUMEN GENERAL ===", fontBold, brush, left, y); y += 20;
                foreach (DataRow row in resumenGeneralActual.Rows)
                {
                    string linea = $"{row["Usuario"],-12} F:{row["CantidadFacturas"],2} T:{Convert.ToDecimal(row["TotalVendido"]):N2}";
                    ev.Graphics.DrawString(linea, font, brush, left, y);
                    y += 15;
                }

                // TIEMPOS DE SESIÓN
                y += 10;
                ev.Graphics.DrawString("=== SESIONES ===", fontBold, brush, left, y); y += 20;
                foreach (DataRow row in tiemposUsuariosActual.Rows)
                {
                    string linea = $"{row["nombreUsuario"],-12} {row["TiempoActivo"]}";
                    ev.Graphics.DrawString(linea, font, brush, left, y);
                    y += 15;
                }

                // PRODUCTOS VENDIDOS
                if (productosVendidosActual != null && productosVendidosActual.Rows.Count > 0)
                {
                    y += 10;
                    ev.Graphics.DrawString("=== PRODUCTOS VENDIDOS ===", fontBold, brush, left, y); y += 20;
                    foreach (DataRow row in productosVendidosActual.Rows)
                    {
                        string linea = $"{row["Producto"],-10} x{row["Cantidad"],-3} RD${Convert.ToDecimal(row["Subtotal"]):N2}";
                        ev.Graphics.DrawString(linea, font, brush, left, y);
                        y += 15;
                    }
                }

                // FACTURAS
                if (facturasDelDia != null && facturasDelDia.Rows.Count > 0)
                {
                    y += 10;
                    ev.Graphics.DrawString("=== FACTURAS DEL DÍA ===", fontBold, brush, left, y); y += 20;
                    int colCount = 0;
                    string lineaFacturas = "";
                    foreach (DataRow row in facturasDelDia.Rows)
                    {
                        lineaFacturas += row["idFactura"].ToString().PadLeft(5) + " ";
                        colCount++;
                        if (colCount == 5)
                        {
                            ev.Graphics.DrawString(lineaFacturas, font, brush, left, y);
                            y += 15;
                            lineaFacturas = "";
                            colCount = 0;
                        }
                    }
                    if (lineaFacturas.Length > 0)
                    {
                        ev.Graphics.DrawString(lineaFacturas, font, brush, left, y);
                        y += 15;
                    }
                }

                // TOTALES
                y += 20;
                ev.Graphics.DrawLine(Pens.Black, left, y, width, y); y += 10;
                ev.Graphics.DrawString("=== TOTALES ===", fontBold, brush, left, y); y += 20;

                // Mostramos los totales en dos líneas separadas
                string[] lineasTotales = totalesActual.Split('\n');
                foreach (string linea in lineasTotales)
                {
                    // Centramos el texto dentro del ancho del ticket
                    SizeF size = ev.Graphics.MeasureString(linea, fontBold);
                    float xCentered = left + (width - size.Width) / 2;
                    ev.Graphics.DrawString(linea, fontBold, brush, xCentered, y);
                    y += 20;
                }
            };

            PrintPreviewDialog preview = new PrintPreviewDialog();
            preview.Document = doc;
            preview.ShowDialog();
        }

    }
}
