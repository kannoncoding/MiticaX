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
        // Matriz 2D fija: filas = jugadores, columnas = slots de inventario por jugador.
        // Solo se usan indices [fila, 0..countPorFila[fila)-1].
        private static readonly InventarioJugadorEntidad[,] _items =
            new InventarioJugadorEntidad[ConstantesDatos.CapacidadJugadores, ConstantesDatos.CapacidadInventarioPorJugador];

        // Mapeo interno: para cada fila (0..CapacidadJugadores-1) guardamos el IdJugador asignado a esa fila.
        // Valor -1 significa "fila libre/no asignada".
        private static readonly int[] _jugadorIdPorFila = InicializarJugadorIdPorFila();

        // Conteo de usados por fila (cantidad efectiva de items en cada jugador).
        private static readonly int[] _countPorFila = new int[ConstantesDatos.CapacidadJugadores];

        // Conteo total de filas asignadas hasta ahora (jugadores que ya tienen fila en inventario).
        private static int _filasUsadas = 0;

        // Inicializador de arreglo de ids con -1.
        private static int[] InicializarJugadorIdPorFila()
        {
            var arr = new int[ConstantesDatos.CapacidadJugadores];
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = -1; // -1 indica que la fila aun no esta asignada a ningun IdJugador
            }
            return arr;
        }

        // Obtiene (o crea) la fila asignada a un IdJugador.
        // Si no existe, asigna la siguiente fila libre, si hay espacio.
        private static int ObtenerOFijarFilaParaJugador(int idJugador, out string error)
        {
            // 1) Buscar si ya existe una fila con ese idJugador.
            for (int f = 0; f < _jugadorIdPorFila.Length; f++)
            {
                if (_jugadorIdPorFila[f] == idJugador)
                {
                    error = string.Empty;
                    return f; // fila encontrada
                }
            }

            // 2) No existe: asignar nueva fila si hay cupo.
            if (_filasUsadas >= ConstantesDatos.CapacidadJugadores)
            {
                error = "No hay mas espacio de filas de inventario: se alcanzo el maximo de jugadores con inventario.";
                return -1;
            }

            int nuevaFila = _filasUsadas;
            _jugadorIdPorFila[nuevaFila] = idJugador; // asigna el jugador a esta fila
            _countPorFila[nuevaFila] = 0;             // inicia con cero items
            _filasUsadas++;                           // avanzamos el contador de filas asignadas

            error = string.Empty;
            return nuevaFila;
        }

        // Inserta una fila de inventario (por jugador), respetando 30 por jugador.
        public static bool Insert(InventarioJugadorEntidad item, out string error)
        {
            // Obtiene o crea la fila para el jugador del item.
            int fila = ObtenerOFijarFilaParaJugador(item.IdJugador, out error);
            if (fila < 0)
            {
                // Error ya establecido por ObtenerOFijarFilaParaJugador.
                return false;
            }

            // Verifica capacidad por jugador.
            if (_countPorFila[fila] >= ConstantesDatos.CapacidadInventarioPorJugador)
            {
                error = "El inventario del jugador esta lleno (llego a su limite por jugador).";
                return false;
            }

            // Inserta en la primera posicion libre de esa fila.
            int col = _countPorFila[fila];
            _items[fila, col] = item;
            _countPorFila[fila] = col + 1;

            error = string.Empty;
            return true;
        }

        // Busca un registro de inventario por IdJugador + IdCriatura dentro de la fila del jugador.
        public static InventarioJugadorEntidad? FindByJugadorAndCriatura(int idJugador, int idCriatura)
        {
            // Encontrar fila asignada a este jugador.
            int fila = -1;
            for (int f = 0; f < _jugadorIdPorFila.Length; f++)
            {
                if (_jugadorIdPorFila[f] == idJugador)
                {
                    fila = f;
                    break;
                }
            }
            if (fila < 0) return null; // jugador aun no tiene inventario

            // Recorrer solo los elementos efectivos de la fila.
            int usados = _countPorFila[fila];
            for (int c = 0; c < usados; c++)
            {
                var it = _items[fila, c];
                if (it != null && it.IdCriatura == idCriatura)
                {
                    return it;
                }
            }
            return null;
        }

        // Retorna todos los registros del inventario (snapshot plano) de todos los jugadores.
        public static InventarioJugadorEntidad[] GetAllSnapshot()
        {
            // 1) Calcular total de elementos usados para dimensionar el arreglo plano.
            int total = 0;
            for (int f = 0; f < _filasUsadas; f++)
            {
                total += _countPorFila[f];
            }

            // 2) Copiar en un arreglo lineal del tamano exacto.
            var copia = new InventarioJugadorEntidad[total];
            int k = 0;
            for (int f = 0; f < _filasUsadas; f++)
            {
                int usados = _countPorFila[f];
                for (int c = 0; c < usados; c++)
                {
                    copia[k] = _items[f, c];
                    k++;
                }
            }
            return copia;
        }

        // Helper: obtiene todos los items de un jugador como arreglo ajustado.
        public static InventarioJugadorEntidad[] FindAllByJugadorId(int idJugador)
        {
            // Localizar fila
            int fila = -1;
            for (int f = 0; f < _jugadorIdPorFila.Length; f++)
            {
                if (_jugadorIdPorFila[f] == idJugador)
                {
                    fila = f;
                    break;
                }
            }
            if (fila < 0) return new InventarioJugadorEntidad[0]; // sin items

            int usados = _countPorFila[fila];
            var resultado = new InventarioJugadorEntidad[usados];
            for (int c = 0; c < usados; c++)
            {
                resultado[c] = _items[fila, c];
            }
            return resultado;
        }

        // Helper: existe ya esa criatura en el inventario del jugador.
        public static bool ExisteJugadorCriatura(int idJugador, int idCriatura)
        {
            return FindByJugadorAndCriatura(idJugador, idCriatura) != null;
        }

        // Conteo total de items en todos los jugadores.
        public static int CountTotal()
        {
            int total = 0;
            for (int f = 0; f < _filasUsadas; f++)
            {
                total += _countPorFila[f];
            }
            return total;
        }

        // Conteo de items para un jugador especifico (0 si aun no tiene fila).
        public static int CountPorJugador(int idJugador)
        {
            for (int f = 0; f < _jugadorIdPorFila.Length; f++)
            {
                if (_jugadorIdPorFila[f] == idJugador)
                {
                    return _countPorFila[f];
                }
            }
            return 0;
        }
    }
}
