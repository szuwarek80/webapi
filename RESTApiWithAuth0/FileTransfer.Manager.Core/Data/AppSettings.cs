using FileTransfer.Manager.Persistence.Connection;
using System;
using System.Xml.Serialization;

namespace FileTransfer.Manager.Core.Data
{
    [Serializable()]
    public class AppSettings : IConnectionSettings
    {
        [XmlElement]
        public string Host { get; set; }

        [XmlElement]
        public string Database { get; set; }

        [XmlElement]
        public AuthenticationMode AuthenticationMode { get; set; }

        [XmlElement]
        public string User { get; set; }

        [XmlElement]
        public string Password { get; set; }

        [XmlElement]
        public int NewRequestsQueryInterval { get; set; }

        [XmlElement]
        public int StatusRequestInterval { get; set; } = 5000;
    }
}
