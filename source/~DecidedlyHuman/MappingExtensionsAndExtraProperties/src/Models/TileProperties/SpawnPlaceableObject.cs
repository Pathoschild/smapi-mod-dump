/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using System;
using Microsoft.Win32.SafeHandles;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Objects;

namespace MappingExtensionsAndExtraProperties.Models.TileProperties;

public struct SpawnPlaceableObject : ITilePropertyData
{
    public static string PropertyKey => "MEEP_SpawnPlaceableObject";
    public bool Breakable = false;
    public Item? bigCraftable;
    private int objectId;

    public SpawnPlaceableObject(int objectId, Item bigCraftable, bool breakable = true)
    {
        this.objectId = objectId;
        this.Breakable = breakable;
        this.bigCraftable = bigCraftable;
    }
}
