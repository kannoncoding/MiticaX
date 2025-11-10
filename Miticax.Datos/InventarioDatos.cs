//UNED
//Mitica X
//Jorge Arias Melendez
//Tercer cuatrimestre 2025
//Acceso a datos: Inventario del jugador con CRUD y compra atomica en SQL Server

using System;
using System.Data;
using Microsoft.Data.SqlClient;
using Miticax.Entidades;

namespace Miticax.Datos
{
    public class InventarioDatos
    {
        // Conexion compartida
        private readonly ConexionBD conexion = new ConexionBD();

        public bool ComprarCriatura(int idJugador, int idCriatura, out string error)
        {
            error = string.Empty;

            // Abrimos conexion manualmente para poder crear una transaccion
            using (SqlConnection cn = conexion.AbrirConexion())
            using (SqlTransaction tx = cn.BeginTransaction())
            {
                try
                {
                    // 1) Validar existencia de jugador y obtener cristales actuales
                    int cristalesActuales = 0;
                    using (SqlCommand cmd = new SqlCommand(
                        "SELECT Cristales FROM dbo.Jugador WHERE IdJugador=@J", cn, tx))
                    {
                        cmd.Parameters.AddWithValue("@J", idJugador);
                        object o = cmd.ExecuteScalar();
                        if (o == null)
                        {
                            error = "El jugador no existe.";
                            tx.Rollback();
                            return false;
                        }
                        cristalesActuales = Convert.ToInt32(o);
                    }

                    // 2) Validar existencia de criatura y obtener costo/poder/resistencia
                    int costo = 0, poderIni = 0, resIni = 0;
                    using (SqlCommand cmd = new SqlCommand(
                        "SELECT Costo, Poder, Resistencia FROM dbo.Criatura WHERE IdCriatura=@C", cn, tx))
                    {
                        cmd.Parameters.AddWithValue("@C", idCriatura);
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (!dr.Read())
                            {
                                error = "La criatura no existe.";
                                tx.Rollback();
                                return false;
                            }
                            costo = dr.GetInt32(0);
                            poderIni = dr.GetInt32(1);
                            resIni = dr.GetInt32(2);
                        }
                    }

                    // 3) Verificar que no exista ya en inventario (UNIQUE(IdJugador,IdCriatura))
                    using (SqlCommand cmd = new SqlCommand(
                        @"SELECT 1 FROM dbo.InventarioJugador 
                          WHERE IdJugador=@J AND IdCriatura=@C", cn, tx))
                    {
                        cmd.Parameters.AddWithValue("@J", idJugador);
                        cmd.Parameters.AddWithValue("@C", idCriatura);
                        object o = cmd.ExecuteScalar();
                        if (o != null)
                        {
                            error = "El jugador ya posee esa criatura en su inventario.";
                            tx.Rollback();
                            return false;
                        }
                    }

                    // 4) Verificar cristales suficientes
                    if (cristalesActuales < costo)
                    {
                        error = "No posee cristales suficientes para comprar la criatura.";
                        tx.Rollback();
                        return false;
                    }

                    // 5) Insertar en InventarioJugador con los stats iniciales
                    using (SqlCommand cmd = new SqlCommand(
                        @"INSERT INTO dbo.InventarioJugador 
                          (IdJugador, IdCriatura, Poder, Resistencia)
                          VALUES (@J, @C, @P, @R)", cn, tx))
                    {
                        cmd.Parameters.AddWithValue("@J", idJugador);
                        cmd.Parameters.AddWithValue("@C", idCriatura);
                        cmd.Parameters.AddWithValue("@P", poderIni);
                        cmd.Parameters.AddWithValue("@R", resIni);
                        int filas = cmd.ExecuteNonQuery();
                        if (filas != 1)
                        {
                            error = "No se pudo insertar en el inventario.";
                            tx.Rollback();
                            return false;
                        }
                    }

                    // 6) Descontar cristales al jugador
                    using (SqlCommand cmd = new SqlCommand(
                        @"UPDATE dbo.Jugador 
                          SET Cristales = Cristales - @Costo
                          WHERE IdJugador=@J", cn, tx))
                    {
                        cmd.Parameters.AddWithValue("@Costo", costo);
                        cmd.Parameters.AddWithValue("@J", idJugador);
                        int filas = cmd.ExecuteNonQuery();
                        if (filas != 1)
                        {
                            error = "No se pudo actualizar el saldo de cristales del jugador.";
                            tx.Rollback();
                            return false;
                        }
                    }

                    // 7) Confirmar
                    tx.Commit();
                    return true;
                }
                catch (SqlException ex)
                {
                    // Ante error SQL, intentar revertir
                    try { tx.Rollback(); } catch { /* si falla rollback, no hacemos nada */ }
                    error = "Error SQL en compra: " + ex.Message;
                    return false;
                }
                catch (Exception ex)
                {
                    try { tx.Rollback(); } catch { }
                    error = "Error general en compra: " + ex.Message;
                    return false;
                }
            }
        }

        // --------------------------------------------------------------------
        // INSERTAR DIRECTO (sin logica de compra). Util por pruebas.
        // Inserta un registro de inventario con los valores provistos.
        // --------------------------------------------------------------------
        public bool Insertar(InventarioJugadorEntidad item, out string error)
        {
            error = string.Empty;
            try
            {
                string sql = @"INSERT INTO dbo.InventarioJugador
                               (IdJugador, IdCriatura, Poder, Resistencia)
                               VALUES (@J, @C, @P, @R)";

                int filas = conexion.EjecutarNoQuery(sql, p =>
                {
                    p.AddWithValue("@J", item.IdJugador);
                    p.AddWithValue("@C", item.IdCriatura);
                    p.AddWithValue("@P", item.Poder);
                    p.AddWithValue("@R", item.Resistencia);
                });

                return filas == 1;
            }
            catch (SqlException ex)
            {
                error = "Error SQL al insertar inventario: " + ex.Message;
                return false;
            }
            catch (Exception ex)
            {
                error = "Error general al insertar inventario: " + ex.Message;
                return false;
            }
        }

        // --------------------------------------------------------------------
        // BUSCAR por (IdJugador, IdCriatura)
        // --------------------------------------------------------------------
        public InventarioJugadorEntidad? BuscarPorJugadorYCriatura(int idJugador, int idCriatura, out string error)
        {
            error = string.Empty;
            try
            {
                string sql = @"SELECT IdJugador, IdCriatura, Poder, Resistencia
                               FROM dbo.InventarioJugador
                               WHERE IdJugador=@J AND IdCriatura=@C";

                using (SqlDataReader dr = conexion.EjecutarReader(sql, p =>
                {
                    p.AddWithValue("@J", idJugador);
                    p.AddWithValue("@C", idCriatura);
                }))
                {
                    if (dr.Read())
                    {
                        return new InventarioJugadorEntidad
                        {
                            IdJugador = dr.GetInt32(0),
                            IdCriatura = dr.GetInt32(1),
                            Poder = dr.GetInt32(2),
                            Resistencia = dr.GetInt32(3)
                        };
                    }
                }
                return null;
            }
            catch (SqlException ex)
            {
                error = "Error SQL al buscar inventario: " + ex.Message;
                return null;
            }
            catch (Exception ex)
            {
                error = "Error general al buscar inventario: " + ex.Message;
                return null;
            }
        }

        // --------------------------------------------------------------------
        // OBTENER TODO como arreglo (sin List ni LINQ)
        // --------------------------------------------------------------------
        public InventarioJugadorEntidad[] ObtenerTodos(out string error, int max = 2000)
        {
            error = string.Empty;
            InventarioJugadorEntidad[] arr = new InventarioJugadorEntidad[max];
            int i = 0;

            try
            {
                string sql = @"SELECT IdJugador, IdCriatura, Poder, Resistencia
                               FROM dbo.InventarioJugador
                               ORDER BY IdJugador, IdCriatura";

                using (SqlDataReader dr = conexion.EjecutarReader(sql))
                {
                    while (dr.Read() && i < max)
                    {
                        arr[i] = new InventarioJugadorEntidad
                        {
                            IdJugador = dr.GetInt32(0),
                            IdCriatura = dr.GetInt32(1),
                            Poder = dr.GetInt32(2),
                            Resistencia = dr.GetInt32(3)
                        };
                        i++;
                    }
                }

                // recorte exacto
                InventarioJugadorEntidad[] res = new InventarioJugadorEntidad[i];
                for (int k = 0; k < i; k++) res[k] = arr[k];
                return res;
            }
            catch (SqlException ex)
            {
                error = "Error SQL al obtener inventarios: " + ex.Message;
                return new InventarioJugadorEntidad[0];
            }
            catch (Exception ex)
            {
                error = "Error general al obtener inventarios: " + ex.Message;
                return new InventarioJugadorEntidad[0];
            }
        }

        // --------------------------------------------------------------------
        // OBTENER TODOS POR JUGADOR (arreglo exacto)
        // --------------------------------------------------------------------
        public InventarioJugadorEntidad[] ObtenerPorJugador(int idJugador, out string error, int maxPorJugador = 200)
        {
            error = string.Empty;
            InventarioJugadorEntidad[] arr = new InventarioJugadorEntidad[maxPorJugador];
            int i = 0;

            try
            {
                string sql = @"SELECT IdJugador, IdCriatura, Poder, Resistencia
                               FROM dbo.InventarioJugador
                               WHERE IdJugador=@J
                               ORDER BY IdCriatura";

                using (SqlDataReader dr = conexion.EjecutarReader(sql, p => p.AddWithValue("@J", idJugador)))
                {
                    while (dr.Read() && i < maxPorJugador)
                    {
                        arr[i] = new InventarioJugadorEntidad
                        {
                            IdJugador = dr.GetInt32(0),
                            IdCriatura = dr.GetInt32(1),
                            Poder = dr.GetInt32(2),
                            Resistencia = dr.GetInt32(3)
                        };
                        i++;
                    }
                }

                InventarioJugadorEntidad[] res = new InventarioJugadorEntidad[i];
                for (int k = 0; k < i; k++) res[k] = arr[k];
                return res;
            }
            catch (SqlException ex)
            {
                error = "Error SQL al obtener inventario del jugador: " + ex.Message;
                return new InventarioJugadorEntidad[0];
            }
            catch (Exception ex)
            {
                error = "Error general al obtener inventario del jugador: " + ex.Message;
                return new InventarioJugadorEntidad[0];
            }
        }

        // --------------------------------------------------------------------
        // Metodos puente (compatibilidad con firmas antiguas, si tu Logica aun las usa)
        // --------------------------------------------------------------------
        // Antes: FindByJugadorAndCriatura(int,int)
        public InventarioJugadorEntidad? FindByJugadorAndCriatura(int idJugador, int idCriatura)
        {
            string _;
            return BuscarPorJugadorYCriatura(idJugador, idCriatura, out _);
        }

        // Antes: GetAllSnapshot()
        public InventarioJugadorEntidad[] GetAllSnapshot()
        {
            string _;
            return ObtenerTodos(out _, 2000);
        }

        // Antes: FindAllByJugadorId(int)
        public InventarioJugadorEntidad[] FindAllByJugadorId(int idJugador)
        {
            string _;
            return ObtenerPorJugador(idJugador, out _, 200);
        }

        // Helper de existencia (API previa)
        public bool ExisteJugadorCriatura(int idJugador, int idCriatura)
        {
            return FindByJugadorAndCriatura(idJugador, idCriatura) != null;
        }
    }
}