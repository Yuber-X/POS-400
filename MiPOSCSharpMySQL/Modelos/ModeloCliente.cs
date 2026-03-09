using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MiPOSCSharpMySQL.Modelos
{
    internal class ModeloCliente
    {
        long idCliente;
        string nombre;
        string apPaterno;
        string apMaterno;

        public long IdCliente { get => idCliente; set => idCliente = value; }
        public string Nombre { get => nombre; set => nombre = value; }
        public string ApPaterno { get => apPaterno; set => apPaterno = value; }
        public string ApMaterno { get => apMaterno; set => apMaterno = value; }
    }
}
