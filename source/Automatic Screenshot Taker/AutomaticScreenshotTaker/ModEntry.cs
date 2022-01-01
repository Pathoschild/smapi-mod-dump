/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Zamiell/AutomaticScreenshotTaker
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Text.RegularExpressions;

namespace AutomaticScreenshotTaker
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            helper.Events.Player.Warped += this.OnWarped;
        }

        private void OnWarped(object sender, WarpedEventArgs e)
        {
            if (!Context.IsWorldReady)
            {
                return;
            }

            var areaName = e.NewLocation.Name;
            var msg = $"Loaded area: {areaName}";
            this.Monitor.Log(msg, LogLevel.Debug);

            var emptyElevatorFloor = false;
            if (areaName.StartsWith("UndergroundMine")) {
                Regex regex = new Regex(@"\d+");
                Match match = regex.Match(areaName);
                if (match.Success) {
                    var floorNumString = match.Value;
                    int floorNum = Int32.Parse(floorNumString);
                    if (floorNum % 10 == 0)
                    {
                        emptyElevatorFloor = true;
                    }
                }
            }

            // The game lags every time we take a screenshot,
            // so we only do it when needed
            if (areaName.StartsWith("Underground") && !emptyElevatorFloor)
            {
                PauseGame();
                TakeScreenshot();
                return;
            }
        }

        private void PauseGame()
        {
            Game1.activeClickableMenu = new StardewValley.Menus.GameMenu();
        }

        private void TakeScreenshot()
        {
            float zoomLevel = 1f;
            string fileName = "current_area";

            string mapScreenshotPath = Game1.game1.takeMapScreenshot(zoomLevel, fileName, null);
        }
    }
}
