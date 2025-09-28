//UNED
//Mitica X
//Jorge Arias Melendez
//Septiembre 2025
//Reglas para registrar/validar jugadores y actualizar su progreso

using System;
using Miticax.Entidades;
using Miticax.Datos;

namespace Miticax.Logica
{
    public class JugadorService
    {
        private readonly JugadorDatos _jugadorDatos;

        public JugadorService(JugadorDatos jugadorDatos)
        {
            _jugadorDatos = jugadorDatos;
        }

        // Registra jugador validando edad > 10, unicidad de Id y estableciendo valores iniciales.
        public ResultadoOperacion RegistrarJugador(JugadorEntidad entidad, DateTime ahora, out string errorDatos)
        {
            errorDatos = "";

            if (!Validaciones.IdPositivo(entidad.IdJugador)) return ResultadoOperacion.Fail("IdJugador debe ser positivo");
            if (!Validaciones.TextoObligatorio(entidad.Nombre)) return ResultadoOperacion.Fail("Nombre es obligatorio");
            if (!Validaciones.EdadMayorA10(entidad.FechaNacimiento, ahora)) return ResultadoOperacion.Fail("El jugador debe tener mas de 10 anios");

            // Unicidad de IdJugador
            var existente = _jugadorDatos.FindById(entidad.IdJugador);
            if (existente != null) return ResultadoOperacion.Fail("Ya existe un jugador con ese IdJugador");

            // Iniciales
            entidad.Cristales = 100;       // Moneda inicial
            entidad.BatallasGanadas = 0;   // Inicia en 0
            entidad.Nivel = 1;             // 1 = Novato

            bool ok = _jugadorDatos.Insert(entidad, out errorDatos);
            if (!ok) return ResultadoOperacion.Fail(errorDatos);

            return ResultadoOperacion.Ok();
        }

        // Recalcula y actualiza nivel segun batallas ganadas.
        public void RecalcularNivel(int idJugador)
        {
            var jugador = _jugadorDatos.FindById(idJugador);
            if (jugador == null) return; // Silencioso: no existe

            jugador.Nivel = Mapeos.CalcularNivelJugadorPorVictorias(jugador.BatallasGanadas);
        }
    }
}
