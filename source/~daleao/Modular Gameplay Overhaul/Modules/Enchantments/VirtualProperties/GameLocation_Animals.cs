/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Enchantments.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;
using Netcode;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class GameLocation_Animals
{
    internal static ConditionalWeakTable<GameLocation, NetCollection<FarmAnimal>> Values { get; } = new();

    internal static NetCollection<FarmAnimal> Get_Animals(this GameLocation location)
    {
        return Values.GetOrCreateValue(location);
    }

    // Net types are readonly
    internal static void Set_Animals(this GameLocation location, NetCollection<FarmAnimal> value)
    {
    }
}
