using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RumbleHeartRateDisplay
{
    public class Settings
    {
        public static Settings Instance { get; private set; }
        public string BluetoothDeviceName { get; set; }

        public static Settings Initialize()
        {
            var newInstance = new Settings
            {
                BluetoothDeviceName = "Pixel Watch" // Default value
            };
            Instance = newInstance;
            return newInstance;
        }

        public static Settings FromXmlFile(string filePath)
        {
            var serializer = new XmlSerializer(typeof(Settings));
            using FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            Settings settings = (Settings)serializer.Deserialize(fileStream);

            Instance = settings;
            return settings;
        }

        public void ToXmlFile(string filePath)
        {
            using var writer = new StreamWriter(filePath);
            var serializer = new XmlSerializer(this.GetType());
            serializer.Serialize(writer, this);
            writer.Flush();
        }
    }
}
