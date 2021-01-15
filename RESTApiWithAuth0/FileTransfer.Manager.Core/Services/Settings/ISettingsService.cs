using FileTransfer.Manager.Core.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileTransfer.Manager.Core.Services.Settings
{
    public interface ISettingsService
    {
        AppSettings AppSettings { get; }

        void DeserializeSettings();
    }
}
