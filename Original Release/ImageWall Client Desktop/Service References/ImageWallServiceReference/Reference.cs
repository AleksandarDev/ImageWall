﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17929
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ImageWallClientDesktop.ImageWallServiceReference {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="ImageDetails", Namespace="http://schemas.datacontract.org/2004/07/ImageWallService")]
    [System.SerializableAttribute()]
    public partial class ImageDetails : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.DateTime CreatedField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ExtensionField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string HashField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private double LatitudeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private double LongitudeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private long SizeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string[] TagsField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string UrlField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string UrlThumbnailField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string UserIdField;
        
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
        public System.DateTime Created {
            get {
                return this.CreatedField;
            }
            set {
                if ((this.CreatedField.Equals(value) != true)) {
                    this.CreatedField = value;
                    this.RaisePropertyChanged("Created");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Extension {
            get {
                return this.ExtensionField;
            }
            set {
                if ((object.ReferenceEquals(this.ExtensionField, value) != true)) {
                    this.ExtensionField = value;
                    this.RaisePropertyChanged("Extension");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Hash {
            get {
                return this.HashField;
            }
            set {
                if ((object.ReferenceEquals(this.HashField, value) != true)) {
                    this.HashField = value;
                    this.RaisePropertyChanged("Hash");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public double Latitude {
            get {
                return this.LatitudeField;
            }
            set {
                if ((this.LatitudeField.Equals(value) != true)) {
                    this.LatitudeField = value;
                    this.RaisePropertyChanged("Latitude");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public double Longitude {
            get {
                return this.LongitudeField;
            }
            set {
                if ((this.LongitudeField.Equals(value) != true)) {
                    this.LongitudeField = value;
                    this.RaisePropertyChanged("Longitude");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Name {
            get {
                return this.NameField;
            }
            set {
                if ((object.ReferenceEquals(this.NameField, value) != true)) {
                    this.NameField = value;
                    this.RaisePropertyChanged("Name");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public long Size {
            get {
                return this.SizeField;
            }
            set {
                if ((this.SizeField.Equals(value) != true)) {
                    this.SizeField = value;
                    this.RaisePropertyChanged("Size");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string[] Tags {
            get {
                return this.TagsField;
            }
            set {
                if ((object.ReferenceEquals(this.TagsField, value) != true)) {
                    this.TagsField = value;
                    this.RaisePropertyChanged("Tags");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Url {
            get {
                return this.UrlField;
            }
            set {
                if ((object.ReferenceEquals(this.UrlField, value) != true)) {
                    this.UrlField = value;
                    this.RaisePropertyChanged("Url");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string UrlThumbnail {
            get {
                return this.UrlThumbnailField;
            }
            set {
                if ((object.ReferenceEquals(this.UrlThumbnailField, value) != true)) {
                    this.UrlThumbnailField = value;
                    this.RaisePropertyChanged("UrlThumbnail");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string UserId {
            get {
                return this.UserIdField;
            }
            set {
                if ((object.ReferenceEquals(this.UserIdField, value) != true)) {
                    this.UserIdField = value;
                    this.RaisePropertyChanged("UserId");
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
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ImageWallServiceReference.IImageWallService")]
    public interface IImageWallService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IImageWallService/BeginImageUploadREST", ReplyAction="http://tempuri.org/IImageWallService/BeginImageUploadRESTResponse")]
        bool BeginImageUploadREST(string name, System.DateTime created, double latitude, double longitude, string hash, long size);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IImageWallService/BeginImageUpload", ReplyAction="http://tempuri.org/IImageWallService/BeginImageUploadResponse")]
        bool BeginImageUpload(ImageWallClientDesktop.ImageWallServiceReference.ImageDetails details);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IImageWallService/UploadImagePart", ReplyAction="http://tempuri.org/IImageWallService/UploadImagePartResponse")]
        System.Exception UploadImagePart(int index, byte[] imageBytes);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IImageWallService/GetAllTags", ReplyAction="http://tempuri.org/IImageWallService/GetAllTagsResponse")]
        string[] GetAllTags();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IImageWallService/GetAllTagSize", ReplyAction="http://tempuri.org/IImageWallService/GetAllTagSizeResponse")]
        System.Collections.Generic.KeyValuePair<string, int>[] GetAllTagSize();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IImageWallService/GetTagModified", ReplyAction="http://tempuri.org/IImageWallService/GetTagModifiedResponse")]
        System.DateTime GetTagModified(string tag);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IImageWallService/GetTagSize", ReplyAction="http://tempuri.org/IImageWallService/GetTagSizeResponse")]
        int GetTagSize(string tag);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IImageWallService/IsTagPopular", ReplyAction="http://tempuri.org/IImageWallService/IsTagPopularResponse")]
        bool IsTagPopular(string tag);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IImageWallService/GetImagesByDate", ReplyAction="http://tempuri.org/IImageWallService/GetImagesByDateResponse")]
        ImageWallClientDesktop.ImageWallServiceReference.ImageDetails[] GetImagesByDate(System.DateTime startDate, System.DateTime endDate);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IImageWallService/GetImagesByAmount", ReplyAction="http://tempuri.org/IImageWallService/GetImagesByAmountResponse")]
        ImageWallClientDesktop.ImageWallServiceReference.ImageDetails[] GetImagesByAmount(int maxAmount, int skip);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IImageWallService/GetImagesByTagDate", ReplyAction="http://tempuri.org/IImageWallService/GetImagesByTagDateResponse")]
        ImageWallClientDesktop.ImageWallServiceReference.ImageDetails[] GetImagesByTagDate(string tag, System.DateTime startDate, System.DateTime endDate);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IImageWallService/GetImagesByTagAmount", ReplyAction="http://tempuri.org/IImageWallService/GetImagesByTagAmountResponse")]
        ImageWallClientDesktop.ImageWallServiceReference.ImageDetails[] GetImagesByTagAmount(string tag, int maxAmount, int skip);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IImageWallServiceChannel : ImageWallClientDesktop.ImageWallServiceReference.IImageWallService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ImageWallServiceClient : System.ServiceModel.ClientBase<ImageWallClientDesktop.ImageWallServiceReference.IImageWallService>, ImageWallClientDesktop.ImageWallServiceReference.IImageWallService {
        
        public ImageWallServiceClient() {
        }
        
        public ImageWallServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ImageWallServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ImageWallServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ImageWallServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public bool BeginImageUploadREST(string name, System.DateTime created, double latitude, double longitude, string hash, long size) {
            return base.Channel.BeginImageUploadREST(name, created, latitude, longitude, hash, size);
        }
        
        public bool BeginImageUpload(ImageWallClientDesktop.ImageWallServiceReference.ImageDetails details) {
            return base.Channel.BeginImageUpload(details);
        }
        
        public System.Exception UploadImagePart(int index, byte[] imageBytes) {
            return base.Channel.UploadImagePart(index, imageBytes);
        }
        
        public string[] GetAllTags() {
            return base.Channel.GetAllTags();
        }
        
        public System.Collections.Generic.KeyValuePair<string, int>[] GetAllTagSize() {
            return base.Channel.GetAllTagSize();
        }
        
        public System.DateTime GetTagModified(string tag) {
            return base.Channel.GetTagModified(tag);
        }
        
        public int GetTagSize(string tag) {
            return base.Channel.GetTagSize(tag);
        }
        
        public bool IsTagPopular(string tag) {
            return base.Channel.IsTagPopular(tag);
        }
        
        public ImageWallClientDesktop.ImageWallServiceReference.ImageDetails[] GetImagesByDate(System.DateTime startDate, System.DateTime endDate) {
            return base.Channel.GetImagesByDate(startDate, endDate);
        }
        
        public ImageWallClientDesktop.ImageWallServiceReference.ImageDetails[] GetImagesByAmount(int maxAmount, int skip) {
            return base.Channel.GetImagesByAmount(maxAmount, skip);
        }
        
        public ImageWallClientDesktop.ImageWallServiceReference.ImageDetails[] GetImagesByTagDate(string tag, System.DateTime startDate, System.DateTime endDate) {
            return base.Channel.GetImagesByTagDate(tag, startDate, endDate);
        }
        
        public ImageWallClientDesktop.ImageWallServiceReference.ImageDetails[] GetImagesByTagAmount(string tag, int maxAmount, int skip) {
            return base.Channel.GetImagesByTagAmount(tag, maxAmount, skip);
        }
    }
}