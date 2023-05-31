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
using Force.DeepCloner;
using StardewValley;

namespace StardewArchipelago.Items.Mail
{
    public class Mailman
    {
        private static readonly Random _random = new Random();
        private bool _sendForTomorrow = true;

        private Dictionary<string, string> _lettersGenerated;

        public Mailman(Dictionary<string, string> lettersGenerated)
        {
            _lettersGenerated = lettersGenerated.DeepClone();
            foreach (var (mailKey, mailContent) in lettersGenerated)
            {
                var mailData = Game1.content.Load<Dictionary<string, string>>("Data\\mail");
                mailData[mailKey] = mailContent;
            }
        }

        public void SendVanillaMail(string mailTitle, bool noLetter)
        {
            if (Game1.player.mailReceived.Contains(mailTitle))
            {
                return;
            }

            SendMail(mailTitle + (noLetter ? "%&NL&%" : ""));
        }

        public void SendArchipelagoInvisibleMail(string mailKey, string apItemName, string findingPlayer, string locationName)
        {
            if (Game1.player.hasOrWillReceiveMail(mailKey))
            {
                return;
            }

            GenerateMail(mailKey, apItemName, findingPlayer, locationName, "");
            SendMail(mailKey + "%&NL&%");
        }

        public void SendArchipelagoGiftMail(string mailKey, string findingPlayer, string attachmentEmbedString)
        {
            var mailContentTemplate = GetRandomApMailGiftString();
            var mailContent = string.Format(mailContentTemplate, findingPlayer, attachmentEmbedString);
            GenerateMail(mailKey, mailContent);
            SendMail(mailKey);
        }

        public void SendArchipelagoMail(MailKey mailKey, string apItemName, string findingPlayer, string locationName, string attachmentEmbedString)
        {
            var mailKeyString = mailKey.ToString();
            if (Game1.player.hasOrWillReceiveMail(mailKeyString))
            {
                return;
            }

            GenerateMail(mailKeyString, apItemName, findingPlayer, locationName, attachmentEmbedString);

            SendMail(mailKeyString);
        }

        private void GenerateMail(string mailKey, string apItemName, string findingPlayer, string locationName,
            string embedString)
        {
            var mailContentTemplate = GetRandomApMailString();
            var mailContent = string.Format(mailContentTemplate, apItemName, findingPlayer, locationName, embedString);
            GenerateMail(mailKey, mailContent);
        }

        public void GenerateMail(string mailKey, string mailContent)
        {
            mailContent = mailContent.Replace("<3", "<");
            var mailData = Game1.content.Load<Dictionary<string, string>>("Data\\mail");
            mailData[mailKey] = mailContent;
            if (_lettersGenerated.ContainsKey(mailKey))
            {
                _lettersGenerated[mailKey] = mailContent;
            }
            else
            {
                _lettersGenerated.Add(mailKey, mailContent);
            }
        }

        public int OpenedMailsContainingKey(string apItemName)
        {
            var numberReceived = 0;
            foreach (var mail in Game1.player.mailReceived)
            {
                if (!mail.Contains(MailKey.GetBeginningOfKeyForItem(apItemName)))
                {
                    continue;
                }

                numberReceived++;
            }

            return numberReceived;
        }

        public Dictionary<string, string> GetAllLettersGenerated()
        {
            return _lettersGenerated.DeepClone();
        }

        public void SendToday()
        {
            _sendForTomorrow = false;
        }

        public void SendTomorrow()
        {
            _sendForTomorrow = true;
        }

        private string GetRandomApMailGiftString()
        {
            var chosenString = ApGiftStrings[_random.Next(0, ApGiftStrings.Length)];
            chosenString += "{1}[#]Archipelago Item";
            return chosenString;
        }

        private string GetRandomApMailString()
        {
            var chosenString = ApMailStrings[_random.Next(0, ApMailStrings.Length)];
            chosenString += "{3}[#]Archipelago Item";
            return chosenString;
        }

        public void SendMail(string mailTitle)
        {
            if (_sendForTomorrow)
            {
                Game1.player.mailForTomorrow.Add(mailTitle);
            }
            else
            {
                if (mailTitle.Contains("%&NL&%"))
                {
                    var cleanedTitle = mailTitle.Replace("%&NL&%", "");
                    if (!Game1.player.mailReceived.Contains(cleanedTitle))
                    {
                        Game1.player.mailReceived.Add(cleanedTitle);
                    }
                }
                else
                {
                    Game1.mailbox.Add(mailTitle);
                }
            }
        }

        // 0: Sender
        // 1: Embed
        private static readonly string[] ApGiftStrings = {
            "It's dangerous to go alone. Take this!^^    -{0}",
            "Hopefully, this will convince you to leave the Burger King...^^    -{0}",
            "Here you go!^^    -{0}",
            "I thought you could use this^^    -{0}",
            "I heard you wanted that?^^    -{0}",
        };

        // 0: Item
        // 1: Sender
        // 2: Location
        // 3: Embed
        private static readonly string[] ApMailStrings = {
            "Hey @, I was at {2}, minding my own business, and there I found a {0}.^I thought you would make better use of it than I ever could.^^    -{1}",
            "I found a {0} in {2}.^Enjoy!^^    -{1}",
            "There was a {0} in my {2}.^Do you think you can make it useful?^^    -{1}",
            "Hey @, I was passing by {2} when I found a {0}. You gotta stop leaving your stuff lying around!^^    -{1}",
            "This is an official notice.^You have been charged and found guilty of 1 case of Littering within multiworld bounds.^{1} was able to identify the {0} left near {2} as yours, so it has been returned to you.^Please leave your trash in appropriate receptacles.^^    -APPD",
            "Hello Valued Customer!^At Jojamart, we understand how hard it can be to get the things you need for a price you can pay.^Please take this free* gift, a {0}, as a token of our good faith.^We would greatly appreciate it if you consider your pre-approved credit card offer enclosed.^^*No monetary costs were incurred by Jojamart  during the procurement of this item from {2}. The mental health of the affected employee {1} is classified.^^    -Joja Customer Representative",
            "I was thinking of you today, so when I saw this {0} at {2}, I thought it would make a perfect gift!^^    -{1}",
            "I know, I know, your birthday was a while ago. Sorry this is so late. It took me a while to get this {0}. It was only being sold at {2}, can you believe that?!^^    -{1}",
            "It's dangerous to go alone. Take this! ({0})^^    -{1}",
            "{0}^{2}^It was hard^You're welcome^^    -{1}",
            "You are a cool person. And because you're cool, I went all the way to {2} to grab your {0}.^^    -{1}",
            "You're our 10,000th* Subscriber!^^Please take this {0}, freshly discovered for you at {2}! Click below to claim your prize!^^*Subscriber count may vary. All items are non-refundable. {1} Inc. is not liable for any illness, financial loss or harm to self that occur to the recipient.",
            "Here at Jojamart, we take pride in being aggressively progressive.^We're currently surveying cooperative* individuals on our progressive products.^Please take the time to test and review this {0}. Then contact your designated reviewer {1} at 1-800-867-5309.^^*All employees, current and prior have agreed to this in their contract, lasting in perpetuity.^Unsubscription is expressly forbidden under Article 307 subsection 4546B clause 3-4",
            "The Stardew Valley Tribune is proud to announce our victory in the recent Ferngill Republic Journalism Ceremony, a contest across the entire nation.^This is due in no small part to our article on your farm, @.^As a token of our gratitude, please take this {0}, which was carefully procured from {2}.^Thank you for your continued support!^^    -Agricultural Editor {1}",
            "Status report.^World infiltration completed.^Retrieval of {0} from {2} successful.^Agent {1} sends their regards.^Unit will fall back and wait for further instructions.",
            "i maed you a cukie but i eated it. have this {0} instaed^^    -{1}",
            "Me hat seller, poke. {1} dropped {0} off here. Me giving it to you okay poke? They say it come from {2}.",
            "Declaration of returned goods.^^{0} is being returned to you after seizure by multiworld officers from {1}. Suspect was found near {2} with stolen goods in hand and have been detained for further questioning.^^    -APPD",
            "Statement of Accounts^{1} Credit Union^Overpaid Balance: 1 {0}^See contained check below.^You may cash at your local {2} or go online with our App",
            "Find enclosed your prescription {0}. Remember to always follow the instructions on the bottle.^If you experience nausea, headaches or bouts of desire to go to Burger King, please consult your physician.^^Brought to you by {1} Pharmacies",
            "So, hear me out, I know you wanted a cool sword, but I got you this {0} instead. It was free, found it on the road by {2}. Cheers!^^    -{1}",
            "Hopefully, this {0} will convince you to leave the Burger King...^^    -{1}",
        };
    }
}
