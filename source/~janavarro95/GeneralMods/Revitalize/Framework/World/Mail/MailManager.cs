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
using Microsoft.Xna.Framework;
using Omegasis.Revitalize.Framework.Constants;
using Omegasis.Revitalize.Framework.Constants.Mail;
using Omegasis.Revitalize.Framework.Constants.PathConstants;
using Omegasis.Revitalize.Framework.ContentPacks;
using Omegasis.Revitalize.Framework.Player;
using Omegasis.Revitalize.Framework.Utilities;
using Omegasis.Revitalize.Framework.Utilities.JsonContentLoading;
using Omegasis.Revitalize.Framework.World.Buildings;
using Omegasis.Revitalize.Framework.World.WorldUtilities;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;

namespace Omegasis.Revitalize.Framework.World.Mail
{
    /// <summary>
    /// Deals with adding custom mail to the game.
    ///
    /// In order to add new mail contents to the game, you must do the following.
    /// 1. Create a new .json file with the mail contents.
    /// 2. Add the mail title constant string and the path to the mail file into the <see cref="mailTitles"/> dictionary.
    /// 3. Update the <see cref="tryToAddAllMailToMailbox"/> method as that will check for specific conditions 
    /// </summary>
    public class MailManager
    {

        public const string newLineInMessageCharacter = "^";
        public const string newPageForMessage = "\n";

        public MailManager()
        { 

        }


        /// <summary>
        /// Tries to add mail to the Player's mailbox when certain events happen for the mod.
        /// </summary>
        public virtual void tryToAddAllMailToMailbox()
        {
            this.addMailIfNotReceived(MailTitles.HayMakerAvailableForPurchase, RevitalizeModCore.SaveDataManager.shopSaveData.animalShopSaveData.getHasBuiltTier2OrHigherBarnOrCoop() || BuildingUtilities.HasBuiltTier2OrHigherBarnOrCoop());
            this.addMailIfNotReceived(MailTitles.SiloRefillServiceAvailable, Utility.numSilos() >= 1);
            this.addMailIfNotReceived(MailTitles.AutomaticFarmingSystemAvailableForPurchase, Game1.player.FarmingLevel >= 10);
            this.addMailIfNotReceived(MailTitles.ElectricFurnaceCanBePurchased, PlayerUtilities.HasObtainedItem(Enums.SDVObject.BatteryPack));

            //Geode crushers available.
            this.addMailIfNotReceived(MailTitles.AdvancedGeodeCrusherUnlock, Game1.stats.GeodesCracked >= 200);
            this.addMailIfNotReceived(MailTitles.ElectricGeodeCrusherUnlock, Game1.stats.GeodesCracked >= 500);
            this.addMailIfNotReceived(MailTitles.NuclearGeodeCrusherUnlock, Game1.stats.GeodesCracked >= 750 && PlayerUtilities.GetNumberOfGoldenWalnutsFound() >= 100);
            this.addMailIfNotReceived(MailTitles.MagicalGeodeCrusherUnlock, Game1.stats.GeodesCracked >= 1000 && PlayerUtilities.GetNumberOfGoldenWalnutsFound() >= 100);

            //Mail from clint
            this.addMailIfNotReceived(MailTitles.MiningDrillsAvailableInClintsShop, Game1.player.hasSkullKey);

            //Charcoal kilns available.
            this.addMailIfNotReceived(MailTitles.AdvancedCharcoalKilnBlueprintUnlock, Game1.player.ForagingLevel>=8);
            this.addMailIfNotReceived(MailTitles.DeluxCharcoalKilnForSale, Game1.player.ForagingLevel >= 8 && PlayerUtilities.GetNumberOfGoldenWalnutsFound()>=1);
            this.addMailIfNotReceived(MailTitles.SuperiorCharcoalKilnForSale,true);

            this.addMailIfNotReceived(MailTitles.BurnerBatteryGeneratorUnlock, Game1.player.deepestMineLevel >= 50);
            this.addMailIfNotReceived(MailTitles.NuclearBatteryGeneratorUnlock, GameLocationUtilities.AreTheHardMinesEnabled());

            this.addMailIfNotReceived(MailTitles.CrystalRefinerUnlock, WorldUtility.GetNumberOfMineralsDonatedToMuseum()>=40);

            this.addMailIfNotReceived(MailTitles.WindmillBlueprintsFromLeah, Game1.player.ForagingLevel >= 5);

            //Special cases where mail needs to repeat over and over again.
            if (!Game1.player.mailbox.Contains(MailTitles.MovieTheaterTicketSubscriptionTickets) && Game1.dayOfMonth == 1 && RevitalizeModCore.SaveDataManager.playerSaveData.hasMovieTheaterTicketSubscription)
            {
                Game1.mailbox.Add(MailTitles.MovieTheaterTicketSubscriptionTickets);
            }


        }

        /// <summary>
        /// Trys to add the mail to the player's mailbox if they have not already received it, as well as some additional conditions that need to be met to send the mail.
        /// </summary>
        /// <param name="MailTitle"></param>
        /// <param name="AdditionalConditions"></param>
        public virtual void addMailIfNotReceived(string MailTitle, bool AdditionalConditions=true)
        {
            if (!this.hasOrWillPlayerReceivedThisMail(MailTitle) && AdditionalConditions)
            {
                Game1.mailbox.Add(MailTitle);
            }
        }

        /// <summary>
        /// Checks to see if the player has received a given peice of mail.
        /// </summary>
        /// <param name="MailTitle">The title of the mail to check.</param>
        /// <returns></returns>
        public virtual bool hasOrWillPlayerReceivedThisMail(string MailTitle)
        {
            return Game1.player.mailReceived.Contains(MailTitle) || Game1.player.mailbox.Contains(MailTitle) || Game1.player.mailForTomorrow.Contains(MailTitle);
        }

        public virtual void editMailAsset(StardewModdingAPI.Events.AssetRequestedEventArgs assetRequest)
        {
            if (assetRequest.NameWithoutLocale.BaseName.Equals("Data/mail"))
            {
                assetRequest.Edit(this.editMailAsset);
            }
        }

        public virtual void editMailAsset(IAssetData asset)
        {
            IAssetDataForDictionary<string, string> assetInformation = asset.AsDictionary<string, string>();
            foreach (MailInfo mail in this.getMailInfo().Values)
                assetInformation.Data[mail.mailTitle] = Game1.parseText(mail.message);
        }

        /// <summary>
        /// Gets all possible mail added in by content packs.
        /// </summary>
        /// <returns></returns>
        public virtual Dictionary<string, MailInfo> getMailInfo()
        {
            List<RevitalizeContentPack> contentPacks = RevitalizeModCore.ModContentManager.revitalizeContentPackManager.getContentPacksForCurrentLanguageCode();

            Dictionary<string, MailInfo> mails = new Dictionary<string, MailInfo>();

            foreach (RevitalizeContentPack contentPack in contentPacks)
            {
                foreach (KeyValuePair<string, MailInfo> letter in contentPack.mail)
                {
                    if (!mails.ContainsKey(letter.Key))
                    {
                        mails.Add(letter.Key, letter.Value);
                    }
                }
            }
            return mails;
        }

        /// <summary>
        /// Parses out the mail message for the menu.
        /// </summary>
        /// <param name="MailMessageText"></param>
        /// <returns></returns>
        public virtual string parseMailMessage(MailInfo mailInfo)
        {
            string MailMessageText = mailInfo.message;
            LetterViewerMenu letterViewerMenu = Game1.activeClickableMenu as LetterViewerMenu;

            //Parse vanilla logic for adding items, objects, furniture, and money to letters.
            if (MailMessageText.Contains("%item"))
            {
                string itemDescription = MailMessageText.Substring(MailMessageText.IndexOf("%item"), MailMessageText.IndexOf("%%") + 2 - MailMessageText.IndexOf("%item"));
                string[] split = itemDescription.Split(' ');
                MailMessageText = MailMessageText.Replace(itemDescription, "");
                Rectangle itemToGetPosition = new Rectangle(letterViewerMenu.xPositionOnScreen + letterViewerMenu.width / 2 - 48, letterViewerMenu.yPositionOnScreen + letterViewerMenu.height - 32 - 96, 96, 96);
                if (!letterViewerMenu.isFromCollection)
                {
                    if (split[1].Equals("object"))
                    {
                        int maxNum3 = split.Length - 1;
                        int which3 = Game1.random.Next(2, maxNum3);
                        which3 -= which3 % 2;
                        StardewValley.Object o3 = new StardewValley.Object(Vector2.Zero, Convert.ToInt32(split[which3]), Convert.ToInt32(split[which3 + 1]));
                        letterViewerMenu.itemsToGrab.Add(new ClickableComponent(itemToGetPosition, o3)
                        {
                            myID = 104,
                            leftNeighborID = 101,
                            rightNeighborID = 102
                        });
                        letterViewerMenu.backButton.rightNeighborID = 104;
                        letterViewerMenu.forwardButton.leftNeighborID = 104;
                    }
                    else if (split[1].Equals("bigobject"))
                    {
                        int maxNum2 = split.Length - 1;
                        int which2 = Game1.random.Next(2, maxNum2);
                        StardewValley.Object o2 = new StardewValley.Object(Vector2.Zero, Convert.ToInt32(split[which2]));
                        letterViewerMenu.itemsToGrab.Add(new ClickableComponent(itemToGetPosition, o2)
                        {
                            myID = 104,
                            leftNeighborID = 101,
                            rightNeighborID = 102
                        });
                        letterViewerMenu.backButton.rightNeighborID = 104;
                        letterViewerMenu.forwardButton.leftNeighborID = 104;
                    }
                    else if (split[1].Equals("furniture"))
                    {
                        int maxNum = split.Length - 1;
                        int which = Game1.random.Next(2, maxNum);
                        Item o = Furniture.GetFurnitureInstance(Convert.ToInt32(split[which]));
                        letterViewerMenu.itemsToGrab.Add(new ClickableComponent(itemToGetPosition, o)
                        {
                            myID = 104,
                            leftNeighborID = 101,
                            rightNeighborID = 102
                        });
                        letterViewerMenu.backButton.rightNeighborID = 104;
                        letterViewerMenu.forwardButton.leftNeighborID = 104;
                    }
                    else if (split[1].Equals("money"))
                    {
                        int moneyToAdd = ((split.Length > 4) ? Game1.random.Next(Convert.ToInt32(split[2]), Convert.ToInt32(split[3])) : Convert.ToInt32(split[2]));
                        moneyToAdd -= moneyToAdd % 10;
                        Game1.player.Money += moneyToAdd;
                        letterViewerMenu.moneyIncluded = moneyToAdd;
                    }
                }
            }

            //Add mail item references to the letter.
            if (mailInfo.itemReference.isNotNull())
            {
                StardewValley.Item obj = mailInfo.itemReference.getItem();
                Rectangle itemToGetPosition = new Rectangle(letterViewerMenu.xPositionOnScreen + letterViewerMenu.width / 2 - 48, letterViewerMenu.yPositionOnScreen + letterViewerMenu.height - 32 - 96, 96, 96);
                letterViewerMenu.itemsToGrab.Add(new ClickableComponent(itemToGetPosition, obj)
                {
                    myID = 104,
                    leftNeighborID = 101,
                    rightNeighborID = 102
                });
            }


            return MailMessageText.Replace("@", Game1.player.Name);
        }

        /// <summary>
        /// Occurs when a new menu is opened.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="menuEventArgs"></param>
        public virtual void onNewMenuOpened(object sender, StardewModdingAPI.Events.MenuChangedEventArgs menuEventArgs)
        {
            if (menuEventArgs.NewMenu != null && menuEventArgs.NewMenu is LetterViewerMenu)
            {
                LetterViewerMenu letterViewerMenu = menuEventArgs.NewMenu as LetterViewerMenu;
                Dictionary<string, MailInfo> mail = this.getMailInfo();

                if (letterViewerMenu.isMail && mail.ContainsKey(letterViewerMenu.mailTitle))
                {
                    letterViewerMenu.mailMessage = this.parseMailMessage(mail[letterViewerMenu.mailTitle]).Split("\n").ToList();
                }
            }
        }

        /// <summary>
        /// Gets the proper mail string for getting items in the mail.
        /// </summary>
        /// <param name="ParentSheetIndex"></param>
        /// <param name="StackSize"></param>
        /// <returns></returns>
        public static string GetItemMailStringFormat(Enums.SDVObject ParentSheetIndex, int StackSize)
        {
            return string.Format("%item object {0} {1} %%", (int)ParentSheetIndex, StackSize);
        }
    }
}
