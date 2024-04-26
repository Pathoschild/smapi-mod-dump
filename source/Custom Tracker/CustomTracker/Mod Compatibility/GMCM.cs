/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/CustomTracker
**
*************************************************/

using GenericModConfigMenu;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;
using static CustomTracker.ModEntry;

namespace CustomTracker
{
    /// <summary>Handles this mod's option menu for Generic Mod Config Menu (GMCM).</summary>
    public static class GMCM
    {
        /// <summary>This mod's SMAPI monitor.</summary>
        private static IMonitor Monitor => ModEntry.Instance.Monitor;
        /// <summary>This mod's SMAPI helper.</summary>
        private static IModHelper Helper => ModEntry.Instance.Helper;
        /// <summary>This mod's SMAPI manifest.</summary>
        private static IManifest ModManifest => ModEntry.Instance.ModManifest;
        /// <summary>This mod's "config.json" class instance.</summary>
        private static ModConfig Config
        {
            get => ModEntry.Config;
            set => ModEntry.Config = value;
        }

        /// <summary>True if the method <see cref="Enable"/> has already run once.</summary>
        /// <remarks>This does NOT indicate whether GMCM is installed, nor whether this mod's menu is actually enabled.</remarks>
        private static bool InitializedGMCM { get; set; } = false;

        /// <summary>A SMAPI event that initializes this mod's GMCM option menu if possible.</summary>
        /// <remarks>This SMAPI event type was used to avoid issues with GMCM readiness timing. Another event may be more appropriate with later GMCM versions.</remarks>
        public static void Enable(object sender, RenderedActiveMenuEventArgs e)
        {
            if (InitializedGMCM)
                return; //do nothing
            InitializedGMCM = true; //prevent this method running multiple times

            try
            {
                var api = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu"); //attempt to get GMCM's API instance

                if (api == null)
                    return;

                //create this mod's menu
                api.Register
                (
                    mod: ModManifest,
                    reset: () => Config = new ModConfig(),
                    save: () => Helper.WriteConfig(Config),
                    titleScreenOnly: false
                );

                //register an option for each config setting
                api.AddBoolOption
                (
                    mod: ModManifest,
                    name: () => Helper.Translation.Get("EnableTrackersWithoutProfession.Name"),
                    tooltip: () => Helper.Translation.Get("EnableTrackersWithoutProfession.Desc"),
                    getValue: () => Config.EnableTrackersWithoutProfession,
                    setValue: value => Config.EnableTrackersWithoutProfession = value
                );

                api.AddBoolOption
                (
                    mod: ModManifest,
                    name: () => Helper.Translation.Get("ReplaceTrackersWithForageIcons.Name"),
                    tooltip: () => Helper.Translation.Get("ReplaceTrackersWithForageIcons.Desc"),
                    getValue: () => Config.ReplaceTrackersWithForageIcons,
                    setValue: value =>
                    {
                        Config.ReplaceTrackersWithForageIcons = value;
                        ModEntry.Instance.LoadTrackerSprites(); //update sprites based on the new value
                    }
                );

                api.AddBoolOption
                (
                    mod: ModManifest,
                    name: () => Helper.Translation.Get("DrawBehindInterface.Name"),
                    tooltip: () => Helper.Translation.Get("DrawBehindInterface.Desc"),
                    getValue: () => Config.DrawBehindInterface,
                    setValue: value => Config.DrawBehindInterface = value
                );

                api.AddNumberOption
                (
                    mod: ModManifest,
                    name: () => Helper.Translation.Get("TrackerPixelScale.Name"),
                    tooltip: () => Helper.Translation.Get("TrackerPixelScale.Desc"),
                    getValue: () => Config.TrackerPixelScale,
                    setValue: value => Config.TrackerPixelScale = value
                );

                api.AddBoolOption
                (
                    mod: ModManifest,
                    name: () => Helper.Translation.Get("EnableTrackingIndoors.Name"),
                    tooltip: () => Helper.Translation.Get("EnableTrackingIndoors.Desc"),
                    getValue: () => Config.EnableTrackingIndoors,
                    setValue: value => Config.EnableTrackingIndoors = value
                );

                api.AddBoolOption
                (
                    mod: ModManifest,
                    name: () => Helper.Translation.Get("TrackDefaultForage.Name"),
                    tooltip: () => Helper.Translation.Get("TrackDefaultForage.Desc"),
                    getValue: () => Config.TrackDefaultForage,
                    setValue: value => Config.TrackDefaultForage = value
                );

                api.AddBoolOption
                (
                    mod: ModManifest,
                    name: () => Helper.Translation.Get("TrackArtifactSpots.Name"),
                    tooltip: () => Helper.Translation.Get("TrackArtifactSpots.Desc"),
                    getValue: () => Config.TrackArtifactSpots,
                    setValue: value => Config.TrackArtifactSpots = value
                );

                api.AddBoolOption
                (
                    mod: ModManifest,
                    name: () => Helper.Translation.Get("TrackPanningSpots.Name"),
                    tooltip: () => Helper.Translation.Get("TrackPanningSpots.Desc"),
                    getValue: () => Config.TrackPanningSpots,
                    setValue: value => Config.TrackPanningSpots = value
                );

                api.AddBoolOption
                (
                    mod: ModManifest,
                    name: () => Helper.Translation.Get("TrackSpringOnions.Name"),
                    tooltip: () => Helper.Translation.Get("TrackSpringOnions.Desc"),
                    getValue: () => Config.TrackSpringOnions,
                    setValue: value => Config.TrackSpringOnions = value
                );

                api.AddBoolOption
                (
                    mod: ModManifest,
                    name: () => Helper.Translation.Get("TrackBerryBushes.Name"),
                    tooltip: () => Helper.Translation.Get("TrackBerryBushes.Desc"),
                    getValue: () => Config.TrackBerryBushes,
                    setValue: value => Config.TrackBerryBushes = value
                );

                api.AddBoolOption
                (
                    mod: ModManifest,
                    name: () => Helper.Translation.Get("TrackWalnutBushes.Name"),
                    tooltip: () => Helper.Translation.Get("TrackWalnutBushes.Desc"),
                    getValue: () => Config.TrackWalnutBushes,
                    setValue: value => Config.TrackWalnutBushes = value
                );


                if (Monitor.IsVerbose)
                    Monitor.Log($"GMCM menu setup complete.", LogLevel.Trace);
            }
            catch (Exception ex)
            {
                Monitor.Log($"Error while setting up in-game menu with Generic Mod Config Menu (GMCM). The menu might be missing or incomplete. Full error message: \n{ex}", LogLevel.Warn);
            }
        }
    }
}