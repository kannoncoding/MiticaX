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
        // ---- Singletons de Datos compartidos por toda la UI ----
        // Se crean una sola vez por AppDomain y todos los formularios comparten
        // los mismos arreglos en memoria.
        private static readonly JugadorDatos _jugadorDatos = new JugadorDatos();
        private static readonly CriaturaDatos _criaturaDatos = new CriaturaDatos();
        private static readonly InventarioDatos _inventarioDatos = new InventarioDatos();
        private static readonly EquipoDatos _equipoDatos = new EquipoDatos();
        private static readonly BatallaDatos _batallaDatos = new BatallaDatos();
        private static readonly RondaDatos _rondaDatos = new RondaDatos();

        // Exponer las mismas funciones que ya usan los formularios,
        // pero devolviendo SIEMPRE la instancia unica (no new).
        internal static JugadorDatos JugadorDatos() => _jugadorDatos;
        internal static CriaturaDatos CriaturaDatos() => _criaturaDatos;
        internal static InventarioDatos InventarioDatos() => _inventarioDatos;
        internal static EquipoDatos EquipoDatos() => _equipoDatos;
        internal static BatallaDatos BatallaDatos() => _batallaDatos;
        internal static RondaDatos RondaDatos() => _rondaDatos;

        // ---- Utilidades internas seguras ----

        // Intenta obtener un entero desde una propiedad numerica (int, int?, short, long, etc.)
        private static bool TryGetIntPropertyValue(object obj, string propertyName, out int value)
        {
            value = 0;
            if (obj == null) return false;
            var t = obj.GetType();
            var p = t.GetProperty(propertyName);
            if (p == null) return false;

            var v = p.GetValue(obj);
            if (v == null) return false;

            try
            {
                // Usa Convert.ToInt32 para abarcar tipos numericos comunes y nullable
                value = Convert.ToInt32(v);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static object CrearInstanciaServicio(Type t)
        {
            try
            {
                // Crea instancia sin parametros si el servicio no es static
                return Activator.CreateInstance(t);
            }
            catch
            {
                return null;
            }
        }

        private static object[] ArrayToObjectArray(object arrayObj)
        {
            // Convierte cualquier System.Array fuertemente tipado a object[]
            if (arrayObj == null) return new object[0];
            var arr = arrayObj as Array;
            if (arr == null) return new object[0];

            int n = arr.Length;
            var res = new object[n];
            for (int i = 0; i < n; i++)
                res[i] = arr.GetValue(i);
            return res;
        }

        // ---- Filtros con arrays (sin LINQ ni listas) ----
        internal static T[] FiltrarPorCampoIgual<T>(T[] fuente, string nombreCampoInt, int valor)
        {
            if (fuente == null) return new T[0];

            // contar
            int n = 0;
            for (int i = 0; i < fuente.Length; i++)
            {
                var it = fuente[i];
                if (it == null) continue;
                int iv;
                if (TryGetIntPropertyValue(it, nombreCampoInt, out iv) && iv == valor) n++;
            }

            var res = new T[n];
            int j = 0;
            for (int i = 0; i < fuente.Length; i++)
            {
                var it = fuente[i];
                if (it == null) continue;
                int iv;
                if (TryGetIntPropertyValue(it, nombreCampoInt, out iv) && iv == valor) res[j++] = it;
            }
            return res;
        }

        // ---- Utilidades Batalla por reflexion para no romper compatibilidad ----
        internal static int UltimoIdBatalla()
        {
            var arr = BatallaDatos().GetAllSnapshot(); // usa singleton
            int max = 0;
            if (arr != null)
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    var it = arr[i];
                    if (it == null) continue;
                    int id;
                    if (TryGetIntPropertyValue(it, "IdBatalla", out id))
                        if (id > max) max = id;
                }
            }
            return max;
        }

        internal static bool TryRegistrarBatalla(int j1, int e1, int j2, int e2, out string mensaje)
        {
            mensaje = "";
            var t = Type.GetType("Miticax.Logica.BatallaService, Miticax.Logica");
            if (t == null) { mensaje = "Servicio BatallaService no disponible."; return false; }

            // Intento 1: static RegistrarBatalla(int,int,int,int,out string)
            var m = t.GetMethod("RegistrarBatalla", new Type[] { typeof(int), typeof(int), typeof(int), typeof(int), typeof(string).MakeByRefType() });
            if (m != null)
            {
                object[] pars = new object[] { j1, e1, j2, e2, null };
                var ro = m.Invoke(null, pars);
                mensaje = ExtraerMensaje(ro) ?? (pars[4] as string) ?? "";
                return ExtraerExito(ro);
            }

            // Intento 1b: instancia RegistrarBatalla(int,int,int,int,out string)
            var inst = CrearInstanciaServicio(t);
            if (inst != null)
            {
                var mi = t.GetMethod("RegistrarBatalla", new Type[] { typeof(int), typeof(int), typeof(int), typeof(int), typeof(string).MakeByRefType() });
                if (mi != null)
                {
                    object[] pars = new object[] { j1, e1, j2, e2, null };
                    var ro = mi.Invoke(inst, pars);
                    mensaje = ExtraerMensaje(ro) ?? (pars[4] as string) ?? "";
                    return ExtraerExito(ro);
                }
            }

            // Intento 2: RegistrarBatalla(BatallaEntidad, out string)
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
                var ro = me.Invoke(me.IsStatic ? null : (inst ?? CrearInstanciaServicio(t)), pars);
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

            // Intento 1: static EjecutarBatalla(int, out string)
            var m1 = t.GetMethod("EjecutarBatalla", new Type[] { typeof(int), typeof(string).MakeByRefType() });
            if (m1 != null)
            {
                object[] pars = new object[] { idBatalla, null };
                var ro = m1.Invoke(null, pars);
                mensaje = ExtraerMensaje(ro) ?? (pars[1] as string) ?? "";
                return ExtraerExito(ro);
            }

            // Intento 1b: instancia EjecutarBatalla(int, out string)
            var inst = CrearInstanciaServicio(t);
            if (inst != null)
            {
                var mi = t.GetMethod("EjecutarBatalla", new Type[] { typeof(int), typeof(string).MakeByRefType() });
                if (mi != null)
                {
                    object[] pars = new object[] { idBatalla, null };
                    var ro = mi.Invoke(inst, pars);
                    mensaje = ExtraerMensaje(ro) ?? (pars[1] as string) ?? "";
                    return ExtraerExito(ro);
                }
            }

            // Intento 2: static EjecutarBatalla(int, Random, out string)
            var m2 = t.GetMethod("EjecutarBatalla", new Type[] { typeof(int), typeof(Random), typeof(string).MakeByRefType() });
            if (m2 != null)
            {
                object[] pars = new object[] { idBatalla, new Random(), null };
                var ro = m2.Invoke(null, pars);
                mensaje = ExtraerMensaje(ro) ?? (pars[2] as string) ?? "";
                return ExtraerExito(ro);
            }

            // Intento 2b: instancia EjecutarBatalla(int, Random, out string)
            var mi2 = t.GetMethod("EjecutarBatalla", new Type[] { typeof(int), typeof(Random), typeof(string).MakeByRefType() });
            if (inst != null && mi2 != null)
            {
                object[] pars = new object[] { idBatalla, new Random(), null };
                var ro = mi2.Invoke(inst, pars);
                mensaje = ExtraerMensaje(ro) ?? (pars[2] as string) ?? "";
                return ExtraerExito(ro);
            }

            mensaje = "No fue posible invocar EjecutarBatalla.";
            return false;
        }

        internal static object[] TopRanking10()
        {
            var t = Type.GetType("Miticax.Logica.RankingService, Miticax.Logica");
            if (t != null)
            {
                // Probar metodos sin parametros primero
                var m = t.GetMethod("Top10", Type.EmptyTypes)
                         ?? t.GetMethod("Top100", Type.EmptyTypes);

                if (m != null)
                {
                    // static
                    if (m.IsStatic) return ArrayToObjectArray(m.Invoke(null, null));
                    // instancia
                    var inst = CrearInstanciaServicio(t);
                    return ArrayToObjectArray(m.Invoke(inst, null));
                }

                // Probar Top(int)
                var mTop = t.GetMethod("Top", new Type[] { typeof(int) });
                if (mTop != null)
                {
                    if (mTop.IsStatic) return ArrayToObjectArray(mTop.Invoke(null, new object[] { 10 }));
                    var inst = CrearInstanciaServicio(t);
                    return ArrayToObjectArray(mTop.Invoke(inst, new object[] { 10 }));
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

            if (v is bool) return (bool)v;
            if (v is bool?) return ((bool?)v).GetValueOrDefault(false);
            return false;
        }

        internal static string ExtraerMensaje(object ro)
        {
            if (ro == null) return null;
            var p = ro.GetType().GetProperty("Mensaje")
                 ?? ro.GetType().GetProperty("Message")
                 ?? ro.GetType().GetProperty("Detalle")
                 ?? ro.GetType().GetProperty("Descripcion")
                 ?? ro.GetType().GetProperty("Error");
            if (p == null) return null;
            var v = p.GetValue(ro);
            return v?.ToString();
        }
    }
}