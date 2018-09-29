using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquivalentExchange
{
    class Reference
    {
        // struct for remembering important item Ids, uses the statics from the game when possible.
        // not all of them are mapped, so some are hard coded.
        public struct Items
        {
            public const int Sap = 92;
            public const int Hay = 178;
            public const int Clay = 330;
            public const int CopperOre = StardewValley.Object.copper;
            public const int IronOre = StardewValley.Object.iron;
            public const int GoldOre = StardewValley.Object.gold;
            public const int IridiumOre = StardewValley.Object.iridium;
            public const int Coal = StardewValley.Object.coal;
            public const int Wood = StardewValley.Object.wood;
            public const int Stone = StardewValley.Object.stone;
            public const int Hardwood = 709;
            public const int Fiber = 771;
            public const int Slime = 766;            
        }

        public struct Characters
        {
            public const string WizardName = "Wizard";
        }

        public struct Localizations
        {
            public const string WizardSpeech = "WizardSpeech";
            public const string Level = "Level";
            public const string Alchemy = "Alchemy";
            public const string LevelUp = "LevelUp";
            public const string LevelUp2 = "LevelUp2";
            public const string LevelUpEven = "LevelUpEven";
            public const string SkillHoverText1 = "SkillHoverText1";
            public const string SkillHoverText2 = "SkillHoverText2";
            public const string StaminaDrain = "StaminaDrain";
            public const string ChooseAProfession = "ChooseAProfession";
            public const string Shaper = "Shaper";
            public const string ShaperDescription = "ShaperDescription";
            public const string Sage = "Sage";
            public const string SageDescription = "SageDescription";
            public const string Transmuter = "Transmuter";
            public const string TransmuterDescription = "TransmuterDescription";
            public const string Adept = "Adept";
            public const string AdeptDescription = "AdeptDescription";
            public const string Aurumancer = "Aurumancer";
            public const string AurumancerDescription = "AurumancerDescription";
            public const string Conduit = "Conduit";
            public const string ConduitDescription = "ConduitDescription";
            public const string AlchemyUnlocked = "AlchemyUnlocked";
            public const string Needs = "Needs";
        }
    }
}
