using System;
using System.Collections.Generic;
using System.Linq;
using AutoGrabberMod.Features;
using AutoGrabberMod.Models;
using AutoGrabberMod.UserInterfaces;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace AutoGrabberMod
{
    /// <summary>The mod entry class loaded by SMAPI.</summary>
    public class ModEntry : Mod
    {
        private AutoGrabber _currentGrabber;
        private bool IsLoaded => Context.IsWorldReady;
        private static readonly Dictionary<string, AutoGrabber> AutoGrabbers = new Dictionary<string, AutoGrabber>();

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Utilities.Monitor = Monitor;
            Utilities.Helper = helper;
            Utilities.Config = helper.ReadConfig<ModConfig>();
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.World.ObjectListChanged += OnObjectListChanged;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.Display.MenuChanged += OnMenuChanged;

            helper.Events.Display.RenderingHud += OnRenderingHud;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            helper.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;

            //var featureType = typeof(Feature);
            //Utilities.FeatureTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(p => featureType.IsAssignableFrom(p) && !p.Equals(featureType)).ToArray();
            Utilities.FeatureTypes = new Type[]
            {
                typeof(DigArtifacts),
                typeof(Fertilize),
                typeof(Forage),
                typeof(Harvest),
                typeof(HoeTiles),
                typeof(PetAnimals),
                typeof(PlantSeeds),
                typeof(WaterFields)
            };
        }

        /// <summary>Raised after the game returns to the title screen.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            _currentGrabber = null;
            AutoGrabbers.Clear();
        }

        /// <summary>Raised after the game begins a new day (including when the player loads a save).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            //Monitor.Log($"Day Started {Context.IsWorldReady}", LogLevel.Trace);

            //create instances if not exist
            if (!AutoGrabbers.Any())
            {
                var list = (from location in Utilities.GetAllLocations()
                            where location != null && location.Objects != null && location.Objects.Any() && location.Objects.Pairs.Any((arg) => arg.Value != null && arg.Value.ParentSheetIndex == AutoGrabber.ParentIndex)
                            select location.Objects.Pairs.Where((arg) => arg.Value.ParentSheetIndex == AutoGrabber.ParentIndex).Select((arg) => new AutoGrabber(location, arg.Value, arg.Key))).SelectMany((arg) => arg);
                foreach (var item in list)
                {
                    if (!AutoGrabbers.ContainsKey(item.Id))
                    {
                        AutoGrabbers.Add(item.Id, item);
                    }
                }
            }

            try
            {
                foreach (var grabber in AutoGrabbers)
                {
                    if (grabber.Value.Location.Objects.ContainsKey(grabber.Value.Tile))
                    {
                        grabber.Value.Action();
                    }
                    else
                    {
                        AutoGrabbers.Remove(grabber.Key);
                    }
                }
            }
            catch(Exception error)
            {
                Monitor.Log($"An error occurred while running Actions", LogLevel.Info);
                Monitor.Log($"{error.TargetSite} {error.Message}: {error.StackTrace}", LogLevel.Error);
            }
        }

        /// <summary>Raised after objects are added or removed in a location.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnObjectListChanged(object sender, ObjectListChangedEventArgs e)
        {
            if (e.Added.Any())
            {
                foreach (KeyValuePair<Vector2, StardewValley.Object> pair in e.Added)
                {
                    //Monitor.Log($"  Object Added: {pair.Value.Name} {pair.Value.ParentSheetIndex}, {e.Location.Name}-{pair.Key.X},{pair.Key.Y}", LogLevel.Trace);

                    //Handle creating new instances
                    if (pair.Value.parentSheetIndex == AutoGrabber.ParentIndex)
                    {
                        var g = new AutoGrabber(e.Location, pair.Value, pair.Key);
                        if (!AutoGrabbers.ContainsKey(g.Id))
                        {
                            Monitor.Log($"  - Adding Instance {g.Id}");
                            AutoGrabbers.Add(g.Id, g);
                        }                        
                    }
                }

                foreach (var grabber in AutoGrabbers.Where(g => (bool)g.Value.FeatureType<Forage>().Value).ToArray())
                {
                    if (grabber.Value.Location.Name == e.Location.Name)
                    {
                        (grabber.Value.FeatureType<Forage>() as Forage).ActionItemAddedRemoved(sender, e);
                    }
                }
            }
            if (e.Removed.Any())
            {
                foreach (KeyValuePair<Vector2, StardewValley.Object> pair in e.Removed)
                {
                    //Monitor.Log($"  Object Removed: {pair.Value.Name}, ID: {pair.Value.ParentSheetIndex}, Location: {e.Location.Name}-{pair.Key.X},{pair.Key.Y}", LogLevel.Trace);

                    //Handle removing instances
                    if (pair.Value.ParentSheetIndex == AutoGrabber.ParentIndex)
                    {
                        var grabber = AutoGrabbers.Values.Where(g => g.Location.Equals(e.Location) && g.Tile.Equals(pair.Key)).FirstOrDefault();
                        if (grabber != null)
                        {
                            Monitor.Log($"  Cleaning instance {grabber.Id}");
                            AutoGrabbers.Remove(grabber.Id);
                        }
                    }
                }
            }
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!this.IsLoaded || !Context.IsPlayerFree) return;
            if (e.Button == Utilities.Config.OpenMenuKey) OpenMenu();
        }

        private void OpenMenu()
        {
            if (Game1.activeClickableMenu != null)
            {
                //Monitor.Log("  Closing current menu", StardewModdingAPI.LogLevel.Trace);
                Game1.exitActiveMenu();
            }
            else
            {
                Monitor.Log("  Active clickable menu seems to be null", StardewModdingAPI.LogLevel.Trace);
            }

            if (!AutoGrabbers.Any())
            {
                Game1.addHUDMessage(new HUDMessage($"No Auto-Grabbers found, please purchase one from Marnie", 3));
            }
            else
            {
                int index = 0;
                var values = AutoGrabbers.Values.ToList();
                var grabber = values.FirstOrDefault(g => g.Location == Game1.currentLocation && g.Tile == Game1.currentCursorTile) ?? values.FirstOrDefault(g => g.Location == Game1.currentLocation);
                if (grabber != null)
                {
                    index = values.FindIndex(g => grabber == g);
                }
                Game1.activeClickableMenu = new MenuContainer(values.ToArray(), index, Utilities.Config);
            }
        }

        /// <summary>Raised after a game menu is opened, closed, or replaced.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.OldMenu is MenuContainer)
            {
                //Monitor.Log("Menu was closed updating Global settings");
                Helper.WriteConfig(Utilities.Config);
            }
        }

        /// <summary>Raised after the game state is updated (≈60 times per second).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            // get tile under cursor
            if (e.IsMultipleOf(4) && Game1.currentLocation != null)
            {
                if (Game1.currentLocation.Objects == null || !AutoGrabbers.TryGetValue(AutoGrabber.MakeId(Game1.currentLocation, Game1.currentCursorTile), out _currentGrabber))
                {
                    _currentGrabber = null;
                    foreach (var g in AutoGrabbers.Values) g.IsMouseOver = false;
                }
                else
                {                    
                    _currentGrabber.IsMouseOver = true;
                }
            }
        }

        /// <summary>Raised before drawing the HUD (item toolbar, clock, etc) to the screen. The vanilla HUD may be hidden at this point (e.g. because a menu is open).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnRenderingHud(object sender, RenderingHudEventArgs e)
        {
            // draw hover range
            var grabbers = AutoGrabbers.Values.Where(g => g.Location.Equals(Game1.currentLocation));
            if (_currentGrabber != null && !_currentGrabber.ShowRange)
            {
                if (!_currentGrabber.RangeEntireMap && Utilities.Config.ShowRangeGridMouseOver) _currentGrabber.DrawTileOutlines();
                _currentGrabber.DrawNameTooltip();
            }
            foreach (var g in (Utilities.Config.ShowAllRangeGrids ? grabbers : grabbers.Where(g => g.ShowRange))) g.DrawTileOutlines();
        }
    }
}
