/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Bouhm/stardew-valley-mods
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley;

namespace LocationCompass.Framework
{
    // Object for keeping track of with NPC to show when
    // there are multiple NPCs in a building
    internal class LocatorScroller
    {
        public string Location { get; set; }
        public HashSet<string> Characters { get; set; }
        public int Index { get; set; }
        public Rectangle LocatorRect { get; set; }

        public void ReceiveLeftClick()
        {
            if (this.LocatorRect.Contains(Game1.getMouseX(), Game1.getMouseY()))
            {
                this.Index++;
                Game1.playSound("drumkit6");

                if (this.Index > this.Characters.Count - 1)
                    this.Index = 0;
            }
        }
    }
}
