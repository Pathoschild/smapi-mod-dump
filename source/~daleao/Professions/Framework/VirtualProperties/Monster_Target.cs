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
internal static class Monster_Target
{
    internal static ConditionalWeakTable<Monster, Farmer?> Values { get; } = [];

    internal static Farmer Get_Target(this Monster monster)
    {
        return Values.GetOrCreateValue(monster) ?? Game1.player;
    }

    internal static void Set_Target(this Monster monster, Farmer? target)
    {
        Values.AddOrUpdate(monster, target);
    }
}
