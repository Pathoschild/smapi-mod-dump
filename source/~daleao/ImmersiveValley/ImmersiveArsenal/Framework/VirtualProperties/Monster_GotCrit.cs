/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Arsenal.Framework.VirtualProperties;

#region using directives

using StardewValley.Monsters;
using System.Runtime.CompilerServices;

#endregion using directives

public static class Monster_GotCrit
{
    internal class Holder
    {
        public bool gotCrit;
    }

    internal static ConditionalWeakTable<Monster, Holder> Values = new();

    public static bool get_GotCrit(this Monster monster) => Values.GetOrCreateValue(monster).gotCrit;

    public static void set_GotCrit(this Monster monster, bool newVal)
    {
        Values.GetOrCreateValue(monster).gotCrit = newVal;
    }
}