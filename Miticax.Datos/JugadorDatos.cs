//UNED
//Mitica X
//Jorge Arias Melendez
//Tercer Cuatrimestre 2025
//Acceso a datos: operaciones CRUD en SQL Server para la entidad Jugador

using System;
using System.Data;
using Microsoft.Data.SqlClient;
using Miticax.Entidades;

namespace Miticax.Datos
{
    public class JugadorDatos
    {
        // Instancia de la clase de conexion, reutiliza metodos ADO.NET centralizados.
        private readonly ConexionBD conexion = new ConexionBD();

        // Inserta un jugador en dbo.Jugador
        public bool Insertar(JugadorEntidad item, out string error)
        {
            error = string.Empty;
            try
            {
                // Sentencia parametrizada para evitar inyeccion SQL.
                string sql = @"INSERT INTO dbo.Jugador
                               (IdJugador, Nombre, FechaNacimiento, Nivel, Cristales, BatallasGanadas)
                               VALUES (@Id, @Nombre, @Fecha, @Nivel, @Cristales, @Ganadas)";

                int filas = conexion.EjecutarNoQuery(sql, p =>
                {
                    p.AddWithValue("@Id", item.IdJugador);
                    p.AddWithValue("@Nombre", item.Nombre);
                    p.AddWithValue("@Fecha", item.FechaNacimiento);
                    p.AddWithValue("@Nivel", item.Nivel);
                    p.AddWithValue("@Cristales", item.Cristales);
                    p.AddWithValue("@Ganadas", item.BatallasGanadas);
                });

                return filas == 1; // true si exactamente 1 fila fue afectada
            }
            catch (SqlException ex)
            {
                error = "Error SQL al insertar jugador: " + ex.Message;
                return false;
            }
            catch (Exception ex)
            {
                error = "Error general al insertar jugador: " + ex.Message;
                return false;
            }
        }

        // Actualiza los datos basicos del jugador
        public bool Actualizar(JugadorEntidad item, out string error)
        {
            error = string.Empty;
            try
            {
                string sql = @"UPDATE dbo.Jugador SET
                               Nombre=@Nombre, FechaNacimiento=@Fecha, Nivel=@Nivel,
                               Cristales=@Cristales, BatallasGanadas=@Ganadas
                               WHERE IdJugador=@Id";

                int filas = conexion.EjecutarNoQuery(sql, p =>
                {
                    p.AddWithValue("@Nombre", item.Nombre);
                    p.AddWithValue("@Fecha", item.FechaNacimiento);
                    p.AddWithValue("@Nivel", item.Nivel);
                    p.AddWithValue("@Cristales", item.Cristales);
                    p.AddWithValue("@Ganadas", item.BatallasGanadas);
                    p.AddWithValue("@Id", item.IdJugador);
                });

                return filas == 1;
            }
            catch (SqlException ex)
            {
                error = "Error SQL al actualizar jugador: " + ex.Message;
                return false;
            }
            catch (Exception ex)
            {
                error = "Error general al actualizar jugador: " + ex.Message;
                return false;
            }
        }

        // Elimina un jugador por su Id
        public bool Eliminar(int idJugador, out string error)
        {
            error = string.Empty;
            try
            {
                string sql = "DELETE FROM dbo.Jugador WHERE IdJugador=@Id";
                int filas = conexion.EjecutarNoQuery(sql, p => p.AddWithValue("@Id", idJugador));
                return filas == 1;
            }
            catch (SqlException ex)
            {
                error = "Error SQL al eliminar jugador: " + ex.Message;
                return false;
            }
            catch (Exception ex)
            {
                error = "Error general al eliminar jugador: " + ex.Message;
                return false;
            }
        }

        // Busca un jugador por Id y lo retorna (o null si no existe)
        public JugadorEntidad? BuscarPorId(int idJugador, out string error)
        {
            error = string.Empty;
            try
            {
                string sql = @"SELECT IdJugador, Nombre, FechaNacimiento, Nivel, Cristales, BatallasGanadas
                               FROM dbo.Jugador WHERE IdJugador=@Id";

                using (SqlDataReader dr = conexion.EjecutarReader(sql, p => p.AddWithValue("@Id", idJugador)))
                {
                    if (dr.Read())
                    {
                        // Mapeo de columnas a propiedades de la entidad
                        return new JugadorEntidad
                        {
                            IdJugador = dr.GetInt32(0),
                            Nombre = dr.GetString(1),
                            FechaNacimiento = dr.GetDateTime(2),
                            Nivel = dr.GetInt32(3),
                            Cristales = dr.GetInt32(4),
                            BatallasGanadas = dr.GetInt32(5)
                        };
                    }
                }
                return null; // no encontrado
            }
            catch (SqlException ex)
            {
                error = "Error SQL al buscar jugador: " + ex.Message;
                return null;
            }
            catch (Exception ex)
            {
                error = "Error general al buscar jugador: " + ex.Message;
                return null;
            }
        }

        // Obtiene todos los jugadores como arreglo (sin colecciones)
        public JugadorEntidad[] ObtenerTodos(out string error, int max = 500)
        {
            error = string.Empty;
            JugadorEntidad[] arreglo = new JugadorEntidad[max]; // arreglo fijo
            int i = 0;

            try
            {
                string sql = @"SELECT IdJugador, Nombre, FechaNacimiento, Nivel, Cristales, BatallasGanadas
                               FROM dbo.Jugador
                               ORDER BY IdJugador ASC";

                using (SqlDataReader dr = conexion.EjecutarReader(sql))
                {
                    // Llena el arreglo sin exceder 'max'
                    while (dr.Read() && i < max)
                    {
                        arreglo[i] = new JugadorEntidad
                        {
                            IdJugador = dr.GetInt32(0),
                            Nombre = dr.GetString(1),
                            FechaNacimiento = dr.GetDateTime(2),
                            Nivel = dr.GetInt32(3),
                            Cristales = dr.GetInt32(4),
                            BatallasGanadas = dr.GetInt32(5)
                        };
                        i++;
                    }
                }

                // Recorta a la cantidad real cargada
                JugadorEntidad[] resultado = new JugadorEntidad[i];
                for (int k = 0; k < i; k++) resultado[k] = arreglo[k];
                return resultado;
            }
            catch (SqlException ex)
            {
                error = "Error SQL al obtener jugadores: " + ex.Message;
                return new JugadorEntidad[0];
            }
            catch (Exception ex)
            {
                error = "Error general al obtener jugadores: " + ex.Message;
                return new JugadorEntidad[0];
            }
        }
    }
}