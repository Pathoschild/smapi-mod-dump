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
using Omegasis.Revitalize.Framework.Constants.Mail;
using Omegasis.Revitalize.Framework.Constants.PathConstants;
using Omegasis.Revitalize.Framework.Utilities;
using Omegasis.Revitalize.Framework.World.WorldUtilities;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace Omegasis.Revitalize.Framework.World
{
    /// <summary>
    /// Deals with adding custom mail to the game.
    /// </summary>
    public class MailManager
    {

        public const string newLineInMessageCharacter= "^";
        public const string newPageForMessage = "\n";

        public Dictionary<string, string> mailTitles = new Dictionary<string, string>()
        {
            { MailTitles.HayMakerAvailableForPurchase ,Path.Combine(StringsPaths.Mail, "AnimalShopHayMakerCanBePurchased.json")},
            { MailTitles.AutomaticFarmingSystemAvailableForPurchase ,Path.Combine(StringsPaths.Mail, "AutomaticFarmingSystemCanBePurchased.json")},

        };

        public MailManager()
        {
        }

        public virtual void tryToAddMailToMailbox()
        {
            if(Game1.player.mailReceived.Contains(MailTitles.HayMakerAvailableForPurchase)==false && (RevitalizeModCore.SaveDataManager.shopSaveData.animalShopSaveData.getHasBuiltTier2OrHigherBarnOrCoop()==true  || BuildingUtilities.HasBuiltTier2OrHigherBarnOrCoop() == true))
            {
                Game1.mailbox.Add(MailTitles.HayMakerAvailableForPurchase);
            }

            if(Game1.player.mailReceived.Contains(MailTitles.AutomaticFarmingSystemAvailableForPurchase)==false && Game1.player.FarmingLevel >= 10 && Game1.netWorldState.Value.GoldenWalnutsFound.Value >= 1)
            {
                Game1.mailbox.Add(MailTitles.AutomaticFarmingSystemAvailableForPurchase);
            }
        }

        public virtual bool canEditAsset(IAssetInfo asset)
        {
            return asset.NameWithoutLocale.IsEquivalentTo("Data/mail");
        }

        public virtual void editMailAsset(IAssetData asset)
        {
            if (asset.NameWithoutLocale.IsEquivalentTo("Data/mail"))
            {
                IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                data[MailTitles.HayMakerAvailableForPurchase] = this.getMailContentsFromTitle(MailTitles.HayMakerAvailableForPurchase);
                data[MailTitles.AutomaticFarmingSystemAvailableForPurchase] = this.getMailContentsFromTitle(MailTitles.AutomaticFarmingSystemAvailableForPurchase);
            }
        }

        /// <summary>
        /// Gets the path to the mail asset from the title of the mail.
        /// </summary>
        /// <param name="mailTitle"></param>
        /// <returns></returns>
        public virtual string getMailPathFromTitle(string mailTitle)
        {
            if (this.mailTitles.ContainsKey(mailTitle) == false) return "";

            if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.en)
            {
                return this.mailTitles[mailTitle];
            }
            else
            {
                return this.mailTitles[mailTitle].Replace(".json", LocalizedContentManager.CurrentLanguageCode.ToString() + ".json");
            }
        }

        /// <summary>
        /// Loads the letter contents from a given mail title.
        /// </summary>
        /// <param name="mailTitle"></param>
        /// <returns></returns>
        public virtual string getMailContentsFromTitle(string mailTitle)
        {
            return JsonUtilities.LoadStringDictionaryFile(this.getMailPathFromTitle(mailTitle)).Values.First();
        }

        /// <summary>
        /// Parses out the mail message for the menu.
        /// </summary>
        /// <param name="MailMessageText"></param>
        /// <returns></returns>
        public virtual string parseMailMessage(string MailMessageText)
        {
           return MailMessageText.Replace("@", Game1.player.name.Value);
        }

        /// <summary>
        /// Occurs when a new menu is opened.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="menuEventArgs"></param>
        public virtual void onNewMenuOpened(object sender, StardewModdingAPI.Events.MenuChangedEventArgs menuEventArgs)
        {
            
            if (menuEventArgs.NewMenu != null)
            {
                if (menuEventArgs.NewMenu is LetterViewerMenu)
                {
                    LetterViewerMenu letterViewerMenu = (menuEventArgs.NewMenu as LetterViewerMenu);
                    if (letterViewerMenu.isMail)
                    {
                        if (this.mailTitles.ContainsKey(letterViewerMenu.mailTitle))
                        {
                            letterViewerMenu.mailMessage = this.parseMailMessage(this.getMailContentsFromTitle(letterViewerMenu.mailTitle)).Split("\n").ToList();
                        }
                    }
                }
            }
            
        }


    }
}
