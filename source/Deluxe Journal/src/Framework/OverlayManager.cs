/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using DeluxeJournal.Framework.Data;
using DeluxeJournal.Menus;
using DeluxeJournal.Task;

namespace DeluxeJournal.Framework
{
    internal class OverlayManager
    {
        /// <summary>Data key for the overlay settings.</summary>
        public const string OverlaySettingsKey = "overlay-settings";

        private readonly IDataHelper _dataHelper;
        private readonly Config _config;
        private readonly PerScreen<Dictionary<string, IOverlay>> _overlays;
        private readonly PerScreen<Dictionary<string, OverlaySettings>> _settings;
        private readonly PerScreen<KeybindList> _toggleKeybind;
        private readonly PerScreen<bool> _toggleVisible;

        /// <summary>Active on-screen <see cref="IOverlay"/> menus by their registered page IDs.</summary>
        public IReadOnlyDictionary<string, IOverlay> Overlays => _overlays.Value;

        /// <summary>Keybind for toggling the visibility of overlays.</summary>
        public KeybindList ToggleKeybind
        {
            get => _toggleKeybind.Value;
            set
            {
                _toggleKeybind.Value = value;

                if (Context.IsMainPlayer)
                {
                    _config.ToggleOverlaysKeybind = value;
                }
            }
        }

        /// <summary>Whether the overlays have been toggled visible or not (hotkey).</summary>
        private bool ToggleVisible
        {
            get => _toggleVisible.Value;
            set => _toggleVisible.Value = value;
        }

        /// <summary>Mapping of <see cref="OverlaySettings"/> by their registered page IDs.</summary>
        /// <remarks>Lazy instantiation allows <see cref="Game1.options"/> to load first.</remarks>
        private Dictionary<string, OverlaySettings> Settings => _settings.Value;

        public OverlayManager(IModEvents events, IDataHelper dataHelper, Config config)
        {
            _dataHelper = dataHelper;
            _config = config;
            _overlays = new(() => new(2));
            _settings = new(GetOrCreateSettings);
            _toggleKeybind = new(() => _config.ToggleOverlaysKeybind);
            _toggleVisible = new();

            if (ColorSchema.HexToColor(config.OverlayBackgroundColor) is Color backgroundColor)
            {
                IOverlay.BackgroundColor = backgroundColor;
            }

            events.Display.WindowResized += OnWindowResized;
            events.GameLoop.SaveLoaded += OnSaveLoaded;
            events.GameLoop.ReturnedToTitle += OnReturnToTitle;
            events.Input.ButtonsChanged += OnButtonsChanged;
        }

        /// <summary>Set the overlay background color.</summary>
        public void SetBackgroundColor(Color color)
        {
            IOverlay.BackgroundColor = color;

            if (Context.IsMainPlayer)
            {
                _config.OverlayBackgroundColor = ColorSchema.ColorToHex(color, true);
                _config.Save();
            }
        }

        /// <summary>Set the <see cref="IOverlay.IsEditing"/> flag in each visible overlay.</summary>
        /// <param name="editing">Whether the overlays are in edit-mode.</param>
        public void SetEditing(bool editing)
        {
            foreach (IOverlay overlay in Overlays.Values)
            {
                if (overlay.IsVisible)
                {
                    overlay.IsEditing = editing;
                }
            }
        }

        /// <summary>Save the overlay settings.</summary>
        public void SaveSettings()
        {
            UpdateSettings();

            if (Context.IsMainPlayer)
            {
                _dataHelper.WriteGlobalData(OverlaySettingsKey, _settings.Value);
            }
        }

        /// <summary>Restore the state of each overlay to their saved settings.</summary>
        public void RestoreSettings()
        {
            foreach (string key in Overlays.Keys)
            {
                if (Settings.TryGetValue(key, out OverlaySettings? settings))
                {
                    settings.Apply(Overlays[key]);
                }
            }
        }

        /// <summary>Update the overlay settings with the state of each active overlay.</summary>
        private void UpdateSettings()
        {
            ToggleVisible = false;

            foreach (string key in Overlays.Keys)
            {
                if (!Settings.TryGetValue(key, out OverlaySettings? settings))
                {
                    settings = OverlaySettings.NewDefault();
                }

                IOverlay overlay = Overlays[key];
                settings.Update(overlay);
                Settings.TryAdd(key, settings);

                if (overlay.IsVisible && !overlay.IsVisibilityLocked)
                {
                    ToggleVisible = true;
                }
            }
        }

        private void OnWindowResized(object? sender, WindowResizedEventArgs e)
        {
            UpdateSettings();
        }

        private void OnButtonsChanged(object? sender, ButtonsChangedEventArgs e)
        {
            if (Game1.activeClickableMenu == null && ToggleKeybind.JustPressed())
            {
                ToggleVisible = !ToggleVisible;

                foreach (var overlay in Overlays.Values)
                {
                    if (!overlay.IsVisibilityLocked)
                    {
                        overlay.IsVisible = ToggleVisible;
                    }
                }

                Game1.playSound(ToggleVisible ? "breathin" : "breathout", 1800);
            }
        }

        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            ToggleVisible = false;

            foreach (string key in PageRegistry.Keys)
            {
                // TODO: Disable notes overlay for farmhands. Remove this when notes are given better split-screen support.
                if (!Context.IsMainPlayer && key == "notes")
                {
                    continue;
                }

                if (!Settings.TryGetValue(key, out OverlaySettings? settings))
                {
                    settings = OverlaySettings.NewDefault();
                }

                if (PageRegistry.CreateOverlay(key, settings) is IOverlay overlay)
                {
                    Settings.TryAdd(key, settings);
                    _overlays.Value.Add(key, overlay);
                    Game1.onScreenMenus.Add(overlay);

                    if (overlay.IsVisible && !overlay.IsVisibilityLocked)
                    {
                        ToggleVisible = true;
                    }
                }
            }
        }

        private void OnReturnToTitle(object? sender, ReturnedToTitleEventArgs e)
        {
            SaveSettings();
            DisposeOverlays(Context.ScreenId);
        }

        private void DisposeOverlays(int screenId)
        {
            Dictionary<string, IOverlay> overlays = _overlays.GetValueForScreen(screenId);

            foreach (IOverlay overlay in overlays.Values)
            {
                overlay.Dispose();
            }

            overlays.Clear();
        }

        private Dictionary<string, OverlaySettings> GetOrCreateSettings()
        {
            if (!Context.IsMainPlayer)
            {
                Dictionary<string, OverlaySettings> settings = new(2);

                foreach ((string key, IOverlay overlay) in _overlays.GetValueForScreen(0))
                {
                    OverlaySettings copy = OverlaySettings.NewDefault();
                    copy.Update(overlay);

                    // Force notes off and locked since they do not work for farmhands.
                    if (overlay is NotesOverlay)
                    {
                        copy.IsVisible = false;
                        copy.IsVisibilityLocked = true;
                    }

                    settings.Add(key, copy);
                }

                return settings;
            }

            Point defaultSize = new(500);
            Point notesPosition = new(0, defaultSize.Y);

            if (Game1.options is Options options)
            {
                notesPosition.Y = (int)Math.Ceiling(Game1.viewport.Height * options.zoomLevel / options.uiScale) - defaultSize.Y;
            }

            return _dataHelper.ReadGlobalData<Dictionary<string, OverlaySettings>>(OverlaySettingsKey) ?? new()
            {
                { "tasks", new(new(Point.Zero, defaultSize), true, false, false, Color.White) },
                { "notes", new(new(notesPosition, defaultSize), false, true, true, Color.White) }
            };
        }
    }
}
