/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jslattery26/stardew_mods
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using HarmonyLib;
using StardewModdingAPI.Events;

#nullable enable

namespace ChestPoolingV2
{
    public class ChestPoolingV2Mod : Mod
    {
        public static IMonitor? StaticMonitor { get; private set; }
        internal static ChestPoolingV2Mod? ModInstance { get; private set; }
        public static void Log(string s, LogLevel l = LogLevel.Trace) => StaticMonitor?.Log(s, l);

        public const string ModUniqueId = "jslattery26.ChestPoolingV2";

        internal const string UserConfigFilename = "config.json";
        private ModConfig? Config;

        public bool Disabled = false;

        public override void Entry(IModHelper helper)
        {
            StaticMonitor = Monitor;
            ChestPatches.Initialize(Monitor);
            ModInstance = this;
            Config = helper.ReadConfig<ModConfig>();



            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.Input.ButtonsChanged += OnButtonsChanged;

            Harmony Harmony = new(ModManifest.UniqueID);
            Harmony.Patch(
                original: AccessTools.Method(typeof(Chest), nameof(Chest.grabItemFromInventory)),
                prefix: new HarmonyMethod(typeof(ChestPatches), nameof(ChestPatches.Chest_grabItemFromInventory_Prefix))
            );

        }


        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            GenericModConfigMenuIntegration.Register(ModManifest, Helper.ModRegistry, Monitor,
                getConfig: () => Config,
                reset: () => Config = new(),
                save: () =>
                {
                    if (Config != null)
                    {
                        Helper.WriteConfig(Config);
                    }
                }
            );
        }

        private void OnButtonsChanged(object? sender, ButtonsChangedEventArgs e)
        {
            if (Config != null && Config.Keys.DisablePoolingToggle.JustPressed())
                ToggleDisable();

        }

        private void ToggleDisable()
        {
            Disabled = !Disabled;
            Game1.addHUDMessage(new HUDMessage($"Chest Pooling is {(Disabled ? "disabled." : "enabled.")}", HUDMessage.newQuest_type) { timeLeft = 1000 });
        }

        public static bool SearchForBestChest(Chest _, List<Chest> chestList, Item itemRemoved, Farmer who)
        {
            Log($"Checking {chestList.Count} chests");

            Chest? bestChest = chestList.GetBestChest(itemRemoved);
            if (bestChest == null)
            {
                Log("No best chest found, keeping in current chest.");
                return true;
            }
            Log($"Best chest found: {bestChest.Items.Select(i => i.Name).Aggregate((a, b) => a + ", " + b)}");
            bestChest.GrabItemFromInventoryFromOtherChest(itemRemoved, who);
            Log("Overrode default behavior..");
            return false;
        }
    }
}
