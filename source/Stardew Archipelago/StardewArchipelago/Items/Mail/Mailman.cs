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
using StardewArchipelago.Serialization;
using StardewValley;

namespace StardewArchipelago.Items.Mail
{
    public class Mailman
    {
        private static readonly Random _random = new();
        private bool _sendForTomorrow = true;

        private ArchipelagoStateDto _state;

        public Mailman(ArchipelagoStateDto state)
        {
            _state = state;
            foreach (var (mailKey, mailContent) in _state.LettersGenerated)
            {
                var mailData = DataLoader.Mail(Game1.content);
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

        public void SendArchipelagoGiftMail(string mailKey, string itemName, string senderName, string senderGame, string attachmentEmbedString)
        {
            var mailContentTemplate = GetRandomApMailGiftString();
            var mailContent = string.Format(mailContentTemplate, itemName, senderName, senderGame, attachmentEmbedString);
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
            var mailContent = string.Format(mailContentTemplate, apItemName, findingPlayer, locationName, embedString, Game1.player.farmName.Value);
            GenerateMail(mailKey, mailContent);
        }

        public void GenerateMail(string mailKey, string mailContent)
        {
            mailContent = mailContent.Replace("<3", "<");
            var mailData = DataLoader.Mail(Game1.content);
            mailData[mailKey] = mailContent;
            if (_state.LettersGenerated.ContainsKey(mailKey))
            {
                _state.LettersGenerated[mailKey] = mailContent;
            }
            else
            {
                _state.LettersGenerated.Add(mailKey, mailContent);
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
            chosenString += "{3}[#]Archipelago Gift"; // Argument {3} is the embed
            return chosenString;
        }

        private string GetRandomApMailString()
        {
            var chosenString = ModEntry.Instance.Config.DisableLetterTemplates ?
                ApConciseMailString:
                ApMailStrings[_random.Next(0, ApMailStrings.Length)];
            chosenString += "{3}[#]Archipelago Item"; // Argument {3} is the embed
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

        // 0: Item Name
        // 1: Sender Name
        // 2: Sender Game
        // 3: Embed
        private static readonly string[] ApGiftStrings =
        {
            "It's dangerous to go alone. Take this!^^    -{1} from {2}",
            "Hopefully, this {0} will convince you to leave the Burger King...^^    -{1} from {2}",
            "Here you go!^^    -{1} from {2}",
            "I thought you could use this {0} from {2}^^    -{1}",
            "I heard you wanted a {0}?^I found one in {2}^^    -{1}",
            "Greetings from {2}^^I got a {0} for you!^^    -{1}",
            "I can't carry {0} for you, but I can carry you!^^    -{1} from {2}",
            "May your {0} be laid under an enchantment of surpassing excellence for seven years!^^    -{1} from {2}",
            "There's some good in this world, Mr. @. And it's worth fighting for.^^    -{1} from {2}",
            "Take this {0}. It contains the Light of Earendil, our most beloved star.^^May it be a light to you in dark places, when all other lights go out.^^    -{1} from {2}",
            "You had {0} on the shopping list right? Picked it up for you^^    -{1} from {2}",
            "It's not too late for Christmas in July right? Here, I got you a {0}, I know you have been looking for it.^^    -{1} from {2}",
            "I hope it's summer, cause I got you this {0} for the Luau^^    -{1} from {2}",
            "I hear you need something for you 'secret friend' this winter and boy do I have the perfect item for you: {0}. No need to thank me, I'm sure it's exactly what you needed.^^    -{1} from {2}",
            "You still need one more item for your grange display at the Fair, right? Here's your winner: a freshly caught {0}, straight from the {2} seas.^^    -{1}",
            "I know you've been trying hard to win over a partner for the Flower Dance. This {0} is sure to win over your crush. I just know they'll love it.^^    -{1} from {2}",
        };

        private const string ApConciseMailString = "{0} from {1} at {2}.";

        // 0: Item
        // 1: Sender
        // 2: Location
        // 3: Embed
        // 4: Farm Name
        private readonly string[] ApMailStrings =
        {
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
            "Here, have this letter.^^    -{1}^^^^^^^^^PS: also a {0} k thx bye",
            "I have something here for you. Your father wanted you to have this when you were old enough, but your uncle wouldn't allow it. He feared you  might follow old {1} on some damned-fool idealistic crusade like your father did. Your father's {0}. This is the weapon of a Jedi Knight. An elegant weapon, for a more civilized age.",
            "{1}^Incoming Trade Offer:^^I Receive: Your Undying Love and Affection^^You receive: {0}",
            "Grandpa's {0}^He's gone to stay with the dwarves. He's left you {4}, along with all of his possessions. The {0} is yours now. Keep it secret. Keep it safe",
            "Dear valued customer.^Joja Online is proud to introduce our new line of innovative products! To celebrate, please enjoy this free sample of {1}'s {0}, made with the finest ingredients from local farmers and artisans from {2}! Now available in the Joja Online shop!",
            "Dear valued customer,^We are sorry to hear that your package was lost in transit through Joja Prime. Find enclosed your replacement {0}. {1} will be appropriately disciplined for their negligence.^Have a wonderful day!",
            "Hey, just got back from the Burger King on {2} road.^I got you a {0} so you hopefully don't need to go there.^Enjoy!^^    -{1}",
            "Send me items!^I'll pay handsomely!^Have this {0} as a down payment. I'm expecting great things from you.^^    -{1}",
            "Hey, I was going through Grandpa's stash at {2}, and I found this {0}, along with a bunch of turnips.^Grandpa did always have strange tastes...^^    -{1}",
            "I have no idea what's going on, but I found this {0} with your name on it.^^    -{1}",
            "Hey, Pierre sent me this {0}, but I can't stand the things. So now it's yours.^^    -{1}",
            "I couldn't get Oak's Parcel back from the thieves, but hopefully you can put {0} to good use instead^^    -{1}",
            "You should show this {0} to Mati, I bet she didn't even know it existed!^^    -{1}",
            "Try putting this {0} in the Luau soup!^^    -{1}",
            "I'm entrusting you with this {0}. I know you'll treat it well. See to it that it makes it to the destination^^    -{1}",
            "Hi, I work for one of Joja's competitors, the up and coming {2} Mart! We sell a variety of goods and services. Here, I'd like you to have this {0} as a promotional item! A {0} can be used at any time, so it's even more useful than a Pokemon Center! Have a nice day and please stop by sometime.^^    -{1}",
            "Here take this {0}! It's comfy and easy to wear!^^    -{1}",
            "{0} at {2}?^^Archipelago was a mistake.^^    -{1}",
            "Who ordered the {0} from {2}? Oh, you did.^^    -{1}",
            "I sold my best podracer for this {0}. You're free now^^    -{1}",
            "The {2} animal shelter didn't have any cats, so I got you a {0} out of their dumpster instead.^^    -{1}",
            "{1} went on vacation to {2} and all they came back with was {0}",
            "Hey, have you heard about the legendary power of the {0}? They say if you share it with someone you really care for, it binds you together forever and ever, through eternity.^^    -{1}",
            "The {0} is a tool that can unlock any lock, whether on a door, a chest, or a heart. It is a powerful weapon that chooses it's owner. It did not choose me.^^It chose you.^^    -{1}",
            "You must use the {0} to defeat the darkness",
            "Have you ever heard the story of {0}?  I thought not.  It's not a story {1} would tell you.",
			"OR, hear me out, we just collect random things people say from Discord and compile them into a massive string library so that every letter is interesting and unique so people don't feel annoyed having to read them^^    -Madolinn^^{0} from {1}",
			"I searched for the perfect item to show my feelings for you, and I got you this {0}^^    -Secret Admirer",
			"I got up early to deliver this mail and all you got was a {0}.",
			"I found this {0} in the back of the mail truck, it has been there a few weeks hope it isn't important.",
			"If you look closely, you might just find a {0} hidden in the fine print.",
			"Are you still looking for your Community Center?  Have you checked in Aginah's cave?  In the meantime, have this {0}.",
			"{1} was piloted to find your {0}. That means it is important, right?",
			"{1} was piloted to find your {0}. I think it might have been a mistake.",
            "Hey, at least this isn't an Emblem for Sonic^^    -{1}^^({0})",
            "Hey, at least this isn't a Coin Bundle for DLCQuest^^    -{1}^^({0})",
			"My original letter wasn't safe for streaming, so you're getting this one instead. Here's your {0}.^^    -{1}",
			"Hope you can add this {0} to your power suit like we can here on Zebes^^    -{1}",
			"Don't worry I didn't forget your birthday, I got you {0}, I hope you love it.^^    -{1}",
        };
    }
}
