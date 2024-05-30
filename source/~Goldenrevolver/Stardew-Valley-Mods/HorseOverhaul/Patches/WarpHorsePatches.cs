/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

namespace HorseOverhaul
{
    using HarmonyLib;
    using StardewValley;
    using StardewValley.Buildings;
    using StardewValley.Characters;
    using StardewValley.Tools;
    using StardewObject = StardewValley.Object;

    internal class WarpHorsePatches
    {
        private static HorseOverhaul mod;
        private const string horseFluteQID = "(O)911";

        internal static void ApplyPatches(HorseOverhaul horseOverhaul, Harmony harmony)
        {
            mod = horseOverhaul;

            harmony.Patch(
               original: AccessTools.Method(typeof(Wand), "wandWarpForReal"),
               prefix: new HarmonyMethod(typeof(WarpHorsePatches), nameof(ReturnScepterHorseWarp)));

            harmony.Patch(
               original: AccessTools.Method(typeof(StardewObject), "totemWarpForReal"),
               prefix: new HarmonyMethod(typeof(WarpHorsePatches), nameof(TotemOrObeliskHorseWarp)));

            harmony.Patch(
               original: AccessTools.Method(typeof(Building), "obeliskWarpForReal"),
               prefix: new HarmonyMethod(typeof(WarpHorsePatches), nameof(TotemOrObeliskHorseWarp)));

            harmony.Patch(
               original: AccessTools.Method(typeof(StardewObject), "CheckForActionOnMiniObelisk"),
               postfix: new HarmonyMethod(typeof(WarpHorsePatches), nameof(MiniObeliskHorseWarp)));

            harmony.Patch(
               original: AccessTools.Method(typeof(Farmer), nameof(Farmer.OnWarp)),
               postfix: new HarmonyMethod(typeof(WarpHorsePatches), nameof(AttemptToWarpHorse)));

            harmony.Patch(
               original: AccessTools.Method(typeof(Utility), nameof(Utility.GetHorseWarpRestrictionsForFarmer)),
               postfix: new HarmonyMethod(typeof(WarpHorsePatches), nameof(AllowHorseWarp)));
        }

        private static void ReturnScepterHorseWarp()
        {
            if (Utility.getHomeOfFarmer(Game1.player) != null)
            {
                CanAllowNextHorseWarp();
            }
        }

        private static void TotemOrObeliskHorseWarp()
        {
            CanAllowNextHorseWarp();
        }

        private static void MiniObeliskHorseWarp(Farmer who, bool justCheckingForActivity, bool __result)
        {
            if (justCheckingForActivity || !__result || who != Game1.player)
            {
                return;
            }

            if (mod.Helper.ModRegistry.IsLoaded("PeacefulEnd.MultipleMiniObelisks"))
            {
                return;
            }

            if (CanAllowNextHorseWarp())
            {
                DelayedAction.functionAfterDelay(AttemptToWarpHorse, 950);
            }
        }

        private static bool CanAllowNextHorseWarp()
        {
            if (!mod.Config.WarpHorseWithYou)
            {
                return false;
            }

            Farmer who = Game1.player;

            if (who?.currentLocation == null)
            {
                return false;
            }

            bool isFluteInInventory = who.Items.ContainsId(horseFluteQID);

            if (mod.Config.WarpHorseFluteRequirement == WarpHorseFluteRequirementOption.In_Inventory.ToString())
            {
                if (!isFluteInInventory)
                {
                    return false;
                }
            }
            else if (mod.Config.WarpHorseFluteRequirement == WarpHorseFluteRequirementOption.Owned.ToString())
            {
                if (!isFluteInInventory && !FindHorseFluteInStorage())
                {
                    return false;
                }
            }

            bool hasNearbyHorse = mod.Config.WarpHorseWithFluteIgnoresRange && isFluteInInventory;

            // when the player owns multiple horses, this might not be the same horse
            if (!hasNearbyHorse)
            {
                foreach (NPC character in who.currentLocation.characters)
                {
                    if (character.EventActor || character is not Horse horse)
                    {
                        continue;
                    }

                    if (horse.getOwner() != who || !horse.WithinRangeOfPlayer(mod, who, mod.Config.MaximumWarpDetectionRange))
                    {
                        continue;
                    }

                    hasNearbyHorse = true;
                    break;
                }
            }

            if (hasNearbyHorse)
            {
                mod.isDoingHorseWarp.Value = true;

                return true;
            }
            else
            {
                return false;
            }
        }

        private static bool FindHorseFluteInStorage()
        {
            bool isInStorage = false;
            Utility.iterateChestsAndStorage(delegate (Item item)
            {
                if (item.HasBeenInInventory && item.QualifiedItemId == horseFluteQID)
                {
                    isInStorage = true;
                }
            });

            return isInStorage;
        }

        private static void AllowHorseWarp(ref Utility.HorseWarpRestrictions __result)
        {
            if (mod.isDoingHorseWarp.Value)
            {
                if ((__result & Utility.HorseWarpRestrictions.NoRoom) == Utility.HorseWarpRestrictions.NoRoom)
                {
                    __result &= ~Utility.HorseWarpRestrictions.NoRoom;
                }
            }
        }

        private static void AttemptToWarpHorse()
        {
            if (mod.isDoingHorseWarp.Value)
            {
                Game1.player.team.requestHorseWarpEvent.Fire(Game1.player.UniqueMultiplayerID);

                mod.isDoingHorseWarp.Value = false;
            }
        }
    }
}