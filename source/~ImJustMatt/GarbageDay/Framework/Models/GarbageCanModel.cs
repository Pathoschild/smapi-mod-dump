/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;

namespace ImJustMatt.GarbageDay.Framework.Models
{
    internal class GarbageCanModel
    {
        internal string MapName { get; set; }
        internal GameLocation Location { get; set; }
        internal Vector2 Tile { get; set; }
    }
}