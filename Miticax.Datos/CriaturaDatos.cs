//UNED
//Mitica X
//Jorge Arias Melendez
//Tercer cuatrimestre 2025
//Acceso a datos: operaciones CRUD en SQL Server para la entidad Criatura

using System;
using System.Data;
using Microsoft.Data.SqlClient;
using Miticax.Entidades;

namespace Miticax.Datos
{
    public class CriaturaDatos
    {
        // Instancia de la clase de conexion para reutilizar los metodos de ADO.NET
        private readonly ConexionBD conexion = new ConexionBD();

        // Inserta una criatura en la tabla dbo.Criatura
        public bool Insertar(CriaturaEntidad item, out string error)
        {
            error = string.Empty;
            try
            {
                string sql = @"INSERT INTO dbo.Criatura
                               (IdCriatura, Nombre, Tipo, Nivel, Poder, Resistencia, Costo)
                               VALUES (@Id, @Nombre, @Tipo, @Nivel, @Poder, @Resistencia, @Costo)";

                int filas = conexion.EjecutarNoQuery(sql, p =>
                {
                    p.AddWithValue("@Id", item.IdCriatura);
                    p.AddWithValue("@Nombre", item.Nombre);
                    p.AddWithValue("@Tipo", item.Tipo);
                    p.AddWithValue("@Nivel", item.Nivel);
                    p.AddWithValue("@Poder", item.Poder);
                    p.AddWithValue("@Resistencia", item.Resistencia);
                    p.AddWithValue("@Costo", item.Costo);
                });

                return filas == 1;
            }
            catch (SqlException ex)
            {
                error = "Error SQL al insertar criatura: " + ex.Message;
                return false;
            }
            catch (Exception ex)
            {
                error = "Error general al insertar criatura: " + ex.Message;
                return false;
            }
        }

        // Actualiza una criatura existente en la BD
        public bool Actualizar(CriaturaEntidad item, out string error)
        {
            error = string.Empty;
            try
            {
                string sql = @"UPDATE dbo.Criatura SET 
                               Nombre=@Nombre, Tipo=@Tipo, Nivel=@Nivel,
                               Poder=@Poder, Resistencia=@Resistencia, Costo=@Costo
                               WHERE IdCriatura=@Id";

                int filas = conexion.EjecutarNoQuery(sql, p =>
                {
                    p.AddWithValue("@Nombre", item.Nombre);
                    p.AddWithValue("@Tipo", item.Tipo);
                    p.AddWithValue("@Nivel", item.Nivel);
                    p.AddWithValue("@Poder", item.Poder);
                    p.AddWithValue("@Resistencia", item.Resistencia);
                    p.AddWithValue("@Costo", item.Costo);
                    p.AddWithValue("@Id", item.IdCriatura);
                });

                return filas == 1;
            }
            catch (SqlException ex)
            {
                error = "Error SQL al actualizar criatura: " + ex.Message;
                return false;
            }
            catch (Exception ex)
            {
                error = "Error general al actualizar criatura: " + ex.Message;
                return false;
            }
        }

        // Elimina una criatura por su Id
        public bool Eliminar(int idCriatura, out string error)
        {
            error = string.Empty;
            try
            {
                string sql = "DELETE FROM dbo.Criatura WHERE IdCriatura=@Id";

                int filas = conexion.EjecutarNoQuery(sql, p =>
                {
                    p.AddWithValue("@Id", idCriatura);
                });

                return filas == 1;
            }
            catch (SqlException ex)
            {
                error = "Error SQL al eliminar criatura: " + ex.Message;
                return false;
            }
            catch (Exception ex)
            {
                error = "Error general al eliminar criatura: " + ex.Message;
                return false;
            }
        }

        // Busca una criatura por su Id
        public CriaturaEntidad? BuscarPorId(int idCriatura, out string error)
        {
            error = string.Empty;
            try
            {
                string sql = @"SELECT IdCriatura, Nombre, Tipo, Nivel, Poder, Resistencia, Costo
                               FROM dbo.Criatura WHERE IdCriatura=@Id";

                using (SqlDataReader dr = conexion.EjecutarReader(sql, p => p.AddWithValue("@Id", idCriatura)))
                {
                    if (dr.Read())
                    {
                        // Mapea los campos leídos a una instancia de CriaturaEntidad
                        return new CriaturaEntidad
                        {
                            IdCriatura = dr.GetInt32(0),
                            Nombre = dr.GetString(1),
                            Tipo = dr.GetString(2),
                            Nivel = dr.GetInt32(3),
                            Poder = dr.GetInt32(4),
                            Resistencia = dr.GetInt32(5),
                            Costo = dr.GetInt32(6)
                        };
                    }
                }
                return null;
            }
            catch (SqlException ex)
            {
                error = "Error SQL al buscar criatura: " + ex.Message;
                return null;
            }
            catch (Exception ex)
            {
                error = "Error general al buscar criatura: " + ex.Message;
                return null;
            }
        }

        // Obtiene todas las criaturas como un arreglo de entidades (limite maximo configurable)
        public CriaturaEntidad[] ObtenerTodos(out string error, int max = 200)
        {
            error = string.Empty;
            CriaturaEntidad[] arreglo = new CriaturaEntidad[max];
            int i = 0;

            try
            {
                string sql = @"SELECT IdCriatura, Nombre, Tipo, Nivel, Poder, Resistencia, Costo
                               FROM dbo.Criatura ORDER BY IdCriatura ASC";

                using (SqlDataReader dr = conexion.EjecutarReader(sql))
                {
                    while (dr.Read() && i < max)
                    {
                        arreglo[i] = new CriaturaEntidad
                        {
                            IdCriatura = dr.GetInt32(0),
                            Nombre = dr.GetString(1),
                            Tipo = dr.GetString(2),
                            Nivel = dr.GetInt32(3),
                            Poder = dr.GetInt32(4),
                            Resistencia = dr.GetInt32(5),
                            Costo = dr.GetInt32(6)
                        };
                        i++;
                    }
                }

                // Redimensiona el arreglo a la cantidad real leída
                CriaturaEntidad[] resultado = new CriaturaEntidad[i];
                for (int j = 0; j < i; j++)
                {
                    resultado[j] = arreglo[j];
                }
                return resultado;
            }
            catch (SqlException ex)
            {
                error = "Error SQL al obtener criaturas: " + ex.Message;
                return new CriaturaEntidad[0];
            }
            catch (Exception ex)
            {
                error = "Error general al obtener criaturas: " + ex.Message;
                return new CriaturaEntidad[0];
            }
        }
    }
}
