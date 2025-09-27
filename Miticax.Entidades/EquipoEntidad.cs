//UNED
//Mitica X
//Jorge Arias Melendez
//Septiembre 2025
//Entidad que representa un equipo de combate de un jugador (POCO).

using System;

namespace Miticax.Entidades
{
    public class EquipoEntidad
    {
        public int IdEquipo { get; set; }

        public int IdJugador { get; set; }

        public int IdCriatura1 { get; set; }

        public int IdCriatura2 { get; set; }

        public int IdCriatura3 { get; set; }
    }
}

