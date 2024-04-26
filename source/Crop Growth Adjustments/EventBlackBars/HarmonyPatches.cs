/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/gzhynko/stardew-mods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

//ReSharper disable InconsistentNaming

namespace EventBlackBars
{
    public class HarmonyPatches
    {
        /// <summary> Patch for the Event.exitEvent method. </summary>
        public static void EventEnd(Event __instance)
        {
            if (__instance.isFestival || __instance.isWedding) return;
            
            ModEntry.Instance.StartMovingBars(Direction.MoveOut);
        }

        public static void DrawAfterMap(SpriteBatch b)
        {
            if (!ModEntry.RenderBars) return;
            
            var viewportWidth = ModEntry.GraphicsDevice.Viewport.Width;
            var viewportHeight = ModEntry.GraphicsDevice.Viewport.Height;
            
            // Top bar
            b.Draw(ModEntry.BlackRectangle, new Vector2(0, 0), null,
                Color.White, 0f, Vector2.Zero, new Vector2(viewportWidth, ModEntry.BarHeight),
                SpriteEffects.None, 0.0f);
            
            // Bottom bar
            b.Draw(ModEntry.BlackRectangle, new Vector2(0, viewportHeight - ModEntry.BarHeight), null,
                Color.White, 0f, Vector2.Zero, new Vector2(viewportWidth, ModEntry.BarHeight),
                SpriteEffects.None, 0.0f);
        }
        
        /// <summary> Patch for the GameLocation.startEvent method. </summary>
        public static void EventStart(Event evt)
        {
            if (evt.isFestival || evt.isWedding) return;
            
            ModEntry.Instance.StartMovingBars(Direction.MoveIn);
        }
    }
}