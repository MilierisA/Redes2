﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ClienteInspeccion.ServicioConciliacion {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="ObjetoNotificacion", Namespace="http://schemas.datacontract.org/2004/07/Notificaciones")]
    [System.SerializableAttribute()]
    public partial class ObjetoNotificacion : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int accionField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string msgField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string usuarioField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int accion {
            get {
                return this.accionField;
            }
            set {
                if ((this.accionField.Equals(value) != true)) {
                    this.accionField = value;
                    this.RaisePropertyChanged("accion");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string msg {
            get {
                return this.msgField;
            }
            set {
                if ((object.ReferenceEquals(this.msgField, value) != true)) {
                    this.msgField = value;
                    this.RaisePropertyChanged("msg");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string usuario {
            get {
                return this.usuarioField;
            }
            set {
                if ((object.ReferenceEquals(this.usuarioField, value) != true)) {
                    this.usuarioField = value;
                    this.RaisePropertyChanged("usuario");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ServicioConciliacion.IServicioConciliacion")]
    public interface IServicioConciliacion {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServicioConciliacion/ObtenerEstadisticas", ReplyAction="http://tempuri.org/IServicioConciliacion/ObtenerEstadisticasResponse")]
        ClienteInspeccion.ServicioConciliacion.ObjetoNotificacion[] ObtenerEstadisticas(string nombreUsuario);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServicioConciliacion/ObtenerEstadisticas", ReplyAction="http://tempuri.org/IServicioConciliacion/ObtenerEstadisticasResponse")]
        System.Threading.Tasks.Task<ClienteInspeccion.ServicioConciliacion.ObjetoNotificacion[]> ObtenerEstadisticasAsync(string nombreUsuario);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IServicioConciliacionChannel : ClienteInspeccion.ServicioConciliacion.IServicioConciliacion, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ServicioConciliacionClient : System.ServiceModel.ClientBase<ClienteInspeccion.ServicioConciliacion.IServicioConciliacion>, ClienteInspeccion.ServicioConciliacion.IServicioConciliacion {
        
        public ServicioConciliacionClient() {
        }
        
        public ServicioConciliacionClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ServicioConciliacionClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ServicioConciliacionClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ServicioConciliacionClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public ClienteInspeccion.ServicioConciliacion.ObjetoNotificacion[] ObtenerEstadisticas(string nombreUsuario) {
            return base.Channel.ObtenerEstadisticas(nombreUsuario);
        }
        
        public System.Threading.Tasks.Task<ClienteInspeccion.ServicioConciliacion.ObjetoNotificacion[]> ObtenerEstadisticasAsync(string nombreUsuario) {
            return base.Channel.ObtenerEstadisticasAsync(nombreUsuario);
        }
    }
}