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
using StardewValley.Objects;
using StardewValley.Pathfinding;

namespace SpousesIsland.Additions;

internal static class Beds
{
    //stuff to make characters path to sleep
    internal static Point GetBedSpot(BedFurniture.BedType bedType = BedFurniture.BedType.Any)
    {
        return GetBed(bedType)?.GetBedSpot() ?? new Point(-1000, -1000);
    }
        
    private static BedFurniture GetBed(BedFurniture.BedType bedType = BedFurniture.BedType.Any, int index = 0)
    {
        //Furniture f in IslandFarmHouse.Object
        foreach (var f in Game1.getLocationFromName("IslandFarmHouse").furniture)
        {
            if (f is not BedFurniture bed) 
                continue;
                
            if (bedType != BedFurniture.BedType.Any && bed.bedType != bedType) 
                continue;
                
            if (index == 0)
                return bed;
                
            index--;
        }
        return null;
    }
    
    private static void SleepEndFunction(Character c, GameLocation location)
    {
        if (c is not NPC npc)
            return;
        
        c.doEmote(Character.sleepEmote);
        
        if (DataLoader.AnimationDescriptions(Game1.content).ContainsKey(npc.Name.ToLower() + "_sleep")) 
            npc.playSleepingAnimation();
        
        foreach (var furniture in location.furniture)
        {
            if (furniture is not BedFurniture bedFurniture)
                continue;
            
            var boundingBox = bedFurniture.GetBoundingBoxAt((int)bedFurniture.TileLocation.X, (int)bedFurniture.TileLocation.Y);
            
            if(!boundingBox.Intersects(npc.GetBoundingBox())) 
                continue;
            
            bedFurniture.ReserveForNPC();
            break;
        }
    }

    /// <summary>
    /// Finds a suitable spouse bed.
    /// </summary>
    /// <returns>The bed's tile location.</returns>
    private static Point GetSpouseBedSpot()
    {
        var bed = GetSpouseBed();

        if (bed == null)
            return new Point(-1000, -1000);
        
        var bedSpot = bed.GetBedSpot();
        bedSpot.X++;
        return bedSpot;
    }

    private static BedFurniture GetSpouseBed() => GetBed(BedFurniture.BedType.Double);
    
    /// <summary>
    /// Pathfinds to a double bed and does sleep functions.
    /// </summary>
    /// <param name="c">Character to path</param>
    /// <param name="location">Location to use</param>
    internal static void MakeSpouseSleep(NPC c, GameLocation location)
    {
        //if not host, not time yet, or moving
        if (!Game1.IsMasterGame || Game1.timeOfDay < 2200 || c.isMoving()) //(Game1.timeOfDay != 2200 && (c.controller != null || Game1.timeOfDay % 100 % 30 != 0))
            return;
        
        //if already pathed
        if(c.TilePoint == GetSpouseBedSpot() || c.isSleeping.Value)
            return;
        
        //stop npc first
        c.Halt();
        c.followSchedule = false;
        c.ignoreScheduleToday = true;

        var bed = GetSpouseBedSpot();
        
        //if no double bed, check for a single one
        if (bed == new Point(-1000, -1000))
            bed = GetBed(BedFurniture.BedType.Single).GetBedSpot();
        
        c.controller = new PathFindController(c, location, bed, 0, SleepEndFunction);
    }

    /// <summary>
    /// Pathfinds to a child bed and does sleep functions.
    /// </summary>
    /// <param name="c">Character to path</param>
    /// <param name="location">Location to use</param>
    internal static void MakeKidSleep(NPC c, GameLocation location)
    {
        if (!Game1.IsMasterGame || Game1.timeOfDay < 2200 || c.isMoving())
            return;
        
        if(c.TilePoint == GetKidBedSpot() || c.isSleeping.Value)
            return;
        
        c.Halt();
        c.followSchedule = false;
        c.ignoreScheduleToday = true;
        
        c.controller = new PathFindController(c, location, GetKidBedSpot(), 2, SleepEndFunction);
    }

    private static Point GetKidBedSpot()
    {
        var bedSpot = GetKidBed().GetBedSpot();
        bedSpot.X++;
        return bedSpot;
    }

    private static BedFurniture GetKidBed()
    {
        return GetBed(BedFurniture.BedType.Child);
    }

    internal static bool HasAnyKidBeds()
    {
        var bed = GetBed(BedFurniture.BedType.Child);
        return bed is not null;
    }

    internal static bool HasCrib(Farmer player)
    {
        var where = Utility.getHomeOfFarmer(player);

        //0 means no crib. so, we return whether crib isn't 0 (doing it this way in case any mod changes crib int to something other than 1)
        return where.cribStyle.Value != 0;
    }
}