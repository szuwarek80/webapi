using FileTransfer.Manager.Core.Data;

namespace FileTransfer.Manager.Core.Services.Settings
{
    public interface ISettingsService
    {
        AppSettings AppSettings { get; }

        void DeserializeSettings();
    }
}
