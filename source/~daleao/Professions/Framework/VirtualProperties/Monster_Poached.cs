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
using Netcode;
using StardewValley.Monsters;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Monster_Poached
{
    internal static ConditionalWeakTable<Monster, NetInt> Values { get; } = [];

    internal static NetInt Get_Poached(this Monster monster)
    {
        return Values.GetOrCreateValue(monster);
    }

    // Net types are readonly
    internal static void Set_Poached(this Monster monster, NetBool value)
    {
    }

    internal static void IncrementPoached(this Monster monster, int amount = 1)
    {
        Values.GetOrCreateValue(monster).Value += amount;
    }
}
