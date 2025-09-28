//UNED
//Mitica X
//Jorge Arias Melendez
//Septiembre 2025
//Acceso a datos: arreglo y operaciones basicas para JugadorEntidad

using Miticax.Entidades;

namespace Miticax.Datos
{
    public class JugadorDatos
    {
        private readonly JugadorEntidad[] _items = new JugadorEntidad[ConstantesDatos.CapacidadJugadores];
        private int _count = 0;

        // Inserta un jugador si hay espacio.
        public bool Insert(JugadorEntidad item, out string error)
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
        public JugadorEntidad? FindById(int idJugador)
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
        public JugadorEntidad[] GetAllSnapshot()
        {
            var copia = new JugadorEntidad[_count];
            for (int i = 0; i < _count; i++)
            {
                copia[i] = _items[i];
            }
            return copia;
        }

        // Helper: indice por Id.
        public int IndexOfById(int idJugador)
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

        public int Count()
        {
            return _count;
        }
    }
}
