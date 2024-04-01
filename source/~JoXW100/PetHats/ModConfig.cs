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

namespace PetHats
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public bool Debug { get; set; } = false;
        public List<Point> CatOffsets { get; set; } = new()
        {
            new Point(0,0),
            new Point(0,0),
            new Point(0,0)
        };
        public List<Point> DogOffsets { get; set; } = new()
        {
            new Point(0,0),
            new Point(0,0),
            new Point(0,8)
        };
        public SButton RetrieveButton { get; set; } = SButton.None;

    }
}
