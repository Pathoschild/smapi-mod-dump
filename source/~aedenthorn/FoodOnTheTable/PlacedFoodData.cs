/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;

namespace FoodOnTheTable
{
    internal class PlacedFoodData
    {
        public Furniture furniture;
        public Vector2 foodTile;
        public Object foodObject;
        public int slot;
        public int value;

        public PlacedFoodData(Furniture furniture, Vector2 foodTile, Object foodObject, int slot)
        {
            this.furniture = furniture;
            this.foodTile = foodTile;
            this.foodObject = foodObject;
            this.slot = slot;
        }
    }
}