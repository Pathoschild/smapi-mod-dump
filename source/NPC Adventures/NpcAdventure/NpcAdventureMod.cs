using StardewModdingAPI;
using StardewModdingAPI.Events;
using NpcAdventure.Loader;
using NpcAdventure.Driver;
using StardewValley;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Harmony;
using System.Reflection;
using NpcAdventure.Events;
using NpcAdventure.Model;
using NpcAdventure.HUD;
using NpcAdventure.Compatibility;

namespace NpcAdventure
{
    /// <summary>The mod entry point.</summary>
    public class NpcAdventureMod : Mod
    {
        private CompanionManager companionManager;
        private CompanionDisplay companionHud;

        public static IMonitor GameMonitor { get; private set; }

        private ContentLoader contentLoader;
        private Config config;

        private DialogueDriver DialogueDriver { get; set; }
        private HintDriver HintDriver { get; set; }
        private StuffDriver StuffDriver { get; set; }
        private ISpecialModEvents SpecialEvents { get; set; }

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
            events.GameLoop.UpdateTicked += this.GameLoop_UpdateTicked;
            events.Display.RenderingHud += this.Display_RenderingHud; ;
        }

        private void Display_RenderingHud(object sender, RenderingHudEventArgs e)
        {
            if (Context.IsWorldReady && this.companionHud != null)
                this.companionHud.Draw(e.SpriteBatch);
        }

        private void GameLoop_UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (Context.IsWorldReady && this.companionHud != null)
                this.companionHud.Update(e);
        }

        private void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // Setup third party mod compatibility bridge
            TPMC.Setup(this.Helper.ModRegistry);

            // Mod's services and drivers
            this.SpecialEvents = new SpecialModEvents();
            this.DialogueDriver = new DialogueDriver(this.Helper.Events);
            this.HintDriver = new HintDriver(this.Helper.Events);
            this.StuffDriver = new StuffDriver(this.Helper.Data, this.Monitor);
            this.contentLoader = new ContentLoader(this.Helper.Content, this.Helper.ContentPacks, this.ModManifest.UniqueID, "assets", this.Helper.DirectoryPath, this.Monitor);
            this.companionHud = new CompanionDisplay(this.config, this.contentLoader);
            this.companionManager = new CompanionManager(this.DialogueDriver, this.HintDriver, this.companionHud, this.config, this.Monitor);
            this.StuffDriver.RegisterEvents(this.Helper.Events);
            
            // Harmony
            HarmonyInstance harmony = HarmonyInstance.Create("Purrplingcat.NpcAdventure");
            
            Patches.SpouseReturnHomePatch.Setup(harmony);
            Patches.CompanionSayHiPatch.Setup(harmony, this.companionManager);
            Patches.GameLocationDrawPatch.Setup(harmony, this.SpecialEvents);
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
            if (Context.IsMultiplayer)
                return;
            this.companionManager.NewDaySetup();
        }

        private void GameLoop_DayEnding(object sender, DayEndingEventArgs e)
        {
            if (Context.IsMultiplayer)
                return;
            this.companionManager.ResetStateMachines();
            this.companionManager.DumpCompanionNonEmptyBags();
        }

        private void GameLoop_ReturnedToTitle(object sender, StardewModdingAPI.Events.ReturnedToTitleEventArgs e)
        {
            if (Context.IsMultiplayer)
                return;
            this.companionManager.UninitializeCompanions();
            this.contentLoader.InvalidateCache();

            // Clean data in patches
            Patches.SpouseReturnHomePatch.recruitedSpouses.Clear();
        }

        private void GameLoop_SaveLoaded(object sender, StardewModdingAPI.Events.SaveLoadedEventArgs e)
        {
            if (Context.IsMultiplayer)
            {
                this.Monitor.Log("Companions not initalized, because multiplayer currently unsupported in NPC Adventures.", LogLevel.Warn);
                return;
            }
            this.companionManager.InitializeCompanions(this.contentLoader, this.Helper.Events, this.SpecialEvents, this.Helper.Reflection);
        }
    }
}