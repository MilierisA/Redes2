using Notificaciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServidorConciliacion
{
    public class ServicioConciliacion : IServicioConciliacion
    {

        public List<ObjetoNotificacion> ObtenerEstadisticas(string nombreUsuario)
        {
            var notificaciones = SistemaConciliacion.GetListaEventos();
            var notificacionesDelUsuario = notificaciones.Where(n => n.usuario == nombreUsuario);
            if (notificacionesDelUsuario == null || notificacionesDelUsuario.Count() == 0)
            {
                return new List<ObjetoNotificacion>();
            }
            else
                return notificacionesDelUsuario.ToList();
        }
    }
}
