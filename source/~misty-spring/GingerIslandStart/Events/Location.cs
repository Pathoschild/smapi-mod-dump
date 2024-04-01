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
using StardewModdingAPI.Events;
using StardewValley;
using static GingerIslandStart.Additions.IslandChanges;

namespace GingerIslandStart.Events;

public static class Location
{
    private static string Id => ModEntry.Id;
    internal static bool NeedsEdit { get; set; }
    private static Point Start => ModEntry.StartingPoint;

    internal static void WarpToIsland()
    {
        Game1.warpFarmer("IslandSouth",Start.X,Start.Y,false);
        ModEntry.NeedsWarp = false;
        Game1.player.lastSleepLocation.Value = "IslandSouth";
        Game1.player.lastSleepPoint.Value = new Point(Start.X, Start.Y);
        Game1.updateMusic();
    }

    internal static void CheckWarpChanges()
    {
        //if not an island save
        if (!Game1.player.modData.TryGetValue(ModEntry.NameInData, out var warp))
            return;
        
        //if has boat OR island house
        if(Game1.player.hasOrWillReceiveMail("willyBoatFixed"))
            return;

        if (Game1.player.hasOrWillReceiveMail("Island_UpgradeHouse") &&
            Game1.player.lastSleepLocation.Value.Equals("IslandFarmHouse", StringComparison.OrdinalIgnoreCase))
            return;
        
        ModEntry.NeedsWarp = bool.Parse(warp);
    }

    internal static void OnWarp(object sender, WarpedEventArgs e)
    {
        if (!Game1.player.modData.ContainsKey(ModEntry.NameInData))
            return;

        if(!e.NewLocation.InIslandContext())
            return;

        if (e.NewLocation.Name != "VolcanoDungeon0") 
            return;
        
        if (Game1.player.hasOrWillReceiveMail("Island_Resort"))
            return;

        e.NewLocation.setMapTile(26,41,Game1.player.canUnderstandDwarves ? 95 : 77,"Buildings",$"OpenShop {Id}_Dwarf");
        e.NewLocation.setMapTile(26,40,61,"Front", null);
            
        if(!Game1.player.canUnderstandDwarves)
            return;
            
        var sourceRect = new Rectangle(
            208,
            48 + 16,
            16,
            16);
            
        var traderBtm = new TemporaryAnimatedSprite("Maps/Mines/volcano_dungeon", sourceRect, new Vector2(26, 41) * 64, false, 0f, Color.White)
        {
            layerDepth = 0.03f,
            scale = 4f
        };
        e.NewLocation.TemporarySprites.Add(traderBtm);
    }

    public static void PropertyChanges(object sender, AssetReadyEventArgs e)
    {
        if(!NeedsEdit)
            return;
        
        Game1.delayedActions.Add(new DelayedAction(1000, AddProperties));
        NeedsEdit = false;
    }

    /// <summary>
    /// Finds the nearest open tile.
    /// </summary>
    /// <param name="location">Location to use for checks.</param>
    /// <param name="target">Initial position.</param>
    /// <returns>A tile that is unoccupied.</returns>
    internal static Vector2 NearestOpenTile(GameLocation location, Vector2 target)
    {
        var position = new Vector2();
        for (var i = 1; i < 30; i++)
        {
            var toLeft = new Vector2(target.X - i, target.Y);
            if (!location.IsTileOccupiedBy(toLeft))
            {
                position = toLeft;
                break;
            }
            
            var toRight = new Vector2(target.X + i, target.Y);
            if (!location.IsTileOccupiedBy(toRight))
            {
                position = toRight;
                break;
            }
            
            var toUp = new Vector2(target.X, target.Y - i);
            if (!location.IsTileOccupiedBy(toUp))
            {
                position = toUp;
                break;
            }
            
            var toDown = new Vector2(target.X, target.Y + i);
            if (!location.IsTileOccupiedBy(toDown))
            {
                position = toDown;
                break;
            }

            var upperLeft= new Vector2(target.X - i, target.Y - 1);
            if (!location.IsTileOccupiedBy(upperLeft))
            {
                position = upperLeft;
                break;
            }
            
            var lowerLeft= new Vector2(target.X - i, target.Y + 1);
            if (!location.IsTileOccupiedBy(lowerLeft))
            {
                position = lowerLeft;
                break;
            }
            
            var upperRight= new Vector2(target.X + i, target.Y - 1);
            if (!location.IsTileOccupiedBy(upperRight))
            {
                position = upperRight;
                break;
            }
            
            var lowerRight= new Vector2(target.X + i, target.Y + 1);
            if (!location.IsTileOccupiedBy(lowerRight))
            {
                position = lowerRight;
                break;
            }
        }

        return position;
    }
}