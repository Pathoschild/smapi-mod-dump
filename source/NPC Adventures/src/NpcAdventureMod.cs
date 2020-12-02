/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/PurrplingMod
**
*************************************************/

using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using NpcAdventure.Loader;
using NpcAdventure.Driver;
using NpcAdventure.Events;
using NpcAdventure.Model;
using NpcAdventure.HUD;
using NpcAdventure.Compatibility;
using NpcAdventure.Story;
using NpcAdventure.Story.Scenario;
using NpcAdventure.Internal.Assets;
using PurrplingCore.Patching;
using QuestFramework.Api;

namespace NpcAdventure
{
    /// <summary>The mod entry point.</summary>
    public class NpcAdventureMod : Mod
    {
        private bool firstTick = true;

        private DialogueDriver DialogueDriver { get; set; }
        private HintDriver HintDriver { get; set; }
        private StuffDriver StuffDriver { get; set; }
        private MailDriver MailDriver { get; set; }
        public IQuestApi QuestApi { get; private set; }
        internal ISpecialModEvents SpecialEvents { get; private set; }
        internal CompanionManager CompanionManager { get; private set; }
        internal CompanionDisplay CompanionHud { get; private set; }
        internal ContentLoader ContentLoader { get; private set; }
        internal GamePatcher Patcher { get; private set; }
        internal GameMaster GameMaster { get; private set; }
        internal Config Config { get; private set; } = new Config();
        internal ContentPackManager ContentPackManager { get; private set; }
        internal static List<string> DebugFlags { get; } = new List<string>();
        internal static IManifest Manifest { get; private set; }
        internal static IReflectionHelper Reflection { get; private set; }

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Manifest = this.ModManifest;
            Reflection = helper.Reflection;
            this.Config = helper.ReadConfig<Config>();

            if (Constants.TargetPlatform == GamePlatform.Android)
            {
                this.Monitor.Log("Android support is an experimental feature, may cause some problems. Before you report a bug please content me on my discord https://discord.gg/wnEDqKF Thank you.", LogLevel.Alert);
            }

            if (this.Config.AllowLegacyContentPacks)
            {
                this.Monitor.Log("Loading of legacy content packs is allowed! This may cause some unexpected side-effects.", LogLevel.Alert);
            }
            
            this.RegisterAssetEditors(helper);
            this.ContentPackManager = new ContentPackManager(this.Monitor, this.Config.EnableDebug, this.Config.AllowLegacyContentPacks);
            this.ContentLoader = new ContentLoader(helper, this.ContentPackManager, this.Monitor);
            this.Patcher = new GamePatcher(this.ModManifest.UniqueID, this.Monitor, this.Config.EnableDebug);
            this.RegisterEvents(helper.Events);
            this.ContentPackManager.LoadContentPacks(helper.ContentPacks.GetOwned());
            Commander.Register(this);
        }

        private void RegisterAssetEditors(IModHelper helper)
        {
            if (Constants.TargetPlatform != GamePlatform.Android && this.Config.UseAsk2FollowCursor)
            {
                helper.Content.AssetEditors.Add(new AskToFollowCursor(helper.Content));
            }
        }

        private void RegisterEvents(IModEvents events)
        {
            events.GameLoop.SaveLoaded += this.GameLoop_SaveLoaded;
            events.GameLoop.Saving += this.GameLoop_Saving;
            events.GameLoop.ReturnedToTitle += this.GameLoop_ReturnedToTitle;
            events.GameLoop.DayEnding += this.GameLoop_DayEnding;
            events.GameLoop.DayStarted += this.GameLoop_DayStarted;
            events.GameLoop.GameLaunched += this.GameLoop_GameLaunched;
            events.GameLoop.UpdateTicked += this.GameLoop_UpdateTicked;
            events.Display.RenderingHud += this.Display_RenderingHud;
            events.Player.Warped += this.Player_Warped;
        }

        private void Player_Warped(object sender, WarpedEventArgs e)
        {
            if (!this.Config.UseCheckForEventsPatch && Context.IsWorldReady && this.GameMaster.Mode != GameMasterMode.OFFLINE)
            {
                // Check for NPC Adventures events in the old way by player warped event. This way will be removed in 0.16.0
                this.GameMaster.CheckForEvents(e.NewLocation, e.Player);
            }
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
            if (this.firstTick)
            {
                // Check if methods patched by NA are patched by other mods
                this.Patcher.CheckPatches();
                this.firstTick = false;
            }

            if (Context.IsWorldReady && this.CompanionHud != null)
                this.CompanionHud.Update(e);
        }

        private void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // Setup third party mod compatibility bridge
            Compat.Setup(this.Helper.ModRegistry, this.Monitor);
            IQuestApi questApi = this.Helper.ModRegistry.GetApi<IQuestApi>("purrplingcat.questframework");
            var storyHelper = new StoryHelper(this.ContentLoader, questApi.GetManagedApi(this.ModManifest));

            questApi.Events.GettingReady += (_, args) => storyHelper.LoadQuests(this.GameMaster);
            questApi.Events.Ready += (_, args) => storyHelper.SanitizeOldAdventureQuestsInLog();

            // Mod's services and drivers
            this.QuestApi = questApi;
            this.SpecialEvents = new SpecialModEvents();
            this.DialogueDriver = new DialogueDriver(this.Helper.Events);
            this.HintDriver = new HintDriver(this.Helper.Events);
            this.StuffDriver = new StuffDriver(this.Helper.Data, this.Monitor);
            this.MailDriver = new MailDriver(this.ContentLoader, this.Monitor);
            this.GameMaster = new GameMaster(this.Helper, storyHelper, this.Monitor);
            this.CompanionHud = new CompanionDisplay(this.Config, this.ContentLoader);
            this.CompanionManager = new CompanionManager(this.GameMaster, this.DialogueDriver, this.HintDriver, this.CompanionHud, this.Config, this.Monitor);
            
            this.StuffDriver.RegisterEvents(this.Helper.Events);
            this.MailDriver.RegisterEvents(this.SpecialEvents);

            this.ApplyPatches(); // Apply harmony patches
            this.InitializeScenarios();
        }

        private void ApplyPatches()
        {
            // Core patches (important)
            this.Patcher.Apply(
                new Patches.MailBoxPatch((SpecialModEvents)this.SpecialEvents),
                new Patches.SpouseReturnHomePatch(this.CompanionManager),
                new Patches.GetCharacterPatch(this.CompanionManager),
                new Patches.NpcCheckActionPatch(this.CompanionManager, this.Helper.Input, this.Config),
                new Patches.GameLocationDrawPatch((SpecialModEvents)this.SpecialEvents),
                new Patches.GameLocationPerformActionPatch(this.CompanionManager, this.Config.AllowEntryLockedCompanionHouse),
                new Patches.MonsterBehaviorPatch(this.CompanionManager)
            );

            if (this.Config.AvoidSayHiToMonsters)
            {
                // Optional patch: Avoid say hi to monsters while companioning (this patch enabled by default)
                this.Patcher.Apply(new Patches.CompanionSayHiPatch(this.CompanionManager));
            }

            if (this.Config.FightThruCompanion)
            {
                // Optional patch: Avoid annoying dialogue shown while use sword over companion (patch enabled by default)
                this.Patcher.Apply(new Patches.GameUseToolPatch(this.CompanionManager));
            }

            if (this.Config.UseCheckForEventsPatch)
            {
                this.Patcher.Apply(new Patches.CheckEventPatch(this.GameMaster));
            }

            if (this.Config.Experimental.UseSwimsuits)
            {
                this.LogExperimental("Support swimsuits for companions");
            }
        }

        private void LogExperimental(string featureName)
        {
            this.Monitor.Log($"You are enabled experimental feature '{featureName}' in mod's config.json.", LogLevel.Warn);
            this.Monitor.Log("   This feature may affect game stability, you can disable it in config.json", LogLevel.Warn);
        }

        private void InitializeScenarios()
        {
            if (!this.Config.AdventureMode)
                return; // Don't init game master scenarios when adventure mode is disabled

            this.GameMaster.RegisterScenario(new AdventureBegins(this.SpecialEvents, this.QuestApi.Events, this.Helper.Events, this.ContentLoader, this.Config, this.Monitor));
            this.GameMaster.RegisterScenario(new RecruitmentScenario());
            this.GameMaster.RegisterScenario(new CompanionCutscenes(this.ContentLoader, this.CompanionManager));
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
            this.Patcher.CheckPatches();
        }
        public override object GetApi()
        {
            return new NpcAdventureModApi(this);
        }
    }
}
