/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Professions.Framework.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;
using StardewValley.Monsters;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Monster_Musked
{
    internal static ConditionalWeakTable<Monster, Musk> Values { get; } = [];

    internal static Musk? Get_Musk(this Monster monster)
    {
        return Values.TryGetValue(monster, out var musk) ? musk : null;
    }

    internal static bool Get_Musked(this Monster monster)
    {
        return Values.TryGetValue(monster, out _);
    }

    internal static void Set_Musked(this Monster monster, int duration)
    {
        var musk = new Musk(monster, duration);
        Values.AddOrUpdate(monster, musk);
        monster.currentLocation.AddMusk(musk);
    }
}
