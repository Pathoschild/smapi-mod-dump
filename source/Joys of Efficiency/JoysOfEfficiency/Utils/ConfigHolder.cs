/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/pomepome/JoysOfEfficiency
**
*************************************************/

using System.IO;
using Newtonsoft.Json;

namespace JoysOfEfficiency.Utils
{
    public abstract class ConfigHolder<T>
    {
        private readonly string _configFileName;

        public T Entry { get; private set; }

        private static Logger Logger = new Logger("ConfigHolder");

        protected ConfigHolder(string filePath)
        {
            _configFileName = filePath;
            Load();
            Save();
        }

        protected void Load()
        {
            if (!File.Exists(_configFileName))
            {
                Entry = GetNewInstance();
                return;
            }

            Logger.Log("Loaded "+ _configFileName);

            string jsonContent = File.ReadAllText(_configFileName);
            Entry = JsonConvert.DeserializeObject<T>(jsonContent);
        }

        public void Save()
        {
            string jsonContent = JsonConvert.SerializeObject(Entry, Formatting.Indented);
            File.WriteAllText(_configFileName, jsonContent);
            Logger.Log("Saved " + Path.GetFullPath(_configFileName));
        }

        protected abstract T GetNewInstance();
    }
}
