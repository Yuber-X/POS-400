using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MiPOSCSharpMySQL.Modelos
{
    internal class ModeloProducto
    {
        long idProducto;
        String nombreProducto;
        Double precioProducto;
        int stockProducto;
        String descripcion;

        public long IdProducto { get => idProducto; set => idProducto = value; }
        public string NombreProducto { get => nombreProducto; set => nombreProducto = value; }
        public double PrecioProducto { get => precioProducto; set => precioProducto = value; }
        public int StockProducto { get => stockProducto; set => stockProducto = value; }
        public string Descripcion { get => descripcion; set => descripcion = value; }
        public DateTime? FechaCaducidad {  get; set; }
    }
}
