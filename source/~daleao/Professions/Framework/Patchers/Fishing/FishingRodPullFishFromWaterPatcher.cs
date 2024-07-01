/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Professions.Framework.Patchers.Fishing;

#region using directives

using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Buildings;
using StardewValley.Extensions;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class FishingRodPullFishFromWaterPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FishingRodPullFishFromWaterPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal FishingRodPullFishFromWaterPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<FishingRod>(nameof(FishingRod.pullFishFromWater));
    }

    #region harmony patches

    /// <summary>Pull out legendary family members.</summary>
    [HarmonyPrefix]
    private static void FishingRodPullFishFromWaterPrefix(FishingRod __instance, ref string fishId, bool fromFishPond)
    {
        if (!fromFishPond || ModHelper.ModRegistry.IsLoaded("DaLion.Ponds"))
        {
            return;
        }

        var (x, y) = Reflector
            .GetUnboundMethodDelegate<Func<FishingRod, Vector2>>(__instance, "calculateBobberTile")
            .Invoke(__instance);
        var pond = Game1.getFarm().buildings.OfType<FishPond>().FirstOrDefault(p =>
            x > p.tileX.Value && x < p.tileX.Value + p.tilesWide.Value - 1 &&
            y > p.tileY.Value && y < p.tileY.Value + p.tilesHigh.Value - 1);
        if (pond is null || pond.FishCount < 0 || Data.ReadAs<int>(__instance, DataKeys.FamilyLivingHere) <= 0 ||
            !Game1.random.NextBool())
        {
            return;
        }

        fishId = Lookups.FamilyPairs[$"(O){pond.fishType.Value}"];
        Data.Increment(__instance, DataKeys.FamilyLivingHere, -1);
    }

    /// <summary>Count trash fished by rod.</summary>
    [HarmonyPostfix]
    private static void FishingRodPullFishFromWaterPostfix(string fishId, bool fromFishPond)
    {
        if (!fromFishPond && fishId.IsTrashId() && Game1.player.HasProfession(Profession.Conservationist))
        {
            Data.Increment(Game1.player, DataKeys.ConservationistTrashCollectedThisSeason);
        }
    }

    #endregion harmony patches
}
