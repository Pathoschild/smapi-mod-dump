/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using StardewModdingAPI.Utilities;

namespace StardewDruid
{

    public class ModData
    {
        public KeybindList riteButtons { get; set; }

        public KeybindList actionButtons { get; set; }

        public KeybindList journalButtons { get; set; }

        public bool slotAttune { get; set; }

        public bool autoProgress { get; set; }

        public string colourPreference { get; set; }

        public bool castBuffs { get; set; }

        public bool consumeRoughage { get; set; }

        public bool consumeQuicksnack { get; set; }

        public bool consumeCaffeine { get; set; }

        public int newProgress { get; set; }

        public bool castAnywhere { get; set; }

        public bool maxDamage { get; set; }

        public string combatDifficulty { get; set; }

        public bool partyHats { get; set; }

        public bool disableSeeds { get; set; }

        public bool disableFish { get; set; }

        public bool disableWildspawn { get; set; }

        public bool disableTrees { get; set; }

        public ModData()
        {
            riteButtons = KeybindList.Parse("MouseX1,MouseX2,V,LeftShoulder");
            actionButtons = KeybindList.Parse("MouseLeft,C,ControllerX");
            journalButtons = KeybindList.Parse("K");
            slotAttune = false;
            autoProgress = false;
            colourPreference = "Red";
            newProgress = -1;
            castBuffs = true;
            consumeRoughage = true;
            consumeQuicksnack = true;
            consumeCaffeine = true;
            maxDamage = false;
            combatDifficulty = "medium";
            castAnywhere = false;
            partyHats = false;
            disableSeeds = false;
            disableFish = false;
            disableWildspawn = false;
            disableTrees = false;
        }

    }

}