/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/barteke22/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;

namespace DiagonalAim
{
    class HarmonyPatches
    {

        public static void GetToolLocation_Diagonals(ref Vector2 __result, Character __instance, Vector2 target_position, bool ignoreClick = false)
        {
            try
            {
                int direction = __instance.FacingDirection;

                if ((Game1.player.CurrentTool == null || (!Game1.player.CurrentTool.CanUseOnStandingTile()) && !ModEntry.config.AllowAnyToolOnStandingTile) && (int)(target_position.X / 64f) == Game1.player.getTileX() && (int)(target_position.Y / 64f) == Game1.player.getTileY())
                {
                    Rectangle bb = __instance.GetBoundingBox();
                    switch (__instance.FacingDirection)
                    {
                        case 0:
                            __result = new Vector2(bb.X + bb.Width / 2, bb.Y - 64);
                            return;
                        case 1:
                            __result = new Vector2(bb.X + bb.Width + 64, bb.Y + bb.Height / 2);
                            return;
                        case 2:
                            __result = new Vector2(bb.X + bb.Width / 2, bb.Y + bb.Height + 64);
                            return;
                        case 3:
                            __result = new Vector2(bb.X - 64, bb.Y + bb.Height / 2);
                            return;
                    }
                }
                if (!ignoreClick && !target_position.Equals(Vector2.Zero) && __instance.Name.Equals(Game1.player.Name))
                {
                    bool allow_clicking_on_same_tile = false;
                    if (ModEntry.config.AllowAnyToolOnStandingTile || (Game1.player.CurrentTool != null && Game1.player.CurrentTool.CanUseOnStandingTile()))
                    {
                        allow_clicking_on_same_tile = true;
                    }
                    if (Utility.withinRadiusOfPlayer((int)target_position.X, (int)target_position.Y, 1, Game1.player))
                    {
                        direction = Game1.player.getGeneralDirectionTowards(new Vector2((int)target_position.X, (int)target_position.Y));
                        if (allow_clicking_on_same_tile || Math.Abs(target_position.X - (float)Game1.player.getStandingX()) >= 32f || Math.Abs(target_position.Y - (float)Game1.player.getStandingY()) >= 32f)
                        {
                            __result = target_position;
                            return;
                        }
                    }
                    else if (Game1.player.CurrentTool == null || !Game1.player.CurrentTool.Name.Equals("Fishing Rod", StringComparison.Ordinal))
                    {
                        int extraReach = ModEntry.config.ExtraReachRadius;

                        List<Vector2> tiles = new List<Vector2>();
                        int endX = __instance.getTileX() + extraReach + 2;
                        int endY = __instance.getTileY() + extraReach + 2;
                        for (int x = __instance.getTileX() - extraReach - 1; x < endX; x++)
                        {
                            for (int y = __instance.getTileY() - extraReach - 1; y < endY; y++)
                            {
                                tiles.Add(new Vector2(x, y));
                            }
                        }
                        foreach (var tile in tiles)
                        {
                            if (Vector2.DistanceSquared(tile * 64, Game1.currentCursorTile * 64) < Vector2.DistanceSquared(__result, Game1.currentCursorTile * 64)) __result = tile * 64;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Error in harmony patch: " + e.Message);
            }
        }
    }
}
