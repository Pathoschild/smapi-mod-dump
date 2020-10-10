/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AairTheGreat/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;

namespace BetterGarbageCans.Data
{
    //  Garbage Can         Index
    //==============================
    //  Jodi/Sam            0
    //  Haley/Emily         1
    //  Mayor Lewis         2
    //  Library/Gunthers    3
    //  Clint's             4    
    //  Stardrop Saloon     5
    //  Evelyn/George       6
    //  JoJa Can            7

    public enum GARBAGE_CANS
    {
        JODI_SAM=0,
        EMILY_HALEY,
        MAYOR_LEWIS,
        GUNTHER,
        CLINT,
        STARDROP_SALOON,
        EVELYN_GEORGE,
        JOJA_MART
    }

    public class GarbageCan 
    {
        public GARBAGE_CANS GarbageCanID { get; set; }

        public int LastTimeChecked { get; set; }

        public int LastTimeFoundItem { get; set; }

        public List<TrashTreasure> treasureList { get; set; }

        public GarbageCan(GARBAGE_CANS id)
        {
            this.GarbageCanID = id;
            this.LastTimeChecked = -1;
            this.LastTimeFoundItem = -1;
        }
    }
}
