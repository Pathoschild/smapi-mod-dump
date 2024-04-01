/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

#nullable enable
using System;
using System.Globalization;
using System.Linq;
using FerngillHelper.Framework;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using StardewValley.TerrainFeatures;
using SObject = StardewValley.Object;

namespace FerngillHelper
{
    public class ModEntry : Mod
    {
        //Variables
        private FerngillHelperConfig? _config;

        /// <summary>
        /// The Entry Method
        /// </summary>
        /// <param name="helper"></param>
        public override void Entry(IModHelper helper)
        {
            //Load up the config file.
            _config = helper.ReadConfig<FerngillHelperConfig>();

            //Helper events.
            helper.Events.GameLoop.SaveLoaded += SaveLoaded;
            helper.Events.Display.RenderedHud += RenderedHud;
            helper.Events.Input.ButtonPressed += ButtonsReleased;
            //helper.Events.Input.CursorMoved += CursorMoved;

        }

        private void ButtonsReleased(object? sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsPlayerFree || _config is null)
                return;

            //Process the buttons
            if (e.IsDown(_config.TeleportKey) && _config.AllowTeleport)
            {
                Game1.player.Position = new Vector2(Game1.currentCursorTile.X * 64f, Game1.currentCursorTile.Y * 64f);
                //Game1.player.Position.Y = Game1.currentCursorTile.Y;
                
            }
            else if (e.IsDown(SButton.F5))
            {
                _config = Helper.ReadConfig<FerngillHelperConfig>();
                Monitor.Log("Config file was reloaded.");
            }
        }
        /// <summary>
        /// Event triggered when a player loads a save.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            //Populate Configs
        }

        private void RenderedHud(object? sender, RenderedHudEventArgs e)
        {
            Game1.player.currentLocation.terrainFeatures.TryGetValue(Game1.currentCursorTile, out var ter);
            Game1.player.currentLocation.objects.TryGetValue(Game1.currentCursorTile, out var obj);
            if ( _config is null || !_config.UseHoverMode)
                return;

            switch (ter)
            {
                case HoeDirt when _config.ShowCropInfo:
                    DrawInfoBox(CropInfo(ter, "crops"));
                    break;
                case Tree tree when _config.ShowTreeInfo:
                    if(tree.growthStage.Value >= 5 && !tree.stump.Value)
                        DrawInfoBox(CropInfo(ter, "tree"));
                    break;
            }

            //Locations objects
            
            if (obj is null)
                return;
            if (_config.ShowMachineInfo)
            {
                if(obj.MinutesUntilReady > 0 && obj.bigCraftable.Value)
                    DrawInfoBox(MachineInfo(obj));
            }
                
        }

        //Custom Methods
        private static void DrawInfoBox(string text)
        {
            //Make sure the game available
            if (text == "" || !Context.IsPlayerFree || Game1.player.IsCarrying())
            {
                return;
            }

            var x = Game1.getOldMouseX() + 32;
            var y = Game1.getOldMouseY() + 32;
            var font = Game1.smallFont.MeasureString(text);

            var width = Convert.ToInt32(font.X + 32);
            var height = Convert.ToInt32(Math.Max((text.Length + 80) , (int)font.Y));
            
            //Check to make sure it's not too large.
            if (x + width > Game1.viewport.Width)
            {
                x = Game1.viewport.Width - width;
                y += 32;
            }

            if (y + height > Game1.viewport.Height)
            {
                x += 32;
                y = Game1.viewport.Height - height;
            }

            //Now we draw
            IClickableMenu.drawTextureBox(Game1.spriteBatch, 
                Game1.menuTexture, 
                new Rectangle(0, 256, 60, 60),
                x,
                y,
                width,
                height,
                Color.White);

            Utility.drawTextWithShadow(Game1.spriteBatch,
                text,
                Game1.smallFont,
                new Vector2(x + 64 / 4, y + 64 / 4),
                Game1.textColor);

        }

        private string CropInfo(TerrainFeature id, string whichTer)
        {
            var output = "";
            switch (whichTer)
            {
                case "crops":
                {
                    if (id is HoeDirt { crop: { } } dirt)
                    {
                        var o = new SObject(dirt.crop.indexOfHarvest.ToString(), 1);
                        var canBeHarvested = (dirt.crop.currentPhase.Value >= (dirt.crop.phaseDays.Count - 1)) && (!dirt.crop.fullyGrown.Value || dirt.crop.dayOfCurrentPhase.Value <= 0);
                        var daysToGo = "";
                        var testDate = getHarvestDate(dirt.crop);
                        var days = getDays(testDate, dirt.crop);

                        //Day or Days
                        var day = days > 1 ? "Days" : "Day";

                        //Lets capitalize the first letter of the season
                        var textInfo = CultureInfo.CurrentCulture.TextInfo;
                        var capSeason = textInfo.ToTitleCase(testDate.Season);

                        daysToGo = canBeHarvested ? "Harvest in: Harvest Ready" : $"Harvest On: {capSeason} {testDate.Day}({days} {day} from now)";
                        daysToGo = testDate.Season != SDate.Now().Season ? "Harvest Won't be ready in time." : daysToGo;
                        output += $"{o.DisplayName}\r\n{daysToGo}"; 
                    }

                    break;
                }
                case "tree":
                {
                    if (id is Tree tree)
                    {
                        var hasSeed = tree.hasSeed.Value ? "Yes" : "No";
                        var beenTapped = tree.tapped.Value ? "Yes" : "No";
                        output += $"{Trees.GetTreeName(tree.treeType.Value)} Tree\r\nHas Seed: {hasSeed}\r\nTapped: {beenTapped}";
                    }

                    break;
                }
            }

            return output;
        }

        /// <summary>
        /// Returns information related to the machine that is hovered.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private string MachineInfo(SObject obj)
        {
            var t = GetTime(TimeSpan.FromMinutes(obj.MinutesUntilReady).ToString());
            return $"{obj.DisplayName}\r\nReady In: {t}";
        }

        /// <summary>
        /// Grab a stardew date of when a crop will be ready to be harvested.
        /// </summary>
        /// <param name="crop">The crop to get the date for.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private SDate getHarvestDate(Crop? crop)
        {
            if (crop == null)
                throw new InvalidOperationException("Crop was null");

            var harvestablePhase = crop.phaseDays.Count - 1;
            var canBeHarvested = (crop.currentPhase.Value >= harvestablePhase) && (!crop.fullyGrown.Value || crop.dayOfCurrentPhase.Value <= 0);
            var firstHarvest = crop.phaseDays.Take(crop.phaseDays.Count - 1).Sum();

            if (canBeHarvested)
                return SDate.Now();
            if (!crop.fullyGrown.Value)
            {
                var i = firstHarvest - crop.dayOfCurrentPhase.Value -
                        crop.phaseDays.Take(crop.currentPhase.Value).Sum();
                return SDate.Now().AddDays(i);
            }

            return SDate.Now().AddDays(crop.dayOfCurrentPhase.Value >= crop.regrowAfterHarvest.Value ? crop.regrowAfterHarvest.Value : crop.dayOfCurrentPhase.Value);
        }

        private int getDays(SDate date, Crop? crop)
        {
            var days = 0;
            var harvestDate = date;

            
            var currentMonthIndex = SDate.Now().SeasonIndex;
            var currentMonth = SDate.Now().Season;
            var currentDay = SDate.Now().Day;

            days = harvestDate.Day - currentDay;
            return (harvestDate.Day - currentDay) < 0 ? 0 : days;
            //Monitor.Log($"Current Season: {SDate.Now().Season}, Current Day: {SDate.Now().Day}, Season Index: {SDate.Now().SeasonIndex}");
        }

        /// <summary>
        /// Takes a TimeSpan and makes it easier to read in game.
        /// </summary>
        /// <param name="time">The TimeSpan</param>
        /// <returns></returns>
        private static string GetTime(string time)
        {
            var timeSplit = time.Split('.', ':');
            var days = Convert.ToInt32(timeSplit[0]) >= 1 ? $"{timeSplit[0]} day{CheckPlural(Convert.ToInt32(timeSplit[0]))}" : "";
            var hours = Convert.ToInt32(timeSplit[1]) >= 1 ? $"{CheckForComma(Convert.ToInt32(timeSplit[0]))}{timeSplit[1]} hour{CheckPlural(Convert.ToInt32(timeSplit[1]))}" : "";
            var minutes = Convert.ToInt32(timeSplit[2]) >= 1 ? $"{CheckForComma(Convert.ToInt32(timeSplit[1]))}{timeSplit[2]} minute{CheckPlural(Convert.ToInt32(timeSplit[2]))}" : "";
            return $"{days}{hours}{minutes}";
            
        }

        /// <summary>
        /// Checks to see if a word should be plural based on an int.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static string CheckPlural(int input)
        {
            return input > 1 ? "s" : "";
        }

        /// <summary>
        /// Checks to see if we should include a comma.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static string CheckForComma(int input)
        {
            return input >= 1 ? ", " : "";
        }
    }
}
