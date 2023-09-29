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

using System.Linq;
using System.Runtime.CompilerServices;
using DaLion.Overhaul.Modules.Combat.Integrations;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Collections;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Farmer_CooldownReduction
{
    internal static ConditionalWeakTable<Farmer, Holder> Values { get; } = new();

    internal static float Get_CooldownReduction(this Farmer farmer)
    {
        if (!JsonAssetsIntegration.GarnetRingIndex.HasValue)
        {
            return 1f;
        }

        return 1f - (Values.GetValue(farmer, Create).CooldownReduction * 0.1f);
    }

    internal static void IncrementCooldownReduction(this Farmer farmer, float amount = 1f)
    {
        Values.GetValue(farmer, Create).CooldownReduction += amount;
    }

    private static Holder Create(Farmer farmer)
    {
        var rings = WearMoreRingsIntegration.Instance
            ?.ModApi
            ?.GetAllRings(farmer) ?? farmer.leftRing.Value.Collect(farmer.rightRing.Value);
        return new Holder
        {
            CooldownReduction = rings.WhereNotNull().Aggregate(
                0,
                (cdr, ring) => cdr + (ring.ParentSheetIndex == JsonAssetsIntegration.GarnetRingIndex!.Value ? 1 : 0)),
        };
    }

    internal class Holder
    {
        public float CooldownReduction { get; internal set; }
    }
}
