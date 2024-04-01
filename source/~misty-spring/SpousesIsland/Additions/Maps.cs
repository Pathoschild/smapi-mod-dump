/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Extensions;

namespace SpousesIsland.Additions;

internal static class Maps
{
    /// <summary>
    /// Returns a random map and position. Requires Ginger Island Extra Locations
    /// </summary>
    /// <param name="spousename">The name of the character. Used as reference, depending on random result.</param>
    /// <returns></returns>
    internal static string RandomOrDefault(string spousename)
    {
        var enabled = ModEntry.Config.ScheduleRandom;

        if(enabled)
        {
            return RandomFree();
        }
        var result = spousename switch
        {
            "Abigail" => "IslandWest 62 84 2",
            "Alex" => "IslandWest 69 77 2 alex_football/1500 IslandWest 64 83 2",
            "Elliott" => "IslandNorth 19 15 0 \"Strings\\schedules\\Elliott:marriage_loc3\"",
            "Emily" => "IslandWestCave1 3 6 1 \"Strings\\schedules\\Emily:marriage_loc3\"",
            "Haley" => "IslandWest 76 12 2 haley_photo \"Strings\\schedules\\Haley:marriage_loc3\"",
            "Harvey" => "IslandWest 88 14 2 harvey_read \"Strings\\schedules\\Harvey:marriage_loc3\"",
            "Krobus" => "IslandFarmCave 2 6 2",
            "Leah" => "IslandWest 89 72 2 leah_draw \"Strings\\schedules\\Leah:marriage_loc3\"",
            "Maru" => "IslandFieldOffice 7 8 0 \"Strings\\schedules\\Maru:marriage_loc3\"",
            "Penny" => "IslandFieldOffice 2 7 2 \"Strings\\schedules\\Penny:marriage_loc3\"",
            "Sam" => "IslandSouthEast 23 14 2 \"Strings\\schedules\\Sam:marriage_loc3\"",
            "Sebastian" => "IslandNorth 40 23 2 \"Strings\\schedules\\Sebastian:marriage_loc3\"",
            "Shane" => "IslandSouthEastCave 29 6 2 \"Strings\\schedules\\Shane:marriage_loc3\"",
            "Claire" => "IslandWest 87 78 2",
            "Lance" => "IslandSouthEast 21 28 2 \"Characters\\Dialogue\\Lance:marriage_loc3\"",
            "Magnus" => "Caldera 22 23 0 \"Characters\\Dialogue\\Wizard:marriage_loc3\"",
            "Wizard" => "Caldera 22 23 0 \"Characters\\Dialogue\\Wizard:marriage_loc3\"",
            "Olivia" => "IslandNorth 36 73 0 \"Characters\\Dialogue\\Olivia:marriage_loc3\"",
            "Sophia" => "IslandFarmHouse 18 12 Sophia_Read \"Characters\\Dialogue\\Sophia:marriage_loc3\"",
            "Victor" => "IslandFarmHouse 19 5 2 Victor_Book2 \"Characters\\Dialogue\\Victor:marriage_loc3\"",
            _ => RandomFree()
        };
        
        return result;
    }

    private static string[] GetUnlockedMaps()
    {
        //those 6 must be enabled already to come in 
        var maps = new List<string> { "IslandNorth", "IslandSouth", "IslandSouthEast", "IslandSouthEastCave", "IslandWest", "IslandEast", "IslandFarmCave", "IslandShrine", "IslandWestCave1" };

        if (Game1.player.hasOrWillReceiveMail("islandNorthCaveOpened"))
            maps.Add("IslandFieldOffice");

        return maps.ToArray();
    }
    
    private static Point GetRandomMapZone(string where)
    {
        var ran = Game1.random;
        Point position;
        
        if (!ModEntry.ModInfo.LnhIsland)
        {
            position = where switch
            {
                "IslandFarmHouse" => new Point(ran.Next(5, 14), ran.Next(2, 28)),
                "IslandWest" => ran.Next(0, 4) switch
                {
                    0 => new Point(ran.Next(73, 90), ran.Next(12, 18)),
                    1 => new Point(ran.Next(56, 104), ran.Next(79, 87)),
                    2 => new Point(ran.Next(34, 52), ran.Next(69, 74)),
                    3 => new Point(ran.Next(25, 34), ran.Next(60, 70)),
                    _ => new Point(0, 0)
                },
                "IslandSouth" => new Point(ran.Next(9, 31), ran.Next(12, 35)),
                "IslandSouthEast" => new Point(ran.Next(12, 28), ran.Next(16, 28)),
                "IslandEast" => new Point(ran.Next(12, 30), ran.Next(30, 45)),
                "IslandNorth" => new Point(ran.Next(25, 61), ran.Next(74, 83)),
                "IslandFieldOffice" => new Point(ran.Next(2, 8), ran.Next(4, 9)),
                "IslandFarmCave" => new Point(ran.Next(4, 6), ran.Next(5, 11)),
                "CaptainRoom" => new Point(ran.Next(1, 5), ran.Next(5, 7)),
                "IslandWestCave1" => new Point(ran.Next(2, 11), ran.Next(3, 9)),
                "IslandSouthEastCave" => new Point(ran.Next(14, 29), ran.Next(8, 11)),
                "IslandShrine" => new Point(ran.Next(19, 28), ran.Next(24, 31)),
                _ => Game1.getLocationFromName(where).getRandomTile().ToPoint()
            };
        }
        else
        {
            position = where switch
            {
                "IslandFarmHouse" => new Point(ran.Next(5, 14), ran.Next(2, 28)),
                "IslandWest" => ran.Next(0, 4) switch
                {
                    0 => new Point(ran.Next(73, 90), ran.Next(12, 18)),
                    1 => new Point(ran.Next(56, 104), ran.Next(79, 87)),
                    2 => new Point(ran.Next(34, 52), ran.Next(69, 74)),
                    3 => new Point(ran.Next(25, 34), ran.Next(60, 70)),
                    _ => new Point(0, 0)
                },
                "IslandSouth" => new Point(ran.Next(9, 31), ran.Next(12, 35)),
                "IslandSouthEast" => new Point(ran.Next(12, 28), ran.Next(16, 28)),
                "IslandEast" => new Point(ran.Next(12, 30), ran.Next(30, 45)),
                "IslandNorth" => new Point(ran.Next(25, 61), ran.Next(74, 83)),
                "IslandFieldOffice" => new Point(ran.Next(2, 8), ran.Next(4, 9)),
                "IslandFarmCave" => new Point(ran.Next(4, 6), ran.Next(5, 11)),
                "CaptainRoom" => new Point(ran.Next(1, 5), ran.Next(5, 7)),
                "IslandWestCave1" => new Point(ran.Next(2, 11), ran.Next(3, 9)),
                "IslandSouthEastCave" => new Point(ran.Next(14, 29), ran.Next(8, 11)),
                _ => new Point(0, 0)
            };
        }

        return position;
    }

    internal static Point RandomPoint(string where)
    {
        var result = Vector2.Zero;
        var place = Game1.getLocationFromName(where);

        for (var i = 0; i < 30; i++)
        {
            result = place.getRandomTile();

            if (place.IsTileOccupiedBy(result) == false)
                break;
        }

        return result.ToPoint();
    }
    
    internal static Point RandomPoint(GameLocation place)
    {
        var result = Vector2.Zero;

        for (var i = 0; i < 30; i++)
        {
            result = place.getRandomTile();

            if (!place.IsTileOccupiedBy(result))
                break;
        }

        return result.ToPoint();
    }

    internal static string RandomFree()
    {
        var ran = Game1.random;
        var avaiableMaps = GetUnlockedMaps();
        
        var mapName = Game1.random.ChooseFrom(avaiableMaps);
        var position = GetRandomMapZone(mapName);
        var location = Game1.getLocationFromName(mapName);

        if (location.IsTileOccupiedBy(position.ToVector2()))
        {
            for(var tries = 0; tries < 30; tries++)
            {
                position = GetRandomMapZone(mapName);
                if (location.IsTileOccupiedBy(position.ToVector2())) 
                    continue;
                
                ModEntry.Mon.Log($"New position: {position.X} {position.Y}");
                break;
            }
        }

        return $"{mapName} {position.X} {position.Y} {ran.Next(0, 4)}";
    }
}