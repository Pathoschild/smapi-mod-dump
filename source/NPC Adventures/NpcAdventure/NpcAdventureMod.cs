using StardewModdingAPI;
using StardewModdingAPI.Events;
using NpcAdventure.Loader;
using NpcAdventure.Driver;
using StardewValley;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using NpcAdventure.Model;

namespace NpcAdventure
{
    /// <summary>The mod entry point.</summary>
    public class NpcAdventureMod : Mod
    {
        private CompanionManager companionManager;
        private ContentLoader contentLoader;
        private Config config;

        private DialogueDriver DialogueDriver { get; set; }
        private HintDriver HintDriver { get; set; }
        private StuffDriver StuffDriver { get; set; }

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.RegisterEvents(helper.Events);
            this.config = helper.ReadConfig<Config>();
        }

        private void RegisterEvents(IModEvents events)
        {
            events.GameLoop.SaveLoaded += this.GameLoop_SaveLoaded;
            events.Specialized.LoadStageChanged += this.Specialized_LoadStageChanged;
            events.GameLoop.ReturnedToTitle += this.GameLoop_ReturnedToTitle;
            events.GameLoop.DayEnding += this.GameLoop_DayEnding;
            events.GameLoop.DayStarted += this.GameLoop_DayStarted;
            events.GameLoop.GameLaunched += this.GameLoop_GameLaunched;
        }

        private void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            this.DialogueDriver = new DialogueDriver(this.Helper.Events);
            this.HintDriver = new HintDriver(this.Helper.Events);
            this.StuffDriver = new StuffDriver(this.Helper.Data, this.Monitor);
            this.contentLoader = new ContentLoader(this.Helper.Content, this.Helper.ContentPacks, this.ModManifest.UniqueID, "assets", this.Helper.DirectoryPath, this.Monitor);
            this.companionManager = new CompanionManager(this.DialogueDriver, this.HintDriver, this.config, this.Monitor);
            this.StuffDriver.RegisterEvents(this.Helper.Events);
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

            var dispositions = this.contentLoader.LoadStrings("Data/CompanionDispositions");

            this.contentLoader.LoadStrings("Data/AnimationDescriptions");
            this.contentLoader.LoadStrings("Data/IdleBehaviors");
            this.contentLoader.LoadStrings("Data/IdleNPCDefinitions");
            this.contentLoader.LoadStrings("Strings/Strings");
            this.contentLoader.LoadStrings("Strings/SpeechBubbles");

            // Preload dialogues for companions
            foreach (string npcName in dispositions.Keys)
            {
                this.contentLoader.LoadStrings($"Dialogue/{npcName}");
            }

            this.Monitor.Log("Assets preloaded!", LogLevel.Info);
        }

        private void GameLoop_DayStarted(object sender, DayStartedEventArgs e)
        {
            this.companionManager.NewDaySetup();
        }

        private void GameLoop_DayEnding(object sender, DayEndingEventArgs e)
        {
            this.companionManager.ResetStateMachines();
            this.companionManager.DumpCompanionNonEmptyBags();
        }

        private void GameLoop_ReturnedToTitle(object sender, StardewModdingAPI.Events.ReturnedToTitleEventArgs e)
        {
            this.companionManager.UninitializeCompanions();
            this.contentLoader.InvalidateCache();
        }

        private void GameLoop_SaveLoaded(object sender, StardewModdingAPI.Events.SaveLoadedEventArgs e)
        {
            this.companionManager.InitializeCompanions(this.contentLoader, this.Helper.Events, this.Helper.Reflection);
        }
    }
}