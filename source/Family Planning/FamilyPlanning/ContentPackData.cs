/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/loe2run/FamilyPlanningMod
**
*************************************************/

using System;
using System.Collections.Generic;

namespace FamilyPlanning
{
    /* 
     * I'm expanding from a ChildSpriteData Content Pack to a generic ContentPackData.
     * This will support child sprites and will also support dialogue for your spouse.
     * Details on these are below.
     */ 

    class ContentPackData
    {
        /* ChildSpriteID:
         * -> The ChildSpriteID data structure contains the data required to pair children with their sprites.
         * -> The first string, the key, is the name of the child.
         * -> The second string, the value, is a pair of file names. First is the baby sprites, second is the toddler sprites.
         */
        public Dictionary<string, Tuple<string, string>> ChildSpriteID { get; set; }

        /* BirthSpouseDialogue:
         * -> The BirthSpouseDialogue data structure contains dialogue information for the CustomBirthingEvent.
         * -> The first string, the key, is the name of the spouse you'd like it to effect. (This matches the displayName value.)
         * -> The second string, the value, is pair of values.
         *    The first is the number of children where the dialogue should trigger, second is the new dialogue.
         *    The second value, the dialogue, supports the use of {0} to represent the baby name and {1} to represent the player name.
         */ 
        public Dictionary<string, List<Tuple<int, string>>> SpouseDialogue { get; set; }

        public ContentPackData()
        {
            ChildSpriteID = new Dictionary<string, Tuple<string, string>>();
            SpouseDialogue = new Dictionary<string, List<Tuple<int, string>>>();
        }
    }
}