//UNED
//Mitica X
//Jorge Arias Melendez
//Septiembre 2025
//Acceso a datos: arreglo y operaciones basicas para RondaEntidad (rondas de batalla)

using Miticax.Entidades;

namespace Miticax.Datos
{
    public class RondaDatos
    {
        // Arreglo fisico de rondas (capacidad fija definida en constantes).
        private readonly RondaEntidad[] _items = new RondaEntidad[ConstantesDatos.CapacidadRondas];

        // Contador real de rondas usadas (segmento valido: [0, _count)).
        private int _count = 0;

        // Inserta una ronda si hay espacio total suficiente.
        public bool Insert(RondaEntidad item, out string error)
        {
            // Verifica que haya espacio disponible
            if (_count >= _items.Length)
            {
                error = "No se pueden ingresar mas registros (rondas): capacidad total alcanzada.";
                return false;
            }

            // Inserta en la posicion libre actual
            _items[_count] = item;

            // Avanza el contador
            _count++;

            // Sin errores
            error = string.Empty;
            return true;
        }

        // Busca una ronda por clave logica: IdBatalla + IdRonda.
        public RondaEntidad? FindByBatallaAndRonda(int idBatalla, int idRonda)
        {
            // Recorre el segmento ocupado unicamente
            for (int i = 0; i < _count; i++)
            {
                var it = _items[i];
                if (it.IdBatalla == idBatalla && it.IdRonda == idRonda)
                {
                    return it;
                }
            }
            return null;
        }

        // Snapshot de todas las rondas almacenadas (longitud exacta = _count).
        public RondaEntidad[] GetAllSnapshot()
        {
            var copia = new RondaEntidad[_count];        // crea arreglo exacto
            for (int i = 0; i < _count; i++)             // copia referencias
            {
                copia[i] = _items[i];
            }
            return copia;
        }

        // Helper: rondas por batalla (longitud exacta).
        public RondaEntidad[] FindAllByBatallaId(int idBatalla)
        {
            // Primera pasada: contar coincidencias
            int coincidencias = 0;
            for (int i = 0; i < _count; i++)
            {
                var it = _items[i];
                if (it.IdBatalla == idBatalla)
                {
                    coincidencias++;
                }
            }

            // Segunda pasada: construir resultado exacto
            var resultado = new RondaEntidad[coincidencias];
            int k = 0;
            for (int i = 0; i < _count; i++)
            {
                var it = _items[i];
                if (it.IdBatalla == idBatalla)
                {
                    resultado[k] = it;
                    k++;
                }
            }
            return resultado;
        }

        // Helper: rondas donde participa un jugador (como jugador1 o jugador2).
        public RondaEntidad[] FindAllByJugadorId(int idJugador)
        {
            int coincidencias = 0;
            for (int i = 0; i < _count; i++)
            {
                var it = _items[i];
                if (it.IdJugador1 == idJugador || it.IdJugador2 == idJugador)
                {
                    coincidencias++;
                }
            }

            var resultado = new RondaEntidad[coincidencias];
            int k = 0;
            for (int i = 0; i < _count; i++)
            {
                var it = _items[i];
                if (it.IdJugador1 == idJugador || it.IdJugador2 == idJugador)
                {
                    resultado[k] = it;
                    k++;
                }
            }
            return resultado;
        }

        // Devuelve la cantidad de rondas almacenadas en ESTA instancia.
        public int Count()
        {
            return _count;
        }

        // Devuelve la capacidad total del arreglo de ESTA instancia.
        public int CapacidadTotal()
        {
            return _items.Length;
        }

        // Devuelve cuantos espacios libres quedan en ESTA instancia.
        public int CapacidadRestante()
        {
            return _items.Length - _count;
        }

        // Elimina el ultimo registro SOLO si pertenece a la batalla indicada (modo LIFO seguro para rollback).
        // Devuelve true si logro remover; false si el ultimo no pertenece a esa batalla o no hay elementos.
        public bool RemoveLastIfMatch(int idBatalla)
        {
            // Si no hay elementos, no hay nada que remover
            if (_count <= 0) return false;

            // Ultimo indice ocupado
            int last = _count - 1;

            // Verifica que la ultima ronda insertada corresponda a la batalla indicada
            if (_items[last].IdBatalla != idBatalla) return false;

            // Limpia referencia (opcional) y decrementa contador
            _items[last] = null;
            _count--;

            return true;
        }
    }
}
