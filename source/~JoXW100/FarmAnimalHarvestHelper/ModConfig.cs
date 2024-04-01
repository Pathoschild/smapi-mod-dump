/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JoXW100/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using System.Collections.Generic;

namespace FarmAnimalHarvestHelper
{
    public class ModConfig
    {
        public bool ModEnabled { get; set; } = true;
        public int MaxWaitHour { get; set; } = 1200;
        public Vector2 FirstSlotTile { get; set; } = new Vector2(8, 4);
    }
}
