//UNED
//Mitica X
//Jorge Arias Melendez
//Septiembre 2025
//Entidad que representa una criatura del catalogo (POCO; solo propiedades).

using System;

namespace Miticax.Entidades
{
    // Clase de entidad sin logica; solo propiedades automaticas (POCO).
    public class CriaturaEntidad
    {
        // Identificador unico de la criatura.
        public int IdCriatura { get; set; }

        // Nombre visible de la criatura.
        public string Nombre { get; set; } = string.Empty;

        // Tipo elemental permitido: "agua", "tierra", "aire", "fuego".
        public string Tipo { get; set; } = string.Empty;

        // Nivel de criatura (1..5): 1=Iniciado, 2=Aprendiz, 3=Estudiante, 4=Avanzado, 5=Maestro.
        public int Nivel { get; set; }

        // Poder base de la criatura.
        public int Poder { get; set; }

        // Resistencia base de la criatura.
        public int Resistencia { get; set; }

        // Costo en cristales segun rango por nivel.
        public int Costo { get; set; }
    }
}