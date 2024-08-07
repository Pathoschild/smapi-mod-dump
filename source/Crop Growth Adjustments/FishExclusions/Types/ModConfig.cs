/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/gzhynko/stardew-mods
**
*************************************************/

using System;
using System.Collections.Generic;
using StardewModdingAPI;

namespace FishExclusions.Types
{
    public interface IGenericModConfigMenuApi
    {
        void Register(IManifest mod, Action reset, Action save, bool titleScreenOnly = false);
        void AddSectionTitle(IManifest mod, Func<string> text, Func<string> tooltip = null);
        void AddNumberOption(IManifest mod, Func<int> getValue, Action<int> setValue, Func<string> name, Func<string> tooltip = null, int? min = null, int? max = null, int? interval = null, Func<int, string> formatValue = null, string fieldId = null);
        void AddTextOption(IManifest mod, Func<string> getValue, Action<string> setValue, Func<string> name, Func<string> tooltip = null, string[] allowedValues = null, Func<string, string> formatAllowedValue = null, string fieldId = null);
        void AddParagraph(IManifest mod, Func<string> text);
    }

    /// <summary> The mod config class. More info here: https://stardewvalleywiki.com/Modding:Modder_Guide/APIs/Config </summary>
    public class ModConfig
    {
        /// <summary>
        /// The items to exclude.
        /// </summary>
        public ItemsToExclude ItemsToExclude { get; set; } = new ();
        
        /// <summary>
        /// The ID of the item to catch if all possible fish for this water body / season / weather is excluded.
        /// </summary>
        public string ItemToCatchIfAllFishIsExcluded { get; set; } = "168";
        
        /// <summary>
        /// The number of times to retry fish selection before giving up and catching the item specified above.
        /// WARNING: Large numbers can cause a Stack Overflow exception. Use with caution.
        /// </summary>
        public int TimesToRetry { get; set; } = 20;
        
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
            
            api.AddSectionTitle(manifest, () => "General");

            api.AddTextOption(manifest, () => config.ItemToCatchIfAllFishIsExcluded, val => config.ItemToCatchIfAllFishIsExcluded = val, () => "Item To Catch If All Fish Is Excluded", () => "The ID of the item to catch if all possible fish for this location / season / weather is excluded.");
            api.AddNumberOption(manifest, () => config.TimesToRetry, val => config.TimesToRetry = val, () => "Times To Retry", () => "The number of times to retry fish selection before giving up and catching the item specified above. WARNING: Large numbers can cause a Stack Overflow exception. Use with caution.", 5, 50);

            api.AddParagraph(manifest, () => "To edit the actual excluded fish, use the config.json file located in [Stardew Valley folder]/Mods/FishExclusions. For instructions on how to add exclusions, refer to the mod description on Nexus.");
        }
    }

    public class ItemsToExclude
    {
        /// <summary>
        /// Season- and location-independent exclusions.
        /// </summary>
        public string[] CommonExclusions { get; set; } = Array.Empty<string>();
        
        /// <summary>
        /// Season- and/or location-dependent exclusions.
        /// </summary>
        public List<ConditionalExclusion> ConditionalExclusions { get; set; } = new ();
    }

    public class ConditionalExclusion
    {
        public string Season { get; set; } = "";
        
        public string Weather { get; set; } = "";

        public string Location { get; set; } = "";
        
        public string[] Exclusions { get; set; } = Array.Empty<string>();
    }
}
