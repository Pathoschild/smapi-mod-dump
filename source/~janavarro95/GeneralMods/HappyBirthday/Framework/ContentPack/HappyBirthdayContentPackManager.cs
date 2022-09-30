/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omegasis.HappyBirthday.Framework.Utilities;
using StardewModdingAPI;
using StardewValley;

namespace Omegasis.HappyBirthday.Framework.ContentPack
{
    public class HappyBirthdayContentPackManager
    {
        /// <summary>
        /// A key of language code to multiple content packs for a given translation.
        /// </summary>
        public Dictionary<string, List<HappyBirthdayContentPack>> contentPacks;

        /// <summary>
        /// A keying of the unique id of a content pack to the LanguageCode list it belongs to.
        /// </summary>
        public Dictionary<string, string> contentPackIdToLanguageCodes;

        public HappyBirthdayContentPackManager()
        {
            this.contentPacks = new Dictionary<string, List<HappyBirthdayContentPack>>();
            this.contentPackIdToLanguageCodes = new Dictionary<string, string>();
        }

        /// <summary>
        /// Registers a new happy birthday content pack.
        /// </summary>
        /// <param name="contentPack"></param>
        public virtual void registerNewContentPack(IContentPack contentPack)
        {
            HappyBirthdayContentPack happyBirthdayContentPack = new HappyBirthdayContentPack();
            happyBirthdayContentPack.load(contentPack);

            if (this.contentPacks.ContainsKey(happyBirthdayContentPack.languageCode))
            {
                this.contentPacks[happyBirthdayContentPack.languageCode].Add(happyBirthdayContentPack);
            }
            else
            {
                this.contentPacks.Add(happyBirthdayContentPack.languageCode, new List<HappyBirthdayContentPack>() { happyBirthdayContentPack });
            }

            if (this.contentPackIdToLanguageCodes.ContainsKey(happyBirthdayContentPack.UniqueId))
            {
                return;
            }

            this.contentPackIdToLanguageCodes.Add(happyBirthdayContentPack.UniqueId, happyBirthdayContentPack.languageCode);
            HappyBirthdayModCore.Instance.Monitor.Log(string.Format("Registering Happy Birthday Content Pack: {0}", happyBirthdayContentPack.UniqueId));
        }

        /// <summary>
        /// Gets a content pack with the given unique id.
        /// </summary>
        /// <param name="UniqueId">The unique id which is the <see cref="IContentPack"/>'s <see cref="IManifest.UniqueID"/>.</param>
        /// <returns>A <see cref="HappyBirthdayContentPack>"/> with the matching unique id.</returns>
        public virtual HappyBirthdayContentPack getContentPack(string UniqueId)
        {
            if (this.contentPackIdToLanguageCodes.ContainsKey(UniqueId) == false)
            {
                throw new ArgumentException(string.Format("Content pack with unique id {0} has not been registered. This should have happened automatically, so is this a typo or is the content pack not installed?", UniqueId));
            }
            string languageCode = this.contentPackIdToLanguageCodes[UniqueId];
            foreach (HappyBirthdayContentPack contentPack in this.contentPacks[languageCode])
            {
                if (contentPack.UniqueId.Equals(UniqueId))
                {
                    return contentPack;
                }
            }
            throw new ArgumentException(string.Format("Content pack with unique id {0} has not been registered to the list of available content packs. This should not have happened since there should be a check in place to prevent this...", UniqueId));
        }

        /// <summary>
        /// Gets all of the <see cref="HappyBirthdayContentPack"/>s that affect the current localization.
        /// </summary>
        /// <returns></returns>
        public virtual List<HappyBirthdayContentPack> getHappyBirthdayContentPacksForCurrentLanguageCode()
        {
            string currentLanguageCode = LocalizationUtilities.GetCurrentLanguageCodeString();
            if (this.contentPacks.ContainsKey(currentLanguageCode))
            {
                List<HappyBirthdayContentPack> contentPacks = this.contentPacks[currentLanguageCode];
                if (contentPacks.Count > 0)
                {
                    return contentPacks;
                }
                if (contentPacks.Count == 0 && HappyBirthdayModCore.Configs.modConfig.fallbackToEnglishTranslationWhenPossible)
                {
                    return this.getHappyBirthdayContentPacksForEnglishLanguageCode();
                }
            }
            else
            {
                HappyBirthdayModCore.Instance.Monitor.Log("Language code {0} not included in content pack manager???");
                if (HappyBirthdayModCore.Configs.modConfig.fallbackToEnglishTranslationWhenPossible)
                {

                    return this.getHappyBirthdayContentPacksForEnglishLanguageCode();

                }
            }
            throw new Exception(string.Format("There were zero content packs for Happy Birthday for the given language code {0}. This is a fatal error as the modded cutscenes WILL NOT work without at least one proper content pack installed. Did you mean to install one? If one is installed, is the language code in TranslationInfo.json correct?", LocalizationUtilities.GetCurrentLanguageCodeString()));

        }

        public virtual List<HappyBirthdayContentPack> getHappyBirthdayContentPacksForEnglishLanguageCode()
        {
            string currentLanguageCode = "en-US";
            if (this.contentPacks.ContainsKey(currentLanguageCode))
            {
                return this.contentPacks[currentLanguageCode];
            }
            else
            {
                throw new Exception(string.Format("There were zero content packs for Happy Birthday for the English language code en-US. This is a fatal error as the modded cutscenes WILL NOT work without at least one proper content pack installed. Did you mean to install one? If one is installed, is the language code in TranslationInfo.json correct?", LocalizationUtilities.GetCurrentLanguageCodeString()));
            }
        }

    }
}
