/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Professions.Framework.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;
using Netcode;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Farmer_TreasureHunt
{
    internal static ConditionalWeakTable<Farmer, NetBool> HuntingState { get; } = [];

    internal static NetBool Get_IsHuntingTreasure(this Farmer farmer)
    {
        return HuntingState.GetOrCreateValue(farmer);
    }

    // Net types are readonly
    internal static void Set_IsHuntingTreasure(this Farmer farmer, NetBool value)
    {
    }
}
