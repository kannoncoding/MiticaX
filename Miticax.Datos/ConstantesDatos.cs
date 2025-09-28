//UNED
//Mitica X
//Jorge Arias Melendez
//Septiembre 2025
//Constantes de capacidades de arreglos para la capa de datos

namespace Miticax.Datos
{
    // Clase estatica para centralizar capacidades y relaciones de tamano entre arreglos.
    public static class ConstantesDatos
    {
        // Catalogo de criaturas (20).
        public const int CapacidadCriaturas = 20;

        // Jugadores (20).
        public const int CapacidadJugadores = 20;

        // Inventario POR JUGADOR (30 por jugador).
        public const int CapacidadInventarioPorJugador = 30;

        // Equipos (40).
        public const int CapacidadEquipos = 40;

        // Batallas (50).
        public const int CapacidadBatallas = 50;

        // Rondas: best-of-3 por batalla.
        public const int MaxRondasPorBatalla = 3;

        // Capacidad total de rondas = batallas * rondas por batalla (ej. 50 * 3 = 150).
        public const int CapacidadRondas = CapacidadBatallas * MaxRondasPorBatalla;
    }
}
