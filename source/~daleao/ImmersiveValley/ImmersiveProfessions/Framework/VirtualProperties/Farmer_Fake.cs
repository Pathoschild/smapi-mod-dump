/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.VirtualProperties;

#region using directives

using Netcode;
using System.Runtime.CompilerServices;

#endregion using directives

public static class Farmer_Fake
{
    internal static ConditionalWeakTable<Farmer, NetBool> Values = new();

    public static NetBool get_IsFake(this Farmer farmer) => Values.GetOrCreateValue(farmer);

    // Net types are readonly
    public static void set_IsFake(this Farmer farmer, NetBool newVal) { }
}