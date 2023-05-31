/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jltaylor-us/StardewToDew
**
*************************************************/

// Copyright 2020 Jamie Taylor
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace ToDew {
    /// <summary>The configuration data model.</summary>
    public class ModConfig {
        public SButton hotkey = SButton.L;
        public KeybindList hotkeyList = new KeybindList();
        public SButton secondaryCloseButton = SButton.ControllerBack;
        public bool debug = false;
        public OverlayConfig overlay = new OverlayConfig();
    }
    /// <summary>The To-Dew mod.</summary>
    /// Encapsulates the game lifecycle events and orchestrates the UI (ToDoMenu) data
    /// model (ToDoList) bits.
    public class ModEntry : Mod {
        private readonly PerScreen<ToDoList?> list = new PerScreen<ToDoList?>();
        private readonly PerScreen<ToDoOverlay?> overlay = new PerScreen<ToDoOverlay?>();
        internal readonly PerScreen<OverlayDataSources> overlayDataSources = new(() => new OverlayDataSources());
        private readonly PerScreen<ToDoListOverlayDataSource> toDoListOverlayDataSource;
        internal ModConfig config = new(); // create (and throw away) a default value to keep nullability check happy

        public ModEntry() {
            toDoListOverlayDataSource = new(() => {
                IToDewApi? api = Helper.ModRegistry.GetApi<IToDewApi>(ModManifest.UniqueID);
                if (api is null) {
                    // this really shouldn't happen
                    api = new ToDoApiImpl(this, ModManifest);
                }
                ToDoListOverlayDataSource source = new(() => api.RefreshOverlay());
                api.AddOverlayDataSource(source);
                return source;
            });
        }

        public override IToDewApi GetApi(IModInfo mod) {
            return new ToDoApiImpl(this, mod.Manifest);
        }

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper) {
            I18n.Init(helper.Translation);
            this.config = helper.ReadConfig<ModConfig>();
            helper.Events.Input.ButtonsChanged += this.OnButtonsChanged;
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            helper.Events.Multiplayer.PeerConnected += this.OnPeerConnected;
            helper.Events.Multiplayer.ModMessageReceived += this.OnModMessageReceived;
            helper.Events.GameLoop.GameLaunched += onLaunched;
            helper.Events.GameLoop.ReturnedToTitle += this.OnReturnedToTitle;
            helper.Events.GameLoop.Saving += onSaving;
            helper.Events.GameLoop.DayStarted += onDayStarted;
        }

        private void onLaunched(object? sender, GameLaunchedEventArgs e) {
            // integrate with Generic Mod Config Menu, if installed
            var api = Helper.ModRegistry.GetApi<GenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");
            if (api != null) {
                var apiExt = Helper.ModRegistry.GetApi<GMCMOptionsAPI>("jltaylor-us.GMCMOptions");
                if (apiExt is null) {
                    Monitor.Log(I18n.Message_InstallGmcmOptions(modName: ModManifest.Name), LogLevel.Info);
                }
                api.Register(
                    mod: ModManifest,
                    reset: () => config = new ModConfig(),
                    save: () => { Helper.WriteConfig(config); overlay.Value?.ConfigSaved(); });
                api.AddKeybind(
                    mod: ModManifest,
                    name: I18n.Config_Hotkey,
                    tooltip: I18n.Config_Hotkey_Desc,
                    getValue: () => config.hotkey,
                    setValue: (SButton val) => config.hotkey = val);
                api.AddKeybind(
                    mod: ModManifest,
                    name: I18n.Config_SecondaryCloseButton,
                    tooltip: I18n.Config_SecondaryCloseButton_Desc,
                    getValue: () => config.secondaryCloseButton,
                    setValue: (SButton val) => config.secondaryCloseButton = val);
                api.AddBoolOption(
                    mod: ModManifest,
                    name: I18n.Config_Debug,
                    tooltip: I18n.Config_Debug_Desc,
                    getValue: () => config.debug,
                    setValue: (bool val) => config.debug = val);
                OverlayConfig.RegisterConfigMenuOptions(() => config.overlay, api, apiExt, ModManifest);
            }

            // integrate with MobilePhone, if installed
            var phoneApi = Helper.ModRegistry.GetApi<IMobilePhoneApi>("aedenthorn.MobilePhone");
            if (phoneApi != null) {
                // This is a whole lot of trouble to be able to use something out of one of the built-in
                // tile sheets, since the mobile phone api doesn't support specifying a rectangle for
                // the sprite.
                Texture2D originalTexture = Game1.content.Load<Texture2D>("Maps\\TownIndoors");
                Rectangle sourceRectangle = new Rectangle(202, 1870, 48, 48);
                Texture2D cropTexture = new Texture2D(Game1.graphics.GraphicsDevice, sourceRectangle.Width, sourceRectangle.Height);
                Color[] data = new Color[sourceRectangle.Width * sourceRectangle.Height];
                originalTexture.GetData(0, sourceRectangle, data, 0, data.Length);
                cropTexture.SetData(data);

                phoneApi.AddApp(Helper.ModRegistry.ModID, "To-Dew", () => { Game1.activeClickableMenu = new ToDoMenu(this, this.list!.Value!); }, cropTexture);
            }

            // add console commands
            Helper.ConsoleCommands.Add("todo-export", "Export the todo list to a file\n\n" + GetCommandExportHelp(), this.CommandExport);
            Helper.ConsoleCommands.Add("todo-import", "Import the todo list from a previously exported file\n\n" + GetCommandImportHelp(), this.CommandImport);
            Helper.ConsoleCommands.Add("todo-file-info", "Get information about a previously exported file\n\n" + GetCommandFileInfoHelp(), this.CommandFileInfo);
        }

        private void OnModMessageReceived(object? sender, ModMessageReceivedEventArgs e) {
            if (config.debug) {
                Monitor.Log($"Received mod message {e.Type} from {e.FromModID}", LogLevel.Debug);
            }
            if (e.FromModID.Equals(this.ModManifest.UniqueID)) {
                list.Value?.ReceiveModMessage(e);
            }
        }

        private void OnPeerConnected(object? sender, PeerConnectedEventArgs e) {
            if (config.debug) {
                this.Monitor.Log("OnPeerConnected", LogLevel.Debug);
            }
        }

        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e) {
            if (config.debug) {
                this.Monitor.Log("OnSaveLoaded", LogLevel.Debug);
                this.Monitor.Log($"My multiplayer ID: {Game1.player.UniqueMultiplayerID}", LogLevel.Debug);
            }
            list.Value = new ToDoList(this);
            toDoListOverlayDataSource.Value.theList = list.Value;
            if (config.overlay.enabled) {
                overlay.Value = new ToDoOverlay(this, overlayDataSources.Value);
            }
        }

        private void onDayStarted(object? sender, DayStartedEventArgs e) {
            list.Value?.RefreshVisibility();
        }

        private void onSaving(object? sender, SavingEventArgs e) {
            list.Value?.PreSaveCleanup();
        }

        private void OnReturnedToTitle(object? sender, ReturnedToTitleEventArgs e) {
            list.Value = null;
            toDoListOverlayDataSource.Value.theList = null;
            overlay.Value?.Dispose();
            overlay.Value = null;
        }

        private void OnButtonsChanged(object? sender, ButtonsChangedEventArgs e) {
            if (Context.IsWorldReady
                && Context.IsPlayerFree
                && list.Value != null
                && !this.list.Value.IncompatibleMultiplayerHost) {

                if (e.Pressed.Contains(this.config.hotkey) || this.config.hotkeyList.JustPressed()) {
                    if (Game1.activeClickableMenu != null)
                        Game1.exitActiveMenu();
                    Game1.activeClickableMenu = new ToDoMenu(this, this.list.Value);
                }
                if (e.Pressed.Contains(this.config.overlay.hotkey) || this.config.overlay.hotkeyList.JustPressed()) {
                    if (overlay.Value != null) {
                        overlay.Value.Dispose();
                        overlay.Value = null;
                    } else if (config.overlay.enabled) {
                        overlay.Value = new ToDoOverlay(this, overlayDataSources.Value);
                    }
                }
            }
        }

        private string GetCommandExportHelp() {
            return $@"Usage: todo-export [<path>]
Exports the current to-do list to a file path specified
relative to the mod's installation directory,
{Helper.DirectoryPath}.
If <path> is not specified then a default value of
list-exports/<farm save name>.json will be used
";
        }
        private void CommandExport(string command, string[] args) {
            if (list.Value != null) {
                if (args.Length == 0) {
                    list.Value.Export($"list-exports/{Constants.SaveFolderName}.json");
                } else if (args.Length == 1) {
                    list.Value.Export(args[0]);
                } else {
                    this.Monitor.Log(GetCommandExportHelp(), LogLevel.Info);
                }
            } else {
                this.Monitor.Log("Export command is only available while a game is loaded.", LogLevel.Info);
                if (args.Length == 1) {
                    this.Monitor.Log($"Filename that would have been used: {args[0]}", LogLevel.Debug);
                }
            }
        }

        private string GetCommandFileInfoHelp() {
            return $@"Usage: todo-file-info [-v|--verbose] <path>
Attempts to read the given file (specified as a path
relative to the mod's installation directory,
{Helper.DirectoryPath})
and print some basic information about it.
If the -v or --verbose flag is specified then also
print the text of each item in the file.
";
        }
        private void CommandFileInfo(string command, string[] args) {
            bool verbose = false;
            string path;

            if (args.Length < 1 || args.Length > 2) {
                this.Monitor.Log(GetCommandFileInfoHelp(), LogLevel.Info);
                return;
            }
            if (args.Length == 1) {
                path = args[0];
            } else {
                if (args[0] == "-v" || args[0] == "--verbose") {
                    verbose = true;
                } else {
                    this.Monitor.Log(GetCommandFileInfoHelp(), LogLevel.Info);
                    return;
                }
                path = args[1];
            }
            ToDoList.PrintFileInfo(this, path, verbose);
        }

        private string GetCommandImportHelp() {
            return $@"Usage: todo-import [<flag> ...] <path>
Import the given file (specified as a path
relative to the mod's installation directory,
{Helper.DirectoryPath})
The specific behavior of the import depends on the flags given.

Import type flag:  Only one of the following flags my be specified.  In the
following descriptions, ""current list"" means the to-do list in the currently
loaded game, and ""new list"" means the list read from <path>.
  --replace
    The current list is replaced by the new list.
  --update-only
    The current list is retained, but with each item replaced by the item
    of the same ID in the new list (if such an item exists).
  --append-update
    The current list is retained, but with each item replaced by the item
    of the same ID in the new list (if such an item exists), and any items in
    the new list that have an ID that does not match an item in the current
    list are appended to the current list.  This is the default import type.
  --prepend-update
    The current list is retained, but with each item replaced by the item
    of the same ID in the new list (if such an item exists), and any items in
    the new list that have an ID that does not match an item in the current
    list are prepended to the current list.
  --append-new
    The current list is retained, and the new list is appended, with each item
    in the new list receiving a new ID
  --prepend-new
    The current list is retained and the new list is prepended, with each item
    in the new list receiving a new ID

Other flags:
  -v, --verbose
    Print more verbose output about the import
  --force
    Force the import even if the data format used by the file has a newer
    major version number.
";
        }
        private static Dictionary<string, ToDoList.ImportType> importTypeMap = new Dictionary<string, ToDoList.ImportType> {
            ["--replace"] = ToDoList.ImportType.Replace,
            ["--update-only"] = ToDoList.ImportType.UpdateOnly,
            ["--append-update"] = ToDoList.ImportType.AppendUpdate,
            ["--prepend-update"] = ToDoList.ImportType.PrependUpdate,
            ["--append-new"] = ToDoList.ImportType.AppendNew,
            ["--prepend-new"] = ToDoList.ImportType.PrependNew
        };
        private void CommandImport(string command, string[] args) {
            if (list.Value == null) {
                this.Monitor.Log("Import command is only available while a game is loaded.", LogLevel.Info);
                return;
            }
            if (!Context.IsMainPlayer) {
                this.Monitor.Log("Import command is only available to the host player.", LogLevel.Info);
                return;
            }
            ToDoList.ImportType importType = ToDoList.ImportType.AppendUpdate;
            bool verbose = false;
            bool force = false;
            bool importTypeSet = false;
            for (int i = 0; i < args.Length; i++) {
                ToDoList.ImportType importTypeOut;
                if (args[i] == "-v" || args[i] == "--verbose") {
                    verbose = true;
                } else if (args[i] == "--force") {
                    force = true;
                } else if (importTypeMap.TryGetValue(args[i], out importTypeOut)) {
                    if (importTypeSet) {
                        this.Monitor.Log("Multiple import type flags specified; ignoring command", LogLevel.Warn);
                        this.Monitor.Log(GetCommandImportHelp(), LogLevel.Info);
                        return;
                    }
                    importType = importTypeOut;
                    importTypeSet = true;
                } else {
                    if (i == args.Length - 1) {
                        list.Value.Import(args[i], importType, force, verbose);
                        return;
                    } else {
                        this.Monitor.Log(GetCommandImportHelp(), LogLevel.Info);
                        return;
                    }
                }
            }
            this.Monitor.Log(GetCommandImportHelp(), LogLevel.Info);
        }

    }
    // See https://github.com/spacechase0/StardewValleyMods/blob/develop/GenericModConfigMenu/IGenericModConfigMenuApi.cs for full API
    public interface GenericModConfigMenuAPI {
        void Register(IManifest mod, Action reset, Action save, bool titleScreenOnly = false);
        void AddSectionTitle(IManifest mod, Func<string> text, Func<string>? tooltip = null);
        void AddBoolOption(IManifest mod, Func<bool> getValue, Action<bool> setValue, Func<string> name, Func<string>? tooltip = null, string? fieldId = null);
        void AddKeybind(IManifest mod, Func<SButton> getValue, Action<SButton> setValue, Func<string> name, Func<string>? tooltip = null, string? fieldId = null);
        void AddNumberOption(IManifest mod, Func<int> getValue, Action<int> setValue, Func<string> name, Func<string>? tooltip = null, int? min = null, int? max = null, int? interval = null, Func<int, string>? formatValue = null, string? fieldId = null);
        void AddTextOption(IManifest mod, Func<string> getValue, Action<string> setValue, Func<string> name, Func<string>? tooltip = null, string[]? allowedValues = null, Func<string, string>? formatAllowedValue = null, string? fieldId = null);
    }
    // See https://github.com/jltaylor-us/StardewGMCMOptions/blob/default/StardewGMCMOptions/IGMCMOptionsAPI.cs
    public interface GMCMOptionsAPI {
        void AddColorOption(IManifest mod, Func<Color> getValue, Action<Color> setValue, Func<string> name,
            Func<string>? tooltip = null, bool showAlpha = true, uint colorPickerStyle = 0, string? fieldId = null);
        #pragma warning disable format
        [Flags]
        public enum ColorPickerStyle : uint {
            Default = 0,
            RGBSliders    = 0b00000001,
            HSVColorWheel = 0b00000010,
            HSLColorWheel = 0b00000100,
            AllStyles     = 0b11111111,
            NoChooser     = 0,
            RadioChooser  = 0b01 << 8,
            ToggleChooser = 0b10 << 8
        }
        #pragma warning restore format
    }
    // See https://www.nexusmods.com/stardewvalley/articles/467
    public interface IMobilePhoneApi {
        bool AddApp(string id, string name, Action action, Texture2D icon);
    }
    public class ToDoApiImpl : IToDewApi {
        private readonly ModEntry theMod;
        private readonly IManifest clientMod;
        private readonly Dictionary<uint, ulong> sourceMap = new();
        private uint nextSourceId = 0;
        internal ToDoApiImpl(ModEntry theMod, IManifest clientMod) {
            this.theMod = theMod;
            this.clientMod = clientMod;
        }
        public uint AddOverlayDataSource(IToDewOverlayDataSource src) {
            sourceMap.Add(nextSourceId, theMod.overlayDataSources.Value.Add(src));
            return nextSourceId++;
        }

        public void RefreshOverlay() {
            theMod.overlayDataSources.Value.Refresh();
        }

        public void RemoveOverlayDataSource(uint handle) {
            if (sourceMap.TryGetValue(handle, out ulong globalId)) {
                theMod.overlayDataSources.Value.Remove(globalId);
                sourceMap.Remove(handle);
            } else {
                theMod.Monitor.Log($"{clientMod.Name} tried to remove overlay datasource with ID {handle}, but there is no datasource with that ID.  Ignoring.", LogLevel.Warn);
            }
        }
    }
}
