/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Combat.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;
using StardewValley.Monsters;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Monster_KnockedBack
{
    internal static ConditionalWeakTable<Monster, Holder> Values { get; } = new();

    internal static bool Get_KnockedBack(this Monster monster)
    {
        return Values.GetOrCreateValue(monster).KnockedBack;
    }

    internal static Farmer? Get_KnockBacker(this Monster monster)
    {
        return Values.GetOrCreateValue(monster).ByWhom;
    }

    internal static void Set_KnockedBack(this Monster monster, Farmer? byWhom)
    {
        Values.GetOrCreateValue(monster).ByWhom = byWhom;
    }

    internal class Holder
    {
        public bool KnockedBack => this.ByWhom is not null;

        public Farmer? ByWhom { get; internal set; }
    }
}
