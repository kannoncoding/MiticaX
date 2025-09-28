//UNED
//Mitica X
//Jorge Arias Melendez
//Septiembre 2025
//Validaciones de reglas (edad, ids, rangos y campos requeridos)

using System;

namespace Miticax.Logica
{
    // Clase estatica con validaciones reutilizables.
    public static class Validaciones
    {
        // Valida que un id sea positivo (>0)
        public static bool IdPositivo(int id)
        {
            return id > 0;
        }

        // Valida edad > 10 anios al momento actual.
        public static bool EdadMayorA10(DateTime fechaNacimiento, DateTime ahora)
        {
            // Calcula edad en anios sin librerias extra.
            int edad = ahora.Year - fechaNacimiento.Year;
            // Ajuste si aun no cumple anios este anio
            if (ahora.Month < fechaNacimiento.Month ||
                (ahora.Month == fechaNacimiento.Month && ahora.Day < fechaNacimiento.Day))
            {
                edad--;
            }
            return edad > 10;
        }

        // Validacion de costo por nivel de criatura (usa Mapeos).
        public static bool CostoPorNivelValido(int nivel, int costo)
        {
            return Mapeos.CostoValidoParaNivel(nivel, costo);
        }

        // Valida texto no nulo/ni vacio post-trim.
        public static bool TextoObligatorio(string valor)
        {
            if (valor == null) return false;
            return valor.Trim().Length > 0;
        }
    }
}
