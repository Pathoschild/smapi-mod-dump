using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley.TerrainFeatures;
using StardewValley.Objects;

namespace MTN_Test
{
    public class SaveGameTest
    {
        //This version of loadDataToLocations reduces the redundancies and double/triple loops that occur.
        //This should decrease the runtime.
        public static void loadDataToLocations(List<GameLocation> gamelocations) {
            foreach (GameLocation i in gamelocations) {
                GameLocation generalLocation = Game1.getLocationFromName(i.name);
                //FarmHouse / Cabins
                if (i is FarmHouse) {
                    FarmHouse farmLocation = (generalLocation as FarmHouse);
                    farmLocation.setMapForUpgradeLevel((i as FarmHouse).upgradeLevel);
                    farmLocation.wallPaper.Set((i as FarmHouse).wallPaper);
                    farmLocation.floor.Set((i as FarmHouse).floor);
                    farmLocation.furniture.Set((i as FarmHouse).furniture);
                    farmLocation.fireplaceOn.Value = (i as FarmHouse).fireplaceOn;
                    farmLocation.fridge.Value = (i as FarmHouse).fridge;
                    farmLocation.farmerNumberOfOwner = (i as FarmHouse).farmerNumberOfOwner;
                    farmLocation.resetForPlayerEntry();
                    foreach (Furniture furniture in (farmLocation as FarmHouse).furniture) {
                        furniture.updateDrawPosition();
                    }
                    farmLocation.lastTouchActionLocation = Game1.player.getTileLocation();
                //Farms
                } else if (i is Farm) {
                    Farm realLocation = (generalLocation as Farm);
                    foreach (Building building in ((Farm)i).buildings) {
                        building.load();
                    }
                    realLocation.buildings.Set(((Farm)i).buildings);
                    foreach (FarmAnimal farmAnimal in ((Farm)i).animals.Values) {
                        farmAnimal.reload(null);
                    }
                    realLocation.animals.MoveFrom((i as Farm).animals);
                    realLocation.piecesOfHay.Value = (i as Farm).piecesOfHay;
                    realLocation.resourceClumps.Set((i as Farm).resourceClumps);
                    realLocation.hasSeenGrandpaNote = (i as Farm).hasSeenGrandpaNote;
                    realLocation.grandpaScore = (i as Farm).grandpaScore;
                } else if (i is Beach) {
                    (generalLocation as Beach).bridgeFixed.Value = (i as Beach).bridgeFixed;
                } else if (i is Woods) {
                    (generalLocation as Woods).stumps.MoveFrom((i as Woods).stumps);
                    (generalLocation as Woods).hasUnlockedStatue.Value = (i as Woods).hasUnlockedStatue.Value;
                } else if (i is CommunityCenter) {
                    (generalLocation as CommunityCenter).areasComplete.Set((i as CommunityCenter).areasComplete);
                } else if (i is SeedShop) {
                    (generalLocation as SeedShop).itemsFromPlayerToSell.MoveFrom((i as SeedShop).itemsFromPlayerToSell);
                    (generalLocation as SeedShop).itemsToStartSellingTomorrow.MoveFrom((i as SeedShop).itemsToStartSellingTomorrow);
                } else if (i is Town) {
                    (generalLocation as Town).daysUntilCommunityUpgrade.Value = (i as Town).daysUntilCommunityUpgrade;
                } else if (i is Forest) { 
                    Forest forestLocation = (generalLocation as Forest);
                    if (Game1.dayOfMonth % 7 % 5 == 0) {
                        forestLocation.travelingMerchantDay = true;
                        forestLocation.travelingMerchantBounds.Clear();
                        forestLocation.travelingMerchantBounds.Add(new Rectangle(1472, 640, 492, 112));
                        forestLocation.travelingMerchantBounds.Add(new Rectangle(1652, 744, 76, 48));
                        forestLocation.travelingMerchantBounds.Add(new Rectangle(1812, 744, 104, 48));
                        foreach (Rectangle r in forestLocation.travelingMerchantBounds) {
                            Utility.clearObjectsInArea(r, forestLocation);
                        }
                    }
                    forestLocation.log = (i as Forest).log;
                }
                //General Locations (GameLocations)

                foreach (NPC c in i.characters) {
                    if (c.DefaultPosition.Equals(Vector2.Zero)) {
                        c.Position = c.DefaultPosition;
                    }
                    c.currentLocation = generalLocation;
                    if (c.datingFarmer) {
                        Friendship friendship = Game1.player.friendshipData[c.Name];
                        if (!friendship.IsDating()) {
                            friendship.Status = FriendshipStatus.Dating;
                        }
                        c.datingFarmer = false;
                    }
                }
                foreach (TerrainFeature terrainFeature in i.terrainFeatures.Values) {
                    terrainFeature.loadSprite();
                }
                foreach (KeyValuePair<Vector2, StardewValley.Object> v in i.objects.Pairs) {
                    v.Value.initializeLightSource(v.Key, false);
                    v.Value.reloadSprite();
                }
                generalLocation.characters.Set(i.characters);
                generalLocation.netObjects.Set(i.netObjects.Pairs);
                generalLocation.numberOfSpawnedObjectsOnMap = i.numberOfSpawnedObjectsOnMap;
                generalLocation.terrainFeatures.Set(i.terrainFeatures.Pairs);
                generalLocation.largeTerrainFeatures.Set(i.largeTerrainFeatures);
                for (int c = 0; c <= generalLocation.characters.Count; c++) {
                    generalLocation.characters[c].reloadSprite();
                }
            }
            //////////////////////////
            Game1.player.currentLocation = Utility.getHomeOfFarmer(Game1.player);
        }
    }
}
