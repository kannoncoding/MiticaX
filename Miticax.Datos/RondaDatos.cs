//UNED
//Mitica X
//Jorge Arias Melendez
//Tercer cuatrimestre 2025
//Acceso a datos: Rondas de batalla con validaciones y efectos (cristales/poder) en SQL Server

using System;
using System.Data;
using Microsoft.Data.SqlClient;
using Miticax.Entidades;

namespace Miticax.Datos
{
    public class RondaDatos
    {
        private readonly ConexionBD conexion = new ConexionBD();

        public bool Insertar(RondaEntidad item, out string error)
        {
            error = string.Empty;

            // Validacion rapida de idRonda
            if (item.IdRonda < 1 || item.IdRonda > 3)
            {
                error = "IdRonda debe estar en el rango 1..3.";
                return false;
            }

            using (SqlConnection cn = conexion.AbrirConexion())
            using (SqlTransaction tx = cn.BeginTransaction())
            {
                try
                {
                    // 1) Leer batalla y validar que esta abierta
                    int? j1, j2, ganadorBatalla;
                    if (!LeerJugadoresBatalla(cn, tx, item.IdBatalla, out j1, out j2, out ganadorBatalla))
                    {
                        error = "La batalla indicada no existe.";
                        tx.Rollback();
                        return false;
                    }
                    if (ganadorBatalla.HasValue)
                    {
                        error = "La batalla ya esta cerrada; no se pueden agregar rondas.";
                        tx.Rollback();
                        return false;
                    }

                    // 2) Jugadores de la ronda deben coincidir con batalla
                    if (item.IdJugador1 != j1 || item.IdJugador2 != j2)
                    {
                        error = "Los jugadores de la ronda no coinciden con los de la batalla.";
                        tx.Rollback();
                        return false;
                    }

                    // 3) No duplicar (IdBatalla, IdRonda)
                    if (ExisteRonda(cn, tx, item.IdBatalla, item.IdRonda))
                    {
                        error = "Ya existe una ronda con el mismo IdBatalla e IdRonda.";
                        tx.Rollback();
                        return false;
                    }

                    // 4) Criaturas pertenecen al inventario de sus jugadores
                    if (!ExisteEnInventario(cn, tx, item.IdJugador1, item.IdCriatura1) ||
                        !ExisteEnInventario(cn, tx, item.IdJugador2, item.IdCriatura2))
                    {
                        error = "Una o mas criaturas no pertenecen al inventario del jugador correspondiente.";
                        tx.Rollback();
                        return false;
                    }

                    // 5) Evitar repetir criaturas dentro de la misma batalla por jugador
                    if (CriaturaUsadaEnBatallaPorJugador(cn, tx, item.IdBatalla, item.IdJugador1, item.IdCriatura1) ||
                        CriaturaUsadaEnBatallaPorJugador(cn, tx, item.IdBatalla, item.IdJugador2, item.IdCriatura2))
                    {
                        error = "Una criatura ya fue usada por ese jugador en una ronda previa de la misma batalla.";
                        tx.Rollback();
                        return false;
                    }

                    // 6) Validar ganador
                    int idGanador = item.GanadorRonda;
                    if (idGanador != item.IdJugador1 && idGanador != item.IdJugador2)
                    {
                        error = "El ganador de la ronda debe ser uno de los jugadores de la ronda.";
                        tx.Rollback();
                        return false;
                    }

                    // 7) Determinar criatura ganadora segun el jugador ganador
                    int idCriaturaGanadora = (idGanador == item.IdJugador1) ? item.IdCriatura1 : item.IdCriatura2;

                    // 8) Insertar ronda
                    using (SqlCommand cmd = new SqlCommand(
                        @"INSERT INTO dbo.Ronda
                          (IdBatalla, IdRonda, IdJugador1, IdCriatura1, IdJugador2, IdCriatura2, GanadorRonda)
                          VALUES (@B, @R, @J1, @C1, @J2, @C2, @G)", cn, tx))
                    {
                        cmd.Parameters.AddWithValue("@B", item.IdBatalla);
                        cmd.Parameters.AddWithValue("@R", item.IdRonda);
                        cmd.Parameters.AddWithValue("@J1", item.IdJugador1);
                        cmd.Parameters.AddWithValue("@C1", item.IdCriatura1);
                        cmd.Parameters.AddWithValue("@J2", item.IdJugador2);
                        cmd.Parameters.AddWithValue("@C2", item.IdCriatura2);
                        cmd.Parameters.AddWithValue("@G", item.GanadorRonda);

                        if (cmd.ExecuteNonQuery() != 1)
                        {
                            error = "No se pudo insertar la ronda.";
                            tx.Rollback();
                            return false;
                        }
                    }

                    // 9) Efectos: +10 cristales al ganador
                    using (SqlCommand cmd = new SqlCommand(
                        @"UPDATE dbo.Jugador SET Cristales = Cristales + 10 WHERE IdJugador=@J", cn, tx))
                    {
                        cmd.Parameters.AddWithValue("@J", idGanador);
                        if (cmd.ExecuteNonQuery() != 1)
                        {
                            error = "No se pudo acreditar cristales al jugador ganador.";
                            tx.Rollback();
                            return false;
                        }
                    }

                    // 10) Efectos: +5 poder a la criatura ganadora dentro del inventario
                    using (SqlCommand cmd = new SqlCommand(
                        @"UPDATE dbo.InventarioJugador 
                          SET Poder = Poder + 5
                          WHERE IdJugador=@J AND IdCriatura=@C", cn, tx))
                    {
                        cmd.Parameters.AddWithValue("@J", idGanador);
                        cmd.Parameters.AddWithValue("@C", idCriaturaGanadora);
                        if (cmd.ExecuteNonQuery() != 1)
                        {
                            error = "No se pudo aumentar el poder de la criatura ganadora.";
                            tx.Rollback();
                            return false;
                        }
                    }

                    // ok
                    tx.Commit();
                    return true;
                }
                catch (SqlException ex)
                {
                    try { tx.Rollback(); } catch { }
                    error = "Error SQL al insertar ronda: " + ex.Message;
                    return false;
                }
                catch (Exception ex)
                {
                    try { tx.Rollback(); } catch { }
                    error = "Error general al insertar ronda: " + ex.Message;
                    return false;
                }
            }
        }

        // -------------------------------------------------------------
        // BUSCAR por (IdBatalla, IdRonda)
        // -------------------------------------------------------------
        public RondaEntidad? BuscarPorBatallaYRonda(int idBatalla, int idRonda, out string error)
        {
            error = string.Empty;
            try
            {
                string sql = @"SELECT IdBatalla, IdRonda, IdJugador1, IdCriatura1, IdJugador2, IdCriatura2, GanadorRonda
                               FROM dbo.Ronda
                               WHERE IdBatalla=@B AND IdRonda=@R";

                using (SqlDataReader dr = conexion.EjecutarReader(sql, p =>
                {
                    p.AddWithValue("@B", idBatalla);
                    p.AddWithValue("@R", idRonda);
                }))
                {
                    if (dr.Read())
                    {
                        return new RondaEntidad
                        {
                            IdBatalla = dr.GetInt32(0),
                            IdRonda = dr.GetInt32(1),
                            IdJugador1 = dr.GetInt32(2),
                            IdCriatura1 = dr.GetInt32(3),
                            IdJugador2 = dr.GetInt32(4),
                            IdCriatura2 = dr.GetInt32(5),
                            GanadorRonda = dr.GetInt32(6)
                        };
                    }
                }
                return null;
            }
            catch (SqlException ex)
            {
                error = "Error SQL al buscar ronda: " + ex.Message;
                return null;
            }
            catch (Exception ex)
            {
                error = "Error general al buscar ronda: " + ex.Message;
                return null;
            }
        }

        // -------------------------------------------------------------
        // OBTENER TODOS (arreglo sin List/LINQ)
        // -------------------------------------------------------------
        public RondaEntidad[] ObtenerTodos(out string error, int max = 3000)
        {
            error = string.Empty;
            RondaEntidad[] arr = new RondaEntidad[max];
            int i = 0;

            try
            {
                string sql = @"SELECT IdBatalla, IdRonda, IdJugador1, IdCriatura1, IdJugador2, IdCriatura2, GanadorRonda
                               FROM dbo.Ronda
                               ORDER BY IdBatalla ASC, IdRonda ASC";

                using (SqlDataReader dr = conexion.EjecutarReader(sql))
                {
                    while (dr.Read() && i < max)
                    {
                        arr[i] = new RondaEntidad
                        {
                            IdBatalla = dr.GetInt32(0),
                            IdRonda = dr.GetInt32(1),
                            IdJugador1 = dr.GetInt32(2),
                            IdCriatura1 = dr.GetInt32(3),
                            IdJugador2 = dr.GetInt32(4),
                            IdCriatura2 = dr.GetInt32(5),
                            GanadorRonda = dr.GetInt32(6)
                        };
                        i++;
                    }
                }

                RondaEntidad[] res = new RondaEntidad[i];
                for (int k = 0; k < i; k++) res[k] = arr[k];
                return res;
            }
            catch (SqlException ex)
            {
                error = "Error SQL al obtener rondas: " + ex.Message;
                return new RondaEntidad[0];
            }
            catch (Exception ex)
            {
                error = "Error general al obtener rondas: " + ex.Message;
                return new RondaEntidad[0];
            }
        }

        // -------------------------------------------------------------
        // OBTENER POR BATALLA
        // -------------------------------------------------------------
        public RondaEntidad[] ObtenerPorBatalla(int idBatalla, out string error, int max = 50)
        {
            error = string.Empty;
            RondaEntidad[] arr = new RondaEntidad[max];
            int i = 0;

            try
            {
                string sql = @"SELECT IdBatalla, IdRonda, IdJugador1, IdCriatura1, IdJugador2, IdCriatura2, GanadorRonda
                               FROM dbo.Ronda
                               WHERE IdBatalla=@B
                               ORDER BY IdRonda ASC";

                using (SqlDataReader dr = conexion.EjecutarReader(sql, p => p.AddWithValue("@B", idBatalla)))
                {
                    while (dr.Read() && i < max)
                    {
                        arr[i] = new RondaEntidad
                        {
                            IdBatalla = dr.GetInt32(0),
                            IdRonda = dr.GetInt32(1),
                            IdJugador1 = dr.GetInt32(2),
                            IdCriatura1 = dr.GetInt32(3),
                            IdJugador2 = dr.GetInt32(4),
                            IdCriatura2 = dr.GetInt32(5),
                            GanadorRonda = dr.GetInt32(6)
                        };
                        i++;
                    }
                }

                RondaEntidad[] res = new RondaEntidad[i];
                for (int k = 0; k < i; k++) res[k] = arr[k];
                return res;
            }
            catch (SqlException ex)
            {
                error = "Error SQL al obtener rondas por batalla: " + ex.Message;
                return new RondaEntidad[0];
            }
            catch (Exception ex)
            {
                error = "Error general al obtener rondas por batalla: " + ex.Message;
                return new RondaEntidad[0];
            }
        }

        // -------------------------------------------------------------
        // OBTENER POR JUGADOR (participa como J1 o J2)
        // -------------------------------------------------------------
        public RondaEntidad[] ObtenerPorJugador(int idJugador, out string error, int max = 1000)
        {
            error = string.Empty;
            RondaEntidad[] arr = new RondaEntidad[max];
            int i = 0;

            try
            {
                string sql = @"SELECT IdBatalla, IdRonda, IdJugador1, IdCriatura1, IdJugador2, IdCriatura2, GanadorRonda
                               FROM dbo.Ronda
                               WHERE IdJugador1=@J OR IdJugador2=@J
                               ORDER BY IdBatalla DESC, IdRonda ASC";

                using (SqlDataReader dr = conexion.EjecutarReader(sql, p => p.AddWithValue("@J", idJugador)))
                {
                    while (dr.Read() && i < max)
                    {
                        arr[i] = new RondaEntidad
                        {
                            IdBatalla = dr.GetInt32(0),
                            IdRonda = dr.GetInt32(1),
                            IdJugador1 = dr.GetInt32(2),
                            IdCriatura1 = dr.GetInt32(3),
                            IdJugador2 = dr.GetInt32(4),
                            IdCriatura2 = dr.GetInt32(5),
                            GanadorRonda = dr.GetInt32(6)
                        };
                        i++;
                    }
                }

                RondaEntidad[] res = new RondaEntidad[i];
                for (int k = 0; k < i; k++) res[k] = arr[k];
                return res;
            }
            catch (SqlException ex)
            {
                error = "Error SQL al obtener rondas por jugador: " + ex.Message;
                return new RondaEntidad[0];
            }
            catch (Exception ex)
            {
                error = "Error general al obtener rondas por jugador: " + ex.Message;
                return new RondaEntidad[0];
            }
        }

        // ======================= Helpers internos =======================

        private bool ExisteRonda(SqlConnection cn, SqlTransaction tx, int idBatalla, int idRonda)
        {
            using (SqlCommand cmd = new SqlCommand(
                @"SELECT 1 FROM dbo.Ronda WHERE IdBatalla=@B AND IdRonda=@R", cn, tx))
            {
                cmd.Parameters.AddWithValue("@B", idBatalla);
                cmd.Parameters.AddWithValue("@R", idRonda);
                return cmd.ExecuteScalar() != null;
            }
        }

        private bool ExisteEnInventario(SqlConnection cn, SqlTransaction tx, int idJugador, int idCriatura)
        {
            using (SqlCommand cmd = new SqlCommand(
                @"SELECT 1 FROM dbo.InventarioJugador 
                  WHERE IdJugador=@J AND IdCriatura=@C", cn, tx))
            {
                cmd.Parameters.AddWithValue("@J", idJugador);
                cmd.Parameters.AddWithValue("@C", idCriatura);
                return cmd.ExecuteScalar() != null;
            }
        }

        private bool CriaturaUsadaEnBatallaPorJugador(SqlConnection cn, SqlTransaction tx, int idBatalla, int idJugador, int idCriatura)
        {
            using (SqlCommand cmd = new SqlCommand(
                @"SELECT 1 
                  FROM dbo.Ronda 
                  WHERE IdBatalla=@B AND 
                        ((IdJugador1=@J AND IdCriatura1=@C) OR (IdJugador2=@J AND IdCriatura2=@C))", cn, tx))
            {
                cmd.Parameters.AddWithValue("@B", idBatalla);
                cmd.Parameters.AddWithValue("@J", idJugador);
                cmd.Parameters.AddWithValue("@C", idCriatura);
                return cmd.ExecuteScalar() != null;
            }
        }

        private bool LeerJugadoresBatalla(SqlConnection cn, SqlTransaction tx, int idBatalla,
                                          out int? j1, out int? j2, out int? ganador)
        {
            j1 = null; j2 = null; ganador = null;
            using (SqlCommand cmd = new SqlCommand(
                "SELECT IdJugador1, IdJugador2, Ganador FROM dbo.Batalla WHERE IdBatalla=@B", cn, tx))
            {
                cmd.Parameters.AddWithValue("@B", idBatalla);
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (!dr.Read()) return false;
                    j1 = dr.GetInt32(0);
                    j2 = dr.GetInt32(1);
                    ganador = dr.IsDBNull(2) ? (int?)null : dr.GetInt32(2);
                    return true;
                }
            }
        }

        public bool RemoveLastIfMatch(int idBatalla)
        {
            try
            {
                using (SqlConnection cn = conexion.AbrirConexion())
                using (SqlTransaction tx = cn.BeginTransaction())
                {
                    // 1) Leer la ultima ronda (TOP 1 ORDER BY IdRonda DESC)
                    int? idRonda = null;
                    int idJugador1 = 0, idCriatura1 = 0, idJugador2 = 0, idCriatura2 = 0, ganador = 0;

                    using (SqlCommand cmd = new SqlCommand(
                        @"SELECT TOP 1 IdRonda, IdJugador1, IdCriatura1, IdJugador2, IdCriatura2, GanadorRonda
                  FROM dbo.Ronda
                  WHERE IdBatalla=@B
                  ORDER BY IdRonda DESC", cn, tx))
                    {
                        cmd.Parameters.AddWithValue("@B", idBatalla);
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (!dr.Read())
                            {
                                // No hay rondas para esa batalla
                                tx.Rollback();
                                return false;
                            }

                            idRonda = dr.GetInt32(0);
                            idJugador1 = dr.GetInt32(1);
                            idCriatura1 = dr.GetInt32(2);
                            idJugador2 = dr.GetInt32(3);
                            idCriatura2 = dr.GetInt32(4);
                            ganador = dr.GetInt32(5);
                        }
                    }

                    // 2) Determinar criatura ganadora segun el ganador
                    int idCriaturaGanadora = (ganador == idJugador1) ? idCriatura1 : idCriatura2;

                    // 3) Eliminar la ronda
                    using (SqlCommand cmd = new SqlCommand(
                        @"DELETE FROM dbo.Ronda 
                  WHERE IdBatalla=@B AND IdRonda=@R", cn, tx))
                    {
                        cmd.Parameters.AddWithValue("@B", idBatalla);
                        cmd.Parameters.AddWithValue("@R", idRonda.Value);
                        if (cmd.ExecuteNonQuery() != 1)
                        {
                            tx.Rollback();
                            return false;
                        }
                    }

                    // 4) Revertir efectos: -10 cristales al ganador
                    using (SqlCommand cmd = new SqlCommand(
                        @"UPDATE dbo.Jugador SET Cristales = Cristales - 10 
                  WHERE IdJugador=@J", cn, tx))
                    {
                        cmd.Parameters.AddWithValue("@J", ganador);
                        if (cmd.ExecuteNonQuery() != 1)
                        {
                            tx.Rollback();
                            return false;
                        }
                    }

                    // 5) Revertir efectos: -5 poder a la criatura ganadora en InventarioJugador
                    using (SqlCommand cmd = new SqlCommand(
                        @"UPDATE dbo.InventarioJugador 
                  SET Poder = Poder - 5
                  WHERE IdJugador=@J AND IdCriatura=@C", cn, tx))
                    {
                        cmd.Parameters.AddWithValue("@J", ganador);
                        cmd.Parameters.AddWithValue("@C", idCriaturaGanadora);
                        if (cmd.ExecuteNonQuery() != 1)
                        {
                            tx.Rollback();
                            return false;
                        }
                    }

                    tx.Commit();
                    return true;
                }
            }
            catch
            {
                // Cualquier problema -> no se pudo revertir
                return false;
            }
        }

        // ================== Metodos puente (compatibilidad API vieja) ==================

        // Antes: Insert(RondaEntidad, out string)
        public bool Insert(RondaEntidad item, out string error) => Insertar(item, out error);

        // Antes: FindByBatallaAndRonda(int,int)
        public RondaEntidad? FindByBatallaAndRonda(int idBatalla, int idRonda)
        {
            string _;
            return BuscarPorBatallaYRonda(idBatalla, idRonda, out _);
        }

        // Antes: GetAllSnapshot()
        public RondaEntidad[] GetAllSnapshot()
        {
            string _;
            return ObtenerTodos(out _, 3000);
        }

        // Antes: FindAllByBatallaId(int)
        public RondaEntidad[] FindAllByBatallaId(int idBatalla)
        {
            string _;
            return ObtenerPorBatalla(idBatalla, out _, 50);
        }

        // Antes: FindAllByJugadorId(int)
        public RondaEntidad[] FindAllByJugadorId(int idJugador)
        {
            string _;
            return ObtenerPorJugador(idJugador, out _, 1000);
        }

        // Antes: Count()
        public int Count()
        {
            string _;
            try
            {
                object o = conexion.EjecutarEscalar("SELECT COUNT(*) FROM dbo.Ronda");
                return Convert.ToInt32(o);
            }
            catch { return 0; }
        }

        // Compatibilidad con CapacidadTotal/CapacidadRestante (sin tope real en BD)
        public int CapacidadTotal() { return int.MaxValue; }
        public int CapacidadRestante() { return int.MaxValue - Count(); }
    }
}