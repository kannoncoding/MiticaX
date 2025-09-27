//UNED
//Mitica X
//Jorge Arias Melendez
//Septiembre 2025
//Acceso a datos: arreglo y operaciones basicas para EquipoEntidad

using Miticax.Entidades;

namespace Miticax.Datos
{
    public static class EquipoDatos
    {
        private static readonly EquipoEntidad[] _items = new EquipoEntidad[ConstantesDatos.CapacidadEquipos];
        private static int _count = 0;

        // Inserta un equipo si hay espacio.
        public static bool Insert(EquipoEntidad item, out string error)
        {
            if (_count >= _items.Length)
            {
                error = "No se pueden ingresar mas registros (equipos).";
                return false;
            }

            _items[_count] = item;
            _count++;

            error = string.Empty;
            return true;
        }

        // Busca por IdEquipo.
        public static EquipoEntidad? FindById(int idEquipo)
        {
            for (int i = 0; i < _count; i++)
            {
                if (_items[i] != null && _items[i].IdEquipo == idEquipo)
                {
                    return _items[i];
                }
            }
            return null;
        }

        // Snapshot de todos los equipos.
        public static EquipoEntidad[] GetAllSnapshot()
        {
            var copia = new EquipoEntidad[_count];
            for (int i = 0; i < _count; i++)
            {
                copia[i] = _items[i];
            }
            return copia;
        }

        // Helper: equipos de un jugador.
        public static EquipoEntidad[] FindAllByJugadorId(int idJugador)
        {
            int coincidencias = 0;
            for (int i = 0; i < _count; i++)
            {
                var it = _items[i];
                if (it != null && it.IdJugador == idJugador)
                {
                    coincidencias++;
                }
            }

            var resultado = new EquipoEntidad[coincidencias];
            int k = 0;
            for (int i = 0; i < _count; i++)
            {
                var it = _items[i];
                if (it != null && it.IdJugador == idJugador)
                {
                    resultado[k] = it;
                    k++;
                }
            }
            return resultado;
        }

        public static int IndexOfById(int idEquipo)
        {
            for (int i = 0; i < _count; i++)
            {
                if (_items[i] != null && _items[i].IdEquipo == idEquipo)
                {
                    return i;
                }
            }
            return -1;
        }

        public static int Count()
        {
            return _count;
        }
    }
}
