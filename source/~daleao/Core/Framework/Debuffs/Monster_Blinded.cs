/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Core.Framework.Debuffs;

#region using directives

using System.Runtime.CompilerServices;
using Netcode;
using StardewValley.Monsters;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Monster_Blinded
{
    internal static ConditionalWeakTable<Monster, NetInt> Values { get; } = [];

    internal static NetInt Get_BlindTimer(this Monster monster)
    {
        return Values.GetOrCreateValue(monster);
    }

    // Net types are readonly
    internal static void Set_BlindTimer(this Monster monster, NetInt value)
    {
    }
}
