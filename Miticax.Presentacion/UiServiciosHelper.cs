//UNED
//Mitica X
//Jorge Arias Melendez
//Septiembre 2025
//Helper de UI: instanciacion de Datos, filtros en arrays y llamadas por reflexion a servicios

using System;
using System.Reflection;
using Miticax.Datos;

namespace Miticax.Presentacion
{
    internal static class UiServiciosHelper
    {
        // ---- Instancias basicas de Datos (no estaticos) ----
        internal static JugadorDatos JugadorDatos() => new JugadorDatos();
        internal static CriaturaDatos CriaturaDatos() => new CriaturaDatos();
        internal static InventarioDatos InventarioDatos() => new InventarioDatos();
        internal static EquipoDatos EquipoDatos() => new EquipoDatos();
        internal static BatallaDatos BatallaDatos() => new BatallaDatos();
        internal static RondaDatos RondaDatos() => new RondaDatos();

        // ---- Filtros con arrays (sin LINQ ni listas) ----
        internal static T[] FiltrarPorCampoIgual<T>(T[] fuente, string nombreCampoInt, int valor)
        {
            if (fuente == null) return null;

            // contar
            int n = 0;
            for (int i = 0; i < fuente.Length; i++)
            {
                var it = fuente[i];
                if (it == null) continue;
                var prop = it.GetType().GetProperty(nombreCampoInt);
                if (prop == null) continue;
                object v = prop.GetValue(it);
                if (v is int iv && iv == valor) n++;
            }

            var res = new T[n];
            int j = 0;
            for (int i = 0; i < fuente.Length; i++)
            {
                var it = fuente[i];
                if (it == null) continue;
                var prop = it.GetType().GetProperty(nombreCampoInt);
                if (prop == null) continue;
                object v = prop.GetValue(it);
                if (v is int iv && iv == valor) res[j++] = it;
            }
            return res;
        }

        // ---- Utilidades Batalla por reflexion para no romper compatibilidad ----
        internal static int UltimoIdBatalla()
        {
            var arr = BatallaDatos().GetAllSnapshot();
            int max = 0;
            if (arr != null)
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    var it = arr[i];
                    if (it == null) continue;
                    var p = it.GetType().GetProperty("IdBatalla");
                    if (p == null) continue;
                    int id = (int)p.GetValue(it);
                    if (id > max) max = id;
                }
            }
            return max;
        }

        internal static bool TryRegistrarBatalla(int j1, int e1, int j2, int e2, out string mensaje)
        {
            mensaje = "";
            // Busco Miticax.Logica.BatallaService en tiempo de ejecucion
            var t = Type.GetType("Miticax.Logica.BatallaService, Miticax.Logica");
            if (t == null) { mensaje = "Servicio BatallaService no disponible."; return false; }

            // Intento 1: RegistrarBatalla(int,int,int,int,out string)
            var m = t.GetMethod("RegistrarBatalla", new Type[] { typeof(int), typeof(int), typeof(int), typeof(int), typeof(string).MakeByRefType() });
            if (m != null)
            {
                object[] pars = new object[] { j1, e1, j2, e2, null };
                var ro = m.Invoke(null, pars); // asumo static en servicio
                mensaje = ExtraerMensaje(ro) ?? (pars[4] as string) ?? "";
                return ExtraerExito(ro);
            }

            // Intento 2: RegistrarBatalla(entidad, out string)
            var te = Type.GetType("Miticax.Entidades.BatallaEntidad, Miticax.Entidades");
            var me = t.GetMethod("RegistrarBatalla", new Type[] { te, typeof(string).MakeByRefType() });
            if (te != null && me != null)
            {
                var ent = Activator.CreateInstance(te);
                te.GetProperty("IdJugador1")?.SetValue(ent, j1);
                te.GetProperty("IdEquipo1")?.SetValue(ent, e1);
                te.GetProperty("IdJugador2")?.SetValue(ent, j2);
                te.GetProperty("IdEquipo2")?.SetValue(ent, e2);

                object[] pars = new object[] { ent, null };
                var ro = me.Invoke(null, pars);
                mensaje = ExtraerMensaje(ro) ?? (pars[1] as string) ?? "";
                return ExtraerExito(ro);
            }

            mensaje = "No fue posible invocar RegistrarBatalla.";
            return false;
        }

        internal static bool TryEjecutarBatalla(int idBatalla, out string mensaje)
        {
            mensaje = "";
            var t = Type.GetType("Miticax.Logica.BatallaService, Miticax.Logica");
            if (t == null) { mensaje = "Servicio BatallaService no disponible."; return false; }

            // Intento 1: EjecutarBatalla(int, out string)
            var m1 = t.GetMethod("EjecutarBatalla", new Type[] { typeof(int), typeof(string).MakeByRefType() });
            if (m1 != null)
            {
                object[] pars = new object[] { idBatalla, null };
                var ro = m1.Invoke(null, pars);
                mensaje = ExtraerMensaje(ro) ?? (pars[1] as string) ?? "";
                return ExtraerExito(ro);
            }

            // Intento 2: EjecutarBatalla(int, Random, out string)
            var m2 = t.GetMethod("EjecutarBatalla", new Type[] { typeof(int), typeof(Random), typeof(string).MakeByRefType() });
            if (m2 != null)
            {
                object[] pars = new object[] { idBatalla, new Random(), null };
                var ro = m2.Invoke(null, pars);
                mensaje = ExtraerMensaje(ro) ?? (pars[2] as string) ?? "";
                return ExtraerExito(ro);
            }

            mensaje = "No fue posible invocar EjecutarBatalla.";
            return false;
        }

        internal static object[] TopRanking10()
        {
            // Intenta RankingService.Top10() o alternativas comunes
            var t = Type.GetType("Miticax.Logica.RankingService, Miticax.Logica");
            if (t != null)
            {
                var m = t.GetMethod("Top10", Type.EmptyTypes)
                        ?? t.GetMethod("Top100", Type.EmptyTypes)
                        ?? t.GetMethod("Top", new Type[] { typeof(int) });

                if (m != null)
                {
                    if (m.GetParameters().Length == 0)
                        return (object[])m.Invoke(null, null);

                    return (object[])m.Invoke(null, new object[] { 10 });
                }
            }
            return new object[0];
        }

        // ---- Extraer Exito/Mensaje de ResultadoOperacion via reflexion ----
        private static bool ExtraerExito(object ro)
        {
            if (ro == null) return false;
            var p = ro.GetType().GetProperty("Exito") ?? ro.GetType().GetProperty("Success");
            if (p == null) return false;
            var v = p.GetValue(ro);
            return v is bool b && b;
        }

        internal static string ExtraerMensaje(object ro)
        {
            if (ro == null) return null;
            var p = ro.GetType().GetProperty("Mensaje")
                 ?? ro.GetType().GetProperty("Message")
                 ?? ro.GetType().GetProperty("Detalle")
                 ?? ro.GetType().GetProperty("Error");
            if (p == null) return null;
            var v = p.GetValue(ro);
            return v?.ToString();
        }
    }
}
