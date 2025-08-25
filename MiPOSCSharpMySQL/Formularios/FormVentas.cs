using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
using MySql.Data.MySqlClient;


namespace MiPOSCSharpMySQL.Formularios
{
    public partial class FormVentas : Form
    {
        public FormVentas()
        {
            InitializeComponent();

            txtIdCliente.ReadOnly = true;
            txtNombreCliente.ReadOnly = true;
            txtApPaterno.ReadOnly = true;
            txtApMaterno.ReadOnly = true;

            txtIdProducto.ReadOnly = true;
            txtNombreProducto.ReadOnly = true;
            txtPrecio.ReadOnly = true;
            txtStock.ReadOnly = true;
            txtPrecioVentaFinal.ReadOnly = true;

            Controlador.NotificadorCaducidad.QuitarSilencioTemporal();

            Controlador.ControladorVenta objetoVenta = new Controlador.ControladorVenta();
            objetoVenta.MostrarUltimaFactura(lbUltimaFactura);

            VerificarProductosCaducidad();

            //Controlador.ControladorProducto objetoProducto = new Controlador.ControladorProducto();
            //Controlador.ControladorCliente objetoCliente = new Controlador.ControladorCliente();
            //objetoProducto.MostrarProductos(dgvProducto);
            //objetoCliente.MostrarClientes(dgvCliente);
        }


        private void label7_Click(object sender, EventArgs e)
        {

        }
        private void groupBox6_Enter(object sender, EventArgs e)
        {

        }
        private void label1_Click(object sender, EventArgs e)
        {

        }
        private void label13_Click(object sender, EventArgs e)
        {

        }
        private void label12_Click(object sender, EventArgs e)
        {

        }
        private void label2_Click(object sender, EventArgs e)
        {

        }
        private void label7_Click_1(object sender, EventArgs e)
        {

        }
        private void label9_Click(object sender, EventArgs e)
        {

        }
        private void label10_Click(object sender, EventArgs e)
        {

        }
        private void dgvCarrito_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void groupBox7_Enter(object sender, EventArgs e)
        {

        }
        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
        private void txtPrecioVentaFinal_TextChanged(object sender, EventArgs e)
        {

        }
        private void dgvCliente_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void txtApPaterno_TextChanged(object sender, EventArgs e)
        {

        }


        /*----------------------------------------------------------------------------------------------------------------------*/


        private void txtBuscarCliente_TextChanged(object sender, EventArgs e)
        {
            Controlador.ControladorVenta objetoVenta = new Controlador.ControladorVenta();
            objetoVenta.BuscarClientes(txtBuscarCliente, dgvCliente);
        }

        private void txtBuscarProducto_TextChanged(object sender, EventArgs e)
        {
            Controlador.ControladorVenta objetoVenta = new Controlador.ControladorVenta();
            objetoVenta.BuscarProductos(txtBuscarProducto, dgvProducto);

        }

        private void dgvProducto_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            Controlador.ControladorVenta objetoVenta = new Controlador.ControladorVenta();
            objetoVenta.SeleccionarProductoVenta(dgvProducto, txtIdProducto, txtNombreProducto, txtPrecio, txtStock, txtPrecioVentaFinal);
        }

        private void dgvCliente_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            Controlador.ControladorVenta objetoVenta = new Controlador.ControladorVenta();
            objetoVenta.SeleccionarClienteVenta(dgvCliente, txtIdCliente, txtNombreCliente, txtApPaterno, txtApMaterno);
        }

        private void btnHabilitar_Click(object sender, EventArgs e)
        {
            txtPrecioVentaFinal.ReadOnly = false;
        }

        private void btnDeshabilitar_Click(object sender, EventArgs e)
        {
            txtPrecioVentaFinal.ReadOnly = true;
        }

        private void btnAgregarProducto_Click(object sender, EventArgs e)
        {
            Controlador.ControladorVenta objetoVenta = new Controlador.ControladorVenta();
            objetoVenta.PasarProductosVenta(dgvCarrito, txtIdProducto, txtNombreProducto, txtPrecioVentaFinal, txtStockVenta, txtStock);
            objetoVenta.CalcularTotal(dgvCarrito, lbIva, lbTotal);
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            Controlador.ControladorVenta objetoVenta = new Controlador.ControladorVenta();
            objetoVenta.EliminarSeleccion(dgvCarrito);
        }

        private async void btnFacturar_Click(object sender, EventArgs e)
        {
            //Controlador.ControladorVenta objetoVenta = new Controlador.ControladorVenta();
            //objetoVenta.CrearFactura(txtIdCliente);

            //objetoVenta.RealizarVentaV2(dgvCarrito,idFactura:0); //OJO

            //objetoVenta.LimpiarCamposVenta(txtBuscarCliente, dgvCliente,txtBuscarProducto,dgvProducto,txtIdCliente,txtNombreCliente, txtApPaterno, txtApMaterno, 
            //                               txtIdProducto, txtNombreProducto, txtPrecio, txtStock, txtPrecioVentaFinal, txtStockVenta, dgvCarrito, lbIva, lbTotal);
            //objetoVenta.MostrarUltimaFactura(lbUltimaFactura);

            Controlador.ControladorReporte objetoReporte = new Controlador.ControladorReporte();
            Controlador.ControladorVenta objetoVenta = new Controlador.ControladorVenta();

            // Verifico si el usuario selecciono el metodo de pago
            if (metodoPago.CheckedItems.Count == 0)
            {
                MessageBox.Show("⚠ Debe seleccionar un método de pago (Efectivo o Tarjeta).",
                                 "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string pagoSeleccionado = metodoPago.CheckedItems[0].ToString();


            // Creamos la factura y obtenemos su ID
            long idFactura = objetoVenta.CrearFacturaV2(txtIdCliente, pagoSeleccionado);


            // Si hubo un error, terminamos
            if (idFactura == -1) return;

            // Registramos los productos vendidos
            objetoVenta.RealizarVentaV2(dgvCarrito, idFactura);


            // Limpiamos la pantalla
            objetoVenta.LimpiarCamposVenta(txtBuscarCliente, dgvCliente, txtBuscarProducto, dgvProducto, txtIdCliente, txtNombreCliente,
                                            txtApPaterno, txtApMaterno, txtIdProducto, txtNombreProducto, txtPrecio, txtStock,
                                            txtPrecioVentaFinal, txtStockVenta, dgvCarrito, lbIva, lbTotal);

            // Mostramos la última factura generada
            objetoVenta.MostrarUltimaFactura(lbUltimaFactura);

            //Esperar que todos los datos se guarden correctamente
            await Task.Delay(1000);

            // Imprimimos la factura
            if (long.TryParse(lbUltimaFactura.Text, out long ultimaFactura))
            {
                objetoReporte.ImprimirFactura(ultimaFactura); // Ahora acepta long directamente
            }
            else
            {
                MessageBox.Show("Error al obtener el número de la última factura.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void VerificarProductosCaducidad()
        {
            // 1) Si el usuario pidió "volver a avisar más tarde" en esta sesión, no avisamos más
            if (Controlador.NotificadorCaducidad.SilenciadoTemporal)
                return;

            Configuracion.CConexion objetoConexion = new Configuracion.CConexion();
            try
            {
                using (MySqlConnection conexion = objetoConexion.estableceConexion())
                {
                    // 2) Consultas por grupo
                    string sql1Mes = @"SELECT COUNT(*) 
                               FROM producto 
                               WHERE fechaCaducidad IS NOT NULL
                                 AND TIMESTAMPDIFF(MONTH, CURDATE(), fechaCaducidad) <= 1";
                    int productos1Mes = Convert.ToInt32(new MySqlCommand(sql1Mes, conexion).ExecuteScalar());

                    string sql3Meses = @"SELECT COUNT(*) 
                                 FROM producto 
                                 WHERE fechaCaducidad IS NOT NULL
                                   AND TIMESTAMPDIFF(MONTH, CURDATE(), fechaCaducidad) <= 3
                                   AND TIMESTAMPDIFF(MONTH, CURDATE(), fechaCaducidad) > 1";
                    int productos3Meses = Convert.ToInt32(new MySqlCommand(sql3Meses, conexion).ExecuteScalar());

                    // 3) Si aparecieron nuevos productos en alguno de los grupos, re-habilitamos ese grupo
                    Controlador.NotificadorCaducidad.SincronizarCounters(productos1Mes, productos3Meses);

                    // 4) Armamos un único mensaje solo para los grupos que NO estén ya "reconocidos"
                    string mensaje = "";
                    bool hayGrupo1 = (productos1Mes > 0) && !Controlador.NotificadorCaducidad.YaNotificado(1);
                    bool hayGrupo3 = (productos3Meses > 0) && !Controlador.NotificadorCaducidad.YaNotificado(3);

                    if (hayGrupo1) mensaje += $"⚠ Hay {productos1Mes} producto(s) que se caducan en ≤ 1 mes.\n";
                    if (hayGrupo3) mensaje += $"⚠ Hay {productos3Meses} producto(s) que se caducan en ≤ 3 meses.\n";

                    if (!string.IsNullOrEmpty(mensaje))
                    {
                        // 5) Un único MessageBox con Yes/No:
                        // Yes = "Volver a avisar más tarde"   |   No = "OK"
                        var result = MessageBox.Show(
                            mensaje + "\n¿Volver a avisar más tarde?",
                            "Advertencia - Caducidad",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Warning);

                        if (result == DialogResult.Yes)
                        {
                            // Silencio temporal (esta sesión, hasta reabrir FormVentas)
                            Controlador.NotificadorCaducidad.ActivarSilencioTemporal();
                        }
                        else
                        {
                            // Marcamos como reconocidos los grupos presentes (persistente)
                            if (hayGrupo1) Controlador.NotificadorCaducidad.MarcarComoNotificado(1, productos1Mes);
                            if (hayGrupo3) Controlador.NotificadorCaducidad.MarcarComoNotificado(3, productos3Meses);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error al verificar caducidad: " + e.Message);
            }
            finally
            {
                objetoConexion.CerrarConexion();
            }
        }

        private void metodoPago_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // Desmarca todas las demás opciones cuando selecciona una
            for (int i = 0; i < metodoPago.Items.Count; i++)
            {
                if (i != e.Index)
                {
                    metodoPago.SetItemChecked(i, false);
                }
            }
        }
    }
}
