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
                totalesActual = $"Total Facturas: {totalFacturas}      ||     Total Vendido: {totalGeneral:C}";
            }
            else
            {
                totalesActual = "Total Facturas: 0     ||     Total Vendido: $0.00";
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
                doc.PrintPage += (s, ev) =>
                {
                    float y = 20;
                    Font font = new Font("Arial", 10);
                    Brush brush = Brushes.Black;

                    // Encabezado
                    ev.Graphics.DrawString(doc.DocumentName, new Font("Arial", 14, FontStyle.Bold), brush, 20, y);
                    y += 40;
                    ev.Graphics.DrawString("Fecha: " + dtpFecha.Value.ToShortDateString(), font, brush, 20, y);
                    y += 30;

                    // Resumen General
                    ev.Graphics.DrawString("=== RESUMEN GENERAL ===", font, brush, 20, y);
                    y += 25;
                    foreach (DataRow row in resumenGeneralActual.Rows)
                    {
                        string linea = $"{row["Usuario"],-15} Facturas: {row["CantidadFacturas"],3} Total: {Convert.ToDecimal(row["TotalVendido"]):C}";
                        ev.Graphics.DrawString(linea, font, brush, 20, y);
                        y += 20;
                    }

                    y += 20;

                    // Tiempos
                    ev.Graphics.DrawString("=== TIEMPOS DE SESIÓN ===", font, brush, 20, y);
                    y += 25;
                    foreach (DataRow row in tiemposUsuariosActual.Rows)
                    {
                        string linea = $"{row["nombreUsuario"],-15} Tiempo Activo: {row["TiempoActivo"]}";
                        ev.Graphics.DrawString(linea, font, brush, 20, y);
                        y += 20;
                    }

                    y += 30;

                    // Productos vendidos
                    if (productosVendidosActual != null && productosVendidosActual.Rows.Count > 0)
                    {
                        ev.Graphics.DrawString("=== PRODUCTOS VENDIDOS ===", font, brush, 20, y);
                        y += 25;
                        foreach (DataRow row in productosVendidosActual.Rows)
                        {
                            string linea = $"{row["Producto"],-20} Cant: {row["Cantidad"],3}  Subtotal: {Convert.ToDecimal(row["Subtotal"]):C}";
                            ev.Graphics.DrawString(linea, font, brush, 20, y);
                            y += 20;
                        }
                        y += 20;
                    }

                    // Facturas del día
                    if (facturasDelDia != null && facturasDelDia.Rows.Count > 0)
                    {
                        ev.Graphics.DrawString("=== FACTURAS DEL DÍA ===", font, brush, 20, y);
                        y += 25;

                        // Mostrar en columnas de 5 por línea para ahorrar espacio
                        int colCount = 0;
                        string lineaFacturas = "";
                        foreach (DataRow row in facturasDelDia.Rows)
                        {
                            lineaFacturas += row["idFactura"].ToString().PadLeft(6) + " ";
                            colCount++;

                            if (colCount == 5)
                            {
                                ev.Graphics.DrawString(lineaFacturas, font, brush, 20, y);
                                y += 20;
                                lineaFacturas = "";
                                colCount = 0;
                            }
                        }
                        if (lineaFacturas.Length > 0)
                        {
                            ev.Graphics.DrawString(lineaFacturas, font, brush, 20, y);
                            y += 20;
                        }
                    }

                    // Totales (al final de todo)
                    y += 40;
                    ev.Graphics.DrawString("=== TOTALES ===", new Font("Arial", 11, FontStyle.Bold), brush, 20, y);
                    y += 25;
                    ev.Graphics.DrawString(totalesActual, new Font("Arial", 11, FontStyle.Bold), brush, 20, y);

                };

                PrintPreviewDialog preview = new PrintPreviewDialog();
                preview.Document = doc;
                preview.ShowDialog(); // Vista previa antes de imprimir

                // Si quieres imprimir directo:
                // doc.Print();
            }

    }
}
