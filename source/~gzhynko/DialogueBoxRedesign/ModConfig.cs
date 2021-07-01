/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/gzhynko/StardewMods
**
*************************************************/

using System;
using StardewModdingAPI;

namespace DialogueBoxRedesign
{
    public interface IGenericModConfigMenuApi
    {
        void RegisterModConfig(IManifest mod, Action revertToDefault, Action saveToFile);

        void SetDefaultIngameOptinValue( IManifest mod, bool optedIn );
        
        void RegisterLabel(IManifest mod, string labelName, string labelDesc);

        void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<bool> optionGet, Action<bool> optionSet);
    }
    
    /// <summary> The mod config class. More info here: https://stardewvalleywiki.com/Modding:Modder_Guide/APIs/Config </summary>
    public class ModConfig
    {
        /// <summary>
        /// Whether to show a darker gradient background in winter for better text readability.
        /// </summary>
        public bool DarkerBackgroundInWinter { get; set; } = true;
        
        /// <summary>
        /// Setup the Generic Mod Config Menu API.
        /// </summary>
        public static void SetUpModConfigMenu(ModConfig config, ModEntry mod)
        {
            IGenericModConfigMenuApi api = mod.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (api == null) return;

            var manifest = mod.ModManifest;

            api.RegisterModConfig(manifest, () =>
            {
                config = new ModConfig();
                mod.SaveConfig(config);
            }, () => mod.SaveConfig(config));
            
            api.SetDefaultIngameOptinValue(manifest, true);

            api.RegisterLabel(manifest, "Appearance", null);

            api.RegisterSimpleOption(manifest, "Darker Background In Winter", "Whether to show a darker gradient background in winter for better text readability.", () => config.DarkerBackgroundInWinter, (bool val) => config.DarkerBackgroundInWinter = val);
        }
    }
}