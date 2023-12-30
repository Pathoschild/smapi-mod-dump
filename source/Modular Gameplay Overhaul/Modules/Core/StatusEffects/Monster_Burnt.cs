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
internal static class Monster_Burnt
{
    internal static ConditionalWeakTable<Monster, Holder> Values { get; } = new();

    internal static void Set_Burnt(this Monster monster, int timer, Farmer? burner)
    {
        var holder = Values.GetOrCreateValue(monster);
        holder.BurnTimer.Value = timer;
        holder.Burner = burner;
    }

    internal static NetInt Get_BurnTimer(this Monster monster)
    {
        return Values.GetOrCreateValue(monster).BurnTimer;
    }

    // Net types are readonly
    internal static void Set_BurnTimer(this Monster monster, NetInt value)
    {
    }

    internal static Farmer? Get_Burner(this Monster monster)
    {
        return Values.GetOrCreateValue(monster).Burner;
    }

    internal static void Set_Burner(this Monster monster, Farmer? burner)
    {
        Values.GetOrCreateValue(monster).Burner = burner;
    }

    // Avoid redundant hashing
    internal static Holder Get_BurnHolder(this Monster monster)
    {
        return Values.GetOrCreateValue(monster);
    }

    internal class Holder
    {
        public NetInt BurnTimer { get; } = new(-1);

        public Farmer? Burner { get; internal set; }

        public void Deconstruct(out NetInt timer, out Farmer? burner)
        {
            timer = this.BurnTimer;
            burner = this.Burner;
        }
    }
}
