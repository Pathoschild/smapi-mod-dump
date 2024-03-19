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

internal static class Day
{
    internal static void OnStart(object sender, DayStartedEventArgs e)
    {
        CheckPlayer();

        if (!IsFromTicket)
        {
            RandomizedInt = Game1.random.Next(1, 101);
            IslandToday = Config.CustomChance >= RandomizedInt;
        }
        
        //if no island or not unlocked, do nothing (if unlocked but no island, make devan invisible)
        if (!IslandToday || !Unlocked)
        {
            if (!Unlocked)
                return;

            var devan = Game1.getCharacterFromName("DevanSpring");
            if (devan == null)
                return;

            devan.IsInvisible = true;
            devan.daysUntilNotInvisible = 1;
            return;
        }

        foreach (var character in ValidSpouses)
        {
            #if DEBUG
            Mon.Log($"Checking {character}...", LogLevel.Debug);
            #endif
            
            if(Status is not null && Status.Any() && !Status.ContainsKey(character))
                continue;
            
            var spouse = Game1.getCharacterFromName(character);
            if (spouse is null)
                continue;

            Schedule.Island(spouse);
        }

        if (Children?.Count == 0 || Children == null)
            return;

        Schedule.Babysitter();

        if (!ModInfo.LittleNpcs)
            return;

        if (!Beds.HasAnyKidBeds() && Config.UseFurnitureBed)
        {
            Mon.Log("There's no child beds in island farmhouse. Farmer's kids won't visit.", LogLevel.Warn);
            return;
        }

        foreach (var child in Children)
        {
            var kid = Game1.getCharacterFromName(child.Name);
            
            if (kid is null)
                continue;
            
            Schedule.Island(kid);
        }
    }

    internal static void OnTimeChange(object sender, TimeChangedEventArgs e)
    {
        //avoid running unnecessary code
        if (IslandToday == false)
            return;

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

                if (spouse.isMovingOnPathFindPath.Value || spouse.isMoving())
                    continue;

                spouse.temporaryController = new PathFindController(spouse, spouse.currentLocation, Schedule.RandomSpot(spouse),
                    Game1.random.Next(0, 3));
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

    internal static void OnEnd(object sender, DayEndingEventArgs e)
    {
        if (Status.Any())
        {
            RandomizedInt = 0;
            IslandToday = true;
            IsFromTicket = true;
        }
        else
        {
            //get new %
            PreviousDayRandom = RandomizedInt;
            RandomizedInt = Game1.random.Next(1, 101);
            IslandToday = Config.CustomChance >= RandomizedInt;
            IsFromTicket = false;
        }

        var hadYesterday = Config.CustomChance >= PreviousDayRandom;
        //
    }
}