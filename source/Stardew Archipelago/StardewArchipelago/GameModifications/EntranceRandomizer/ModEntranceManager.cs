/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Constants.Modded;

namespace StardewArchipelago.GameModifications.EntranceRandomizer
{
    public class ModEntranceManager
    {

        public static List<string> GrandpaShedEdgeCase = new(){
            "Custom_GrandpasShed to Custom_GrandpasShedGreenhouse", "Custom_GrandpasShedGreenhouse to Custom_GrandpasShed"
        };

        private static readonly Dictionary<string, string> _locationSVE = new()
        {
            { "Willy's Bedroom", "Custom_WillyRoom" },
            { "Grandpa's Shed Interior", "Custom_GrandpasShedRuins" },
            { "Grandpa's Shed Upstairs", "Custom_GrandpasShedGreenhouse" },
            { "Grandpa's Shed", "Custom_GrandpasShedOutside" },
            { "Marnie's Shed", "Custom_MarnieShed" },
            { "Fairhaven Farm Cellar", "Custom_AndyCellar" },
            { "Fairhaven Farm", "Custom_AndyHouse" },
            { "Sophia's House", "Custom_SophiaHouse" },
            { "Sophia's Cellar", "Custom_SophiaCellar" },
            { "Jenkins' Residence", "Custom_JenkinsHouse" },
            { "Jenkins' Cellar", "Custom_OliviaCellar" },
            { "Unclaimed Plot", "Custom_TownEast" },
            { "Shearwater Bridge", "Custom_ShearwaterBridge" },
            { "Fable Reef", "Custom_FableReef" },
            { "First Slash Guild", "Custom_FirstSlashGuild" },
            { "Highlands Outside", "Custom_Highlands" },
            { "Highlands Cavern", "Custom_HighlandsCavern"},
            { "Highlands Cavern Prison", "Custom_HighlandsCavernPrison"},
            { "Lance's House Ladder", "Custom_HighlandsOutpost|12|5" },
            { "Lance's House Main", "Custom_HighlandsOutpost|7|9" },
            { "Lost Woods", "Custom_JunimoWoods|37|2" },
            { "Badlands Entrance", "Custom_DesertRailway" },
            { "Enchanted Grove", "Custom_EnchantedGrove" },
            { "Grove Aurora Vineyard Warp", "Custom_EnchantedGrove|20|41" },
            { "Grove Junimo Woods Warp", "Custom_EnchantedGrove|40|40" },
            { "Grove Outpost Warp", "Custom_EnchantedGrove|40|10" },
            { "Grove Farm Warp", "Custom_EnchantedGrove|30|14" },
            { "Grove Wizard Warp", "Custom_EnchantedGrove|17|25" },
            { "Grove Guild Warp", "Custom_EnchantedGrove|43|25" },
            { "Grove Sprite Spring Warp", "Custom_EnchantedGrove|20|10" },
            { "Sprite Spring Cave", "Custom_SpriteSpringCave" },
            { "Sprite Spring", "Custom_SpriteSpring2" },
            { "Junimo Woods", "Custom_JunimoWoods" },
            { "Aurora Vineyard Basement", "Custom_ApplesRoom" },
            { "Blue Moon Vineyard", "Custom_BlueMoonVineyard" },
            { "Aurora Vineyard", "Custom_AuroraVineyard" },
            { "Galmoran Outpost", "Custom_CastleVillageOutpost" },
            { "Crimson Badlands", "Custom_CrimsonBadlands" },
            { "Badlands Cave", "Custom_TreasureCave" },
            { "Susan's House", "Custom_SusanHouse" },
            { "Guild Summit", "Custom_AdventurerSummit" },
            { "Forest West", "Custom_ForestWest" },
            { "First Slash Hallway", "Custom_FirstSlashHallway" },
            { "First Slash Spare Room", "Custom_FirstSlashGuestRoom" },
            { "Grampleton Suburbs", "Custom_GrampletonSuburbs" },
            { "Scarlett's House", "Custom_ScarlettHouse" },
            { "Wizard Basement", "Custom_WizardBasement" },
            { "Gunther's Bedroom", "Custom_GunthersRoom"}
        };

        private static readonly Dictionary<string, string> _locationEugene = new()
        {
            { "Eugene's Garden", "Custom_EugeneNPC_EugeneHouse" },
            { "Eugene's Bedroom", "Custom_EugeneNPC_EugeneRoom" },
        };

        private static readonly Dictionary<string, string> _locationDeepWoods = new()
        {
            { "Deep Woods House", "DeepWoodsMaxHouse" },
        };

        private static readonly Dictionary<string, string> _locationAlec = new()
        {
            { "Alec's Pet Shop", "Custom_AlecsPetShop" },
            { "Alec's Bedroom", "Custom_AlecsRoom" },
        };

        private static readonly Dictionary<string, string> _locationJuna = new()
        {
            { "Juna's Cave", "Custom_JunaNPC_JunaCave" },
        };

        private static readonly Dictionary<string, string> _locationAyeisha = new()
        {
            { "Ayeisha's Mail Van", "Custom_AyeishaVanRoad" },
        };

        private static readonly Dictionary<string, string> _locationJasper = new()
        {
            { "Jasper's Bedroom", "Custom_LK_Museum2" },
        };
    
        private static readonly Dictionary<string, string> _locationYoba = new()
        {
            { "Yoba's Clearing", "Custom_Woods3" },
        };                

        private static readonly Dictionary<string, string> _locationAlecto = new()
        {
            { "Witch's Attic", "Custom_Alecto_WitchHutUpstairs"},
        };

        private static readonly Dictionary<string, string> _locationLacey = new()
        {
            { "Mouse House", "Custom_HatMouseLacey_MouseHouse"}
        };

        private static readonly Dictionary<string, string> _locationBoardingHouse = new()
        {
            { "Boarding House Outside", "Custom_BoardingHouse_BackwoodsPlateau" },
            { "Boarding House - First Floor", "Custom_BoardingHouse_BoardingHouse"},
            { "Boarding House - Second Floor", "Custom_BoardingHouse_BoardingHouse2" },
            { "Abandoned Mines - 1A", "Custom_BoardingHouse_AbandonedMine1A" },
            { "Abandoned Mines - 1B", "Custom_BoardingHouse_AbandonedMine1B" },
            { "Abandoned Mines - 2A", "Custom_BoardingHouse_AbandonedMine2A" },
            { "Abandoned Mines - 2B", "Custom_BoardingHouse_AbandonedMine2B" },
            { "Abandoned Mines - 3", "Custom_BoardingHouse_AbandonedMine3" },
            { "Abandoned Mines - 4", "Custom_BoardingHouse_AbandonedMine4" },
            { "Abandoned Mines - 5", "Custom_BoardingHouse_AbandonedMine5" },
            { "Abandoned Mines Entrance", "Custom_BoardingHouse_MineEntrance" },
            { "The Lost Valley", "Custom_BoardingHouse_TheLostValley" },
            { "Gregory's Tent", "Custom_BoardingHouse_GregoryTent" },
            { "Lost Valley Ruins - First House", "Custom_BoardingHouse_Ruins_House1" },
            { "Lost Valley Ruins - Second House", "Custom_BoardingHouse_Ruins_House2" },
            { "Lost Valley Ruins", "Custom_BoardingHouse_Ruins" },
            { "Buffalo's Ranch", "Custom_BoardingHouse_BuffalosRanch" },
            { "Lost Valley Minecart", "Custom_BoardingHouse_TheLostValley|3|22" }


        };                

        private static readonly Dictionary<string, Dictionary<string, string>> _locationAliasesByMod = new()
        {
            { ModNames.ALEC, _locationAlec },
            { ModNames.AYEISHA, _locationAyeisha },
            { ModNames.DEEP_WOODS, _locationDeepWoods },
            { ModNames.EUGENE, _locationEugene },
            { ModNames.JASPER, _locationJasper },
            { ModNames.JUNA, _locationJuna },
            { ModNames.SVE, _locationSVE },
            { ModNames.YOBA, _locationYoba },
            { ModNames.ALECTO, _locationAlecto },
            { ModNames.LACEY, _locationLacey },
            { ModNames.BOARDING_HOUSE, _locationBoardingHouse },
        };

        public Dictionary<string, string> GetModLocationAliases(SlotData slotData)
        {
            var modLocationAliases = new Dictionary<string, string>();
            foreach (var (_, locationAliases) in _locationAliasesByMod.Where(x => slotData.Mods.HasMod(x.Key)))
            {
                foreach (var (key, value) in locationAliases)
                {
                    modLocationAliases.Add(key, value);
                }
            }

            return modLocationAliases;
        }
         
        public static bool CheckForGrandpasShedGreenhouseEdgeCase(string currentLocationName, string locationRequestName)
        {
            var case1 = currentLocationName == "Custom_GrandpasShed" && locationRequestName == "Custom_GrandpasShedGreenhouse";
            var case2 = locationRequestName == "Custom_GrandpasShed" && currentLocationName == "Custom_GrandpasShedGreenhouse";
            if (case1 || case2)
            {
                return true;
            }
            return false;
        }

            
    }
}