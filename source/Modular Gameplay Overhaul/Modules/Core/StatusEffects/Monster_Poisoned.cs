/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Core.StatusEffects;

#region using directives

using System.Runtime.CompilerServices;
using Netcode;
using StardewValley.Monsters;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Monster_Poisoned
{
    internal static ConditionalWeakTable<Monster, Holder> Values { get; } = new();

    internal static void SetOrIncrement_Poisoned(this Monster monster, int timer, int stacks, Farmer? poisoner)
    {
        var holder = Values.GetOrCreateValue(monster);
        holder.PoisonTimer.Value = timer;
        holder.PoisonStacks.Value = Math.Min(holder.PoisonStacks.Value + stacks, 4);
        holder.Poisoner = poisoner;
    }

    internal static void Set_Poisoned(this Monster monster, int timer, int stacks, Farmer? poisoner)
    {
        var holder = Values.GetOrCreateValue(monster);
        holder.PoisonTimer.Value = timer;
        holder.PoisonStacks.Value = stacks;
        holder.Poisoner = poisoner;
    }

    internal static NetInt Get_PoisonTimer(this Monster monster)
    {
        return Values.GetOrCreateValue(monster).PoisonTimer;
    }

    // Net types are readonly
    internal static void Set_PoisonTimer(this Monster monster, NetInt value)
    {
    }

    internal static NetInt Get_PoisonStacks(this Monster monster)
    {
        return Values.GetOrCreateValue(monster).PoisonStacks;
    }

    // Net types are readonly
    internal static void Set_PoisonStacks(this Monster monster, NetInt stacks)
    {
    }

    internal static Farmer? Get_Poisoner(this Monster monster)
    {
        return Values.GetOrCreateValue(monster).Poisoner;
    }

    internal static void Set_Poisoner(this Monster monster, Farmer? poisoner)
    {
        Values.GetOrCreateValue(monster).Poisoner = poisoner;
    }

    // Avoid redundant hashing
    internal static Holder Get_PoisonHolder(this Monster monster)
    {
        return Values.GetOrCreateValue(monster);
    }

    internal class Holder
    {
        public NetInt PoisonTimer { get; } = new(-1);

        public NetInt PoisonStacks { get; internal set; } = new(0);

        public Farmer? Poisoner { get; internal set; }

        public void Deconstruct(out NetInt timer, out NetInt stacks, out Farmer? poisoner)
        {
            timer = this.PoisonTimer;
            stacks = this.PoisonStacks;
            poisoner = this.Poisoner;
        }
    }
}
