/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NermNermNerm/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.GameData.Buildings;
using StardewValley.GameData.GarbageCans;
using StardewValley.GameData.Objects;
using StardewValley.GameData.Tools;

namespace NermNermNerm.Stardew.QuestableTractor
{
    public class ModEntry
        : Mod, ISimpleLog
    {
        private IReadOnlyCollection<BaseQuestController> QuestControllers = null!;
        private LoaderQuestController loaderQuestController = null!;
        private ScytheQuestController scytheQuestController = null!;
        private SeederQuestController seederQuestController = null!;
        private BorrowHarpoonQuestController borrowHarpoonQuestController = null!;
        private RestoreTractorQuestController restoreTractorQuestController = null!;

        public WatererQuestController WatererQuestController = null!;
        public BorrowHarpoonQuestController BorrowHarpoonQuestController => this.borrowHarpoonQuestController;
        public RestoreTractorQuestController RestoreTractorQuestController => this.restoreTractorQuestController;

        public const string SpritesPseudoPath = "Mods/NermNermNerm/QuestableTractor/Sprites";

        public Harmony Harmony = null!;
        internal readonly TractorModConfig TractorModConfig;

        public static ModEntry Instance = null!; // Set in ModEntry

        public ModEntry()
        {
            this.TractorModConfig = new TractorModConfig(this);
        }

        public PetFindsThings PetFindsThings = new PetFindsThings();


        public override void Entry(IModHelper helper)
        {
            Instance = this;
            this.Harmony = new Harmony(this.ModManifest.UniqueID);

            this.PetFindsThings.Entry(this);
            this.loaderQuestController = new LoaderQuestController(this);
            this.scytheQuestController = new ScytheQuestController(this);
            this.seederQuestController = new SeederQuestController(this);
            this.WatererQuestController = new WatererQuestController(this);
            this.borrowHarpoonQuestController = new BorrowHarpoonQuestController(this);
            this.restoreTractorQuestController = new RestoreTractorQuestController(this);
            this.QuestControllers = new List<BaseQuestController> { this.loaderQuestController, this.scytheQuestController, this.seederQuestController, this.WatererQuestController, this.borrowHarpoonQuestController, this.restoreTractorQuestController };

            this.Helper.Events.Content.AssetRequested += this.OnAssetRequested;
            this.Helper.Events.GameLoop.OneSecondUpdateTicked += this.GameLoop_OneSecondUpdateTicked;
            this.Helper.Events.GameLoop.SaveLoaded += (_, _) =>
            {
                this.UpdateTractorModConfig();
                BaseQuest.ClearOldNews();

                // Clean up an entry in modData that was added in 1.1.4 that's no longer a thing.
                Game1.getFarm()?.modData.Remove("PetFindsThings.InterestingTiles");
            };
            this.Helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            this.Helper.Events.GameLoop.DayEnding += this.OnDayEnding;

            this.Helper.ConsoleCommands.Add(
                "fixqt",
                "Fixes Questable Tractor - It finds all the objects buried on the farm and adds them to our inventory.  It starts the main tractor quest and the quests for the two hidden parts.  It replaces any missing quest items and deletes excess quest items.  Note this command does not look at or alter chests or other players' inventories.  You can run this command again to get rid of the duplicate if you need to.",
                this.FixItAll);
        }

        private void FixItAll(string cmd, string[] args)
        {
            if (Game1.player is null || !Game1.hasLoadedGame)
            {
                this.LogInfo("Cannot run command - the game isn't loaded/started yet.");
                return;
            }

            if (!Game1.IsMasterGame)
            {
                this.LogInfo("Cannot run command - only the master player in a multiplayer game can run this command.");
                return;
            }

            if (Game1.player.freeSpotsInInventory() < 5)
            {
                this.LogInfo("Cannot run command - need at least 5 free spots in your inventory.");
                return;
            }

            foreach (var qc in this.QuestControllers)
            {
                qc.Fix();
            }
        }

        public void WriteToLog(string message, LogLevel level, bool isOnceOnly)
        {
            if (isOnceOnly)
            {
                this.Monitor.LogOnce(message, level);
            }
            else
            {
                this.Monitor.Log(message, level);
            }
        }

        public void UpdateTractorModConfig()
        {
            this.TractorModConfig.SetConfig(
                isHoeUnlocked: true, // <- comes stock
                isLoaderUnlocked: this.loaderQuestController.IsCompletedByMasterPlayer,
                isHarvesterUnlocked: this.scytheQuestController.IsCompletedByMasterPlayer,
                isSpreaderUnlocked: this.seederQuestController.IsCompletedByMasterPlayer,
                isWatererUnlocked: this.WatererQuestController.IsCompletedByMasterPlayer);
        }

        private void GameLoop_OneSecondUpdateTicked(object? sender, OneSecondUpdateTickedEventArgs e)
        {
            if (!Game1.hasLoadedGame)
            {
                return;
            }

            if (Context.IsMainPlayer)
            {
                bool IsPlayerInGarage(Character c, Stable b)
                    => b.intersects(new Rectangle(new Point((int)c.Position.X, (int)c.Position.Y - 128), new Point(64, 128)));

                if (Game1.player is not null
                    && Game1.player.IsMainPlayer
                    && Game1.player.currentLocation is Farm
                    && Game1.player.CurrentItem is not null
                    && Game1.player.currentLocation.buildings
                        .OfType<Stable>()
                        .Where(s => s.buildingType.Value == TractorModConfig.GarageBuildingId)
                        .Any(s => IsPlayerInGarage(Game1.player, s)))
                {
                    foreach (var qc in this.QuestControllers)
                    {
                        qc.PlayerIsInGarage(Game1.player.CurrentItem);
                        if (Game1.player.CurrentItem == null)
                        {
                            break;
                        }
                    }
                }
            }
        }

        [EventPriority(EventPriority.Low)] // Causes our OnDayStarted to come after TractorMod's, which does not set EventPriority
        public void OnDayStarted(object? sender, DayStartedEventArgs e)
        {
            foreach (var qc in this.QuestControllers)
            {
                // It's up to the QuestControllers to decide if their stuff matters for the current player.
                qc.OnDayStarted();
            }

            if (Game1.IsMasterGame)
            {
                // Farmhands aren't grandpa's grandchildren, thus they shouldn't get the same stuff.
                this.SetupMissingPartConversations();
            }

            // Every player should get this treatment.
            this.TractorModConfig.OnDayStarted();
        }

        private void SetupMissingPartConversations()
        {
            // Our stuff recurs every week for 4 days out of the week.  Delay until after the
            // first week so that the introductions quest runs to completion.  Perhaps it
            // would be better to delay until all the villagers we care about have been greeted.
            if (Game1.Date.DayOfWeek != DayOfWeek.Sunday)
            {
                return;
            }

            // A case could be made to having code that removes these conversation keys as
            // things get found, but maybe it'd be better to figure that it takes a while for
            // word to get around...  Although there might be some awkward dialogs with
            // townspeople directly involved in the quest.
            if (!this.restoreTractorQuestController.IsStarted)
            {
                this.LogTrace($"Added conversation key: {ConversationKeys.TractorNotFound}");
                Game1.player.activeDialogueEvents.Add(ConversationKeys.TractorNotFound, 4);
            }
            else
            {
                string[] possibleHintTopics = new BaseQuestController[] { this.loaderQuestController, this.scytheQuestController, this.WatererQuestController, this.seederQuestController }
                    .Where(qc => !qc.IsStarted && qc.HintTopicConversationKey is not null)
                    .Select(qc => qc.HintTopicConversationKey!).ToArray();
                if (possibleHintTopics.Any())
                {
                    string keyOfTheDay = possibleHintTopics[Game1.random.Next(possibleHintTopics.Length)];
                    this.LogTrace($"Added conversation key: {keyOfTheDay}");
                    Game1.player.activeDialogueEvents.Add(keyOfTheDay, 4);
                }
                else
                {
                    this.LogTraceOnce("All quests are in-progress, no hint topics given.");
                }
            }
        }

        /// <summary>
        ///   Handle end-of-day by removing stuff to do with our mod that would be difficult for the player to remove
        ///   if they uninstalled the mod.
        /// </summary>
        private void OnDayEnding(object? sender, DayEndingEventArgs e)
        {
            // ? This should be needless if !MasterGame
            Game1.getFarm().terrainFeatures.RemoveWhere(p => p.Value is DerelictTractorTerrainFeature);

            FakeQuest.RemoveAllFakeQuests(Game1.player);
        }

        [EventPriority(EventPriority.Low)] // Causes us to come after TractorMod's, which does not set EventPriority
        internal void OnAssetRequested(object? _, AssetRequestedEventArgs e)
        {
            // this.Monitor.Log($"OnAssetRequested({e.NameWithoutLocale.Name})");
            if (e.NameWithoutLocale.IsEquivalentTo(SpritesPseudoPath))
            {
                e.LoadFromModFile<Texture2D>("assets/Sprites.png", AssetLoadPriority.Exclusive);
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("Data/GarbageCans"))
            {
                e.Edit(editor => this.loaderQuestController.EditGarbageCanAsset(editor.GetData<GarbageCanData>()));
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("Data/Buildings"))
            {
                e.Edit(editor =>
                {
                    this.TractorModConfig.EditBuildings(editor.AsDictionary<string, BuildingData>().Data);
                });
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("Data/Objects"))
            {
                e.Edit(editor =>
                {
                    ObjectIds.EditAssets(editor.AsDictionary<string, ObjectData>().Data);
                });
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("Data/CraftingRecipes"))
            {
                // TODO: Make this recipe only available when its quest is running?
                e.Edit(editor =>
                {
                    IDictionary<string, string> recipes = editor.AsDictionary<string, string>().Data;
                    recipes["TractorMod.ScytheAttachment"] = $"{ObjectIds.BustedScythe} 1 {ObjectIds.ScythePart1} 1 {ObjectIds.ScythePart2} 1/Field/{ObjectIds.WorkingScythe}/false/default/";
                });
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("Data/Mail"))
            {
                e.Edit(editor =>
                {
                    MailKeys.EditAssets(editor.AsDictionary<string, string>().Data);
                });
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("Data/Tools"))
            {
                e.Edit(editor =>
                {
                    BorrowHarpoonQuestController.EditToolAssets(editor.AsDictionary<string, ToolData>().Data);
                });
            }
            else if (e.NameWithoutLocale.StartsWith("Characters/Dialogue/"))
            {
                e.Edit(editor =>
                {
                    var topics = editor.AsDictionary<string, string>().Data;
                    ConversationKeys.EditAssets(e.NameWithoutLocale, topics);
                });
            }
        }
    }
}
