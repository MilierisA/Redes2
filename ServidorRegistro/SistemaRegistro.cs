using Notificaciones;
using System.Collections.Generic;

namespace ServidorRegistro
{
    public class SistemaRegistro
    {
        public static List<ObjetoNotificacion> listaEventos { get; set; }

        public SistemaRegistro()
        {
            listaEventos = new List<ObjetoNotificacion>();
        }

        public static List<ObjetoNotificacion> GetListaEventos()
        {
            if (listaEventos == null)
                listaEventos = new List<ObjetoNotificacion>();

            return listaEventos;
        }

        public static void GuardarEvento (ObjetoNotificacion evento)
        {
            if (listaEventos == null)
                listaEventos = new List<ObjetoNotificacion>();
            listaEventos.Add(evento);
        }
    }
}
