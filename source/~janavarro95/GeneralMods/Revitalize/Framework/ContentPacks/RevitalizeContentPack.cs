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
using Omegasis.Revitalize.Framework.Content.JsonContent.Objects;
using Omegasis.Revitalize.Framework.Utilities;
using Omegasis.Revitalize.Framework.World.Mail;
using Omegasis.Revitalize.Framework.World.WorldUtilities.Items;
using Omegasis.StardustCore.Events;
using StardewModdingAPI;

namespace Omegasis.Revitalize.Framework.ContentPacks
{
    /// <summary>
    /// A wrapper class that wraps <see cref="IContentPack"/> for <see cref="RevitalizeModCore"/> loaded in from SMAPI.
    /// </summary>
    public class RevitalizeContentPack
    {
        /// <summary>
        /// The base content pack that was loaded in from SMAPI for happy birthday.
        /// </summary>
        public IContentPack baseContentPack;

        /// <summary>
        /// The language code for this content pack.
        /// </summary>
        public string languageCode;

        /// <summary>
        /// Includes mail strings which are displayed to the player.
        /// </summary>
        public Dictionary<string, MailInfo> mail = new Dictionary<string, MailInfo>();

        /// <summary>
        /// Includes strings to be displayed for objects and items.
        /// </summary>
        public Dictionary<string, IdToDisplayStrings> objectDisplayStrings = new Dictionary<string, IdToDisplayStrings>();

        /// <summary>
        /// Includes strings to be displayed for objects and items.
        /// </summary>
        public Dictionary<string, IdToDisplayStrings> buildingDisplayStrings = new Dictionary<string, IdToDisplayStrings>();

        public ContentPackProcessingRecipeManager objectProcessingRecipeManager;

        /// <summary>
        /// The unique id for this content pack.
        /// </summary>
        public string UniqueId
        {
            get
            {
                return this.baseContentPack.Manifest.UniqueID;
            }
        }

        /// <summary>
        /// Default constuctor.
        /// </summary>
        public RevitalizeContentPack()
        {
        }

        /// <summary>
        /// Loads all of the information from a given content pack from HappyBirthday into this helper class.
        /// </summary>
        /// <param name="contentPack"></param>
        public virtual void load(StardewModdingAPI.IContentPack contentPack)
        {
            this.baseContentPack = contentPack;
            this.languageCode = JsonUtilities.loadFirstString(this,"TranslationInfo.json");

            this.mail = new Dictionary<string, MailInfo>();
            this.objectDisplayStrings = new Dictionary<string, IdToDisplayStrings>();

            this.loadInDisplayStrings();
            this.loadInMail();

            this.objectProcessingRecipeManager = new ContentPackProcessingRecipeManager(this);
            this.objectProcessingRecipeManager.loadRecipes();

        }

        /// <summary>
        /// Loads in all display strings from a given .json dictionary file.
        /// </summary>
        protected virtual void loadInDisplayStrings()
        {
            List<Dictionary<string, IdToDisplayStrings>> objectDisplayStringInfo = JsonUtilities.LoadJsonFilesFromDirectories<Dictionary<string, IdToDisplayStrings>>(this,Constants.PathConstants.StringsPaths.ObjectDisplayStrings);
            foreach (Dictionary<string, IdToDisplayStrings> dict in objectDisplayStringInfo)
            {
                foreach (KeyValuePair<string, IdToDisplayStrings> pair in dict)
                {
                    if (this.objectDisplayStrings.ContainsKey(pair.Key))
                    {
                        continue;
                    }
                    this.objectDisplayStrings.Add(pair.Key, pair.Value);
                }
            }

            List<Dictionary<string, IdToDisplayStrings>> buildingDisplayStrings = JsonUtilities.LoadJsonFilesFromDirectories<Dictionary<string, IdToDisplayStrings>>(this, Constants.PathConstants.StringsPaths.BuildingDisplayStrings);
            foreach (Dictionary<string, IdToDisplayStrings> dict in buildingDisplayStrings)
            {
                foreach (KeyValuePair<string, IdToDisplayStrings> pair in dict)
                {
                    if (this.buildingDisplayStrings.ContainsKey(pair.Key))
                    {
                        continue;
                    }
                    this.buildingDisplayStrings.Add(pair.Key, pair.Value);
                }
            }
        }

        /// <summary>
        /// Loads in all mail information from .json files.
        /// </summary>
        protected virtual void loadInMail()
        {
            List<MailInfo> loadedMail = JsonUtilities.LoadJsonFilesFromDirectories<MailInfo>(this,Constants.PathConstants.StringsPaths.Mail);
            foreach (MailInfo letter in loadedMail)
            {
                if (this.mail.ContainsKey(letter.mailTitle))
                {
                    continue;
                }
                this.mail.Add(letter.mailTitle, letter);
            }
        }

        /// <summary>
        /// Gets a mail string from this content pack.
        /// </summary>
        /// <param name="MailTitle"></param>
        /// <param name="LogError"></param>
        /// <returns></returns>
        public virtual string getMailString(string MailTitle, bool LogError = true)
        {
            if (this.mail.ContainsKey(MailTitle) && string.IsNullOrEmpty(this.mail[MailTitle].message) == false)
            {
                return this.mail[MailTitle].message;
            }
            if (LogError)
            {
                RevitalizeModCore.Instance.Monitor.Log(string.Format("Error: Attempted to get a mail string with key {0} from Revitalize content pack, but the given mail string key does not exist for the given content pack {1}.", MailTitle, this.UniqueId), LogLevel.Error);
            }
            return "";
        }

        /// <summary>
        /// Gets the path to the content pack's strings folder.
        /// </summary>
        /// <returns></returns>
        public virtual string getContentPackStringsPath()
        {
            return Path.Combine("ModAssets", "Strings");
        }

        public virtual string loadShopDialogue(string RelativePath, string Key)
        {
            return "";
        }
    }
}
