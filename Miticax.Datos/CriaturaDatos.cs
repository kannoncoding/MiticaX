//UNED
//Mitica X
//Jorge Arias Melendez
//Septiembre 2025
//Acceso a datos: arreglo y operaciones basicas para CriaturaEntidad

using Miticax.Entidades;

namespace Miticax.Datos
{
    public class CriaturaDatos
    {
        // Arreglo de almacenamiento fijo para CriaturaEntidad.
        private readonly CriaturaEntidad[] _items = new CriaturaEntidad[ConstantesDatos.CapacidadCriaturas];

        // Contador de elementos realmente ocupados en el arreglo.
        private int _count = 0;

        // Inserta una criatura si hay espacio.
        public bool Insert(CriaturaEntidad item, out string error)
        {
            // Verifica espacio disponible comparando _count con la capacidad del arreglo.
            if (_count >= _items.Length)
            {
                error = "No se pueden ingresar mas registros (criaturas).";
                return false;
            }

            // Coloca el item en la posicion _count y luego incrementa el contador.
            _items[_count] = item;
            _count++;

            error = string.Empty;
            return true;
        }

        // Busca linealmente por IdCriatura. Retorna null si no encuentra coincidencias.
        public CriaturaEntidad? FindById(int idCriatura)
        {
            // Recorre solo hasta _count para evitar revisar posiciones vacias.
            for (int i = 0; i < _count; i++)
            {
                if (_items[i] != null && _items[i].IdCriatura == idCriatura)
                {
                    return _items[i];
                }
            }
            return null;
        }

        // Devuelve un clon superficial de los elementos ocupados (snapshot inmutable para el exterior).
        public CriaturaEntidad[] GetAllSnapshot()
        {
            // Crea un nuevo arreglo del tamano exacto de lo almacenado.
            var copia = new CriaturaEntidad[_count];
            // Copia con un for para cumplir la restriccion de no usar helpers externos.
            for (int i = 0; i < _count; i++)
            {
                copia[i] = _items[i];
            }
            return copia;
        }

        // Helper: obtiene el indice dentro del arreglo por Id (o -1 si no existe).
        public int IndexOfById(int idCriatura)
        {
            for (int i = 0; i < _count; i++)
            {
                if (_items[i] != null && _items[i].IdCriatura == idCriatura)
                {
                    return i;
                }
            }
            return -1;
        }

        // Expone solo lectura del conteo actual.
        public int Count()
        {
            return _count;
        }
    }
}
