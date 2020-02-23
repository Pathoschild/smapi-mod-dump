using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using ModSettingsTab.Framework.Components;
using ModSettingsTab.Menu;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StardewModdingAPI;
using StardewValley;

namespace ModSettingsTab.Framework.Integration
{
    public class SmapiIntegration
    {
        public readonly List<OptionsElement> Options = new List<OptionsElement>();
        private readonly StaticConfig _staticConfig;

        public SmapiIntegration()
        {
            var configPath = Path.Combine(Constants.ExecutionPath, "smapi-internal/config.json");
            if (!File.Exists(configPath))
            {
                Helper.Console.Error("SMAPI Config not found? :)");
                return;
            }

            var json = File.ReadAllText(configPath);
            json = Regex.Replace(json, "\\/{2}\"(ParanoidWarnings|UseBetaChannel)\": true,", "\"$1\": false,");
            var jObj = JObject.Parse(json);
            _staticConfig = new StaticConfig(configPath, jObj,"Pathoschild.SMAPI");

            InitOptions();
        }

        private void InitOptions()
        {
            const string uniqueId = "Pathoschild.SMAPI";
            var lang = LocalizedContentManager.CurrentLanguageCode;
            var nI9NPath = Path.Combine(Helper.DirectoryPath, "data/I9N/SmapiIntegration.json");
            ModIntegrationSettings nI9N = null;
            try
            {
                nI9N = File.Exists(nI9NPath)
                    ? JsonConvert.DeserializeObject<ModIntegrationSettings>(File.ReadAllText(nI9NPath))
                    : null;
            }
            catch (Exception e)
            {
                Helper.Console.Warn(e.Message);
            }
            
            Options.Add(new SmapiHeading(BaseOptionsModPage.SlotSize)
            {
                Label = "SMAPI",
                HoverText = nI9N?.Description[lang] ??
                            "The default values are mirrored in StardewModdingAPI.Framework.Models.SConfig to log custom changes.",
                HoverTitle = "Pathoschild",
            });
            foreach (KeyValuePair<string, JToken> param in _staticConfig)
            {
                var option = Mod.CreateOption(
                    param.Key, uniqueId, _staticConfig,
                    BaseOptionsModPage.SlotSize, null, nI9N);
                if (option == null) continue;
                Options.Add(option);
            }
        }
    }
}