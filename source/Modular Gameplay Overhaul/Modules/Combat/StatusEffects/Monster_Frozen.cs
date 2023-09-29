/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Combat.StatusEffects;

#region using directives

using System.Runtime.CompilerServices;
using Netcode;
using StardewValley.Monsters;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Monster_Frozen
{
    internal static ConditionalWeakTable<Monster, Holder> Values { get; } = new();

    internal static NetBool Get_Frozen(this Monster monster)
    {
        return Values.GetOrCreateValue(monster).Frozen;
    }

    // Net types are readonly
    internal static void Set_Frozen(this Monster monster, NetBool value)
    {
    }

    internal class Holder
    {
        public NetBool Frozen { get; } = new(false);
    }
}
