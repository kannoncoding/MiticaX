//UNED
//Mitica X
//Jorge Arias Melendez
//Septiembre 2025
//Entidad que representa una criatura dentro del inventario de un jugador (POCO).

using System;

namespace Miticax.Entidades
{
    public class InventarioJugadorEntidad
    {
        public int IdJugador { get; set; }

        public int IdCriatura { get; set; }

        public int Poder { get; set; }

        public int Resistencia { get; set; }
    }
}
