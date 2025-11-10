//UNED
//Mitica X
//Jorge Arias Melendez
//Septiembre 2025
//Reglas para registrar/validar criaturas

using System;
using Miticax.Entidades;
using Miticax.Datos;

namespace Miticax.Logica
{
    public class CriaturaService
    {
        private readonly CriaturaDatos _criaturaDatos;

        // Inyeccion simple de la capa de datos.
        public CriaturaService(CriaturaDatos criaturaDatos)
        {
            _criaturaDatos = criaturaDatos;
        }

        // Inserta una criatura con validaciones de unicidad, tipo y costo por nivel.
        public ResultadoOperacion RegistrarCriatura(CriaturaEntidad entidad, out string errorDatos)
        {
            errorDatos = "";

            // Validaciones basicas
            if (!Validaciones.IdPositivo(entidad.IdCriatura)) return ResultadoOperacion.Fail("IdCriatura debe ser positivo");
            if (!Validaciones.TextoObligatorio(entidad.Nombre)) return ResultadoOperacion.Fail("Nombre es obligatorio");
            if (!Mapeos.EsTipoValido(entidad.Tipo)) return ResultadoOperacion.Fail("Tipo no permitido (use: agua, tierra, aire, fuego)");
            if (entidad.Nivel < 1 || entidad.Nivel > 5) return ResultadoOperacion.Fail("Nivel de criatura debe estar entre 1 y 5");
            if (entidad.Poder < 0 || entidad.Resistencia < 0) return ResultadoOperacion.Fail("Poder y Resistencia no pueden ser negativos");
            if (!Validaciones.CostoPorNivelValido(entidad.Nivel, entidad.Costo)) return ResultadoOperacion.Fail("Costo no valido para el nivel de criatura");

            // Unicidad de IdCriatura
            var existente = _criaturaDatos.BuscarPorId(entidad.IdCriatura, out string error);
            if (existente != null) return ResultadoOperacion.Fail("Ya existe una criatura con ese IdCriatura");

            // Normalizar tipo a minusculas
            entidad.Tipo = Mapeos.NormalizarTipo(entidad.Tipo);

            // Insertar en arreglo (Datos controla capacidad)
            bool ok = _criaturaDatos.Insertar(entidad, out errorDatos);
            if (!ok) return ResultadoOperacion.Fail(errorDatos);

            return ResultadoOperacion.Ok();
        }
    }
}
