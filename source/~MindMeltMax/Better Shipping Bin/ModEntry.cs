/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;

namespace BetterShipping
{
    internal class ModEntry : Mod
    {
        public static IModHelper IHelper;
        public static IMonitor IMonitor;
        public static Config IConfig;

        public override void Entry(IModHelper helper)
        {
            IHelper = Helper;
            IMonitor = Monitor;
            IConfig = Helper.ReadConfig<Config>();

            Helper.Events.Display.WindowResized += OnResize;
            Helper.Events.Multiplayer.ModMessageReceived += OnModMessageReceived;
            Helper.Events.GameLoop.GameLaunched += (_, _) => registerForGMCM();

            HarmonyPather.Init(Helper);
        }

        private void OnModMessageReceived(object? sender, ModMessageReceivedEventArgs e)
        {
            if (e.Type == "reloadItemsInBin")
                if (Game1.activeClickableMenu is BinMenuOverride menu)
                    menu.RenderItems();
        }

        private void OnResize(object? sender, WindowResizedEventArgs e)
        {
            if (Game1.activeClickableMenu is BinMenuOverride menu)
                Game1.activeClickableMenu = new BinMenuOverride(menu.Offset);
        }

        private void registerForGMCM()
        {
            var api = Helper.ModRegistry.GetApi<IGMCMApi>("spacechase0.GenericModConfigMenu");
            if (api is null)
                return;
            api.Register(ModManifest, () => IConfig = new(), () => Helper.WriteConfig(IConfig));

            api.AddBoolOption(ModManifest, () => IConfig.ShowTotalValueBanner, (v) => IConfig.ShowTotalValueBanner = v, () => Helper.Translation.Get("Config.TotalValue.Title"), () => Helper.Translation.Get("Config.TotalValue.Description"));
            api.AddBoolOption(ModManifest, () => IConfig.ShowTotalValueMiniBin, (v) => IConfig.ShowTotalValueMiniBin = v, () => Helper.Translation.Get("Config.MiniTotalValue.Title"), () => Helper.Translation.Get("Config.MiniTotalValue.Description"));
            api.AddBoolOption(ModManifest, () => IConfig.InvertScrollWheelDirection, (v) => IConfig.InvertScrollWheelDirection = v, () => Helper.Translation.Get("Config.InvertScroll.Title"), () => Helper.Translation.Get("Config.InvertScroll.Description"));
        }
    }

    public interface IGMCMApi
    {
        void Register(IManifest mod, Action reset, Action save, bool titleScreenOnly = false);

        void AddBoolOption(IManifest mod, Func<bool> getValue, Action<bool> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null);
    }
}
