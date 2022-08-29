/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using StardewValley;

namespace DecidedlyShared.Constants
{
    public static class Translations
    {
        private static readonly Dictionary<string, string> MapToDisplayName = new()
        {
            { "Farm", "<FarmName> Farm" },
            { "FarmHouse", "<FarmName> Farm House" },
            { "FarmCave", "<FarmName> Farm Cave" },
            { "Town", "Pelican Town" },
            { "JoshHouse", "Alex's Home" },
            { "HaleyHouse", "Haley and Emily's House" },
            { "SamHouse", "Sam's Home" },
            { "Blacksmith", "Clint's Blacksmith" },
            { "ManorHouse", "Lewis's Home" },
            { "SeedShop", "Pierre's Seed Shop (Abigail's Home)" },
            { "Saloon", "Gus's Saloon" },
            { "Trailer", "Penny and Pam's Trailer" },
            { "Hospital", "Harvey's Clinic" },
            { "HarveyRoom", "Harvey's Bedroom" },
            { "Beach", "The Beach" },
            { "ElliottHouse", "Elliott's Home" },
            { "Mountain", "The Mountain" },
            { "ScienceHouse", "Robin's Shop (Maru and Sebastian's Home)" },
            { "SebastianRoom", "Sebastian's Room" },
            { "Tent", "Linus's Tent" },
            { "Forest", "Cindersap Forest" },
            { "WizardHouse", "Wizard Tower" },
            { "AnimalShop", "Marnie's Shop (Shane's Home)" },
            { "LeahHouse", "Leah's Home" },
            { "BusStop", "The Bus Stop" },
            { "Mine", "Mine Entrance" },
            { "Sewer", "Krobus's Sewer" },
            { "BugLand", "Sewer Bug Swamp" },
            { "Desert", "Calico Desert" },
            { "Club", "Mr. Qi's Club" },
            { "SandyHouse", "Sandy's Shop" },
            { "ArchaeologyHouse", "Pelican Town Library and Museum" },
            { "WizardHouseBasement", "Wizard's Basement" },
            { "AdventureGuild", "Adventurer's Guild" },
            { "Woods", "Secret Woods" },
            { "Railroad", "Railroad" },
            { "WitchSwamp", "Witch's Swamp" },
            { "WitchHut", "Witch's Hut" },
            { "WitchWarpCave", "Witch's Warp Room" },
            { "Summit", "The Summit" },
            { "FishShop", "Willy's Shop" },
            { "BathHouse_Entry", "Bath House Entry" },
            { "BathHouse_MensLocker", "Bath House Locker (Male)" },
            { "BathHouse_WomensLocker", "Bath House Locker (Female)" },
            { "BathHouse_Pool", "Bath House Pool/Spa" },
            { "CommunityCenter", "Community Center" },
            { "JojaMart", "JojaMart" },
            { "Greenhouse", "<FarmName> Greenhouse" },
            { "SkullCave", "Skull Cavern Entrance" },
            { "Backwoods", "The Backwoods" },
            { "Tunnel", "Bus Tunnel" },
            { "Trailer_Big", "Penny and Pam's Home" },
            { "Cellar", "Cellar 1" },
            { "Cellar2", "Cellar 2" },
            { "Cellar3", "Cellar 3" },
            { "Cellar4", "Cellar 4" },
            { "BeachNightMarket", "Night Market" },
            { "MermaidHouse", "Mermaid House" },
            { "Submarine", "Night Market Submarine" },
            { "AbandonedJojaMart", "Abandoned JojaMart" },
            { "MovieTheater", "Movie Theater" },
            { "Sunroom", "Sunroom" },
            { "BoatTunnel", "Willy's Boat Storage" },
            { "IslandSouth", "Ginger Island South" },
            { "IslandSouthEast", "Ginger Island South East" },
            { "IslandSouthEastCave", "Ginger Island Suspicious Cave" },
            { "IslandEast", "Ginger Island East" },
            { "IslandWest", "Ginger Island West" },
            { "IslandNorth", "Ginger Island North" },
            { "IslandHut", "Ginger Island Hut" },
            { "IslandWestCave1", "Ginger Island Puzzle Cave" },
            { "IslandNorthCave1", "Ginger Island Mushroom Cave" },
            { "IslandFieldOffice", "Ginger Island Field Office" },
            { "IslandFarmHouse", "Ginger Island Farm House" },
            { "CaptainRoom", "Captain's Room" },
            { "IslandShrine", "Ginger Island Shrine" },
            { "IslandFarmCave", "Ginger Island Gourmand Cave" },
            { "Caldera", "Volcano Caldera" },
            { "LeoTreeHouse", "Leo's Treehouse" },
            { "QiNutRoom", "Mr. Qi's Nut Room" }
        };

        public static string GetMapDisplayName(GameLocation location, string farmName)
        {
            if (MapToDisplayName.ContainsKey(location.NameOrUniqueName))
            {
                string formattedName = MapToDisplayName[location.Name].Replace("<FarmName>", farmName);
                return formattedName;
            }

            return location.Name;
        }
    }
}
