//UNED
//Mitica X
//Jorge Arias Melendez
//Septiembre 2025
//Estructura simple para devolver exito/error desde servicios

namespace Miticax.Logica
{
    // Struct para respuestas de logica: exito y mensaje de error (si aplica).
    public struct ResultadoOperacion
    {
        public bool Exito;   // Indica si la operacion fue exitosa
        public string Error; // Mensaje de error si Exito=false

        // Crea un resultado exitoso.
        public static ResultadoOperacion Ok()
        {
            return new ResultadoOperacion { Exito = true, Error = "" };
        }

        // Crea un resultado con error.
        public static ResultadoOperacion Fail(string error)
        {
            if (error == null) error = "Error no especificado";
            return new ResultadoOperacion { Exito = false, Error = error };
        }
    }
}
