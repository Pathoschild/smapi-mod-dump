/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/QuestFramework
**
*************************************************/

using PurrplingCore.Patching;
using QuestFramework.Api;
using QuestFramework.Extensions;
using QuestFramework.Framework;
using QuestFramework.Framework.Bridges;
using QuestFramework.Framework.ContentPacks;
using QuestFramework.Framework.Controllers;
using QuestFramework.Framework.Events;
using QuestFramework.Framework.Hooks;
using QuestFramework.Framework.Menus;
using QuestFramework.Framework.Networing;
using QuestFramework.Framework.Stats;
using QuestFramework.Framework.Store;
using QuestFramework.Framework.Watchdogs;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QuestFramework
{
    /// <summary>The mod entry point.</summary>
    public class QuestFrameworkMod : Mod
    {
        private bool hasSaveLoaded = false;
        private bool hasInitialized = false;
        private bool hasInitMessageArrived = false;

        internal State Status { get; private set; }
        internal Config Config { get; private set; }
        internal Bridge Bridge { get; private set; }
        internal QuestManager QuestManager { get; private set; }
        internal QuestStateStore QuestStateStore { get; private set; }
        internal StatsManager StatsManager { get; private set; }
        internal ConditionManager ConditionManager { get; private set; }
        internal QuestOfferManager QuestOfferManager { get; private set; }
        internal Loader ContentPackLoader { get; private set; }
        internal QuestController QuestController { get; private set; }
        internal MailController MailController { get; private set; }
        internal CustomBoardController CustomBoardController { get; private set; }
        internal QuestLogWatchdog QuestLogWatchdog { get; private set; }
        internal NetworkOperator NetworkOperator { get; private set; }
        internal EventManager EventManager { get; private set; }

        internal static QuestFrameworkMod Instance { get; private set; }
        internal static Multiplayer Multiplayer { get; private set; }
        internal GamePatcher Patcher { get; private set; }

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Instance = this;
            Multiplayer = helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();

            this.Config = helper.ReadConfig<Config>();
            this.Bridge = new Bridge(helper.ModRegistry, this.Config.DebugMode);
            this.EventManager = new EventManager(this.Monitor);
            this.QuestManager = new QuestManager(this.Monitor);
            this.QuestStateStore = new QuestStateStore(helper.Data, this.Monitor);
            this.CustomBoardController = new CustomBoardController(helper.Events);
            this.StatsManager = new StatsManager(helper.Multiplayer, helper.Data);
            this.ConditionManager = new ConditionManager(this.Monitor);
            this.QuestOfferManager = new QuestOfferManager(this.ConditionManager, this.QuestManager);
            this.ContentPackLoader = new Loader(this.Monitor, this.QuestManager, this.QuestOfferManager, this.ConditionManager, this.CustomBoardController);
            this.QuestController = new QuestController(this.QuestManager, this.QuestOfferManager, this.Monitor);
            this.MailController = new MailController(this.QuestManager, this.QuestOfferManager, this.Monitor);
            this.QuestLogWatchdog = new QuestLogWatchdog(helper.Events, this.EventManager, this.StatsManager, this.Monitor);
            this.NetworkOperator = new NetworkOperator(
                helper: helper.Multiplayer,
                events: helper.Events.Multiplayer,
                questStateStore: this.QuestStateStore,
                statsManager: this.StatsManager,
                questController: this.QuestController,
                modManifest: this.ModManifest,
                monitor: this.Monitor);

            helper.Content.AssetEditors.Add(this.QuestController);
            helper.Content.AssetEditors.Add(this.MailController);

            helper.Events.GameLoop.GameLaunched += this.OnGameStarted;
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            helper.Events.GameLoop.Saving += this.OnSaving;
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            helper.Events.GameLoop.DayEnding += this.OnDayEnding;
            helper.Events.GameLoop.ReturnedToTitle += this.OnReturnToTitle;
            helper.Events.Display.MenuChanged += this.OnQuestLogMenuChanged;
            helper.Events.Player.Warped += this.OnPlayerWarped;
            this.NetworkOperator.InitReceived += this.OnNetworkInitMessageReceived;

            this.Patcher = new GamePatcher(this.ModManifest.UniqueID, this.Monitor, false);

            this.Patcher.Apply(
                new Patches.QuestPatch(this.QuestManager, this.EventManager),
                new Patches.LocationPatch(this.ConditionManager, this.CustomBoardController),
                new Patches.Game1Patch(this.QuestManager, this.QuestOfferManager),
                new Patches.DialoguePatch(this.QuestManager),
                new Patches.NPCPatch(this.QuestManager, this.QuestOfferManager),
                new Patches.BillboardPatch());

            helper.ConsoleCommands.Add("quests_list", "List all managed quests", Commands.ListQuests);
            helper.ConsoleCommands.Add("quests_log", "List all managed quests which are in player's quest log", Commands.ListLog);
            helper.ConsoleCommands.Add("quests_stats", "Show quest statistics", Commands.QuestStats);
            helper.ConsoleCommands.Add("quests_invalidate", "Invalidate quest assets cache", Commands.InvalidateCache);
            helper.ConsoleCommands.Add("quests_accept", "Accept managed quest and add it to questlog", Commands.AcceptQuest);
            helper.ConsoleCommands.Add("quests_complete", "Complete managed quest in questlog", Commands.CompleteQuest);
            helper.ConsoleCommands.Add("quests_remove", "Remove managed quest from questlog", Commands.RemoveQuest);
            helper.ConsoleCommands.Add("quests_customtypes", "List of exposed custom quest types" ,Commands.ListTypeFactories);

            var packs = helper.ContentPacks.GetOwned();
            if (packs.Any())
                this.ContentPackLoader.LoadPacks(packs);
        }

        private void OnPlayerWarped(object sender, WarpedEventArgs e)
        {
            if (e.NewLocation != e.OldLocation)
                this.ContentPackLoader.OnLocationChange(e.NewLocation);
        }

        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsGameLaunched)
                return;

            this.QuestManager.Update();
        }

        [EventPriority(EventPriority.High + 100)]
        private void OnGameStarted(object sender, GameLaunchedEventArgs e)
        {
            this.Bridge.Init();
            this.Patcher.CheckPatches();
            this.ChangeState(State.STANDBY);
            this.RegisterDefaultHooks();
            this.Monitor.Log("Quest framework established their internal systems");
        }

        private void OnSaving(object sender, SavingEventArgs e)
        {
            if (Context.IsMainPlayer)
            {
                this.Helper.Data.WriteSaveData("storedQuestIds", this.QuestController.GetQuestIds());
                this.QuestStateStore.Persist();
                this.StatsManager.SaveStats();
            }
        }

        [EventPriority(EventPriority.Low - 100)]
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            this.QuestController.RefreshAllManagedQuestsInQuestLog();
            this.QuestController.RefreshBulletinboardQuestOffer();
            this.CustomBoardController.RefreshBoards();
            this.MailController.ReceiveQuestLetterToMailbox();
            this.EventManager.Refreshed.Fire(new EventArgs(), this);
        }

        [EventPriority(EventPriority.Low - 100)]
        private void OnDayEnding(object sender, DayEndingEventArgs e)
        {
            this.QuestController.SanitizeAllManagedQuestsInQuestLog(this.Helper.Translation);

            if (this.Config.EnableStateVerification)
                this.QuestStateStore.Verify(
                    Game1.player.UniqueMultiplayerID, 
                    this.QuestManager.Quests.Where(q => q is IStateRestorable));
        }

        public override object GetApi()
        {
            return new QuestApi(this);
        }

        /// <summary>
        /// Force reload assets which are used by Quest Framework
        /// </summary>
        public static void InvalidateCache()
        {
            Instance?.Helper.Content.InvalidateCache("Data\\Quests");
            Instance?.Helper.Content.InvalidateCache("Data\\Mail");
        }

        private void OnReturnToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            this.ChangeState(State.CLEANING);
            this.QuestController.Reset();
            this.CustomBoardController.Reset();
            this.QuestManager.Quests.Clear();
            this.QuestOfferManager.Offers.Clear();
            this.ConditionManager.Clean();
            this.QuestStateStore.Clean();
            this.StatsManager.Clean();
            this.NetworkOperator.Reset();
            this.ContentPackLoader.OnReturnedToTitle();
            this.hasInitialized = false;
            this.hasSaveLoaded = false;
            this.hasInitMessageArrived = false;
            this.ChangeState(State.STANDBY);
            InvalidateCache();

            this.Monitor.Log($"Quest framework unloaded all stuff for this session.", LogLevel.Info);
        }

        [EventPriority(EventPriority.High + 100)]
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            this.hasSaveLoaded = true;
            this.Patcher.CheckPatches();
            this.Initialize();
        }

        private void OnNetworkInitMessageReceived(object sender, System.EventArgs e)
        {
            this.hasInitMessageArrived = true;
            this.Initialize();
        }

        private void Initialize()
        {
            if (!this.hasSaveLoaded)
                return; // Save must be loaded before initialization of quest framework
            if (!Context.IsMainPlayer && !this.hasInitMessageArrived)
                return; // Init meessage must be received or player must be main player (singleplayer game or host in co-op)
            if (this.hasInitialized)
                return; // Don't init again if it's already initialized

            this.ChangeState(State.LAUNCHING);

            // Restore data from savefile on the main player 
            // (farmhands get initial state from init message from host)
            this.QuestStateStore.RestoreData();
            this.StatsManager.LoadStats();
            this.ContentPackLoader.ApplyLoadedContentPacks();

            this.EventManager.GettingReady.Fire(new Events.GettingReadyEventArgs(), this);
            this.ChangeState(State.LAUNCHED);

            if (Context.IsMainPlayer)
            {
                // Main player generate new id's, assign them to managed and fix them in quest log
                this.QuestController.ReintegrateQuests(
                    this.Helper.Data.ReadSaveData<Dictionary<string, int>>("storedQuestIds"));
                
            } else
            {
                // Farmhands ony re-assign generated ids received from host
                this.QuestController.Reassign();
            }


            this.RestoreStatefullQuests();
            this.ConditionManager.CollectHooks(this.QuestManager.Quests);
            this.QuestLogWatchdog.Initialize();
            InvalidateCache();

            this.hasInitialized = true;
            this.EventManager.Ready.Fire(new Events.ReadyEventArgs(), this);

            // Post-ready executions
            this.QuestController.RefreshBulletinboardQuestOffer();
            this.MailController.ReceiveQuestLetterToMailbox();

            this.Monitor.Log($"Quest framework sucessfully initialized all stuff for this loaded save game and it's ready!", LogLevel.Info);
        }

        private void OnQuestLogMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu is QuestLog && !(e.NewMenu is ManagedQuestLog))
            {
                Game1.activeClickableMenu = new ManagedQuestLog();
                return;
            }

            if (e.NewMenu is QuestLog && !(e.OldMenu is QuestLog))
                this.EventManager.QuestLogMenuOpen.Fire(new System.EventArgs(), this);
            if (!(e.NewMenu is QuestLog) && e.OldMenu is QuestLog)
            {
                this.EventManager.QuestLogMenuClosed.Fire(new System.EventArgs(), this);

                // Remove quests marked for destroy from questlog
                int i = 0;
                while(i < Game1.player.questLog.Count)
                {
                    if (Game1.player.questLog[i].destroy.Value == true)
                        Game1.player.questLog.RemoveAt(i);
                    else
                        i++;
                }
            }
        }

        private void RegisterDefaultHooks()
        {
            this.ConditionManager.AddHookObserver(new LocationHook(this.Helper));
            this.ConditionManager.AddHookObserver(new TileHook(this.Helper));
        }

        private void RestoreStatefullQuests()
        {
            var store = this.QuestStateStore.GetPayloadList(Game1.player.UniqueMultiplayerID);

            if (store == null)
                return;

            foreach (var quest in this.QuestManager.Quests)
            {
                if (quest is IStateRestorable statefullQuest && store.TryGetValue(quest.GetFullName(), out StatePayload payload))
                {
                    statefullQuest.RestoreState(payload);
                    this.Monitor.VerboseLog($"State for `{quest.GetFullName()}` was restored, type `{payload.StateData.Type}`");
                }
            }
        }

        private void ChangeState(State state)
        {
            if (this.Status == state)
                return;

            var previous = this.Status;
            this.Status = state;
            this.EventManager.ChangeState.Fire(new Events.ChangeStateEventArgs(previous, this.Status), this);
            this.Monitor.Log($"State changed: {previous} -> {this.Status}");
        }
    }
}
