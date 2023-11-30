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
using StardewValley.Tools;

namespace StardewDruid
{

    public class ModData
    {

        public StardewModdingAPI.Utilities.KeybindList riteButtons { get; set; }

        public StardewModdingAPI.Utilities.KeybindList actionButtons { get; set; }

        public bool slotAttune { get; set; }

        //public bool autoLesson { get; set; }

        public Dictionary<string, int> blessingList { get; set; }

        public Dictionary<string, bool> questList { get; set; }

        public bool castBuffs { get; set; }

        public bool consumeRoughage { get; set; }

        public bool consumeQuicksnack { get; set; }

        public bool consumeCaffeine { get; set; }

        public int setProgress { get; set; }

        public bool castAnywhere { get; set; }

        public bool maxDamage { get; set; }

        public string combatDifficulty { get; set; }

        public Dictionary<int, string> weaponAttunement { get; set; }

        public bool partyHats { get; set; }

        public bool disableSeeds { get; set; }

        public bool disableFish { get; set; }

        public bool disableWildspawn { get; set; }

        public bool disableTrees { get; set; }

        public ModData() 
        {
  
            riteButtons = KeybindList.Parse("MouseX1,MouseX2,V,LeftShoulder");

            actionButtons = KeybindList.Parse("MouseLeft,C,ControllerX");

            slotAttune = false;

            //autoLesson = false;

            questList = new();

            blessingList = new();

            setProgress = -1;

            castBuffs = true;

            consumeRoughage = true;

            consumeQuicksnack = true;

            consumeCaffeine = true;

            maxDamage = false;

            combatDifficulty = "medium";

            castAnywhere = false;

            weaponAttunement = new()
            {
                [15] = "earth",
                [14] = "water",
                [9] = "stars",
                [53] = "fates",
            };

            partyHats = false;

            disableSeeds = false;

            disableFish = false;

            disableWildspawn = false;

            disableTrees = false;

        }

    }

}