//UNED
//Mitica X
//Jorge Arias Melendez
//Septiembre 2025
//Resolver rondas, registrar sin recompensas, aplicar recompensas y utilidades transaccionales

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

        public RondaService(RondaDatos rondaDatos, InventarioDatos inventarioDatos, JugadorDatos jugadorDatos)
        {
            _rondaDatos = rondaDatos;
            _inventarioDatos = inventarioDatos;
            _jugadorDatos = jugadorDatos;
        }

        // registra una ronda SIN tocar cristales ni inventario
        public ResultadoOperacion RegistrarRonda(RondaEntidad ronda, out string errorDatos)
        {
            errorDatos = "";

            // Validaciones minimas de integridad
            if (ronda.IdRonda < 1 || ronda.IdRonda > 3) return ResultadoOperacion.Fail("IdRonda debe ser 1, 2 o 3");
            if (!Validaciones.IdPositivo(ronda.IdBatalla)) return ResultadoOperacion.Fail("IdBatalla no valido");

            // NUEVO: validar que el ganador pertenezca a la ronda (debe ser uno de los dos jugadores)
            if (ronda.GanadorRonda != ronda.IdJugador1 && ronda.GanadorRonda != ronda.IdJugador2)
            {
                return ResultadoOperacion.Fail("GanadorRonda invalido: debe coincidir con IdJugador1 o IdJugador2 de la ronda");
            }

            // Intentar insertar la ronda
            bool ok = _rondaDatos.Insert(ronda, out errorDatos);
            if (!ok) return ResultadoOperacion.Fail(errorDatos);

            return ResultadoOperacion.Ok();
        }

        // aplica solo recompensas por una ronda ya registrada
        public void AplicarRecompensasRonda(RondaEntidad ronda, int poderGanadorIncremento)
        {
            // NUEVO: verificacion defensiva (no otorgar recompensas si el ganador no pertenece a la ronda)
            if (ronda.GanadorRonda != ronda.IdJugador1 && ronda.GanadorRonda != ronda.IdJugador2)
            {
                // Datos inconsistentes: no hacer nada.
                return;
            }

            // +10 cristales al ganador de la ronda
            var ganador = _jugadorDatos.FindById(ronda.GanadorRonda);
            if (ganador != null)
            {
                ganador.Cristales += 10;
            }

            // +poder a la criatura ganadora en inventario
            // Si gano Jugador1, la criatura ganadora es IdCriatura1; si gano Jugador2, es IdCriatura2.
            int idJugadorGanador = ronda.GanadorRonda;
            int idCriaturaGanadora = (ronda.GanadorRonda == ronda.IdJugador1) ? ronda.IdCriatura1 : ronda.IdCriatura2;

            var snap = _inventarioDatos.GetAllSnapshot(); // snapshot exacto (sin nulos)
            for (int i = 0; i < snap.Length; i++)
            {
                var item = snap[i];
                if (item.IdJugador == idJugadorGanador && item.IdCriatura == idCriaturaGanadora)
                {
                    item.Poder += poderGanadorIncremento; // tipicamente +5
                    break;
                }
            }
        }

        // Mantenemos el metodo combinado (para otros flujos que no requieran transaccionalidad de multiples rondas).
        public ResultadoOperacion RegistrarRondaYRecompensas(RondaEntidad ronda, int poderGanadorIncremento, out string errorDatos)
        {
            errorDatos = "";

            if (ronda.IdRonda < 1 || ronda.IdRonda > 3) return ResultadoOperacion.Fail("IdRonda debe ser 1, 2 o 3");
            if (!Validaciones.IdPositivo(ronda.IdBatalla)) return ResultadoOperacion.Fail("IdBatalla no valido");

            bool ok = _rondaDatos.Insert(ronda, out errorDatos);
            if (!ok) return ResultadoOperacion.Fail(errorDatos);

            var ganador = _jugadorDatos.FindById(ronda.GanadorRonda);
            if (ganador != null)
            {
                ganador.Cristales += 10;
            }

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

            return ResultadoOperacion.Ok();
        }

        // Verifica si hay capacidad suficiente para registrar 'requeridas' rondas.
        public bool VerificarCapacidadRondas(int requeridas, out string error)
        {
            error = "";
            int libres = _rondaDatos.CapacidadRestante(); // consulta directa al repositorio
            if (libres < requeridas)
            {
                error = "No hay espacio suficiente para registrar las rondas requeridas (" + requeridas + "). Libres: " + libres + ".";
                return false;
            }
            return true;
        }

        // Rollback LIFO: intenta remover las ultimas 'rondasARevertir' rondas de idBatalla en el orden inverso de insercion.
        public void RollbackRondas(int idBatalla, int rondasARevertir)
        {
            // Intenta revertir una por una; se detiene si la ultima no corresponde a esa batalla
            for (int i = 0; i < rondasARevertir; i++)
            {
                bool ok = _rondaDatos.RemoveLastIfMatch(idBatalla);
                if (!ok) break; // si ya no coincide, detenemos (evita afectar otras batallas)
            }
        }

        // Resuelve una ronda dada la info de dos criaturas con sus stats de inventario y decide ganador.
        // Devuelve: idJugadorGanador.
        public int ResolverRonda(Random rng, int idJugador1, int poder1, int resistencia1, int idJugador2, int poder2, int resistencia2)
        {
            // 1) Atacante inicial aleatorio
            bool atacaPrimeroJ1 = (rng.Next(0, 2) == 0);

            // 2) Ataque inicial
            if (atacaPrimeroJ1)
            {
                int resta = resistencia2 - poder1;
                if (resta <= 0) return idJugador1; // gana j1
            }
            else
            {
                int resta = resistencia1 - poder2;
                if (resta <= 0) return idJugador2; // gana j2
            }

            // 3) Contraataque si no definio ganador
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

            // 4) Remanentes
            int rem1 = poder1 - resistencia2;
            int rem2 = poder2 - resistencia1;

            if (rem1 > rem2) return idJugador1;
            if (rem2 > rem1) return idJugador2;

            // 5) Desempate aleatorio si iguales
            return (rng.Next(0, 2) == 0) ? idJugador1 : idJugador2;
        }
    }
}
