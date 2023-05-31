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
using StardewValley.Monsters;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Monster_Taunted
{
    internal static ConditionalWeakTable<Monster, Holder> Values { get; } = new();

    internal static Monster? Get_Taunter(this Monster monster)
    {
        return Values.GetOrCreateValue(monster).Taunter;
    }

    internal static void Set_Taunter(this Monster monster, Monster? taunter)
    {
        var holder = Values.GetOrCreateValue(monster);
        holder.Taunter = taunter;
        holder.FakeFarmer = taunter is null
            ? null
            : new FakeFarmer { UniqueMultiplayerID = monster.GetHashCode(), currentLocation = monster.currentLocation };
    }

    internal static Farmer? Get_FakeFarmer(this Monster monster)
    {
        return Values.GetOrCreateValue(monster).FakeFarmer;
    }

    internal class Holder
    {
        public Monster? Taunter { get; internal set; }

        public FakeFarmer? FakeFarmer { get; internal set; }
    }
}
