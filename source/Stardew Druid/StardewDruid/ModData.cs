/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using System.Collections.Generic;
using StardewModdingAPI;
using Microsoft.Xna.Framework;
using System.Reflection.PortableExecutable;
using StardewModdingAPI.Utilities;
using System.Runtime.CompilerServices;

namespace StardewDruid
{

    public class ModData
    {

        public StardewModdingAPI.Utilities.KeybindList riteButtons { get; set; }

        public Dictionary<string, int> blessingList { get; set; }

        public Dictionary<string, bool> questList { get; set; }

        public bool castBuffs { get; set; }

        public bool consumeRoughage { get; set; }

        public bool consumeQuicksnack { get; set; }

        public bool consumeCaffeine { get; set; }

        public int setProgress { get; set; }

        //public bool masterStart { get; set; }

        public bool maxDamage { get; set; }

        public bool unrestrictedStars { get; set; }

        public string combatDifficulty { get; set; }

        public Dictionary<int, string> weaponAttunement { get; set; }

        public bool checkQuests { get; set; }

        public bool partyHats { get; set; }

        public int farmCaveStatueX { get; set; }

        public int farmCaveStatueY { get; set; }

        public int farmCaveActionX { get; set; }

        public int farmCaveActionY { get; set; }

        public bool farmCaveHideStatue { get; set; }

        public bool farmCaveMakeSpace { get; set; }

        public ModData()
        {

            riteButtons = KeybindList.Parse("MouseX1,MouseX2,V,LeftShoulder");

            questList = new();

            blessingList = new();

            /*blessingList = new()
            {
                ["earth"] = 5,
                ["water"] = 5,
                ["stars"] = 1,
                ["levelPickaxe"] = 5,
                ["levelAxe"] = 5,
            };*/

            setProgress = -1;

            castBuffs = true;

            consumeRoughage = true;

            consumeQuicksnack = true;

            consumeCaffeine = true;

            //masterStart = false;

            maxDamage = false;

            combatDifficulty = "medium";

            unrestrictedStars = false;

            weaponAttunement = new()
            {
                [15] = "earth",
                [14] = "water",
                [9] = "stars",
            };

            partyHats = false;

            farmCaveActionX = 6;

            farmCaveActionY = 4;

            farmCaveStatueX = 6;

            farmCaveStatueY = 3;

            farmCaveHideStatue = false;

            farmCaveMakeSpace = true;

        }

    }

}