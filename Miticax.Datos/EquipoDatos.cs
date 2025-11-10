//UNED
//Mitica X
//Jorge Arias Melendez
//Tercer cuatrimestre 2025
//Acceso a datos: CRUD de Equipo en SQL Server con validaciones contra Jugador e Inventario

using System;
using System.Data;
using Microsoft.Data.SqlClient;
using Miticax.Entidades;

namespace Miticax.Datos
{
    public class EquipoDatos
    {
        // Reutiliza la clase de conexion centralizada
        private readonly ConexionBD conexion = new ConexionBD();

        // -------------------------------------------------------------
        // INSERTAR con validaciones:
        // - IdEquipo no debe existir (PK)
        // - Jugador existe
        // - Las 3 criaturas son distintas
        // - Las 3 criaturas pertenecen al inventario del mismo jugador
        // Todo en una transaccion para consistencia
        // -------------------------------------------------------------
        public bool Insertar(EquipoEntidad item, out string error)
        {
            error = string.Empty;

            // Validacion rapida en memoria: 3 criaturas distintas
            if (!SonDistintas(item.IdCriatura1, item.IdCriatura2, item.IdCriatura3))
            {
                error = "Las criaturas del equipo no pueden repetirse.";
                return false;
            }

            using (SqlConnection cn = conexion.AbrirConexion())
            using (SqlTransaction tx = cn.BeginTransaction())
            {
                try
                {
                    // 0) PK no duplicada
                    if (ExisteEquipo(cn, tx, item.IdEquipo))
                    {
                        error = "Ya existe un equipo con ese IdEquipo.";
                        tx.Rollback();
                        return false;
                    }

                    // 1) Jugador existe
                    if (!ExisteJugador(cn, tx, item.IdJugador))
                    {
                        error = "El jugador especificado no existe.";
                        tx.Rollback();
                        return false;
                    }

                    // 2) Criaturas pertenecen al inventario del jugador
                    if (!ExisteEnInventario(cn, tx, item.IdJugador, item.IdCriatura1) ||
                        !ExisteEnInventario(cn, tx, item.IdJugador, item.IdCriatura2) ||
                        !ExisteEnInventario(cn, tx, item.IdJugador, item.IdCriatura3))
                    {
                        error = "Una o mas criaturas no pertenecen al inventario del jugador.";
                        tx.Rollback();
                        return false;
                    }

                    // 3) Insertar registro
                    using (SqlCommand cmd = new SqlCommand(
                        @"INSERT INTO dbo.Equipo (IdEquipo, IdJugador, IdCriatura1, IdCriatura2, IdCriatura3)
                          VALUES (@E, @J, @C1, @C2, @C3)", cn, tx))
                    {
                        cmd.Parameters.AddWithValue("@E", item.IdEquipo);
                        cmd.Parameters.AddWithValue("@J", item.IdJugador);
                        cmd.Parameters.AddWithValue("@C1", item.IdCriatura1);
                        cmd.Parameters.AddWithValue("@C2", item.IdCriatura2);
                        cmd.Parameters.AddWithValue("@C3", item.IdCriatura3);

                        int filas = cmd.ExecuteNonQuery();
                        if (filas != 1)
                        {
                            error = "No se pudo insertar el equipo.";
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
                    error = "Error SQL al insertar equipo: " + ex.Message;
                    return false;
                }
                catch (Exception ex)
                {
                    try { tx.Rollback(); } catch { }
                    error = "Error general al insertar equipo: " + ex.Message;
                    return false;
                }
            }
        }

        // -------------------------------------------------------------
        // ACTUALIZAR con validaciones (mismas reglas que Insertar)
        // -------------------------------------------------------------
        public bool Actualizar(EquipoEntidad item, out string error)
        {
            error = string.Empty;

            if (!SonDistintas(item.IdCriatura1, item.IdCriatura2, item.IdCriatura3))
            {
                error = "Las criaturas del equipo no pueden repetirse.";
                return false;
            }

            using (SqlConnection cn = conexion.AbrirConexion())
            using (SqlTransaction tx = cn.BeginTransaction())
            {
                try
                {
                    // Debe existir el equipo
                    if (!ExisteEquipo(cn, tx, item.IdEquipo))
                    {
                        error = "No existe un equipo con ese IdEquipo.";
                        tx.Rollback();
                        return false;
                    }

                    if (!ExisteJugador(cn, tx, item.IdJugador))
                    {
                        error = "El jugador especificado no existe.";
                        tx.Rollback();
                        return false;
                    }

                    if (!ExisteEnInventario(cn, tx, item.IdJugador, item.IdCriatura1) ||
                        !ExisteEnInventario(cn, tx, item.IdJugador, item.IdCriatura2) ||
                        !ExisteEnInventario(cn, tx, item.IdJugador, item.IdCriatura3))
                    {
                        error = "Una o mas criaturas no pertenecen al inventario del jugador.";
                        tx.Rollback();
                        return false;
                    }

                    using (SqlCommand cmd = new SqlCommand(
                        @"UPDATE dbo.Equipo SET
                          IdJugador=@J, IdCriatura1=@C1, IdCriatura2=@C2, IdCriatura3=@C3
                          WHERE IdEquipo=@E", cn, tx))
                    {
                        cmd.Parameters.AddWithValue("@E", item.IdEquipo);
                        cmd.Parameters.AddWithValue("@J", item.IdJugador);
                        cmd.Parameters.AddWithValue("@C1", item.IdCriatura1);
                        cmd.Parameters.AddWithValue("@C2", item.IdCriatura2);
                        cmd.Parameters.AddWithValue("@C3", item.IdCriatura3);

                        int filas = cmd.ExecuteNonQuery();
                        if (filas != 1)
                        {
                            error = "No se pudo actualizar el equipo.";
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
                    error = "Error SQL al actualizar equipo: " + ex.Message;
                    return false;
                }
                catch (Exception ex)
                {
                    try { tx.Rollback(); } catch { }
                    error = "Error general al actualizar equipo: " + ex.Message;
                    return false;
                }
            }
        }

        // -------------------------------------------------------------
        // ELIMINAR por IdEquipo
        // -------------------------------------------------------------
        public bool Eliminar(int idEquipo, out string error)
        {
            error = string.Empty;
            try
            {
                string sql = "DELETE FROM dbo.Equipo WHERE IdEquipo=@E";
                int filas = conexion.EjecutarNoQuery(sql, p => p.AddWithValue("@E", idEquipo));
                return filas == 1;
            }
            catch (SqlException ex)
            {
                error = "Error SQL al eliminar equipo: " + ex.Message;
                return false;
            }
            catch (Exception ex)
            {
                error = "Error general al eliminar equipo: " + ex.Message;
                return false;
            }
        }

        // -------------------------------------------------------------
        // BUSCAR por IdEquipo
        // -------------------------------------------------------------
        public EquipoEntidad? BuscarPorId(int idEquipo, out string error)
        {
            error = string.Empty;
            try
            {
                string sql = @"SELECT IdEquipo, IdJugador, IdCriatura1, IdCriatura2, IdCriatura3
                               FROM dbo.Equipo WHERE IdEquipo=@E";

                using (SqlDataReader dr = conexion.EjecutarReader(sql, p => p.AddWithValue("@E", idEquipo)))
                {
                    if (dr.Read())
                    {
                        return new EquipoEntidad
                        {
                            IdEquipo = dr.GetInt32(0),
                            IdJugador = dr.GetInt32(1),
                            IdCriatura1 = dr.GetInt32(2),
                            IdCriatura2 = dr.GetInt32(3),
                            IdCriatura3 = dr.GetInt32(4)
                        };
                    }
                }
                return null;
            }
            catch (SqlException ex)
            {
                error = "Error SQL al buscar equipo: " + ex.Message;
                return null;
            }
            catch (Exception ex)
            {
                error = "Error general al buscar equipo: " + ex.Message;
                return null;
            }
        }

        // -------------------------------------------------------------
        // OBTENER TODOS (arreglo sin List/LINQ)
        // -------------------------------------------------------------
        public EquipoEntidad[] ObtenerTodos(out string error, int max = 500)
        {
            error = string.Empty;
            EquipoEntidad[] arr = new EquipoEntidad[max];
            int i = 0;

            try
            {
                string sql = @"SELECT IdEquipo, IdJugador, IdCriatura1, IdCriatura2, IdCriatura3
                               FROM dbo.Equipo ORDER BY IdEquipo ASC";

                using (SqlDataReader dr = conexion.EjecutarReader(sql))
                {
                    while (dr.Read() && i < max)
                    {
                        arr[i] = new EquipoEntidad
                        {
                            IdEquipo = dr.GetInt32(0),
                            IdJugador = dr.GetInt32(1),
                            IdCriatura1 = dr.GetInt32(2),
                            IdCriatura2 = dr.GetInt32(3),
                            IdCriatura3 = dr.GetInt32(4)
                        };
                        i++;
                    }
                }

                EquipoEntidad[] res = new EquipoEntidad[i];
                for (int k = 0; k < i; k++) res[k] = arr[k];
                return res;
            }
            catch (SqlException ex)
            {
                error = "Error SQL al obtener equipos: " + ex.Message;
                return new EquipoEntidad[0];
            }
            catch (Exception ex)
            {
                error = "Error general al obtener equipos: " + ex.Message;
                return new EquipoEntidad[0];
            }
        }

        // -------------------------------------------------------------
        // OBTENER POR JUGADOR (arreglo exacto)
        // -------------------------------------------------------------
        public EquipoEntidad[] ObtenerPorJugador(int idJugador, out string error, int max = 200)
        {
            error = string.Empty;
            EquipoEntidad[] arr = new EquipoEntidad[max];
            int i = 0;

            try
            {
                string sql = @"SELECT IdEquipo, IdJugador, IdCriatura1, IdCriatura2, IdCriatura3
                               FROM dbo.Equipo WHERE IdJugador=@J ORDER BY IdEquipo ASC";

                using (SqlDataReader dr = conexion.EjecutarReader(sql, p => p.AddWithValue("@J", idJugador)))
                {
                    while (dr.Read() && i < max)
                    {
                        arr[i] = new EquipoEntidad
                        {
                            IdEquipo = dr.GetInt32(0),
                            IdJugador = dr.GetInt32(1),
                            IdCriatura1 = dr.GetInt32(2),
                            IdCriatura2 = dr.GetInt32(3),
                            IdCriatura3 = dr.GetInt32(4)
                        };
                        i++;
                    }
                }

                EquipoEntidad[] res = new EquipoEntidad[i];
                for (int k = 0; k < i; k++) res[k] = arr[k];
                return res;
            }
            catch (SqlException ex)
            {
                error = "Error SQL al obtener equipos del jugador: " + ex.Message;
                return new EquipoEntidad[0];
            }
            catch (Exception ex)
            {
                error = "Error general al obtener equipos del jugador: " + ex.Message;
                return new EquipoEntidad[0];
            }
        }

        // -------------------------------------------------------------
        // COUNT total (por si tu Logica lo usa)
        // -------------------------------------------------------------
        public int Count(out string error)
        {
            error = string.Empty;
            try
            {
                object o = conexion.EjecutarEscalar("SELECT COUNT(*) FROM dbo.Equipo");
                return Convert.ToInt32(o);
            }
            catch (Exception ex)
            {
                error = "Error al contar equipos: " + ex.Message;
                return 0;
            }
        }

        // ======================= Helpers internos =======================

        private static bool SonDistintas(int a, int b, int c)
        {
            return (a != b) && (a != c) && (b != c);
        }

        private bool ExisteEquipo(SqlConnection cn, SqlTransaction tx, int idEquipo)
        {
            using (SqlCommand cmd = new SqlCommand(
                "SELECT 1 FROM dbo.Equipo WHERE IdEquipo=@E", cn, tx))
            {
                cmd.Parameters.AddWithValue("@E", idEquipo);
                object o = cmd.ExecuteScalar();
                return o != null;
            }
        }

        private bool ExisteJugador(SqlConnection cn, SqlTransaction tx, int idJugador)
        {
            using (SqlCommand cmd = new SqlCommand(
                "SELECT 1 FROM dbo.Jugador WHERE IdJugador=@J", cn, tx))
            {
                cmd.Parameters.AddWithValue("@J", idJugador);
                object o = cmd.ExecuteScalar();
                return o != null;
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
                object o = cmd.ExecuteScalar();
                return o != null;
            }
        }

        // --------------- Metodos puente (compatibilidad API vieja) ---------------
        // Antes: Insert(EquipoEntidad, out string)
        public bool Insert(EquipoEntidad item, out string error)
        {
            return Insertar(item, out error);
        }

        // Antes: FindById(int)
        public EquipoEntidad? FindById(int idEquipo)
        {
            string _;
            return BuscarPorId(idEquipo, out _);
        }

        // Antes: GetAllSnapshot()
        public EquipoEntidad[] GetAllSnapshot()
        {
            string _;
            return ObtenerTodos(out _, 500);
        }

        // Antes: FindAllByJugadorId(int)
        public EquipoEntidad[] FindAllByJugadorId(int idJugador)
        {
            string _;
            return ObtenerPorJugador(idJugador, out _, 200);
        }

        // Antes: Count()
        public int Count()
        {
            string _;
            return Count(out _);
        }
    }
}