using StardewModdingAPI;

namespace AnimalHusbandryMod
{
    public class ModConfig
    {
        public bool Softmode;
        public SButton? AddMeatCleaverToInventoryKey;
        public SButton? AddInseminationSyringeToInventoryKey;
        public SButton? AddFeedingBasketToInventoryKey;
        public bool DisableFullBuildingForBirthNotification;
        public bool DisableTomorrowBirthNotification;
        public bool DisablePregnancy;
        public bool DisableMeat;
        public bool DisableTreats;
        public bool DisableAnimalContest;
        public bool DisableRancherMeatPriceAjust;
        public bool DisableMoodInscreseWithTreats;
        public bool DisableFriendshipInscreseWithTreats;
        public bool EnableTreatsCountAsAnimalFeed;
        public bool DisableMeatFromDinosaur;
        public double PercentualAjustOnFriendshipInscreaseFromProfessions = 0.25;
        public bool DisableContestBonus;
        public bool DisableMeatToolLetter;
    }
}
