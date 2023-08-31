/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Rafseazz/Ridgeside-Village-Mod
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RidgesideVillage
{
    class ModConfig
    {
        //Ridgeside Village SMAPI Config Options

        public bool ShowVillagersOnMap = true;

        //events offset by install date via RSVInstallDay token
        public bool ProgressiveStory = true;

        public bool RepeatCableCarCutscene = true;

        public bool EnableOtherNPCsInCableCar = true;

        public bool EnableRidgesideMusic = true;

        public bool RSVNPCSAttendFestivals = true;

        public bool ExpandedFestivalMaps = true;

        public bool EasyIntroduction = false;

        public bool EnableBetterBusStop = true;

        public bool PoleAtBackwoods = true;

        public bool SeasonalRSVMap = true;

        public bool EnableTouristNPCs = true;

    }
}
