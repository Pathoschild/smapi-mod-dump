using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using FelixDev.StardewMods.FeTK.ModHelpers;
using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FelixDev.StardewMods.FeTK.Framework.Helpers;
using FelixDev.StardewMods.FeTK.Framework.Serialization;
using FelixDev.StardewMods.Common.StardewValley;

namespace FelixDev.StardewMods.FeTK.Framework.Services
{
    /// <summary>
    /// Provides an API to add mails to the player's mailbox.
    /// </summary>
    public class MailService : IMailSender
    {
        /// <summary>The prefix of the key used to identify the save data created by this mail service.</summary>
        private const string SAVE_DATA_KEY_PREFIX = "FelixDev.StardewMods.FeTK.Framework.Services.MailService";

        /// <summary>Provides access to the <see cref="IModEvents"/> API provided by SMAPI.</summary>
        private static readonly IModEvents events = ToolkitMod.ModHelper.Events;

        /// <summary>Provides access to the <see cref="IMonitor"/> API provided by SMAPI.</summary>
        private static readonly IMonitor monitor = ToolkitMod._Monitor;

        /// <summary>The ID of the mod which uses this mail service.</summary>
        private readonly string modId;

        /// <summary>The key used to identify the save data created by this mail service.</summary>
        private readonly string saveDataKey;

        /// <summary>The mail manager used to add mails to the game and provide mail events.</summary>
        private readonly IMailManager mailManager;

        /// <summary>The save data manager for this mail service.</summary>
        private readonly ModSaveDataHelper saveDataHelper;

        /// <summary>A helper to write and retrieve the save data for this mail service.</summary>
        private readonly SaveDataBuilder saveDataBuilder;

        /// <summary>
        /// Contains all mails added with this mail service which have not been read by the player yet. 
        /// For each day a collection of mails with that arrival day is stored (using a mapping [mail ID] -> [mail]).
        /// </summary>
        private Dictionary<int, Dictionary<string, Mail>> mailListForDay = new Dictionary<int, Dictionary<string, Mail>>();

        /// <summary>
        /// Raised when a mail begins to open. The mail content can still be changed at this point.
        /// </summary>
        public event EventHandler<MailOpeningEventArgs> MailOpening;

        /// <summary>
        /// Raised when a mail has been closed.
        /// </summary>
        public event EventHandler<MailClosedEventArgs> MailClosed;

        /// <summary>
        /// Create a new instance of the <see cref="MailService"/> class.
        /// </summary>
        /// <param name="modId">The ID of the mod for which this mail service will be created for.</param>
        /// <param name="mailManager">The <see cref="IMailManager"/> instance which will be used by this service to add mails to the game.</param>
        /// <exception cref="ArgumentNullException">
        /// The specified <paramref name="modId"/> is <c>null</c> or does not contain at least one 
        /// non-whitespace character.</exception>
        /// <exception cref="ArgumentNullException">The specified <paramref name="mailManager"/> is <c>null</c>.</exception>
        internal MailService(string modId, IMailManager mailManager)
        {
            if (string.IsNullOrWhiteSpace(modId))
            {
                throw new ArgumentException("The mod ID needs to contain at least one non-whitespace character!", nameof(modId));
            }

            if (mailManager == null)
            {
                throw new ArgumentNullException(nameof(mailManager));
            }

            this.modId = modId;
            this.mailManager = mailManager;

            this.saveDataKey = SAVE_DATA_KEY_PREFIX + "." + modId;

            this.saveDataHelper = ModSaveDataHelper.GetSaveDataHelper(modId);
            this.saveDataBuilder = new SaveDataBuilder(new ItemSerializer());

            events.GameLoop.Saving += OnSaving;
            events.GameLoop.SaveLoaded += OnSaveLoaded;
        }

        /// <summary>
        /// Add a mail to the player's mailbox.
        /// </summary>
        /// <param name="mail">The mail to add.</param>
        /// <param name="daysFromNow">
        /// The in-game day offset when the mail will arrive in the mailbox. Pass in "0" to instantly add a mail to the player's mailbox.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="daysFromNow"/> has to be greater than or equal to <c>0</c>.</exception>
        /// <exception cref="ArgumentNullException">The specified <paramref name="mail"/> is <c>null</c>.</exception>
        public void AddMail(Mail mail, int daysFromNow)
        {
            if (daysFromNow < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(daysFromNow), "The day offset cannot be a negative number!");
            }

            if (mail == null)
            {
                throw new ArgumentNullException(nameof(mail));
            }

            var arrivalDate = SDate.Now().AddDays(daysFromNow);
            var arrivalGameDay = arrivalDate.DaysSinceStart;

            if (HasMailForDayCore(arrivalGameDay, mail.Id))
            {
                string message = $"A mail with the ID \"{mail.Id}\" already exists for the date {arrivalDate}!";

                monitor.Log(message + " Please use a different mail ID!");
                throw new ArgumentException(message);
            }

            // Add the mail to the mail manager. Surface exceptions, if any, as they will indicate
            // errors with the user supplied arguments.
            mailManager.Add(this.modId, mail.Id, arrivalDate);
            
            if (!mailListForDay.TryGetValue(arrivalGameDay, out Dictionary<string, Mail> mailForDay))
            {
                mailListForDay.Add(arrivalGameDay, mailForDay = new Dictionary<string, Mail>()); 
            }

            mailForDay.Add(mail.Id, mail);
        }

        /// <summary>
        /// Add a mail to the player's mailbox.
        /// </summary>
        /// <param name="mail">The mail to add.</param>
        /// <param name="arrivalDay">
        /// The in-game day when the mail will arrive in the player's mailbox. Pass in the current date to instantly add a mail to the mailbox.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="arrivalDay"/> is in the past.</exception>
        /// <exception cref="ArgumentNullException">The specified <paramref name="mail"/> is be <c>null</c>.</exception>
        public void AddMail(Mail mail, SDate arrivalDay)
        {
            if (arrivalDay == null)
            {
                throw new ArgumentNullException(nameof(arrivalDay));
            }

            AddMail(mail, SDateHelper.GetCurrentDayOffsetFromDate(arrivalDay));
        }

        /// <summary>
        /// Get whether a mail added by this mail service already exists for the specified day.
        /// </summary>
        /// <param name="day">The day to check for.</param>
        /// <param name="mailId">The ID of the mail.</param>
        /// <returns>
        /// <c>true</c> if a mail with the specified <paramref name="mailId"/> has already been added for the specified <paramref name="day"/>;
        /// otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">The specified <paramref name="day"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">
        /// The specified <paramref name="mailId"/> is <c>null</c> or does not contain at least one 
        /// non-whitespace character.
        /// </exception>
        public bool HasMailForDay(SDate day, string mailId)
        {
            if (day == null)
            {
                throw new ArgumentNullException(nameof(day));
            }

            if (string.IsNullOrWhiteSpace(mailId))
            {
                throw new ArgumentException("The mail ID needs to contain at least one non-whitespace character!", nameof(mailId));
            }

            var gameDay = day.DaysSinceStart;
            return HasMailForDayCore(gameDay, mailId);
        }

        /// <summary>
        /// Get whether the specified mail was already sent to the player.
        /// </summary>
        /// <param name="mailId">The ID of the mail.</param>
        /// <returns>
        /// <c>true</c> if a mail with the specified <paramref name="mailId"/> was already sent to the player; 
        /// otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// The specified <paramref name="mailId"/> is <c>null</c> or does not contain at least one 
        /// non-whitespace character.
        /// </exception>
        public bool HasReceivedMail(string mailId)
        {
            if (string.IsNullOrWhiteSpace(mailId))
            {
                throw new ArgumentException("The mail ID needs to contain at least one non-whitespace character!", nameof(mailId));
            }

            return mailManager.HasReceivedMail(this.modId, mailId);
        }

        /// <summary>
        /// Get whether a mail by this mail service is currently in the mailbox.
        /// </summary>
        /// <param name="mailId">The ID of the mail.</param>
        /// <returns><c>true</c> if a mail with the specified <paramref name="mailId"/> has already been registered and 
        /// is currently in the mailbox; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentException">
        /// The specified <paramref name="mailId"/> is <c>null</c> or does not contain at least one 
        /// non-whitespace character.
        /// </exception>
        public bool HasMailInMailbox(string mailId)
        {
            if (string.IsNullOrWhiteSpace(mailId))
            {
                throw new ArgumentException("The mail ID needs to contain at least one non-whitespace character!", nameof(mailId));
            }

            return mailManager.HasMailInMailbox(this.modId, mailId);
        }

        /// <summary>
        /// Get whether a mail added by this mail service already exists for the specified day.
        /// </summary>
        /// <param name="gameDay">The day to check for.</param>
        /// <param name="mailId">The ID of the mail.</param>
        /// <returns>
        /// <c>true</c> if a mail with the specified <paramref name="mailId"/> has already been added for the specified <paramref name="gameDay"/>; 
        /// otherwise, <c>false</c>.
        /// </returns>
        private bool HasMailForDayCore(int gameDay, string mailId)
        {
            return this.mailListForDay.ContainsKey(gameDay) && this.mailListForDay[gameDay].ContainsKey(mailId);
        }

        /// <summary>
        /// Notify an observer that a mail is being opened.
        /// </summary>
        /// <param name="e">Information about the mail being opened.</param>
        void IMailObserver.OnMailOpening(MailOpeningEventArgs e)
        {
            // Raise the mail-opening event.
            this.MailOpening?.Invoke(this, e);
        }

        /// <summary>
        /// Notify an observer that a mail has been closed.
        /// </summary>
        /// <param name="e">Information about the closed mail.</param>
        void IMailObserver.OnMailClosed(MailClosedCoreEventArgs e)
        {
            // Remove the mail from the service. 
            // We don't need to do key checks here because the service is only notified 
            // for closed mails belonging to it.
            this.mailListForDay[e.ArrivalDay.DaysSinceStart].Remove(e.Id);

            // Raise the mail-closed event.
            this.MailClosed?.Invoke(this, new MailClosedEventArgs(e.Id, e.InteractionRecord));
        }

        /// <summary>
        /// Retrieve a mail by its ID and arrival day.
        /// </summary>
        /// <param name="mailId">The ID of the mail.</param>
        /// <param name="arrivalDay">The mail's arrival day in the mailbox of the receiver.</param>
        /// <returns>
        /// A <see cref="Mail"/> instance with the specified <paramref name="mailId"/> and <paramref name="arrivalDay"/> on success;
        /// otherwise, <c>null</c>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// The specified <paramref name="mailId"/> is <c>null</c> or does not contain at least one non-whitespace character.
        /// </exception>
        /// <exception cref="ArgumentNullException">The specified <paramref name="arrivalDay"/> is <c>null</c>.</exception>
        Mail IMailSender.GetMailFromId(string mailId, SDate arrivalDay)
        {
            if (string.IsNullOrWhiteSpace(mailId))
            {
                throw new ArgumentException("The mail ID needs to contain at least one non-whitespace character!", nameof(mailId));
            }

            if (arrivalDay == null)
            {
                throw new ArgumentNullException(nameof(arrivalDay));
            }

            int arrivalGameDay = arrivalDay.DaysSinceStart;
            return !mailListForDay.TryGetValue(arrivalGameDay, out Dictionary<string, Mail> mailForDay)
                || !mailForDay.TryGetValue(mailId, out Mail mail)
                ? null
                : mail;
        }

        /// <summary>
        /// Called when the game is about to write data to a save file. This function is responsible for saving
        /// the mail list to the save file.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnSaving(object sender, SavingEventArgs e)
        {
            var mailSaveData = this.saveDataBuilder.Construct(this.mailListForDay);

            var saveData = new SaveData(mailSaveData);
            this.saveDataHelper.WriteData(this.saveDataKey, saveData);
        }

        /// <summary>
        /// Called after the game loaded a save file. This function is responsible for reading the mail list
        /// from the loaded save file.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            var saveData = saveDataHelper.ReadData<SaveData>(this.saveDataKey);

            this.mailListForDay = saveData != null
                ? this.saveDataBuilder.Reconstruct(saveData.MailSaved)
                : new Dictionary<int, Dictionary<string, Mail>>();
        }

        /// <summary>
        /// The <see cref="MailSaveData"/> class encapsulates data about a <see cref="Mail"/> instance which needs to 
        /// be written to/read from a game's save file.
        /// </summary>
        private class MailSaveData
        {
            /// <summary>
            /// Create a new instance of the <see cref="MailSaveData"/> class.
            /// </summary>
            /// <remarks>
            /// This constructur is used by the used save game serializer.
            /// </remarks>
            public MailSaveData() { }

            /// <summary>
            /// Create a new instance of the <see cref="MailSaveData"/> class.
            /// </summary>
            /// <param name="id">The ID of the mail.</param>
            /// <param name="text">The text content of the mail.</param>
            /// <param name="arrivalDay">The in-game day when the mail arrives in the player's mailbox.</param>
            public MailSaveData(string id, string text, int arrivalDay)
            {
                Id = id;
                AbsoluteArrivalDay = arrivalDay;
                Text = text;
            }

            /// <summary>
            /// The type of the mail.
            /// </summary>
            public MailType MailType { get; set; }

            /// <summary>
            /// The ID of the mail.
            /// </summary>
            public string Id { get; set; }

            /// <summary>
            /// The text content of the mail.
            /// </summary>
            public string Text { get; set; }

            /// <summary>
            /// The in-game day when the mail arrives in the player's mailbox.
            /// </summary>
            public int AbsoluteArrivalDay { get; set; }

            #region Item Mail Content

            /// <summary>
            /// The items attached to the mail.
            /// </summary>
            public List<ItemSaveData> AttachedItemsSaveData { get; set; }

            #endregion // Item Mail Content

            #region Money Mail Content

            /// <summary>
            /// The monetary value attached to the mail.
            /// </summary>
            public int Money { get; set; }

            /// <summary>
            /// The currency of the monetary value attached to the mail.
            /// </summary>
            public Currency Currency { get; set; }

            #endregion // Money Mail Content

            #region Quest Mail Content

            /// <summary>
            /// The ID of the quest attached to the mail.
            /// </summary>
            public int QuestId { get; set; }

            /// <summary>
            /// Indicates if the quest is automatically accepted or needs to be accepted manually.
            /// </summary>
            public bool IsAutomaticallyAccepted { get; set; }

            #endregion

            #region Recipe Mail Content

            /// <summary>
            /// The recipe attached to the mail.
            /// </summary>
            public RecipeData Recipe { get; set; }

            #endregion // Recipe Mail Content
        }

        /// <summary>
        /// The <see cref="saveDataBuilder"/> class provides an API to (de-)serialize the mail list of a <see cref="MailService"/>
        /// instance.
        /// </summary>
        private class SaveDataBuilder
        {
            /// <summary>The serializer to use when (de-)serializing <see cref="Item"/> instances.</summary>
            private readonly IItemSerializer itemSerializer;

            /// <summary>
            /// Create a new instance of the <see cref="saveDataBuilder"/> class.
            /// </summary>
            /// <param name="itemSerializer">The <see cref="IItemSerializer"/> instance to (de-)serialize <see cref="Item"/> instances.</param>
            public SaveDataBuilder(IItemSerializer itemSerializer)
            {
                this.itemSerializer = itemSerializer;
            }

            /// <summary>
            /// Serialize the mail list of a <see cref="MailService"/> instance.
            /// </summary>
            /// <param name="mailList">The mail list to serialize.</param>
            /// <returns>A serializable format of the mail list.</returns>
            public List<MailSaveData> Construct(Dictionary<int, Dictionary<string, Mail>> mailList)
            {
                var mailSaveDataList = new List<MailSaveData>();

                foreach (KeyValuePair<int, Dictionary<string, Mail>> daysAndMail in mailList)
                {
                    foreach (var mail in daysAndMail.Value.Values)
                    {
                        var mailSaveData = new MailSaveData(mail.Id, mail.Text, daysAndMail.Key);

                        switch (mail)
                        {
                            case ItemMail itemMail:
                                mailSaveData.MailType = MailType.ItemMail;
                                mailSaveData.AttachedItemsSaveData = itemMail.AttachedItems?.Select(item => itemSerializer.Deconstruct(item)).ToList();
                                break;
                            case MoneyMail moneyMail:
                                mailSaveData.MailType = MailType.MoneyMail;
                                mailSaveData.Money = moneyMail.AttachedMoney;
                                mailSaveData.Currency = moneyMail.Currency;
                                break;
                            case RecipeMail recipeMail:
                                mailSaveData.MailType = MailType.RecipeMail;
                                mailSaveData.Recipe = recipeMail.Recipe;
                                break;
                            case QuestMail questMail:
                                mailSaveData.MailType = MailType.QuestMail;
                                mailSaveData.QuestId = questMail.QuestId;
                                mailSaveData.IsAutomaticallyAccepted = questMail.IsAutomaticallyAccepted;
                                break;
                            default:
                                mailSaveData.MailType = MailType.PlainMail;
                                break;
                        }

                        mailSaveDataList.Add(mailSaveData);
                    }
                }

                return mailSaveDataList;
            }

            /// <summary>
            /// Deserialize the mail list of a <see cref="MailService"/> instance.
            /// </summary>
            /// <param name="mailSaveDataList">The serializable format of the mail list.</param>
            /// <returns>The deserialized mail list.</returns>
            public Dictionary<int, Dictionary<string, Mail>> Reconstruct(List<MailSaveData> mailSaveDataList)
            {
                var mailList = new Dictionary<int, Dictionary<string, Mail>>();

                foreach (var mailSaveData in mailSaveDataList)
                {
                    if (!mailList.ContainsKey(mailSaveData.AbsoluteArrivalDay))
                    {
                        mailList[mailSaveData.AbsoluteArrivalDay] = new Dictionary<string, Mail>();
                    }

                    Mail mail = null;
                    switch (mailSaveData.MailType)
                    {
                        case MailType.ItemMail:
                            var attachedItems = mailSaveData.AttachedItemsSaveData.Select(itemSaveData => itemSerializer.Construct(itemSaveData)).ToList();
                            mail = new ItemMail(mailSaveData.Id, mailSaveData.Text, attachedItems);
                            break;
                        case MailType.MoneyMail:
                            mail = new MoneyMail(mailSaveData.Id, mailSaveData.Text, mailSaveData.Money, mailSaveData.Currency);
                            break;
                        case MailType.RecipeMail:
                            mail = new RecipeMail(mailSaveData.Id, mailSaveData.Text, mailSaveData.Recipe);
                            break;
                        case MailType.QuestMail:
                            mail = new QuestMail(mailSaveData.Id, mailSaveData.Text, mailSaveData.QuestId, mailSaveData.IsAutomaticallyAccepted);
                            break;
                        default:
                            mail = new Mail(mailSaveData.Id, mailSaveData.Text);
                            break;

                    }

                    mailList[mailSaveData.AbsoluteArrivalDay].Add(mailSaveData.Id, mail);
                }

                return mailList;
            }
        }

        /// <summary>
        /// The <see cref="SaveData"/> class encapsulates data used by the <see cref="MailService"/> class which needs to 
        /// be written to/read from a game's save file.
        /// </summary>
        private class SaveData
        {
            /// <summary>
            /// Create a new instance of the <see cref="SaveData"/> class.
            /// </summary>
            /// <remarks>
            /// This constructur is used by the used save game serializer.
            /// </remarks>
            public SaveData() { }

            /// <summary>
            /// Create a new instance of the <see cref="SaveData"/> class.
            /// </summary>
            /// <param name="mailSaved">Contains the custom mail for each day.</param>
            public SaveData(List<MailSaveData> mailSaved)
            {
                MailSaved = mailSaved;
            }

            /// <summary>
            /// Contains the custom mail for each day.
            /// </summary>
            public List<MailSaveData> MailSaved { get; set; }
        }
    }
}
