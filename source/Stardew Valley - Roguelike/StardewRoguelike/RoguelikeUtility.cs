/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyRoguelike
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;

namespace StardewRoguelike
{
    internal class RoguelikeUtility
    {
        public static void AddItemsByMenu(List<Item> items, ItemGrabMenu.behaviorOnItemSelect itemSelectedCallback = null)
        {
            Game1.activeClickableMenu = new ItemGrabMenu(items).setEssential(essential: true);
            (Game1.activeClickableMenu as ItemGrabMenu).inventory.showGrayedOutSlots = true;
            (Game1.activeClickableMenu as ItemGrabMenu).inventory.onAddItem = itemSelectedCallback;
            (Game1.activeClickableMenu as ItemGrabMenu).source = 2;
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
    }
}
