using System;
using OperacionConciliacion;
using Notificaciones;
using System.Collections.Generic;

namespace ServidorConciliacion
{
    class OperacionConciliacion : MarshalByRefObject, IOperacionConciliacion
    {
        public void GuardarEvento (ObjetoNotificacion notificacion)
        {
            Console.WriteLine(notificacion.msg);
            SistemaConciliacion.GetListaEventos().Add(notificacion);
        }

        public void getListaRegistro(List<ObjetoNotificacion> lista)
        {
            SistemaConciliacion.listaRegistroActividad = lista;
        }
    }
}
