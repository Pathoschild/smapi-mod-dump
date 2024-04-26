/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Zamiell/stardew-valley-mods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using System.Text.RegularExpressions;

namespace AutomaticScreenshotTaker
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            helper.Events.Player.Warped += this.OnWarped;
        }

        private void OnWarped(object? sender, WarpedEventArgs e)
        {
            // The game lags every time we take a screenshot, so we only do it when needed.
            if (IsDungeonBattleFloor(e.NewLocation))
            {
                // We have to pause before taking the screenshot because otherwise if other mods pause the game after the screenshot is taken,
                // then it will appear in a bugged location.
                EmulatePause();

                TakeScreenshot();
            }
        }

        public bool IsDungeonBattleFloor(GameLocation location)
        {
            return (
                // e.g. "UndergroundMine1" is the first floor of the mines.
                (location is MineShaft mineShaft && mineShaft.Name.StartsWith("UndergroundMine") && !IsMineEmptyFloor(mineShaft))
                // "VolcanoDungeon0" is the first floor of the Volcano Dungeon.
                || (location.Name.StartsWith("VolcanoDungeon") && location.Name != "VolcanoDungeon0")
            );
        }

        private bool IsMineEmptyFloor(MineShaft mineShaft)
        {
            var potentialTuple = GetDungeonFloorNum(mineShaft.Name);
            if (potentialTuple is not (int, bool) tuple)
            {
                return false;
            }

            var (floorNum, mines) = tuple;
            return mines ? floorNum % 10 == 0 : false;
        }

        public (int floorNum, bool mines)? GetDungeonFloorNum(string locationName)
        {
            string pattern = @"\d+";
            Match match = Regex.Match(locationName, pattern);

            if (!match.Success)
            {
                return null;
            }

            string numericPart = match.Value;

            if (!int.TryParse(numericPart, out int floorNum))
            {
                return null;
            }

            // In Skull Caverns, floor 121 is floor 1.
            bool mines = true;
            if (floorNum > 120)
            {
                floorNum -= 120;
                mines = false;
            }

            return (floorNum, mines);
        }

        private void TakeScreenshot()
        {
            Game1.game1.takeMapScreenshot(1f, "current_area", null);
        }

        private void EmulatePause()
        {
            if (Game1.activeClickableMenu is null)
            {
                Game1.activeClickableMenu = new GameMenu();
            }
        }

        private void Log(string msg)
        {
            this.Monitor.Log(msg, LogLevel.Debug);
        }
    }
}
