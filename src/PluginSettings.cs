using Advanced_Combat_Tracker;
using System.IO;
using System.Text;
using System.Xml;

namespace ACTTimeline
{
    public class PluginSettings : SettingsSerializer
    {
        private string settingsFile;

        public PluginSettings(object actPlugin)
            : base(actPlugin)
        {
            string configFolderPath = Path.Combine(ActGlobals.oFormActMain.AppDataFolder.FullName, "Config");

            string oldSettingsFile = Path.Combine(configFolderPath, "ACTTimeline_a.config.xml");
            string newSettingsFile = Path.Combine(configFolderPath, "ACTTimeline.config");

            if (File.Exists(oldSettingsFile))
            {
                File.Move(oldSettingsFile, newSettingsFile);
            }

            if (File.Exists(newSettingsFile))
            {
                string temp = File.ReadAllText(newSettingsFile);
                temp = temp.Replace("Int32 Name=\"Bar", "NumericUpDown Name=\"Bar");
                temp = temp.Replace("Int32 Name=\"OpacityPercentage", "TrackBar Name=\"OpacityPercentage");
                File.WriteAllText(newSettingsFile, temp);
            }

            settingsFile = newSettingsFile;
        }

        public void Load()
        {
            if (File.Exists(settingsFile))
            {
                FileStream fs = new FileStream(settingsFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                XmlTextReader reader = new XmlTextReader(fs);
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.LocalName == "SettingsSerializer")
                        ImportFromXml(reader);
                }
                reader.Close();
            }
        }

        public void Save()
        {
            FileStream stream = new FileStream(settingsFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8);
            writer.Formatting = Formatting.Indented;
            writer.Indentation = 1;
            writer.IndentChar = '\t';
            writer.WriteStartDocument(true);
            writer.WriteStartElement("Config");
            writer.WriteStartElement("SettingsSerializer");
            ExportToXml(writer);
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            writer.Close();
        }
    }
}
