/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-desert-bloom-farm
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewValley;
using Microsoft.Xna.Framework;
using StardewValley.TerrainFeatures;
using xTile.Dimensions;
using Desert_Bloom.API;

namespace Desert_Bloom.Lib
{
    public class CustomTerrainSpawns
    {
        public static Mod Mod;
        public static IMonitor Monitor;
        public static IModHelper Helper;

        public static bool BetterGrassSpread = false;
        public static void main()
        {
            Mod = ModEntry.Mod;
            Monitor = ModEntry._Monitor;
            Helper = ModEntry._Helper;

            Helper.Events.GameLoop.DayStarted += DayStarted;
        }

        private static void DayStarted(object sender, StardewModdingAPI.Events.DayStartedEventArgs e)
        {
            if (!ModEntry.IsMyFarm() || !Context.IsMainPlayer)
                return;


            growGrassOnNonDiggable();
            irrigateCrops();
        }

        private static void irrigateCrops()
        {
            //5% better odds than advertised to make people happy about being "lucky"
            var chance = Mill.Tier switch {
                0 => 0.0,
                1 => 0.55,
                2 => 0.7,
                3 => 0.85,
                4 => 1,
                5 => 1,
                _ => 0
            };

            if (chance == 0)
                return;

            var farm = Game1.getFarm();
            var oasisArea = new Microsoft.Xna.Framework.Rectangle(107, 13, 29, 29);

            foreach (HoeDirt dirt in farm.terrainFeatures.Values.Where(e => e is HoeDirt && !oasisArea.Contains(e.currentTileLocation))) {
                if (chance == 1 || Game1.random.NextDouble() < chance)
                    dirt.state.Value = 1; //Watered HoeDirt

                //Skipping a day worth of growth
                if (Mill.Tier >= 5
                        && dirt.crop != null
                        && dirt.crop.currentPhase.Value == 1
                        && dirt.crop.dayOfCurrentPhase.Value == 0)
                    if (dirt.crop.phaseDays[1] == 1)
                        dirt.crop.currentPhase.Value++;
                    else
                        dirt.crop.dayOfCurrentPhase.Value++;
            }
        }

        //Grass doesn't usually spread on tiles without the 'Diggable' property, but I want that for this map
        //This function does the same thing the game does just for non diggable tiles as well
        public static void growGrassOnNonDiggable()
        {
            BetterGrassSpread = UnlockableAreas.unlocked("DLX.Desert_Bloom.Mining_Area_3");
            var farm = Game1.getFarm();

            for (int j = farm.terrainFeatures.Count() - 1; j >= 0; j--) {
                KeyValuePair<Vector2, TerrainFeature> kvp = farm.terrainFeatures.Pairs.ElementAt(j);
                if (kvp.Value is not Grass || Game1.random.NextDouble() >= 0.65)
                    continue;

                if ((int)((Grass)kvp.Value).numberOfWeeds.Value < 4)
                    ((Grass)kvp.Value).numberOfWeeds.Value = Math.Max(0, Math.Min(4, (int)((Grass)kvp.Value).numberOfWeeds.Value + Game1.random.Next(3)));

                else if ((int)((Grass)kvp.Value).numberOfWeeds.Value >= 4) {
                    trySpreadGrass(farm, kvp, (int)kvp.Key.X, (int)kvp.Key.Y, -1, 0);
                    trySpreadGrass(farm, kvp, (int)kvp.Key.X, (int)kvp.Key.Y, 1, 0);
                    trySpreadGrass(farm, kvp, (int)kvp.Key.X, (int)kvp.Key.Y, 0, -1);
                    trySpreadGrass(farm, kvp, (int)kvp.Key.X, (int)kvp.Key.Y, 0, 1);

                }
            }
        }

        public static void trySpreadGrass(GameLocation farm, KeyValuePair<Vector2, TerrainFeature> kvp, int xCoord, int yCoord, int xOffset, int yOffset)
        {
            if (farm.isTileOnMap(xCoord, yCoord)
                && !farm.isTileOccupied(kvp.Key + new Vector2(xOffset, yOffset))
                && farm.isTileLocationOpenIgnoreFrontLayers(new Location(xCoord + xOffset, yCoord + yOffset))
                && farm.doesTileHaveProperty(xCoord + xOffset, yCoord + yOffset, "NoSpawn", "Back") == null) {

                var diggable = farm.doesTileHaveProperty(xCoord + xOffset, yCoord + yOffset, "Diggable", "Back") != null;

                var spreadFactor = BetterGrassSpread
                    ? diggable ? 0.15 : 0.4
                    : diggable ? 0 : 0.25;

                if (spreadFactor != 0 && Game1.random.NextDouble() < spreadFactor)
                    farm.terrainFeatures.Add(kvp.Key + new Vector2(xOffset, yOffset), new Grass((byte)((Grass)kvp.Value).grassType.Value, Game1.random.Next(1, 3)));
            }
        }

    }
}
