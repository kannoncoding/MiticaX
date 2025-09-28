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
        // ---- Singletons LAZY de Datos compartidos por toda la UI ----
        private static JugadorDatos _jugadorDatos;
        private static CriaturaDatos _criaturaDatos;
        private static InventarioDatos _inventarioDatos;
        private static EquipoDatos _equipoDatos;
        private static BatallaDatos _batallaDatos;
        private static RondaDatos _rondaDatos;

        internal static JugadorDatos JugadorDatos() => _jugadorDatos ??= new JugadorDatos();
        internal static CriaturaDatos CriaturaDatos() => _criaturaDatos ??= new CriaturaDatos();
        internal static InventarioDatos InventarioDatos() => _inventarioDatos ??= new InventarioDatos();
        internal static EquipoDatos EquipoDatos() => _equipoDatos ??= new EquipoDatos();
        internal static BatallaDatos BatallaDatos() => _batallaDatos ??= new BatallaDatos();
        internal static RondaDatos RondaDatos() => _rondaDatos ??= new RondaDatos();

        // ---- Factoría generica por reflexion: encuentra un ctor cuyas dependencias esten disponibles ----
        private static T CrearServicio<T>(params object[] deps) where T : class
        {
            var t = typeof(T);
            // Probar ctors del mas "largo" al mas corto
            var ctors = t.GetConstructors();
            Array.Sort(ctors, (a, b) => b.GetParameters().Length.CompareTo(a.GetParameters().Length));

            foreach (var ctor in ctors)
            {
                var pars = ctor.GetParameters();
                var args = new object[pars.Length];
                bool ok = true;

                for (int i = 0; i < pars.Length; i++)
                {
                    var pt = pars[i].ParameterType;
                    object match = null;

                    // buscar un candidato asignable por tipo
                    for (int j = 0; j < deps.Length; j++)
                    {
                        var d = deps[j];
                        if (d != null && pt.IsInstanceOfType(d)) { match = d; break; }
                    }

                    if (match == null) { ok = false; break; }
                    args[i] = match;
                }

                if (ok)
                {
                    return (T)ctor.Invoke(args);
                }
            }

            // Si existe ctor sin parametros, usarlo
            var ctor0 = t.GetConstructor(Type.EmptyTypes);
            if (ctor0 != null) return (T)Activator.CreateInstance(t);

            throw new InvalidOperationException("No fue posible construir el servicio " + typeof(T).FullName);
        }

        // ---- Singletons LAZY de Servicios (inyectan Datos compartidos; orden de ctor descubierto por reflexion) ----
        private static Miticax.Logica.JugadorService _jugadorService;
        private static Miticax.Logica.CriaturaService _criaturaService;
        private static Miticax.Logica.InventarioService _inventarioService;
        private static Miticax.Logica.EquipoService _equipoService;
        private static Miticax.Logica.RondaService _rondaService;
        private static Miticax.Logica.BatallaService _batallaService;
        private static Miticax.Logica.RankingService _rankingService;

        internal static Miticax.Logica.JugadorService JugadorService()
            => _jugadorService ??= CrearServicio<Miticax.Logica.JugadorService>(JugadorDatos());

        internal static Miticax.Logica.CriaturaService CriaturaService()
            => _criaturaService ??= CrearServicio<Miticax.Logica.CriaturaService>(CriaturaDatos());

        internal static Miticax.Logica.InventarioService InventarioService()
            => _inventarioService ??= CrearServicio<Miticax.Logica.InventarioService>(InventarioDatos(), JugadorDatos(), CriaturaDatos());

        internal static Miticax.Logica.EquipoService EquipoService()
            => _equipoService ??= CrearServicio<Miticax.Logica.EquipoService>(EquipoDatos(), JugadorDatos(), InventarioDatos());

        internal static Miticax.Logica.RondaService RondaService()
            // algunos diseños requieren varias dependencias; se entregan todas las que tenemos y la factoría elige
            => _rondaService ??= CrearServicio<Miticax.Logica.RondaService>(RondaDatos(), InventarioDatos(), JugadorDatos(), BatallaDatos(), EquipoDatos());

        internal static Miticax.Logica.BatallaService BatallaService()
            => _batallaService ??= CrearServicio<Miticax.Logica.BatallaService>(BatallaDatos(), JugadorDatos(), EquipoDatos(), InventarioDatos(), RondaService());

        internal static Miticax.Logica.RankingService RankingService()
            => _rankingService ??= CrearServicio<Miticax.Logica.RankingService>(JugadorDatos());

        // ---- Utilidades internas seguras ----
        private static bool TryGetIntPropertyValue(object obj, string propertyName, out int value)
        {
            value = 0;
            if (obj == null) return false;
            var p = obj.GetType().GetProperty(propertyName);
            if (p == null) return false;

            var v = p.GetValue(obj);
            if (v == null) return false;

            try
            {
                value = Convert.ToInt32(v);
                return true;
            }
            catch { return false; }
        }

        private static object[] ArrayToObjectArray(object arrayObj)
        {
            if (arrayObj == null) return new object[0];
            var arr = arrayObj as Array;
            if (arr == null) return new object[0];
            int n = arr.Length;
            var res = new object[n];
            for (int i = 0; i < n; i++) res[i] = arr.GetValue(i);
            return res;
        }

        // ---- Filtros con arrays (sin LINQ ni listas) ----
        internal static T[] FiltrarPorCampoIgual<T>(T[] fuente, string nombreCampoInt, int valor)
        {
            if (fuente == null) return new T[0];

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

        // ---- Utilidades Batalla (usar instancia real del servicio, no Activator directo) ----
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
            var srv = BatallaService();
            var t = srv.GetType();

            // RegistrarBatalla(int,int,int,int, out string)
            var m = t.GetMethod("RegistrarBatalla", new Type[] { typeof(int), typeof(int), typeof(int), typeof(int), typeof(string).MakeByRefType() });
            if (m != null)
            {
                object[] pars = new object[] { j1, e1, j2, e2, null };
                var ro = m.Invoke(srv, pars);
                mensaje = ExtraerMensaje(ro) ?? (pars[4] as string) ?? "";
                return ExtraerExito(ro);
            }

            // RegistrarBatalla(BatallaEntidad, out string)
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
                var ro = me.Invoke(srv, pars);
                mensaje = ExtraerMensaje(ro) ?? (pars[1] as string) ?? "";
                return ExtraerExito(ro);
            }

            mensaje = "No fue posible invocar RegistrarBatalla.";
            return false;
        }

        internal static bool TryEjecutarBatalla(int idBatalla, out string mensaje)
        {
            mensaje = "";
            var srv = BatallaService();
            var t = srv.GetType();

            // EjecutarBatalla(int, out string)
            var m1 = t.GetMethod("EjecutarBatalla", new Type[] { typeof(int), typeof(string).MakeByRefType() });
            if (m1 != null)
            {
                object[] pars = new object[] { idBatalla, null };
                var ro = m1.Invoke(srv, pars);
                mensaje = ExtraerMensaje(ro) ?? (pars[1] as string) ?? "";
                return ExtraerExito(ro);
            }

            // EjecutarBatalla(int, Random, out string)
            var m2 = t.GetMethod("EjecutarBatalla", new Type[] { typeof(int), typeof(Random), typeof(string).MakeByRefType() });
            if (m2 != null)
            {
                object[] pars = new object[] { idBatalla, new Random(), null };
                var ro = m2.Invoke(srv, pars);
                mensaje = ExtraerMensaje(ro) ?? (pars[2] as string) ?? "";
                return ExtraerExito(ro);
            }

            mensaje = "No fue posible invocar EjecutarBatalla.";
            return false;
        }

        internal static object[] TopRanking10()
        {
            var srv = RankingService();
            var t = srv.GetType();

            var m = t.GetMethod("Top10", Type.EmptyTypes)
                  ?? t.GetMethod("Top100", Type.EmptyTypes);

            if (m != null)
                return ArrayToObjectArray(m.Invoke(srv, null));

            var mTop = t.GetMethod("Top", new Type[] { typeof(int) });
            if (mTop != null)
                return ArrayToObjectArray(mTop.Invoke(srv, new object[] { 10 }));

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
