using Servidor;
using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using DominioCliente;

namespace DominioServidor
{
    public static class ConexionServidor
    {
        public static List<ObjetoConexion> ClientesConectados { get; set; }
        private const string directorioServidor = @"../../../Servidor/directorioServidor";
        private static string Ip { get; set; }
        private static int Puerto { get; set; }

        public static void DesconectarCliente(Socket cli)
        {
            var cliente = ClientesConectados.FirstOrDefault(c => c.socket.RemoteEndPoint == cli.RemoteEndPoint);
            ClientesConectados.Remove(cliente);
            Console.WriteLine("El cliente " + cliente.nombreCliente + " se ha desconectado");
        }
        public static void AgregarCliente(ObjetoConexion cli)
        {
            if (ClientesConectados == null)
                ClientesConectados = new List<ObjetoConexion>();
            ClientesConectados.Add(cli);
        }

        public static void CerrarTodasLasConexiones()
        {
            foreach (var cli in ClientesConectados)
            {
                cli.socket.Shutdown(SocketShutdown.Both);
                cli.socket.Close();
            }
        }

        public static bool YaEstaConectado(string nombreUsuario)
        {
            bool esta = false;
            if (ClientesConectados == null)
                ClientesConectados = new List<ObjetoConexion>();
            lock (ClientesConectados)
            {
                foreach (var conexion in ClientesConectados)
                {
                    if (conexion.nombreCliente == nombreUsuario)
                        esta = true;
                }
            }

            return esta;
        }

        public static void AceptarConexiones(string ip, int puerto)
        {
            Puerto = puerto;
            Ip = ip;
            Console.WriteLine("Esperando a nuevos clientes...");

            Socket socketServer = new Socket(AddressFamily.InterNetwork,
                                        SocketType.Stream,
                                        ProtocolType.Tcp);
            IPEndPoint localEp = new IPEndPoint(IPAddress.Parse(ip), puerto);
            try
            {
                socketServer.Bind(localEp);
                socketServer.Listen(100);
                while (true)
                {
                    Socket client = socketServer.Accept();
                    Thread clientHandler = new Thread(() => HandleClient(client));
                    clientHandler.Start();
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("Error de conexion. Revise configuracion de ip y puerto");
                Console.ReadLine();
            }

        }

        static void HandleClient(Socket client)
        {
            bool reciboNombre = true;
            bool connected = true;
            while (connected)
            {
                try
                {
                    int i;
                    byte[] data = new byte[9999];
                    byte[] loginResponse = new byte[1];
                    i = client.Receive(data);
                    if (reciboNombre)
                    {
                        byte[] respuestaLogin = SistemaServidor.IniciarSesion(data);
                        string respuestaString = Encoding.UTF8.GetString(SistemaServidor.getDatos(respuestaLogin)).TrimEnd().Replace("\0", string.Empty);
                        bool usuarioValido = Boolean.Parse(respuestaString.Split('#')[1]);
                        string nombreUsuario = respuestaString.Split('#')[0];
                        if (usuarioValido)
                        {
                            ObjetoConexion con = new ObjetoConexion(client, nombreUsuario);
                            AgregarCliente(con);
                            reciboNombre = false;
                            client.Send(respuestaLogin);
                            Console.WriteLine("Conectado al cliente " + nombreUsuario);
                        }
                        else
                        {
                            client.Send(respuestaLogin);
                            Console.WriteLine("Cliente " + nombreUsuario + " inválido.");
                        }
                    }
                    else
                    {
                        string comando = SistemaServidor.getComando(data);
                        byte[] datos = SistemaServidor.getDatos(data);
                        byte[] respuestaServidor = new byte[9999];
                        if (comando == "2")
                        {
                            respuestaServidor = SistemaServidor.ListarArchivos();
                            client.Send(respuestaServidor);
                        }
                        if (comando == "3")
                        {
                            respuestaServidor = SistemaServidor.SubirArchivo(Encoding.UTF8.GetString(datos).TrimEnd());
                            int cantPaquetes = Int16.Parse(Encoding.UTF8.GetString(datos).TrimEnd().Replace("\0", string.Empty).Split('#')[1]);
                            string nombreArchivoASubir = Encoding.UTF8.GetString(datos).TrimEnd().Replace("\0", string.Empty).Split('#')[0];
                            client.Send(respuestaServidor);
                            int paqueteActual = 1;
                            int offset = 0;
                            FileStream fileStream = new FileStream(directorioServidor + "\\" + nombreArchivoASubir, FileMode.Create, FileAccess.Write);
                            while (paqueteActual <= cantPaquetes)
                            {
                                i = client.Receive(data);
                                Console.WriteLine("Recibiendo " + nombreArchivoASubir + ". Paquete " + paqueteActual + " de " + cantPaquetes);
                                int largoPaquete = SistemaServidor.darLargo(data) - 9;
                                byte[] PaqueteArchivo = SistemaServidor.getDatos(data);
                                if (paqueteActual == cantPaquetes)
                                    PaqueteArchivo = PaqueteArchivo.Take(largoPaquete).ToArray();
                                SistemaServidor.GuardarPaqueteDeArchivo(fileStream, PaqueteArchivo, nombreArchivoASubir);
                                paqueteActual++;
                                respuestaServidor = SistemaServidor.SubirPaqueteDeArchivo(data);
                                client.Send(respuestaServidor);
                            }
                            fileStream.Close();

                        }

                        if (comando == "4")
                        {
                            respuestaServidor = SistemaServidor.DescargarArchivo(datos);
                            client.Send(respuestaServidor);
                            Protocolo.Protocolo protocoloRespuesta = Protocolo.Protocolo.BytesAProtocolo(respuestaServidor);
                            byte[] bytesRespuesta = protocoloRespuesta.darBytes();
                            string stringRespuesta = Encoding.UTF8.GetString(bytesRespuesta);
                            int cantPaquetesAMandar = Int16.Parse(stringRespuesta.Split('#')[1]);
                            string nombreArchivo = Encoding.UTF8.GetString(bytesRespuesta).Split('#')[0];
                            if (cantPaquetesAMandar == 0)
                            { //No se encontro el archivo
                                Console.WriteLine("Error. No se encontro el archivo " + nombreArchivo + " en el directorio del cliente");
                            }
                            else
                            {
                                string rutaArchivo = directorioServidor + "\\" + nombreArchivo;
                                FileStream fs = new FileStream(rutaArchivo, FileMode.Open);
                                int paqueteActual = 1;
                                while (paqueteActual <= cantPaquetesAMandar)
                                {
                                    byte[] req = new byte[256];
                                    i = client.Receive(req);
                                    int bytesPorDefecto = 9990;
                                    if (paqueteActual == cantPaquetesAMandar)
                                    {
                                        var largo = fs.Length;
                                        bytesPorDefecto = (int)largo - (9990 * (paqueteActual - 1));
                                    }
                                    byte[] dataParaEnviar = new byte[bytesPorDefecto];
                                    int iDataRead = fs.Read(dataParaEnviar, 0, bytesPorDefecto);
                                    byte[] dataProtocolo = SistemaServidor.DescargarPaqueteDeArchivo(dataParaEnviar);
                                    client.Send(dataProtocolo);
                                    Console.WriteLine("Descargando " + nombreArchivo + ". Paquete " + paqueteActual + " de " + cantPaquetesAMandar);
                                    paqueteActual++;

                                }
                                fs.Close();
                            }
                        }
                        if (comando == "5")
                        {
                            string[] datosServidor = Encoding.UTF8.GetString(datos).TrimEnd().Replace("\0", string.Empty).Split('#');
                            string ipServidor = datosServidor[0];
                            string puertoServidor = datosServidor[1];
                            IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Parse(Ip), Puerto + 2);
                            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(ipServidor), Int32.Parse(puertoServidor));
                            Socket socketSinc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                            socketSinc.Bind(clientEndPoint);
                            try
                            {
                                socketSinc.Connect(serverEndPoint);
                                bool loggedIn = false;
                                byte[] dataLog = SistemaCliente.ProcesarPedido("1", "servidor");
                                socketSinc.Send(dataLog);
                                byte[] loginRes = new byte[256];
                                int response = socketSinc.Receive(loginRes);
                                loggedIn = Boolean.Parse(SistemaCliente.ProcesarRespuesta(loginRes).Split('#')[1]);
                                if (!loggedIn)
                                {
                                    Console.WriteLine("Nombre de cliente inválido!");
                                }
                                else
                                {
                                    byte[] serverRequest = SistemaCliente.ProcesarPedido("2");
                                    socketSinc.Send(serverRequest);
                                    byte[] serverResponse = new byte[256];
                                    int k = socketSinc.Receive(serverResponse);
                                    string listaArchivos2 = SistemaCliente.ProcesarRespuesta(serverResponse);
                                    /*listaArchivos2 = listaArchivos2.Replace("#", "\n");
                                    Console.WriteLine("Lista de archivos");
                                    Console.WriteLine(listaArchivos2);*/
                                    string[] archivosServidor2 = listaArchivos2.TrimEnd().Replace("\0", string.Empty).Split('#');
                                    byte[] listaArchivosByte = SistemaServidor.ListarArchivos();
                                    string listaArchivos1 = SistemaCliente.ProcesarRespuesta(listaArchivosByte);
                                    string[] archivosServidor1 = listaArchivos1.Split('#');
                                    string[] archivosFaltan = new string[archivosServidor2.Length];
                                    int h = 0;
                                    foreach (string archivo in archivosServidor2)
                                    {
                                        if (!archivosServidor1.Contains(archivo))
                                        {
                                            archivosFaltan[h] = archivo;
                                            h++;
                                        }
                                    }
                                    foreach (string archivo in archivosFaltan)
                                    {
                                        if (archivo != null)
                                        {
                                            string nombreArchivo = archivo.Substring(37);
                                            byte[] dataRequest = SistemaCliente.ProcesarPedido("4", nombreArchivo);
                                            socketSinc.Send(dataRequest);
                                            byte[] dataResponse = new byte[9999];
                                            int z = socketSinc.Receive(dataResponse);
                                            dataResponse = dataResponse.Take(z).ToArray();
                                            int cantPaquetes = Int16.Parse(SistemaCliente.ProcesarRespuesta(dataResponse).Split('#')[1]);
                                            int paqueteActual = 1;
                                            if (cantPaquetes == 0)
                                            {
                                                Console.WriteLine("No es posible descargar el archivo indicado.");
                                            }
                                            else
                                            {
                                                FileStream fileStream = new FileStream(directorioServidor + "\\" + nombreArchivo, FileMode.Create, FileAccess.Write);
                                                while (paqueteActual <= cantPaquetes)
                                                {
                                                    byte[] requestPaquete = SistemaCliente.ProcesarPedido("8", paqueteActual.ToString());
                                                    socketSinc.Send(requestPaquete);
                                                    byte[] responsePaquete = new byte[9999];
                                                    int j = socketSinc.Receive(responsePaquete);
                                                    Console.WriteLine("Recibiendo " + nombreArchivo + ". Paquete " + paqueteActual + " de " + cantPaquetes);
                                                    int largoPaquete = SistemaCliente.darLargo(responsePaquete) - 9;
                                                    byte[] PaqueteArchivo = SistemaCliente.getDatos(responsePaquete);
                                                    if (paqueteActual == cantPaquetes)
                                                        PaqueteArchivo = PaqueteArchivo.Take(largoPaquete).ToArray();
                                                    SistemaCliente.GuardarPaqueteDeArchivo(fileStream, PaqueteArchivo, nombreArchivo);
                                                    paqueteActual++;
                                                }
                                                fileStream.Close();
                                            }
                                        }
                                    }

                                    byte[] respuestaSinc = SistemaServidor.NotificarSincronizacion();
                                    client.Send(respuestaSinc);
                                }


                            }

                            catch (SocketException ex)
                            {
                                socketSinc.Shutdown(SocketShutdown.Both);
                                socketSinc.Close();
                                Console.WriteLine("El servidor cerró la conexión");
                                Console.ReadLine();
                            }
                            //respuestaServidor = SistemaServidor.SincronizarServidor(Encoding.UTF8.GetString(datos).TrimEnd().Replace("\0", string.Empty));
                        }
                        if (comando == "6")
                        {
                            DesconectarCliente(client);
                            connected = false;
                        }
                        /*if (comando == "8")
                        {
                            respuestaServidor = SistemaServidor.DescargarPaqueteDeArchivo(datos);
                        }
                        if (comando == "7")
                        {
                            respuestaServidor = SistemaServidor.SubirPaqueteDeArchivo(datos);
                        }*/
                        //byte[] respuestaServidor = SistemaServidor.ProcesarPedido(data);
                        //aca separo segun el comando que haya llegado
                        //client.Send(respuestaServidor);
                        //Console.WriteLine(Encoding.UTF8.GetString(data).TrimEnd());
                    }

                }
                catch (SocketException ex)
                {
                    Console.WriteLine("El cliente cerró la conexión");
                    connected = false;
                }
            }
            client.Shutdown(SocketShutdown.Both);
            client.Close();
        }
    }
}
