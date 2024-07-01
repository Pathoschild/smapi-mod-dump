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
using StardewValley.Characters;

namespace StardewDruid.Data
{

    public class ModData
    {
        public KeybindList riteButtons { get; set; }

        public KeybindList actionButtons { get; set; }

        public KeybindList specialButtons { get; set; }

        public KeybindList journalButtons { get; set; }

        public KeybindList effectsButtons { get; set; }

        public KeybindList relicsButtons { get; set; }

        public KeybindList herbalismButtons { get; set; }

        public string modDifficulty { get; set; }

        public bool disableHands { get; set; }

        public bool autoProgress { get; set; }

        public int setMilestone { get; set; }

        public bool setOnce { get; set; }

        public bool maxDamage { get; set; }

        public bool slotAttune { get; set; }

        public bool slotConsume { get; set; }

        public bool slotFreedom { get; set; }

        public string slotOne { get; set; }

        public string slotTwo { get; set; }

        public string slotThree { get; set; }

        public string slotFour { get; set; }

        public string slotFive { get; set; }

        public string slotSix { get; set; }

        public string slotSeven { get; set; }

        public string slotEight { get; set; }

        public string slotNine { get; set; }

        public string slotTen { get; set; }

        public string slotEleven { get; set; }

        public string slotTwelve { get; set; }

        public int cultivateBehaviour { get; set; }

        public int meteorBehaviour { get; set; }

        public bool cardinalMovement { get; set; }

        public bool castAnywhere { get; set; }

        public bool reverseJournal { get; set; }

        public bool activeJournal { get; set; }

        public bool disableSeeds { get; set; }

        public bool disableFish { get; set; }

        public bool disableTrees { get; set; }

        public bool disableGrass { get; set; }

        public ModData()
        {
            riteButtons = KeybindList.Parse("MouseX1,MouseX2,V,LeftShoulder");
            actionButtons = KeybindList.Parse("MouseLeft,C,ControllerX");
            specialButtons = KeybindList.Parse("MouseRight,X,ControllerY");
            journalButtons = KeybindList.Parse("K");
            effectsButtons = KeybindList.Parse("L");
            relicsButtons = KeybindList.Parse("I");
            herbalismButtons = KeybindList.Parse("O");
            reverseJournal = false;
            activeJournal = true;
            disableHands = false;
            autoProgress = false;
            setMilestone = 0;
            setOnce = false;
            maxDamage = false;
            modDifficulty = "medium";
            slotAttune = true;
            slotConsume = true;
            slotFreedom = false;
            slotOne = "weald";
            slotTwo = "mists";
            slotThree = "stars";
            slotFour = "fates";
            slotFive = "ether";
            slotSix = "none";
            slotSeven = "none";
            slotEight = "none";
            slotNine = "lunch";
            slotTen = "lunch";
            slotEleven = "lunch";
            slotTwelve = "lunch";
            cultivateBehaviour = 1;
            meteorBehaviour = 1;
            cardinalMovement = false;
            castAnywhere = false;
            disableSeeds = false;
            disableFish = false;
            disableTrees = false;
            disableGrass = false;
        }

    }

}