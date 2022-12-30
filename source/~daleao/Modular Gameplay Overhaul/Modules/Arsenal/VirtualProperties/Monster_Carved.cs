/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Arsenal.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;
using StardewValley.Monsters;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Monster_Carved
{
    internal static ConditionalWeakTable<Monster, Holder> Values { get; } = new();

    internal static int Get_Carved(this Monster monster)
    {
        return Values.GetOrCreateValue(monster).Carved;
    }

    internal static void Set_Carved(this Monster monster, int value)
    {
        Values.GetOrCreateValue(monster).Carved = value;
    }

    internal static void IncrementCarved(this Monster monster, int amount = 1)
    {
        Values.GetOrCreateValue(monster).Carved += amount;
    }

    internal class Holder
    {
        public int Carved { get; internal set; }
    }
}
