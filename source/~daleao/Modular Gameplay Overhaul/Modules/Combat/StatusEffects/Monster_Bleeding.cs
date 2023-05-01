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
internal static class Monster_Bleeding
{
    internal static ConditionalWeakTable<Monster, Holder> Values { get; } = new();

    internal static NetInt Get_BleedTimer(this Monster monster)
    {
        return Values.GetOrCreateValue(monster).BleedTimer;
    }

    // Net types are readonly
    internal static void Set_BleedTimer(this Monster monster, NetInt value)
    {
    }

    internal static NetInt Get_BleedStacks(this Monster monster)
    {
        return Values.GetOrCreateValue(monster).BleedStacks;
    }

    // Net types are readonly
    internal static void Set_BleedStacks(this Monster monster, NetInt stacks)
    {
    }

    internal static Farmer? Get_Bleeder(this Monster monster)
    {
        return Values.GetOrCreateValue(monster).Bleeder;
    }

    internal static void Set_Bleeder(this Monster monster, Farmer? bleeder)
    {
        Values.GetOrCreateValue(monster).Bleeder = bleeder;
    }

    internal class Holder
    {
        public NetInt BleedTimer { get; } = new(-1);

        public NetInt BleedStacks { get; internal set; } = new(0);

        public Farmer? Bleeder { get; internal set; }
    }
}
