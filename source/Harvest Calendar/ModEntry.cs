/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LonerAxl/Stardew_HarvestCalendar
**
*************************************************/

using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewModdingAPI.Utilities;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Menus;
using StardewValley.TerrainFeatures;
using System.Linq;
using HarvestCalendar.Framework;
using HarvestCalendar.Menu;

namespace HarvestCalendar
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {

        Dictionary<int, CalendarDayItem> CalendarDayDict = new();
        internal Configuration Config = null!;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            I18n.Init(helper.Translation);
            this.Config = helper.ReadConfig<Configuration>();
            this.Config.AddHelper(helper);

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;

            helper.Events.Input.ButtonsChanged += OnButtonChanged;
            helper.Events.Display.MenuChanged += this.OnCalendarOpen;
            helper.Events.Display.MenuChanged += this.OnCalendarClosed;
            helper.Events.Display.RenderedActiveMenu += this.OnRenderedActiveCalendar;
            
        }


        /*********
        ** Private methods
        *********/
        /// <summary>
        /// Set up Generic Mod Config Menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu == null)
                return;
            configMenu.Register(
                mod: this.ModManifest,
                reset: () => this.Config = new Configuration(),
                save: () => this.Helper.WriteConfig(this.Config)
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => I18n.ToggleMod(),
                tooltip: () => I18n.ToggleMod_Desccription(),
                getValue: () => this.Config.ToggleMod,
                setValue: value => this.Config.ToggleMod = value
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => I18n.IconSize(),
                tooltip: () => I18n.IconSize_Desccription(),
                getValue: () => this.Config.IconSize,
                setValue: value => this.Config.IconSize = (int)value,
                min: 1, max: 4,
                interval: 1,
                formatValue: value => {
                    string[] _ = { I18n.IconSize_Small(),
                                I18n.IconSize_Medium(),
                                I18n.IconSize_Large(),
                                I18n.IconSize_XLarge() };
                    return _[(int)value - 1];
                }
            );
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => I18n.IconPositionX(),
                tooltip: () => I18n.IconPositionX_Description(),
                getValue: () => this.Config.IconX,
                setValue: value => this.Config.IconX = value,
                min: 0f, max: 1f,
                interval: 0.1f
            );
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => I18n.IconPositionY(),
                tooltip: () => I18n.IconPositionY_Description(),
                getValue: () => this.Config.IconY,
                setValue: value => this.Config.IconY = value,
                min: 0f, max: 1f,
                interval: 0.1f
            );
            configMenu.AddKeybindList(
                mod: this.ModManifest,
                name: () => I18n.ToggleBlackList(),
                tooltip: () => I18n.ToggleBlackList_Description(),
                getValue: () => this.Config.ToggleBlackListKeybind,
                setValue: val => this.Config.ToggleBlackListKeybind = val
            );
            configMenu.AddKeybindList(
                mod: this.ModManifest,
                name: () => I18n.BlacklistCropKeybind(),
                tooltip: () => I18n.BlacklistCropKeybind_Description(),
                getValue: () => this.Config.BlacklistTheCropKeybind,
                setValue: val => this.Config.BlacklistTheCropKeybind = val
            );
            configMenu.AddKeybindList(
                mod: this.ModManifest,
                name: () => I18n.ToggleCalendarDayDetail(),
                tooltip: () => I18n.ToggleCalendarDayDetail_Description(),
                getValue: () => this.Config.ToggleCalendarDayDetailKeybind,
                setValue: val => this.Config.ToggleCalendarDayDetailKeybind = val
            );
        }

        

        /// <summary>when the player open the Calendar, calculate all crops on the field and predict the harvest day then show it on the calendar.</summary>
        private void OnCalendarOpen(object? sender, MenuChangedEventArgs e) 
        {
            if (!this.Config.ToggleMod)
                return;
            // Check if is on Calendar menu
            if (this.IsCalendarPage())
            {
                if (this.CalendarDayDict.Count != 0)
                    return;

                // TODO: Currently not support multiplayer as farmhands can't get all locations? Not sure

                int today = Game1.dayOfMonth;
                Dictionary<(int, string, string), int> allCropsHarvestDay = new();

                foreach (GameLocation location in Game1.locations)
                {
                    if (!(location.IsFarm || location.IsGreenhouse || location.InIslandContext()))
                        continue;
                    allCropsHarvestDay = allCropsHarvestDay.Concat(GetAllCropsbyLocation(location)).ToDictionary(kvp => kvp.Key, kvp=> kvp.Value);
                }

                // sum up number of crops by day and cropId, to get the cropId with the largest amount, so that the icon will be showed on the calendar
                var iconQuery = allCropsHarvestDay.GroupBy(x => new { day = x.Key.Item1, cropId = x.Key.Item3 })
                                                .Select(g => new
                                                {
                                                    g.Key.day,
                                                    g.Key.cropId,
                                                    count = g.Sum(t => t.Value)
                                                }).OrderBy(g=>g.day).ThenByDescending(g=>g.count).GroupBy(g=>g.day);

                var countQuery = allCropsHarvestDay.OrderBy(x=>x.Key.Item1).ThenBy(x=>x.Key.Item2).ThenBy(x=>x.Key.Item3);

                foreach (var i in iconQuery)
                {
                    foreach (var j in i)
                    {
                        CalendarDayDict.Add(j.day, new CalendarDayItem(j.day, j.cropId));
                        break;
                    }
                }

                foreach (var i in countQuery) 
                {
                    CalendarDayDict[i.Key.Item1].AddCrops(i.Key.Item2, i.Key.Item3, i.Value);
                }
                
            } 
        }


        /// <summary>
        /// draw the icon on calendar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRenderedActiveCalendar(object? sender, RenderedActiveMenuEventArgs e) 
        {
            if (!this.Config.ToggleMod)
                return;
            if (Game1.activeClickableMenu == null || !IsCalendarPage())
                return;

            if (Game1.activeClickableMenu is Billboard billboard)
            {
                List<ClickableTextureComponent> days = billboard.calendarDays;

                int today = Game1.dayOfMonth;
                for (int i = today; i <= 28; i++)
                {
                    if (!CalendarDayDict.ContainsKey(i))
                        continue;
                    CalendarDayItem item = CalendarDayDict[i];
                    var produce = ItemRegistry.GetDataOrErrorItem(item.iconId);
                    var iconTexture = produce.GetTexture();

                    int offsetX = days[i - 1].bounds.Width / 10 * this.Config.IconSize;
                    int offsetY = days[i - 1].bounds.Height / 10 * this.Config.IconSize;

                    Vector2 position = new Vector2((days[i - 1].bounds.Width - offsetX) * this.Config.IconX + days[i - 1].bounds.Left,
                                                    (days[i - 1].bounds.Height - offsetY) * this.Config.IconY + days[i - 1].bounds.Top);

                    e.SpriteBatch.Draw(iconTexture, new Rectangle((int)position.X, (int)position.Y, offsetX, offsetY), produce.GetSourceRect(), Color.White);

                }

                // Redraw the cursor
                billboard.drawMouse(e.SpriteBatch);

                string text = Helper.Reflection.GetField<string>(billboard, "hoverText").GetValue();
                // Redraw the hover text
                if (text.Length > 0)
                {
                    IClickableMenu.drawHoverText(e.SpriteBatch, text, Game1.dialogueFont);
                }

            }
        }


        /// <summary>
        ///  when closing the menu, reset the dictionary
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCalendarClosed(object? sender, MenuChangedEventArgs e) 
        {
            if (!this.Config.ToggleMod)
                return;
            if (Game1.activeClickableMenu == null && e.OldMenu is Billboard)
                this.CalendarDayDict = new();
            
        }


        /// <summary>
        /// Handle button Change event, 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e) 
        {
            if (Game1.activeClickableMenu is null) 
            {
                if (Context.IsPlayerFree && Game1.currentMinigame is null) 
                {
                    if (Config.ToggleBlackListKeybind.JustPressed())
                    {
                        // open blacklist menu
                        Helper.Input.SuppressActiveKeybinds(Config.ToggleBlackListKeybind);
                        Game1.activeClickableMenu = new CropBlackListMenu(Config);
                    }
                    else if (Config.BlacklistTheCropKeybind.JustPressed()) 
                    {
                        // blacklist a crop
                        Vector2 gamepadTile = Game1.player.CurrentTool != null
                                              ? Utility.snapToInt(Game1.player.GetToolLocation() / Game1.tileSize)
                                              : Utility.snapToInt(Game1.player.GetGrabTile());
                        Vector2 mouseTile = Game1.currentCursorTile;

                        Vector2 tile = Game1.options.gamepadControls && Game1.timerUntilMouseFade <= 0 ? gamepadTile : mouseTile;

                        if (Game1.currentLocation.terrainFeatures?.TryGetValue(tile, out TerrainFeature? terrain) ?? false) 
                        {
                            if (terrain is HoeDirt hoeDirt)
                            {
                                Crop crop = hoeDirt.crop;
                                if (crop != null)
                                {
                                    string cropId = crop.indexOfHarvest.Value;
                                    string message;
                                    if (Config.CropBlackList.Contains(cropId))
                                    {
                                        Config.RemoveFromBlackList(cropId);
                                        Game1.playSound("pickUpItem");
                                        message = I18n.UI_Remove_Notification(ItemRegistry.GetDataOrErrorItem(cropId).DisplayName);
                                    }
                                    else
                                    {
                                        Config.AddToBlackList(cropId);
                                        Game1.playSound("trashcan");
                                        message = I18n.UI_Add_Notification(ItemRegistry.GetDataOrErrorItem(cropId).DisplayName);
                                    }
                                    Game1.addHUDMessage(new HUDMessage(message) { noIcon = true });
                                }
                            }
                        }
                    }
                }
            }
            else if (IsCalendarPage()) 
            {
                if (Config.ToggleCalendarDayDetailKeybind.JustPressed())
                {
                    // open Calendar day detail menu
                    if (Game1.activeClickableMenu is Billboard billboard)
                    { 
                        List<ClickableTextureComponent> days = billboard.calendarDays;
                        int selectedDay = -1;

                        Rectangle b = new Rectangle(days[0].bounds.X, days[0].bounds.Y, days[0].bounds.Width * 7, days[0].bounds.Height * 4);
                        var x = Utility.ModifyCoordinateForUIScale(Game1.getMouseX());
                        var y = Utility.ModifyCoordinateForUIScale(Game1.getMouseY());
                        if (b.Contains(x, y))
                        {
                            x -= days[0].bounds.X;
                            y -= days[0].bounds.Y;
                            var width = days[0].bounds.Width;
                            var height = days[0].bounds.Height;
                            selectedDay = 7 * (int)Math.Floor(y / height) + (int)Math.Floor(x / width) + 1;
                        }

                        if (CalendarDayDict.ContainsKey(selectedDay))
                        {
                            Game1.activeClickableMenu = new CalendarDayDetailMenu(CalendarDayDict[selectedDay], billboard);
                        }
                    }

                }

                
            }
        }



        /// <summary>Return if Calendar is open or not.</summary>
        private bool IsCalendarPage()
        {
            return
                Game1.activeClickableMenu is Billboard;
        }



        /// copied and modified from gottyduke's stardew-informant mod
        /// https://github.com/gottyduke/stardew-informant/blob/main/Informant/Implementation/TooltipGenerator/CropTooltipGenerator.cs
        /// <summary>Return days left until harvest</summary>
        /// <param name="crop">The crop.</param>
        /// <returns>a tuple with two values, the first one is the days left until harvest, the second one is the days of regrow, -1 if not regrowable</returns>
        internal (int, int) CalculateDaysLeft(Crop crop)
        {
            var currentPhase = crop.currentPhase.Value;
            var dayOfCurrentPhase = crop.dayOfCurrentPhase.Value;
            var regrowAfterHarvest = crop.RegrowsAfterHarvest();
            var cropPhaseDays = crop.phaseDays.ToArray();
            // Amaranth:  current = 4 | day = 0 | days = 1, 2, 2, 2, 99999 | result => 0
            // Fairy Rose:  current = 4 | day = 1 | days = 1, 4, 4, 3, 99999 | result => 0
            // Cranberry:  current = 5 | day = 4 | days = 1, 2, 1, 1, 2, 99999 | result => ???
            // Ancient Fruit: current = 5 | day = 4 | days = 1 5 5 6 4 99999 | result => 4
            // Blueberry (harvested): current = 5 | day = 4 | days = 1 3 3 4 2 99999 | regrowAfterHarvest = 4 | result => 4
            // Blueberry (harvested): current = 5 | day = 0 | days = 1 3 3 4 2 99999 | regrowAfterHarvest = 4 | result => 0
            var result = 0;
            if (crop.Dirt.readyForHarvest()) 
            {
                return (result, crop.RegrowsAfterHarvest() ? crop.GetData().RegrowDays : -1);
            }
            for (var phase = currentPhase; phase < cropPhaseDays.Length; phase++)
            {
                if (cropPhaseDays[phase] < 99999)
                {
                    result += cropPhaseDays[phase];
                    if (phase == currentPhase)
                    {
                        result -= dayOfCurrentPhase;
                    }
                }
                else if (currentPhase == cropPhaseDays.Length - 1 && regrowAfterHarvest)
                {
                    // calculate the repeating harvests, it seems the dayOfCurrentPhase counts backwards now
                    result = dayOfCurrentPhase;
                }
            }

            return (result, crop.GetData().RegrowDays);
        }



        /// <summary>
        /// return all crops harvest data for a location
        /// </summary>
        /// <param name="location"></param>
        /// <returns>return a dictionary, key: (harvestDayOfMonth, locationName, cropId), value: numberOfCrops</returns>
        internal Dictionary<(int, string, string), int> GetAllCropsbyLocation(GameLocation location)
        {

            Dictionary<(int, string, string), int> result = new();
            int today = Game1.dayOfMonth;
            
            foreach (TerrainFeature value in location.terrainFeatures.Values)
            {
                if (value is HoeDirt hoeDirt)
                {
                    Crop crop = hoeDirt.crop;
                    if (crop == null || crop.dead.Value || crop.whichForageCrop.Value == Crop.forageCrop_gingerID)
                        continue;

                    var cropId = crop.indexOfHarvest.Value;
                    if (Config.CropBlackList.Contains(cropId))
                        continue;
                    (int, int) days = CalculateDaysLeft(crop);


                    string locationName;
                    if (location.NameOrUniqueName == "Greenhouse" || location.NameOrUniqueName == "IslandWest")
                    {
                        locationName = Helper.Translation.Get(location.NameOrUniqueName);
                    }
                    else 
                    {
                        locationName = location.DisplayName;
                    }

                    for (int i = today + days.Item1; i <= 28 && i >= today + days.Item1; i += days.Item2)
                    {
                        if (result.ContainsKey((i, locationName, cropId)))
                        {
                            result[(i, locationName, cropId)] += 1;
                        }
                        else
                        {
                            result.Add((i, locationName, cropId), 1);
                        }
                    }
                }
            }


            return result;
        }




    }
}