//UNED
//Mitica X
//Jorge Arias Melendez
//Septiembre 2025
//Acceso a datos: arreglo y operaciones basicas para RondaEntidad (rondas de batalla)

using Miticax.Entidades;

namespace Miticax.Datos
{
    public static class RondaDatos
    {
        // Ahora el arreglo soporta CapacidadBatallas * MaxRondasPorBatalla (best-of-3).
        private static readonly RondaEntidad[] _items = new RondaEntidad[ConstantesDatos.CapacidadRondas];

        // Contador real de rondas usadas.
        private static int _count = 0;

        // Inserta una ronda si hay espacio total suficiente.
        public static bool Insert(RondaEntidad item, out string error)
        {
            if (_count >= _items.Length)
            {
                error = "No se pueden ingresar mas registros (rondas): capacidad total alcanzada.";
                return false;
            }

            _items[_count] = item;
            _count++;

            error = string.Empty;
            return true;
        }

        // Busca una ronda por clave logica: IdBatalla + IdRonda.
        public static RondaEntidad? FindByBatallaAndRonda(int idBatalla, int idRonda)
        {
            for (int i = 0; i < _count; i++)
            {
                var it = _items[i];
                if (it != null && it.IdBatalla == idBatalla && it.IdRonda == idRonda)
                {
                    return it;
                }
            }
            return null;
        }

        // Snapshot de todas las rondas almacenadas.
        public static RondaEntidad[] GetAllSnapshot()
        {
            var copia = new RondaEntidad[_count];
            for (int i = 0; i < _count; i++)
            {
                copia[i] = _items[i];
            }
            return copia;
        }

        // Helper: rondas por batalla.
        public static RondaEntidad[] FindAllByBatallaId(int idBatalla)
        {
            int coincidencias = 0;
            for (int i = 0; i < _count; i++)
            {
                var it = _items[i];
                if (it != null && it.IdBatalla == idBatalla)
                {
                    coincidencias++;
                }
            }

            var resultado = new RondaEntidad[coincidencias];
            int k = 0;
            for (int i = 0; i < _count; i++)
            {
                var it = _items[i];
                if (it != null && it.IdBatalla == idBatalla)
                {
                    resultado[k] = it;
                    k++;
                }
            }
            return resultado;
        }

        // Helper: rondas donde participa un jugador (como jugador1 o jugador2).
        public static RondaEntidad[] FindAllByJugadorId(int idJugador)
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

            var resultado = new RondaEntidad[coincidencias];
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

        public static int Count()
        {
            return _count;
        }
    }
}

