/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Professions.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;
using Netcode;
using StardewValley.Monsters;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Monster_Feared
{
    internal static ConditionalWeakTable<Monster, Holder> Values { get; } = new();

    internal static NetInt Get_FearTimer(this Monster monster)
    {
        return Values.GetOrCreateValue(monster).FearTimer;
    }

    // Net types are readonly
    internal static void Set_FearTimer(this Monster monster, NetInt value)
    {
    }

    internal static NetInt Get_FearIntensity(this Monster monster)
    {
        return Values.GetOrCreateValue(monster).FearIntensity;
    }

    // Net types are readonly
    internal static void Set_FearIntensity(this Monster monster, NetInt value)
    {
    }

    internal static Farmer Get_Fearer(this Monster monster)
    {
        return Values.GetOrCreateValue(monster).Fearer;
    }

    internal static void Set_Fearer(this Monster monster, Farmer fearer)
    {
        Values.GetOrCreateValue(monster).Fearer = fearer;
    }

    internal class Holder
    {
        public NetInt FearIntensity { get;  } = new(-1);

        public NetInt FearTimer { get; } = new(-1);

        public Farmer Fearer { get; internal set; } = null!;
    }
}
