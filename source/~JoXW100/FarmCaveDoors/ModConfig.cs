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

namespace FarmCaveDoors
{
    public class ModConfig
    {
        public bool ModEnabled { get; set; } = true;
        public int MineDoorX { get; set; } = 2;
        public int MineDoorY { get; set; } = 4;
        public int SkullDoorX { get; set; } = 10;
        public int SkullDoorY { get; set; } = 4;
    }
}
