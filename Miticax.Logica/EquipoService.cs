//UNED
//Mitica X
//Jorge Arias Melendez
//Septiembre 2025
//Reglas para registrar equipos de 3 criaturas del jugador

using System;
using Miticax.Entidades;
using Miticax.Datos;

namespace Miticax.Logica
{
    public class EquipoService
    {
        private readonly EquipoDatos _equipoDatos;
        private readonly JugadorDatos _jugadorDatos;
        private readonly InventarioDatos _inventarioDatos;

        public EquipoService(EquipoDatos equipoDatos, JugadorDatos jugadorDatos, InventarioDatos inventarioDatos)
        {
            _equipoDatos = equipoDatos;
            _jugadorDatos = jugadorDatos;
            _inventarioDatos = inventarioDatos;
        }

        // Valida e inserta un equipo (criaturas deben pertenecer al jugador y no repetirse).
        public ResultadoOperacion RegistrarEquipo(EquipoEntidad entidad, out string errorDatos)
        {
            errorDatos = "";

            if (!Validaciones.IdPositivo(entidad.IdEquipo)) return ResultadoOperacion.Fail("IdEquipo debe ser positivo");
            if (!Validaciones.IdPositivo(entidad.IdJugador)) return ResultadoOperacion.Fail("IdJugador debe ser positivo");

            // Unicidad IdEquipo
            var ya = _equipoDatos.FindById(entidad.IdEquipo);
            if (ya != null) return ResultadoOperacion.Fail("Ya existe un equipo con ese IdEquipo");

            // Validar jugador existe
            var jugador = _jugadorDatos.BuscarPorId(entidad.IdJugador, out string error);
            if (jugador == null) return ResultadoOperacion.Fail("Jugador no existe");

            // Validar no repetidos
            if (entidad.IdCriatura1 == entidad.IdCriatura2 ||
                entidad.IdCriatura1 == entidad.IdCriatura3 ||
                entidad.IdCriatura2 == entidad.IdCriatura3)
            {
                return ResultadoOperacion.Fail("Las criaturas del equipo no pueden repetirse");
            }

            // Validar pertenencia en inventario (busqueda lineal)
            if (!PerteneceAlJugador(entidad.IdJugador, entidad.IdCriatura1)) return ResultadoOperacion.Fail("IdCriatura1 no pertenece al jugador");
            if (!PerteneceAlJugador(entidad.IdJugador, entidad.IdCriatura2)) return ResultadoOperacion.Fail("IdCriatura2 no pertenece al jugador");
            if (!PerteneceAlJugador(entidad.IdJugador, entidad.IdCriatura3)) return ResultadoOperacion.Fail("IdCriatura3 no pertenece al jugador");

            // Insertar
            bool ok = _equipoDatos.Insert(entidad, out errorDatos);
            if (!ok) return ResultadoOperacion.Fail(errorDatos);

            return ResultadoOperacion.Ok();
        }

        // Verifica si una criatura esta en el inventario del jugador.
        private bool PerteneceAlJugador(int idJugador, int idCriatura)
        {
            var snap = _inventarioDatos.GetAllSnapshot();
            for (int i = 0; i < snap.Length; i++)
            {
                var item = snap[i];
                if (item == null) break;
                if (item.IdJugador == idJugador && item.IdCriatura == idCriatura) return true;
            }
            return false;
        }
    }
}
