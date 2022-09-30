/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System.IO;
using StardewModdingAPI;

namespace Omegasis.HappyBirthday.Framework.Configs
{
    /// <summary>The mod configuration.</summary>
    public class ModConfig
    {
        /// <summary>The key which shows the menu.</summary>
        public SButton KeyBinding { get; set; } = SButton.O;

        /// <summary>The minimum amount of friendship needed to get a happy birthday greeting from an npc.</summary>
        public int minimumFriendshipLevelForBirthdayWish = 2;

        /// <summary>
        /// The minimum amount of friendship needed with all villagers that would be present to get the saloon birthday party;
        /// </summary>
        public int minimumFriendshipLevelForCommunityBirthdayParty = 5;

        /// <summary>
        /// Attempts to use the English content pack when a properly localized one does not exist.
        /// </summary>
        public bool fallbackToEnglishTranslationWhenPossible=false;
        

        /// <summary>Construct an instance.</summary>
        public ModConfig()
        {
        }


        /// <summary>
        /// Initializes the config for the blacksmith shop prices.
        /// </summary>
        /// <returns></returns>
        public static ModConfig InitializeConfig()
        {
            if (HappyBirthdayModCore.Configs.doesConfigExist("ModConfig.json"))
            {
                return HappyBirthdayModCore.Configs.ReadConfig<ModConfig>("ModConfig.json");
            }
            else
            {
                ModConfig Config = new ModConfig();
                HappyBirthdayModCore.Configs.WriteConfig("ModConfig.json", Config);
                return Config;
            }
        }
    }
}
