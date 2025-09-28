//UNED
//Mitica X
//Jorge Arias Melendez
//Septiembre 2025
//Entidad de jugador (POCO; solo propiedades)

using System;

namespace Miticax.Entidades
{
    public class JugadorEntidad
    {
        // Identificador positivo
        public int IdJugador { get; set; }

        // Nombre obligatorio
        public string Nombre { get; set; } = "";

        // Fecha de nacimiento para validar >10 anios
        public DateTime FechaNacimiento { get; set; }

        // Moneda del juego
        public int Cristales { get; set; }

        // Estadistica acumulada
        public int BatallasGanadas { get; set; }

        // Nivel del jugador (1 Novato, 2 Estudiante, 3 Maestro, 4 Supremo)
        public int Nivel { get; set; }
    }
}
