using System;
using System.Messaging;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using OperacionConciliacion;
using Notificaciones;

namespace ServidorRegistro
{
    class Program
    {
        private static IOperacionConciliacion remoteConciliacion;
        private const string pathMsgQueue = @".\private$\ClientQueue";
        private static MessageQueue colaDeMensajes;
        static void Main(string[] args)
        {
            bool condicion = true;
            TcpChannel chan = new TcpChannel();
            ChannelServices.RegisterChannel(chan, false);
            // Create an instance of the remote object
            remoteConciliacion = (IOperacionConciliacion)Activator.GetObject(
              typeof(IOperacionConciliacion), "tcp://localhost:5000/OperacionConciliacion");
            while (condicion) // x esto saca un punto
                RecibirNotificaciones();
        }

        public static void RecibirNotificaciones()

        {
            if (!MessageQueue.Exists(pathMsgQueue))
                colaDeMensajes = MessageQueue.Create(pathMsgQueue); 
            else
                colaDeMensajes = new MessageQueue(pathMsgQueue);

            using (colaDeMensajes)
            {
                colaDeMensajes.Formatter = new XmlMessageFormatter(new Type[]{
typeof(ObjetoNotificacion) });
                Message msg = colaDeMensajes.Peek();
                Message msg2 = colaDeMensajes.Receive();
                Console.WriteLine(((ObjetoNotificacion)msg.Body).msg);
                SistemaRegistro.GuardarEvento((ObjetoNotificacion)msg.Body);
                // ObjetoNotificacion notificacion = new ObjetoNotificacion(usuario, msg.Body);
                var notificacion = (ObjetoNotificacion)msg.Body;
                remoteConciliacion.GuardarEvento(notificacion);
                remoteConciliacion.getListaRegistro(SistemaRegistro.listaEventos);
            }
        }
    }
}
