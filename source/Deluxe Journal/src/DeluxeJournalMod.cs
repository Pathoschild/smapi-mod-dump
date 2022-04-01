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
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using DeluxeJournal.Api;
using DeluxeJournal.Framework;
using DeluxeJournal.Framework.Data;
using DeluxeJournal.Framework.Events;
using DeluxeJournal.Framework.Tasks;
using DeluxeJournal.Menus;

namespace DeluxeJournal
{
    /// <summary>The mod entry point.</summary>
    internal class DeluxeJournalMod : Mod
    {
        public const string NOTES_DATA_KEY = "notes-data";
        public const string TASKS_DATA_KEY = "tasks-data";

        private static DeluxeJournalMod? _instance;

        public static DeluxeJournalMod? Instance => _instance;

        public static Texture2D? UiTexture { get; private set; }

        public static Texture2D? CharacterIconsTexture { get; private set; }

        private NotesData? _notesData;

        public Config? Config { get; private set; }

        public TaskEventManager? TaskEventManager { get; private set; }

        public TaskManager? TaskManager { get; private set; }

        public PageManager? PageManager { get; private set; }

        public override void Entry(IModHelper helper)
        {
            _instance = this;

            RuntimeHelpers.RunClassConstructor(typeof(TaskTypes).TypeHandle);

            UiTexture = helper.Content.Load<Texture2D>("assets/ui.png");
            CharacterIconsTexture = helper.Content.Load<Texture2D>("assets/character-icons.png");
            Config = helper.ReadConfig<Config>();

            PageManager = new PageManager();
            PageManager.RegisterPage("notes", (bounds) => new NotesPage(bounds, UiTexture, helper.Translation), 100);
            PageManager.RegisterPage("tasks", (bounds) => new TasksPage(bounds, UiTexture, helper.Translation), 101);
            PageManager.RegisterPage("quests", (bounds) => new QuestsPage(bounds, UiTexture, helper.Translation), 102);

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.GameLoop.DayEnding += OnDayEnding;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.Saving += OnSaving;
            helper.Events.Display.RenderingActiveMenu += OnRenderingActiveMenu;
        }

        public string GetNotes()
        {
            if (_notesData != null && _notesData.Text.ContainsKey(Constants.SaveFolderName))
            {
                return _notesData.Text[Constants.SaveFolderName];
            }

            return string.Empty;
        }

        public void SaveNotes(string text)
        {
            if (_notesData != null)
            {
                _notesData.Text[Constants.SaveFolderName] = text;
                Helper.Data.WriteGlobalData(NOTES_DATA_KEY, _notesData);
            }
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            TaskEventManager = new TaskEventManager(Helper.Events, Monitor);
            TaskManager = new TaskManager(new TaskEvents(TaskEventManager), Helper.Data);

            _notesData = Helper.Data.ReadGlobalData<NotesData>(NOTES_DATA_KEY) ?? new NotesData();
        }

        public void OnDayStarted(object? sender, DayStartedEventArgs e)
        {
            TaskEventManager?.DeployListeners();
        }

        private void OnDayEnding(object? sender, DayEndingEventArgs e)
        {
            TaskManager?.OnDayEnding();
            TaskEventManager?.CleanupListeners();
        }

        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            TaskManager?.Load();

            if (PageManager != null)
            {
                Game1.onScreenMenus.Add(new JournalButton(PageManager, Helper.Translation));
            }
        }

        private void OnSaving(object? sender, SavingEventArgs e)
        {
            if (Config != null)
            {
                Helper.WriteConfig(Config);
            }

            TaskManager?.Save();
        }

        [EventPriority(EventPriority.Low)]
        private void OnRenderingActiveMenu(object? sender, RenderingActiveMenuEventArgs e)
        {
            // Hijack vanilla QuestLog and replace it with DeluxeJournalMenu before rendering
            if (Game1.activeClickableMenu is QuestLog && PageManager != null)
            {
                Game1.activeClickableMenu = new DeluxeJournalMenu(PageManager);
            }
        }

        public override object GetApi()
        {
            return new DeluxeJournalApi(this);
        }
    }
}
