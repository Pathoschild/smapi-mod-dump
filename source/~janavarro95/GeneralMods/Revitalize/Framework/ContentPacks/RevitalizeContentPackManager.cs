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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omegasis.Revitalize.Framework.Utilities;
using StardewModdingAPI;
using StardewValley;

namespace Omegasis.Revitalize.Framework.ContentPacks
{
    public class RevitalizeContentPackManager
    {
        /// <summary>
        /// A key of language code to multiple content packs for a given translation.
        /// </summary>
        public Dictionary<string, List<RevitalizeContentPack>> contentPacks;

        /// <summary>
        /// A keying of the unique id of a content pack to the LanguageCode list it belongs to.
        /// </summary>
        public Dictionary<string, string> contentPackIdToLanguageCodes;

        public bool contentPacksInitialized;

        public RevitalizeContentPackManager()
        {
            this.contentPacks = new Dictionary<string, List<RevitalizeContentPack>>();
            this.contentPackIdToLanguageCodes = new Dictionary<string, string>();
        }

        public virtual void initializeContentPacks()
        {
            if (this.contentPacksInitialized) return;
            if (RevitalizeModCore.Instance.Helper.ContentPacks.GetOwned().Count() == 0)
            {
                throw new InvalidDataException("There are ZERO Revitalize content packs found for the mod. Without at least one installed there is no guaranteed that this mod will work due to missing dialogue errors. Please install at least one Revitalize content pack before continuing. Thank you!");
            }

            foreach (IContentPack contentPack in RevitalizeModCore.Instance.Helper.ContentPacks.GetOwned())
            {
                this.registerNewContentPack(contentPack);
            }
            this.contentPacksInitialized = true;
        }

        /// <summary>
        /// Registers a new happy birthday content pack.
        /// </summary>
        /// <param name="contentPack"></param>
        public virtual void registerNewContentPack(IContentPack contentPack)
        {
            RevitalizeContentPack revitalizeContentPack = new RevitalizeContentPack();
            revitalizeContentPack.load(contentPack);

            if (this.contentPacks.ContainsKey(revitalizeContentPack.languageCode))
            {
                this.contentPacks[revitalizeContentPack.languageCode].Add(revitalizeContentPack);
            }
            else
            {
                this.contentPacks.Add(revitalizeContentPack.languageCode, new List<RevitalizeContentPack>() { revitalizeContentPack });
            }

            if (this.contentPackIdToLanguageCodes.ContainsKey(revitalizeContentPack.UniqueId))
            {
                return;
            }

            this.contentPackIdToLanguageCodes.Add(revitalizeContentPack.UniqueId, revitalizeContentPack.languageCode);
            RevitalizeModCore.Instance.Monitor.Log(string.Format("Registering Revitalize Content Pack: {0}", revitalizeContentPack.UniqueId));
        }

        /// <summary>
        /// Gets a content pack with the given unique id.
        /// </summary>
        /// <param name="UniqueId">The unique id which is the <see cref="IContentPack"/>'s <see cref="IManifest.UniqueID"/>.</param>
        /// <returns>A <see cref="HappyBirthdayContentPack>"/> with the matching unique id.</returns>
        public virtual RevitalizeContentPack getContentPack(string UniqueId)
        {
            if (this.contentPackIdToLanguageCodes.ContainsKey(UniqueId) == false)
            {
                throw new ArgumentException(string.Format("Content pack with unique id {0} has not been registered. This should have happened automatically, so is this a typo or is the content pack not installed?", UniqueId));
            }
            string languageCode = this.contentPackIdToLanguageCodes[UniqueId];
            foreach (RevitalizeContentPack contentPack in this.contentPacks[languageCode])
            {
                if (contentPack.UniqueId.Equals(UniqueId))
                {
                    return contentPack;
                }
            }
            throw new ArgumentException(string.Format("Content pack with unique id {0} has not been registered to the list of available content packs. This should not have happened since there should be a check in place to prevent this...", UniqueId));
        }

        /// <summary>
        /// Gets all content packs that have been loaded in.
        /// </summary>
        /// <returns></returns>
        public virtual List<RevitalizeContentPack> getAllContentPacks()
        {
            List<RevitalizeContentPack> contentPacks= new List<RevitalizeContentPack>();
            foreach(KeyValuePair<string,List<RevitalizeContentPack>> contentPacksByLanguageCode in this.contentPacks)
            {
                foreach(RevitalizeContentPack contentPack in contentPacksByLanguageCode.Value)
                {
                    contentPacks.Add(contentPack);
                }
            }
            return contentPacks;
        }

        /// <summary>
        /// Gets all of the <see cref="HappyBirthdayContentPack"/>s that affect the current localization.
        /// </summary>
        /// <returns></returns>
        public virtual List<RevitalizeContentPack> getContentPacksForCurrentLanguageCode()
        {
            string currentLanguageCode = LocalizationUtilities.GetCurrentLanguageCodeString();
            if (this.contentPacks.ContainsKey(currentLanguageCode))
            {
                List<RevitalizeContentPack> contentPacks = this.contentPacks[currentLanguageCode];
                if (contentPacks.Count > 0)
                {
                    return contentPacks;
                }
                if (contentPacks.Count == 0 && RevitalizeModCore.Configs.contentPackConfig.fallbackToEnglish)
                {
                    return this.getContentPacksForEnglishLanguageCode();
                }
            }
            else
            {
                RevitalizeModCore.Instance.Monitor.Log("Language code {0} not included in content pack manager???");
                if (RevitalizeModCore.Configs.contentPackConfig.fallbackToEnglish)
                {

                    return this.getContentPacksForEnglishLanguageCode();

                }
            }
            throw new Exception(string.Format("There were zero content packs for Revitalize for the given language code {0}. This is a fatal error as the modded cutscenes WILL NOT work without at least one proper content pack installed. Did you mean to install one? If one is installed, is the language code in TranslationInfo.json correct?", LocalizationUtilities.GetCurrentLanguageCodeString()));

        }

        public virtual List<RevitalizeContentPack> getContentPacksForEnglishLanguageCode()
        {
            string currentLanguageCode = "en-US";
            if (this.contentPacks.ContainsKey(currentLanguageCode))
            {
                return this.contentPacks[currentLanguageCode];
            }
            else
            {
                throw new Exception(string.Format("There were zero content packs for Revitalize for the English language code en-US. This is a fatal error as the modded cutscenes WILL NOT work without at least one proper content pack installed. Did you mean to install one? If one is installed, is the language code in TranslationInfo.json correct?", LocalizationUtilities.GetCurrentLanguageCodeString()));
            }
        }

    }
}
