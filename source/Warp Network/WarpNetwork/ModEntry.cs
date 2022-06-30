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
using WarpNetwork.api;
using WarpNetwork.models;

namespace WarpNetwork
{
    class ModEntry : Mod
    {
        //const
        public static readonly string pathLocData = PathUtilities.NormalizeAssetName("Data/WarpNetwork/Destinations");
        public static readonly string pathItemData = PathUtilities.NormalizeAssetName("Data/WarpNetwork/WarpItems");
        public static readonly string pathIcons = PathUtilities.NormalizeAssetName("Data/WarpNetwork/Icons");
        internal static readonly HashSet<string> knownIcons = new(new[] {"DEFAULT", "farm", "mountain", "island", "desert", "beach", "RETURN"});

        //main
        internal static Config config;
        internal static IModHelper helper;
        internal static IMonitor monitor;
        internal static IDynamicGameAssets dgaAPI = null;
        internal static ITranslationHelper i18n;
        public static API api = new();
        public override void Entry(IModHelper helper)
        {
            config = helper.ReadConfig<Config>();
            ModEntry.helper = helper;
            monitor = Monitor;
            i18n = helper.Translation;
            helper.ConsoleCommands.Add("warpnet", "Master command for Warp Network mod. Use 'warpnet' or 'warpnet help' to see a list of subcommands.", CommandHandler.Main);
            helper.Events.Content.AssetRequested += DataPatcher.AssetRequested;
            helper.Events.Content.AssetRequested += LoadAssets;
            helper.Events.GameLoop.GameLaunched += GameLaunched;
            helper.Events.Input.ButtonPressed += ItemHandler.ButtonPressed;
            helper.Events.Player.Warped += ObeliskPatch.MoveAfterWarp;
            SpaceEvents.ActionActivated += WarpHandler.HandleAction;
        }
        public void GameLaunched(object sender, GameLaunchedEventArgs ev)
        {
            if (helper.ModRegistry.IsLoaded("spacechase0.DynamicGameAssets"))
                dgaAPI = helper.ModRegistry.GetApi<IDynamicGameAssets>("spacechase0.DynamicGameAssets");
            config.RegisterModConfigMenu(ModManifest);
            CPIntegration.AddTokens(ModManifest);
        }
        public override object GetApi() => api;
        private void LoadAssets(object _, AssetRequestedEventArgs ev)
        {
            if (ev.Name.IsEquivalentTo(pathLocData))
                ev.LoadFromModFile<Dictionary<string, WarpLocation>>("assets/Destinations.json", AssetLoadPriority.Medium);
            else if (ev.Name.IsEquivalentTo(pathItemData))
                ev.LoadFromModFile<Dictionary<string, WarpItem>>("assets/WarpItems.json", AssetLoadPriority.Medium);
            else if (ev.Name.StartsWith(pathIcons))
            {
                var name = ev.Name.ToString().WithoutPath(pathIcons);
                if (knownIcons.Contains(name))
                    ev.LoadFromModFile<Texture2D>($"assets/icons/{name}.png", AssetLoadPriority.Low);
                else
                    ev.LoadFrom(() => helper.GameContent.Load<Texture2D>($"{pathIcons}/DEFAULT"), AssetLoadPriority.Low);
            }
        }
    }
}
