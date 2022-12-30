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
internal static class Monster_KnockedBack
{
    internal static ConditionalWeakTable<Monster, Holder> Values { get; } = new();

    internal static bool Get_KnockedBack(this Monster monster)
    {
        return Values.GetOrCreateValue(monster).KnockedBack;
    }

    internal static void Set_KnockedBack(this Monster monster, bool value)
    {
        Values.GetOrCreateValue(monster).KnockedBack = value;
    }

    internal class Holder
    {
        public bool KnockedBack { get; internal set; }
    }
}
