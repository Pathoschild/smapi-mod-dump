using StardewModdingAPI;
using StardewModdingAPI.Events;
using NpcAdventure.Loader;
using NpcAdventure.Driver;
using Harmony;
using NpcAdventure.Events;
using NpcAdventure.Model;
using NpcAdventure.HUD;
using NpcAdventure.Compatibility;
using NpcAdventure.Story;
using NpcAdventure.Story.Scenario;

namespace NpcAdventure
{
    /// <summary>The mod entry point.</summary>
    public class NpcAdventureMod : Mod
    {
        private DialogueDriver DialogueDriver { get; set; }
        private HintDriver HintDriver { get; set; }
        private StuffDriver StuffDriver { get; set; }
        private MailDriver MailDriver { get; set; }
        internal ISpecialModEvents SpecialEvents { get; private set; }
        internal CompanionManager CompanionManager { get; private set; }
        internal CompanionDisplay CompanionHud { get; private set; }
        internal ContentLoader ContentLoader { get; private set; }
        internal GameMaster GameMaster { get; private set; }
        internal Config Config { get; private set; } = new Config();

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            if (Constants.TargetPlatform == GamePlatform.Android)
            {
                this.Monitor.Log("Android support is an experimental feature, may cause some problems. Before you report a bug please content me on my discord https://discord.gg/wnEDqKF Thank you.", LogLevel.Alert);
            }

            this.RegisterEvents(helper.Events);
            this.Config = helper.ReadConfig<Config>();
            this.ContentLoader = new ContentLoader(this.Helper.Content, this.Helper.ContentPacks, this.ModManifest.UniqueID, "assets", this.Monitor);
            Commander.Register(this);
        }

        private void RegisterEvents(IModEvents events)
        {
            events.GameLoop.SaveLoaded += this.GameLoop_SaveLoaded;
            events.GameLoop.Saving += this.GameLoop_Saving;
            events.Specialized.LoadStageChanged += this.Specialized_LoadStageChanged;
            events.GameLoop.ReturnedToTitle += this.GameLoop_ReturnedToTitle;
            events.GameLoop.DayEnding += this.GameLoop_DayEnding;
            events.GameLoop.DayStarted += this.GameLoop_DayStarted;
            events.GameLoop.GameLaunched += this.GameLoop_GameLaunched;
            events.GameLoop.UpdateTicked += this.GameLoop_UpdateTicked;
            events.Display.RenderingHud += this.Display_RenderingHud;
        }

        private void GameLoop_Saving(object sender, SavingEventArgs e)
        {
            this.GameMaster.SaveData();
        }

        private void Display_RenderingHud(object sender, RenderingHudEventArgs e)
        {
            if (Context.IsWorldReady && this.CompanionHud != null)
                this.CompanionHud.Draw(e.SpriteBatch);
        }

        private void GameLoop_UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (Context.IsWorldReady && this.CompanionHud != null)
                this.CompanionHud.Update(e);
        }

        private void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // Setup third party mod compatibility bridge
            TPMC.Setup(this.Helper.ModRegistry, this.Monitor);

            // Mod's services and drivers
            this.SpecialEvents = new SpecialModEvents();
            this.DialogueDriver = new DialogueDriver(this.Helper.Events);
            this.HintDriver = new HintDriver(this.Helper.Events);
            this.StuffDriver = new StuffDriver(this.Helper.Data, this.Monitor);
            this.MailDriver = new MailDriver(this.ContentLoader, this.Monitor);
            this.GameMaster = new GameMaster(this.Helper, new StoryHelper(this.ContentLoader), this.Monitor);
            this.CompanionHud = new CompanionDisplay(this.Config, this.ContentLoader);
            this.CompanionManager = new CompanionManager(this.GameMaster, this.DialogueDriver, this.HintDriver, this.CompanionHud, this.Config, this.Monitor);
            
            this.StuffDriver.RegisterEvents(this.Helper.Events);
            this.MailDriver.RegisterEvents(this.SpecialEvents);

            this.ApplyPatches(); // Apply harmony patches
            this.InitializeScenarios();
        }

        private void ApplyPatches()
        {
            HarmonyInstance harmony = HarmonyInstance.Create("Purrplingcat.NpcAdventure");

            Patches.MailBoxPatch.Setup(harmony, (SpecialModEvents)this.SpecialEvents);
            Patches.QuestPatch.Setup(harmony, (SpecialModEvents)this.SpecialEvents);
            Patches.SpouseReturnHomePatch.Setup(harmony, this.CompanionManager);
            Patches.CompanionSayHiPatch.Setup(harmony, this.CompanionManager);
            Patches.GameLocationDrawPatch.Setup(harmony, this.SpecialEvents);
            Patches.GetCharacterPatch.Setup(harmony, this.CompanionManager);
            Patches.NpcCheckActionPatch.Setup(harmony, this.CompanionManager);
        }

        private void Specialized_LoadStageChanged(object sender, LoadStageChangedEventArgs e)
        {
            if (e.NewStage == StardewModdingAPI.Enums.LoadStage.Loaded)
            {
                this.PreloadAssets();
            }
        }

        private void PreloadAssets()
        {
            /* Preload assets to cache */
            this.Monitor.Log("Preloading assets...", LogLevel.Info);

            var dispositions = this.ContentLoader.LoadStrings("Data/CompanionDispositions");

            this.ContentLoader.LoadStrings("Data/AnimationDescriptions");
            this.ContentLoader.LoadStrings("Data/IdleBehaviors");
            this.ContentLoader.LoadStrings("Data/IdleNPCDefinitions");
            this.ContentLoader.LoadStrings("Strings/Strings");
            this.ContentLoader.LoadStrings("Strings/SpeechBubbles");

            // Preload dialogues for companions
            foreach (string npcName in dispositions.Keys)
            {
                this.ContentLoader.LoadStrings($"Dialogue/{npcName}");
            }

            this.Monitor.Log("Assets preloaded!", LogLevel.Info);
        }

        private void InitializeScenarios()
        {
            if (!this.Config.AdventureMode)
                return; // Don't init gamem aster scenarios when adventure mode is disabled

            this.GameMaster.RegisterScenario(new AdventureBegins(this.SpecialEvents, this.Helper.Events, this.ContentLoader, this.Config, this.Monitor));
            this.GameMaster.RegisterScenario(new QuestScenario(this.SpecialEvents, this.ContentLoader, this.Monitor));
        }

        private void GameLoop_DayStarted(object sender, DayStartedEventArgs e)
        {
            if (Context.IsMultiplayer)
                return;
            this.CompanionManager.NewDaySetup();
        }

        private void GameLoop_DayEnding(object sender, DayEndingEventArgs e)
        {
            if (Context.IsMultiplayer)
                return;

            this.CompanionManager.ResetStateMachines();
            this.CompanionManager.DumpCompanionNonEmptyBags();
        }

        private void GameLoop_ReturnedToTitle(object sender, StardewModdingAPI.Events.ReturnedToTitleEventArgs e)
        {
            if (Context.IsMultiplayer)
                return;

            this.GameMaster.Uninitialize();
            this.CompanionManager.UninitializeCompanions();
            this.ContentLoader.InvalidateCache();
        }

        private void GameLoop_SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            if (Context.IsMultiplayer)
            {
                this.Monitor.Log("Companions not initalized, because multiplayer currently unsupported by NPC Adventures.", LogLevel.Warn);
                return;
            }

            if (this.Config.AdventureMode)
                this.GameMaster.Initialize();
            else
                this.Monitor.Log("Started in non-adventure mode", LogLevel.Info);

            this.CompanionManager.InitializeCompanions(this.ContentLoader, this.Helper.Events, this.SpecialEvents, this.Helper.Reflection);
        }
    }
}