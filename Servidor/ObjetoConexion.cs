using System.Net.Sockets;

namespace DominioServidor
{
    public class ObjetoConexion
    {
        public string nombreCliente { get; set; }
        public Socket socket{ get; set; }

        public ObjetoConexion(Socket sock, string cli)
        {
            nombreCliente = cli;
            socket = sock;
        }
    }
}