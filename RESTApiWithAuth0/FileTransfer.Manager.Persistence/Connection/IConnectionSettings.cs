using System;
using System.Collections.Generic;
using System.Text;

namespace FileTransfer.Manager.Persistence.Connection
{
    public interface IConnectionSettings
    {
        string Host { get; }

        string Database { get; }

        AuthenticationMode AuthenticationMode { get; }

        string User { get; }

        string Password { get; }

        int NewRequestsQueryInterval { get; set; }
    }

    [Serializable]
    public enum AuthenticationMode
    {
        WINDOWS_USER,
        DATABASE_USER,
    }
}
