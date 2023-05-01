/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Goals;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.TerrainFeatures;
using xTile.Dimensions;
using Rectangle = xTile.Dimensions.Rectangle;

namespace StardewArchipelago.GameModifications.CodeInjections
{
    internal class FarmInjections
    {
        private static IMonitor _monitor;
        private static ArchipelagoClient _archipelago;

        public static void Initialize(IMonitor monitor, ArchipelagoClient archipelago)
        {
            _monitor = monitor;
            _archipelago = archipelago;
        }

        public static bool CheckAction_GrandpaNote_PreFix(Farm __instance, Location tileLocation, Rectangle viewport, Farmer who, ref bool __result)
        {
            try
            {
                var rect = new Microsoft.Xna.Framework.Rectangle(tileLocation.X * 64, tileLocation.Y * 64, 64, 64);
                if (!__instance.objects.ContainsKey(new Vector2(tileLocation.X, tileLocation.Y)) &&
                    __instance.CheckPetAnimal(rect, who))
                {
                    __result = true;
                    return false; // don't run original logic
                }
                var grandpaShrinePosition = __instance.GetGrandpaShrinePosition();
                if (tileLocation.X < grandpaShrinePosition.X - 1 || tileLocation.X > grandpaShrinePosition.X + 1 ||
                    tileLocation.Y != grandpaShrinePosition.Y)
                {
                    return true; // run original logic
                }

                if (__instance.hasSeenGrandpaNote)
                {
                    return true; // run original logic
                }

                Game1.addMail("hasSeenGrandpaNote", true);
                __instance.hasSeenGrandpaNote = true;
                var noteContentTemplate = "{0}^^I may be gone, but I am still watching over you^Don't forget why you are here:^{1}^^-Grandpa";
                var goalGrandpaString = GoalCodeInjection.GetGoalStringGrandpa();
                var noteContent = string.Format(noteContentTemplate, _archipelago.SlotData.SlotName, goalGrandpaString);
                Game1.activeClickableMenu = new LetterViewerMenu(noteContent);
                __result = true;
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(CheckAction_GrandpaNote_PreFix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        public static bool SpawnWeedsAndStones_ConsiderUserPreference_PreFix(GameLocation __instance, ref int numDebris, bool weedsOnly, bool spawnFromOldWeeds)
        {
            try
            {
                if (numDebris < 0)
                {
                    return true; // run original logic;
                }

                switch (_archipelago.SlotData.DebrisMultiplier)
                {
                    case DebrisMultiplier.Vanilla:
                        break;
                    case DebrisMultiplier.HalfDebris:
                        numDebris /= 2;
                        break;
                    case DebrisMultiplier.QuarterDebris:
                        numDebris /= 4;
                        break;
                    case DebrisMultiplier.NoDebris:
                        numDebris = 0;
                        break;
                    case DebrisMultiplier.StartClear:
                        if (Game1.Date.TotalDays == 0)
                        {
                            numDebris = 0;
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }


                return true; // run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(SpawnWeedsAndStones_ConsiderUserPreference_PreFix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        public static void DeleteStartingDebris()
        {
            try
            {
                var farm = Game1.getFarm();

                if (Game1.Date.TotalDays < 1)
                {
                    var chanceOfStaying = GetChanceOfStaying();
                    for (var i = farm.resourceClumps.Count - 1; i >= 0; i--)
                    {
                        var clump = farm.resourceClumps[i];
                        if (Game1.random.NextDouble() > chanceOfStaying)
                        {
                            farm.removeEverythingFromThisTile((int)clump.tile.X, (int)clump.tile.Y);
                        }
                    }

                    foreach (var (tile, feature) in farm.terrainFeatures.Pairs)
                    {
                        if (!(feature is Tree) && !(feature is Grass))
                        {
                            continue;
                        }
                        if (Game1.random.NextDouble() > chanceOfStaying)
                        {
                            farm.removeEverythingFromThisTile((int)tile.X, (int)tile.Y);
                        }
                    }
                    foreach (var (tile, obj) in farm.Objects.Pairs)
                    {
                        if (obj.name != "Stone" && !obj.name.StartsWith("Weed") && obj.name != "Twig")
                        {
                            continue;
                        }

                        if (Game1.random.NextDouble() > chanceOfStaying)
                        {
                            farm.removeEverythingFromThisTile((int)tile.X, (int)tile.Y);
                        }
                    }
                }
                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(DeleteStartingDebris)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        private static double GetChanceOfStaying()
        {
            switch (_archipelago.SlotData.DebrisMultiplier)
            {
                case DebrisMultiplier.Vanilla:
                    return 1;
                case DebrisMultiplier.HalfDebris:
                    return 0.5;
                case DebrisMultiplier.QuarterDebris:
                    return 0.25;
                case DebrisMultiplier.NoDebris:
                    return 0;
                case DebrisMultiplier.StartClear:
                    if (Game1.Date.TotalDays < 1)
                    {
                        return 0;
                    }
                    return 1;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
