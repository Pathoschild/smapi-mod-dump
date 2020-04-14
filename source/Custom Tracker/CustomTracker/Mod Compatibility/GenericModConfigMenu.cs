using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace CustomTracker
{
    /// <summary>The mod's main class.</summary>
    public partial class ModEntry : Mod
    {
        public void EnableGMCM(object sender, GameLaunchedEventArgs e)
        {
            GenericModConfigMenuAPI api = Helper.ModRegistry.GetApi<GenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu"); //attempt to get GMCM's API instance

            if (api == null) //if the API is not available
                return;

            api.RegisterModConfig(ModManifest, () => MConfig = new ModConfig(), () => Helper.WriteConfig(MConfig)); //register "revert to default" and "write" methods for this mod's config

            //register an option for each of this mod's config settings
            api.RegisterSimpleOption(ModManifest, "Enable trackers without profession", "If this box is checked, you won't need the Foraging skill's \"Tracker\" perk to see trackers.", () => MConfig.EnableTrackersWithoutProfession, (bool val) => MConfig.EnableTrackersWithoutProfession = val);
            api.RegisterSimpleOption(ModManifest, "Replace trackers with forage icons", "If this box is checked, trackers will display the objects they're pointing to.", () => MConfig.ReplaceTrackersWithForageIcons, (bool val) => MConfig.ReplaceTrackersWithForageIcons = val);
            api.RegisterSimpleOption(ModManifest, "Draw trackers behind interface", "If this box is checked, trackers will be drawn behind the game's interface, making it easier to see the UI.", () => MConfig.DrawBehindInterface, (bool val) => MConfig.DrawBehindInterface = val);
            api.RegisterSimpleOption(ModManifest, "Tracker pixel scale", "The size of the tracker icon's pixels (default 4). Increase this to make trackers easier to see.", () => MConfig.TrackerPixelScale, (float val) => MConfig.TrackerPixelScale = val);

            api.RegisterSimpleOption(ModManifest, "Track default forage", "If this box is checked, the mod will track most types of forage spawned by the base game.", () => MConfig.TrackDefaultForage, (bool val) => MConfig.TrackDefaultForage = val);
            api.RegisterSimpleOption(ModManifest, "Track artifact spots", "If this box is checked, the mod will track buried artifact locations.", () => MConfig.TrackArtifactSpots, (bool val) => MConfig.TrackArtifactSpots = val);
            api.RegisterSimpleOption(ModManifest, "Track panning spots", "If this box is checked, the mod will track ore panning locations in the water.", () => MConfig.TrackPanningSpots, (bool val) => MConfig.TrackPanningSpots = val);
            api.RegisterSimpleOption(ModManifest, "Track spring onions", "If this box is checked, the mod will track harvestable spring onions.", () => MConfig.TrackSpringOnions, (bool val) => MConfig.TrackSpringOnions = val);
            api.RegisterSimpleOption(ModManifest, "Track berry bushes", "If this box is checked, the mod will track harvestable salmonberry and blackberry bushes.", () => MConfig.TrackBerryBushes, (bool val) => MConfig.TrackBerryBushes = val);
        }
    }

    /// <summary>Generic Mod Config Menu's API interface. Used to recognize & interact with the mod's API when available.</summary>
    public interface GenericModConfigMenuAPI
    {
        void RegisterModConfig(IManifest mod, Action revertToDefault, Action saveToFile);

        void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<bool> optionGet, Action<bool> optionSet);
        void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<float> optionGet, Action<float> optionSet);
    }
}
