/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Arsenal.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;
using StardewValley.Monsters;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Monster_Overkill
{
    internal static ConditionalWeakTable<Monster, Holder> Values { get; } = new();

    internal static int Get_Overkill(this Monster monster)
    {
        return Values.GetOrCreateValue(monster).Overkill;
    }

    internal static void Set_Overkill(this Monster monster, int value)
    {
        Values.GetOrCreateValue(monster).Overkill = value;
    }

    internal class Holder
    {
        public int Overkill { get; internal set; }
    }
}
