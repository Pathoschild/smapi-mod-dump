/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Ponds.Framework.Patchers;

#region using directives

using System.Reflection;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Buildings;
using StardewValley.Extensions;

#endregion using directives

[UsedImplicitly]
internal sealed class FishPondClearPondPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FishPondClearPondPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal FishPondClearPondPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<FishPond>(nameof(FishPond.ClearPond));
    }

    #region harmony patches

    /// <summary>Clear Family Living Here data.</summary>
    [HarmonyPrefix]
    private static bool FishPondClearPondPrefix(
        FishPond __instance,
        ref SObject? ____fishObject,
        ref bool ____hasAnimatedSpawnedFish)
    {
        try
        {
            var r = __instance.GetBoundingBox();
            for (var i = 0; i < __instance.currentOccupants.Value; i++)
            {
                var pos = Utility.PointToVector2(r.Center);
                var direction = Game1.random.Next(4);
                pos = direction switch
                {
                    0 => new Vector2(Game1.random.Next(r.Left, r.Right), r.Top),
                    1 => new Vector2(r.Right, Game1.random.Next(r.Top, r.Bottom)),
                    2 => new Vector2(Game1.random.Next(r.Left, r.Right), r.Bottom),
                    3 => new Vector2(r.Left, Game1.random.Next(r.Top, r.Bottom)),
                    _ => pos,
                };

                if (__instance.HasAlgae())
                {
                    var algaeItem = CreateAlgaeInstance(__instance);
                    Game1.createItemDebris(algaeItem, pos, direction, Game1.currentLocation, flopFish: true);
                    continue;
                }

                var fishItem = CreateFishInstance(__instance);
                Game1.createItemDebris(fishItem, pos, direction, Game1.currentLocation, flopFish: true);
            }

            ____hasAnimatedSpawnedFish = false;
            __instance.hasSpawnedFish.Value = false;
            __instance._fishSilhouettes.Clear();
            __instance._jumpingFish.Clear();
            __instance.goldenAnimalCracker.Value = false;
            ____fishObject = null;
            __instance.currentOccupants.Value = 0;
            __instance.daysSinceSpawn.Value = 0;
            __instance.neededItem.Value = null;
            __instance.neededItemCount.Value = -1;
            __instance.lastUnlockedPopulationGate.Value = 0;
            __instance.fishType.Value = null;
            __instance.Reseed();
            __instance.overrideWaterColor.Value = Color.White;
            Data.Write(__instance, DataKeys.PondFish, string.Empty);
            Data.Write(__instance, "FamilyLivingHere", string.Empty);
            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches

    #region handlers

    private static Item CreateAlgaeInstance(FishPond pond)
    {
        var algae = pond.ParsePondFishes();
        var pulled = algae.Choose();
        var id = pulled.Id;
        Data.Write(pond, DataKeys.PondFish, string.Join(';', algae));
        return ItemRegistry.Create(id);
    }

    private static Item CreateFishInstance(FishPond pond)
    {
        var fishes = pond.ParsePondFishes();
        fishes.SortDescending();
        PondFish pulled;
        if (pond.HasBossFish())
        {
            if (Data.ReadAs<int>(pond, "FamilyLivingHere") > 0 && Game1.random.NextBool())
            {
                pulled = fishes.Last(f => $"(O){f.Id}" == Lookups.FamilyPairs[$"(O){pond.fishType.Value}"]);
                Data.Increment(pond, "FamilyLivingHere", -1);
            }
            else
            {
                pulled = fishes.Last(f => f.Id == pond.fishType.Value);
            }
        }
        else
        {
            pulled = fishes.Last();
        }

        var (id, quality) = pulled;
        Data.Write(pond, DataKeys.PondFish, string.Join(';', fishes));
        return ItemRegistry.Create(id, quality: quality);
    }

    #endregion handlers
}
