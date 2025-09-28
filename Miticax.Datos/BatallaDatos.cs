//UNED
//Mitica X
//Jorge Arias Melendez
//Septiembre 2025
//Acceso a datos: arreglo y operaciones basicas para BatallaEntidad

using System;
using Miticax.Entidades;

namespace Miticax.Datos
{
    public class BatallaDatos
    {
        private static readonly BatallaEntidad[] _items = new BatallaEntidad[ConstantesDatos.CapacidadBatallas];
        private static int _count = 0;

        // Inserta una batalla si hay espacio (los checks de consistencia van en Logica).
        public bool Insert(BatallaEntidad item, out string error)
        {
            if (_count >= _items.Length)
            {
                error = "No se pueden ingresar mas registros (batallas).";
                return false;
            }

            _items[_count] = item;
            _count++;

            error = string.Empty;
            return true;
        }

        // Busca por IdBatalla.
        public BatallaEntidad? FindById(int idBatalla)
        {
            for (int i = 0; i < _count; i++)
            {
                if (_items[i] != null && _items[i].IdBatalla == idBatalla)
                {
                    return _items[i];
                }
            }
            return null;
        }

        // Snapshot de todas las batallas.
        public BatallaEntidad[] GetAllSnapshot()
        {
            var copia = new BatallaEntidad[_count];
            for (int i = 0; i < _count; i++)
            {
                copia[i] = _items[i];
            }
            return copia;
        }

        // Helper: batallas donde participa un jugador (como jugador1 o jugador2).
        public BatallaEntidad[] FindAllByJugadorId(int idJugador)
        {
            int coincidencias = 0;
            for (int i = 0; i < _count; i++)
            {
                var it = _items[i];
                if (it != null && (it.IdJugador1 == idJugador || it.IdJugador2 == idJugador))
                {
                    coincidencias++;
                }
            }

            var resultado = new BatallaEntidad[coincidencias];
            int k = 0;
            for (int i = 0; i < _count; i++)
            {
                var it = _items[i];
                if (it != null && (it.IdJugador1 == idJugador || it.IdJugador2 == idJugador))
                {
                    resultado[k] = it;
                    k++;
                }
            }
            return resultado;
        }

        // Helper: batallas por fecha exacta (si lo necesitas en UI/Logica).
        public BatallaEntidad[] FindAllByFecha(DateTime fecha)
        {
            int coincidencias = 0;
            for (int i = 0; i < _count; i++)
            {
                var it = _items[i];
                if (it != null && it.Fecha.Date == fecha.Date)
                {
                    coincidencias++;
                }
            }

            var resultado = new BatallaEntidad[coincidencias];
            int k = 0;
            for (int i = 0; i < _count; i++)
            {
                var it = _items[i];
                if (it != null && it.Fecha.Date == fecha.Date)
                {
                    resultado[k] = it;
                    k++;
                }
            }
            return resultado;
        }

        public int Count()
        {
            return _count;
        }
    }
}
