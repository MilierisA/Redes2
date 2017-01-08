﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ClienteInspeccion.ServicioArchivos {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ServicioArchivos.IServicioArchivos")]
    public interface IServicioArchivos {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServicioArchivos/ListarArchivos", ReplyAction="http://tempuri.org/IServicioArchivos/ListarArchivosResponse")]
        string[] ListarArchivos();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServicioArchivos/ListarArchivos", ReplyAction="http://tempuri.org/IServicioArchivos/ListarArchivosResponse")]
        System.Threading.Tasks.Task<string[]> ListarArchivosAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServicioArchivos/EliminarArchivo", ReplyAction="http://tempuri.org/IServicioArchivos/EliminarArchivoResponse")]
        bool EliminarArchivo(string nombreArchivo);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServicioArchivos/EliminarArchivo", ReplyAction="http://tempuri.org/IServicioArchivos/EliminarArchivoResponse")]
        System.Threading.Tasks.Task<bool> EliminarArchivoAsync(string nombreArchivo);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IServicioArchivosChannel : ClienteInspeccion.ServicioArchivos.IServicioArchivos, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ServicioArchivosClient : System.ServiceModel.ClientBase<ClienteInspeccion.ServicioArchivos.IServicioArchivos>, ClienteInspeccion.ServicioArchivos.IServicioArchivos {
        
        public ServicioArchivosClient() {
        }
        
        public ServicioArchivosClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ServicioArchivosClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ServicioArchivosClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ServicioArchivosClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public string[] ListarArchivos() {
            return base.Channel.ListarArchivos();
        }
        
        public System.Threading.Tasks.Task<string[]> ListarArchivosAsync() {
            return base.Channel.ListarArchivosAsync();
        }
        
        public bool EliminarArchivo(string nombreArchivo) {
            return base.Channel.EliminarArchivo(nombreArchivo);
        }
        
        public System.Threading.Tasks.Task<bool> EliminarArchivoAsync(string nombreArchivo) {
            return base.Channel.EliminarArchivoAsync(nombreArchivo);
        }
    }
}
