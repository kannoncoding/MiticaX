//UNED
//Mitica X
//Jorge Arias Melendez
//Tercer cuatrimestre 2025
//Reglas del juego expuestas como protocolo de texto linea-a-linea.

using System;
using Miticax.Datos;
using Miticax.Entidades;

namespace Miticax.Logica
{
    // Expone comandos tipo: STATUS?, JUGADOR|ALTA|..., JUGADOR|?id, CRIATURAS?, CRIATURA?x, 
    // BATALLA|RETO|idJ1|idEq1|idJ2|idEq2, BATALLA|GANAR|idBatalla|1|2
    public class JuegoLogica
    {
        private readonly JuegoDatos datos;

        public JuegoLogica(JuegoDatos d)
        {
            datos = d; // referencia a arreglos base
        }

        // Procesa un comando de juego y devuelve respuesta textual.
        public string ProcesarComandoJuego(string cmd)
        {
            // Busca primer separador para obtener el nombre del comando.
            int p = cmd.IndexOf('|');
            string nombre = p >= 0 ? cmd.Substring(0, p) : cmd;
            string resto = p >= 0 ? cmd.Substring(p + 1) : string.Empty;
            string n = nombre.ToUpperInvariant();

            // ============= STATUS? =============
            if (n == "STATUS?")
                return "OK|SERVIDOR|ACTIVO";

            // ============= CRIATURAS? =============
            if (n == "CRIATURAS?")
                return "OK|CRIATURAS|" + datos.ContarCriaturas();

            // ============= CRIATURA?<id> =============
            if (n == "CRIATURA?")
            {
                if (!int.TryParse(resto.Trim(), out int idc)) return "ERROR|FORMATO_CRIATURA?";
                var c = datos.ObtenerCriaturaPorId(idc);
                if (c == null) return "ERROR|CRIATURA_NO_ENCONTRADA";
                // Devuelve todos tus campos reales
                return "OK|CRIATURA|" + c.IdCriatura + "|" + c.Nombre +
                       "|TIPO=" + c.Tipo +
                       "|NIVEL=" + c.Nivel +
                       "|PODER=" + c.Poder +
                       "|RES=" + c.Resistencia +
                       "|COSTO=" + c.Costo;
            }

            // ============= JUGADOR =============
            // Subcomandos:
            //  JUGADOR|ALTA|<nombre>|<yyyy-MM-dd>|<nivel>
            //  JUGADOR|?id
            if (n == "JUGADOR")
            {
                int p2 = resto.IndexOf('|');
                string sub = p2 >= 0 ? resto.Substring(0, p2) : resto;
                string param = p2 >= 0 ? resto.Substring(p2 + 1) : string.Empty;
                string s = sub.ToUpperInvariant();

                // ----- ALTA -----
                if (s == "ALTA")
                {
                    // Espera 3 parametros: nombre | fechaNac | nivel
                    // Parse manual (sin Split LINQ).
                    string[] trozos = Partir(param, 3);
                    if (trozos == null) return "ERROR|FORMATO_JUGADOR_ALTA";

                    string nomJugador = (trozos[0] ?? "").Trim();
                    string fechaTxt = (trozos[1] ?? "").Trim();
                    string nivelTxt = (trozos[2] ?? "").Trim();

                    if (nomJugador.Length == 0) return "ERROR|NOMBRE_REQUERIDO";
                    if (!DateTime.TryParse(fechaTxt, out DateTime fechaNac)) return "ERROR|FECHA_INVALIDA";
                    if (!int.TryParse(nivelTxt, out int nivel)) nivel = 1;

                    int res = datos.AltaJugador(nomJugador, fechaNac, nivel);
                    if (res == -1) return "ERROR|SIN_ESPACIO_JUGADOR";
                    if (res == -2) return "ERROR|NOMBRE_REQUERIDO";
                    if (res == -3) return "ERROR|EDAD_MINIMA_10";
                    return "OK|JUGADOR|" + res;
                }

                // ----- ?id -----
                if (s.StartsWith("?"))
                {
                    string idtxt = s.Length > 1 ? s.Substring(1) : param;
                    if (!int.TryParse(idtxt, out int idj)) return "ERROR|FORMATO_JUGADOR?";
                    var j = datos.ObtenerJugadorPorId(idj);
                    if (j == null) return "ERROR|JUGADOR_NO_ENCONTRADO";

                    // Devuelve tus campos reales
                    return "OK|JUGADOR|" + j.IdJugador + "|" + j.Nombre +
                           "|FECNAC=" + j.FechaNacimiento.ToString("yyyy-MM-dd") +
                           "|CRISTALES=" + j.Cristales +
                           "|GANADAS=" + j.BatallasGanadas +
                           "|NIVEL=" + j.Nivel;
                }

                return "ERROR|SUBCOMANDO_JUGADOR";
            }

            // ============= BATALLA =============
            // Subcomandos:
            //  BATALLA|RETO|idJ1|idEq1|idJ2|idEq2
            //  BATALLA|GANAR|idBatalla|1|2
            if (n == "BATALLA")
            {
                int p2 = resto.IndexOf('|');
                string sub = p2 >= 0 ? resto.Substring(0, p2) : resto;
                string param = p2 >= 0 ? resto.Substring(p2 + 1) : string.Empty;
                string s = sub.ToUpperInvariant();

                // ----- RETO -----
                if (s == "RETO")
                {
                    int[] vals = PartirEnteros(param, 4);
                    if (vals == null) return "ERROR|FORMATO_BATALLA_RETO";

                    int idJ1 = vals[0], idEq1 = vals[1], idJ2 = vals[2], idEq2 = vals[3];

                    int idBat = datos.AltaBatalla(idJ1, idEq1, idJ2, idEq2, DateTime.Now);
                    if (idBat == -1) return "ERROR|SIN_ESPACIO_BATALLA";
                    if (idBat == -2) return "ERROR|J1_NO_EXISTE";
                    if (idBat == -3) return "ERROR|J2_NO_EXISTE";

                    return "OK|BATALLA|" + idBat;
                }

                // ----- GANAR -----
                if (s == "GANAR")
                {
                    int[] vals = PartirEnteros(param, 2);
                    if (vals == null) return "ERROR|FORMATO_BATALLA_GANAR";

                    int idBat = vals[0];
                    int quien = vals[1]; // 1 => jugador1, 2 => jugador2
                    bool ok = datos.CerrarBatallaConGanador(idBat, quien);
                    if (!ok) return "ERROR|NO_PUDO_CERRAR";
                    return "OK|BATALLA_CERRADA|" + idBat;
                }

                return "ERROR|SUBCOMANDO_BATALLA";
            }

            // Si no coincide, devuelve error generico.
            return "ERROR|COMANDO_NO_SOPORTADO";
        }

        // ===== Utilidades de parseo (sin LINQ, solo arrays) =====

        // Divide en exactamente 'n' piezas (si no, retorna null).
        private string[]? Partir(string texto, int n)
        {
            string[] piezas = new string[n];
            int inicio = 0, cuenta = 0;

            for (int i = 0; i <= texto.Length; i++)
            {
                if (i == texto.Length || texto[i] == '|')
                {
                    if (cuenta >= n) return null;
                    piezas[cuenta++] = texto.Substring(inicio, i - inicio);
                    inicio = i + 1;
                }
            }
            if (cuenta != n) return null;
            return piezas;
        }

        // Igual que Partir pero parseando a int.
        private int[]? PartirEnteros(string texto, int n)
        {
            int[] valores = new int[n];
            int inicio = 0, cuenta = 0;

            for (int i = 0; i <= texto.Length; i++)
            {
                if (i == texto.Length || texto[i] == '|')
                {
                    if (cuenta >= n) return null;
                    string trozo = texto.Substring(inicio, i - inicio);
                    if (!int.TryParse(trozo, out int v)) return null;
                    valores[cuenta++] = v;
                    inicio = i + 1;
                }
            }
            if (cuenta != n) return null;
            return valores;
        }
    }
}
