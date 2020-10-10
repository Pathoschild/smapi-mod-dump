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
    class ChildSpriteData
    {
        /* The ChildSpriteData data structure contains the data required to pair children with their sprites.
         * The first string, the key, is the name of the child.
         * The second string, the value, is a pair of file names. First is the baby sprites, second is the toddler sprites.
         */
        public Dictionary<string, Tuple<string, string>> ChildSpriteID { get; set; }

        public ChildSpriteData()
        {
            ChildSpriteID = new Dictionary<string, Tuple<string, string>>();
        }

        /* Example case:
         * 
         * Your farmer is married to Leah, and you'd like to use Lakoria's BabiesTakeAfterSpouse sprites for two daughters.
         * 
         * For the first daughter, Amber, you'd like to use:
         * -> the sprite "leahbaby.png" as a baby, and
         * -> the sprite "leahhairbuns.png" as a toddler.
         * For the second daughter, Beverly, you'd like to use:
         * -> the sprite "leahbaby.png" as a baby, and
         * -> the sprite "leahpigtails.png" as a toddler.
         * 
         * Put the sprites you're using, "leahbaby.png", "leahhairbuns.png", and "leahpigtails.png", into the assets folder.
         * Then edit the data.json in the assets folder so that it reads:
         * {
         *   "ChildSpriteID": {
         *     "Amber": {
         *       "Item1": "leahbaby.png",
         *       "Item2": "leahhairbuns.png"
         *     },
         *     "Beverly": {
         *       "Item1": "leahbaby.png",
         *       "Item2": "leahpigtails.png"
         *     }
         *   }
         * }
         */
    }
}
