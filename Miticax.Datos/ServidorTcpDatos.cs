//UNED
//Mitica X
//Jorge Arias Melendez
//Tercer cuatrimestre 2025
//Capa de datos: manejo de arreglos/slots para conexiones TCP, sin colecciones.

using System;
using System.Net.Sockets;
using Miticax.Entidades;

namespace Miticax.Datos
{
    // Clase de acceso a datos: solo arreglos y operaciones CRUD de bajo nivel sobre ellos.
    public class ServidorTcpDatos
    {
        // Tamano maximo de clientes simultaneos (arreglo fijo).
        private const int MaxClientes = 20;

        // Arreglo de entidades (metadatos) para mostrar en UI o consultar.
        private readonly ClienteConexionEntidad[] clientesMeta;

        // Arreglo paralelo para objetos TcpClient (I/O), separado de la entidad.
        private readonly TcpClient[] clientesIo;

        // Arreglo paralelo para hilos por cliente.
        private readonly System.Threading.Thread[] hilosCliente;

        // Bandera de ocupacion por slot.
        private readonly bool[] ocupado;

        // Constructor: inicializa arreglos.
        public ServidorTcpDatos()
        {
            clientesMeta = new ClienteConexionEntidad[MaxClientes];
            clientesIo = new TcpClient[MaxClientes];
            hilosCliente = new System.Threading.Thread[MaxClientes];
            ocupado = new bool[MaxClientes];

            // Inicializa las entidades para evitar nulls.
            for (int i = 0; i < MaxClientes; i++)
            {
                clientesMeta[i] = new ClienteConexionEntidad { Indice = i };
                ocupado[i] = false;
            }
        }

        // Devuelve el arreglo de metadatos (solo lectura por referencia).
        public ClienteConexionEntidad[] ObtenerClientesMeta() => clientesMeta;

        // Intenta reservar un slot libre; retorna indice o -1 si no hay.
        public int ReservarSlot()
        {
            // Recorre el arreglo buscando el primer slot libre.
            for (int i = 0; i < ocupado.Length; i++)
            {
                if (!ocupado[i])
                {
                    ocupado[i] = true; // Marca como ocupado.
                    return i;          // Retorna el indice reservado.
                }
            }
            return -1; // No hay espacio.
        }

        // Libera un slot: limpia referencias y marca disponible.
        public void LiberarSlot(int indice)
        {
            if (indice < 0 || indice >= ocupado.Length) return;

            try
            {
                // Intenta cerrar TcpClient si aun sigue abierto.
                if (clientesIo[indice] != null)
                {
                    try { clientesIo[indice].Close(); } catch { /* ignora */ }
                }
            }
            finally
            {
                // Limpia metadatos y marca estado.
                clientesIo[indice] = null;
                hilosCliente[indice] = null;
                clientesMeta[indice].Estado = "Cerrado";
                clientesMeta[indice].HoraFin = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                ocupado[indice] = false;
            }
        }

        // Guarda el TcpClient en el slot.
        public void EstablecerClienteIo(int indice, TcpClient cliente)
        {
            clientesIo[indice] = cliente; // Referencia al socket del cliente.
        }

        // Lee el TcpClient del slot (para uso interno de logica).
        public TcpClient? ObtenerClienteIo(int indice) => clientesIo[indice];

        // Asocia el hilo de atencion al slot (para control y cierre ordenado).
        public void EstablecerHiloCliente(int indice, System.Threading.Thread hilo)
        {
            hilosCliente[indice] = hilo;
        }

        // Actualiza metadatos basicos al conectar.
        public void MarcarConectado(int indice, string remoto)
        {
            var meta = clientesMeta[indice];
            meta.Remoto = remoto;
            meta.Estado = "Conectado";
            meta.HoraInicio = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            meta.HoraFin = string.Empty;
        }
    }
}

