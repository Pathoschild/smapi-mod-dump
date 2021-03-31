/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Harmony;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using SObject = StardewValley.Object;

namespace Pathoschild.Stardew.SmallBeachFarm.Framework
{
    /// <summary>Encapsulates Harmony patches for the <see cref="Farm"/> instance.</summary>
    internal static class FarmPatcher
    {
        /*********
        ** Fields
        *********/
        /// <summary>Encapsulates logging for the Harmony patch.</summary>
        private static IMonitor Monitor;

        /// <summary>Use the beach's background music (i.e. wave sounds) on the beach farm.</summary>
        private static bool UseBeachMusic;

        /// <summary>Whether to add the campfire to the farm map.</summary>
        private static bool AddCampfire;

        /// <summary>Get whether the given location is the Small Beach Farm.</summary>
        private static Func<GameLocation, bool> IsSmallBeachFarm;

        /// <summary>Get the fish that should be available from the given tile.</summary>
        private static Func<Farm, int, int, FishType> GetFishType;

        /// <summary>Whether the mod is currently applying patch changes (to avoid infinite recursion),</summary>
        private static bool IsInPatch = false;

        /// <summary>The arrival tiles by farm type when the player warps from the <see cref="IslandWest"/> obelisk, with a <c>-1</c> key for the default.</summary>
        /// <remarks>Derived from <see cref="IslandWest.performAction"/>.</remarks>
        private static readonly Dictionary<int, Point> IslandWarpTargets = new()
        {
            [Farm.fourCorners_layout] = new Point(48, 39),
            [Farm.beach_layout] = new Point(82, 29),
            [-1] = new Point(48, 7)
        };


        /*********
        ** Public methods
        *********/
        /// <summary>Initialize the Harmony patches.</summary>
        /// <param name="harmony">The Harmony patching API.</param>
        /// <param name="monitor">Encapsulates logging for the Harmony patch.</param>
        /// <param name="addCampfire">Whether to add the campfire to the farm map.</param>
        /// <param name="useBeachMusic">Use the beach's background music (i.e. wave sounds) on the beach farm.</param>
        /// <param name="isSmallBeachFarm">Get whether the given location is the Small Beach Farm.</param>
        /// <param name="getFishType">Get the fish that should be available from the given tile.</param>
        public static void Hook(HarmonyInstance harmony, IMonitor monitor, bool addCampfire, bool useBeachMusic, Func<GameLocation, bool> isSmallBeachFarm, Func<Farm, int, int, FishType> getFishType)
        {
            FarmPatcher.Monitor = monitor;
            FarmPatcher.AddCampfire = addCampfire;
            FarmPatcher.UseBeachMusic = useBeachMusic;
            FarmPatcher.IsSmallBeachFarm = isSmallBeachFarm;
            FarmPatcher.GetFishType = getFishType;

            harmony.Patch(
                original: AccessTools.Method(typeof(Farm), nameof(Farm.getFish)),
                prefix: new HarmonyMethod(typeof(FarmPatcher), nameof(FarmPatcher.Before_GetFish))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Farm), "resetLocalState"),
                prefix: new HarmonyMethod(typeof(FarmPatcher), nameof(FarmPatcher.After_ResetLocalState))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Farm), "resetSharedState"),
                prefix: new HarmonyMethod(typeof(FarmPatcher), nameof(FarmPatcher.After_ResetSharedState))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.cleanupBeforePlayerExit)),
                prefix: new HarmonyMethod(typeof(FarmPatcher), nameof(FarmPatcher.After_CleanupBeforePlayerExit))
            );
        }


        /*********
        ** Private methods
        *********/
        /// <summary>A method called via Harmony before <see cref="Farm.getFish"/>, which gets ocean fish from the beach properties if fishing the ocean water.</summary>
        /// <param name="__instance">The farm instance.</param>
        /// <param name="millisecondsAfterNibble">An argument passed through to the underlying method.</param>
        /// <param name="bait">An argument passed through to the underlying method.</param>
        /// <param name="waterDepth">An argument passed through to the underlying method.</param>
        /// <param name="who">An argument passed through to the underlying method.</param>
        /// <param name="baitPotency">An argument passed through to the underlying method.</param>
        /// <param name="bobberTile">The tile containing the bobber.</param>
        /// <param name="__result">The return value to use for the method.</param>
        /// <returns>Returns <c>true</c> if the original logic should run, or <c>false</c> to use <paramref name="__result"/> as the return value.</returns>
        [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "The naming convention is defined by Harmony.")]
        private static bool Before_GetFish(Farm __instance, float millisecondsAfterNibble, int bait, int waterDepth, Farmer who, double baitPotency, Vector2 bobberTile, ref SObject __result)
        {
            if (FarmPatcher.IsInPatch || !FarmPatcher.IsSmallBeachFarm(who?.currentLocation))
                return true;

            try
            {
                FarmPatcher.IsInPatch = true;

                FishType type = FarmPatcher.GetFishType(__instance, (int)bobberTile.X, (int)bobberTile.Y);
                FarmPatcher.Monitor.VerboseLog($"Fishing {type.ToString().ToLower()} tile at ({bobberTile.X / Game1.tileSize}, {bobberTile.Y / Game1.tileSize}).");
                switch (type)
                {
                    case FishType.Ocean:
                        __result = __instance.getFish(millisecondsAfterNibble, bait, waterDepth, who, baitPotency, bobberTile, "Beach");
                        return false;

                    case FishType.River:
                        // match riverland farm behavior
                        __result = Game1.random.NextDouble() < 0.3
                            ? __instance.getFish(millisecondsAfterNibble, bait, waterDepth, who, baitPotency, bobberTile, "Forest")
                            : __instance.getFish(millisecondsAfterNibble, bait, waterDepth, who, baitPotency, bobberTile, "Town");
                        return false;

                    default:
                        return true; // run original method
                }
            }
            finally
            {
                FarmPatcher.IsInPatch = false;
            }
        }

        /// <summary>A method called via Harmony after <see cref="Farm.resetLocalState"/>.</summary>
        /// <param name="__instance">The farm instance.</param>
        [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "The naming convention is defined by Harmony.")]
        private static void After_ResetLocalState(GameLocation __instance)
        {
            if (!FarmPatcher.IsSmallBeachFarm(__instance))
                return;

            // change background track
            if (FarmPatcher.ShouldUseBeachMusic())
                Game1.changeMusicTrack("ocean", music_context: Game1.MusicContext.SubLocation);

            // fix island totem warp
            if (FarmPatcher.IsOnIslandWarpArrivalTile())
            {
                var tile = __instance.GetMapPropertyPosition("WarpTotemEntry", 48, 7);
                Game1.player.Position = new Vector2(tile.X * Game1.tileSize, tile.Y * Game1.tileSize);
            }
        }

        /// <summary>A method called via Harmony after <see cref="Farm.resetSharedState"/>.</summary>
        /// <param name="__instance">The farm instance.</param>
        [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "The naming convention is defined by Harmony.")]
        private static void After_ResetSharedState(GameLocation __instance)
        {
            if (!FarmPatcher.IsSmallBeachFarm(__instance))
                return;

            // toggle campfire (derived from StardewValley.Locations.Mountain:resetSharedState
            Vector2 campfireTile = new Vector2(64, 22);
            if (FarmPatcher.AddCampfire)
            {
                if (!__instance.objects.ContainsKey(campfireTile))
                {
                    __instance.objects.Add(campfireTile, new Torch(campfireTile, 146, true)
                    {
                        IsOn = false,
                        Fragility = SObject.fragility_Indestructable
                    });
                }
            }
            else if (__instance.objects.TryGetValue(campfireTile, out SObject obj) && obj is Torch torch && torch.ParentSheetIndex == 146)
                __instance.objects.Remove(campfireTile);
        }

        /// <summary>A method called via Harmony after <see cref="Farm.cleanupBeforePlayerExit"/>.</summary>
        /// <param name="__instance">The farm instance.</param>
        [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "The naming convention is defined by Harmony.")]
        private static void After_CleanupBeforePlayerExit(GameLocation __instance)
        {
            if (!FarmPatcher.IsSmallBeachFarm(__instance))
                return;

            // change background track
            if (FarmPatcher.ShouldUseBeachMusic())
                Game1.changeMusicTrack("none", music_context: Game1.MusicContext.SubLocation);
        }

        /// <summary>Get whether the Small Beach Farm's music should be overridden with the beach sounds.</summary>
        private static bool ShouldUseBeachMusic()
        {
            return FarmPatcher.UseBeachMusic && !Game1.isRaining;
        }

        /// <summary>Whether the player is on the arrival tile for the <see cref="IslandWest"/> obelisk warp.</summary>
        private static bool IsOnIslandWarpArrivalTile()
        {
            Point curTile = Utility.Vector2ToPoint(Game1.player.Position / Game1.tileSize);

            if (!FarmPatcher.IslandWarpTargets.TryGetValue(Game1.whichFarm, out Point arrivalTile))
                arrivalTile = FarmPatcher.IslandWarpTargets[-1];

            return curTile == arrivalTile;
        }
    }
}
