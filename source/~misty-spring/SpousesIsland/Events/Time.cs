/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using System;
using System.Linq;
using SpousesIsland.Additions;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Pathfinding;
using static SpousesIsland.ModEntry;

namespace SpousesIsland.Events;

public static class Time
{
    internal static void OnChange(object sender, TimeChangedEventArgs e)
    {
        //avoid running unnecessary code
        if (IslandToday == false)
            return;

        //in the future, make this a general calculation of "if directions to new location (count) is bigger than x, set addedspeed to this. if bigger than (e.g count*2), set to this + 0.2. for a little more ease of travelling?? perhaps.
        if (e.NewTime < 800)
        {
            foreach (var character in ValidSpouses)
            {
#if DEBUG
                Mon.Log($"Checking {character} speed...", LogLevel.Info);
#endif
                if (Status is not null && Status.Any() && !Status.ContainsKey(character))
                    continue;
                
                var spouse = Game1.getCharacterFromName(character);
                
                if (spouse is null)
                    continue;
                
                if(spouse.currentLocation.Name == "Town")
                    spouse.addedSpeed = 0.5f;
                else
                    spouse.addedSpeed = 0.4f;
            }

            return;
        }
        
        if (e.NewTime >= 2200)
        {
            var islandFarmHouse = Game1.getLocationFromName("IslandFarmHouse");

            //then, path them to bed
            foreach (var c in islandFarmHouse.characters)
            {
                if (c.isMarried() || c.isRoommate())
                {
                    Mon.Log($"Pathing {c.Name} to bed in {islandFarmHouse.Name}...");
                    try
                    {
                        Beds.MakeSpouseSleep(c, islandFarmHouse);
                    }
                    catch (Exception ex)
                    {
                        Mon.Log($"An error ocurred while pathing {c.Name} to bed: {ex}");
                    }
                }
                else if (ModInfo.LittleNpcs)
                {
                    Mon.Log($"Pathing {c.Name} to kid bed in {islandFarmHouse.Name}...");
                    try
                    {
                        Beds.MakeKidSleep(c, islandFarmHouse);
                    }
                    catch (Exception ex)
                    {
                        Mon.Log($"An error ocurred while pathing {c.Name} to bed: {ex}");
                    }
                }
            }

            return;
        }
        
        if (e.NewTime % 20 == 0) //200
        {
            foreach (var character in ValidSpouses)
            {
                if(Status is not null && Status.Any() && !Status.ContainsKey(character))
                    continue;
                
                var spouse = Game1.getCharacterFromName(character);
                if (spouse is null)
                    continue;

                if (spouse.isMoving() || FreeNext20Minutes(spouse) == false || spouse.controller != null || spouse.temporaryController != null) //spouse.isMovingOnPathFindPath.Value || 
                    continue;

                #if DEBUG
                Mon.Log($"Moving companion {character}...", LogLevel.Debug);
                #endif

                spouse.temporaryController = null;
                spouse.controller = new PathFindController(spouse, spouse.currentLocation, Schedule.RandomSpot(spouse), Game1.random.Next(0, 3))
                    {
                        //endBehaviorFunction = FollowSchedule,
                        NPCSchedule = true
                    };
            }
        }

        if (!DevanExists) 
            return;

        var devan = Utility.fuzzyCharacterSearch("DevanSpring");
        if(devan.isMoving() || devan.doingEndOfRouteAnimation.Value)
            return;
        
        switch (e.NewTime)
        {
            //if >910 && <930, walk to baby
            case > 830 and < 900:
                var age = int.MaxValue;
                var youngest = Children[0];
                foreach (var child in Children)
                {
                    if (child.daysOld.Value >= age)
                        continue;
                    
                    age = child.daysOld.Value;
                    youngest = child;
                }
                Schedule.WalkTo(devan, youngest);
                break;
            //walk to kid(s). use random to choose
            case > 1300 and < 1400:
                var whichkid = Game1.random.Next(Children.Count);
                Schedule.WalkTo(devan, Children[whichkid]);
                break;
            default:
                if(e.NewTime % 200 == 0)
                    Schedule.Wander(devan, Game1.random.Next(3,6));
                break;
        }
    }

    private static void FollowSchedule(Character c, GameLocation location)
    {
        if (c is not NPC npc)
            return;
        
        npc.followSchedule = true;
    }

    private static bool FreeNext20Minutes(NPC spouse)
    {
        var time = Game1.timeOfDay;
        var schedule = spouse.Schedule;
        if (schedule == null)
            return false;

        foreach (var data in schedule)
        {
#if DEBUG
                Mon.Log($"Checking schedule point: {data.Key} at {time}", LogLevel.Debug);
#endif
            //if already passed, ignore
            if (data.Key <= time) continue;
            
            //if next one is more than 20min apart, break (→ returns true)
            if ((data.Key - time) > 50) break;
            
#if DEBUG
                Mon.Log($"Time: {data.Value.time}, Endofroutebehavior: {data.Value.endOfRouteBehavior}");
#endif

            return false;
        }

        return true;
    }
}