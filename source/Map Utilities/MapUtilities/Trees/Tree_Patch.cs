using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.TerrainFeatures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MapUtilities.Trees
{
    class Tree_draw_Patch
    {
        public static bool Prefix(SpriteBatch spriteBatch, Vector2 tileLocation, Tree __instance)
        {
            if(TreeHandler.currentTrees != null && TreeHandler.currentTrees.ContainsKey(__instance))
            {
                TreeHandler.currentTrees[__instance].draw(spriteBatch, tileLocation);
                return false;
            }
            return true;
        }
    }

    class Tree_performToolAction_patch
    {
        public static void Postfix(Tool t, int explosion, Vector2 tileLocation, GameLocation location, Tree __instance)
        {
            if(__instance.health.Value > 0)
            {
                return;
            }
            if (!TreeHandler.currentTrees.ContainsKey(__instance))
            {
                Logger.log("Tree not found!");
                return;
            }
            TreeRenderer renderer = TreeHandler.currentTrees[__instance];
            int treeHeight = renderer.treeStructure.getExtent(new Type[] { typeof(Stump), typeof(Trunk) });
            bool falling = Reflector.reflector.GetField<bool>(__instance, "falling").GetValue();
            if (falling)
            {
                if (t != null && t.getLastFarmerToUse().IsLocalPlayer)
                {
                    if (t != null)
                        t.getLastFarmerToUse().gainExperience(2, (1 * treeHeight));
                }
                Game1.createDebris(12, (int)tileLocation.X, (int)tileLocation.Y, 4, (GameLocation)null);
                Game1.createRadialDebris(location, 12, (int)tileLocation.X, (int)tileLocation.Y, Game1.random.Next(20, 30), false, -1, false, -1);
            }
        }
    }
}
