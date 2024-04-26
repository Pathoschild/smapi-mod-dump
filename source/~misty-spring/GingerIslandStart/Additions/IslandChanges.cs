/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Locations;
using StardewValley.Objects;
using Object = StardewValley.Object;

namespace GingerIslandStart.Additions;

public static class IslandChanges
{
    private static Point Start => ModEntry.StartingPoint;
    private static string Id => ModEntry.Id;
    private static double Difficulty => ModEntry.GeneralDifficultyMultiplier;
    
    internal static void AddProperties()
    {
        var where = Utility.fuzzyLocationSearch("IslandSouth");

        ((IslandSouth)where).resortOpenToday.Value = false;
        
        //where.setMapTileIndex(20, 44, 32, "Buildings");
        //where.setMapTileIndex(18, 43, 32, "Buildings");
        //where.setMapTileIndex(22, 42, 32, "Buildings");
        
        //AddAction(where, 20, 44, $"{Id}_Hull");
        //AddAction(where, 18, 43, $"{Id}_Anchor");
        //AddAction(where, 22, 42, $"{Id}_Batteries");
        
        where.removeTileProperty(19,43,"Buildings","Action");
    }

    private static void AddAction(GameLocation where, int x, int y, string whichAction)
    {
        var tile = where.map.GetLayer("Buildings")?.Tiles[x, y];
        tile?.Properties.Add("Action", whichAction);
    }

    internal static void ChangeGiftLocation()
    {
        var house = Utility.getHomeOfFarmer(Game1.player);
        var gift = house.getObjectAtTile(3, 8);
        
        if (gift is not Chest)
        {
            foreach (var obj in house.Objects.Values)
            {
                if (obj is not Chest c)
                    continue;
                
                if(!c.giftbox.Value)
                    continue;

                gift = c;
                break;
            }
        }

        if (gift is null)
        {
#if DEBUG
            ModEntry.Mon.Log("Gift is null.", StardewModdingAPI.LogLevel.Warn);
#endif
            return;
        }

        Game1.player.mailReceived.Add(ModEntry.GiftWarpId);

        (gift as Chest)?.addItem(ItemRegistry.Create("(T)BambooPole"));

        //random sword, difficulty-dependant
        var sword = Difficulty switch
        {
            0.5 => Game1.random.ChooseFrom(new[] { 14, 5, 7 }),
            2 => Game1.random.ChooseFrom(new[] { 24, 43, 1 }),
            _ => Game1.random.ChooseFrom(new[] { 49, 3 })
        };
        
        (gift as Chest)?.addItem(ItemRegistry.Create($"(W){sword}"));

        var boots  = Difficulty switch
        {
            0.5 => 504,
            2 => 508,
            _ => 507
        };
        //(gift as Chest)?.addItem(ItemRegistry.Create($"(B){boots}"));
        
        //if no boots, give new ones
        if (Game1.player.boots.Value is null)
        {
            Game1.player.boots.Set(new Boots($"{boots}"));
        }
        
        var positionInHouse = gift.TileLocation;
        var island = Utility.fuzzyLocationSearch("IslandSouth");
        var tile = new Vector2(Start.X + 1, Start.Y);
        if (island.IsTileOccupiedBy(tile))
            tile = Events.Location.NearestOpenTile(island, tile);
        
        island.Objects.Add(tile, gift);

        house.removeObject(positionInHouse,false);
        
        Game1.player.addQuest("6");
        Game1.dayTimeMoneyBox.PingQuestLog();
    }

    internal static void CheckBarn(bool build)
    {
        ModEntry.Mon.Log("Checking Island barn....");
        
        if (!build)
            return;

        var west = Game1.getLocationFromName("IslandWest");
        var tile = new Vector2(95,45);

        west.AddDefaultBuilding($"{Id}_OstrichBarn", tile);
        
        //add feeder
        var feeder = ItemRegistry.Create("(BC)99");
        if (feeder is Object obj)
        {
            obj.destroyOvernight = false;
            obj.CanBeGrabbed = false;
            obj.Fragility = 2;
        
            var building = west.getBuildingByType($"{Id}_OstrichBarn");
            building.GetIndoors().Objects.Add(new Vector2(3, 3), obj);
        }

        Game1.addMail($"{Id}_BuiltCoop",true,true);

        var points = new Rectangle(95, 45, 4, 6);
        foreach (var point in points.GetPoints())
        {
            west.removeTile(point.X, point.Y, "Buildings");
            west.removeTile(point.X, point.Y, "Front");
        }
    }

    /*
    public static void SetTrash()
    {
        var islandSouth = Game1.getLocationFromName("IslandSouth");
        islandSouth.setMapTileIndex(20,33,1478,"Buildings");
        islandSouth.setMapTileIndex(21,33,1479,"Buildings");
        islandSouth.setMapTileIndex(22,33,282,"Buildings",1);
        islandSouth.setMapTileIndex(18,33,1498,"Buildings",1);
        islandSouth.setMapTileIndex(20,34,1523,"Buildings",1);
        islandSouth.setMapTileIndex(21,35,1524,"Buildings",1);
        islandSouth.setMapTileIndex(22,35,1548,"Buildings",1);
        islandSouth.setMapTileIndex(20,36,1499,"Buildings",1);
    }*/
}