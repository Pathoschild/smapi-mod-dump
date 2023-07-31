/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SObject = StardewValley.Object;

namespace AchtuurCore.Extensions;
public static class GameLocationExtensions
{
    public static SObject getObjectAtTile(this GameLocation gameLocation, Vector2 tile)
    {
        return gameLocation.getObjectAtTile((int)tile.X, (int)tile.Y);
    }

    public static bool isObjectAtTile(this GameLocation gameLocation, Vector2 tile)
    {
        return gameLocation.isObjectAtTile((int)tile.X, (int)tile.Y);
    }
}
