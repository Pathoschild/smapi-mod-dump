/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Professions.Patchers.Fishing;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Constants;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Objects;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class CrabPotCheckForActionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="CrabPotCheckForActionPatcher"/> class.</summary>
    internal CrabPotCheckForActionPatcher()
    {
        this.Target = this.RequireMethod<CrabPot>(nameof(CrabPot.checkForAction));
        this.Prefix!.priority = Priority.High;
    }

    #region harmony patches

    /// <summary>Patch to handle Luremaster-caught non-trap fish.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.High)]
    private static bool CrabPotCheckForActionPrefix(
        ref CrabPot __instance,
        ref bool __result,
        ref bool ___lidFlapping,
        ref float ___lidFlapTimer,
        ref Vector2 ___shake,
        ref float ___shakeTimer,
        Farmer who,
        bool justCheckingForActivity = false)
    {
        try
        {
            if (__instance.tileIndexToShow != 714 || justCheckingForActivity ||
                !__instance.HasSpecialLuremasterCatch())
            {
                return true; // run original logic
            }

            var item = __instance.heldObject.Value;
            bool addedToInventory;
            switch (__instance.heldObject.Value.ParentSheetIndex)
            {
                case WeaponIds.NeptuneGlaive:
                case WeaponIds.BrokenTrident:
                    // caught a weapon
                    var weapon = new MeleeWeapon(__instance.heldObject.Value.ParentSheetIndex);
                    if (CombatModule.ShouldEnable && CombatModule.Config.EnableWeaponOverhaul &&
                        weapon.InitialParentTileIndex == WeaponIds.NeptuneGlaive)
                    {
                        weapon.specialItem = true;
                    }

                    addedToInventory = who.addItemToInventoryBool(weapon);
                    who.mostRecentlyGrabbedItem = weapon;
                    break;

                case 516:
                case 517:
                case 518:
                case 519:
                case 527:
                case 529:
                case 530:
                case 531:
                case 532:
                case 533:
                case 534:
                    // caught a ring
                    var ring = new Ring(__instance.heldObject.Value.ParentSheetIndex);
                    addedToInventory = who.addItemToInventoryBool(ring);
                    who.mostRecentlyGrabbedItem = ring;
                    break;

                default:
                    addedToInventory = who.addItemToInventoryBool(item);
                    break;
            }

            __instance.heldObject.Value = null;
            if (who.IsLocalPlayer && !addedToInventory)
            {
                __instance.heldObject.Value = item;
                Game1.showRedMessage(
                    Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
                __result = false;
                return false; // don't run original logic;
            }

            var fishData = Game1.content.Load<Dictionary<int, string>>("Data\\Fish");
            if (fishData.TryGetValue(item.ParentSheetIndex, out var specificFishData))
            {
                var fields = specificFishData.SplitWithoutAllocation('/');
                var minFishSize = int.Parse(fields[3]);
                var maxFishSize = int.Parse(fields[4]);
                who.caughtFish(item.ParentSheetIndex, Game1.random.Next(minFishSize, maxFishSize + 1));
            }

            __instance.readyForHarvest.Value = false;
            __instance.tileIndexToShow = 710;
            ___lidFlapping = true;
            ___lidFlapTimer = 60f;

            if (!item.IsTrash() || !TweexModule.Config.TrashDoesNotConsumeBait)
            {
                __instance.bait.Value = null;
            }

            who.animateOnce(279 + who.FacingDirection);
            who.currentLocation.playSound("fishingRodBend");
            DelayedAction.playSoundAfterDelay("coin", 500);
            who.gainExperience(1, 5);
            ___shake = Vector2.Zero;
            ___shakeTimer = 0f;

            __result = true;
            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}
