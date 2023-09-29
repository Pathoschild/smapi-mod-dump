/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using System.Collections.Generic;

namespace DeathTweaks
{
    public class ModConfig
    {
        public bool ModEnabled { get; set; } = true;
        public bool DropEverything { get; set; } = false;
        public bool DropNothing { get; set; } = false;
        public bool CreateTombstone { get; set; } = true;
        public float MoneyLostMult { get; set; } = 1;
        public string TombStonePath { get; set; } = "Maps/spring_town";
        public Rectangle TombStoneRect { get; set; } = new Rectangle(16, 160, 16, 16);
        public Vector2 TombStoneOffset { get; set; } = Vector2.Zero;
        public float TombStoneScale { get; set; } = 4;
    }
}
