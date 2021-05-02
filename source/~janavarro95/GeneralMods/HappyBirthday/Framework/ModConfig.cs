/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using StardewModdingAPI;

namespace Omegasis.HappyBirthday.Framework
{
    /// <summary>The mod configuration.</summary>
    public class ModConfig
    {
        /// <summary>The key which shows the menu.</summary>
        public SButton KeyBinding { get; set; } = SButton.O;

        /// <summary>The minimum amount of friendship needed to get a happy birthday greeting from an npc.</summary>
        public int minimumFriendshipLevelForBirthdayWish = 2;

        public bool autoSetTranslation { get; set; } = true;

        /// <summary>Handles different translations of files.</summary>
        public TranslationInfo translationInfo;

        public bool defaultToEnglishTranslation;
        

        /// <summary>Construct an instance.</summary>
        public ModConfig()
        {
            this.translationInfo = new TranslationInfo();
            this.defaultToEnglishTranslation = true;
        }
    }
}
