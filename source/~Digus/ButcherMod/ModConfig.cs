/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

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
        public bool DisableMeatInBlundle;
        public bool ForceDrawAttachmentOnAnyOS;
    }
}
