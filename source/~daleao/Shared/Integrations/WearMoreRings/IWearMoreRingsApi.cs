/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

#pragma warning disable CS1591
namespace DaLion.Shared.Integrations.WearMoreRings;

#region using directives

using System.Collections.Generic;
using StardewValley;
using StardewValley.Objects;

#endregion using directives

/// <summary>The API provided by Wear More Rings.</summary>
public interface IWearMoreRingsApi
{
    int CountEquippedRings(Farmer f, int which);

    IEnumerable<Ring> GetAllRings(Farmer f);
}
