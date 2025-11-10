//UNED
//Mitica X
//Jorge Arias Melendez
//Septiembre 2025
//Resolver rondas, registrar con validaciones fuertes (batalla, jugadores, inventario), recompensas y utilidades transaccionales

using System;
using Miticax.Entidades;
using Miticax.Datos;

namespace Miticax.Logica
{
    public class RondaService
    {
        private readonly RondaDatos _rondaDatos;
        private readonly InventarioDatos _inventarioDatos;
        private readonly JugadorDatos _jugadorDatos;
        private readonly BatallaDatos _batallaDatos; // validar existencia/participantes de la batalla
        private readonly EquipoDatos _equipoDatos;

        // Constructor (asegurar inyectar BatallaDatos al crear el servicio)
        public RondaService(RondaDatos rondaDatos, InventarioDatos inventarioDatos, JugadorDatos jugadorDatos, BatallaDatos batallaDatos, EquipoDatos equipoDatos)
        {
            _rondaDatos = rondaDatos;
            _inventarioDatos = inventarioDatos;
            _jugadorDatos = jugadorDatos;
            _batallaDatos = batallaDatos;
            _equipoDatos = equipoDatos;
        }

        // -------------------------------------------------------------
        // RegistrarRonda: valida batalla, jugadores, inventario y unicidad
        // -------------------------------------------------------------
        public ResultadoOperacion RegistrarRonda(RondaEntidad ronda, out string errorDatos)
        {
            errorDatos = "";

            // 1) Validaciones basicas
            if (ronda.IdRonda < 1 || ronda.IdRonda > 3) return ResultadoOperacion.Fail("IdRonda debe ser 1, 2 o 3");
            if (!Validaciones.IdPositivo(ronda.IdBatalla)) return ResultadoOperacion.Fail("IdBatalla no valido");

            // 2) Ganador debe pertenecer a la ronda
            if (ronda.GanadorRonda != ronda.IdJugador1 && ronda.GanadorRonda != ronda.IdJugador2)
                return ResultadoOperacion.Fail("GanadorRonda invalido: debe coincidir con IdJugador1 o IdJugador2");

            // 3) La batalla debe existir
            var batalla = _batallaDatos.FindById(ronda.IdBatalla);
            if (batalla == null) return ResultadoOperacion.Fail("La batalla indicada no existe (IdBatalla=" + ronda.IdBatalla + ")");

            // 3.1) No registrar rondas si la batalla ya tiene ganador
            if (batalla.Ganador != 0)
                return ResultadoOperacion.Fail("La batalla ya finalizo; no se pueden registrar mas rondas (Ganador=" + batalla.Ganador + ")");

            // 4) Los jugadores de la ronda deben ser los de la batalla (en cualquier orden)
            bool parIgualDirecto = (ronda.IdJugador1 == batalla.IdJugador1 && ronda.IdJugador2 == batalla.IdJugador2);
            bool parIgualInvertido = (ronda.IdJugador1 == batalla.IdJugador2 && ronda.IdJugador2 == batalla.IdJugador1);
            if (!parIgualDirecto && !parIgualInvertido)
                return ResultadoOperacion.Fail("Los jugadores de la ronda no pertenecen a la batalla especificada");

            // 5) Verificar existencia de jugadores (defensivo)
            var j1 = _jugadorDatos.BuscarPorId(ronda.IdJugador1, out string errorJ1);
            if (j1 == null) return ResultadoOperacion.Fail("IdJugador1 no existe (Id=" + ronda.IdJugador1 + ")");
            var j2 = _jugadorDatos.BuscarPorId(ronda.IdJugador2, out string errorJ2);
            if (j2 == null) return ResultadoOperacion.Fail("IdJugador2 no existe (Id=" + ronda.IdJugador2 + ")");

            // 5.1) Cargar equipos de la batalla
            var equipo1 = _equipoDatos.FindById(batalla.IdEquipo1);
            var equipo2 = _equipoDatos.FindById(batalla.IdEquipo2);
            if (equipo1 == null || equipo2 == null) return ResultadoOperacion.Fail("Equipos de la batalla no existen");

            // Mapear que equipo corresponde a cada jugador de la ronda (reutilizando parIgualDirecto/parIgualInvertido)
            EquipoEntidad equipoDeJ1EnRonda;
            EquipoEntidad equipoDeJ2EnRonda;

            if (parIgualDirecto)
            {
                // Jugador1 de la ronda usa Equipo1 de la batalla; Jugador2 usa Equipo2
                equipoDeJ1EnRonda = equipo1;
                equipoDeJ2EnRonda = equipo2;
            }
            else // parIgualInvertido es true (ya se valido arriba)
            {
                // Jugador1 de la ronda usa Equipo2; Jugador2 usa Equipo1
                equipoDeJ1EnRonda = equipo2;
                equipoDeJ2EnRonda = equipo1;
            }

            // 5.2) Enforzar roster (las criaturas declaradas deben estar en sus equipos)
            if (!EsCriaturaDelEquipo(equipoDeJ1EnRonda, ronda.IdCriatura1))
                return ResultadoOperacion.Fail("IdCriatura1 no pertenece al roster del jugador1 para esta batalla");
            if (!EsCriaturaDelEquipo(equipoDeJ2EnRonda, ronda.IdCriatura2))
                return ResultadoOperacion.Fail("IdCriatura2 no pertenece al roster del jugador2 para esta batalla");

            // 6) Verificar que cada criatura pertenezca al jugador en inventario
            if (!PerteneceAlJugadorEnInventario(ronda.IdJugador1, ronda.IdCriatura1))
                return ResultadoOperacion.Fail("IdCriatura1 no pertenece al IdJugador1 en el inventario");
            if (!PerteneceAlJugadorEnInventario(ronda.IdJugador2, ronda.IdCriatura2))
                return ResultadoOperacion.Fail("IdCriatura2 no pertenece al IdJugador2 en el inventario");

            // 7) Evitar duplicado (IdBatalla, IdRonda)
            var existente = _rondaDatos.FindByBatallaAndRonda(ronda.IdBatalla, ronda.IdRonda);
            if (existente != null)
                return ResultadoOperacion.Fail("Ya existe la ronda " + ronda.IdRonda + " para la batalla " + ronda.IdBatalla);

            // 8) Insertar
            bool ok = _rondaDatos.Insert(ronda, out errorDatos);
            if (!ok) return ResultadoOperacion.Fail(errorDatos);

            return ResultadoOperacion.Ok();
        }


        // ---------------------------------------------------------------------
        // RegistrarRondaYRecompensas: mismas validaciones + otorgar recompensas
        // ---------------------------------------------------------------------
        public ResultadoOperacion RegistrarRondaYRecompensas(RondaEntidad ronda, int poderGanadorIncremento, out string errorDatos)
        {
            errorDatos = "";

            // 1) Basicas
            if (ronda.IdRonda < 1 || ronda.IdRonda > 3) return ResultadoOperacion.Fail("IdRonda debe ser 1, 2 o 3");
            if (!Validaciones.IdPositivo(ronda.IdBatalla)) return ResultadoOperacion.Fail("IdBatalla no valido");

            // 2) Ganador debe pertenecer a la ronda
            if (ronda.GanadorRonda != ronda.IdJugador1 && ronda.GanadorRonda != ronda.IdJugador2)
                return ResultadoOperacion.Fail("GanadorRonda invalido: debe coincidir con IdJugador1 o IdJugador2");

            // 3) La batalla debe existir
            var batalla = _batallaDatos.FindById(ronda.IdBatalla);
            if (batalla == null) return ResultadoOperacion.Fail("La batalla indicada no existe (IdBatalla=" + ronda.IdBatalla + ")");

            // 3.1) No permitir rondas si la batalla ya finalizo
            if (batalla.Ganador != 0)
                return ResultadoOperacion.Fail("La batalla ya finalizo; no se pueden registrar mas rondas (Ganador=" + batalla.Ganador + ")");

            // 4) Los jugadores de la ronda deben ser los de la batalla (cualquier orden)
            bool parIgualDirecto = (ronda.IdJugador1 == batalla.IdJugador1 && ronda.IdJugador2 == batalla.IdJugador2);
            bool parIgualInvertido = (ronda.IdJugador1 == batalla.IdJugador2 && ronda.IdJugador2 == batalla.IdJugador1);
            if (!parIgualDirecto && !parIgualInvertido)
                return ResultadoOperacion.Fail("Los jugadores de la ronda no pertenecen a la batalla especificada");

            // 5) Existencia de jugadores (defensivo)
            var j1 = _jugadorDatos.BuscarPorId(ronda.IdJugador1, out string errorJ1);
            if (j1 == null) return ResultadoOperacion.Fail("IdJugador1 no existe (Id=" + ronda.IdJugador1 + ")");
            var j2 = _jugadorDatos.BuscarPorId(ronda.IdJugador2, out string errorJ2);
            if (j2 == null) return ResultadoOperacion.Fail("IdJugador2 no existe (Id=" + ronda.IdJugador2 + ")");

            // 6) Cargar equipos de la batalla
            var equipo1 = _equipoDatos.FindById(batalla.IdEquipo1);
            var equipo2 = _equipoDatos.FindById(batalla.IdEquipo2);
            if (equipo1 == null || equipo2 == null) return ResultadoOperacion.Fail("Equipos de la batalla no existen");

            // 7) Mapear equipo correspondiente a cada jugador de la ronda
            EquipoEntidad equipoDeJ1EnRonda;
            EquipoEntidad equipoDeJ2EnRonda;
            if (parIgualDirecto)
            {
                equipoDeJ1EnRonda = equipo1; // J1 -> Equipo1
                equipoDeJ2EnRonda = equipo2; // J2 -> Equipo2
            }
            else
            {
                equipoDeJ1EnRonda = equipo2; // J1 -> Equipo2
                equipoDeJ2EnRonda = equipo1; // J2 -> Equipo1
            }

            // 8) Enforzar roster: las criaturas declaradas deben estar en esos equipos
            if (!EsCriaturaDelEquipo(equipoDeJ1EnRonda, ronda.IdCriatura1))
                return ResultadoOperacion.Fail("IdCriatura1 no pertenece al roster del jugador1 para esta batalla");
            if (!EsCriaturaDelEquipo(equipoDeJ2EnRonda, ronda.IdCriatura2))
                return ResultadoOperacion.Fail("IdCriatura2 no pertenece al roster del jugador2 para esta batalla");

            // 9) Verificar propiedad en inventario (evita criaturas “fantasma”)
            if (!PerteneceAlJugadorEnInventario(ronda.IdJugador1, ronda.IdCriatura1))
                return ResultadoOperacion.Fail("IdCriatura1 no pertenece al IdJugador1 en el inventario");
            if (!PerteneceAlJugadorEnInventario(ronda.IdJugador2, ronda.IdCriatura2))
                return ResultadoOperacion.Fail("IdCriatura2 no pertenece al IdJugador2 en el inventario");

            // 10) Evitar duplicado (IdBatalla, IdRonda)
            var existente = _rondaDatos.FindByBatallaAndRonda(ronda.IdBatalla, ronda.IdRonda);
            if (existente != null)
                return ResultadoOperacion.Fail("Ya existe la ronda " + ronda.IdRonda + " para la batalla " + ronda.IdBatalla);

            // 11) Insertar ronda
            bool ok = _rondaDatos.Insert(ronda, out errorDatos);
            if (!ok) return ResultadoOperacion.Fail(errorDatos);

            // 12) Recompensas por ronda
            var ganador = _jugadorDatos.BuscarPorId(ronda.GanadorRonda, out string error);
            if (ganador != null) ganador.Cristales += 10;

            int idJugadorGanador = ronda.GanadorRonda;
            int idCriaturaGanadora = (ronda.GanadorRonda == ronda.IdJugador1) ? ronda.IdCriatura1 : ronda.IdCriatura2;

            var snap = _inventarioDatos.GetAllSnapshot();
            for (int i = 0; i < snap.Length; i++)
            {
                var item = snap[i];
                if (item.IdJugador == idJugadorGanador && item.IdCriatura == idCriaturaGanadora)
                {
                    item.Poder += poderGanadorIncremento; // tipicamente +5
                    break;
                }
            }

            return ResultadoOperacion.Ok();
        }

        // Verifica si una criatura del inventario pertenece al jugador indicado.
        // Recorre snapshot exacto (sin nulls) para encontrar la pareja (idJugador, idCriatura).
        private bool PerteneceAlJugadorEnInventario(int idJugador, int idCriatura)
        {
            var snap = _inventarioDatos.GetAllSnapshot();
            for (int i = 0; i < snap.Length; i++)
            {
                var it = snap[i];
                if (it.IdJugador == idJugador && it.IdCriatura == idCriatura)
                {
                    return true; // encontrado
                }
            }
            return false; // no pertenece
        }

        // Verifica si una criatura esta en el roster del equipo
        private bool EsCriaturaDelEquipo(EquipoEntidad eq, int idCriatura)
        {
            // Inline checks sin LINQ ni colecciones
            if (eq.IdCriatura1 == idCriatura) return true;
            if (eq.IdCriatura2 == idCriatura) return true;
            if (eq.IdCriatura3 == idCriatura) return true;
            return false;
        }


        // Registra una ronda y otorga +10 cristales al ganador y +5 poder a la criatura ganadora en inventario.
        public ResultadoOperacion RegistrarRondaYRecompensasLegacy(RondaEntidad ronda, int poderGanadorIncremento, out string errorDatos)
        {
          
            return RegistrarRondaYRecompensas(ronda, poderGanadorIncremento, out errorDatos);
        }

        // Aplica SOLAMENTE recompensas por una ronda ya registrada.
        public void AplicarRecompensasRonda(RondaEntidad ronda, int poderGanadorIncremento)
        {
            // Verificacion defensiva para evitar recompensas incoherentes
            if (ronda.GanadorRonda != ronda.IdJugador1 && ronda.GanadorRonda != ronda.IdJugador2) return;

            var ganador = _jugadorDatos.BuscarPorId(ronda.GanadorRonda, out string errorJ);
            if (ganador != null) ganador.Cristales += 10;

            int idJugadorGanador = ronda.GanadorRonda;
            int idCriaturaGanadora = (ronda.GanadorRonda == ronda.IdJugador1) ? ronda.IdCriatura1 : ronda.IdCriatura2;

            var snap = _inventarioDatos.GetAllSnapshot();
            for (int i = 0; i < snap.Length; i++)
            {
                var item = snap[i];
                if (item.IdJugador == idJugadorGanador && item.IdCriatura == idCriaturaGanadora)
                {
                    item.Poder += poderGanadorIncremento;
                    break;
                }
            }
        }

        // Verifica capacidad en RondaDatos (usada por BatallaService para ejecucion transaccional)
        public bool VerificarCapacidadRondas(int requeridas, out string error)
        {
            error = "";
            int libres = _rondaDatos.CapacidadRestante();
            if (libres < requeridas)
            {
                error = "No hay espacio suficiente para registrar las rondas requeridas (" + requeridas + "). Libres: " + libres + ".";
                return false;
            }
            return true;
        }

        // Rollback LIFO: intenta remover las ultimas 'rondasARevertir' rondas de idBatalla
        public void RollbackRondas(int idBatalla, int rondasARevertir)
        {
            for (int i = 0; i < rondasARevertir; i++)
            {
                bool ok = _rondaDatos.RemoveLastIfMatch(idBatalla);
                if (!ok) break;
            }
        }

        // Resuelve una ronda dada la info de dos criaturas con sus stats de inventario y decide ganador.
        public int ResolverRonda(Random rng, int idJugador1, int poder1, int resistencia1, int idJugador2, int poder2, int resistencia2)
        {
            bool atacaPrimeroJ1 = (rng.Next(0, 2) == 0);

            if (atacaPrimeroJ1)
            {
                int resta = resistencia2 - poder1;
                if (resta <= 0) return idJugador1;
            }
            else
            {
                int resta = resistencia1 - poder2;
                if (resta <= 0) return idJugador2;
            }

            if (atacaPrimeroJ1)
            {
                int resta = resistencia1 - poder2;
                if (resta <= 0) return idJugador2;
            }
            else
            {
                int resta = resistencia2 - poder1;
                if (resta <= 0) return idJugador1;
            }

            int rem1 = poder1 - resistencia2;
            int rem2 = poder2 - resistencia1;

            if (rem1 > rem2) return idJugador1;
            if (rem2 > rem1) return idJugador2;

            return (rng.Next(0, 2) == 0) ? idJugador1 : idJugador2;
        }
    }
}
