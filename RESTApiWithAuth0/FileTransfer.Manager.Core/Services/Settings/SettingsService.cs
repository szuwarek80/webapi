using FileTransfer.Manager.Core.Data;
using System;
using System.IO;
using System.Xml.Serialization;

namespace FileTransfer.Manager.Core.Services.Settings
{
    public class SettingsService : ISettingsService
    {
        [XmlIgnore]
        private readonly string fileName = "AppSettings.xml";

        [XmlIgnore]
        private readonly string cfgDirName = "Config";

        public AppSettings AppSettings { get; private set; }

        public SettingsService()
        {
            this.DeserializeSettings();
        }

        public SettingsService(bool aIsDummy)
        {
            this.AppSettings = new AppSettings();
        }
        
        public void DeserializeSettings()
        {
            var path = Path.Combine(AppContext.BaseDirectory, cfgDirName, fileName);

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(AppSettings));

                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    this.AppSettings = (AppSettings)serializer.Deserialize(fs);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Config file doesn't exist or is invalid.", e);
            }
        }
    }
}
