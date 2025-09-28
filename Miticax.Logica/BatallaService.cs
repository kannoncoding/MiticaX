//UNED
//Mitica X
//Jorge Arias Melendez
//Septiembre 2025
//Registro y ejecucion de batallas (mejor de 3), con actualizacion de jugadores e inventario

using System;
using Miticax.Entidades;
using Miticax.Datos;

namespace Miticax.Logica
{
    public class BatallaService
    {
        private readonly BatallaDatos _batallaDatos;
        private readonly JugadorDatos _jugadorDatos;
        private readonly EquipoDatos _equipoDatos;
        private readonly InventarioDatos _inventarioDatos;
        private readonly RondaService _rondaService;

        public BatallaService(
            BatallaDatos batallaDatos,
            JugadorDatos jugadorDatos,
            EquipoDatos equipoDatos,
            InventarioDatos inventarioDatos,
            RondaService rondaService)
        {
            _batallaDatos = batallaDatos;
            _jugadorDatos = jugadorDatos;
            _equipoDatos = equipoDatos;
            _inventarioDatos = inventarioDatos;
            _rondaService = rondaService;
        }

        // Registrar batalla: valida jugadores, equipos y unicidad de IdBatalla.
        public ResultadoOperacion RegistrarBatalla(BatallaEntidad batalla, out string errorDatos)
        {
            errorDatos = "";

            if (!Validaciones.IdPositivo(batalla.IdBatalla)) return ResultadoOperacion.Fail("IdBatalla debe ser positivo");

            // Unicidad
            var ya = _batallaDatos.FindById(batalla.IdBatalla);
            if (ya != null) return ResultadoOperacion.Fail("Ya existe una batalla con ese IdBatalla");

            // Jugadores deben existir y ser distintos
            var j1 = _jugadorDatos.FindById(batalla.IdJugador1);
            var j2 = _jugadorDatos.FindById(batalla.IdJugador2);
            if (j1 == null || j2 == null) return ResultadoOperacion.Fail("Jugador1 o Jugador2 no existe");
            if (batalla.IdJugador1 == batalla.IdJugador2) return ResultadoOperacion.Fail("Los jugadores deben ser distintos");

            // Equipos deben existir
            var e1 = _equipoDatos.FindById(batalla.IdEquipo1);
            var e2 = _equipoDatos.FindById(batalla.IdEquipo2);
            if (e1 == null || e2 == null) return ResultadoOperacion.Fail("Equipo1 o Equipo2 no existe");

            // Equipos deben pertenecer a sus respectivos jugadores
            if (e1.IdJugador != batalla.IdJugador1) return ResultadoOperacion.Fail("Equipo1 no pertenece a Jugador1");
            if (e2.IdJugador != batalla.IdJugador2) return ResultadoOperacion.Fail("Equipo2 no pertenece a Jugador2");

            // Insertar batalla con fecha inicial y ganador sin definir (0)
            batalla.Ganador = 0;
            batalla.Fecha = DateTime.Now;
            bool ok = _batallaDatos.Insert(batalla, out errorDatos);
            if (!ok) return ResultadoOperacion.Fail(errorDatos);

            return ResultadoOperacion.Ok();
        }

        // Ejecuta una batalla (mejor de 3) y actualiza ganador, jugadores y rondas.
        public ResultadoOperacion EjecutarBatalla(int idBatalla, Random rng, out string error)
        {
            error = "";

            // 1) Buscar la batalla solicitada
            var batalla = _batallaDatos.FindById(idBatalla);
            if (batalla == null)
            {
                error = "Batalla no existe";
                return ResultadoOperacion.Fail(error);
            }

            // 2) Guard de seguridad: no permitir re-ejecutar una batalla ya finalizada
            //    Si Ganador != 0, significa que ya se resolvio previamente y no debe repetirse.
            if (batalla.Ganador != 0)
            {
                error = "La batalla ya fue ejecutada anteriormente (IdBatalla=" + batalla.IdBatalla + ", Ganador=" + batalla.Ganador + ")";
                return ResultadoOperacion.Fail(error);
            }

            // 3) Obtener y validar equipos de ambos jugadores
            var e1 = _equipoDatos.FindById(batalla.IdEquipo1);
            if (e1 == null)
            {
                error = "Equipo1 no existe";
                return ResultadoOperacion.Fail(error);
            }

            var e2 = _equipoDatos.FindById(batalla.IdEquipo2);
            if (e2 == null)
            {
                error = "Equipo2 no existe";
                return ResultadoOperacion.Fail(error);
            }

            // 4) Estructuras auxiliares para evitar repetir criaturas de un mismo equipo en esta batalla
            int[] usadosJ1 = new int[3];
            int[] usadosJ2 = new int[3];

            int rondasGanadasJ1 = 0;
            int rondasGanadasJ2 = 0;

            // 5) Ejecutar hasta 3 rondas (o terminar antes si alguien alcanza 2 victorias)
            for (int numRonda = 1; numRonda <= 3; numRonda++)
            {
                // 5.1) Seleccionar criaturas no repetidas para esta ronda
                int idC1 = SeleccionarCriaturaSinRepetir(rng, e1, usadosJ1);
                int idC2 = SeleccionarCriaturaSinRepetir(rng, e2, usadosJ2);

                // 5.2) Obtener stats actuales desde inventario (poder/resistencia)
                int poder1, res1, poder2, res2;

                if (!StatsDeInventario(batalla.IdJugador1, idC1, out poder1, out res1))
                {
                    error = "Stats no encontrados en inventario para Jugador1 (IdCriatura=" + idC1 + ")";
                    return ResultadoOperacion.Fail(error);
                }

                if (!StatsDeInventario(batalla.IdJugador2, idC2, out poder2, out res2))
                {
                    error = "Stats no encontrados en inventario para Jugador2 (IdCriatura=" + idC2 + ")";
                    return ResultadoOperacion.Fail(error);
                }

                // 5.3) Resolver la ronda segun reglas
                int idGanadorRonda = _rondaService.ResolverRonda(
                    rng,
                    batalla.IdJugador1, poder1, res1,
                    batalla.IdJugador2, poder2, res2
                );

                if (idGanadorRonda == batalla.IdJugador1) rondasGanadasJ1++;
                else rondasGanadasJ2++;

                // 5.4) Registrar la ronda y aplicar recompensas por ronda
                var ronda = new RondaEntidad
                {
                    IdRonda = numRonda,
                    IdBatalla = batalla.IdBatalla,
                    IdJugador1 = batalla.IdJugador1,
                    IdCriatura1 = idC1,
                    IdJugador2 = batalla.IdJugador2,
                    IdCriatura2 = idC2,
                    GanadorRonda = idGanadorRonda
                };

                var resRonda = _rondaService.RegistrarRondaYRecompensas(ronda, 5, out string errRonda);
                if (!resRonda.Exito)
                {
                    error = "Fallo al registrar la ronda " + numRonda + ": " + errRonda;
                    return ResultadoOperacion.Fail(error);
                }

                // 5.5) Cierre anticipado si ya hay 2 rondas ganadas por alguno
                if (rondasGanadasJ1 == 2 || rondasGanadasJ2 == 2) break;
            }

            // 6) Determinar el ganador final (mejor de 3)
            int idGanador = (rondasGanadasJ1 > rondasGanadasJ2) ? batalla.IdJugador1 : batalla.IdJugador2;
            batalla.Ganador = idGanador;           // marcar batalla como resuelta (evita re-ejecucion futura)
            batalla.Fecha = DateTime.Now;          // actualizar timestamp de resolucion

            // 7) Aplicar recompensas finales al ganador
            var ganador = _jugadorDatos.FindById(idGanador);
            if (ganador == null)
            {
                error = "No se pudo encontrar al jugador ganador para otorgar recompensas (IdJugador=" + idGanador + ")";
                return ResultadoOperacion.Fail(error);
            }

            ganador.Cristales += 30;               // +30 por ganar la batalla
            ganador.BatallasGanadas += 1;          // +1 victoria al ranking
            ganador.Nivel = Mapeos.CalcularNivelJugadorPorVictorias(ganador.BatallasGanadas);

            // 8) Exito
            error = "";
            return ResultadoOperacion.Ok();
        }



        // Selecciona una criatura random del equipo que no se haya usado aun en esta batalla.
        private int SeleccionarCriaturaSinRepetir(Random rng, EquipoEntidad equipo, int[] usados)
        {
            // Poner ids del equipo en un arreglo
            int[] ids = new int[3];
            ids[0] = equipo.IdCriatura1;
            ids[1] = equipo.IdCriatura2;
            ids[2] = equipo.IdCriatura3;

            // Intentar seleccionar hasta encontrar una no usada.
            // Como max 3 intentos (tamanio 3).
            for (int intento = 0; intento < 10; intento++)
            {
                int idx = rng.Next(0, 3);
                int candidato = ids[idx];
                if (!YaUsada(candidato, usados))
                {
                    // Marcar usada en la primera posicion libre
                    for (int k = 0; k < usados.Length; k++)
                    {
                        if (usados[k] == 0)
                        {
                            usados[k] = candidato;
                            break;
                        }
                    }
                    return candidato;
                }
            }

            // Si llega aqui por algun motivo, elegir la primera no usada de forma determinista
            for (int i = 0; i < ids.Length; i++)
            {
                if (!YaUsada(ids[i], usados))
                {
                    for (int k = 0; k < usados.Length; k++)
                    {
                        if (usados[k] == 0)
                        {
                            usados[k] = ids[i];
                            break;
                        }
                    }
                    return ids[i];
                }
            }

            // Como fallback (no deberia pasar), devolver la primera
            return ids[0];
        }

        // Verifica si idCriatura ya fue usada.
        private bool YaUsada(int idCriatura, int[] usados)
        {
            for (int i = 0; i < usados.Length; i++)
            {
                if (usados[i] == idCriatura) return true;
            }
            return false;
        }

        // Obtiene poder/resistencia actuales de inventario para jugador+criatura.
        private bool StatsDeInventario(int idJugador, int idCriatura, out int poder, out int resistencia)
        {
            poder = 0;
            resistencia = 0;
            var snap = _inventarioDatos.GetAllSnapshot();
            for (int i = 0; i < snap.Length; i++)
            {
                var item = snap[i];
                if (item == null) break;
                if (item.IdJugador == idJugador && item.IdCriatura == idCriatura)
                {
                    poder = item.Poder;
                    resistencia = item.Resistencia;
                    return true;
                }
            }
            return false;
        }
    }
}
