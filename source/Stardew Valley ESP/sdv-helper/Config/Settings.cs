using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sdv_helper.Config
{
    class Settings
    {
        private static readonly string defaultContent = "{}";
        public Dictionary<string, int> dSettings { get; } = new Dictionary<string, int>();
        private readonly IModHelper helper;
        private string path;
        public Settings(IModHelper helper)
        {
            this.helper = helper;
            LoadSettings();
        }

        public void LoadSettings()
        {
            path = Path.Combine(helper.DirectoryPath, "settings.json");
            if (!File.Exists(path))
            {
                File.WriteAllText(path, defaultContent);
            }
            string text = File.ReadAllText(path);
            dynamic json = JsonConvert.DeserializeObject(text);
            dSettings.Clear();
            foreach (var entry in json)
            {
                dSettings.Add(entry.Name, entry.Value.ToObject<int>());
            }
        }

        public void SaveSettings()
        {
            string text = JsonConvert.SerializeObject(dSettings);
            File.WriteAllText(path, text);
        }

        public void SetDefaultsFor(string name)
        {
            dSettings.Add(name, 19);
            SaveSettings();
        }

        public void SetColorFor(string name, int color)
        {
            dSettings[name] = color;
            SaveSettings();
        }

        public int GetColorFor(string name)
        {
            if (!dSettings.ContainsKey(name)) SetDefaultsFor(name);
            return dSettings[name];
        }
    }
}
