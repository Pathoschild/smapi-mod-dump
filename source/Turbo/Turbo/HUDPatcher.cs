/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/PrimmR/Turbo
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Extensions;

namespace Turbo
{
    /// <summary>Handles patching methods relating to Stardew's HUD</summary>
    internal class HUDPatcher
    {
        private static IMonitor Mntr;

        internal static void Initialise(IMonitor monitor)
        {
            Mntr = monitor;
        }

        /// <summary>Prefix patch for Stardew's HUDMessage.draw method</summary>
        internal static void HUDDraw_Final(HUDMessage __instance, SpriteBatch b, int i, ref int heightUsed)
        {
            try
            {
                if (__instance.whatType == 2082)
                {
                    Rectangle tsarea = Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea();
                    Vector2 itemBoxPosition = new Vector2(tsarea.Left + 16, tsarea.Bottom - heightUsed - 64);
                    if (Game1.isOutdoorMapSmallerThanViewport())
                    {
                        itemBoxPosition.X = Math.Max(tsarea.Left + 16, -Game1.uiViewport.X + 16);
                    }
                    if (Game1.uiViewport.Width < 1400)
                    {
                        itemBoxPosition.Y -= 48f;
                    }
                    itemBoxPosition.X += 16f;
                    itemBoxPosition.Y += 16f;

                    switch (ModEntry.change)
                    {
                        case 0:
                            b.Draw(Game1.mouseCursors, itemBoxPosition + new Vector2(6.5f, 8f) * 4f, new Rectangle(410, 495, 9, 15), Color.White * __instance.transparency, 0f, new Vector2(3f, 7f), 4f + Math.Max(0f, (__instance.timeLeft / (float)ModEntry.speed - 2000f) / 900f), SpriteEffects.None, 1f);
                            break;
                        case 1:
                            b.Draw(Game1.mouseCursors, itemBoxPosition + new Vector2(6.5f, 8f) * 4f, new Rectangle(410, 495, 9, 15), Color.White * __instance.transparency, 0f, new Vector2(3f, 7f), 4f + Math.Max(0f, (__instance.timeLeft / (float)ModEntry.speed - 2000f) / 900f), SpriteEffects.FlipVertically, 1f);
                            break;
                        case 2:
                            b.Draw(Game1.mouseCursors, itemBoxPosition + new Vector2(6.5f, 10.5f) * 4f, new Rectangle(434, 475, 9, 9), Color.White * __instance.transparency, 0f, new Vector2(3f, 7f), 4f + Math.Max(0f, (__instance.timeLeft / (float)ModEntry.speed - 2000f) / 900f), SpriteEffects.None, 1f);
                            break;
                        default:
                            Mntr.Log($"Could not draw custom icon", LogLevel.Error);
                            break;

                    }

                    Mntr.LogOnce($"Drew custom icon", LogLevel.Trace);
                }
            }
            catch (Exception ex)
            {
                Mntr.Log($"Failed in {nameof(HUDDraw_Final)}:\n{ex}", LogLevel.Error);
            }
        }
    }
}
