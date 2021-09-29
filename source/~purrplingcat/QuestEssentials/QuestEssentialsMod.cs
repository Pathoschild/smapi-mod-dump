/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/StardewMods
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using QuestFramework.Api;
using QuestEssentials.Quests;
using QuestEssentials.Framework;
using Microsoft.Xna.Framework;
using QuestEssentials.Quests.Messages;
using Patches = QuestEssentials.Framework.Patches;
using StardewValley.Tools;
using StardewValley.Objects;

namespace QuestEssentials
{
    /// <summary>The mod entry point.</summary>
    public class QuestEssentialsMod : Mod
    {
        internal static IMonitor ModMonitor { get; private set; }
        internal static IModHelper ModHelper { get; private set; }
        internal static IManagedQuestApi QuestApi { get; private set; }

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            ModMonitor = this.Monitor;
            ModHelper = helper;
            helper.Events.GameLoop.GameLaunched += this.GameLoop_GameLaunched;
            helper.Events.GameLoop.DayEnding += this.GameLoop_DayEnding;
            helper.Events.GameLoop.UpdateTicked += this.GameLoop_UpdateTicked;
            helper.Events.Player.Warped += this.Player_Warped;
            helper.Events.Display.MenuChanged += this.Display_MenuChanged;

            var harmony = new Harmony(this.ModManifest.UniqueID);

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
            harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.onGiftGiven)),
                postfix: new HarmonyMethod(typeof(Patches), nameof(Patches.After_onGiftGiven))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(NPC), nameof(NPC.tryToReceiveActiveObject)),
                prefix: new HarmonyMethod(typeof(Patches), nameof(Patches.Before_tryToReceiveActiveObject))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(NPC), nameof(NPC.hasTemporaryMessageAvailable)),
                postfix: new HarmonyMethod(typeof(Patches), nameof(Patches.After_hasTemporaryMessageAvailable))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(FishingRod), nameof(FishingRod.playerCaughtFishEndFunction)),
                postfix: new HarmonyMethod(typeof(Patches), nameof(Patches.After_playerCaughtFishEndFunction))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(CrabPot), nameof(CrabPot.checkForAction)),
                prefix: new HarmonyMethod(typeof(Patches), nameof(Patches.Before_checkForAction))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.checkAction)),
                prefix: new HarmonyMethod(typeof(Patches), nameof(Patches.Before_LocationCheckAction))
            );
        }

        private void Player_Warped(object sender, WarpedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            QuestApi.CheckForQuestComplete<AdventureQuest>(
                new PlayerMovedMessage(
                    location: e.NewLocation,
                    position: e.Player.Position,
                    tilePosition: e.Player.getTileLocationPoint(),
                    trigger: "PlayerWarped"));
        }

        private GameLocation _lastLocation;
        private Point _lastTilePosition;
        private void GameLoop_UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady || !Context.CanPlayerMove || Game1.player.isMoving())
                return;

            if (this._lastLocation != Game1.player.currentLocation || this._lastTilePosition != Game1.player.getTileLocationPoint())
            {
                this._lastLocation = Game1.player.currentLocation;
                this._lastTilePosition = Game1.player.getTileLocationPoint();

                QuestApi.CheckForQuestComplete<AdventureQuest>(
                    new PlayerMovedMessage(
                        location: this._lastLocation,
                        position: Game1.player.getStandingPosition(),
                        tilePosition: this._lastTilePosition));
            }
        }

        private void Display_MenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu is DialogueBox)
            {
                QuestCheckers.CheckTalkQuests(Game1.player, Game1.currentSpeaker);
            }
        }

        private void GameLoop_DayEnding(object sender, DayEndingEventArgs e)
        {
            // Check item sell quests for shipping bin items
            foreach (Item item in Game1.getFarm().getShippingBin(Game1.player))
            {
                int itemPrice = item is StardewValley.Object obj ? obj.sellToStorePrice(-1L) : item.salePrice();

                QuestCheckers.CheckSellQuests(item, itemPrice * item.Stack, ship: true);
            }
        }

        private void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            IManagedQuestApi questApi = this.Helper
                .ModRegistry
                .GetApi<IQuestApi>("PurrplingCat.QuestFramework")
                .GetManagedApi(this.ModManifest);

            // Expose QF custom quest types
            questApi.ExposeQuestType<SellItemQuest>("SellItem");
            questApi.ExposeQuestType<EarnMoneyQuest>("EarnMoney");
            questApi.ExposeQuestType<TalkQuest>("Talk");
            questApi.ExposeQuestType<AdventureQuest>("Adventure");
            questApi.ExposeQuestType<CollectQuest>("Collect");

            QuestApi = questApi;
        }
    }
}