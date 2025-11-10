//UNED
//Mitica X
//Jorge Arias Melendez
//Tercer cuatrimestre 2025
//Logica del servidor TCP: listener, aceptacion, hilos por cliente y protocolo de texto.

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Miticax.Datos;

namespace Miticax.Logica
{
    // Delegado para notificar bitacora a la UI.
    public delegate void BitacoraHandler(string linea);

    // Clase de logica que opera sobre la capa de datos y expone eventos a la UI.
    public class ServidorTcpLogica
    {
        // Evento para escribir lineas en la bitacora (suscrito por la UI).
        public event BitacoraHandler? OnBitacora;

        // Puerto a escuchar (por defecto 14100).
        private int puerto = 14100;

        // Listener TCP (sockets de servidor).
        private TcpListener? listener;

        // Bandera de servicio activo (control del bucle de aceptacion).
        private volatile bool activo = false;

        // Hilo principal de aceptacion.
        private Thread? hiloAceptador;

        // Capa de datos para manejar arreglos/slots.
        private readonly ServidorTcpDatos datos;

        // Constructor: recibe instancia de datos (o crea una).
        public ServidorTcpLogica(ServidorTcpDatos? datosOpcional = null)
        {
            datos = datosOpcional ?? new ServidorTcpDatos();
        }

        // Inicia el servidor en el puerto indicado.
        public void IniciarServidor(int puertoEscucha = 14100)
        {
            // Guarda puerto y crea listener.
            puerto = puertoEscucha;
            listener = new TcpListener(IPAddress.Any, puerto);
            listener.Start(); // Comienza a escuchar conexiones.
            activo = true;    // Habilita el bucle de aceptacion.

            // Lanza el hilo aceptador.
            hiloAceptador = new Thread(BucleAceptar) { IsBackground = true };
            hiloAceptador.Start();

            // Notifica a la bitacora.
            EscribirBitacora($"Servidor iniciado en puerto {puerto}.");
        }

        // Detiene el servidor y cierra conexiones.
        public void DetenerServidor()
        {
            // Desactiva bandera para terminar bucle.
            activo = false;

            try
            {
                // Cierra listener si existe.
                listener?.Stop();
            }
            catch { /* ignora */ }

            // Notifica cierre.
            EscribirBitacora("Servidor detenido.");
        }

        // Devuelve el arreglo de metadatos para la UI (tabla, si lo deseas).
        public Miticax.Entidades.ClienteConexionEntidad[] ObtenerEstadoConexiones()
        {
            return datos.ObtenerClientesMeta();
        }

        // Hilo principal: acepta clientes mientras 'activo' sea true.
        private void BucleAceptar()
        {
            while (activo)
            {
                try
                {
                    // Bloquea hasta que llegue un cliente o lance excepcion al detener.
                    var cliente = listener!.AcceptTcpClient();

                    // Intenta reservar un slot en los arreglos.
                    int idx = datos.ReservarSlot();
                    if (idx == -1)
                    {
                        // Si no hay espacio, rechaza amablemente.
                        EscribirBitacora("Conexion rechazada: no hay slots disponibles.");
                        try
                        {
                            using var ns = cliente.GetStream();
                            using var w = new StreamWriter(ns, Encoding.UTF8) { AutoFlush = true };
                            w.WriteLine("ERROR|Servidor ocupado");
                        }
                        catch { /* ignora */ }
                        finally
                        {
                            try { cliente.Close(); } catch { /* ignora */ }
                        }
                        continue; // Busca siguiente.
                    }

                    // Registra al cliente en datos y metadatos.
                    string remoto = cliente.Client.RemoteEndPoint?.ToString() ?? "desconocido";
                    datos.EstablecerClienteIo(idx, cliente);
                    datos.MarcarConectado(idx, remoto);
                    EscribirBitacora($"Conexion aceptada [slot {idx}] desde {remoto}.");

                    // Lanza el hilo por cliente para atender protocolo.
                    var hilo = new Thread(() => AtenderCliente(idx)) { IsBackground = true };
                    datos.EstablecerHiloCliente(idx, hilo);
                    hilo.Start();
                }
                catch (SocketException)
                {
                    // Ocurre cuando se hace Stop() al listener; salimos si no esta activo.
                    if (!activo) break;
                    // Si aun activo, reporta y continua.
                    EscribirBitacora("SocketException en aceptacion (continuando)...");
                }
                catch (Exception ex)
                {
                    // Cualquier otro error: log y continua.
                    EscribirBitacora($"Error en aceptacion: {ex.Message}");
                }
            }
        }

        // Atiende el protocolo de texto para un cliente especifico (slot).
        private void AtenderCliente(int indice)
        {
            TcpClient? cli = null;
            try
            {
                // Obtiene la referencia al TcpClient desde datos.
                cli = datos.ObtenerClienteIo(indice);
                if (cli == null) return; // Slot invalido (seguridad).

                // Configura stream y lectores/escritores UTF-8.
                using var ns = cli.GetStream();
                using var lector = new StreamReader(ns, Encoding.UTF8, false, 1024, leaveOpen: false);
                using var escritor = new StreamWriter(ns, Encoding.UTF8) { AutoFlush = true };

                // Mensaje de bienvenida (opcional).
                escritor.WriteLine("OK|Servidor Miticax listo");

                // Bucle de lectura linea-a-linea hasta BYE o cierre.
                string? linea;
                while ((linea = lector.ReadLine()) != null)
                {
                    // Normaliza quitando espacios laterales.
                    var cmd = linea.Trim();

                    // Si la linea esta vacia, ignora.
                    if (cmd.Length == 0) continue;

                    // Procesa el comando y obtiene respuesta.
                    var respuesta = ProcesarComando(cmd);

                    // Escribe en la bitacora lo recibido.
                    EscribirBitacora($"[slot {indice}] RX: {cmd}");

                    // Si el comando indica cierre, responde y sale.
                    if (respuesta == "__CERRAR__")
                    {
                        escritor.WriteLine("BYE");
                        EscribirBitacora($"[slot {indice}] Conexion cerrada por BYE.");
                        break;
                    }

                    // Envia la respuesta normal al cliente.
                    escritor.WriteLine(respuesta);

                    // Bitacora de TX.
                    EscribirBitacora($"[slot {indice}] TX: {respuesta}");
                }
            }
            catch (IOException)
            {
                // Cierre normal por desconexion de cliente.
                EscribirBitacora($"[slot {indice}] Cliente desconectado.");
            }
            catch (Exception ex)
            {
                // Error inesperado.
                EscribirBitacora($"[slot {indice}] Error: {ex.Message}");
            }
            finally
            {
                // Libera recursos del slot.
                datos.LiberarSlot(indice);
            }
        }

        // Implementacion del protocolo de texto (sin JSON, ni colecciones).
        private string ProcesarComando(string cmd)
        {
            // Busca el primer separador '|', si existe.
            int p = cmd.IndexOf('|');

            // Extrae nombre del comando y parametro (si lo hay).
            string nombre = p >= 0 ? cmd.Substring(0, p) : cmd;
            string param = p >= 0 ? cmd.Substring(p + 1) : string.Empty;

            // Normaliza a mayusculas para comparar comandos.
            string n = nombre.ToUpperInvariant();

            // Comando: PING -> PONG
            if (n == "PING") return "PONG";

            // Comando: ECHO|<texto> -> <texto>
            if (n == "ECHO") return param;

            // Comando: HELLO|<nombre> -> OK|Bienvenido <nombre>
            if (n == "HELLO")
            {
                var texto = string.IsNullOrWhiteSpace(param) ? "Invitado" : param.Trim();
                return "OK|Bienvenido " + texto;
            }

            // Comando: BYE -> cerrar
            if (n == "BYE") return "__CERRAR__";

            // Comando no reconocido.
            return "ERROR|Comando no reconocido";
        }

        // Metodo auxiliar para escribir en la bitacora (si hay suscriptores).
        private void EscribirBitacora(string linea)
        {
            // Invoca el evento si hay escuchas.
            OnBitacora?.Invoke($"[{DateTime.Now:HH:mm:ss}] {linea}");
        }
    }
}
