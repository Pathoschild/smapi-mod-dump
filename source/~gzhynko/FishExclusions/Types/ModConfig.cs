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
using System.Collections.Generic;
using StardewModdingAPI;

namespace FishExclusions.Types
{
    public interface IGenericModConfigMenuApi
    {
        void RegisterModConfig(IManifest mod, Action revertToDefault, Action saveToFile);

        void SetDefaultIngameOptinValue( IManifest mod, bool optedIn );
        
        void RegisterLabel(IManifest mod, string labelName, string labelDesc);
        void RegisterParagraph(IManifest mod, string paragraph);
        
        void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<bool> optionGet, Action<bool> optionSet);
        void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<int> optionGet, Action<int> optionSet);

        void RegisterClampedOption(IManifest mod, string optionName, string optionDesc, Func<int> optionGet, Action<int> optionSet, int min, int max);
    }

    /// <summary> The mod config class. More info here: https://stardewvalleywiki.com/Modding:Modder_Guide/APIs/Config </summary>
    public class ModConfig
    {
        /// <summary>
        /// The items to exclude.
        /// </summary>
        public ItemsToExclude ItemsToExclude { get; set; } = new ItemsToExclude();
        
        /// <summary>
        /// The ID of the item to catch if all possible fish for this water body / season / weather is excluded.
        /// </summary>
        public int ItemToCatchIfAllFishIsExcluded { get; set; } = 168;
        
        /// <summary>
        /// The number of times to retry the 'fish choosing' algorithm before giving up and catching the item specified above.
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

            api.RegisterModConfig(manifest, () =>
            {
                config = new ModConfig();
                mod.SaveConfig(config);
            }, () => mod.SaveConfig(config));
            
            api.SetDefaultIngameOptinValue(manifest, true);

            api.RegisterLabel(manifest, "General", null);

            api.RegisterSimpleOption(manifest, "Item To Catch If All Fish Is Excluded", "The ID of the item to catch if all possible fish for this water body / season / weather is excluded.", () => config.ItemToCatchIfAllFishIsExcluded, (int val) => config.ItemToCatchIfAllFishIsExcluded = val);
            api.RegisterClampedOption(manifest, "Times To Retry", "The number of times to retry the 'fish choosing' algorithm before giving up and catching the item specified above.", () => config.TimesToRetry, (int val) => config.TimesToRetry = val, 5, 50);

            api.RegisterParagraph(manifest, "To edit the actual excluded fish, please use the config file. For instructions on how to add the exclusions, refer to the mod description on Nexus. Thanks!");
        }
    }

    public class ItemsToExclude
    {
        /// <summary>
        /// Season- and location-independent exclusions.
        /// </summary>
        public object[] CommonExclusions { get; set; } = { };
        
        /// <summary>
        /// Season- and/or location-dependent exclusions.
        /// </summary>
        public List<ConditionalExclusion> ConditionalExclusions { get; set; } = new List<ConditionalExclusion>();
    }

    public class ConditionalExclusion
    {
        public string Season { get; set; } = "";
        
        public string Weather { get; set; } = "";

        public string Location { get; set; } = "";
        
        public object[] FishToExclude { get; set; } = { };
    }
}
