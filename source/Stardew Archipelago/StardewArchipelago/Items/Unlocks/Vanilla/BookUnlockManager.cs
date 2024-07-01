/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using System.Collections.Generic;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Constants.Vanilla;
using StardewArchipelago.Items.Mail;
using StardewValley;

namespace StardewArchipelago.Items.Unlocks.Vanilla
{
    public class BookUnlockManager : IUnlockManager
    {
        private const string POWER_PREFIX = "Power: ";
        private const string BOOK_PREFIX = "Book: ";

        public BookUnlockManager()
        {
        }

        public void RegisterUnlocks(IDictionary<string, Func<ReceivedItem, LetterAttachment>> unlocks)
        {
            RegisterPowerBooks(unlocks);
            RegisterRealBooks(unlocks);
        }

        private void RegisterPowerBooks(IDictionary<string, Func<ReceivedItem, LetterAttachment>> unlocks)
        {
            foreach (var (name, id) in PowerBooks.BookNamesToIds)
            {
                unlocks.Add($"{POWER_PREFIX}{name}", (x) => SendPowerBookLetter(x, id));
            }
        }

        private void RegisterRealBooks(IDictionary<string, Func<ReceivedItem, LetterAttachment>> unlocks)
        {
            foreach (var (name, id) in PowerBooks.BookNamesToIds)
            {
                unlocks.Add($"{BOOK_PREFIX}{name}", (x) => SendBookLetter(x, id));
            }
        }

        private LetterAttachment SendPowerBookLetter(ReceivedItem receivedItem, string bookId)
        {
            Game1.player.stats.Increment(bookId);
            DelayedAction.functionAfterDelay((Action)(() => Game1.showGlobalMessage(Game1.content.LoadString("Strings\\1_6_Strings:LearnedANewPower"))), 1000);
            if (!Game1.player.mailReceived.Contains("read_a_book"))
            {
                Game1.player.mailReceived.Add("read_a_book");
            }
            Utility.checkForBooksReadAchievement();
            return new LetterInformationAttachment(receivedItem);
        }

        private LetterAttachment SendBookLetter(ReceivedItem receivedItem, string bookId)
        {
            return new LetterItemIdAttachment(receivedItem, bookId);
        }
    }
}
