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

namespace EventBlackBars
{
    public interface IGenericModConfigMenuApi
    {
        void Register(IManifest mod, Action reset, Action save, bool titleScreenOnly = false);
        void AddSectionTitle(IManifest mod, Func<string> text, Func<string> tooltip = null);
        void AddBoolOption(IManifest mod, Func<bool> getValue, Action<bool> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null);
        void AddNumberOption(IManifest mod, Func<float> getValue, Action<float> setValue, Func<string> name, Func<string> tooltip = null, float? min = null, float? max = null, float? interval = null, Func<float, string> formatValue = null, string fieldId = null);
    }
    
    /// <summary> The mod config class. More info here: https://stardewvalleywiki.com/Modding:Modder_Guide/APIs/Config </summary>
    public class ModConfig
    {
        /// <summary>
        /// The percentage of the height of the screen for a bar to take up.
        /// </summary>
        public double BarHeightPercentage { get; set; } = 10;
        
        /// <summary>
        /// Whether to gradually move the bars in when an event starts, or have them fully out right away.
        /// </summary>
        public bool MoveBarsInSmoothly { get; set; } = true;
        
        /// <summary>
        /// Setup the Generic Mod Config Menu API.
        /// </summary>
        public static void SetUpModConfigMenu(ModConfig config, ModEntry mod)
        {
            IGenericModConfigMenuApi api = mod.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (api == null) return;

            var manifest = mod.ModManifest;

            api.Register(manifest, () =>
            {
                config = new ModConfig();
                mod.SaveConfig(config);
            }, () => mod.SaveConfig(config));
            
            api.AddSectionTitle(manifest, () => "Appearance");

            api.AddNumberOption(manifest, () => (float)config.BarHeightPercentage, val => config.BarHeightPercentage = val, () => "Bar Height Percentage", () => "The percentage of the height of the screen for a bar to take up.", 1f, 49f, 0.1f);
            api.AddBoolOption(manifest, () => config.MoveBarsInSmoothly, val => config.MoveBarsInSmoothly = val, () => "Move Bars In Smoothly", () => "Whether to gradually move the bars in when an event starts, or have them fully out right away.");
        }
    }
}
