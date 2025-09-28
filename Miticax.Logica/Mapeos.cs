//UNED
//Mitica X
//Jorge Arias Melendez
//Septiembre 2025
//Mapeos y constantes de juego (niveles, tipos y rangos de costo)

using System;

namespace Miticax.Logica
{
    // Clase estatica con utilidades de mapeo y constantes del dominio.
    public static class Mapeos
    {
        // Tipos permitidos de criatura en minusculas para comparacion.
        private static readonly string[] _tiposPermitidos = new string[] { "agua", "tierra", "aire", "fuego" };

        // Normaliza un tipo (minusc y trim).
        public static string NormalizarTipo(string tipo)
        {
            if (tipo == null) return "";
            // Quitar espacios y pasar a minusculas
            return tipo.Trim().ToLowerInvariant();
        }

        // Verifica si un tipo es permitido.
        public static bool EsTipoValido(string tipo)
        {
            string t = NormalizarTipo(tipo);
            // Recorrido lineal en arreglo (sin LINQ)
            for (int i = 0; i < _tiposPermitidos.Length; i++)
            {
                if (_tiposPermitidos[i] == t) return true;
            }
            return false;
        }

        // Etiquetas de nivel de criatura (1..5) segun enunciado.
        public static string EtiquetaNivelCriatura(int nivel)
        {
            // Mapea numero a etiqueta de criatura
            switch (nivel)
            {
                case 1: return "Iniciado";
                case 2: return "Aprendiz";
                case 3: return "Estudiante";
                case 4: return "Avanzado";
                case 5: return "Maestro";
                default: return "Desconocido";
            }
        }

        // Rango de costo permitido por nivel de criatura.
        // Devuelve true si costo es valido para el nivel (segun tabla).
        public static bool CostoValidoParaNivel(int nivel, int costo)
        {
            // Tabla del PDF:
            // Iniciado <100
            // Aprendiz [100,300)
            // Estudiante [300,600)
            // Avanzado [600,1200)
            // Maestro >=1200
            if (costo < 0) return false; // regla general: nunca costos negativos

            if (nivel == 1) return costo >= 0 && costo < 100;
            if (nivel == 2) return costo >= 100 && costo < 300;
            if (nivel == 3) return costo >= 300 && costo < 600;
            if (nivel == 4) return costo >= 600 && costo < 1200;
            if (nivel == 5) return costo >= 1200;
            return false;
        }

        // Reglas de nivel de jugador por batallas ganadas.
        // 0 -> Novato
        // >=5 Estudiante
        // >=10 Maestro
        // >=20 Supremo
        public static int CalcularNivelJugadorPorVictorias(int batallasGanadas)
        {
            if (batallasGanadas >= 20) return 4; // Supremo
            if (batallasGanadas >= 10) return 3; // Maestro
            if (batallasGanadas >= 5) return 2; // Estudiante
            return 1;                             // Novato
        }

        // Etiqueta de nivel de jugador (1..4).
        public static string EtiquetaNivelJugador(int nivel)
        {
            switch (nivel)
            {
                case 1: return "Novato";
                case 2: return "Estudiante";
                case 3: return "Maestro";
                case 4: return "Supremo";
                default: return "Desconocido";
            }
        }
    }
}
