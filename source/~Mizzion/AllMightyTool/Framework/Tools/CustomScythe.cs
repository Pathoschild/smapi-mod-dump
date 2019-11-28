using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Tools;

namespace AllMightyTool.Framework.Tools
{
    internal class CustomScythe : MeleeWeapon
    {

        public CustomScythe(int spriteIndex) : base(spriteIndex)
        {

        }

        public void DoDamage(GameLocation loc, int x, int y, int facingDirection, int power, Farmer who)
        {
            var grid = GetTileGrid(new Vector2(x, y), power);
            isOnSpecial = false;
            if (type.Value != 2)
                DoFunction(loc, x, y, power, who);

            Vector2 zero = Vector2.Zero;
            Vector2 zero1 = Vector2.Zero;
            string str = "";

            foreach (var g in grid)
            {
                if (loc.terrainFeatures.ContainsKey(g) && loc.terrainFeatures[g].performToolAction(this, 0, g, null))
                    loc.terrainFeatures.Remove(g);

                if (loc.objects.ContainsKey(g) && loc.objects[g].Name.Contains("Weed") &&
                    loc.objects[g].performToolAction(this, loc))
                    loc.objects.Remove(g);

                if (loc.performToolAction(this, (int)g.X, (int)g.Y))
                    break;
            }

            if (!str.Equals(""))
                Game1.playSound(str);
            CurrentParentTileIndex = IndexOfMenuItemView;
            if (who == null || who.isRidingHorse())
                return;
            who.completelyStopAnimatingOrDoingAction();
        }

        /// <summary>Get a grid of tiles.</summary>
        /// <param name="origin">The center of the grid.</param>
        /// <param name="distance">The number of tiles in each direction to include.</param>
        private IEnumerable<Vector2> GetTileGrid(Vector2 origin, int distance)
        {
            for (int x = -distance; x <= distance; x++)
            {
                for (int y = -distance; y <= distance; y++)
                    yield return new Vector2(origin.X + x, origin.Y + y);
            }
        }
    }
}