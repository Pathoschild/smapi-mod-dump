/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using Harmony;
using MailFrameworkMod;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;

namespace MailServicesMod
{
    /// <summary>The mod entry class loaded by SMAPI.</summary>
    public class MailServicesModEntry : Mod
    {
        public static IModHelper ModHelper;
        public static IMonitor ModMonitor;

        /// <summary>
        /// The mod entry point, called after the mod is first loaded.
        /// Here it loads the custom event handlers for the start of the day, after load and after returning to the title screen.
        /// </summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            ModHelper = helper;
            ModMonitor = Monitor;

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        }

        /// <summary>Raised after the game is launched, right before the first update tick. This happens once per game session (unrelated to loading saves). All mods are loaded and initialized at this point, so this is a good time to set up mod integrations.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            new DataLoader(ModHelper);
            var harmony = HarmonyInstance.Create("Digus.MailServiceMod");

            harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.blacksmith)),
                postfix: new HarmonyMethod(typeof(ToolUpgradeOverrides), nameof(ToolUpgradeOverrides.blacksmith))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.addItemsByMenuIfNecessary)),
                prefix: new HarmonyMethod(typeof(ToolUpgradeOverrides), nameof(ToolUpgradeOverrides.addItemsByMenuIfNecessary))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.mailbox)),
                prefix: new HarmonyMethod(typeof(ToolUpgradeOverrides), nameof(ToolUpgradeOverrides.mailbox))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.mailbox)),
                prefix: new HarmonyMethod(typeof(ItemShipmentQuestOverrides), nameof(ItemShipmentQuestOverrides.mailbox))
            );
        }
    }
}
