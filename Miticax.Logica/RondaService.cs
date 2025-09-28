//UNED
//Mitica X
//Jorge Arias Melendez
//Septiembre 2025
//Resolver rondas segun las reglas del enunciado y registrar en datos

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

        // Registra una ronda y otorga +10 cristales al ganador y +5 poder a la criatura ganadora en inventario.
        public ResultadoOperacion RegistrarRondaYRecompensas(RondaEntidad ronda, int poderGanadorIncremento, out string errorDatos)
        {
            errorDatos = "";

            if (ronda.IdRonda < 1 || ronda.IdRonda > 3) return ResultadoOperacion.Fail("IdRonda debe ser 1, 2 o 3");
            if (!Validaciones.IdPositivo(ronda.IdBatalla)) return ResultadoOperacion.Fail("IdBatalla no valido");

            // Insertar ronda
            bool ok = _rondaDatos.Insert(ronda, out errorDatos);
            if (!ok) return ResultadoOperacion.Fail(errorDatos);

            // Otorgar +10 cristales al ganador de la ronda
            var ganador = _jugadorDatos.FindById(ronda.GanadorRonda);
            if (ganador != null)
            {
                ganador.Cristales += 10;
            }

            // +5 poder a la criatura ganadora segun el ganador de la ronda
            int idJugadorGanador = ronda.GanadorRonda;
            int idCriaturaGanadora = (ronda.GanadorRonda == ronda.IdJugador1) ? ronda.IdCriatura1 : ronda.IdCriatura2;

            // Buscar en inventario y subir poder
            var snap = _inventarioDatos.GetAllSnapshot();
            for (int i = 0; i < snap.Length; i++)
            {
                var item = snap[i];
                if (item == null) break;
                if (item.IdJugador == idJugadorGanador && item.IdCriatura == idCriaturaGanadora)
                {
                    item.Poder += poderGanadorIncremento; // tipicamente +5
                    break;
                }
            }

            return ResultadoOperacion.Ok();
        }

        // Resuelve una ronda dada la info de dos criaturas con sus stats de inventario y decide ganador.
        // Devuelve: idJugadorGanador.
        public int ResolverRonda(Random rng, int idJugador1, int poder1, int resistencia1, int idJugador2, int poder2, int resistencia2)
        {
            // 1) Elegir atacante aleatoriamente (0 -> jugador1 ataca primero; 1 -> jugador2)
            bool atacaPrimeroJ1 = (rng.Next(0, 2) == 0);

            // 2) Ataque inicial
            if (atacaPrimeroJ1)
            {
                // resistencia del 2 menos poder del 1
                int resta = resistencia2 - poder1;
                if (resta <= 0) return idJugador1; // gana j1
            }
            else
            {
                int resta = resistencia1 - poder2;
                if (resta <= 0) return idJugador2; // gana j2
            }

            // 3) Contraataque si no hubo gane
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

            // 4) Remanentes de poder
            int rem1 = poder1 - resistencia2;
            int rem2 = poder2 - resistencia1;

            if (rem1 > rem2) return idJugador1;
            if (rem2 > rem1) return idJugador2;

            // 5) Desempate aleatorio si iguales
            return (rng.Next(0, 2) == 0) ? idJugador1 : idJugador2;
        }
    }
}
