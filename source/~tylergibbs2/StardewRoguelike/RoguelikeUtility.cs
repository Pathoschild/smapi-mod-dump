/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;

namespace StardewRoguelike
{
    internal class RoguelikeUtility
    {
        public static void AddItemsByMenu(List<Item> items, ItemGrabMenu.behaviorOnItemSelect? itemSelectedCallback = null)
        {
            Game1.activeClickableMenu = new ItemGrabMenu(items).setEssential(essential: true);
            ((ItemGrabMenu)Game1.activeClickableMenu).inventory.showGrayedOutSlots = true;
            ((ItemGrabMenu)Game1.activeClickableMenu).inventory.onAddItem = itemSelectedCallback;
            ((ItemGrabMenu)Game1.activeClickableMenu).source = 2;
        }

        internal class SpeechBubble
        {
            public float Alpha { get; set; } = 0f;

            public string Text { get; set; }
            public int Duration { get; set; }

            public Vector2 DrawPosition { get; set; }

            public SpeechBubble(Vector2 drawPosition, string text, int duration)
            {
                Text = text;
                Duration = duration;
                DrawPosition = drawPosition;
            }
        }

        public static Vector2 VectorFromDegrees(int degrees)
        {
            double radians = DegreesToRadians(degrees);
            return new((float)Math.Cos(radians), (float)Math.Sin(radians));
        }

        public static float VectorToRadians(Vector2 vector)
        {
            return (float)Math.Atan2(vector.Y, vector.X);
        }

        public static int VectorToDegrees(Vector2 vector)
        {
            return (int)(VectorToRadians(vector) * (180f / Math.PI));
        }

        public static float DegreesToRadians(float degrees)
        {
            return (float)(degrees * (Math.PI / 180));
        }
    }
}
