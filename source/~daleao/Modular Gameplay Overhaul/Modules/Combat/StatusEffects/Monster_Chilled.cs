/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Combat.StatusEffects;

#region using directives

using System.Runtime.CompilerServices;
using Netcode;
using StardewValley.Monsters;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Monster_Chilled
{
    internal static ConditionalWeakTable<Monster, Holder> Values { get; } = new();

    internal static NetBool Get_Chilled(this Monster monster)
    {
        return Values.GetOrCreateValue(monster).Chilled;
    }

    // Net types are readonly
    internal static void Set_Chilled(this Monster monster, NetBool value)
    {
    }

    internal class Holder
    {
        public NetBool Chilled { get; } = new(false);
    }
}
