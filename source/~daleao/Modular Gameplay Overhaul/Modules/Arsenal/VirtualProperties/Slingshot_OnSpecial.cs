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
using StardewValley.Tools;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Slingshot_OnSpecial
{
    internal static ConditionalWeakTable<Slingshot, Holder> Values { get; } = new();

    internal static bool Get_IsOnSpecial(this Slingshot slingshot)
    {
        return Values.GetOrCreateValue(slingshot).IsOnSpecial;
    }

    internal static void Set_IsOnSpecial(this Slingshot slingshot, bool value)
    {
        Values.GetOrCreateValue(slingshot).IsOnSpecial = value;
    }

    internal class Holder
    {
        public bool IsOnSpecial { get; internal set; }
    }
}
