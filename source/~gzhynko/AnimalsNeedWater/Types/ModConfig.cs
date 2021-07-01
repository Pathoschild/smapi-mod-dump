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

namespace AnimalsNeedWater.Types
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
        /// Whether to show "love" bubbles over animals inside the building when watered the trough.
        /// </summary>
        public bool ShowLoveBubblesOverAnimalsWhenWateredTrough { get; set; } = true;

        /// <summary>
        /// Whether to enable the watering system in Deluxe Coops and Deluxe Barns.
        /// </summary>
        public bool WateringSystemInDeluxeBuildings { get; set; } = true;

        /// <summary>
        /// Whether to replace coop's and big coop's textures when troughs inside them are empty.
        /// </summary>
        public bool ReplaceCoopTextureIfTroughIsEmpty { get; set; } = true;

        /// <summary>
        /// The amount of friendship points player gets for watering a trough.
        /// </summary>
        public int FriendshipPointsForWateredTrough { get; set; } = 15;

        /// <summary>
        /// The amount of friendship points player gets for watering a trough with animals inside the building.
        /// </summary>
        public int AdditionalFriendshipPointsForWateredTroughWithAnimalsInsideBuilding { get; set; } = 15;

        /// <summary>
        /// The amount of friendship points player loses for not watering a trough.
        /// </summary>
        public int NegativeFriendshipPointsForNotWateredTrough { get; set; } = 20;

        /// <summary>
        /// Whether animals can drink outside.
        /// </summary>
        public bool AnimalsCanDrinkOutside { get; set; } = true;

        /// <summary>
        /// Whether animals can only drink from lakes/rivers/seas etc. If set to false, animals will drink from any place you can refill your watering can at (well, troughs, water bodies etc.).
        /// </summary>
        public bool AnimalsCanOnlyDrinkFromWaterBodies { get; set; } = true;
        
        /// <summary>
        /// Whether troughs should have a cleaner texture.
        /// </summary>
        public bool CleanerTroughs { get; set; } = false;
        
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

            api.RegisterLabel(manifest, "Visual", null);

            api.RegisterSimpleOption(manifest, "Show Love Bubbles", "Whether to show \"love\" bubbles over animals inside the building when watered the trough.", () => config.ShowLoveBubblesOverAnimalsWhenWateredTrough, (bool val) => config.ShowLoveBubblesOverAnimalsWhenWateredTrough = val);
            api.RegisterSimpleOption(manifest, "Replace Coop Texture If Trough Is Empty", "Whether to replace coop's and big coop's textures when troughs inside them are empty.", () => config.ReplaceCoopTextureIfTroughIsEmpty, (bool val) => config.ReplaceCoopTextureIfTroughIsEmpty = val);
            api.RegisterSimpleOption(manifest, "Cleaner Troughs", "Whether troughs should have a cleaner texture. Note: Won't change until a day update.", () => config.CleanerTroughs, (bool val) => config.CleanerTroughs = val);
            
            api.RegisterLabel(manifest, "Functionality", null);
            
            api.RegisterSimpleOption(manifest, "Watering System In Deluxe Buildings", "Whether to enable the watering system in Deluxe Coops and Deluxe Barns.", () => config.WateringSystemInDeluxeBuildings, (bool val) => config.WateringSystemInDeluxeBuildings = val);
            api.RegisterSimpleOption(manifest, "Animals Can Drink Outside", "Whether animals can drink outside.", () => config.AnimalsCanDrinkOutside, (bool val) => config.AnimalsCanDrinkOutside = val);
            api.RegisterSimpleOption(manifest, "Animals Can Only Drink From Water Bodies", "Whether animals can only drink from lakes/rivers/seas etc. If set to false, animals will drink from any place you can refill your watering can at (well, troughs, water bodies etc.).", () => config.AnimalsCanOnlyDrinkFromWaterBodies, (bool val) => config.AnimalsCanOnlyDrinkFromWaterBodies = val);
            
            api.RegisterLabel(manifest, "Friendship", null);

            api.RegisterSimpleOption(manifest, "Watered Trough", "The amount of friendship points player gets for watering a trough.", () => config.FriendshipPointsForWateredTrough, (int val) => config.FriendshipPointsForWateredTrough = val);
            api.RegisterSimpleOption(manifest, "Watered Trough With Animals Inside", "The amount of friendship points player gets for watering a trough with animals inside the building.", () => config.AdditionalFriendshipPointsForWateredTroughWithAnimalsInsideBuilding, (int val) => config.AdditionalFriendshipPointsForWateredTroughWithAnimalsInsideBuilding = val);
            api.RegisterSimpleOption(manifest, "Negative; Not Watered Trough", "The amount of friendship points player loses for not watering a trough.", () => config.NegativeFriendshipPointsForNotWateredTrough, (int val) => config.NegativeFriendshipPointsForNotWateredTrough = val);
        }
    }
}
