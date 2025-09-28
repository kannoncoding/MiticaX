//UNED
//Mitica X
//Jorge Arias Melendez
//Septiembre 2025
//Acceso a datos: arreglo y operaciones basicas para BatallaEntidad (almacenamiento por instancia, sin estado global)

using System;
using Miticax.Entidades;

namespace Miticax.Datos
{
    // Clase de datos de batallas con almacenamiento por INSTANCIA.
    // Importante: _items y _count ya NO son static; cada instancia es independiente (aislamiento para pruebas).
    public class BatallaDatos
    {
        // Arreglo fisico donde se almacenan las batallas de ESTA instancia.
        private readonly BatallaEntidad[] _items;

        // Cantidad de elementos ocupados dentro de _items (rango valido: [0, _items.Length)).
        private int _count;

        // Constructor por defecto: usa la capacidad configurada en ConstantesDatos.
        public BatallaDatos()
        {
            // Crea el arreglo con la capacidad fija definida para batallas.
            _items = new BatallaEntidad[ConstantesDatos.CapacidadBatallas];
            _count = 0; // inicia sin elementos
        }

        // Constructor alterno: permite especificar capacidad explicita (util para pruebas).
        public BatallaDatos(int capacidad)
        {
            // Si pasan un valor no valido, aseguramos al menos 1 para evitar excepciones.
            if (capacidad < 1) capacidad = 1;
            _items = new BatallaEntidad[capacidad];
            _count = 0;
        }

        // Inserta una batalla si hay espacio disponible.
        // Regresa true en exito y false en error; el mensaje va en 'error'.
        public bool Insert(BatallaEntidad item, out string error)
        {
            // Verifica espacio disponible en el arreglo.
            if (_count >= _items.Length)
            {
                error = "No se pueden ingresar mas registros (batallas): capacidad llena.";
                return false;
            }

            // Inserta la referencia de la entidad en la siguiente posicion libre.
            _items[_count] = item;

            // Incrementa el contador de elementos ocupados.
            _count++;

            // No hubo errores en la insercion.
            error = string.Empty;
            return true;
        }

        // Busca y devuelve la batalla con el IdBatalla indicado.
        // Si no existe, devuelve null.
        public BatallaEntidad? FindById(int idBatalla)
        {
            // Recorre unicamente el segmento ocupado del arreglo [0, _count).
            for (int i = 0; i < _count; i++)
            {
                // Compara por clave primaria de la entidad.
                if (_items[i].IdBatalla == idBatalla)
                {
                    // Devuelve la referencia almacenada (POCO en memoria).
                    return _items[i];
                }
            }

            // No se encontro coincidencia.
            return null;
        }

        // Devuelve un "snapshot" exacto de las batallas almacenadas en esta instancia.
        // El arreglo retornado tiene longitud EXACTA = _count (sin elementos nulos).
        public BatallaEntidad[] GetAllSnapshot()
        {
            // Crea un nuevo arreglo del tamaño exacto del contenido.
            var copia = new BatallaEntidad[_count];

            // Copia las referencias existentes desde _items hacia la copia.
            for (int i = 0; i < _count; i++)
            {
                copia[i] = _items[i];
            }

            // Retorna la copia para lectura segura (no altera el almacenamiento interno).
            return copia;
        }

        // Busca todas las batallas donde participa el jugador indicado (ya sea como Jugador1 o como Jugador2).
        // Retorna un arreglo con longitud exacta a la cantidad de coincidencias.
        public BatallaEntidad[] FindAllByJugadorId(int idJugador)
        {
            // Primer paso: contar coincidencias para dimensionar el resultado sin usar colecciones.
            int coincidencias = 0;
            for (int i = 0; i < _count; i++)
            {
                var it = _items[i];
                if (it.IdJugador1 == idJugador || it.IdJugador2 == idJugador)
                {
                    coincidencias++;
                }
            }

            // Segundo paso: crear el arreglo resultado con el tamaño exacto y llenarlo.
            var resultado = new BatallaEntidad[coincidencias];
            int k = 0;
            for (int i = 0; i < _count; i++)
            {
                var it = _items[i];
                if (it.IdJugador1 == idJugador || it.IdJugador2 == idJugador)
                {
                    resultado[k] = it;
                    k++; // avanza indice de llenado en el resultado
                }
            }

            // Retorna las coincidencias encontradas.
            return resultado;
        }

        // Busca todas las batallas que coinciden con una fecha (comparando solo la parte de fecha, no la hora).
        // Retorna un arreglo con longitud exacta a la cantidad de coincidencias.
        public BatallaEntidad[] FindAllByFecha(DateTime fecha)
        {
            // Normaliza la fecha a .Date para comparar solo año/mes/dia.
            DateTime f = fecha.Date;

            // Conteo de coincidencias.
            int coincidencias = 0;
            for (int i = 0; i < _count; i++)
            {
                var it = _items[i];
                if (it.Fecha.Date == f)
                {
                    coincidencias++;
                }
            }

            // Construye arreglo resultado del tamaño justo.
            var resultado = new BatallaEntidad[coincidencias];
            int k = 0;
            for (int i = 0; i < _count; i++)
            {
                var it = _items[i];
                if (it.Fecha.Date == f)
                {
                    resultado[k] = it;
                    k++;
                }
            }

            // Devuelve las batallas de esa fecha.
            return resultado;
        }

        // Devuelve la cantidad de batallas almacenadas en ESTA instancia.
        public int Count()
        {
            return _count;
        }

        // Devuelve la capacidad total del arreglo interno de ESTA instancia.
        public int CapacidadTotal()
        {
            return _items.Length;
        }

        // Devuelve cuantos espacios libres quedan en ESTA instancia.
        public int CapacidadRestante()
        {
            return _items.Length - _count;
        }
    }
}
