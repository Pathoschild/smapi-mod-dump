/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

#pragma warning disable CS1591
namespace DaLion.Shared.Integrations.CustomResourceClumps;

#region using directives

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley.TerrainFeatures;

#endregion using directives

/// <summary>The API provided by Custom Ore Nodes.</summary>
public interface ICustomResourceClumpsApi
{
    ResourceClump GetCustomClump(string id, Vector2 tile);

    bool TryPlaceClump(GameLocation location, string id, Vector2 tile);

    List<object> GetCustomClumpData();

    List<string> GetCustomClumpIDs();
}
