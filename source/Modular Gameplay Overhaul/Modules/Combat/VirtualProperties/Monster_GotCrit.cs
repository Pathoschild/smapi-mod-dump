/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Combat.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;
using StardewValley.Monsters;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Monster_GotCrit
{
    internal static ConditionalWeakTable<Monster, Holder> Values { get; } = new();

    internal static bool Get_GotCrit(this Monster monster)
    {
        return Values.GetOrCreateValue(monster).GotCrit;
    }

    internal static void Set_GotCrit(this Monster monster, bool value)
    {
        Values.GetOrCreateValue(monster).GotCrit = value;
    }

    internal class Holder
    {
        public bool GotCrit { get; internal set; }
    }
}
