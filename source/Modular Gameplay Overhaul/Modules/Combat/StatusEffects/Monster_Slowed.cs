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
internal static class Monster_Slowed
{
    internal static ConditionalWeakTable<Monster, Holder> Values { get; } = new();

    internal static void SetOrIncrement_Slowed(this Monster monster, int timer, float intensity)
    {
        var holder = Values.GetOrCreateValue(monster);
        holder.SlowTimer.Value = timer;
        holder.SlowIntensity.Value += intensity;
    }

    internal static void Set_Slowed(this Monster monster, int timer, float intensity)
    {
        var holder = Values.GetOrCreateValue(monster);
        holder.SlowTimer.Value = timer;
        holder.SlowIntensity.Value = intensity;
    }

    internal static NetInt Get_SlowTimer(this Monster monster)
    {
        return Values.GetOrCreateValue(monster).SlowTimer;
    }

    // Net types are readonly
    internal static void Set_SlowTmer(this Monster monster, NetInt value)
    {
    }

    internal static NetFloat Get_SlowIntensity(this Monster monster)
    {
        return Values.GetOrCreateValue(monster).SlowIntensity;
    }

    // Net types are readonly
    internal static void Set_SlowIntensity(this Monster monster, NetInt value)
    {
    }

    internal class Holder
    {
        public NetInt SlowTimer { get; } = new(-1);

        public NetFloat SlowIntensity { get; } = new(0);
    }
}
