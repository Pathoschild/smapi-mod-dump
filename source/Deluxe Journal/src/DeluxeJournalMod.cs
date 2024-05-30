/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework.Graphics;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using DeluxeJournal.Api;
using DeluxeJournal.Framework;
using DeluxeJournal.Framework.Data;
using DeluxeJournal.Framework.Events;
using DeluxeJournal.Framework.Task;
using DeluxeJournal.Menus;
using DeluxeJournal.Menus.Components;
using DeluxeJournal.Patching;
using DeluxeJournal.Task.Tasks;

namespace DeluxeJournal
{
    /// <summary>The mod entry point.</summary>
    internal class DeluxeJournalMod : Mod
    {
        /// <summary>Data key for the notes save data.</summary>
        public const string NOTES_DATA_KEY = "notes-data";

        /// <summary>Data key for the tasks save data.</summary>
        public const string TASKS_DATA_KEY = "tasks-data";

        /// <summary>UI spirte sheet texture.</summary>
        public static Texture2D? UiTexture { get; private set; }

        /// <summary>Animal icon spirte sheet texture.</summary>
        public static Texture2D? AnimalIconsTexture { get; private set; }

        /// <summary>Building icon spirte sheet texture.</summary>
        public static Texture2D? BuildingIconsTexture { get; private set; }

        /// <summary>
        /// Check if this is the main screen. Returns <c>false</c> if this is a co-op player
        /// while playing in split-screen mode, and <c>true</c> otherwise.
        /// </summary>
        public static bool IsMainScreen => !Context.IsSplitScreen || Context.ScreenId == 0;

        /// <summary>The mod instance.</summary>
        public static DeluxeJournalMod? Instance { get; private set; }

        /// <summary>Translation helper.</summary>
        public static ITranslationHelper? Translation { get; private set; }

        /// <summary>Notes save data.</summary>
        private NotesData? NotesData { get; set; }

        /// <summary>Configuration settings.</summary>
        public Config? Config { get; private set; }

        /// <summary>Event manager for handling event subscriptions.</summary>
        public EventManager? EventManager { get; private set; }

        /// <summary>Task manager.</summary>
        public TaskManager? TaskManager { get; private set; }

        /// <summary>Page manager for accessing journal pages.</summary>
        public PageManager? PageManager { get; private set; }

        public override void Entry(IModHelper helper)
        {
            Instance = this;
            Translation = helper.Translation;

            RuntimeHelpers.RunClassConstructor(typeof(TaskTypes).TypeHandle);

            UiTexture = helper.ModContent.Load<Texture2D>("assets/ui.png");
            AnimalIconsTexture = helper.ModContent.Load<Texture2D>("assets/animal-icons.png");
            BuildingIconsTexture = helper.ModContent.Load<Texture2D>("assets/building-icons.png");
            SmartIconComponent.AnimalIconIds = helper.ModContent.Load<Dictionary<string, int>>("assets/data/animal-icons.json");
            SmartIconComponent.BuildingIconData = helper.ModContent.Load<Dictionary<string, BuildingIconData>>("assets/data/building-icons.json");
            Config = helper.ReadConfig<Config>();
            NotesData = helper.Data.ReadGlobalData<NotesData>(NOTES_DATA_KEY) ?? new NotesData();

            EventManager = new EventManager(helper.Events, helper.Multiplayer, Monitor);
            TaskManager = new TaskManager(new TaskEvents(EventManager), helper.Data, Config, ModManifest.Version);
            PageManager = new PageManager();

            PageManager.RegisterPage("quests", (bounds) => new QuestLogPage(bounds, UiTexture, helper.Translation), 102);
            PageManager.RegisterPage("tasks", (bounds) => new TasksPage(bounds, UiTexture, helper.Translation), 101);
            PageManager.RegisterPage("notes", (bounds) => new NotesPage(bounds, UiTexture, helper.Translation), 100);

            helper.Events.Display.MenuChanged += OnMenuChanged;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.Saving += OnSaving;

            Patcher.Apply(new Harmony(ModManifest.UniqueID), Monitor,
                new QuestLogPatch(Monitor),
                new FarmerPatch(EventManager, Monitor),
                new CarpenterMenuPatch(EventManager, Monitor),
                new PurchaseAnimalsMenuPatch(EventManager, Monitor),
                new ShopMenuPatch(EventManager, Monitor)
            );
        }

        public override object GetApi()
        {
            return new DeluxeJournalApi(this);
        }

        /// <summary>Get the stored notes page text.</summary>
        public string GetNotes()
        {
            if (NotesData != null && Constants.SaveFolderName != null && NotesData.Text.ContainsKey(Constants.SaveFolderName))
            {
                return NotesData.Text[Constants.SaveFolderName];
            }

            return string.Empty;
        }

        /// <summary>Save the notes page text.</summary>
        public void SaveNotes(string text)
        {
            if (NotesData != null && Constants.SaveFolderName != null)
            {
                NotesData.Text[Constants.SaveFolderName] = text;
                Helper.Data.WriteGlobalData(NOTES_DATA_KEY, NotesData);
            }
        }

        [EventPriority(EventPriority.Low)]
        private void OnMenuChanged(object? sender, MenuChangedEventArgs e)
        {
            // Hijack QuestLog and replace it with DeluxeJournalMenu
            if (PageManager != null && Game1.activeClickableMenu is QuestLog questLog)
            {
                DeluxeJournalMenu deluxeJournalMenu = new DeluxeJournalMenu(PageManager);
                deluxeJournalMenu.SetQuestLog(questLog);
                Game1.activeClickableMenu = deluxeJournalMenu;
            }
        }

        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            if (IsMainScreen)
            {
                TaskManager?.Load();
            }

            if (PageManager != null && !Game1.onScreenMenus.OfType<JournalButton>().Any())
            {
                Game1.onScreenMenus.Add(new JournalButton());
            }
        }

        private void OnSaving(object? sender, SavingEventArgs e)
        {
            if (Config != null)
            {
                Helper.WriteConfig(Config);
            }

            if (IsMainScreen)
            {
                TaskManager?.Save();
            }
        }
    }
}
