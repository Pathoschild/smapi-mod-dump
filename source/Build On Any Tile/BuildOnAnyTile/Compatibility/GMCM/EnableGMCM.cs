/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/BuildOnAnyTile
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;

namespace BuildOnAnyTile
{
    public partial class ModEntry : Mod
    {
        /// <summary>True if the method <see cref="EnableGMCM"/> has already run. Does NOT indicate whether GMCM is available or this mod's menu was successfully enabled.</summary>
        private static bool InitializedGMCM { get; set; } = false;

        // <summary>A SMAPI GameLaunched event that enables GMCM support if that mod is available.</summary>
        public void EnableGMCM(object sender, RenderedActiveMenuEventArgs e)
        {
            if (InitializedGMCM)
                return; //do nothing
            InitializedGMCM = true; //don't run this more than once

            try
            {
                var api = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu"); //attempt to get GMCM's API instance

                if (api == null) //if the API is not available
                    return;

                //register "revert to default" and "write" methods for this mod's config
                api.Register
                (
                    mod: ModManifest,
                    reset: () => Config = new ModConfig(),
                    save: () => Helper.WriteConfig(Config),
                    titleScreenOnly: false
                );

                //register an option for each of this mod's config settings
                api.AddBoolOption
                (
                    mod: ModManifest,
                    getValue: () => Config.BuildOnAllTerrainFeatures,
                    setValue: (bool val) => Config.BuildOnAllTerrainFeatures = val,
                    name: () => Helper.Translation.Get("BuildOnAllTerrainFeatures.Name"),
                    tooltip: () => Helper.Translation.Get("BuildOnAllTerrainFeatures.Description")
                );

                api.AddBoolOption
                (
                    mod: ModManifest,
                    getValue: () => Config.BuildOnOtherBuildings,
                    setValue: (bool val) => Config.BuildOnOtherBuildings = val,
                    name: () => Helper.Translation.Get("BuildOnOtherBuildings.Name"),
                    tooltip: () => Helper.Translation.Get("BuildOnOtherBuildings.Description")
                );

                api.AddBoolOption
                (
                    mod: ModManifest,
                    getValue: () => Config.BuildOnWater,
                    setValue: (bool val) => Config.BuildOnWater = val,
                    name: () => Helper.Translation.Get("BuildOnWater.Name"),
                    tooltip: () => Helper.Translation.Get("BuildOnWater.Description")
                );

                api.AddBoolOption
                (
                    mod: ModManifest,
                    getValue: () => Config.BuildOnImpassableTiles,
                    setValue: (bool val) => Config.BuildOnImpassableTiles = val,
                    name: () => Helper.Translation.Get("BuildOnImpassableTiles.Name"),
                    tooltip: () => Helper.Translation.Get("BuildOnImpassableTiles.Description")
                );

                api.AddBoolOption
                (
                    mod: ModManifest,
                    getValue: () => Config.BuildOnNoFurnitureTiles,
                    setValue: (bool val) => Config.BuildOnNoFurnitureTiles = val,
                    name: () => Helper.Translation.Get("BuildOnNoFurnitureTiles.Name"),
                    tooltip: () => Helper.Translation.Get("BuildOnNoFurnitureTiles.Description")
                );

                api.AddBoolOption
                (
                    mod: ModManifest,
                    getValue: () => Config.BuildOnCaveAndShippingZones,
                    setValue: (bool val) => Config.BuildOnCaveAndShippingZones = val,
                    name: () => Helper.Translation.Get("BuildOnCaveAndShippingZones.Name"),
                    tooltip: () => Helper.Translation.Get("BuildOnCaveAndShippingZones.Description")
                );
            }
            catch (Exception ex)
            {
                Monitor.Log($"An error happened while loading this mod's GMCM options menu. Its menu might be missing or fail to work. The auto-generated error message has been added to the log.", LogLevel.Warn);
                Monitor.Log($"----------", LogLevel.Trace);
                Monitor.Log($"{ex.ToString()}", LogLevel.Trace);
            }
        }
    }
}
