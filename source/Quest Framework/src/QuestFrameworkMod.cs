using PurrplingCore.Patching;
using QuestFramework.Api;
using QuestFramework.Framework;
using QuestFramework.Framework.Bridges;
using QuestFramework.Framework.ContentPacks;
using QuestFramework.Framework.Controllers;
using QuestFramework.Framework.Events;
using QuestFramework.Framework.Hooks;
using QuestFramework.Framework.Networing;
using QuestFramework.Framework.Store;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
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
        internal Bridge Bridge { get; private set; }
        internal QuestManager QuestManager { get; private set; }
        internal QuestApi Api { get; private set; }
        internal QuestStateStore QuestStateStore { get; private set; }
        internal HookManager HookManager { get; private set; }
        internal QuestOfferManager QuestOfferManager { get; private set; }
        internal Loader ContentPackLoader { get; private set; }
        internal QuestController QuestController { get; private set; }
        internal MailController MailController { get; private set; }
        internal NetworkOperator NetworkOperator { get; private set; }
        internal EventManager EventManager { get; private set; }

        internal static QuestFrameworkMod Instance { get; private set; }

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Instance = this;

            this.Bridge = new Bridge(helper.ModRegistry);
            this.EventManager = new EventManager();
            this.QuestManager = new QuestManager(this.Monitor);
            this.QuestStateStore = new QuestStateStore(helper.Data, this.Monitor);
            this.HookManager = new HookManager();
            this.QuestOfferManager = new QuestOfferManager(this.HookManager);
            this.ContentPackLoader = new Loader(this.Monitor, this.QuestManager, this.QuestOfferManager);
            this.QuestController = new QuestController(this.QuestManager, this.Monitor);
            this.MailController = new MailController(this.QuestManager, this.QuestOfferManager, this.Monitor);
            this.NetworkOperator = new NetworkOperator(
                helper.Multiplayer, helper.Events.Multiplayer, this.QuestStateStore, this.QuestController,
                this.ModManifest, this.Monitor);

            helper.Content.AssetEditors.Add(this.QuestController);
            helper.Content.AssetEditors.Add(this.MailController);

            helper.Events.GameLoop.GameLaunched += this.OnGameStarted;
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            helper.Events.GameLoop.Saving += this.OnSaving;
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            helper.Events.GameLoop.DayEnding += this.OnDayEnding;
            helper.Events.GameLoop.ReturnedToTitle += this.OnReturnToTitle;
            this.NetworkOperator.InitReceived += this.OnNetworkInitMessageReceived;

            GamePatcher patcher = new GamePatcher(this.ModManifest.UniqueID, this.Monitor);

            patcher.Apply(
                new Patches.QuestPatch(this.QuestManager),
                new Patches.LocationPatch(this.HookManager),
                new Patches.Game1Patch(this.QuestManager, this.QuestOfferManager),
                new Patches.DialoguePatch(this.QuestManager),
                new Patches.NPCPatch(this.QuestManager, this.QuestOfferManager),
                new Patches.BillboardPatch());

            helper.ConsoleCommands.Add("quests_list", "List all managed quests", Commands.ListQuests);
            helper.ConsoleCommands.Add("quests_log", "List all managed quests which are in player's quest log", Commands.ListLog);

            var packs = helper.ContentPacks.GetOwned();
            if (packs.Any())
                this.ContentPackLoader.LoadPacks(packs);
        }

        private void OnSaving(object sender, SavingEventArgs e)
        {
            if (Context.IsMainPlayer)
            {
                this.Helper.Data.WriteSaveData("storedQuestIds", this.QuestController.GetQuestIds());
                this.QuestStateStore.Persist();
            }
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            this.QuestController.RefreshAllManagedQuestsInQuestLog();
            this.MailController.ReceiveQuestLetterToMailbox();
        }

        private void OnDayEnding(object sender, DayEndingEventArgs e)
        {
            this.QuestController.SanitizeAllManagedQuestsInQuestLog(this.Helper.Translation);
        }

        public override object GetApi()
        {
            if (this.Api == null)
            {
                this.Api = new QuestApi(this.QuestManager, this.EventManager, this.QuestOfferManager, this.Monitor);
            }

            return this.Api;
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
            this.QuestManager.Quests.Clear();
            this.QuestOfferManager.Offers.Clear();
            this.HookManager.Clean();
            this.NetworkOperator.Reset();
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
                return; // Don't init again if it'S already initialized

            this.ChangeState(State.LAUNCHING);

            // Restore data from savefile on the main player 
            // (farmhands get initial state from init message from host)
            this.QuestStateStore.RestoreData();
            this.ContentPackLoader.RegisterQuestsFromPacks();

            // TODO: fire 'GettingReady' event on this line
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
            this.HookManager.CollectHooks(this.QuestManager.Quests);
            this.hasInitialized = true;
            this.EventManager.Ready.Fire(new Events.ReadyEventArgs(), this);
            
            InvalidateCache();
            Game1.RefreshQuestOfTheDay();
            this.MailController.ReceiveQuestLetterToMailbox();

            this.Monitor.Log($"Quest framework sucessfully initialized all stuff for this loaded save game and it's ready!", LogLevel.Info);
        }

        private void RegisterDefaultHooks()
        {
            this.HookManager.AddHookObserver(new LocationHook(this.Helper));
            this.HookManager.AddHookObserver(new TileHook(this.Helper));
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

        [EventPriority(EventPriority.High + 100)]
        private void OnGameStarted(object sender, GameLaunchedEventArgs e)
        {
            this.Bridge.Init();
            this.ChangeState(State.STANDBY);
            this.RegisterDefaultHooks();
            this.Monitor.Log("Quest framework established their internal systems");
        }
    }
}
