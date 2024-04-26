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

            api.AddBoolOption(ModManifest, () => IConfig.ShowTotalValueBanner, (v) => IConfig.ShowTotalValueBanner = v, () => "Show Total Value", () => "Show a banner at the top of the shipping bin inventory with the total value of all items");
            api.AddBoolOption(ModManifest, () => IConfig.ShowTotalValueMiniBin, (v) => IConfig.ShowTotalValueMiniBin = v, () => "Show Total Value (Mini bin)", () => "Show the same banner as on the regular inventory in the mini shipping bin");
            api.AddBoolOption(ModManifest, () => IConfig.InvertScrollWheelDirection, (v) => IConfig.InvertScrollWheelDirection = v, () => "Invert Scroll Wheel", () => "Invert the direction of the scroll wheel when scrolling through the inventory");
        }
    }

    public interface IGMCMApi
    {
        void Register(IManifest mod, Action reset, Action save, bool titleScreenOnly = false);

        void AddBoolOption(IManifest mod, Func<bool> getValue, Action<bool> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null);
    }
}
