/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LukeSeewald/PublicStardewValleyMods
**
*************************************************/

using System.Collections.Generic;

namespace WhatAreYouMissing
{
    public class LocationDisplayNames
    {
        //<gameName<areaCode, locationDisplayName>>
        private Dictionary<string, Dictionary<int,string[]>> PossibleLocationDisplayNames;

        public LocationDisplayNames()
        {
            PossibleLocationDisplayNames = new Dictionary<string, Dictionary<int, string[]>>();
            AddDisplayNames();
        }

        public string[] GetLocationDisplayNames(string gameName, int areaCode)
        {
            if (PossibleLocationDisplayNames.ContainsKey(gameName))
            {
                Dictionary<int, string[]> displayNames = PossibleLocationDisplayNames[gameName];
                if (displayNames.ContainsKey(areaCode))
                {
                    return displayNames[areaCode];
                }
            }
            if (Utilities.IsTempOrFishingGameOrBackwoodsLocation(gameName))
            {
                return new string[1] { "" };
            }

            if(areaCode == -1)
            {
                //Its availble everywhere in this area
                return new string[1] { gameName };
            }
            else
            {
                return new string[1] { gameName + " " + Utilities.GetTranslation("AREA") + " " + areaCode };
            }
        }

        private void AddDisplayNames()
        {

            AddName(Constants.GAME_NAME_SECRET_WOODS, Constants.DEFAULT_AREA_CODE, Utilities.GetTranslation("SECRET_WOODS_DISPLAY_NAME"));

            AddName(Constants.GAME_NAME_TOWN_RIVER, Constants.DEFAULT_AREA_CODE, Utilities.GetTranslation("TOWN_RIVER_DISPLAY_NAME"));
            AddName(Constants.GAME_NAME_MOUNTAIN_LAKE, Constants.DEFAULT_AREA_CODE, Utilities.GetTranslation("MOUNTAIN_LAKE_DISPLAY_NAME"));
            AddName(Constants.GAME_NAME_BEACH, Constants.DEFAULT_AREA_CODE, Utilities.GetTranslation("BEACH_DISPLAY_NAME"));

            AddName(Constants.GAME_NAME_SEWER, Constants.DEFAULT_AREA_CODE, Utilities.GetTranslation("SEWER_DISPLAY_NAME"));
            AddName(Constants.GAME_NAME_MUTANT_BUG_LAIR, Constants.DEFAULT_AREA_CODE, Utilities.GetTranslation("MUTANT_BUG_LAIR_DISPLAY_NAME"));

            AddName(Constants.GAME_NAME_MINES, Constants.DEFAULT_AREA_CODE, Utilities.GetTranslation("MINES_DISPLAY_NAME"));
            AddName(Constants.GAME_NAME_DESERT, Constants.DEFAULT_AREA_CODE, Utilities.GetTranslation("DESERT_DISPLAY_NAME"));
            AddName(Constants.GAME_NAME_WITCHS_SWAMP, Constants.DEFAULT_AREA_CODE, Utilities.GetTranslation("WITCHS_SWAMP_DISPLAY_NAME"));

            AddCindersapForestNames();
        }

        private void AddName(string gameName, int areaCode, string displayName)
        {
            PossibleLocationDisplayNames.Add(gameName, new Dictionary<int, string[]> { [areaCode] = new string[1] { displayName } });
        }

        private void AddCindersapForestNames()
        {
            string[] pondAndRiver = new string[2] { Utilities.GetTranslation("CINDERSAP_POND_DISPLAY_NAME"), Utilities.GetTranslation("CINDERSAP_RIVER_DISPLAY_NAME")};
            Dictionary<int, string[]> cindersapDict = new Dictionary<int, string[]>
            {
                [Constants.DEFAULT_AREA_CODE] = pondAndRiver,
                [Constants.CINDERSAP_POND_AREA_CODE] = new string[1] { Utilities.GetTranslation("CINDERSAP_POND_DISPLAY_NAME")},
                [Constants.CINDERSAP_RIVER_AREA_CODE] = new string[1] { Utilities.GetTranslation("CINDERSAP_RIVER_DISPLAY_NAME")},
            };

            PossibleLocationDisplayNames.Add(Constants.GAME_NAME_CINDERSAP_FOREST, cindersapDict);
        }
    }
}
