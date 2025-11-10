//UNED
//Mitica X
//Jorge Arias Melendez
//Tercer Cuatrimestre 2025
//Clase de conexion a SQL Server con metodos de ayuda para ADO.NET

using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace Miticax.Datos
{
    // Clase de acceso a datos: abre conexiones y ejecuta comandos SQL.
    public class ConexionBD
    {
        // Cadena de conexion por defecto: SQLEXPRESS local con seguridad integrada.
        private const string CadenaPorDefecto = "Server=.\\SQLEXPRESS;Database=MiticaxDB;Trusted_Connection=True;TrustServerCertificate=True;";

        // Permite sobreescribir la cadena mediante variable de entorno (evita recompilar para cambiarla).
        private static readonly string cadena;

        static ConexionBD()
        {
            string? desdeEntorno = Environment.GetEnvironmentVariable("MITICAX_SQL_CONNECTION");
            cadena = string.IsNullOrWhiteSpace(desdeEntorno) ? CadenaPorDefecto : desdeEntorno;
        }

        // Metodo que retorna una conexion abierta lista para usar.
        public SqlConnection AbrirConexion()
        {
            // Crea la conexion con la cadena indicada.
            SqlConnection cn = new SqlConnection(cadena);
            try
            {
                // Intenta abrir la conexion.
                cn.Open();
                // Retorna la conexion abierta al llamador.
                return cn;
            }
            catch
            {
                // Asegura liberar recursos si la apertura falla.
                cn.Dispose();
                throw;
            }
        }

        // Ejecuta un comando INSERT/UPDATE/DELETE y retorna filas afectadas.
        public int EjecutarNoQuery(string sql, Action<SqlParameterCollection>? parametros = null)
        {
            // Usa using para garantizar Dispose de conexion y comando.
            using (SqlConnection cn = AbrirConexion())
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                // Si el llamador desea agregar parametros, los agrega aqui.
                parametros?.Invoke(cmd.Parameters);
                // Ejecuta y retorna las filas afectadas.
                int afectadas = cmd.ExecuteNonQuery();
                return afectadas;
            }
        }

        // Ejecuta un SELECT y retorna un SqlDataReader abierto (el llamador debe leer y cerrar).
        public SqlDataReader EjecutarReader(string sql, Action<SqlParameterCollection>? parametros = null)
        {
            SqlConnection cn = AbrirConexion(); // se cierra cuando cerremos el reader
            SqlCommand cmd = new SqlCommand(sql, cn);
            parametros?.Invoke(cmd.Parameters);
            // CommandBehavior.CloseConnection permite que al cerrar el reader, se cierre la conexion.
            return cmd.ExecuteReader(CommandBehavior.CloseConnection);
        }

        // Ejecuta un escalar (por ejemplo COUNT(*) o SCOPE_IDENTITY()).
        public object EjecutarEscalar(string sql, Action<SqlParameterCollection>? parametros = null)
        {
            using (SqlConnection cn = AbrirConexion())
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                parametros?.Invoke(cmd.Parameters);
                return cmd.ExecuteScalar();
            }
        }
    }
}
