/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Bouhm/stardew-valley-mods
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using NPCMapLocations.Framework.Models;
using StardewModdingAPI;
using StardewValley.Menus;

namespace NPCMapLocations.Framework
{
    /*
    Static class that stores constants for map mod.
    Do NOT modify anything here other than MapVectors
    */
    public static class ModConstants
    {
        // The page index of mapTab
        public static int MapTabIndex => Constants.TargetPlatform == GamePlatform.Android ? 4 : GameMenu.mapTab;

        // Cropping heads for NPC markers
        // Values shift the head up (negative) or down (positive)
        public static Dictionary<string, int> NpcMarkerOffsets => new()
        {
            ["Abigail"] = 3,
            ["Alex"] = 0,
            ["Birdie"] = 6,
            ["Caroline"] = 2,
            ["Clint"] = -1,
            ["Demetrius"] = -2,
            ["Dwarf"] = 1,
            ["Elliott"] = -1,
            ["Emily"] = 1,
            ["Evelyn"] = 4,
            ["George"] = 4,
            ["Gus"] = 2,
            ["Gunther"] = 3,
            ["Haley"] = 2,
            ["Harvey"] = -1,
            ["Jas"] = 7,
            ["Jodi"] = 3,
            ["Kent"] = -1,
            ["Krobus"] = 0,
            ["Leah"] = 2,
            ["Leo"] = 6,
            ["Lewis"] = 1,
            ["Linus"] = 6,
            ["Marlon"] = 2,
            ["Marnie"] = 4,
            ["Maru"] = 2,
            ["Pam"] = 5,
            ["Penny"] = 3,
            ["Pierre"] = 0,
            ["Robin"] = 2,
            ["Sam"] = 0,
            ["Sandy"] = 2,
            ["Sebastian"] = 1,
            ["Shane"] = 1,
            ["Vincent"] = 8,
            ["Willy"] = -1,
            ["Wizard"] = 0
        };

        // NPCs with no schedules 
        public static List<string> ExcludedNpcs => new()
        {
            // "Dwarf",
            "Mister Qi",
            "Bouncer",
            "Henchman",
            "Birdie"
            // "Gunther",
            // "Krobus",
            // "Dusty"
        };

        // Spoiler characters that are unlocked later in the game
        public static List<string> ConditionalNpcs => new()
        {
            "Dwarf",
            "Kent",
            "Krobus",
            "Marlon",
            "Merchant",
            "Sandy",
            "Wizard",
            "Leo"
        };

        // tileX and TileY (the first two values) are tile positions in the game for that location
        // X and Y (the latter values) are CENTERED pixel positions in the map sprite that correspond to the game location
        // MapModMain handles all the calculations to make sure the positions are center-based.
        public static Dictionary<string, MapVector[]> MapVectors => new()
        {
            // Outdoor
            ["Backwoods"] = new MapVector[]
            {
                new(385, 122, 0, 0),
                new(529, 238, 50, 40)
            },
            ["Backwoods_Region"] = new MapVector[]
            {
                new(460, 190)
            },
            ["Farm"] = new MapVector[]
            {
                new(330, 237, 0, 0),
                new(514, 386, 80, 65)
            },
            ["Farm_Beach"] = new MapVector[]
            {
                new(330, 237, 0, 0),
                new(514, 386, 102, 106)
            },
            ["Farm_Region"] = new MapVector[]
            {
                new(423, 321)
            },
            ["BusStop"] = new MapVector[]
            {
                new(517, 182, 0, 0),
                new(594, 300, 35, 30)
            },
            ["BusStop_Region"] = new MapVector[]
            {
                new(555, 229)
            },
            ["Merchant"] = new MapVector[]
            {
                new(320, 410)
            },
            ["Forest"] = new MapVector[]
            {
                new(250, 383, 0, 0),
                new(554, 687, 120, 120)
            },
            ["Woods"] = new MapVector[]
            {
                new(136, 346, 0, 0),
                new(230, 395, 60, 32)
            },
            ["DeepWoods"] = new MapVector[]
            {
                new(136, 346, 0, 0),
                new(230, 395, 60, 32)
            },
            ["RuinedHouse"] = new MapVector[]
            {
                new(333, 622)
            },
            ["Town"] = new MapVector[]
            {
                // Top half of town
                new(593, 173, 0, 0),
                new(940, 275, 120, 40),

                // Bottom half of town
                new(602, 268, 0, 41),
                new(958, 513, 120, 110)
            },
            ["TownSquare"] = new MapVector[]
            {
                new(686, 366)
            },
            ["Graveyard"] = new MapVector[]
            {
                new(738, 438)
            },
            ["Beach"] = new MapVector[]
            {
                new(726, 541, 0, 0),
                new(997, 688, 104, 50)
            },
            ["BeachNightMarket"] = new MapVector[]
            {
                new(726, 541, 0, 0),
                new(997, 688, 104, 50)
            },
            ["LonelyStone"] = new MapVector[]
            {
                new(714, 636)
            },
            ["Railroad"] = new MapVector[]
            {
                new(589, 0, 0, 34),
                new(794, 81, 70, 62)
            },
            ["Railroad_Region"] = new MapVector[]
            {
                new(730, 47)
            },
            ["Summit"] = new MapVector[]
            {
                new(819, 36)
            },
            ["Mountain"] = new MapVector[]
            {
                new(718, 81, 0, 0),
                new(1074, 186, 135, 41)
            },
            ["Quarry"] = new MapVector[]
            {
                new(1032, 139)
            },
            ["Desert"] = new MapVector[]
            {
                new(64, 2, 0, 0),
                new(216, 166, 50, 60)
            },
            ["Desert_Region"] = new MapVector[]
            {
                new(130, 96)
            },
            ["MovieTheater"] = new MapVector[]
            {
                new(885, 302)
            },
            ["WizardHouseBasement"] = new MapVector[]
            {
                new(263, 447)
            },
            ["IslandSouth"] = new MapVector[]
            {
                new(1114, 676, 0, 10),
                new(1140, 696, 42, 30)
            },
            ["IslandSouthEast"] = new MapVector[]
            {
                new(1142, 678, 0, 0),
                new(1167, 706, 34, 34)
            },
            ["IslandEast"] = new MapVector[]
            {
                new(1166, 662)
            },
            ["IslandShrine"] = new MapVector[]
            {
                new(1182, 652)
            },
            ["IslandWest"] = new MapVector[]
            {
                new(1030, 660, 14, 4),
                new(1105, 700, 108, 89)
            },
            ["IslandNorth"] = new MapVector[]
            {
                new(1090, 615, 0, 0),
                new(1156, 670, 69, 89)
            },
            ["GingerIsland"] = new MapVector[]
            {
                new(1110, 640)
            }
        };

        // Custom farm markers
        // Also used to do a quick check for currentLocation is farm building
        public static Dictionary<string, Rectangle> FarmBuildingRects => new()
        {
            ["Shed"] = new Rectangle(0, 0, 5, 7),
            ["Coop"] = new Rectangle(5, 0, 5, 7),
            ["Big Coop"] = new Rectangle(5, 0, 5, 7),
            ["Deluxe Coop"] = new Rectangle(5, 0, 5, 7),
            ["Barn"] = new Rectangle(10, 0, 6, 7),
            ["Big Barn"] = new Rectangle(10, 0, 6, 7),
            ["Deluxe Barn"] = new Rectangle(10, 0, 6, 7),
            ["SlimeHutch"] = new Rectangle(16, 0, 7, 7),
            ["Greenhouse"] = new Rectangle(23, 0, 5, 7),
            ["FarmHouse"] = new Rectangle(28, 0, 5, 7),
            ["Cabin"] = new Rectangle(33, 0, 4, 7),
            ["Log Cabin"] = new Rectangle(33, 0, 4, 7),
            ["Plank Cabin"] = new Rectangle(37, 0, 4, 7),
            ["Stone Cabin"] = new Rectangle(41, 0, 4, 7)
        };
    }
}
