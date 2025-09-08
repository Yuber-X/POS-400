using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiPOSCSharpMySQL.Configuracion
{
    public static class SesionActual
    {
        public static long IdUsuario { get; set; }
        public static string NombreUsuario { get; set; }
        public static DateTime LoginTime { get; set; }
    }
}

