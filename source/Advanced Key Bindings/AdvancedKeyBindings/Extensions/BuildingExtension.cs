using System;
using System.Linq;
using AdvancedKeyBindings.StaticHelpers;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;

namespace AdvancedKeyBindings.Extensions
{
    public static class BuildingExtension
    {
        /// <summary>
        /// Pans the viewport to the building.
        /// </summary>
        /// <param name="building">The building</param>
        /// <param name="centerMouse">True to center the mouse at the building</param>
        /// <param name="playSound">True to play a selection sound</param>
        public static void PanToBuilding(this Building building, bool centerMouse = false, bool playSound = false)
        {
            var centerX = (building.tilesWide * 64) / 2;
            var centerY = (building.tilesHigh * 64) / 2;
            Action callbackAction = delegate
                {
                    if (centerMouse)
                    {
                        var cursorTarget = Game1.GlobalToLocal(new Vector2(
                            building.tileX * 64 + centerX,
                            building.tileY * 64 + centerY));

                        Game1.setMousePosition((int) cursorTarget.X, (int) cursorTarget.Y);
                    }
                    
                    if (playSound)
                    {
                        Game1.playSound("smallSelect");
                    }
                };
            

            SmoothPanningHelper.GetInstance().AbsolutePanTo(
                building.tileX * 64 - (Game1.viewport.Width / 2) + centerX,
                building.tileY * 64 - (Game1.viewport.Height / 2) + centerY,callbackAction);

            
        }

        public static bool CanDemolish(this Building building)
        {
            if (building.daysOfConstructionLeft > 0 ||
                building.daysUntilUpgrade > 0)
            {
                return false;
            }

            if (building.indoors.Value is AnimalHouse animalHouse)
            {
                if (animalHouse.animalsThatLiveHere.Count > 0)
                {
                    return false;
                }
            }

            if (building.indoors.Value != null && building.indoors.Value.farmers.Count > 0)
            {
                return false;
            }

            if (building.indoors.Value is Cabin)
            {
                return Game1.getAllFarmers().Cast<Character>().All(allFarmer => allFarmer.currentLocation.Name != (building.indoors.Value as Cabin)?.GetCellarName());
            }

            if (building.indoors.Value is Cabin cabin &&
                cabin.farmhand.Value.isActive())
            {
                return false;
            }

            return true;
        }
    }
}