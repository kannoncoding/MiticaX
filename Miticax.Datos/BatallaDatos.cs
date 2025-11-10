//UNED
//Mitica X
//Jorge Arias Melendez
//Septiembre 2025
//Acceso a datos: CRUD de Batalla en SQL Server con validaciones y cierre de batalla

using System;
using System.Data;
using Microsoft.Data.SqlClient;
using Miticax.Entidades;

namespace Miticax.Datos
{
    public class BatallaDatos
    {
        // Acceso central de conexion
        private readonly ConexionBD conexion = new ConexionBD();

        public bool Insertar(BatallaEntidad item, out string error)
        {
            error = string.Empty;

            if (item.IdJugador1 == item.IdJugador2)
            {
                error = "Los jugadores deben ser distintos.";
                return false;
            }
            if (item.IdEquipo1 == item.IdEquipo2)
            {
                error = "Los equipos deben ser distintos.";
                return false;
            }

            using (SqlConnection cn = conexion.AbrirConexion())
            using (SqlTransaction tx = cn.BeginTransaction())
            {
                try
                {
                    // PK no duplicada
                    if (ExisteBatalla(cn, tx, item.IdBatalla))
                    {
                        error = "Ya existe una batalla con ese IdBatalla.";
                        tx.Rollback();
                        return false;
                    }

                    // Jugadores existen
                    if (!ExisteJugador(cn, tx, item.IdJugador1) || !ExisteJugador(cn, tx, item.IdJugador2))
                    {
                        error = "Uno o ambos jugadores no existen.";
                        tx.Rollback();
                        return false;
                    }

                    // Equipos existen
                    if (!ExisteEquipo(cn, tx, item.IdEquipo1) || !ExisteEquipo(cn, tx, item.IdEquipo2))
                    {
                        error = "Uno o ambos equipos no existen.";
                        tx.Rollback();
                        return false;
                    }

                    // Pertenencia de equipos a jugadores
                    if (!EquipoPerteneceAJugador(cn, tx, item.IdEquipo1, item.IdJugador1) ||
                        !EquipoPerteneceAJugador(cn, tx, item.IdEquipo2, item.IdJugador2))
                    {
                        error = "Un equipo no pertenece al jugador asignado.";
                        tx.Rollback();
                        return false;
                    }

                    // Insertar registro
                    using (SqlCommand cmd = new SqlCommand(
                        @"INSERT INTO dbo.Batalla
                          (IdBatalla, IdJugador1, IdEquipo1, IdJugador2, IdEquipo2, Ganador, Fecha)
                          VALUES (@B, @J1, @E1, @J2, @E2, NULL, @F)", cn, tx))
                    {
                        cmd.Parameters.AddWithValue("@B", item.IdBatalla);
                        cmd.Parameters.AddWithValue("@J1", item.IdJugador1);
                        cmd.Parameters.AddWithValue("@E1", item.IdEquipo1);
                        cmd.Parameters.AddWithValue("@J2", item.IdJugador2);
                        cmd.Parameters.AddWithValue("@E2", item.IdEquipo2);
                        cmd.Parameters.AddWithValue("@F", item.Fecha.Date);

                        int filas = cmd.ExecuteNonQuery();
                        if (filas != 1)
                        {
                            error = "No se pudo insertar la batalla.";
                            tx.Rollback();
                            return false;
                        }
                    }

                    tx.Commit();
                    return true;
                }
                catch (SqlException ex)
                {
                    try { tx.Rollback(); } catch { }
                    error = "Error SQL al insertar batalla: " + ex.Message;
                    return false;
                }
                catch (Exception ex)
                {
                    try { tx.Rollback(); } catch { }
                    error = "Error general al insertar batalla: " + ex.Message;
                    return false;
                }
            }
        }


        public bool CerrarBatalla(int idBatalla, int idJugadorGanador, out string error)
        {
            error = string.Empty;

            using (SqlConnection cn = conexion.AbrirConexion())
            using (SqlTransaction tx = cn.BeginTransaction())
            {
                try
                {
                    // Obtener info de la batalla
                    int? j1, j2;
                    int? ganadorActual;
                    if (!LeerJugadoresBatalla(cn, tx, idBatalla, out j1, out j2, out ganadorActual))
                    {
                        error = "La batalla no existe.";
                        tx.Rollback();
                        return false;
                    }

                    if (ganadorActual.HasValue)
                    {
                        error = "La batalla ya fue cerrada anteriormente.";
                        tx.Rollback();
                        return false;
                    }

                    if (idJugadorGanador != j1 && idJugadorGanador != j2)
                    {
                        error = "El ganador debe ser uno de los jugadores de la batalla.";
                        tx.Rollback();
                        return false;
                    }

                    // Actualizar ganador
                    using (SqlCommand cmd = new SqlCommand(
                        "UPDATE dbo.Batalla SET Ganador=@G WHERE IdBatalla=@B", cn, tx))
                    {
                        cmd.Parameters.AddWithValue("@G", idJugadorGanador);
                        cmd.Parameters.AddWithValue("@B", idBatalla);
                        if (cmd.ExecuteNonQuery() != 1)
                        {
                            error = "No se pudo actualizar el ganador de la batalla.";
                            tx.Rollback();
                            return false;
                        }
                    }

                    // Sumar ganadas al jugador
                    using (SqlCommand cmd = new SqlCommand(
                        "UPDATE dbo.Jugador SET BatallasGanadas = BatallasGanadas + 1 WHERE IdJugador=@J", cn, tx))
                    {
                        cmd.Parameters.AddWithValue("@J", idJugadorGanador);
                        if (cmd.ExecuteNonQuery() != 1)
                        {
                            error = "No se pudo actualizar BatallasGanadas del jugador.";
                            tx.Rollback();
                            return false;
                        }
                    }

                    tx.Commit();
                    return true;
                }
                catch (SqlException ex)
                {
                    try { tx.Rollback(); } catch { }
                    error = "Error SQL al cerrar batalla: " + ex.Message;
                    return false;
                }
                catch (Exception ex)
                {
                    try { tx.Rollback(); } catch { }
                    error = "Error general al cerrar batalla: " + ex.Message;
                    return false;
                }
            }
        }

        // ============================================================
        // BUSCAR por Id
        // ============================================================
        public BatallaEntidad? BuscarPorId(int idBatalla, out string error)
        {
            error = string.Empty;
            try
            {
                string sql = @"SELECT IdBatalla, IdJugador1, IdEquipo1, IdJugador2, IdEquipo2, Ganador, Fecha
                               FROM dbo.Batalla WHERE IdBatalla=@B";

                using (SqlDataReader dr = conexion.EjecutarReader(sql, p => p.AddWithValue("@B", idBatalla)))
                {
                    if (dr.Read())
                    {
                        return new BatallaEntidad
                        {
                            IdBatalla = dr.GetInt32(0),
                            IdJugador1 = dr.GetInt32(1),
                            IdEquipo1 = dr.GetInt32(2),
                            IdJugador2 = dr.GetInt32(3),
                            IdEquipo2 = dr.GetInt32(4),
                            Ganador = dr.IsDBNull(5) ? (int?)null : dr.GetInt32(5),
                            Fecha = dr.GetDateTime(6)
                        };
                    }
                }
                return null;
            }
            catch (SqlException ex)
            {
                error = "Error SQL al buscar batalla: " + ex.Message;
                return null;
            }
            catch (Exception ex)
            {
                error = "Error general al buscar batalla: " + ex.Message;
                return null;
            }
        }
        public BatallaEntidad[] ObtenerTodos(out string error, int max = 1000)
        {
            error = string.Empty;
            BatallaEntidad[] arr = new BatallaEntidad[max];
            int i = 0;

            try
            {
                string sql = @"SELECT IdBatalla, IdJugador1, IdEquipo1, IdJugador2, IdEquipo2, Ganador, Fecha
                               FROM dbo.Batalla ORDER BY IdBatalla ASC";

                using (SqlDataReader dr = conexion.EjecutarReader(sql))
                {
                    while (dr.Read() && i < max)
                    {
                        arr[i] = new BatallaEntidad
                        {
                            IdBatalla = dr.GetInt32(0),
                            IdJugador1 = dr.GetInt32(1),
                            IdEquipo1 = dr.GetInt32(2),
                            IdJugador2 = dr.GetInt32(3),
                            IdEquipo2 = dr.GetInt32(4),
                            Ganador = dr.IsDBNull(5) ? (int?)null : dr.GetInt32(5),
                            Fecha = dr.GetDateTime(6)
                        };
                        i++;
                    }
                }

                BatallaEntidad[] res = new BatallaEntidad[i];
                for (int k = 0; k < i; k++) res[k] = arr[k];
                return res;
            }
            catch (SqlException ex)
            {
                error = "Error SQL al obtener batallas: " + ex.Message;
                return new BatallaEntidad[0];
            }
            catch (Exception ex)
            {
                error = "Error general al obtener batallas: " + ex.Message;
                return new BatallaEntidad[0];
            }
        }

        // ============================================================
        // OBTENER POR JUGADOR (participa como J1 o J2)
        // ============================================================
        public BatallaEntidad[] ObtenerPorJugador(int idJugador, out string error, int max = 1000)
        {
            error = string.Empty;
            BatallaEntidad[] arr = new BatallaEntidad[max];
            int i = 0;

            try
            {
                string sql = @"SELECT IdBatalla, IdJugador1, IdEquipo1, IdJugador2, IdEquipo2, Ganador, Fecha
                               FROM dbo.Batalla
                               WHERE IdJugador1=@J OR IdJugador2=@J
                               ORDER BY Fecha DESC, IdBatalla DESC";

                using (SqlDataReader dr = conexion.EjecutarReader(sql, p => p.AddWithValue("@J", idJugador)))
                {
                    while (dr.Read() && i < max)
                    {
                        arr[i] = new BatallaEntidad
                        {
                            IdBatalla = dr.GetInt32(0),
                            IdJugador1 = dr.GetInt32(1),
                            IdEquipo1 = dr.GetInt32(2),
                            IdJugador2 = dr.GetInt32(3),
                            IdEquipo2 = dr.GetInt32(4),
                            Ganador = dr.IsDBNull(5) ? (int?)null : dr.GetInt32(5),
                            Fecha = dr.GetDateTime(6)
                        };
                        i++;
                    }
                }

                BatallaEntidad[] res = new BatallaEntidad[i];
                for (int k = 0; k < i; k++) res[k] = arr[k];
                return res;
            }
            catch (SqlException ex)
            {
                error = "Error SQL al obtener batallas por jugador: " + ex.Message;
                return new BatallaEntidad[0];
            }
            catch (Exception ex)
            {
                error = "Error general al obtener batallas por jugador: " + ex.Message;
                return new BatallaEntidad[0];
            }
        }

        // ============================================================
        // OBTENER POR FECHA (comparando solo parte de fecha)
        // ============================================================
        public BatallaEntidad[] ObtenerPorFecha(DateTime fecha, out string error, int max = 1000)
        {
            error = string.Empty;
            BatallaEntidad[] arr = new BatallaEntidad[max];
            int i = 0;

            try
            {
                DateTime f = fecha.Date;
                string sql = @"SELECT IdBatalla, IdJugador1, IdEquipo1, IdJugador2, IdEquipo2, Ganador, Fecha
                               FROM dbo.Batalla
                               WHERE CONVERT(date, Fecha) = @F
                               ORDER BY IdBatalla ASC";

                using (SqlDataReader dr = conexion.EjecutarReader(sql, p => p.AddWithValue("@F", f)))
                {
                    while (dr.Read() && i < max)
                    {
                        arr[i] = new BatallaEntidad
                        {
                            IdBatalla = dr.GetInt32(0),
                            IdJugador1 = dr.GetInt32(1),
                            IdEquipo1 = dr.GetInt32(2),
                            IdJugador2 = dr.GetInt32(3),
                            IdEquipo2 = dr.GetInt32(4),
                            Ganador = dr.IsDBNull(5) ? (int?)null : dr.GetInt32(5),
                            Fecha = dr.GetDateTime(6)
                        };
                        i++;
                    }
                }

                BatallaEntidad[] res = new BatallaEntidad[i];
                for (int k = 0; k < i; k++) res[k] = arr[k];
                return res;
            }
            catch (SqlException ex)
            {
                error = "Error SQL al obtener batallas por fecha: " + ex.Message;
                return new BatallaEntidad[0];
            }
            catch (Exception ex)
            {
                error = "Error general al obtener batallas por fecha: " + ex.Message;
                return new BatallaEntidad[0];
            }
        }

        // ============================================================
        // COUNT total
        // ============================================================
        public int Count(out string error)
        {
            error = string.Empty;
            try
            {
                object o = conexion.EjecutarEscalar("SELECT COUNT(*) FROM dbo.Batalla");
                return Convert.ToInt32(o);
            }
            catch (Exception ex)
            {
                error = "Error al contar batallas: " + ex.Message;
                return 0;
            }
        }

        // ======================= Helpers internos =======================

        private bool ExisteBatalla(SqlConnection cn, SqlTransaction tx, int idBatalla)
        {
            using (SqlCommand cmd = new SqlCommand(
                "SELECT 1 FROM dbo.Batalla WHERE IdBatalla=@B", cn, tx))
            {
                cmd.Parameters.AddWithValue("@B", idBatalla);
                return cmd.ExecuteScalar() != null;
            }
        }

        private bool ExisteJugador(SqlConnection cn, SqlTransaction tx, int idJugador)
        {
            using (SqlCommand cmd = new SqlCommand(
                "SELECT 1 FROM dbo.Jugador WHERE IdJugador=@J", cn, tx))
            {
                cmd.Parameters.AddWithValue("@J", idJugador);
                return cmd.ExecuteScalar() != null;
            }
        }

        private bool ExisteEquipo(SqlConnection cn, SqlTransaction tx, int idEquipo)
        {
            using (SqlCommand cmd = new SqlCommand(
                "SELECT 1 FROM dbo.Equipo WHERE IdEquipo=@E", cn, tx))
            {
                cmd.Parameters.AddWithValue("@E", idEquipo);
                return cmd.ExecuteScalar() != null;
            }
        }

        private bool EquipoPerteneceAJugador(SqlConnection cn, SqlTransaction tx, int idEquipo, int idJugador)
        {
            using (SqlCommand cmd = new SqlCommand(
                "SELECT 1 FROM dbo.Equipo WHERE IdEquipo=@E AND IdJugador=@J", cn, tx))
            {
                cmd.Parameters.AddWithValue("@E", idEquipo);
                cmd.Parameters.AddWithValue("@J", idJugador);
                return cmd.ExecuteScalar() != null;
            }
        }

        private bool LeerJugadoresBatalla(SqlConnection cn, SqlTransaction tx, int idBatalla,
                                          out int? j1, out int? j2, out int? ganador)
        {
            j1 = 0; j2 = 0; ganador = null;
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

        // ================== Metodos puente (compatibilidad) ==================

        // Antes: Insert(BatallaEntidad, out string)
        public bool Insert(BatallaEntidad item, out string error) => Insertar(item, out error);

        // Antes: FindById(int)
        public BatallaEntidad? FindById(int idBatalla)
        {
            string _;
            return BuscarPorId(idBatalla, out _);
        }

        // Antes: GetAllSnapshot()
        public BatallaEntidad[] GetAllSnapshot()
        {
            string _;
            return ObtenerTodos(out _, 1000);
        }

        // Antes: FindAllByJugadorId(int)
        public BatallaEntidad[] FindAllByJugadorId(int idJugador)
        {
            string _;
            return ObtenerPorJugador(idJugador, out _, 1000);
        }

        // Antes: FindAllByFecha(DateTime)
        public BatallaEntidad[] FindAllByFecha(DateTime fecha)
        {
            string _;
            return ObtenerPorFecha(fecha, out _, 1000);
        }

        // Antes: Count()
        public int Count() { string _; return Count(out _); }

        // Antes: CapacidadTotal() / CapacidadRestante() (solo para compatibilidad)
        public int CapacidadTotal() { return int.MaxValue; }               // sin tope real en BD
        public int CapacidadRestante() { string _; return int.MaxValue - Count(out _); }
    }
}