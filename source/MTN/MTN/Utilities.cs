using MTN.FarmInfo;
using StardewValley;
using StardewValley.Locations;
using System.Collections.Generic;
using System.Linq;
using SObject = StardewValley.Object;
using xTile;

namespace MTN
{
    public static class Utilities
    {

        public static void additionalMapLoad()
        {
            Map map;
            string mapAssetKey;

            if (Game1.multiplayerMode != 0) return;

            if (Memory.isCustomFarmLoaded && Memory.loadedFarm.additionalMaps != null)
            {
                foreach (additionalMap<GameLocation> m in Memory.loadedFarm.additionalMaps)
                {
                    object newMap;
                    object temp;

                    if (m.type == fileType.raw)
                    {
                        map = Memory.loadedFarm.contentpack.LoadAsset<Map>(m.FileName + ".tbin");
                    }

                    mapAssetKey = Memory.loadedFarm.contentpack.GetActualAssetKey(m.FileName + ((m.type == fileType.raw) ? ".tbin" : ".xnb"));

                    switch (m.mapType)
                    {
                        case "Farm":
                            newMap = new Farm(mapAssetKey, m.Location);
                            Game1.locations.Add((Farm)newMap);
                            Memory.farmMaps.Add(new additionalMap<Farm>(m, Game1.locations.Last() as Farm));
                            break;
                        case "FarmCave":
                            newMap = new FarmCave(mapAssetKey, m.Location);
                            Game1.locations.Add((FarmCave)newMap);
                            break;
                        case "GameLocation":
                            newMap = new GameLocation(mapAssetKey, m.Location);
                            Game1.locations.Add((GameLocation)newMap);
                            break;
                        default:
                            newMap = new GameLocation(mapAssetKey, m.Location);
                            Game1.locations.Add((GameLocation)newMap);
                            break;
                    }
                }
            }
        }

        public static List<SObject> getPurchaseAnimalStock(string locationName) {
            List<SObject> list = new List<SObject>();
            SObject o;
            BuildableGameLocation location;
            GameLocation searchResults = Game1.getLocationFromName(locationName);

            if (searchResults is BuildableGameLocation) {
                location = (BuildableGameLocation)searchResults;
            } else {
                return list;
            }

            o = new SObject(100, 1, false, 400, 0) {
                Name = "Chicken",
                Type = ((location.isBuildingConstructed("Coop") || location.isBuildingConstructed("Deluxe Coop") || location.isBuildingConstructed("Big Coop")) ? null : Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5926")),
                displayName = Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5922")
            };
            list.Add(o);
            o = new SObject(100, 1, false, 750, 0) {
                Name = "Dairy Cow",
                Type = ((location.isBuildingConstructed("Barn") || location.isBuildingConstructed("Deluxe Barn") || location.isBuildingConstructed("Big Barn")) ? null : Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5931")),
                displayName = Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5927")
            };
            list.Add(o);
            o = new SObject(100, 1, false, 2000, 0) {
                Name = "Goat",
                Type = ((location.isBuildingConstructed("Big Barn") || location.isBuildingConstructed("Deluxe Barn")) ? null : Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5936")),
                displayName = Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5933")
            };
            list.Add(o);
            o = new SObject(100, 1, false, 2000, 0) {
                Name = "Duck",
                Type = ((location.isBuildingConstructed("Big Coop") || location.isBuildingConstructed("Deluxe Coop")) ? null : Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5940")),
                displayName = Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5937")
            };
            list.Add(o);
            o = new SObject(100, 1, false, 4000, 0) {
                Name = "Sheep",
                Type = (location.isBuildingConstructed("Deluxe Barn") ? null : Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5944")),
                displayName = Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5942")
            };
            list.Add(o);
            o = new SObject(100, 1, false, 4000, 0) {
                Name = "Rabbit",
                Type = (location.isBuildingConstructed("Deluxe Coop") ? null : Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5947")),
                displayName = Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5945")
            };
            list.Add(o);
            o = new SObject(100, 1, false, 8000, 0) {
                Name = "Pig",
                Type = (location.isBuildingConstructed("Deluxe Barn") ? null : Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5950")),
                displayName = Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5948")
            };
            list.Add(o);
            return list;
        }
    }

    
}
