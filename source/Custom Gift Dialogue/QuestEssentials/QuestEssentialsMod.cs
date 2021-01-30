/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/StardewMods
**
*************************************************/

using Harmony;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using QuestFramework.Api;
using QuestEssentials.Quests;
using QuestEssentials.Framework;
using Patches = QuestEssentials.Framework.Patches;

namespace QuestEssentials
{
    /// <summary>The mod entry point.</summary>
    public class QuestEssentialsMod : Mod
    {
        internal static IMonitor ModMonitor { get; private set; }

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            ModMonitor = this.Monitor;
            helper.Events.GameLoop.GameLaunched += this.GameLoop_GameLaunched;
            helper.Events.GameLoop.DayEnding += this.GameLoop_DayEnding;
            helper.Events.Display.MenuChanged += this.Display_MenuChanged;

            var harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);

            harmony.Patch(
                original: AccessTools.Method(typeof(ShopMenu), nameof(ShopMenu.receiveLeftClick)),
                prefix: new HarmonyMethod(typeof(Patches), nameof(Patches.Before_receiveLeftClick))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(ShopMenu), nameof(ShopMenu.receiveRightClick)),
                prefix: new HarmonyMethod(typeof(Patches), nameof(Patches.Before_receiveRightClick))
            );
            harmony.Patch(
                original: AccessTools.Property(typeof(Farmer), nameof(Farmer.Money)).GetSetMethod(),
                prefix: new HarmonyMethod(typeof(Patches), nameof(Patches.Before_set_Money))
            );
        }

        private void Display_MenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu is DialogueBox)
            {
                QuestCheckers.CheckTalkQuests(Game1.currentSpeaker);
            }
        }

        private void GameLoop_DayEnding(object sender, DayEndingEventArgs e)
        {
            // Check item sell quests for shipping bin items
            foreach (ISalable item in Game1.getFarm().getShippingBin(Game1.player))
                QuestCheckers.CheckSellQuests(item);
        }

        private void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // IPurrplingCoreApi core = this.Helper.ModRegistry.GetApi<IPurrplingCoreApi>("PurrplingCat.PurrplingCore");
            IManagedQuestApi questApi = this.Helper
                .ModRegistry
                .GetApi<IQuestApi>("PurrplingCat.QuestFramework")
                .GetManagedApi(this.ModManifest);

            // core.Events.OnSellItem += this.OnSellItem;

            // Expose QF custom quest types
            questApi.ExposeQuestType<SellItemQuest>("SellItem");
            questApi.ExposeQuestType<EarnMoneyQuest>("EarnMoney");
            questApi.ExposeQuestType<TalkQuest>("Talk");
        }

        /*private void OnSellItem(object sender, PurrplingCore.Events.SellItemArgs e)
        {
            CheckSellQuests(e.SoldItem);
        }*/
    }
}