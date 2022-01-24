using System;
using System.IO;
using System.Xml.Serialization;

namespace ASSWinFormsApp1
{
    public class Settings
    {
        static string settingsFilePath = Helper.GetPathForAppFolder("settings.xml");
        static Settings s_settings;

        private Settings() { }

        public bool FirstRun { get; set; } = true;
        public string savePath { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        public int saveName2 { get; set; }
        public int saveExt { get; set; }
        public int openExe { get; set; }
        public bool isPre { get; set; } = true;
        public bool isSou { get; set; } = true;

        public static Settings Load()
        {
            if (s_settings == null)
            {
                if (File.Exists(settingsFilePath))
                {
                    using (FileStream fileStream = File.OpenRead(settingsFilePath))
                    {
                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(Settings));
                        s_settings = (Settings)xmlSerializer.Deserialize(fileStream);
                    }
                }
                else
                {
                    s_settings = new Settings();
                }
            }
            return s_settings;
        }

        public static void Save()
        {
            using (FileStream fileStream = File.Create(settingsFilePath))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(Settings));
                xmlSerializer.Serialize(fileStream, s_settings);
            }
        }
    }
}
