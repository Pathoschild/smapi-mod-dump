using Harmony;
using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley.TerrainFeatures;
using Netcode;

namespace BeeHouseFix
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            //Harmony patcher
            //https://github.com/kirbylink/FestivalEndTimeTweak.git
            var harmony = HarmonyInstance.Create("com.github.kirbylink.beehousefix");
            var original = typeof(Utility).GetMethod("findCloseFlower");
            var prefix = helper.Reflection.GetMethod(typeof(FixBeeHouses), "Prefix").MethodInfo;
            harmony.Patch(original, new HarmonyMethod(prefix), null);

        }
    }

    public static class FixBeeHouses
    {
        static bool Prefix(GameLocation location, Vector2 startTileLocation, ref Crop __result)
        {
            
            Queue<Vector2> flowersToCheck = new Queue<Vector2>();
            HashSet<Vector2> checkedFlowers = new HashSet<Vector2>();
            Crop poppy, summerSpangle, blueJazz, tulip;
            poppy = summerSpangle = blueJazz = tulip = null;
            var terrainFeatures = location.terrainFeatures;

            flowersToCheck.Enqueue(startTileLocation);

            while (flowersToCheck.Count > 0)
            {
                Vector2 tile = flowersToCheck.Dequeue();
                bool containsHoeDirt = terrainFeatures.ContainsKey(tile) && terrainFeatures[tile] is HoeDirt;
                if (!containsHoeDirt)
                {
                    goto CheckAdjacent;
                }

                HoeDirt dirt = terrainFeatures[tile] as HoeDirt;
                bool isFlower = dirt.crop != null && dirt.crop.programColored.Value;
                if (!isFlower)
                {
                    goto CheckAdjacent;
                }

                bool isGrownAndNotDead = dirt.crop.currentPhase.Value >= dirt.crop.phaseDays.Count - 1 && !dirt.crop.dead.Value;
                if (isGrownAndNotDead)
                {
                    switch (dirt.crop.indexOfHarvest.Value)
                    {
                        case 376:
                            poppy = dirt.crop;
                            break;
                        case 591:
                            tulip = dirt.crop;
                            break;
                        case 593:
                            summerSpangle = dirt.crop;
                            break;
                        case 595: //Found most expensive flower
                            __result = dirt.crop;
                            return false;
                        case 597:
                            blueJazz = dirt.crop;
                            break;
                    }
                }

            CheckAdjacent:
                if (checkedFlowers.Count < 59) //At max tiles, stop queing
                {
                    foreach (Vector2 adjacentTileLocation in Utility.getAdjacentTileLocations(tile))
                    {
                        if (!checkedFlowers.Contains(adjacentTileLocation) && !flowersToCheck.Contains(adjacentTileLocation))
                        {
                            flowersToCheck.Enqueue(adjacentTileLocation);
                        }
                    }
                    checkedFlowers.Add(tile);
                }
            }

            __result = poppy ?? summerSpangle ?? blueJazz ?? tulip;
            return false;
            
        }
    }
}
