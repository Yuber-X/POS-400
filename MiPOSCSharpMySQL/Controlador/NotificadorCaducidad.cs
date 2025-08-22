using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiPOSCSharpMySQL.Controlador
{
    public static class NotificadorCaducidad
    {

            // Silencio temporal: dura hasta que cierres y reabras FormVentas
            private static bool _silenciarHastaCerrar = false;
            public static bool SilenciadoTemporal => _silenciarHastaCerrar;
            public static void ActivarSilencioTemporal() => _silenciarHastaCerrar = true;
            public static void QuitarSilencioTemporal() => _silenciarHastaCerrar = false;

            // Persistencia por grupo (1 y 3 meses) usando Settings:
            // Agrega en Properties/Settings.settings (tipo):
            // - Notif1MesHecha (bool, default false)
            // - Notif3MesesHecha (bool, default false)
            // - UltimoCount1Mes (int, default 0)
            // - UltimoCount3Meses (int, default 0)

            public static bool YaNotificado(int grupo)
            {
                if (grupo == 1) return Properties.Settings.Default.Notif1MesHecha;
                if (grupo == 3) return Properties.Settings.Default.Notif3MesesHecha;
                return false;
            }

            public static void MarcarComoNotificado(int grupo)
            {
                if (grupo == 1) Properties.Settings.Default.Notif1MesHecha = true;
                if (grupo == 3) Properties.Settings.Default.Notif3MesesHecha = true;
                Properties.Settings.Default.Save();
            }

            public static void ResetearGrupo(int grupo)
            {
                if (grupo == 1) Properties.Settings.Default.Notif1MesHecha = false;
                if (grupo == 3) Properties.Settings.Default.Notif3MesesHecha = false;
                Properties.Settings.Default.Save();
            }

            // Si aparecen "nuevos" productos en un grupo (sube el conteo), se habilita de nuevo ese grupo
            public static void SincronizarCounters(int count1, int count3)
            {
                if (count1 > Properties.Settings.Default.UltimoCount1Mes)
                {
                    Properties.Settings.Default.Notif1MesHecha = false;
                    Properties.Settings.Default.UltimoCount1Mes = count1;
                }

                if (count3 > Properties.Settings.Default.UltimoCount3Meses)
                {
                    Properties.Settings.Default.Notif3MesesHecha = false;
                    Properties.Settings.Default.UltimoCount3Meses = count3;
                }

                Properties.Settings.Default.Save();
            }
    }
}
