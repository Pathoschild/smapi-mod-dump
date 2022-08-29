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

using StardewValley.Tools;
using System.Runtime.CompilerServices;

#endregion using directives

public static class Slingshot_OnSpecial
{
    internal class Holder
    {
        public bool isOnSpecial;
    }

    internal static ConditionalWeakTable<Slingshot, Holder> Values = new();

    public static bool get_IsOnSpecial(this Slingshot slingshot) => Values.GetOrCreateValue(slingshot).isOnSpecial;

    public static void set_IsOnSpecial(this Slingshot slingshot, bool newVal)
    {
        Values.GetOrCreateValue(slingshot).isOnSpecial = newVal;
    }
}