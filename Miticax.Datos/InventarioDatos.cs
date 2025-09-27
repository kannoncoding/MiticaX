//UNED
//Mitica X
//Jorge Arias Melendez
//Septiembre 2025
//Acceso a datos: arreglo y operaciones basicas para InventarioJugadorEntidad

using Miticax.Entidades;

namespace Miticax.Datos
{
    public static class InventarioDatos
    {
        private static readonly InventarioJugadorEntidad[] _items = new InventarioJugadorEntidad[ConstantesDatos.CapacidadInventario];
        private static int _count = 0;

        // Inserta una fila de inventario si hay espacio.
        public static bool Insert(InventarioJugadorEntidad item, out string error)
        {
            if (_count >= _items.Length)
            {
                error = "No se pueden ingresar mas registros (inventario).";
                return false;
            }

            _items[_count] = item;
            _count++;

            error = string.Empty;
            return true;
        }

        // Busca un registro de inventario por una clave compuesta logica (IdJugador + IdCriatura).
        // Si necesitas FindById unico, ajusta cuando definas la clave primaria en Entidades/Logica.
        public static InventarioJugadorEntidad? FindByJugadorAndCriatura(int idJugador, int idCriatura)
        {
            for (int i = 0; i < _count; i++)
            {
                var it = _items[i];
                if (it != null && it.IdJugador == idJugador && it.IdCriatura == idCriatura)
                {
                    return it;
                }
            }
            return null;
        }

        // Retorna todos los registros del inventario (snapshot).
        public static InventarioJugadorEntidad[] GetAllSnapshot()
        {
            var copia = new InventarioJugadorEntidad[_count];
            for (int i = 0; i < _count; i++)
            {
                copia[i] = _items[i];
            }
            return copia;
        }

        // Helper: obtiene todos los items de un jugador como un nuevo arreglo ajustado al tamano real.
        public static InventarioJugadorEntidad[] FindAllByJugadorId(int idJugador)
        {
            // Primero contamos coincidencias para crear un arreglo del tamano exacto.
            int coincidencias = 0;
            for (int i = 0; i < _count; i++)
            {
                var it = _items[i];
                if (it != null && it.IdJugador == idJugador)
                {
                    coincidencias++;
                }
            }

            // Creamos el arreglo de salida y lo llenamos con un segundo recorrido.
            var resultado = new InventarioJugadorEntidad[coincidencias];
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

        // Helper: existe ya esa criatura en el inventario del jugador (para evitar duplicados a nivel datos si lo deseas).
        public static bool ExisteJugadorCriatura(int idJugador, int idCriatura)
        {
            return FindByJugadorAndCriatura(idJugador, idCriatura) != null;
        }

        public static int Count()
        {
            return _count;
        }
    }
}
