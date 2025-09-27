//UNED
//Mitica X
//Jorge Arias Melendez
//Septiembre 2025
//Acceso a datos: arreglo y operaciones basicas para JugadorEntidad

using Miticax.Entidades;

namespace Miticax.Datos
{
    public static class JugadorDatos
    {
        private static readonly JugadorEntidad[] _items = new JugadorEntidad[ConstantesDatos.CapacidadJugadores];
        private static int _count = 0;

        // Inserta un jugador si hay espacio.
        public static bool Insert(JugadorEntidad item, out string error)
        {
            if (_count >= _items.Length)
            {
                error = "No se pueden ingresar mas registros (jugadores).";
                return false;
            }

            _items[_count] = item;
            _count++;

            error = string.Empty;
            return true;
        }

        // Busca por IdJugador.
        public static JugadorEntidad? FindById(int idJugador)
        {
            for (int i = 0; i < _count; i++)
            {
                if (_items[i] != null && _items[i].IdJugador == idJugador)
                {
                    return _items[i];
                }
            }
            return null;
        }

        // Snapshot de todos los jugadores almacenados.
        public static JugadorEntidad[] GetAllSnapshot()
        {
            var copia = new JugadorEntidad[_count];
            for (int i = 0; i < _count; i++)
            {
                copia[i] = _items[i];
            }
            return copia;
        }

        // Helper: indice por Id.
        public static int IndexOfById(int idJugador)
        {
            for (int i = 0; i < _count; i++)
            {
                if (_items[i] != null && _items[i].IdJugador == idJugador)
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
