//UNED
//Mitica X
//Jorge Arias Melendez
//Septiembre 2025
//Compra de criaturas: valida cristales, duplicados y copia stats base al inventario

using System;
using Miticax.Entidades;
using Miticax.Datos;

namespace Miticax.Logica
{
    public class InventarioService
    {
        private readonly InventarioDatos _inventarioDatos;
        private readonly JugadorDatos _jugadorDatos;
        private readonly CriaturaDatos _criaturaDatos;

        public InventarioService(InventarioDatos inventarioDatos, JugadorDatos jugadorDatos, CriaturaDatos criaturaDatos)
        {
            _inventarioDatos = inventarioDatos;
            _jugadorDatos = jugadorDatos;
            _criaturaDatos = criaturaDatos;
        }

        // Compra una criatura: valida existencia, saldo, duplicados para ese jugador y descuenta costo.
        public ResultadoOperacion ComprarCriatura(int idJugador, int idCriatura, out string errorDatos)
        {
            errorDatos = "";

            // Verificar jugador
            var jugador = _jugadorDatos.FindById(idJugador);
            if (jugador == null) return ResultadoOperacion.Fail("Jugador no existe");

            // Verificar criatura
            var criatura = _criaturaDatos.FindById(idCriatura);
            if (criatura == null) return ResultadoOperacion.Fail("Criatura no existe");

            // Duplicado en inventario del jugador (busqueda lineal con snapshot)
            var snap = _inventarioDatos.GetAllSnapshot();
            for (int i = 0; i < snap.Length; i++)
            {
                var item = snap[i];
                if (item.IdJugador == idJugador && item.IdCriatura == idCriatura)
                {
                    return ResultadoOperacion.Fail("El jugador ya posee esa criatura en su inventario");
                }
            }

            // Verificar cristales suficientes
            if (jugador.Cristales < criatura.Costo)
            {
                return ResultadoOperacion.Fail("No posee la cantidad de cristales suficientes para obtener la criatura");
            }

            // Crear registro de inventario copiando poder/resistencia base de la criatura
            var inv = new InventarioJugadorEntidad
            {
                IdJugador = idJugador,
                IdCriatura = idCriatura,
                Poder = criatura.Poder,
                Resistencia = criatura.Resistencia
            };

            // Intentar insertar en inventario
            bool ok = _inventarioDatos.Insert(inv, out errorDatos);
            if (!ok) return ResultadoOperacion.Fail(errorDatos);

            // Solo si el insert fue exitoso, se descuentan cristales
            jugador.Cristales -= criatura.Costo;

            return ResultadoOperacion.Ok();
        }



        // Sube poder de una criatura del inventario del jugador en +5 (si existe).
        public void IncrementarPoderInventario(int idJugador, int idCriatura, int incremento)
        {
            var snap = _inventarioDatos.GetAllSnapshot();
            for (int i = 0; i < snap.Length; i++)
            {
                var item = snap[i];
                if (item == null) break;
                if (item.IdJugador == idJugador && item.IdCriatura == idCriatura)
                {
                    item.Poder += incremento;
                    return;
                }
            }
        }
    }
}
