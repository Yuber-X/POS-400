using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiPOSCSharpMySQL.Formularios
{
    public partial class FormAlmacen : Form
    {
        public FormAlmacen()
        {
            InitializeComponent();
            Controlador.ControladorProducto objetoProducto = new Controlador.ControladorProducto();
            objetoProducto.MostrarProductos(dgvproductos, lblProductosTotal);
            dgvproductos.ReadOnly = true;
        }
    }
}
