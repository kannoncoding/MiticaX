//UNED
//Mitica X
//Jorge Arias Melendez
//Tercer cuatrimestre 2025
//Cliente TCP de bajo nivel: conectar, enviar y recibir lineas; expone eventos para la UI.

using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Miticax.Logica
{
    // Clase que maneja toda la logica de conexion TCP del lado cliente.
    // No toca UI: expone eventos para que la capa de presentacion se suscriba.
    public class ClienteTcpLogica
    {
        // Evento que notifica cualquier cambio de estado (conectado, desconectado, error, etc.)
        public event Action<string>? EventoEstado;

        // Evento que notifica cada linea de texto recibida desde el servidor.
        public event Action<string>? EventoLineaRecibida;

        private TcpClient? cliente;               // Socket cliente TCP.
        private NetworkStream? flujo;             // Flujo de red asociado.
        private StreamReader? lector;             // Lector de lineas (texto).
        private StreamWriter? escritor;           // Escritor de lineas (texto).
        private Thread? hiloLectura;              // Hilo dedicado para lectura bloqueante.
        private volatile bool seguirLeyendo;      // Bandera para controlar ciclo del hilo.

        // Metodo: Conectar al servidor con host y puerto.
        public void Conectar(string host, int puerto, int timeoutMs = 5000)
        {
            // Si ya esta conectado, primero cerramos.
            Desconectar();

            try
            {
                // Crear instancia de TcpClient y conectar con timeout simple.
                cliente = new TcpClient();
                IAsyncResult ar = cliente.BeginConnect(host, puerto, null, null);
                bool ok = ar.AsyncWaitHandle.WaitOne(timeoutMs); // espera bloqueante
                if (!ok)
                {
                    cliente.Close();
                    cliente = null;
                    LanzarEstado("No se pudo conectar: tiempo de espera agotado.");
                    return;
                }
                cliente.EndConnect(ar);

                // Obtener el flujo de red y envolver en Reader/Writer para texto por lineas.
                flujo = cliente.GetStream();
                lector = new StreamReader(flujo, Encoding.UTF8, leaveOpen: true);
                escritor = new StreamWriter(flujo, Encoding.UTF8, bufferSize: 1024, leaveOpen: true);
                escritor.AutoFlush = true; // enviamos de inmediato cada linea

                LanzarEstado($"Conectado a {host}:{puerto}");

                // Iniciar el hilo de lectura continua (bloqueante).
                seguirLeyendo = true;
                hiloLectura = new Thread(HiloLeerLoop);
                hiloLectura.IsBackground = true; // no bloquea cierre de la app
                hiloLectura.Start();
            }
            catch (Exception ex)
            {
                LanzarEstado("Error al conectar: " + ex.Message);
                Desconectar(); // limpia cualquier residuo
            }
        }

        // Metodo: Enviar una linea de texto al servidor (agrega '\n' automaticamente).
        public void EnviarLinea(string linea)
        {
            try
            {
                if (escritor == null)
                {
                    LanzarEstado("No se puede enviar: no hay conexion.");
                    return;
                }
                // Envia la linea tal cual; el servidor debe esperar protocolo por lineas.
                escritor.WriteLine(linea);
                // Informar en estado para bitacora local (opcional).
                LanzarEstado(">> " + linea);
            }
            catch (Exception ex)
            {
                LanzarEstado("Error al enviar: " + ex.Message);
            }
        }

        // Metodo conveniente: envia un comando y, si corresponde, parametros.
        // Ejemplo de uso: EnviarComando("PING");  EnviarComando("LOGIN","usuario","clave");
        public void EnviarComando(string comando, params string[] parametros)
        {
            // Armamos un protocolo de texto simple: COMANDO|param1|param2|...
            // Mantiene bajo acoplamiento con la capa servidor.
            string linea = comando;
            if (parametros != null && parametros.Length > 0)
            {
                for (int i = 0; i < parametros.Length; i++)
                {
                    linea += "|" + parametros[i];
                }
            }
            EnviarLinea(linea);
        }

        // Metodo: Desconectar limpiamente.
        public void Desconectar()
        {
            try
            {
                // Parar hilo de lectura si esta en ejecucion.
                seguirLeyendo = false;

                // Cerrar recursos en orden inverso.
                try { lector?.Close(); } catch { }
                try { escritor?.Close(); } catch { }
                try { flujo?.Close(); } catch { }
                try { cliente?.Close(); } catch { }

                lector = null;
                escritor = null;
                flujo = null;
                cliente = null;

                LanzarEstado("Conexion cerrada.");
            }
            catch (Exception ex)
            {
                LanzarEstado("Error al desconectar: " + ex.Message);
            }
        }

        // Bucle del hilo lector: lee lineas bloqueantes hasta que se indique parar o se cierre la conexion.
        private void HiloLeerLoop()
        {
            try
            {
                while (seguirLeyendo && lector != null)
                {
                    // ReadLine() es bloqueante; si el servidor cierra, regresara null.
                    string? linea = lector.ReadLine();
                    if (linea == null)
                    {
                        // Conexion cerrada por el servidor.
                        LanzarEstado("El servidor cerro la conexion.");
                        break;
                    }

                    // Emitir evento hacia la UI o la capa que procese las respuestas.
                    LanzarLinea(linea);
                }
            }
            catch (IOException)
            {
                // IOException tipico cuando el socket se cierra abruptamente.
                LanzarEstado("Conexion interrumpida (E/S).");
            }
            catch (ObjectDisposedException)
            {
                // Ocurre si cerramos mientras ReadLine esta bloqueando.
                LanzarEstado("Conexion finalizada.");
            }
            catch (Exception ex)
            {
                // Cualquier otra excepcion durante la lectura.
                LanzarEstado("Error en lectura: " + ex.Message);
            }
            finally
            {
                // Asegurar que quedemos consistentes en desconexion.
                Desconectar();
            }
        }

        // Helpers para disparar eventos de forma segura.
        private void LanzarEstado(string msg)
        {
            var h = EventoEstado;
            if (h != null) h(msg);
        }

        private void LanzarLinea(string linea)
        {
            var h = EventoLineaRecibida;
            if (h != null) h(linea);
        }
    }
}
