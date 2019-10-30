using System.Linq;
using JoysOfEfficiency.Utils;
using Microsoft.Xna.Framework;
using StardewValley;

namespace JoysOfEfficiency.EventHandler
{
    internal class FenceGateAutomation
    {
        public static void TryToggleGate(Farmer player)
        {
            foreach (Fence fence in Util.GetObjectsWithin<Fence>(2).Where(f => f.isGate.Value))
            {
                Vector2 loc = fence.TileLocation;

                bool? isUpDown = IsUpsideDown(fence);
                if (isUpDown == null)
                {
                    if (!fence.getBoundingBox(loc).Intersects(player.GetBoundingBox()))
                    {
                        fence.gatePosition.Value = 0;
                    }
                    continue;
                }

                int gatePosition = fence.gatePosition.Value;
                bool flag = IsPlayerInClose(player, fence, fence.TileLocation, isUpDown);

                if (flag && gatePosition == 0)
                {
                    fence.gatePosition.Value = 88;
                    Game1.playSound("doorClose");
                }
                else if (!flag && gatePosition >= 88)
                {
                    fence.gatePosition.Value = 0;
                    Game1.playSound("doorClose");
                }
            }
        }

        /// <summary>
        /// Returns type of the gate
        /// </summary>
        /// <param name="fence">The fence</param>
        /// <returns>true for horizontal, false for vertical, null for invalid</returns>
        private static bool? IsUpsideDown(Fence fence)
        {
            int num2 = 0;
            GameLocation currentLocation = Game1.currentLocation;
            Vector2 tileLocation = fence.TileLocation;
            int whichType = fence.whichType.Value;
            tileLocation.X += 1f;
            if (currentLocation.objects.ContainsKey(tileLocation) && currentLocation.objects[tileLocation].GetType() == typeof(Fence) && ((Fence)currentLocation.objects[tileLocation]).countsForDrawing(whichType))
            {
                num2 += 100;
            }
            tileLocation.X -= 2f;
            if (currentLocation.objects.ContainsKey(tileLocation) && currentLocation.objects[tileLocation].GetType() == typeof(Fence) && ((Fence)currentLocation.objects[tileLocation]).countsForDrawing(whichType))
            {
                num2 += 10;
            }
            tileLocation.X += 1f;
            tileLocation.Y += 1f;
            if (currentLocation.objects.ContainsKey(tileLocation) && currentLocation.objects[tileLocation].GetType() == typeof(Fence) && ((Fence)currentLocation.objects[tileLocation]).countsForDrawing(whichType))
            {
                num2 += 500;
            }
            tileLocation.Y -= 2f;
            if (currentLocation.objects.ContainsKey(tileLocation) && currentLocation.objects[tileLocation].GetType() == typeof(Fence) && ((Fence)currentLocation.objects[tileLocation]).countsForDrawing(whichType))
            {
                num2 += 1000;
            }

            if (fence.isGate.Value)
            {
                switch (num2)
                {
                    case 110:
                        return true;
                    case 1500:
                        return false;
                    default:
                        return null;
                }
            }
            return null;
        }

        private static bool IsPlayerInClose(Farmer player, Fence fence, Vector2 fenceLocation, bool? isUpDown)
        {
            if (isUpDown == null)
            {
                return fence.getBoundingBox(fence.TileLocation).Intersects(player.GetBoundingBox());
            }
            Vector2 playerTileLocation = player.getTileLocation();
            if (playerTileLocation == fenceLocation)
            {
                return true;
            }
            if (!IsPlayerFaceOrBackToFence(isUpDown == true, player))
            {
                return false;
            }
            return isUpDown.Value ? ExpandSpecific(fence.getBoundingBox(fenceLocation), 0, 16).Intersects(player.GetBoundingBox()) : ExpandSpecific(fence.getBoundingBox(fenceLocation), 16, 0).Intersects(player.GetBoundingBox());
        }

        private static bool IsPlayerFaceOrBackToFence(bool isUpDown, Farmer player)
        {
            return isUpDown ? player.FacingDirection % 2 == 0 : player.FacingDirection % 2 == 1;
        }

        private static Rectangle ExpandSpecific(Rectangle rect, int deltaX, int deltaY)
        {
            return new Rectangle(rect.X - deltaX, rect.Y - deltaY, rect.Width + deltaX * 2, rect.Height + deltaY * 2);
        }
    }
}
