using Notificaciones;
using System.Collections.Generic;

namespace OperacionConciliacion
{
    public interface IOperacionConciliacion
    {
        void GuardarEvento(ObjetoNotificacion msg);
        void getListaRegistro(List<ObjetoNotificacion> lista);

    }
}
