using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace HudsonBeacon.Helpers
{
    public delegate void SettingsGotSavedEvent(string path);

    [Serializable]
    public class Settings
    {
        public string Uri { get; set; }
        public int Lightness { get; set; }

        public int FailurePulseIntervall { get; set; }
        public int SuccessPulseIntervall { get; set; }
        public int FetchMinuteIntervall { get; set; }

        public event SettingsGotSavedEvent OnSettingsGotSaved = delegate { };

        public Settings()
        {
            Lightness = 10;

            FailurePulseIntervall = 1;
            SuccessPulseIntervall = 15;
            FetchMinuteIntervall = 30;
        }

        public static Settings Load(string path = "")
        {
            if (path == "")
                path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "HudsonBeacon",
                    "settings.xml");

            Directory.CreateDirectory(Path.GetDirectoryName(path));

            // Create a new file stream for reading the XML file
            using(FileStream ReadFileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read))
            {
                Settings settings = new Settings();
                try
                {
                    XmlSerializer SerializerObj = new XmlSerializer(typeof (Settings));

                    settings = (Settings)SerializerObj.Deserialize(ReadFileStream);
                }
                catch{}
                // Load the object saved above by using the Deserialize function
                return settings;
            }
        }

        public void Save(string path = "")
        {
            if (path == "")
                path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "HudsonBeacon",
                    "settings.xml");

            Directory.CreateDirectory(Path.GetDirectoryName(path));

            // Create a new XmlSerializer instance with the type of the test class
            XmlSerializer SerializerObj = new XmlSerializer(typeof(Settings));

            // Create a new file stream to write the serialized object to a file
            using (TextWriter WriteFileStream = new StreamWriter(path))
            {
                SerializerObj.Serialize(WriteFileStream, this);
            }
        }
    }
}
