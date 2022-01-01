/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/WarpNetwork
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using SpaceCore.Events;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using System.Collections.Generic;
using System.IO;
using WarpNetwork.api;
using WarpNetwork.models;

namespace WarpNetwork
{
    class ModEntry : Mod, IAssetLoader
    {
        //const
        public static readonly string pathLocData = PathUtilities.NormalizeAssetName("Data/WarpNetwork/Destinations");
        public static readonly string pathItemData = PathUtilities.NormalizeAssetName("Data/WarpNetwork/WarpItems");
        public static readonly string pathIcons = PathUtilities.NormalizeAssetName("Data/WarpNetwork/Icons");

        //main
        internal static Config config;
        internal static IModHelper helper;
        internal static IMonitor monitor;
        internal static IDynamicGameAssets dgaAPI = null;
        public static API api = new();
        public override void Entry(IModHelper helper)
        {
            config = helper.ReadConfig<Config>();
            ModEntry.helper = helper;
            monitor = Monitor;
            helper.ConsoleCommands.Add("warpnet", "Master command for Warp Network mod. Use 'warpnet' or 'warpnet help' to see a list of subcommands.", CommandHandler.Main);
            helper.Content.AssetEditors.Add(new DataPatcher());
            helper.Events.GameLoop.GameLaunched += GameLaunched;
            helper.Events.Input.ButtonPressed += ItemHandler.ButtonPressed;
            helper.Events.Player.Warped += ObeliskPatch.MoveAfterWarp;
            SpaceEvents.ActionActivated += WarpHandler.HandleAction;
        }
        public void GameLaunched(object sender, GameLaunchedEventArgs ev)
        {
            if (helper.ModRegistry.IsLoaded("spacechase0.DynamicGameAssets"))
                dgaAPI = helper.ModRegistry.GetApi<IDynamicGameAssets>("spacechase0.DynamicGameAssets");
            config.RegisterModConfigMenu(Helper, ModManifest);
            CPIntegration.AddTokens(ModManifest);
        }
        public override object GetApi()
        {
            return api;
        }
        //loading
        public bool CanLoad<T>(IAssetInfo asset)
        {
            return (
                asset.AssetNameEquals(pathLocData) ||
                asset.AssetNameEquals(pathItemData) ||
                asset.AssetNameEquals(pathIcons + "/DEFAULT") ||
                asset.AssetNameEquals(pathIcons + "/farm") ||
                asset.AssetNameEquals(pathIcons + "/mountain") ||
                asset.AssetNameEquals(pathIcons + "/island") ||
                asset.AssetNameEquals(pathIcons + "/desert") ||
                asset.AssetNameEquals(pathIcons + "/beach")
            );
        }
        public T Load<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals(pathItemData))
            {
                Dictionary<string, WarpItem> items = Helper.Content.Load<Dictionary<string, WarpItem>>(Path.Combine("assets", "WarpItems.json"));
                DataPatcher.AddApiItems(items);
                return (T)(object)items;
            }
            else if (asset.AssetNameEquals(pathLocData))
            {
                Dictionary<string, WarpLocation> locs = Helper.Content.Load<Dictionary<string, WarpLocation>>(Path.Combine("assets", "Destinations.json"));
                DataPatcher.EditLocationsEnabled(locs);
                DataPatcher.AddApiLocs(locs);
                DataPatcher.TranslateDefaultWarps(locs);
                return (T)(object)locs;
            }
            else if (asset.AssetName.StartsWith(pathIcons))
            {
                return (T)(object)Helper.Content.Load<Texture2D>(Path.Combine("assets", "icons", asset.AssetName.Split(PathUtilities.PreferredAssetSeparator)[3] + ".png"));
            }
            return default;
        }
    }
}
