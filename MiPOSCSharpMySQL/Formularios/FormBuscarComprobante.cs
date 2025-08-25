using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiPOSCSharpMySQL.Formularios
{
    public partial class FormBuscarComprobante : Form
    {
        public FormBuscarComprobante()
        {
            InitializeComponent();
        }

        private void btnbuscar_Click(object sender, EventArgs e)
        {
            Controlador.ControladorReporte objetoReporte = new Controlador.ControladorReporte();
            //objetoReporte.MostrarDatosFactura(txtnumerofactura, lblnumerofactura, lblfechaventa, lblnombres, lblappaterno, lblapmaterno);
            objetoReporte.MostrarVentaFactura(txtnumerofactura,dgvproducto,lbliva,lbltotal);

            if (!long.TryParse(txtnumerofactura.Text, out long idFactura))
            {
                MessageBox.Show("Ingrese un número de factura válido.");
                return;
            }

            DataTable factura = objetoReporte.ObtenerFactura(idFactura);

            if (factura != null && factura.Rows.Count > 0)
            {
                DataRow fila = factura.Rows[0];
                lblnumerofactura.Text = fila["idfactura"].ToString();
                lblfechaventa.Text = fila["fechaFactura"].ToString();
                lblnombres.Text = fila["nombres"].ToString();
                lblappaterno.Text = fila["appaterno"].ToString();
                lblapmaterno.Text = fila["appmaterno"].ToString();
                lblMetodoPago.Text = fila["metodoPago"].ToString();
            }
            else
            {
                MessageBox.Show("Factura no encontrada.");
            }
        }

        private void btnimprimir_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtnumerofactura.Text, out int numeroFactura))
            {
                Controlador.ControladorReporte objetoReporte = new Controlador.ControladorReporte();
                objetoReporte.ImprimirFactura(numeroFactura);
            }
            else
            {
                MessageBox.Show("Ingrese un número de factura válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
