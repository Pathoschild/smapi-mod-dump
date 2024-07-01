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
using Newtonsoft.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using DeluxeJournal.Framework;
using DeluxeJournal.Framework.Data;
using DeluxeJournal.Framework.Events;
using DeluxeJournal.Framework.Task;
using DeluxeJournal.Menus;
using DeluxeJournal.Menus.Components;
using DeluxeJournal.Patching;
using DeluxeJournal.Task;
using DeluxeJournal.Task.Tasks;

namespace DeluxeJournal
{
    /// <summary>The mod entry point.</summary>
    internal sealed class DeluxeJournalMod : Mod
    {
        /// <summary>Data key for the notes save data.</summary>
        public const string NotesDataKey = "notes-data";

        /// <summary>Color schema data file path.</summary>
        public const string ColorDataPath = "assets/data/colors";

        /// <summary>Default color schema data file path.</summary>
        public const string ColorDataDefault = $"{ColorDataPath}/default.json";

        /// <summary>UI spirte sheet texture.</summary>
        public static Texture2D? UiTexture { get; private set; }

        /// <summary>Animal icon spirte sheet texture.</summary>
        public static Texture2D? AnimalIconsTexture { get; private set; }

        /// <summary>Building icon spirte sheet texture.</summary>
        public static Texture2D? BuildingIconsTexture { get; private set; }

        /// <summary>Transparency mask for a colored task entry.</summary>
        public static Texture2D? ColoredTaskMask { get; private set; }

        /// <summary>Loaded color schema data.</summary>
        public static IList<ColorSchema> ColorSchemas { get; private set; } = [ColorSchema.ErrorSchema];

        /// <summary>The color of the task entry border.</summary>
        public static Color TaskBorderColor { get; private set; } = new(68, 18, 28);

        /// <summary>The mod instance.</summary>
        public static DeluxeJournalMod? Instance { get; private set; }

        /// <summary>Translation helper.</summary>
        public static ITranslationHelper? Translation { get; private set; }

        /// <summary>Configuration settings.</summary>
        public static Config? Config { get; private set; }

        /// <summary>Event manager for handling event subscriptions.</summary>
        public static EventManager? EventManager { get; private set; }

        /// <summary>Task manager.</summary>
        public static TaskManager? TaskManager { get; private set; }

        /// <summary>Overlay manager.</summary>
        public static OverlayManager? OverlayManager { get; private set; }

        /// <summary>Notes save data.</summary>
        private NotesData? NotesData { get; set; }

        public override void Entry(IModHelper helper)
        {
            Instance = this;
            Translation = helper.Translation;

            RuntimeHelpers.RunClassConstructor(typeof(TaskTypes).TypeHandle);

            UiTexture = helper.ModContent.Load<Texture2D>("assets/ui.png");
            AnimalIconsTexture = helper.ModContent.Load<Texture2D>("assets/animal-icons.png");
            BuildingIconsTexture = helper.ModContent.Load<Texture2D>("assets/building-icons.png");
            ColoredTaskMask = helper.ModContent.Load<Texture2D>("assets/colored-task-mask.png");
            SmartIconComponent.AnimalIconIds = helper.ModContent.Load<Dictionary<string, int>>("assets/data/animal-icons.json");
            SmartIconComponent.BuildingIconData = helper.ModContent.Load<Dictionary<string, BuildingIconData>>("assets/data/building-icons.json");

            Config = helper.ReadConfig<Config>();
            NotesData = helper.Data.ReadGlobalData<NotesData>(NotesDataKey) ?? new NotesData();

            EventManager = new EventManager(helper.Events, helper.Multiplayer, Monitor);
            TaskManager = new TaskManager(EventManager, helper.Data, Config, ModManifest.Version);
            OverlayManager = new OverlayManager(helper.Events, helper.Data, Config);

            PageRegistry.Register("quests", (bounds) => new QuestLogPage("quests", bounds, UiTexture, helper.Translation), null, 999);
            PageRegistry.Register("tasks", (bounds) => new TasksPage("tasks", bounds, UiTexture, helper.Translation), (bounds) => new TasksOverlay(bounds, helper.Input), 998);
            PageRegistry.Register("notes", (bounds) => new NotesPage("notes", bounds, UiTexture, helper.Translation, GetNotes()), (bounds) => new NotesOverlay(bounds, GetNotes()), 997);
            PageRegistry.Register("overlays", (bounds) => new OverlaysPage("overlays", bounds, UiTexture, helper.Translation), null, 996);

            helper.Events.Display.MenuChanged += OnMenuChanged;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.Saving += OnSaving;

            Patcher.Apply(new Harmony(ModManifest.UniqueID), Monitor,
                new QuestLogPatch(Monitor),
                new FarmerPatch(EventManager, Monitor),
                new CarpenterMenuPatch(EventManager, Monitor),
                new PurchaseAnimalsMenuPatch(EventManager, Monitor),
                new ShopMenuPatch(EventManager, Monitor)
            );

            ConsoleCommands.AddCommands(helper.ConsoleCommands, Monitor);

#if DEBUG
            Program.enableCheats = true;
#endif
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
                Helper.Data.WriteGlobalData(NotesDataKey, NotesData);

                foreach (IClickableMenu menu in Game1.onScreenMenus)
                {
                    if (menu is NotesOverlay overlay)
                    {
                        overlay.UpdateText(text);
                        return;
                    }
                }
            }
        }

        /// <summary>Load the color schemas.</summary>
        /// <param name="relativePath">Optional file path relative to the mod folder. If <c>null</c>, attempts to load a custom file first, then falls back on the default.</param>
        /// <param name="tryNext">Attempt to load the next available color file on failure. Attempts to load the default first. Ignored if <paramref name="relativePath"/> is <c>null</c>.</param>
        /// <returns>The relative file path of the loaded color data.</returns>
        /// <exception cref="ContentLoadException">Could not load a color schema.</exception>
        public string LoadColorSchemas(string? relativePath = null, bool tryNext = true)
        {
            string loadedPath = string.Empty;
            IEnumerable<string> paths = Directory.GetFiles($"{Helper.DirectoryPath}/{ColorDataPath}")
                .Select(f => $"{ColorDataPath}/{Path.GetFileName(f)}")
                .Where(f => f != ColorDataDefault);

            if (relativePath == null)
            {
                paths = paths.Append(ColorDataDefault);
            }
            else
            {
                IEnumerable<string> target = [relativePath];
                paths = tryNext ? target.Append(ColorDataDefault).Concat(paths) : target;
            }

            foreach (string path in paths)
            {
                try
                {
                    if (Helper.Data.ReadJsonFile<ColorData>(path)?.Colors is IList<ColorSchema> customColors)
                    {
                        ColorSchemas = customColors;
                        loadedPath = path;
                        break;
                    }
                    else
                    {
                        Monitor.Log($"Unable to load color data from '{path}' ... skipping", LogLevel.Warn);
                    }
                }
                catch(JsonReaderException)
                {
                    Monitor.Log($"Unable to parse color data from '{path}' ... skipping");
                }
            }

            if (string.IsNullOrEmpty(loadedPath))
            {
                throw new ContentLoadException($"Could not load color data. Do not remove or modify '{ColorDataDefault}'.");
            }

            if (Game1.mouseCursors != null)
            {
                ColorSchemas.Insert(0, ColorSchema.ExtractFromTextureBox(Game1.mouseCursors, new(384, 396, 15, 15), out Color border));
                TaskBorderColor = border;
            }
            else
            {
                ColorSchemas.Insert(0, ColorSchema.ErrorSchema);
                Monitor.Log("Color schemas loaded before game textures. Unable to extract default color schema.\n\tTry running the following command: dj_colors_load", LogLevel.Error);
            }

            return loadedPath;
        }

        /// <summary>Save the color schemas.</summary>
        /// <param name="relativePath">Optional file path relative to the mod folder. Overwrites default file if <c>null</c>.</param>
        /// <exception cref="InvalidOperationException">Relative path is not within the colors data folder.</exception>
        public void SaveColorSchemas(string? relativePath = null)
        {
            if (relativePath?.StartsWith(ColorDataPath) == false)
            {
                throw new InvalidOperationException($"Relative path is not within '{ColorDataPath}/'.");
            }

            Helper.Data.WriteJsonFile(relativePath ?? ColorDataDefault, new ColorData(ColorSchemas.Skip(1).ToList()));
        }

        [EventPriority(EventPriority.Low)]
        private void OnMenuChanged(object? sender, MenuChangedEventArgs e)
        {
            // Hijack QuestLog and replace it with DeluxeJournalMenu
            if (Game1.activeClickableMenu is QuestLog questLog)
            {
                DeluxeJournalMenu deluxeJournalMenu = new DeluxeJournalMenu();
                deluxeJournalMenu.SetQuestLog(questLog);
                Game1.activeClickableMenu = deluxeJournalMenu;
            }
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            if (Context.IsMainPlayer)
            {
                LoadColorSchemas(string.IsNullOrEmpty(Config?.TargetColorSchemaFile) ? null : $"{ColorDataPath}/{Config.TargetColorSchemaFile}");
            }
        }

        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            if (!Game1.onScreenMenus.OfType<JournalButton>().Any())
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
        }
    }
}
