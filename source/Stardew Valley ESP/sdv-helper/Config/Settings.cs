using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using StardewModdingAPI;
using System.Collections.Generic;
using System.IO;

namespace sdv_helper.Config
{
    class Settings
    {
        class InternalSettings
        {
            public SButton MenuKey { get; set; } = SButton.K;
            public SButton LoadKey { get; set; } = SButton.L;
            public Dictionary<string, int> Colors { get; set; }
        }

        private static readonly string defaultContent = "{\"MenuKey\":\"K\", \"LoadKey\":\"L\",\"Colors\":{}}";
        private readonly IModHelper helper;
        private InternalSettings settings;
        private string path;

        public SButton LoadKey
        {
            get { return settings.LoadKey; }
            set { settings.LoadKey = value; SaveSettings(); }
        }
        public SButton MenuKey
        {
            get { return settings.MenuKey; }
            set { settings.MenuKey = value; SaveSettings(); }
        }
        public Dictionary<string, int> DSettings
        {
            get { return settings.Colors; }
            set { settings.Colors = value; }
        }

        public Settings(IModHelper helper)
        {
            this.helper = helper;
            LoadSettings();
        }

        public void LoadSettings()
        {
            path = Path.Combine(helper.DirectoryPath, "settings.json");
            if (!File.Exists(path))
                File.WriteAllText(path, defaultContent);
            string text = File.ReadAllText(path);
            settings = JsonConvert.DeserializeObject<InternalSettings>(text);
        }

        public void SaveSettings()
        {
            string text = JsonConvert.SerializeObject(settings);
            File.WriteAllText(path, text);
        }

        public void SetDefaultsFor(string name)
        {
            DSettings.Add(name, DSettings.Count == 0 ? 19 : 0);
            SaveSettings();
        }

        public void SetColorFor(string name, int color)
        {
            DSettings[name] = color;
            SaveSettings();
        }

        public int GetColorFor(string name)
        {
            if (!DSettings.ContainsKey(name))
                SetDefaultsFor(name);
            return DSettings[name];
        }
    }
}
