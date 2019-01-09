using MTN2.MapData;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN2.Compatibility {
    public class resourceSpawns : spawn<ResourceClump> {
        public int width = 2;
        public int height = 2;

        public static void Convert(CustomFarm farm, CustomFarmVer1 oldFarm) {
            LargeDebris largeDebris = new LargeDebris();
            largeDebris.ResourceList = new List<Spawn>();

            foreach (resourceSpawns rs in oldFarm.Resource) {
                Spawn item = new Spawn();
                if (rs.itemId == 0) {
                    if (rs.itemName == "Stump") {
                        item.ItemId = 600;
                    } else if (rs.itemName == "Log") {
                        item.ItemId = 602;
                    } else if (rs.itemName == "Boulder") {
                        item.ItemId = 672;
                    } else {
                        continue;
                    }
                } else if (rs.itemId == 600 || rs.itemId == 602 || rs.itemId == 672) {
                    item.ItemId = rs.itemId;
                } else {
                    continue;
                }

                item.Seasons = rs.SeasonsToSpawn;
                item.Boundary = rs.SpawnType;
                item.AreaBinding = rs.area;
                item.TileBinding = rs.TileIndex;
                item.Chance = rs.chance;
                item.RainModifier = new Modifier(rs.rainAddition, rs.rainMultipler);
                item.NewMonthModifier = new Modifier(rs.newMonthAddition, rs.newMonthMultipler);
                item.NewYearModifier = new Modifier(rs.newYearAddition, rs.newYearMultipler);
                item.AmountMin = rs.minimumAmount;
                item.AmountMax = rs.maximumAmount;
                item.CooldownMin = rs.minCooldown;
                item.CooldownMax = rs.maxCooldown;
                item.DaysLeft = rs.daysTilNextSpawn;

                largeDebris.ResourceList.Add(item);
            }
            farm.ResourceClumps = largeDebris;
        }

        public void checkIntegrity() {
            if (itemId > 0) return;
            if (itemId < 0) {
                itemId = 0;
                return;
            }
            switch (itemName) {
                case "Stump":
                    itemId = 600;
                    break;
                case "Log":
                    itemId = 602;
                    break;
                case "Boulder":
                    itemId = 672;
                    break;
            }
            if (minimumAmount < 1) minimumAmount = 1;
            if (maximumAmount < 0) maximumAmount = 0;
            if (minCooldown < 1) minCooldown = 1;
            if (maxCooldown < 0) maxCooldown = 0;
            return;
        }
    }


}
