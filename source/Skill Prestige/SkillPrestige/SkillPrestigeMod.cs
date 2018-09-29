using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.IO;
using System.Linq;
using SkillPrestige.Commands;
using SkillPrestige.Logging;
using SkillPrestige.Menus;
using SkillPrestige.Menus.Elements.Buttons;
using SkillPrestige.Mods;
using SkillPrestige.Professions;
using static SkillPrestige.InputHandling.Mouse;

namespace SkillPrestige
{
    /// <summary>
    /// The Skill Prestige Mod by Alphablackwolf. Enjoy! 
    /// </summary>
    public class SkillPrestigeMod : Mod
    {
        #region Manifest Data

        public static string ModPath { get; private set; }

        public static string OptionsPath { get; private set; }

        public static IMonitor LogMonitor { get; private set; }

        public static string CurrentSaveOptionsPath { get; private set; }

        public static string PerSaveOptionsDirectory { get; private set; }

        public static Texture2D PrestigeIconTexture { get; private set; }

        public static Texture2D CheckmarkTexture { get; private set; }

        public static IModRegistry ModRegistry { get; private set; }

        private static bool SaveIsLoaded { get; set; }

        private IModHelper ModHelper { get; set; }

        private static bool _isFirstUpdate = true;

        #endregion

        public override void Entry(IModHelper helper)
        {
            ModHelper = helper;
            LogMonitor = Monitor;
            ModPath = helper.DirectoryPath;
            ModRegistry = helper.ModRegistry;
            PerSaveOptionsDirectory = Path.Combine(ModPath, "psconfigs/");
            OptionsPath = Path.Combine(ModPath, "config.json");
            Logger.LogInformation("Detected game entry.");
            PrestigeSaveData.Instance.Read();
            
            if (ModHelper.ModRegistry.IsLoaded("community.AllProfessions"))
            {
                Logger.LogCriticalWarning("Conflict Detected. This mod cannot work with AllProfessions. Skill Prestige disabled.");
                Logger.LogDisplay("Skill Prestige Mod: If you wish to use this mod in place of AllProfessions, remove the AllProfessions mod and run the player_resetallprofessions command.");
                return;
            }
            LoadSprites();
            RegisterGameEvents();
            //ReplaceStardewValleyCode();
            Logger.LogDisplay($"{ModManifest.Name} version {ModManifest.Version} by {ModManifest.Author} Initialized.");
        }

        private void RegisterGameEvents()
        {
            Logger.LogInformation("Registering game events...");
            ControlEvents.MouseChanged += MouseChanged;
            LocationEvents.CurrentLocationChanged += LocationChanged;
            GraphicsEvents.OnPostRenderGuiEvent += PostRenderGuiEvent;
            GameEvents.UpdateTick += GameUpdate;
            GameEvents.HalfSecondTick += HalfSecondTick;
            GameEvents.OneSecondTick += OneSecondTick;
            TimeEvents.AfterDayStarted += AfterDayStarted;
            Logger.LogInformation("Game events registered.");
            SaveEvents.AfterLoad += SaveFileLoaded;
            SaveEvents.AfterReturnToTitle += ReturnToTitle;
        }

        private static void MouseChanged(object sender, EventArgsMouseStateChanged args)
        {
            HandleState(args);
        }

        private static void LocationChanged(object sender, EventArgs args)
        {
            Logger.LogVerbose("Location change detected.");
            CurrentSaveOptionsPath = Path.Combine(ModPath, "psconfigs/", $@"{Game1.player.name.RemoveNonAlphanumerics()}_{Game1.uniqueIDForThisGame}.json");
            PrestigeSaveData.Instance.UpdateCurrentSaveFileInformation();
            PerSaveOptions.Instance.Check();
            Profession.AddMissingProfessions();
        }

        private static void AfterDayStarted(object sender, EventArgs args)
        {
            Logger.LogVerbose("New Day Started");
            AnimalProduceHandler.HandleSpawnedAnimalProductQuantityIncrease();
        }

        private static void SaveFileLoaded(object sender, EventArgs args)
        {
            PrestigeSaveData.Instance.UpdateCurrentSaveFileInformation();
            SaveIsLoaded = true;
        }

        private static void ReturnToTitle(object sender, EventArgs args)
        {
            PrestigeSaveData.Instance.Read();
            SaveIsLoaded = false;
            Logger.LogInformation("Return To Title.");
            PerSaveOptions.ClearLoadedPerSaveOptionsFile();
            ExperienceHandler.ResetExperience();
        }
        private static void PostRenderGuiEvent(object sender, EventArgs args)
        {
            SkillsMenuExtension.AddPrestigeButtonsToMenu();
        }

        private static void LoadSprites()
        {
            Logger.LogInformation("Loading sprites...");
            Button.DefaultButtonTexture = Game1.content.Load<Texture2D>(@"LooseSprites\DialogBoxGreen");
            MinimalistProfessionButton.ProfessionButtonTexture = Game1.content.Load<Texture2D>(@"LooseSprites\boardGameBorder");

            var prestigeIconFilePath = Path.Combine(ModPath, @"PrestigeIcon.png");
            Logger.LogInformation($"Prestige Icon Path: {prestigeIconFilePath}");
            var prestigeIconFileStream = new FileStream(prestigeIconFilePath, FileMode.Open);
            PrestigeIconTexture = Texture2D.FromStream(Game1.graphics.GraphicsDevice, prestigeIconFileStream);

            var checkmarkFilePath = Path.Combine(ModPath, @"Checkmark.png");
            Logger.LogInformation($"Checkmark Path: {checkmarkFilePath}");
            var checkmarkFileStream = new FileStream(checkmarkFilePath, FileMode.Open);
            CheckmarkTexture = Texture2D.FromStream(Game1.graphics.GraphicsDevice, checkmarkFileStream);
            Logger.LogInformation("Sprites loaded.");
        }

        private void GameUpdate(object sender, EventArgs args)
        {
            if (_isFirstUpdate)
            {
                ModHandler.RegisterLoadedMods();
                if (Options.Instance.TestingMode) RegisterTestingCommands();
                RegisterCommands();
                _isFirstUpdate = false;
            }
            CheckForGameSave();
            CheckForLevelUpMenu();
        }


        private static void OneSecondTick(object sender, EventArgs args)
        {
            //one second tick for this, as the detection of changed experience can happen as infrequently as possible. a 10 second tick would be well within tolerance.
            UpdateExperience();
        }

        private static void HalfSecondTick(object sender, EventArgs args)
        {
            //from what I can tell of the original game code, tools cannot be used quicker than 600ms, so a half second tick is the largest tick that will always catch that the tool was used.
            ToolProficiencyHandler.HandleToolProficiency();
        }

        private static void UpdateExperience()
        {
            if (SaveIsLoaded) ExperienceHandler.UpdateExperience();
        }

        private static void CheckForGameSave()
        {
            if (!Game1.newDay || Game1.fadeToBlackAlpha <= 0.95f) return;
            Logger.LogInformation("New game day detected.");
            PrestigeSaveData.Instance.Save();
        }

        private static void CheckForLevelUpMenu()
        {
            foreach (var levelUpManager in Skill.AllSkills.Select(x => x.LevelUpManager).GroupBy(x => x.MenuType).Select(g => g.First()))
            {
                if (Game1.activeClickableMenu == null || Game1.activeClickableMenu.GetType() != levelUpManager.MenuType) continue;
                var currentLevel = levelUpManager.GetLevel.Invoke();
                if (currentLevel % 5 != 0) return;
                Logger.LogInformation("Level up menu as profession chooser detected.");
                var currentSkill = levelUpManager.GetSkill.Invoke();
                Game1.activeClickableMenu = levelUpManager.CreateNewLevelUpMenu.Invoke(currentSkill, currentLevel);
                Logger.LogInformation("Replaced level up menu with custom menu.");
            }
        }

        private void RegisterTestingCommands()
        {
            Logger.LogInformation("Registering Testing commands...");
            SkillPrestigeCommand.RegisterCommands(ModHelper.ConsoleCommands, true);
            Logger.LogInformation("Testing commands registered.");
        }

        private void RegisterCommands()
        {
            Logger.LogInformation("Registering commands...");
            SkillPrestigeCommand.RegisterCommands(ModHelper.ConsoleCommands, false);
            Logger.LogInformation("Commands registered.");
        }

        private static void ReplaceStardewValleyCode()
        {
            Logger.LogInformation("Hijacking Methods...");
            Logger.LogInformation("Hijacking Crop Harvest method...");
            typeof(Crop).ReplaceMethod("harvest", typeof(CropReplacement), "HarvestReplacement");
            Logger.LogInformation("Crop Harvest method hijacked!");
        }
    }
}