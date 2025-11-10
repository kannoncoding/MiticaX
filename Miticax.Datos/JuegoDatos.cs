//UNED
//Mitica X
//Jorge Arias Melendez
//Tercer cuatrimestre 2025
//Capa de datos del juego (arreglos + CRUD basico) segun tus POCOs reales.

using System;
using Miticax.Entidades;

namespace Miticax.Datos
{
    // Maneja arreglos de Jugador, Criatura y Batalla. Sin List, sin LINQ.
    public class JuegoDatos
    {
        // Capacidades maximas (ajusta si necesitas).
        private const int MaxJugadores = 100;
        private const int MaxCriaturas = 100;
        private const int MaxBatallas = 200;

        // Arreglos de almacenamiento.
        private readonly JugadorEntidad[] jugadores;
        private readonly CriaturaEntidad[] criaturas;
        private readonly BatallaEntidad[] batallas;

        // Contadores y secuencias de ids.
        private int cantJugadores = 0;
        private int cantCriaturas = 0;
        private int cantBatallas = 0;
        private int sigIdJugador = 1;
        private int sigIdCriatura = 1;
        private int sigIdBatalla = 1;

        // Constructor: inicializa arreglos y datos base de criaturas para pruebas.
        public JuegoDatos()
        {
            jugadores = new JugadorEntidad[MaxJugadores];
            criaturas = new CriaturaEntidad[MaxCriaturas];
            batallas = new BatallaEntidad[MaxBatallas];

            // Seed minimo de criaturas (usa tus campos reales: Tipo, Nivel, Poder, Resistencia, Costo).
            AltaCriatura("Fenix Carmesi", "fuego", 3, 12, 8, 120);
            AltaCriatura("Golem de Bruma", "tierra", 3, 9, 12, 115);
            AltaCriatura("Serpiente Astral", "aire", 3, 10, 10, 118);
        }

        // ========================= Jugadores =========================

        // Alta de jugador con validaciones basicas (edad >= 10).
        public int AltaJugador(string nombre, DateTime fechaNac, int nivelInicial)
        {
            if (cantJugadores >= jugadores.Length) return -1; // sin espacio

            // Valida nombre.
            if (string.IsNullOrWhiteSpace(nombre)) return -2;

            // Valida edad >= 10.
            int edad = CalcularEdad(fechaNac, DateTime.Now);
            if (edad < 10) return -3;

            // Valida nivel (1..4). Si viene fuera de rango, clamp a 1..4.
            if (nivelInicial < 1) nivelInicial = 1;
            if (nivelInicial > 4) nivelInicial = 4;

            // Crea entidad con defaults.
            var ent = new JugadorEntidad
            {
                IdJugador = sigIdJugador++,
                Nombre = nombre.Trim(),
                FechaNacimiento = fechaNac,
                Cristales = 0,          // saldo inicial 0
                BatallasGanadas = 0,
                Nivel = nivelInicial
            };

            jugadores[cantJugadores++] = ent;
            return ent.IdJugador;
        }

        // Calculo de edad simple (anios completos).
        private int CalcularEdad(DateTime nacimiento, DateTime hoy)
        {
            int edad = hoy.Year - nacimiento.Year;
            if (hoy.Month < nacimiento.Month || (hoy.Month == nacimiento.Month && hoy.Day < nacimiento.Day))
                edad--;
            return edad;
        }

        // Buscar jugador por id (lineal).
        public JugadorEntidad? ObtenerJugadorPorId(int id)
        {
            for (int i = 0; i < cantJugadores; i++)
                if (jugadores[i].IdJugador == id) return jugadores[i];
            return null;
        }

        public int ContarJugadores() => cantJugadores;

        // ========================= Criaturas =========================

        // Alta interna (seed) de criaturas del catalogo.
        private int AltaCriatura(string nombre, string tipo, int nivel, int poder, int resistencia, int costo)
        {
            if (cantCriaturas >= criaturas.Length) return -1;

            var ent = new CriaturaEntidad
            {
                IdCriatura = sigIdCriatura++,
                Nombre = nombre,
                Tipo = tipo,
                Nivel = nivel,
                Poder = poder,
                Resistencia = resistencia,
                Costo = costo
            };
            criaturas[cantCriaturas++] = ent;
            return ent.IdCriatura;
        }

        public int ContarCriaturas() => cantCriaturas;

        public CriaturaEntidad? ObtenerCriaturaPorId(int id)
        {
            for (int i = 0; i < cantCriaturas; i++)
                if (criaturas[i].IdCriatura == id) return criaturas[i];
            return null;
        }

        // ========================= Batallas =========================

        // Crea una batalla segun POCO BatallaEntidad (usa tus campos: IdEquipo1/2).
        public int AltaBatalla(int idJugador1, int idEquipo1, int idJugador2, int idEquipo2, DateTime fecha)
        {
            if (cantBatallas >= batallas.Length) return -1;

            // Valida existencia de jugadores (equipos se validaran cuando exista EquipoDatos).
            if (ObtenerJugadorPorId(idJugador1) == null) return -2;
            if (ObtenerJugadorPorId(idJugador2) == null) return -3;

            var ent = new BatallaEntidad
            {
                IdBatalla = sigIdBatalla++,
                IdJugador1 = idJugador1,
                IdEquipo1 = idEquipo1,
                IdJugador2 = idJugador2,
                IdEquipo2 = idEquipo2,
                Ganador = null,
                Fecha = fecha
            };

            batallas[cantBatallas++] = ent;
            return ent.IdBatalla;
        }

        public int ContarBatallas() => cantBatallas;

        public BatallaEntidad? ObtenerBatallaPorId(int id)
        {
            for (int i = 0; i < cantBatallas; i++)
                if (batallas[i].IdBatalla == id) return batallas[i];
            return null;
        }

        // Marca ganador (1 => jugador1, 2 => jugador2) y acumula estadistica.
        public bool CerrarBatallaConGanador(int idBatalla, int ganadorFlag)
        {
            var b = ObtenerBatallaPorId(idBatalla);
            if (b == null) return false;
            if (ganadorFlag != 1 && ganadorFlag != 2) return false;

            int idGanador = ganadorFlag == 1 ? b.IdJugador1 : b.IdJugador2;
            b.Ganador = idGanador;

            var j = ObtenerJugadorPorId(idGanador);
            if (j != null) j.BatallasGanadas += 1;

            return true;
        }
    }
}
