/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/miome/MultitoolMod
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley.Tools;
using StardewValley;
using StardewValley.Menus;

namespace MultitoolMod.Framework
{
    public class PickaxeBlade : Pickaxe
    {
        public PickaxeBlade()
        {
         
        }
        public override void DoFunction(GameLocation location, int x, int y, int power, Farmer who)
        {
            int x_pixels = (int)Game1.currentCursorTile.X * Game1.tileSize;
            int y_pixels = (int)Game1.currentCursorTile.Y * Game1.tileSize;
            base.DoFunction(location, x_pixels, y_pixels, power, who);
        }
    }
}
