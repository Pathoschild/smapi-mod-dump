using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return new string[1] { "" };
        }

        private void AddDisplayNames()
        {

            AddName(Constants.GAME_NAME_SECRET_WOODS, Constants.DEFAULT_AREA_CODE, Constants.SECRET_WOODS_DISPLAY_NAME);

            AddName(Constants.GAME_NAME_TOWN_RIVER, Constants.DEFAULT_AREA_CODE, Constants.TOWN_RIVER_DISPLAY_NAME);
            AddName(Constants.GAME_NAME_MOUNTAIN_LAKE, Constants.DEFAULT_AREA_CODE, Constants.MOUNTAIN_LAKE_DISPLAY_NAME);
            AddName(Constants.GAME_NAME_BEACH, Constants.DEFAULT_AREA_CODE, Constants.BEACH_DISPLAY_NAME);

            AddName(Constants.GAME_NAME_SEWER, Constants.DEFAULT_AREA_CODE, Constants.SEWER_DISPLAY_NAME);
            AddName(Constants.GAME_NAME_MUTANT_BUG_LAIR, Constants.DEFAULT_AREA_CODE, Constants.MUTANT_BUG_LAIR_DISPLAY_NAME);

            AddName(Constants.GAME_NAME_MINES, Constants.DEFAULT_AREA_CODE, Constants.MINES_DISPLAY_NAME);
            AddName(Constants.GAME_NAME_DESERT, Constants.DEFAULT_AREA_CODE, Constants.DESERT_DISPLAY_NAME);
            AddName(Constants.GAME_NAME_WITCHS_SWAMP, Constants.DEFAULT_AREA_CODE, Constants.WITCHS_SWAMP_DISPLAY_NAME);

            AddCindersapForestNames();
        }

        private void AddName(string gameName, int areaCode, string displayName)
        {
            PossibleLocationDisplayNames.Add(gameName, new Dictionary<int, string[]> { [areaCode] = new string[1] { displayName } });
        }

        private void AddCindersapForestNames()
        {
            string[] pondAndRiver = new string[2] { Constants.CINDERSAP_POND_DISPLAY_NAME, Constants.CINDERSAP_RIVER_DISPLAY_NAME };
            Dictionary<int, string[]> cindersapDict = new Dictionary<int, string[]>
            {
                [Constants.DEFAULT_AREA_CODE] = pondAndRiver,
                [Constants.CINDERSAP_POND_AREA_CODE] = new string[1] { Constants.CINDERSAP_POND_DISPLAY_NAME },
                [Constants.CINDERSAP_RIVER_AREA_CODE] = new string[1] { Constants.CINDERSAP_RIVER_DISPLAY_NAME },
            };

            PossibleLocationDisplayNames.Add(Constants.GAME_NAME_CINDERSAP_FOREST, cindersapDict);
        }
    }
}
