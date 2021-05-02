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
using System.Text;
using Newtonsoft.Json;
using StardewModdingAPI;
using StardewValley;
using static StardewValley.LocalizedContentManager;

namespace Omegasis.HappyBirthday
{
    public class BirthdayMessages
    {
        /// <summary>The actual birthday wishes given by an npc.</summary>
        public Dictionary<string, string> birthdayWishes;

        public Dictionary<string, string> spouseBirthdayWishes;

        public Dictionary<string, string> defaultSpouseBirthdayWishes = new Dictionary<string, string>()
        {
            ["Alex"] = "",
            ["Elliott"] = "",
            ["Harvey"] = "",
            ["Sam"] = "",
            ["Sebastian"] = "",
            ["Shane"] = "",
            ["Abigail"] = "",
            ["Emily"] = "",
            ["Haley"] = "",
            ["Leah"] = "",
            ["Maru"] = "",
            ["Penny"] = "",
        };

        public Dictionary<string, Func<string, string>> spouseEnglishGeneratedMessages = new Dictionary<string, Func<string, string>>();

        /// <summary>Used to contain birthday wishes should the mod not find any available. These were written myself, Omegasis.</summary>
        public Dictionary<string, string> defaultBirthdayWishesLegacy = new Dictionary<string, string>()
        {
            ["Robin"] = "Hey @, happy birthday! I'm glad you choose this town to move here to. ",
            ["Demetrius"] = "Happy birthday @! Make sure you take some time off today to enjoy yourself. $h",
            ["Maru"] = "Happy birthday @. I tried to make you an everlasting candle for you, but sadly that didn't work out. Maybe next year right? $h",
            ["Sebastian"] = "Happy birthday @. Here's to another year of chilling. ",
            ["Linus"] = "Happy birthday @. Thanks for visiting me even on your birthday. It makes me really happy. ",
            ["Pierre"] = "Hey @, happy birthday! Hopefully this next year for you will be a great one! ",
            ["Caroline"] = "Happy birthday @. Thank you for all that you've done for our community. I'm sure your parents must be proud of you.$h",
            ["Abigail"] = "Happy birthday @! Hopefully this year we can go on even more adventures together $h!",
            ["Alex"] = "Yo @, happy birthday! Maybe this will be your best year yet.$h",
            ["George"] = "When you get to my age birthdays come and go. Still happy birthday @.",
            ["Evelyn"] = "Happy birthday @. You have grown up to be such a fine individual and I'm sure you'll continue to grow. ",
            ["Lewis"] = "Happy birthday @! I'm thankful for what you have done for the town and I'm sure your grandfather would be proud of you.",
            ["Clint"] = "Hey happy birthday @. I'm sure this year is going to be great for you.",
            ["Penny"] = "Happy birthday @. May you enjoy all of life's blessings this year. ",
            ["Pam"] = "Happy birthday kid. We should have a drink to celebrate another year of life for you! $h",
            ["Emily"] = "I'm sensing a strong positive life energy about you, so it must be your birthday. Happy birthday @!$h",
            ["Haley"] = "Happy birthday @. Hopefully this year you'll get some good presents!$h",
            ["Jas"] = "Happy birthday @. I hope you have a good birthday.",
            ["Vincent"] = "Hey @ have you come to pl...oh it's your birthday? Happy birthday! ",
            ["Jodi"] = "Hello there @. Rumor has it that today is your birthday. In that case, happy birthday!$h",
            ["Kent"] = "Jodi told me that it was your birthday today @. Happy birthday and make sure to cherish every single day.",
            ["Sam"] = "Yo @ happy birthday! We'll have to have a birthday jam session for you some time!$h ",
            ["Leah"] = "Hey @ happy birthday! We should go to the saloon tonight and celebrate!$h ",
            ["Shane"] = "Happy birthday @. Keep working hard and I'm sure this next year for you will be a great one.",
            ["Marnie"] = "Hello there @. Everyone is talking about your birthday today and I wanted to make sure that I wished you a happy birthday as well, so happy birthday! $h ",
            ["Elliott"] = "What a wonderful day isn't it @? Especially since today is your birthday. I tried to make you a poem but I feel like the best way of putting it is simply, happy birthday. $h ",
            ["Gus"] = "Hey @ happy birthday! Hopefully you enjoy the rest of the day and make sure you aren't a stranger at the saloon!",
            ["Dwarf"] = "Happy birthday @. I hope that what I got you is acceptable for humans as well. ",
            ["Wizard"] = "The spirits told me that today is your birthday. In that case happy birthday @. May your year shine bright! ",
            ["Harvey"] = "Hey @, happy birthday! Make sure to come in for a checkup some time to make sure you live many more years! ",
            ["Sandy"] = "Hello there @. I heard that today was your birthday and I didn't want you feeling left out, so happy birthday!",
            ["Willy"] = "Aye @ happy birthday. Looking at you reminds me of ye days when I was just a guppy swimming out to sea. Continue to enjoy them youngin.$h",
            ["Krobus"] = "I have heard that it is tradition to give a gift to others on their birthday. In that case, happy birthday @."
        };

        /// <summary>Used to contain birthday wishes should the mod not find any available. These were written nexus mods user cerreli.</summary>
        public Dictionary<string, string> defaultBirthdayWishes = new Dictionary<string, string>()
        {

            ["Robin"] = "Hey, @, happy birthday! I'm really glad you decided to move to the valley. ",
            ["Demetrius"] = "Happy birthday, @! Make sure you take some time off today to enjoy yourself. $h",
            ["Maru"] = "Happy birthday, @. I tried to make an everlasting candle for you, but sadly that didn't work out. Maybe next year, right? $h",
            ["Sebastian"] = "Happy birthday, @. Hope things are going well down at the farm.",
            ["Linus"] = "Happy birthday, @. Thanks for visiting me even on your birthday. It makes me really happy. ",
            ["Pierre"] = "Hey @, happy birthday! Hopefully this next year will be a great one for you! ",
            ["Caroline"] = "Happy birthday, @. Thank you for all that you've done for our community. I'm sure your parents must be proud of you.$h",
            ["Abigail"] = "Happy birthday, @! Hopefully this year we can go on even more adventures together $h!",
            ["Alex"] = "Yo @, happy birthday! Here's to making this the best year yet!$h",
            ["George"] = "When you get to my age, birthdays start to come and go. Still, happy birthday, @.",
            ["Evelyn"] = "Happy birthday, @. You have grown into such a fine individual, and I'm sure your grandfather would be proud to see who you've become. ",
            ["Lewis"] = "Happy birthday, @! I really appreciate everything you've done for the town. Keep up the good work.",
            ["Clint"] = "Hey, happy birthday, @. I'm sure this year is going to be great for you.",
            ["Penny"] = "Happy birthday, @. May you enjoy all of life's blessings this year. ",
            ["Pam"] = "Happy birthday, kid. We should have a drink to celebrate another year of life for you! $h",
            ["Emily"] = "Happy birthday, @! I can see your future shining bright.$h",
            ["Haley"] = "Happy birthday, @. Hopefully this year you'll get some good presents!$h",
            ["Jas"] = "Happy birthday, @. I'm glad I decided to get to know you.",
            ["Vincent"] = "Hey, @, have you come to pl... oh, it's your birthday? Happy birthday! ",
            ["Jodi"] = "Hello there, @. Rumor has it that today is your birthday. In that case, happy birthday!$h",
            ["Kent"] = "Jodi told me that it was your birthday today, @. Happy birthday, and make sure to cherish every single day.",
            ["Sam"] = "Hey @, happy birthday! We'll have to have a birthday jam session for you sometime!$h ",
            ["Leah"] = "Hey @, happy birthday! I'm glad to see you're doing well on the farm.$h ",
            ["Shane"] = "Happy birthday, @. Keep working hard, and I'm sure this next year will be a great one for you.",
            ["Marnie"] = "Hi, @! Everyone is talking about your birthday today, and I wanted to make sure that I wished you a happy birthday as well. So, happy birthday! $h ",
            ["Elliott"] = "What a wonderful day, isn't it, @? Though I'm sure it's even lovelier to you, it being your birthday!$h#$b#I was actually in the middle of writing a poem for the occasion, but a 'happy birthday' will have to suffice for now.",
            ["Gus"] = "Hey, @, happy birthday! Hopefully you enjoy the rest of the day, and make sure you aren't a stranger at the saloon!",
            ["Dwarf"] = "Happy birthday, @. Wait, humans celebrate those, right?",
            ["Wizard"] = "Happy birthday, @. Never forget that you alone make your future.#$e#How did I know? Ah, I overheard a few of the locals conferring on what gifts to give you. I think you'll appreciate them.",
            ["Harvey"] = "Happy birthday, @. Come in for a checkup sometime, alright? I want to make sure you'll see plenty more.",
            ["Sandy"] = "Aww, sweetie, you came all the way out here to see me on your birthday?$h#$b#Well, then, the least I can do is wish you a happy one!~",
            ["Willy"] = "Ahoy, @, happy birthday. Looking at you reminds me of the days when I was just a guppy myself. $bEnjoy yours while you can, young'un.$h",
            ["Krobus"] = "Happy birthday, @. I still don't quite understand humans, but I'm glad to have met you all the same."

        };

        public BirthdayMessages()
        {
            this.spouseEnglishGeneratedMessages.Add("Alex", this.generateSpouseMessage);
            this.spouseEnglishGeneratedMessages.Add("Elliott", this.generateSpouseMessage);
            this.spouseEnglishGeneratedMessages.Add("Harvey", this.generateSpouseMessage);
            this.spouseEnglishGeneratedMessages.Add("Sam", this.generateSpouseMessage);
            this.spouseEnglishGeneratedMessages.Add("Sebastian", this.generateSpouseMessage);
            this.spouseEnglishGeneratedMessages.Add("Shane", this.generateSpouseMessage);
            this.spouseEnglishGeneratedMessages.Add("Abigail", this.generateSpouseMessage);
            this.spouseEnglishGeneratedMessages.Add("Emily", this.generateSpouseMessage);
            this.spouseEnglishGeneratedMessages.Add("Haley", this.generateSpouseMessage);
            this.spouseEnglishGeneratedMessages.Add("Leah", this.generateSpouseMessage);
            this.spouseEnglishGeneratedMessages.Add("Maru", this.generateSpouseMessage);
            this.spouseEnglishGeneratedMessages.Add("Penny", this.generateSpouseMessage);
            //HappyBirthday.Config.translationInfo.setTranslationFromLanguageCode(Game1.content.GetCurrentLanguage());
            this.createBirthdayGreetings();
            this.loadTranslationStrings();
        }


        public Dictionary<StardewValley.LocalizedContentManager.LanguageCode, Dictionary<string, string>> translatedStrings = new Dictionary<StardewValley.LocalizedContentManager.LanguageCode, Dictionary<string, string>>()
        {

            [StardewValley.LocalizedContentManager.LanguageCode.en] = new Dictionary<string, string>()
            {
                ["Mail:birthdayMom"] = "Dear @,^  Happy birthday sweetheart. It's been amazing watching you grow into the kind, hard working person that I've always dreamed that you would become. I hope you continue to make many more fond memories with the ones you love. ^  Love, Mom ^ P.S. Here's a little something that I made for you. %item object 221 1 %%",
                ["Mail:birthdayDad"] = "Dear @,^  Happy birthday kiddo. It's been a little quiet around here on your birthday since you aren't around, but your mother and I know that you are making both your grandpa and us proud.  We both know that living on your own can be tough but we believe in you one hundred percent, just keep following your dreams.^  Love, Dad ^ P.S. Here's some spending money to help you out on the farm. Good luck! %item money 5000 5001 %%",
                ["Mail:birthdayJunimos"] = "Please come to the community center. ^ Sincerly,^      -The Junimos",
                ["Mail:birthdayDatingPenny"] = "Dear @. ^ My mom and I decided to have a little birthday party for you. Could you come by sometime today?^Sincerly,^      -Penny",
                ["Mail:birthdayDatingMaru"] = "Hey @. ^ My family and I decided to have a birthday party for you at our place. Could you come by our house sometime today?^Sincerly,^      -Maru",
                ["Mail:birthdayDatingSebastian"] = "Hey @. ^ Could you come by my place later? My family and I decided to have a birthday party for you to celebrate. ^^      -Sebastian",
                ["Mail:birthdayDatingLeah"] = "Hey @. ^ Could you come by my place later? I thought it would be nice if we had a small party for you. ^Sincerly,^      -Leah",
                ["Mail:birthdayDatingAbigail"] = "Hey @. ^ My family and I decided to have a birthday party for you at our place. Could you come by our house sometime today?^Sincerly,^      -Abigail",
                ["Mail:birthdayDatingAbigail_Wednesday"] = "Hey @. ^ I thought it would be fun if we had a small party for you today. Could you come by the mines later?^ Sincerly,^      -Abigail",
                ["Mail:birthdayDatingEmily"] = "Hi @. ^ I thought it would be nice to have a birthday party for you at our place. Could you come by my house sometime today?^Sincerly,^      -Emily",
                ["Mail:birthdayDatingHaley"] = "Hey @. ^ I thought it would be nice to have a birthday party for you at our place. Could you come by my house sometime today?^Sincerly,^      -Haley",
                ["Mail:birthdayDatingHarvey"] = "Hey @. ^ I thought it would be nice to have a birthday party for you at my place. Could you come by my room on the second floor of the clinic sometime today?^Sincerly,^      -Harvey",
                ["Mail:birthdayDatingElliott"] = "Hello @. ^ I thought it would be nice to have a birthday party for you at my place. Could you come by my humble home later today?^Sincerly,^      -Elloitt",
                ["Mail:birthdayDatingSam"] = "Hey @. ^ I thought it would be fun to have a birthday party for you at our place. Could you come by my house sometime today?^^      -Sam",
                ["Mail:birthdayDatingAlex"] = "Hey @. ^ I thought it would be fun to have a birthday party for you at our place. Could you come by my house sometime today?^^      -Alex",
                ["Mail:birthdayDatingShane"] = "Hey @. ^Could you come by my house sometime today? I thought I'd have a little get together for you.^^      -Alex",

                ["Happy Birthday: Star Message"] = "It's your birthday today! Happy birthday!",
                ["Happy Birthday: Farmhand Birthday Message"] = "It's @'s birthday! Happy birthday to them!",
                ["Season"] = "Season",
                ["Date"] = "Date",
                ["BirthdayError_FestivalDay"]="You can't have your birthday on a festival day. Sorry!",
                ["FavoriteGift"] = "Favorite Gift",

                ["Event:JunimoBirthdayParty_0"] = "It looks like the junimos wanted to throw you a party!",
                ["Event:JunimoBirthdayParty_1"] = "It looks like there was some cake left over too!",

                ["Event:DatingPennyBirthday_Pam:0"] = "Come on in kid. The party has just begun!$h",
                ["Event:DatingPennyBirthday_Pam:1"] = "Here, pull up a seat and have a beer to celebrate!",
                ["Event:DatingPennyBirthday_Pam:2"] = "Alright, cheers kid! Happy birthday and here is to another great year! $h",
                ["Event:DatingPennyBirthday_Penny:0"] = "Oh, @ you are here just in time!$h",
                ["Event:DatingPennyBirthday_Penny:1"] = "I thought it would be nice if we threw you a small party. Granted it's not much but I hope you like it. $l",
                ["Event:DatingPennyBirthday_Penny:2"] = "Mom!$a",
                ["Event:DatingPennyBirthday_Penny:3"] = "*sigh* Well make yourself at home. I'll get the cake out.",
                ["Event:DatingPennyBirthday_Penny:4"] = "Happy birthday @. Here is hoping we get to spend many more birthdays together. $l",
                ["Event:DatingPennyBirthday_Finish:0"] = "It was nice celebrating my birthday with Pam and Penny.",
                ["Event:DatingPennyBirthday_Finish:1"] = "Looks like there was some leftover cake and beer too!",

                ["Event:DatingMaruBirthday_Demetrius:0"] = "Welcome @, come in and make yourself at home.$h",
                ["Event:DatingMaruBirthday_Demetrius:1"] = "I agree. I think this party is perfecty wonderful. Besides studies show that your productivity is boosted when you have fun once in a while.$h",
                ["Event:DatingMaruBirthday_Maru:0"] = "Ohh @, you are just in time for the party.$h",
                ["Event:DatingMaruBirthday_Maru:1"] = "I tried to build you a robot to help you out on your farm as a gift but I ran out of time. Hopefully I'll have it done by next year.",
                ["Event:DatingMaruBirthday_Maru:2"] = "Alright, @ make a wish!",
                ["Event:DatingMaruBirthday_Maru:3"] = "Happy birthday @. Hopefully this is just the beginning of many more years to come.$l",
                ["Event:DatingMaruBirthday_Robin:0"] = "You know I have to agree. I think it's the thought that counts not necessarily the gift.$h",
                ["Event:DatingMaruBirthday_Robin:1"] = "Oh I think the cake is ready!",
                ["Event:DatingMaruBirthday_Sebastian:0"] = "You know I think just having this party is good enough. No need to go overboard.",
                ["Event:DatingMaruBirthday_Sebastian:1"] = "Sweet, let's eat.",
                ["Event:DatingMaruBirthday_Finish:0"] = "It was nice celebrating my birthday with Maru and her family.",
                ["Event:DatingMaruBirthday_Finish:1"] = "It looks like there was some leftover cake too!",

                ["Event:DatingSebastianBirthday_Demetrius:0"] = "I agree. I think this party is perfecty wonderful. Besides studies show that your productivity is boosted when you have fun once in a while.$h",
                ["Event:DatingSebastianBirthday_Demetrius:1"] = "Alright, @ go ahead and make a wish!$h",
                ["Event:DatingSebastianBirthday_Maru:0"] = "Happy birthday @. Honestly, I'm a bit surprised that we are having this party. Sebastian never been too keen on celebrations.",
                ["Event:DatingSebastianBirthday_Maru:1"] = "Oh sweet we finally get to have some cake! Let me get you the first slice @!$h",
                ["Event:DatingSebastianBirthday_Robin:0"] = "Welcome @, come in and make yourself at home.$h",
                ["Event:DatingSebastianBirthday_Robin:1"] = "Hey now, I think that it's great that Sebastian wanted to have a party for @. $h",
                ["Event:DatingSebastianBirthday_Robin:2"] = "Oh I think the cake is ready!",
                ["Event:DatingSebastianBirthday_Sebastian:0"] = "Hey @ you are here just in time. Mom is just finishing the cake right now. $h",
                ["Event:DatingSebastianBirthday_Sebastian:1"] = "Honestly... and they wonder why I don't do stuff like this more often.",
                ["Event:DatingSebastianBirthday_Sebastian:2"] = "Happy Birthday @. I'm glad we got to spend time like this together. $h",
                ["Event:DatingSebastianBirthday_Finish:0"] = "It was nice celebrating my birthday with Sebastian and his family.",
                ["Event:DatingSebastianBirthday_Finish:1"] = "It looks like there was some leftover cake too!",

                ["Event:DatingLeahBirthday_Leah:0"] = "Welcome @! Come in and make yourself at home!$h",
                ["Event:DatingLeahBirthday_Leah:1"] = "I knew today was your birthday so I thought we would have a little celebration for you. I event got some cake for us.$h",
                ["Event:DatingLeahBirthday_Leah:2"] = "You know I haven't celebrated a birthday with anyone in a few years, but somehow I felt like I really wanted to spend today with you.$l",
                ["Event:DatingLeahBirthday_Leah:3"] = "I would have never though I'd change my mind about celebrations like this but I guess being with you has really opened me up again.$l",
                ["Event:DatingLeahBirthday_Leah:4"] = "Anyways enough of this embarrasing talk. Happy birthday @. Now shall we have some cake?$h",
                ["Event:DatingLeahBirthday_Finish:0"] = "It was nice celebrating my birthday with just Leah.",
                ["Event:DatingLeahBirthday_Finish:1"] = "It looks like there was some leftover cake too!",

                ["Event:DatingAbigailBirthday_Abigail:0"] = "Hey @! You are just in time for the party!$h",
                ["Event:DatingAbigailBirthday_Abigail:1"] = "Wow this cake looks delicious mom!$h",
                ["Event:DatingAbigailBirthday_Abigail:2"] = "Go ahead and make a wish @.",
                ["Event:DatingAbigailBirthday_Abigail:3"] = "So @, what did you wish for? Hopefully for some awesome skills!",
                ["Event:DatingAbigailBirthday_Abigail:4"] = "Anyways, happy birthday! I hope we get to have many more adventures to come. $l",
                ["Event:DatingAbigailBirthday_Caroline:0"] = "Please, I'm sure your customers won't mind if you are gone for a few minutes. Anyways the cake is done.",
                ["Event:DatingAbigailBirthday_Caroline:1"] = "*sigh* Honestly.",
                ["Event:DatingAbigailBirthday_Caroline:2"] = "Well @ feel free to help yourself to as much as you like.$h",
                ["Event:DatingAbigailBirthday_Pierre:0"] = "Make yourself at home @! I can't stay too long since I have to attend to the store but I thought a party would be a good idea!$h",
                ["Event:DatingAbigailBirthday_Pierre:1"] = "Well of course it would be. It was made from the *highest* quality ingredients from the store! $h",
                ["Event:DatingAbigailBirthday_Finish:0"] = "It was nice celebrating my birthday with Abigail and her family.",
                ["Event:DatingAbigailBirthday_Finish:1"] = "It looks like there was some leftover cake too!",

                ["Event:DatingAbigailBirthday_Mine_Abigail:0"] = "Hey @! You are just in time for the party. Well more like adventure!$h",
                ["Event:DatingAbigailBirthday_Mine_Abigail:1"] = "I thought it would be fun if we spent some time together in the mine for your birthday. Nothing says an exciting birthday like some adventure right?$h",
                ["Event:DatingAbigailBirthday_Mine_Abigail:2"] = "Don't worry I wasn't saying that we go fight monsters. I just thought a change of location could be exciting. Plus my house is closed today since my dad takes the day off on Wednesdays.",
                ["Event:DatingAbigailBirthday_Mine_Abigail:3"] = "I'm just happy that we get to spend time together like this. I even brought some cake for us to make it a proper celebration. Go ahead and make a wish!",
                ["Event:DatingAbigailBirthday_Mine_Abigail:4"] = "Anyways, happy birthday @! I hope we get to have many more adventures to come. $l",
                ["Event:DatingAbigailBirthday_Mine_Finish:0"] = "It was nice celebrating my birthday with just Abigail.",
                ["Event:DatingAbigailBirthday_Mine_Finish:1"] = "It looks like there was some leftover cake too!",

                ["Event:DatingEmilyBirthday_Emily:0"] = "Hi, @ you are just in time for the celebration.$h",
                ["Event:DatingEmilyBirthday_Emily:1"] = "I thought it would be nice to have a small party in your honor.",
                ["Event:DatingEmilyBirthday_Emily:2"] = "Somehow I feel like birthdays are an important reminder for us to live in the moment and live with the harmony of nature.",
                ["Event:DatingEmilyBirthday_Emily:3"] = "Anyways I made us some chocolate cake! It's made with all natural ingredients and sweetened with cactus syrup!$h",
                ["Event:DatingEmilyBirthday_Emily:4"] = "Make a wish @!$h",
                ["Event:DatingEmilyBirthday_Emily:5"] = "Happy birthday @! I feel a strong fortune energy coming from the spirits for you this year. $l",
                ["Event:DatingEmilyBirthday_Finish:0"] = "It was nice celebrating my birthday with just Emily.",
                ["Event:DatingEmilyBirthday_Finish:1"] = "It looks like there was some leftover cake too!",

                ["Event:DatingHaleyBirthday_Haley:0"] = "Hey, @ you are just in time for the party!$h",
                ["Event:DatingHaleyBirthday_Haley:1"] = "Well I mean it would be a party, but I thought it would be nice to have something with just the two of us. $l",
                ["Event:DatingHaleyBirthday_Haley:2"] = "I think birthdays can be pretty fun when you do them right, but I wanted this to be special because... you are special you know?$l",
                ["Event:DatingHaleyBirthday_Haley:3"] = "Anyways, I got us some cake to eat too! Make sure to make a wish alright?$h",
                ["Event:DatingHaleyBirthday_Haley:4"] = "Happy Birthday @. I'm so glad we got to spend some time together. Let's take a picture to remember this moment. $l",
                ["Event:DatingHaleyBirthday_Finish:0"] = "It was nice celebrating my birthday with just Haley.",
                ["Event:DatingHaleyBirthday_Finish:1"] = "It looks like there was some leftover cake too!",

                ["Event:DatingHarveyBirthday_Harvey:0"] = "Hi there, @ you are just in time! Let me clean up a bit.$h",
                ["Event:DatingHarveyBirthday_Harvey:1"] = "I though it would be nice to have a small celebration for you so I cooked us something special.$l",
                ["Event:DatingHarveyBirthday_Harvey:2"] = "Don't worry about the wine. A little moderation is needed but this is a special day!$h",
                ["Event:DatingHarveyBirthday_Harvey:3"] = "Now cheers! Happy Birthday, here is to good health and many more years to come!$l",
                ["Event:DatingHarveyBirthday_Finish:0"] = "It was nice celebrating my birthday with just Harvey.",
                ["Event:DatingHarveyBirthday_Finish:1"] = "It looks like there was some leftover food too!",

                ["Event:DatingElliottBirthday_Elliott:0"] = "Ahh @, wonderful! You are just in time! $h",
                ["Event:DatingElliottBirthday_Elliott:1"] = "I thought it would be splendid if we had a small celebration together in honor of your birthday!$h",
                ["Event:DatingElliottBirthday_Elliott:2"] = "I know that birthdays can be quite an ordeal and I'm sure you are quite busy today, but I do appreciate you coming out all this way to spend your special day with me. $l",
                ["Event:DatingElliottBirthday_Elliott:3"] = "Alas, I tried writing something for you to express my affection for you but somehow words just couldn't seem to capture every little detail.$l",
                ["Event:DatingElliottBirthday_Elliott:4"] = "Alright now how about we have some cake to mark the occassion. Go ahead and make a wish!",
                ["Event:DatingElliottBirthday_Elliott:5"] = "Happy birthday @! Here is hoping there are many more stories to be told with your life! $l",
                ["Event:DatingElliottBirthday_Finish:0"] = "It was nice celebrating my birthday with just Elliott.",
                ["Event:DatingElliottBirthday_Finish:1"] = "It looks like there was some leftover cake too!",

                ["Event:DatingSamBirthday_Sam:0"] = "Hey @ you are just in time. $h",
                ["Event:DatingSamBirthday_Sam:1"] = "I thought it would be fun to have a small party for you since it's your birthday. Don't worry, my family is busy doing other things so we can just chill. $h",
                ["Event:DatingSamBirthday_Sam:2"] = "I got us some pizza and some joja cola. I hope you like it! I would have made you a cake but I never really figured out how to do that sort of thing.$h",
                ["Event:DatingSamBirthday_Sam:3"] = "Anyways happy birthday @. I'm super lucky to be with someone like you. $l",
                ["Event:DatingSamBirthday_Finish:0"] = "It was nice celebrating my birthday with just Sam.",
                ["Event:DatingSamBirthday_Finish:1"] = "It looks like there was some leftover food too!",


                ["Event:DatingAlexBirthday_Alex:0"] = "Hey @ you are just in time. $h",
                ["Event:DatingAlexBirthday_Alex:1"] = "I thought it would be fun to have a small party for you since it's your birthday. It's always a good idea to relax after working out so hard! $h",
                ["Event:DatingAlexBirthday_Alex:2"] = "I got us some pizza and some joja cola. I hope you enjoy it! .$h",
                ["Event:DatingAlexBirthday_Alex:3"] = "Happy birthday @. I'm super lucky to be with someone like you. $l",
                ["Event:DatingAlexBirthday_Finish:0"] = "It was nice celebrating my birthday with just Alex.",
                ["Event:DatingAlexBirthday_Finish:1"] = "It looks like there was some leftover food too!",


                ["Event:DatingShaneBirthday_Shane:0"] = "Hey @ you are just in time. $h",
                ["Event:DatingShaneBirthday_Shane:1"] = "I thought it would be good to have a party for you. $h",
                ["Event:DatingShaneBirthday_Shane:2"] = "I got us some pizza and some joja cola. I hope you enjoy it! I would have cooked you something like a cake with eggs from the chickens but I wasn't too sure how to do it.$h",
                ["Event:DatingShaneBirthday_Shane:3"] = "Anyways, happy birthday @. I'm glad to be with someone like you. $l",
                ["Event:DatingShaneBirthday_Finish:0"] = "It was nice celebrating my birthday with just Shane.",
                ["Event:DatingShaneBirthday_Finish:1"] = "It looks like there was some leftover food too!",

                ["Event:PartyOver"] = "That was a fun party. Back to work!"

            },
            [StardewValley.LocalizedContentManager.LanguageCode.ja] = new Dictionary<string, string>()
            {
                ["Mail:birthdayMom"] = "",
                ["Mail:birthdayDad"] = "",
                ["Mail:birthdayJunimos"] = "",
                ["Mail:birthdayDatingPenny"] = "",
                ["Mail:birthdayDatingMaru"] = "",
                ["Mail:birthdayDatingSebastian"] = "",
                ["Mail:birthdayDatingLeah"] = "",
                ["Mail:birthdayDatingAbigail"] = "",
                ["Mail:birthdayDatingAbigail_Wednesday"] = "",
                ["Mail:birthdayDatingEmily"] = "",
                ["Mail:birthdayDatingHaley"] = "",
                ["Mail:birthdayDatingHarvey"] = "",
                ["Mail:birthdayDatingElliott"] = "",
                ["Mail:birthdayDatingSam"] = "",
                ["Mail:birthdayDatingAlex"] = "",
                ["Mail:birthdayDatingShane"] = "",

                ["Happy Birthday: Star Message"] = "",
                ["Happy Birthday: Farmhand Birthday Message"] = "",
                ["Season"] = "Season",
                ["Date"] = "Date",
                ["BirthdayError_FestivalDay"] = "",
                ["FavoriteGift"] = "",


                ["Event:JunimoBirthdayParty_0"] = "",
                ["Event:JunimoBirthdayParty_1"] = "",

                ["Event:DatingPennyBirthday_Pam:0"] = "",
                ["Event:DatingPennyBirthday_Pam:1"] = "",
                ["Event:DatingPennyBirthday_Pam:2"] = "",
                ["Event:DatingPennyBirthday_Penny:0"] = "",
                ["Event:DatingPennyBirthday_Penny:1"] = "",
                ["Event:DatingPennyBirthday_Penny:2"] = "",
                ["Event:DatingPennyBirthday_Penny:3"] = "",
                ["Event:DatingPennyBirthday_Penny:4"] = "",
                ["Event:DatingPennyBirthday_Finish:0"] = "",
                ["Event:DatingPennyBirthday_Finish:1"] = "",

                ["Event:DatingMaruBirthday_Demetrius:0"] = "",
                ["Event:DatingMaruBirthday_Demetrius:1"] = "",
                ["Event:DatingMaruBirthday_Maru:0"] = "",
                ["Event:DatingMaruBirthday_Maru:1"] = "",
                ["Event:DatingMaruBirthday_Maru:2"] = "",
                ["Event:DatingMaruBirthday_Maru:3"] = "",
                ["Event:DatingMaruBirthday_Robin:0"] = "",
                ["Event:DatingMaruBirthday_Robin:1"] = "",
                ["Event:DatingMaruBirthday_Sebastian:0"] = "",
                ["Event:DatingMaruBirthday_Sebastian:1"] = "",
                ["Event:DatingMaruBirthday_Finish:0"] = "",
                ["Event:DatingMaruBirthday_Finish:1"] = "",

                ["Event:DatingSebastianBirthday_Demetrius:0"] = "",
                ["Event:DatingSebastianBirthday_Demetrius:1"] = "",
                ["Event:DatingSebastianBirthday_Maru:0"] = "",
                ["Event:DatingSebastianBirthday_Maru:1"] = "",
                ["Event:DatingSebastianBirthday_Robin:0"] = "",
                ["Event:DatingSebastianBirthday_Robin:1"] = "",
                ["Event:DatingSebastianBirthday_Robin:2"] = "",
                ["Event:DatingSebastianBirthday_Sebastian:0"] = "",
                ["Event:DatingSebastianBirthday_Sebastian:1"] = "",
                ["Event:DatingSebastianBirthday_Sebastian:2"] = "",
                ["Event:DatingSebastianBirthday_Finish:0"] = "",
                ["Event:DatingSebastianBirthday_Finish:1"] = "",

                ["Event:DatingLeahBirthday_Leah:0"] = "",
                ["Event:DatingLeahBirthday_Leah:1"] = "",
                ["Event:DatingLeahBirthday_Leah:2"] = "",
                ["Event:DatingLeahBirthday_Leah:3"] = "",
                ["Event:DatingLeahBirthday_Leah:4"] = "",
                ["Event:DatingLeahBirthday_Finish:0"] = "",
                ["Event:DatingLeahBirthday_Finish:1"] = "",

                ["Event:DatingAbigailBirthday_Abigail:0"] = "",
                ["Event:DatingAbigailBirthday_Abigail:1"] = "",
                ["Event:DatingAbigailBirthday_Abigail:2"] = "",
                ["Event:DatingAbigailBirthday_Abigail:3"] = "",
                ["Event:DatingAbigailBirthday_Abigail:4"] = "",
                ["Event:DatingAbigailBirthday_Caroline:0"] = "",
                ["Event:DatingAbigailBirthday_Caroline:1"] = "",
                ["Event:DatingAbigailBirthday_Caroline:2"] = "",
                ["Event:DatingAbigailBirthday_Pierre:0"] = "",
                ["Event:DatingAbigailBirthday_Pierre:1"] = "",
                ["Event:DatingAbigailBirthday_Finish:0"] = "",
                ["Event:DatingAbigailBirthday_Finish:1"] = "",

                ["Event:DatingAbigailBirthday_Mine_Abigail:0"] = "",
                ["Event:DatingAbigailBirthday_Mine_Abigail:1"] = "",
                ["Event:DatingAbigailBirthday_Mine_Abigail:2"] = "",
                ["Event:DatingAbigailBirthday_Mine_Abigail:3"] = "",
                ["Event:DatingAbigailBirthday_Mine_Abigail:4"] = "",
                ["Event:DatingAbigailBirthday_Mine_Finish:0"] = "",
                ["Event:DatingAbigailBirthday_Mine_Finish:1"] = "",

                ["Event:DatingEmilyBirthday_Emily:0"] = "",
                ["Event:DatingEmilyBirthday_Emily:1"] = "",
                ["Event:DatingEmilyBirthday_Emily:2"] = "",
                ["Event:DatingEmilyBirthday_Emily:3"] = "",
                ["Event:DatingEmilyBirthday_Emily:4"] = "",
                ["Event:DatingEmilyBirthday_Emily:5"] = "",
                ["Event:DatingEmilyBirthday_Finish:0"] = "",
                ["Event:DatingEmilyBirthday_Finish:1"] = "",

                ["Event:DatingHaleyBirthday_Haley:0"] = "",
                ["Event:DatingHaleyBirthday_Haley:1"] = "",
                ["Event:DatingHaleyBirthday_Haley:2"] = "",
                ["Event:DatingHaleyBirthday_Haley:3"] = "",
                ["Event:DatingHaleyBirthday_Haley:4"] = "",
                ["Event:DatingHaleyBirthday_Finish:0"] = "",
                ["Event:DatingHaleyBirthday_Finish:1"] = "",

                ["Event:DatingHarveyBirthday_Harvey:0"] = "",
                ["Event:DatingHarveyBirthday_Harvey:1"] = "",
                ["Event:DatingHarveyBirthday_Harvey:2"] = "",
                ["Event:DatingHarveyBirthday_Harvey:3"] = "",
                ["Event:DatingHarveyBirthday_Finish:0"] = "",
                ["Event:DatingHarveyBirthday_Finish:1"] = "",

                ["Event:DatingElliottBirthday_Elliott:0"] = "",
                ["Event:DatingElliottBirthday_Elliott:1"] = "",
                ["Event:DatingElliottBirthday_Elliott:2"] = "",
                ["Event:DatingElliottBirthday_Elliott:3"] = "",
                ["Event:DatingElliottBirthday_Elliott:4"] = "",
                ["Event:DatingElliottBirthday_Elliott:5"] = "",
                ["Event:DatingElliottBirthday_Finish:0"] = "",
                ["Event:DatingElliottBirthday_Finish:1"] = "",

                ["Event:DatingSamBirthday_Sam:0"] = "",
                ["Event:DatingSamBirthday_Sam:1"] = "",
                ["Event:DatingSamBirthday_Sam:2"] = "",
                ["Event:DatingSamBirthday_Sam:3"] = "",
                ["Event:DatingSamBirthday_Finish:0"] = "",
                ["Event:DatingSamBirthday_Finish:1"] = "",


                ["Event:DatingAlexBirthday_Alex:0"] = "",
                ["Event:DatingAlexBirthday_Alex:1"] = "",
                ["Event:DatingAlexBirthday_Alex:2"] = "",
                ["Event:DatingAlexBirthday_Alex:3"] = "",
                ["Event:DatingAlexBirthday_Finish:0"] = "",
                ["Event:DatingAlexBirthday_Finish:1"] = "",


                ["Event:DatingShaneBirthday_Shane:0"] = "",
                ["Event:DatingShaneBirthday_Shane:1"] = "",
                ["Event:DatingShaneBirthday_Shane:2"] = "",
                ["Event:DatingShaneBirthday_Shane:3"] = "",
                ["Event:DatingShaneBirthday_Finish:0"] = "",
                ["Event:DatingShaneBirthday_Finish:1"] = "",


                ["Event:PartyOver"] = ""
            },
            [StardewValley.LocalizedContentManager.LanguageCode.ru] = new Dictionary<string, string>()
            {
                ["Mail:birthdayMom"] = "ƒорогой @,^  — днем рождени¤, мо¤ радость. Ёто были замечательные моменты, когда ты выростал в доброго, трудолюбивого человека. я надеюсь, в твоей жизни будет куча превосходных моментов. ^  — любовью, мама ^  P.S. «десь находить небольшой подарок, который ¤ сделала дл¤ теб¤.  %item object 221 1 %%",
                ["Mail:birthdayDad"] = "ƒорогой @,^  — днем рождени¤, мой ребенок. «десь немного тихо в твой день рождени¤ с тех пор, как ты уехал на ферму, но тво¤ мать и ¤ знаем, что ты со своим дедушкой делаешь нас гордыми. ћы оба знаем, что жить на ферме может быть трудно, но мы верим в теб¤ на все 100%, просто  продолжай следовать своим мечтам.^  — любовью папа ^ P.S. “ут есть немного денег, которые помогут тебе на ферме. ”дачи! %item money 5000 5001%%",
                ["Mail:birthdayJunimos"] = "",
                ["Mail:birthdayDatingPenny"] = "",
                ["Mail:birthdayDatingMaru"] = "",
                ["Mail:birthdayDatingSebastian"] = "",
                ["Mail:birthdayDatingLeah"] = "",
                ["Mail:birthdayDatingAbigail"] = "",
                ["Mail:birthdayDatingAbigail_Wednesday"] = "",
                ["Mail:birthdayDatingEmily"] = "",
                ["Mail:birthdayDatingHaley"] = "",
                ["Mail:birthdayDatingHarvey"] = "",
                ["Mail:birthdayDatingElliott"] = "",
                ["Mail:birthdayDatingSam"] = "",
                ["Mail:birthdayDatingAlex"] = "",
                ["Mail:birthdayDatingShane"] = "",

                ["Happy Birthday: Star Message"] = "Ёто твой день рождени¤! — днем рождени¤!",
                ["Happy Birthday: Farmhand Birthday Message"] = "Ёто твой день рождени¤! ѕоздравл¤ю с этим!",
                ["Season"] = "Season",
                ["Date"] = "Date",
                ["BirthdayError_FestivalDay"] = "",
                ["FavoriteGift"] = "",


                ["Event:JunimoBirthdayParty_0"] = "",
                ["Event:JunimoBirthdayParty_1"] = "",
                ["Event:DatingPennyBirthday_Pam:0"] = "",
                ["Event:DatingPennyBirthday_Pam:1"] = "",
                ["Event:DatingPennyBirthday_Pam:2"] = "",
                ["Event:DatingPennyBirthday_Penny:0"] = "",
                ["Event:DatingPennyBirthday_Penny:1"] = "",
                ["Event:DatingPennyBirthday_Penny:2"] = "",
                ["Event:DatingPennyBirthday_Penny:3"] = "",
                ["Event:DatingPennyBirthday_Penny:4"] = "",
                ["Event:DatingPennyBirthday_Finish:0"] = "",
                ["Event:DatingPennyBirthday_Finish:1"] = "",

                ["Event:DatingMaruBirthday_Demetrius:0"] = "",
                ["Event:DatingMaruBirthday_Demetrius:1"] = "",
                ["Event:DatingMaruBirthday_Maru:0"] = "",
                ["Event:DatingMaruBirthday_Maru:1"] = "",
                ["Event:DatingMaruBirthday_Maru:2"] = "",
                ["Event:DatingMaruBirthday_Maru:3"] = "",
                ["Event:DatingMaruBirthday_Robin:0"] = "",
                ["Event:DatingMaruBirthday_Robin:1"] = "",
                ["Event:DatingMaruBirthday_Sebastian:0"] = "",
                ["Event:DatingMaruBirthday_Sebastian:1"] = "",
                ["Event:DatingMaruBirthday_Finish:0"] = "",
                ["Event:DatingMaruBirthday_Finish:1"] = "",

                ["Event:DatingSebastianBirthday_Demetrius:0"] = "",
                ["Event:DatingSebastianBirthday_Demetrius:1"] = "",
                ["Event:DatingSebastianBirthday_Maru:0"] = "",
                ["Event:DatingSebastianBirthday_Maru:1"] = "",
                ["Event:DatingSebastianBirthday_Robin:0"] = "",
                ["Event:DatingSebastianBirthday_Robin:1"] = "",
                ["Event:DatingSebastianBirthday_Robin:2"] = "",
                ["Event:DatingSebastianBirthday_Sebastian:0"] = "",
                ["Event:DatingSebastianBirthday_Sebastian:1"] = "",
                ["Event:DatingSebastianBirthday_Sebastian:2"] = "",
                ["Event:DatingSebastianBirthday_Finish:0"] = "",
                ["Event:DatingSebastianBirthday_Finish:1"] = "",

                ["Event:DatingLeahBirthday_Leah:0"] = "",
                ["Event:DatingLeahBirthday_Leah:1"] = "",
                ["Event:DatingLeahBirthday_Leah:2"] = "",
                ["Event:DatingLeahBirthday_Leah:3"] = "",
                ["Event:DatingLeahBirthday_Leah:4"] = "",
                ["Event:DatingLeahBirthday_Finish:0"] = "",
                ["Event:DatingLeahBirthday_Finish:1"] = "",

                ["Event:DatingAbigailBirthday_Abigail:0"] = "",
                ["Event:DatingAbigailBirthday_Abigail:1"] = "",
                ["Event:DatingAbigailBirthday_Abigail:2"] = "",
                ["Event:DatingAbigailBirthday_Abigail:3"] = "",
                ["Event:DatingAbigailBirthday_Abigail:4"] = "",
                ["Event:DatingAbigailBirthday_Caroline:0"] = "",
                ["Event:DatingAbigailBirthday_Caroline:1"] = "",
                ["Event:DatingAbigailBirthday_Caroline:2"] = "",
                ["Event:DatingAbigailBirthday_Pierre:0"] = "",
                ["Event:DatingAbigailBirthday_Pierre:1"] = "",
                ["Event:DatingAbigailBirthday_Finish:0"] = "",
                ["Event:DatingAbigailBirthday_Finish:1"] = "",

                ["Event:DatingAbigailBirthday_Mine_Abigail:0"] = "",
                ["Event:DatingAbigailBirthday_Mine_Abigail:1"] = "",
                ["Event:DatingAbigailBirthday_Mine_Abigail:2"] = "",
                ["Event:DatingAbigailBirthday_Mine_Abigail:3"] = "",
                ["Event:DatingAbigailBirthday_Mine_Abigail:4"] = "",
                ["Event:DatingAbigailBirthday_Mine_Finish:0"] = "",
                ["Event:DatingAbigailBirthday_Mine_Finish:1"] = "",

                ["Event:DatingEmilyBirthday_Emily:0"] = "",
                ["Event:DatingEmilyBirthday_Emily:1"] = "",
                ["Event:DatingEmilyBirthday_Emily:2"] = "",
                ["Event:DatingEmilyBirthday_Emily:3"] = "",
                ["Event:DatingEmilyBirthday_Emily:4"] = "",
                ["Event:DatingEmilyBirthday_Emily:5"] = "",
                ["Event:DatingEmilyBirthday_Finish:0"] = "",
                ["Event:DatingEmilyBirthday_Finish:1"] = "",

                ["Event:DatingHaleyBirthday_Haley:0"] = "",
                ["Event:DatingHaleyBirthday_Haley:1"] = "",
                ["Event:DatingHaleyBirthday_Haley:2"] = "",
                ["Event:DatingHaleyBirthday_Haley:3"] = "",
                ["Event:DatingHaleyBirthday_Haley:4"] = "",
                ["Event:DatingHaleyBirthday_Finish:0"] = "",
                ["Event:DatingHaleyBirthday_Finish:1"] = "",

                ["Event:DatingHarveyBirthday_Harvey:0"] = "",
                ["Event:DatingHarveyBirthday_Harvey:1"] = "",
                ["Event:DatingHarveyBirthday_Harvey:2"] = "",
                ["Event:DatingHarveyBirthday_Harvey:3"] = "",
                ["Event:DatingHarveyBirthday_Finish:0"] = "",
                ["Event:DatingHarveyBirthday_Finish:1"] = "",

                ["Event:DatingElliottBirthday_Elliott:0"] = "",
                ["Event:DatingElliottBirthday_Elliott:1"] = "",
                ["Event:DatingElliottBirthday_Elliott:2"] = "",
                ["Event:DatingElliottBirthday_Elliott:3"] = "",
                ["Event:DatingElliottBirthday_Elliott:4"] = "",
                ["Event:DatingElliottBirthday_Elliott:5"] = "",
                ["Event:DatingElliottBirthday_Finish:0"] = "",
                ["Event:DatingElliottBirthday_Finish:1"] = "",

                ["Event:DatingSamBirthday_Sam:0"] = "",
                ["Event:DatingSamBirthday_Sam:1"] = "",
                ["Event:DatingSamBirthday_Sam:2"] = "",
                ["Event:DatingSamBirthday_Sam:3"] = "",
                ["Event:DatingSamBirthday_Finish:0"] = "",
                ["Event:DatingSamBirthday_Finish:1"] = "",


                ["Event:DatingAlexBirthday_Alex:0"] = "",
                ["Event:DatingAlexBirthday_Alex:1"] = "",
                ["Event:DatingAlexBirthday_Alex:2"] = "",
                ["Event:DatingAlexBirthday_Alex:3"] = "",
                ["Event:DatingAlexBirthday_Finish:0"] = "",
                ["Event:DatingAlexBirthday_Finish:1"] = "",


                ["Event:DatingShaneBirthday_Shane:0"] = "",
                ["Event:DatingShaneBirthday_Shane:1"] = "",
                ["Event:DatingShaneBirthday_Shane:2"] = "",
                ["Event:DatingShaneBirthday_Shane:3"] = "",
                ["Event:DatingShaneBirthday_Finish:0"] = "",
                ["Event:DatingShaneBirthday_Finish:1"] = "",



                ["Event:PartyOver"] = ""
            },
            [StardewValley.LocalizedContentManager.LanguageCode.zh] = new Dictionary<string, string>()
            {
                ["Mail:birthdayMom"] = "亲爱的@，^  生日快乐宝贝。看着你成长成为一个善良努力的人，就如我一直梦想着你成为的样子，我感到十分欣喜。我希望你能继续跟你爱的人制造更多美好的回忆。 ^  爱你的，妈妈 ^ 附言：这是我给你做的一点小礼物。 %item object 221 1 %%",
                ["Mail:birthdayDad"] = "亲爱的@，^  生日快乐孩子。你生日的这天没有你，我们这儿还挺寂寞的，但我和你妈妈都知道你让我们和你爷爷感到骄傲。我们知道你一个人生活可能会很艰难，但我们百分百相信你能做到，所以继续追求你的梦想吧。^  爱你的，爸爸 ^ 附言：这是能在农场上帮到你的一些零用钱。祝你好运！ %item money 5000 5001 %%",
                ["Mail:birthdayJunimos"] = "",
                ["Mail:birthdayDatingPenny"] = "",
                ["Mail:birthdayDatingMaru"] = "",
                ["Mail:birthdayDatingSebastian"] = "",
                ["Mail:birthdayDatingLeah"] = "",
                ["Mail:birthdayDatingAbigail"] = "",
                ["Mail:birthdayDatingAbigail_Wednesday"] = "",
                ["Mail:birthdayDatingEmily"] = "",
                ["Mail:birthdayDatingHaley"] = "",
                ["Mail:birthdayDatingHarvey"] = "",
                ["Mail:birthdayDatingElliott"] = "",
                ["Mail:birthdayDatingSam"] = "",
                ["Mail:birthdayDatingAlex"] = "",
                ["Mail:birthdayDatingShane"] = "",

                ["Happy Birthday: Star Message"] = "今天是你的生日！生日快乐！",
                ["Happy Birthday: Farmhand Birthday Message"] = "",
                ["Season"] = "Season",
                ["Date"] = "Date",
                ["BirthdayError_FestivalDay"] = "",
                ["FavoriteGift"] = "",


                ["Event:JunimoBirthdayParty_0"] = "",
                ["Event:JunimoBirthdayParty_1"] = "",
                ["Event:DatingPennyBirthday_Pam:0"] = "",
                ["Event:DatingPennyBirthday_Pam:1"] = "",
                ["Event:DatingPennyBirthday_Pam:2"] = "",
                ["Event:DatingPennyBirthday_Penny:0"] = "",
                ["Event:DatingPennyBirthday_Penny:1"] = "",
                ["Event:DatingPennyBirthday_Penny:2"] = "",
                ["Event:DatingPennyBirthday_Penny:3"] = "",
                ["Event:DatingPennyBirthday_Penny:4"] = "",
                ["Event:DatingPennyBirthday_Finish:0"] = "",
                ["Event:DatingPennyBirthday_Finish:1"] = "",

                ["Event:DatingMaruBirthday_Demetrius:0"] = "",
                ["Event:DatingMaruBirthday_Demetrius:1"] = "",
                ["Event:DatingMaruBirthday_Maru:0"] = "",
                ["Event:DatingMaruBirthday_Maru:1"] = "",
                ["Event:DatingMaruBirthday_Maru:2"] = "",
                ["Event:DatingMaruBirthday_Maru:3"] = "",
                ["Event:DatingMaruBirthday_Robin:0"] = "",
                ["Event:DatingMaruBirthday_Robin:1"] = "",
                ["Event:DatingMaruBirthday_Sebastian:0"] = "",
                ["Event:DatingMaruBirthday_Sebastian:1"] = "",
                ["Event:DatingMaruBirthday_Finish:0"] = "",
                ["Event:DatingMaruBirthday_Finish:1"] = "",

                ["Event:DatingSebastianBirthday_Demetrius:0"] = "",
                ["Event:DatingSebastianBirthday_Demetrius:1"] = "",
                ["Event:DatingSebastianBirthday_Maru:0"] = "",
                ["Event:DatingSebastianBirthday_Maru:1"] = "",
                ["Event:DatingSebastianBirthday_Robin:0"] = "",
                ["Event:DatingSebastianBirthday_Robin:1"] = "",
                ["Event:DatingSebastianBirthday_Robin:2"] = "",
                ["Event:DatingSebastianBirthday_Sebastian:0"] = "",
                ["Event:DatingSebastianBirthday_Sebastian:1"] = "",
                ["Event:DatingSebastianBirthday_Sebastian:2"] = "",
                ["Event:DatingSebastianBirthday_Finish:0"] = "",
                ["Event:DatingSebastianBirthday_Finish:1"] = "",

                ["Event:DatingLeahBirthday_Leah:0"] = "",
                ["Event:DatingLeahBirthday_Leah:1"] = "",
                ["Event:DatingLeahBirthday_Leah:2"] = "",
                ["Event:DatingLeahBirthday_Leah:3"] = "",
                ["Event:DatingLeahBirthday_Leah:4"] = "",
                ["Event:DatingLeahBirthday_Finish:0"] = "",
                ["Event:DatingLeahBirthday_Finish:1"] = "",

                ["Event:DatingAbigailBirthday_Abigail:0"] = "",
                ["Event:DatingAbigailBirthday_Abigail:1"] = "",
                ["Event:DatingAbigailBirthday_Abigail:2"] = "",
                ["Event:DatingAbigailBirthday_Abigail:3"] = "",
                ["Event:DatingAbigailBirthday_Abigail:4"] = "",
                ["Event:DatingAbigailBirthday_Caroline:0"] = "",
                ["Event:DatingAbigailBirthday_Caroline:1"] = "",
                ["Event:DatingAbigailBirthday_Caroline:2"] = "",
                ["Event:DatingAbigailBirthday_Pierre:0"] = "",
                ["Event:DatingAbigailBirthday_Pierre:1"] = "",
                ["Event:DatingAbigailBirthday_Finish:0"] = "",
                ["Event:DatingAbigailBirthday_Finish:1"] = "",

                ["Event:DatingAbigailBirthday_Mine_Abigail:0"] = "",
                ["Event:DatingAbigailBirthday_Mine_Abigail:1"] = "",
                ["Event:DatingAbigailBirthday_Mine_Abigail:2"] = "",
                ["Event:DatingAbigailBirthday_Mine_Abigail:3"] = "",
                ["Event:DatingAbigailBirthday_Mine_Abigail:4"] = "",
                ["Event:DatingAbigailBirthday_Mine_Finish:0"] = "",
                ["Event:DatingAbigailBirthday_Mine_Finish:1"] = "",

                ["Event:DatingEmilyBirthday_Emily:0"] = "",
                ["Event:DatingEmilyBirthday_Emily:1"] = "",
                ["Event:DatingEmilyBirthday_Emily:2"] = "",
                ["Event:DatingEmilyBirthday_Emily:3"] = "",
                ["Event:DatingEmilyBirthday_Emily:4"] = "",
                ["Event:DatingEmilyBirthday_Emily:5"] = "",
                ["Event:DatingEmilyBirthday_Finish:0"] = "",
                ["Event:DatingEmilyBirthday_Finish:1"] = "",

                ["Event:DatingHaleyBirthday_Haley:0"] = "",
                ["Event:DatingHaleyBirthday_Haley:1"] = "",
                ["Event:DatingHaleyBirthday_Haley:2"] = "",
                ["Event:DatingHaleyBirthday_Haley:3"] = "",
                ["Event:DatingHaleyBirthday_Haley:4"] = "",
                ["Event:DatingHaleyBirthday_Finish:0"] = "",
                ["Event:DatingHaleyBirthday_Finish:1"] = "",

                ["Event:DatingHarveyBirthday_Harvey:0"] = "",
                ["Event:DatingHarveyBirthday_Harvey:1"] = "",
                ["Event:DatingHarveyBirthday_Harvey:2"] = "",
                ["Event:DatingHarveyBirthday_Harvey:3"] = "",
                ["Event:DatingHarveyBirthday_Finish:0"] = "",
                ["Event:DatingHarveyBirthday_Finish:1"] = "",

                ["Event:DatingElliottBirthday_Elliott:0"] = "",
                ["Event:DatingElliottBirthday_Elliott:1"] = "",
                ["Event:DatingElliottBirthday_Elliott:2"] = "",
                ["Event:DatingElliottBirthday_Elliott:3"] = "",
                ["Event:DatingElliottBirthday_Elliott:4"] = "",
                ["Event:DatingElliottBirthday_Elliott:5"] = "",
                ["Event:DatingElliottBirthday_Finish:0"] = "",
                ["Event:DatingElliottBirthday_Finish:1"] = "",

                ["Event:DatingSamBirthday_Sam:0"] = "",
                ["Event:DatingSamBirthday_Sam:1"] = "",
                ["Event:DatingSamBirthday_Sam:2"] = "",
                ["Event:DatingSamBirthday_Sam:3"] = "",
                ["Event:DatingSamBirthday_Finish:0"] = "",
                ["Event:DatingSamBirthday_Finish:1"] = "",


                ["Event:DatingAlexBirthday_Alex:0"] = "",
                ["Event:DatingAlexBirthday_Alex:1"] = "",
                ["Event:DatingAlexBirthday_Alex:2"] = "",
                ["Event:DatingAlexBirthday_Alex:3"] = "",
                ["Event:DatingAlexBirthday_Finish:0"] = "",
                ["Event:DatingAlexBirthday_Finish:1"] = "",


                ["Event:DatingShaneBirthday_Shane:0"] = "",
                ["Event:DatingShaneBirthday_Shane:1"] = "",
                ["Event:DatingShaneBirthday_Shane:2"] = "",
                ["Event:DatingShaneBirthday_Shane:3"] = "",
                ["Event:DatingShaneBirthday_Finish:0"] = "",
                ["Event:DatingShaneBirthday_Finish:1"] = "",

                ["Event:PartyOver"] = ""
            },
            [StardewValley.LocalizedContentManager.LanguageCode.pt] = new Dictionary<string, string>()
            {
                ["Mail:birthdayMom"] = "",
                ["Mail:birthdayDad"] = "",
                ["Mail:birthdayJunimos"] = "",
                ["Mail:birthdayDatingPenny"] = "",
                ["Mail:birthdayDatingMaru"] = "",
                ["Mail:birthdayDatingSebastian"] = "",
                ["Mail:birthdayDatingLeah"] = "",
                ["Mail:birthdayDatingAbigail"] = "",
                ["Mail:birthdayDatingAbigail_Wednesday"] = "",
                ["Mail:birthdayDatingEmily"] = "",
                ["Mail:birthdayDatingHaley"] = "",
                ["Mail:birthdayDatingHarvey"] = "",
                ["Mail:birthdayDatingElliott"] = "",
                ["Mail:birthdayDatingSam"] = "",
                ["Mail:birthdayDatingAlex"] = "",
                ["Mail:birthdayDatingShane"] = "",

                ["Happy Birthday: Star Message"] = "",
                ["Happy Birthday: Farmhand Birthday Message"] = "",
                ["Season"] = "Season",
                ["Date"] = "Date",
                ["BirthdayError_FestivalDay"] = "",
                ["FavoriteGift"] = "",

                ["Event:JunimoBirthdayParty_0"] = "",
                ["Event:JunimoBirthdayParty_1"] = "",
                ["Event:DatingPennyBirthday_Pam:0"] = "",
                ["Event:DatingPennyBirthday_Pam:1"] = "",
                ["Event:DatingPennyBirthday_Pam:2"] = "",
                ["Event:DatingPennyBirthday_Penny:0"] = "",
                ["Event:DatingPennyBirthday_Penny:1"] = "",
                ["Event:DatingPennyBirthday_Penny:2"] = "",
                ["Event:DatingPennyBirthday_Penny:3"] = "",
                ["Event:DatingPennyBirthday_Penny:4"] = "",
                ["Event:DatingPennyBirthday_Finish:0"] = "",
                ["Event:DatingPennyBirthday_Finish:1"] = "",

                ["Event:DatingMaruBirthday_Demetrius:0"] = "",
                ["Event:DatingMaruBirthday_Demetrius:1"] = "",
                ["Event:DatingMaruBirthday_Maru:0"] = "",
                ["Event:DatingMaruBirthday_Maru:1"] = "",
                ["Event:DatingMaruBirthday_Maru:2"] = "",
                ["Event:DatingMaruBirthday_Maru:3"] = "",
                ["Event:DatingMaruBirthday_Robin:0"] = "",
                ["Event:DatingMaruBirthday_Robin:1"] = "",
                ["Event:DatingMaruBirthday_Sebastian:0"] = "",
                ["Event:DatingMaruBirthday_Sebastian:1"] = "",
                ["Event:DatingMaruBirthday_Finish:0"] = "",
                ["Event:DatingMaruBirthday_Finish:1"] = "",

                ["Event:DatingSebastianBirthday_Demetrius:0"] = "",
                ["Event:DatingSebastianBirthday_Demetrius:1"] = "",
                ["Event:DatingSebastianBirthday_Maru:0"] = "",
                ["Event:DatingSebastianBirthday_Maru:1"] = "",
                ["Event:DatingSebastianBirthday_Robin:0"] = "",
                ["Event:DatingSebastianBirthday_Robin:1"] = "",
                ["Event:DatingSebastianBirthday_Robin:2"] = "",
                ["Event:DatingSebastianBirthday_Sebastian:0"] = "",
                ["Event:DatingSebastianBirthday_Sebastian:1"] = "",
                ["Event:DatingSebastianBirthday_Sebastian:2"] = "",
                ["Event:DatingSebastianBirthday_Finish:0"] = "",
                ["Event:DatingSebastianBirthday_Finish:1"] = "",

                ["Event:DatingLeahBirthday_Leah:0"] = "",
                ["Event:DatingLeahBirthday_Leah:1"] = "",
                ["Event:DatingLeahBirthday_Leah:2"] = "",
                ["Event:DatingLeahBirthday_Leah:3"] = "",
                ["Event:DatingLeahBirthday_Leah:4"] = "",
                ["Event:DatingLeahBirthday_Finish:0"] = "",
                ["Event:DatingLeahBirthday_Finish:1"] = "",

                ["Event:DatingAbigailBirthday_Abigail:0"] = "",
                ["Event:DatingAbigailBirthday_Abigail:1"] = "",
                ["Event:DatingAbigailBirthday_Abigail:2"] = "",
                ["Event:DatingAbigailBirthday_Abigail:3"] = "",
                ["Event:DatingAbigailBirthday_Abigail:4"] = "",
                ["Event:DatingAbigailBirthday_Caroline:0"] = "",
                ["Event:DatingAbigailBirthday_Caroline:1"] = "",
                ["Event:DatingAbigailBirthday_Caroline:2"] = "",
                ["Event:DatingAbigailBirthday_Pierre:0"] = "",
                ["Event:DatingAbigailBirthday_Pierre:1"] = "",
                ["Event:DatingAbigailBirthday_Finish:0"] = "",
                ["Event:DatingAbigailBirthday_Finish:1"] = "",

                ["Event:DatingAbigailBirthday_Mine_Abigail:0"] = "",
                ["Event:DatingAbigailBirthday_Mine_Abigail:1"] = "",
                ["Event:DatingAbigailBirthday_Mine_Abigail:2"] = "",
                ["Event:DatingAbigailBirthday_Mine_Abigail:3"] = "",
                ["Event:DatingAbigailBirthday_Mine_Abigail:4"] = "",
                ["Event:DatingAbigailBirthday_Mine_Finish:0"] = "",
                ["Event:DatingAbigailBirthday_Mine_Finish:1"] = "",

                ["Event:DatingEmilyBirthday_Emily:0"] = "",
                ["Event:DatingEmilyBirthday_Emily:1"] = "",
                ["Event:DatingEmilyBirthday_Emily:2"] = "",
                ["Event:DatingEmilyBirthday_Emily:3"] = "",
                ["Event:DatingEmilyBirthday_Emily:4"] = "",
                ["Event:DatingEmilyBirthday_Emily:5"] = "",
                ["Event:DatingEmilyBirthday_Finish:0"] = "",
                ["Event:DatingEmilyBirthday_Finish:1"] = "",

                ["Event:DatingHaleyBirthday_Haley:0"] = "",
                ["Event:DatingHaleyBirthday_Haley:1"] = "",
                ["Event:DatingHaleyBirthday_Haley:2"] = "",
                ["Event:DatingHaleyBirthday_Haley:3"] = "",
                ["Event:DatingHaleyBirthday_Haley:4"] = "",
                ["Event:DatingHaleyBirthday_Finish:0"] = "",
                ["Event:DatingHaleyBirthday_Finish:1"] = "",

                ["Event:DatingHarveyBirthday_Harvey:0"] = "",
                ["Event:DatingHarveyBirthday_Harvey:1"] = "",
                ["Event:DatingHarveyBirthday_Harvey:2"] = "",
                ["Event:DatingHarveyBirthday_Harvey:3"] = "",
                ["Event:DatingHarveyBirthday_Finish:0"] = "",
                ["Event:DatingHarveyBirthday_Finish:1"] = "",

                ["Event:DatingElliottBirthday_Elliott:0"] = "",
                ["Event:DatingElliottBirthday_Elliott:1"] = "",
                ["Event:DatingElliottBirthday_Elliott:2"] = "",
                ["Event:DatingElliottBirthday_Elliott:3"] = "",
                ["Event:DatingElliottBirthday_Elliott:4"] = "",
                ["Event:DatingElliottBirthday_Elliott:5"] = "",
                ["Event:DatingElliottBirthday_Finish:0"] = "",
                ["Event:DatingElliottBirthday_Finish:1"] = "",

                ["Event:DatingSamBirthday_Sam:0"] = "",
                ["Event:DatingSamBirthday_Sam:1"] = "",
                ["Event:DatingSamBirthday_Sam:2"] = "",
                ["Event:DatingSamBirthday_Sam:3"] = "",
                ["Event:DatingSamBirthday_Finish:0"] = "",
                ["Event:DatingSamBirthday_Finish:1"] = "",


                ["Event:DatingAlexBirthday_Alex:0"] = "",
                ["Event:DatingAlexBirthday_Alex:1"] = "",
                ["Event:DatingAlexBirthday_Alex:2"] = "",
                ["Event:DatingAlexBirthday_Alex:3"] = "",
                ["Event:DatingAlexBirthday_Finish:0"] = "",
                ["Event:DatingAlexBirthday_Finish:1"] = "",


                ["Event:DatingShaneBirthday_Shane:0"] = "",
                ["Event:DatingShaneBirthday_Shane:1"] = "",
                ["Event:DatingShaneBirthday_Shane:2"] = "",
                ["Event:DatingShaneBirthday_Shane:3"] = "",
                ["Event:DatingShaneBirthday_Finish:0"] = "",
                ["Event:DatingShaneBirthday_Finish:1"] = "",
                ["Event:PartyOver"] = ""
            },
            [StardewValley.LocalizedContentManager.LanguageCode.es] = new Dictionary<string, string>()
            {
                ["Mail:birthdayMom"] = "",
                ["Mail:birthdayDad"] = "",
                ["Mail:birthdayJunimos"] = "",
                ["Mail:birthdayDatingPenny"] = "",
                ["Mail:birthdayDatingMaru"] = "",
                ["Mail:birthdayDatingSebastian"] = "",
                ["Mail:birthdayDatingLeah"] = "",
                ["Mail:birthdayDatingAbigail"] = "",
                ["Mail:birthdayDatingAbigail_Wednesday"] = "",
                ["Mail:birthdayDatingEmily"] = "",
                ["Mail:birthdayDatingHaley"] = "",
                ["Mail:birthdayDatingHarvey"] = "",
                ["Mail:birthdayDatingElliott"] = "",
                ["Mail:birthdayDatingSam"] = "",
                ["Mail:birthdayDatingAlex"] = "",
                ["Mail:birthdayDatingShane"] = "",

                ["Happy Birthday: Star Message"] = "",
                ["Happy Birthday: Farmhand Birthday Message"] = "",
                ["Season"] = "Season",
                ["Date"] = "Date",
                ["BirthdayError_FestivalDay"] = "",
                ["FavoriteGift"] = "",

                ["Event:JunimoBirthdayParty_0"] = "",
                ["Event:JunimoBirthdayParty_1"] = "",
                ["Event:DatingPennyBirthday_Pam:0"] = "",
                ["Event:DatingPennyBirthday_Pam:1"] = "",
                ["Event:DatingPennyBirthday_Pam:2"] = "",
                ["Event:DatingPennyBirthday_Penny:0"] = "",
                ["Event:DatingPennyBirthday_Penny:1"] = "",
                ["Event:DatingPennyBirthday_Penny:2"] = "",
                ["Event:DatingPennyBirthday_Penny:3"] = "",
                ["Event:DatingPennyBirthday_Penny:4"] = "",
                ["Event:DatingPennyBirthday_Finish:0"] = "",
                ["Event:DatingPennyBirthday_Finish:1"] = "",

                ["Event:DatingMaruBirthday_Demetrius:0"] = "",
                ["Event:DatingMaruBirthday_Demetrius:1"] = "",
                ["Event:DatingMaruBirthday_Maru:0"] = "",
                ["Event:DatingMaruBirthday_Maru:1"] = "",
                ["Event:DatingMaruBirthday_Maru:2"] = "",
                ["Event:DatingMaruBirthday_Maru:3"] = "",
                ["Event:DatingMaruBirthday_Robin:0"] = "",
                ["Event:DatingMaruBirthday_Robin:1"] = "",
                ["Event:DatingMaruBirthday_Sebastian:0"] = "",
                ["Event:DatingMaruBirthday_Sebastian:1"] = "",
                ["Event:DatingMaruBirthday_Finish:0"] = "",
                ["Event:DatingMaruBirthday_Finish:1"] = "",

                ["Event:DatingSebastianBirthday_Demetrius:0"] = "",
                ["Event:DatingSebastianBirthday_Demetrius:1"] = "",
                ["Event:DatingSebastianBirthday_Maru:0"] = "",
                ["Event:DatingSebastianBirthday_Maru:1"] = "",
                ["Event:DatingSebastianBirthday_Robin:0"] = "",
                ["Event:DatingSebastianBirthday_Robin:1"] = "",
                ["Event:DatingSebastianBirthday_Robin:2"] = "",
                ["Event:DatingSebastianBirthday_Sebastian:0"] = "",
                ["Event:DatingSebastianBirthday_Sebastian:1"] = "",
                ["Event:DatingSebastianBirthday_Sebastian:2"] = "",
                ["Event:DatingSebastianBirthday_Finish:0"] = "",
                ["Event:DatingSebastianBirthday_Finish:1"] = "",

                ["Event:DatingLeahBirthday_Leah:0"] = "",
                ["Event:DatingLeahBirthday_Leah:1"] = "",
                ["Event:DatingLeahBirthday_Leah:2"] = "",
                ["Event:DatingLeahBirthday_Leah:3"] = "",
                ["Event:DatingLeahBirthday_Leah:4"] = "",
                ["Event:DatingLeahBirthday_Finish:0"] = "",
                ["Event:DatingLeahBirthday_Finish:1"] = "",

                ["Event:DatingAbigailBirthday_Abigail:0"] = "",
                ["Event:DatingAbigailBirthday_Abigail:1"] = "",
                ["Event:DatingAbigailBirthday_Abigail:2"] = "",
                ["Event:DatingAbigailBirthday_Abigail:3"] = "",
                ["Event:DatingAbigailBirthday_Abigail:4"] = "",
                ["Event:DatingAbigailBirthday_Caroline:0"] = "",
                ["Event:DatingAbigailBirthday_Caroline:1"] = "",
                ["Event:DatingAbigailBirthday_Caroline:2"] = "",
                ["Event:DatingAbigailBirthday_Pierre:0"] = "",
                ["Event:DatingAbigailBirthday_Pierre:1"] = "",
                ["Event:DatingAbigailBirthday_Finish:0"] = "",
                ["Event:DatingAbigailBirthday_Finish:1"] = "",

                ["Event:DatingAbigailBirthday_Mine_Abigail:0"] = "",
                ["Event:DatingAbigailBirthday_Mine_Abigail:1"] = "",
                ["Event:DatingAbigailBirthday_Mine_Abigail:2"] = "",
                ["Event:DatingAbigailBirthday_Mine_Abigail:3"] = "",
                ["Event:DatingAbigailBirthday_Mine_Abigail:4"] = "",
                ["Event:DatingAbigailBirthday_Mine_Finish:0"] = "",
                ["Event:DatingAbigailBirthday_Mine_Finish:1"] = "",

                ["Event:DatingEmilyBirthday_Emily:0"] = "",
                ["Event:DatingEmilyBirthday_Emily:1"] = "",
                ["Event:DatingEmilyBirthday_Emily:2"] = "",
                ["Event:DatingEmilyBirthday_Emily:3"] = "",
                ["Event:DatingEmilyBirthday_Emily:4"] = "",
                ["Event:DatingEmilyBirthday_Emily:5"] = "",
                ["Event:DatingEmilyBirthday_Finish:0"] = "",
                ["Event:DatingEmilyBirthday_Finish:1"] = "",

                ["Event:DatingHaleyBirthday_Haley:0"] = "",
                ["Event:DatingHaleyBirthday_Haley:1"] = "",
                ["Event:DatingHaleyBirthday_Haley:2"] = "",
                ["Event:DatingHaleyBirthday_Haley:3"] = "",
                ["Event:DatingHaleyBirthday_Haley:4"] = "",
                ["Event:DatingHaleyBirthday_Finish:0"] = "",
                ["Event:DatingHaleyBirthday_Finish:1"] = "",

                ["Event:DatingHarveyBirthday_Harvey:0"] = "",
                ["Event:DatingHarveyBirthday_Harvey:1"] = "",
                ["Event:DatingHarveyBirthday_Harvey:2"] = "",
                ["Event:DatingHarveyBirthday_Harvey:3"] = "",
                ["Event:DatingHarveyBirthday_Finish:0"] = "",
                ["Event:DatingHarveyBirthday_Finish:1"] = "",

                ["Event:DatingElliottBirthday_Elliott:0"] = "",
                ["Event:DatingElliottBirthday_Elliott:1"] = "",
                ["Event:DatingElliottBirthday_Elliott:2"] = "",
                ["Event:DatingElliottBirthday_Elliott:3"] = "",
                ["Event:DatingElliottBirthday_Elliott:4"] = "",
                ["Event:DatingElliottBirthday_Elliott:5"] = "",
                ["Event:DatingElliottBirthday_Finish:0"] = "",
                ["Event:DatingElliottBirthday_Finish:1"] = "",

                ["Event:DatingSamBirthday_Sam:0"] = "",
                ["Event:DatingSamBirthday_Sam:1"] = "",
                ["Event:DatingSamBirthday_Sam:2"] = "",
                ["Event:DatingSamBirthday_Sam:3"] = "",
                ["Event:DatingSamBirthday_Finish:0"] = "",
                ["Event:DatingSamBirthday_Finish:1"] = "",


                ["Event:DatingAlexBirthday_Alex:0"] = "",
                ["Event:DatingAlexBirthday_Alex:1"] = "",
                ["Event:DatingAlexBirthday_Alex:2"] = "",
                ["Event:DatingAlexBirthday_Alex:3"] = "",
                ["Event:DatingAlexBirthday_Finish:0"] = "",
                ["Event:DatingAlexBirthday_Finish:1"] = "",


                ["Event:DatingShaneBirthday_Shane:0"] = "",
                ["Event:DatingShaneBirthday_Shane:1"] = "",
                ["Event:DatingShaneBirthday_Shane:2"] = "",
                ["Event:DatingShaneBirthday_Shane:3"] = "",
                ["Event:DatingShaneBirthday_Finish:0"] = "",
                ["Event:DatingShaneBirthday_Finish:1"] = "",
                ["Event:PartyOver"] = ""
            },
            [StardewValley.LocalizedContentManager.LanguageCode.de] = new Dictionary<string, string>()
            {
                ["Mail:birthdayMom"] = "",
                ["Mail:birthdayDad"] = "",
                ["Mail:birthdayJunimos"] = "",
                ["Mail:birthdayDatingPenny"] = "",
                ["Mail:birthdayDatingMaru"] = "",
                ["Mail:birthdayDatingSebastian"] = "",
                ["Mail:birthdayDatingLeah"] = "",
                ["Mail:birthdayDatingAbigail"] = "",
                ["Mail:birthdayDatingAbigail_Wednesday"] = "",
                ["Mail:birthdayDatingEmily"] = "",
                ["Mail:birthdayDatingHaley"] = "",
                ["Mail:birthdayDatingHarvey"] = "",
                ["Mail:birthdayDatingElliott"] = "",
                ["Mail:birthdayDatingSam"] = "",
                ["Mail:birthdayDatingAlex"] = "",
                ["Mail:birthdayDatingShane"] = "",

                ["Happy Birthday: Star Message"] = "",
                ["Happy Birthday: Farmhand Birthday Message"] = "",
                ["Season"] = "Season",
                ["Date"] = "Date",
                ["BirthdayError_FestivalDay"] = "",
                ["FavoriteGift"] = "",

                ["Event:JunimoBirthdayParty_0"] = "",
                ["Event:JunimoBirthdayParty_1"] = "",
                ["Event:DatingPennyBirthday_Pam:0"] = "",
                ["Event:DatingPennyBirthday_Pam:1"] = "",
                ["Event:DatingPennyBirthday_Pam:2"] = "",
                ["Event:DatingPennyBirthday_Penny:0"] = "",
                ["Event:DatingPennyBirthday_Penny:1"] = "",
                ["Event:DatingPennyBirthday_Penny:2"] = "",
                ["Event:DatingPennyBirthday_Penny:3"] = "",
                ["Event:DatingPennyBirthday_Penny:4"] = "",
                ["Event:DatingPennyBirthday_Finish:0"] = "",
                ["Event:DatingPennyBirthday_Finish:1"] = "",

                ["Event:DatingMaruBirthday_Demetrius:0"] = "",
                ["Event:DatingMaruBirthday_Demetrius:1"] = "",
                ["Event:DatingMaruBirthday_Maru:0"] = "",
                ["Event:DatingMaruBirthday_Maru:1"] = "",
                ["Event:DatingMaruBirthday_Maru:2"] = "",
                ["Event:DatingMaruBirthday_Maru:3"] = "",
                ["Event:DatingMaruBirthday_Robin:0"] = "",
                ["Event:DatingMaruBirthday_Robin:1"] = "",
                ["Event:DatingMaruBirthday_Sebastian:0"] = "",
                ["Event:DatingMaruBirthday_Sebastian:1"] = "",
                ["Event:DatingMaruBirthday_Finish:0"] = "",
                ["Event:DatingMaruBirthday_Finish:1"] = "",

                ["Event:DatingSebastianBirthday_Demetrius:0"] = "",
                ["Event:DatingSebastianBirthday_Demetrius:1"] = "",
                ["Event:DatingSebastianBirthday_Maru:0"] = "",
                ["Event:DatingSebastianBirthday_Maru:1"] = "",
                ["Event:DatingSebastianBirthday_Robin:0"] = "",
                ["Event:DatingSebastianBirthday_Robin:1"] = "",
                ["Event:DatingSebastianBirthday_Robin:2"] = "",
                ["Event:DatingSebastianBirthday_Sebastian:0"] = "",
                ["Event:DatingSebastianBirthday_Sebastian:1"] = "",
                ["Event:DatingSebastianBirthday_Sebastian:2"] = "",
                ["Event:DatingSebastianBirthday_Finish:0"] = "",
                ["Event:DatingSebastianBirthday_Finish:1"] = "",

                ["Event:DatingLeahBirthday_Leah:0"] = "",
                ["Event:DatingLeahBirthday_Leah:1"] = "",
                ["Event:DatingLeahBirthday_Leah:2"] = "",
                ["Event:DatingLeahBirthday_Leah:3"] = "",
                ["Event:DatingLeahBirthday_Leah:4"] = "",
                ["Event:DatingLeahBirthday_Finish:0"] = "",
                ["Event:DatingLeahBirthday_Finish:1"] = "",

                ["Event:DatingAbigailBirthday_Abigail:0"] = "",
                ["Event:DatingAbigailBirthday_Abigail:1"] = "",
                ["Event:DatingAbigailBirthday_Abigail:2"] = "",
                ["Event:DatingAbigailBirthday_Abigail:3"] = "",
                ["Event:DatingAbigailBirthday_Abigail:4"] = "",
                ["Event:DatingAbigailBirthday_Caroline:0"] = "",
                ["Event:DatingAbigailBirthday_Caroline:1"] = "",
                ["Event:DatingAbigailBirthday_Caroline:2"] = "",
                ["Event:DatingAbigailBirthday_Pierre:0"] = "",
                ["Event:DatingAbigailBirthday_Pierre:1"] = "",
                ["Event:DatingAbigailBirthday_Finish:0"] = "",
                ["Event:DatingAbigailBirthday_Finish:1"] = "",

                ["Event:DatingAbigailBirthday_Mine_Abigail:0"] = "",
                ["Event:DatingAbigailBirthday_Mine_Abigail:1"] = "",
                ["Event:DatingAbigailBirthday_Mine_Abigail:2"] = "",
                ["Event:DatingAbigailBirthday_Mine_Abigail:3"] = "",
                ["Event:DatingAbigailBirthday_Mine_Abigail:4"] = "",
                ["Event:DatingAbigailBirthday_Mine_Finish:0"] = "",
                ["Event:DatingAbigailBirthday_Mine_Finish:1"] = "",

                ["Event:DatingEmilyBirthday_Emily:0"] = "",
                ["Event:DatingEmilyBirthday_Emily:1"] = "",
                ["Event:DatingEmilyBirthday_Emily:2"] = "",
                ["Event:DatingEmilyBirthday_Emily:3"] = "",
                ["Event:DatingEmilyBirthday_Emily:4"] = "",
                ["Event:DatingEmilyBirthday_Emily:5"] = "",
                ["Event:DatingEmilyBirthday_Finish:0"] = "",
                ["Event:DatingEmilyBirthday_Finish:1"] = "",

                ["Event:DatingHaleyBirthday_Haley:0"] = "",
                ["Event:DatingHaleyBirthday_Haley:1"] = "",
                ["Event:DatingHaleyBirthday_Haley:2"] = "",
                ["Event:DatingHaleyBirthday_Haley:3"] = "",
                ["Event:DatingHaleyBirthday_Haley:4"] = "",
                ["Event:DatingHaleyBirthday_Finish:0"] = "",
                ["Event:DatingHaleyBirthday_Finish:1"] = "",

                ["Event:DatingHarveyBirthday_Harvey:0"] = "",
                ["Event:DatingHarveyBirthday_Harvey:1"] = "",
                ["Event:DatingHarveyBirthday_Harvey:2"] = "",
                ["Event:DatingHarveyBirthday_Harvey:3"] = "",
                ["Event:DatingHarveyBirthday_Finish:0"] = "",
                ["Event:DatingHarveyBirthday_Finish:1"] = "",

                ["Event:DatingElliottBirthday_Elliott:0"] = "",
                ["Event:DatingElliottBirthday_Elliott:1"] = "",
                ["Event:DatingElliottBirthday_Elliott:2"] = "",
                ["Event:DatingElliottBirthday_Elliott:3"] = "",
                ["Event:DatingElliottBirthday_Elliott:4"] = "",
                ["Event:DatingElliottBirthday_Elliott:5"] = "",
                ["Event:DatingElliottBirthday_Finish:0"] = "",
                ["Event:DatingElliottBirthday_Finish:1"] = "",

                ["Event:DatingSamBirthday_Sam:0"] = "",
                ["Event:DatingSamBirthday_Sam:1"] = "",
                ["Event:DatingSamBirthday_Sam:2"] = "",
                ["Event:DatingSamBirthday_Sam:3"] = "",
                ["Event:DatingSamBirthday_Finish:0"] = "",
                ["Event:DatingSamBirthday_Finish:1"] = "",


                ["Event:DatingAlexBirthday_Alex:0"] = "",
                ["Event:DatingAlexBirthday_Alex:1"] = "",
                ["Event:DatingAlexBirthday_Alex:2"] = "",
                ["Event:DatingAlexBirthday_Alex:3"] = "",
                ["Event:DatingAlexBirthday_Finish:0"] = "",
                ["Event:DatingAlexBirthday_Finish:1"] = "",


                ["Event:DatingShaneBirthday_Shane:0"] = "",
                ["Event:DatingShaneBirthday_Shane:1"] = "",
                ["Event:DatingShaneBirthday_Shane:2"] = "",
                ["Event:DatingShaneBirthday_Shane:3"] = "",
                ["Event:DatingShaneBirthday_Finish:0"] = "",
                ["Event:DatingShaneBirthday_Finish:1"] = "",
                ["Event:PartyOver"] = ""
            },
            [StardewValley.LocalizedContentManager.LanguageCode.th] = new Dictionary<string, string>()
            {
                ["Mail:birthdayMom"] = "",
                ["Mail:birthdayDad"] = "",
                ["Mail:birthdayJunimos"] = "",
                ["Mail:birthdayDatingPenny"] = "",
                ["Mail:birthdayDatingMaru"] = "",
                ["Mail:birthdayDatingSebastian"] = "",
                ["Mail:birthdayDatingLeah"] = "",
                ["Mail:birthdayDatingAbigail"] = "",
                ["Mail:birthdayDatingAbigail_Wednesday"] = "",
                ["Mail:birthdayDatingEmily"] = "",
                ["Mail:birthdayDatingHaley"] = "",
                ["Mail:birthdayDatingHarvey"] = "",
                ["Mail:birthdayDatingElliott"] = "",
                ["Mail:birthdayDatingSam"] = "",
                ["Mail:birthdayDatingAlex"] = "",
                ["Mail:birthdayDatingShane"] = "",

                ["Happy Birthday: Star Message"] = "",
                ["Happy Birthday: Farmhand Birthday Message"] = "",
                ["Season"] = "Season",
                ["Date"] = "Date",
                ["BirthdayError_FestivalDay"] = "",
                ["FavoriteGift"] = "",

                ["Event:JunimoBirthdayParty_0"] = "",
                ["Event:JunimoBirthdayParty_1"] = "",
                ["Event:DatingPennyBirthday_Pam:0"] = "",
                ["Event:DatingPennyBirthday_Pam:1"] = "",
                ["Event:DatingPennyBirthday_Pam:2"] = "",
                ["Event:DatingPennyBirthday_Penny:0"] = "",
                ["Event:DatingPennyBirthday_Penny:1"] = "",
                ["Event:DatingPennyBirthday_Penny:2"] = "",
                ["Event:DatingPennyBirthday_Penny:3"] = "",
                ["Event:DatingPennyBirthday_Penny:4"] = "",
                ["Event:DatingPennyBirthday_Finish:0"] = "",
                ["Event:DatingPennyBirthday_Finish:1"] = "",

                ["Event:DatingMaruBirthday_Demetrius:0"] = "",
                ["Event:DatingMaruBirthday_Demetrius:1"] = "",
                ["Event:DatingMaruBirthday_Maru:0"] = "",
                ["Event:DatingMaruBirthday_Maru:1"] = "",
                ["Event:DatingMaruBirthday_Maru:2"] = "",
                ["Event:DatingMaruBirthday_Maru:3"] = "",
                ["Event:DatingMaruBirthday_Robin:0"] = "",
                ["Event:DatingMaruBirthday_Robin:1"] = "",
                ["Event:DatingMaruBirthday_Sebastian:0"] = "",
                ["Event:DatingMaruBirthday_Sebastian:1"] = "",
                ["Event:DatingMaruBirthday_Finish:0"] = "",
                ["Event:DatingMaruBirthday_Finish:1"] = "",

                ["Event:DatingSebastianBirthday_Demetrius:0"] = "",
                ["Event:DatingSebastianBirthday_Demetrius:1"] = "",
                ["Event:DatingSebastianBirthday_Maru:0"] = "",
                ["Event:DatingSebastianBirthday_Maru:1"] = "",
                ["Event:DatingSebastianBirthday_Robin:0"] = "",
                ["Event:DatingSebastianBirthday_Robin:1"] = "",
                ["Event:DatingSebastianBirthday_Robin:2"] = "",
                ["Event:DatingSebastianBirthday_Sebastian:0"] = "",
                ["Event:DatingSebastianBirthday_Sebastian:1"] = "",
                ["Event:DatingSebastianBirthday_Sebastian:2"] = "",
                ["Event:DatingSebastianBirthday_Finish:0"] = "",
                ["Event:DatingSebastianBirthday_Finish:1"] = "",

                ["Event:DatingLeahBirthday_Leah:0"] = "",
                ["Event:DatingLeahBirthday_Leah:1"] = "",
                ["Event:DatingLeahBirthday_Leah:2"] = "",
                ["Event:DatingLeahBirthday_Leah:3"] = "",
                ["Event:DatingLeahBirthday_Leah:4"] = "",
                ["Event:DatingLeahBirthday_Finish:0"] = "",
                ["Event:DatingLeahBirthday_Finish:1"] = "",

                ["Event:DatingAbigailBirthday_Abigail:0"] = "",
                ["Event:DatingAbigailBirthday_Abigail:1"] = "",
                ["Event:DatingAbigailBirthday_Abigail:2"] = "",
                ["Event:DatingAbigailBirthday_Abigail:3"] = "",
                ["Event:DatingAbigailBirthday_Abigail:4"] = "",
                ["Event:DatingAbigailBirthday_Caroline:0"] = "",
                ["Event:DatingAbigailBirthday_Caroline:1"] = "",
                ["Event:DatingAbigailBirthday_Caroline:2"] = "",
                ["Event:DatingAbigailBirthday_Pierre:0"] = "",
                ["Event:DatingAbigailBirthday_Pierre:1"] = "",
                ["Event:DatingAbigailBirthday_Finish:0"] = "",
                ["Event:DatingAbigailBirthday_Finish:1"] = "",

                ["Event:DatingAbigailBirthday_Mine_Abigail:0"] = "",
                ["Event:DatingAbigailBirthday_Mine_Abigail:1"] = "",
                ["Event:DatingAbigailBirthday_Mine_Abigail:2"] = "",
                ["Event:DatingAbigailBirthday_Mine_Abigail:3"] = "",
                ["Event:DatingAbigailBirthday_Mine_Abigail:4"] = "",
                ["Event:DatingAbigailBirthday_Mine_Finish:0"] = "",
                ["Event:DatingAbigailBirthday_Mine_Finish:1"] = "",

                ["Event:DatingEmilyBirthday_Emily:0"] = "",
                ["Event:DatingEmilyBirthday_Emily:1"] = "",
                ["Event:DatingEmilyBirthday_Emily:2"] = "",
                ["Event:DatingEmilyBirthday_Emily:3"] = "",
                ["Event:DatingEmilyBirthday_Emily:4"] = "",
                ["Event:DatingEmilyBirthday_Emily:5"] = "",
                ["Event:DatingEmilyBirthday_Finish:0"] = "",
                ["Event:DatingEmilyBirthday_Finish:1"] = "",

                ["Event:DatingHaleyBirthday_Haley:0"] = "",
                ["Event:DatingHaleyBirthday_Haley:1"] = "",
                ["Event:DatingHaleyBirthday_Haley:2"] = "",
                ["Event:DatingHaleyBirthday_Haley:3"] = "",
                ["Event:DatingHaleyBirthday_Haley:4"] = "",
                ["Event:DatingHaleyBirthday_Finish:0"] = "",
                ["Event:DatingHaleyBirthday_Finish:1"] = "",

                ["Event:DatingHarveyBirthday_Harvey:0"] = "",
                ["Event:DatingHarveyBirthday_Harvey:1"] = "",
                ["Event:DatingHarveyBirthday_Harvey:2"] = "",
                ["Event:DatingHarveyBirthday_Harvey:3"] = "",
                ["Event:DatingHarveyBirthday_Finish:0"] = "",
                ["Event:DatingHarveyBirthday_Finish:1"] = "",

                ["Event:DatingElliottBirthday_Elliott:0"] = "",
                ["Event:DatingElliottBirthday_Elliott:1"] = "",
                ["Event:DatingElliottBirthday_Elliott:2"] = "",
                ["Event:DatingElliottBirthday_Elliott:3"] = "",
                ["Event:DatingElliottBirthday_Elliott:4"] = "",
                ["Event:DatingElliottBirthday_Elliott:5"] = "",
                ["Event:DatingElliottBirthday_Finish:0"] = "",
                ["Event:DatingElliottBirthday_Finish:1"] = "",

                ["Event:DatingSamBirthday_Sam:0"] = "",
                ["Event:DatingSamBirthday_Sam:1"] = "",
                ["Event:DatingSamBirthday_Sam:2"] = "",
                ["Event:DatingSamBirthday_Sam:3"] = "",
                ["Event:DatingSamBirthday_Finish:0"] = "",
                ["Event:DatingSamBirthday_Finish:1"] = "",


                ["Event:DatingAlexBirthday_Alex:0"] = "",
                ["Event:DatingAlexBirthday_Alex:1"] = "",
                ["Event:DatingAlexBirthday_Alex:2"] = "",
                ["Event:DatingAlexBirthday_Alex:3"] = "",
                ["Event:DatingAlexBirthday_Finish:0"] = "",
                ["Event:DatingAlexBirthday_Finish:1"] = "",


                ["Event:DatingShaneBirthday_Shane:0"] = "",
                ["Event:DatingShaneBirthday_Shane:1"] = "",
                ["Event:DatingShaneBirthday_Shane:2"] = "",
                ["Event:DatingShaneBirthday_Shane:3"] = "",
                ["Event:DatingShaneBirthday_Finish:0"] = "",
                ["Event:DatingShaneBirthday_Finish:1"] = "",
                ["Event:PartyOver"] = ""
            },
            [StardewValley.LocalizedContentManager.LanguageCode.fr] = new Dictionary<string, string>()
            {
                ["Mail:birthdayMom"] = "",
                ["Mail:birthdayDad"] = "",
                ["Mail:birthdayJunimos"] = "",
                ["Mail:birthdayDatingPenny"] = "",
                ["Mail:birthdayDatingMaru"] = "",
                ["Mail:birthdayDatingSebastian"] = "",
                ["Mail:birthdayDatingLeah"] = "",
                ["Mail:birthdayDatingAbigail"] = "",
                ["Mail:birthdayDatingAbigail_Wednesday"] = "",
                ["Mail:birthdayDatingEmily"] = "",
                ["Mail:birthdayDatingHaley"] = "",
                ["Mail:birthdayDatingHarvey"] = "",
                ["Mail:birthdayDatingElliott"] = "",
                ["Mail:birthdayDatingSam"] = "",
                ["Mail:birthdayDatingAlex"] = "",
                ["Mail:birthdayDatingShane"] = "",

                ["Happy Birthday: Star Message"] = "",
                ["Happy Birthday: Farmhand Birthday Message"] = "",
                ["Season"] = "Season",
                ["Date"] = "Date",
                ["BirthdayError_FestivalDay"] = "",
                ["FavoriteGift"] = "",

                ["Event:JunimoBirthdayParty_0"] = "",
                ["Event:JunimoBirthdayParty_1"] = "",
                ["Event:DatingPennyBirthday_Pam:0"] = "",
                ["Event:DatingPennyBirthday_Pam:1"] = "",
                ["Event:DatingPennyBirthday_Pam:2"] = "",
                ["Event:DatingPennyBirthday_Penny:0"] = "",
                ["Event:DatingPennyBirthday_Penny:1"] = "",
                ["Event:DatingPennyBirthday_Penny:2"] = "",
                ["Event:DatingPennyBirthday_Penny:3"] = "",
                ["Event:DatingPennyBirthday_Penny:4"] = "",
                ["Event:DatingPennyBirthday_Finish:0"] = "",
                ["Event:DatingPennyBirthday_Finish:1"] = "",

                ["Event:DatingMaruBirthday_Demetrius:0"] = "",
                ["Event:DatingMaruBirthday_Demetrius:1"] = "",
                ["Event:DatingMaruBirthday_Maru:0"] = "",
                ["Event:DatingMaruBirthday_Maru:1"] = "",
                ["Event:DatingMaruBirthday_Maru:2"] = "",
                ["Event:DatingMaruBirthday_Maru:3"] = "",
                ["Event:DatingMaruBirthday_Robin:0"] = "",
                ["Event:DatingMaruBirthday_Robin:1"] = "",
                ["Event:DatingMaruBirthday_Sebastian:0"] = "",
                ["Event:DatingMaruBirthday_Sebastian:1"] = "",
                ["Event:DatingMaruBirthday_Finish:0"] = "",
                ["Event:DatingMaruBirthday_Finish:1"] = "",

                ["Event:DatingSebastianBirthday_Demetrius:0"] = "",
                ["Event:DatingSebastianBirthday_Demetrius:1"] = "",
                ["Event:DatingSebastianBirthday_Maru:0"] = "",
                ["Event:DatingSebastianBirthday_Maru:1"] = "",
                ["Event:DatingSebastianBirthday_Robin:0"] = "",
                ["Event:DatingSebastianBirthday_Robin:1"] = "",
                ["Event:DatingSebastianBirthday_Robin:2"] = "",
                ["Event:DatingSebastianBirthday_Sebastian:0"] = "",
                ["Event:DatingSebastianBirthday_Sebastian:1"] = "",
                ["Event:DatingSebastianBirthday_Sebastian:2"] = "",
                ["Event:DatingSebastianBirthday_Finish:0"] = "",
                ["Event:DatingSebastianBirthday_Finish:1"] = "",

                ["Event:DatingLeahBirthday_Leah:0"] = "",
                ["Event:DatingLeahBirthday_Leah:1"] = "",
                ["Event:DatingLeahBirthday_Leah:2"] = "",
                ["Event:DatingLeahBirthday_Leah:3"] = "",
                ["Event:DatingLeahBirthday_Leah:4"] = "",
                ["Event:DatingLeahBirthday_Finish:0"] = "",
                ["Event:DatingLeahBirthday_Finish:1"] = "",

                ["Event:DatingAbigailBirthday_Abigail:0"] = "",
                ["Event:DatingAbigailBirthday_Abigail:1"] = "",
                ["Event:DatingAbigailBirthday_Abigail:2"] = "",
                ["Event:DatingAbigailBirthday_Abigail:3"] = "",
                ["Event:DatingAbigailBirthday_Abigail:4"] = "",
                ["Event:DatingAbigailBirthday_Caroline:0"] = "",
                ["Event:DatingAbigailBirthday_Caroline:1"] = "",
                ["Event:DatingAbigailBirthday_Caroline:2"] = "",
                ["Event:DatingAbigailBirthday_Pierre:0"] = "",
                ["Event:DatingAbigailBirthday_Pierre:1"] = "",
                ["Event:DatingAbigailBirthday_Finish:0"] = "",
                ["Event:DatingAbigailBirthday_Finish:1"] = "",

                ["Event:DatingAbigailBirthday_Mine_Abigail:0"] = "",
                ["Event:DatingAbigailBirthday_Mine_Abigail:1"] = "",
                ["Event:DatingAbigailBirthday_Mine_Abigail:2"] = "",
                ["Event:DatingAbigailBirthday_Mine_Abigail:3"] = "",
                ["Event:DatingAbigailBirthday_Mine_Abigail:4"] = "",
                ["Event:DatingAbigailBirthday_Mine_Finish:0"] = "",
                ["Event:DatingAbigailBirthday_Mine_Finish:1"] = "",

                ["Event:DatingEmilyBirthday_Emily:0"] = "",
                ["Event:DatingEmilyBirthday_Emily:1"] = "",
                ["Event:DatingEmilyBirthday_Emily:2"] = "",
                ["Event:DatingEmilyBirthday_Emily:3"] = "",
                ["Event:DatingEmilyBirthday_Emily:4"] = "",
                ["Event:DatingEmilyBirthday_Emily:5"] = "",
                ["Event:DatingEmilyBirthday_Finish:0"] = "",
                ["Event:DatingEmilyBirthday_Finish:1"] = "",

                ["Event:DatingHaleyBirthday_Haley:0"] = "",
                ["Event:DatingHaleyBirthday_Haley:1"] = "",
                ["Event:DatingHaleyBirthday_Haley:2"] = "",
                ["Event:DatingHaleyBirthday_Haley:3"] = "",
                ["Event:DatingHaleyBirthday_Haley:4"] = "",
                ["Event:DatingHaleyBirthday_Finish:0"] = "",
                ["Event:DatingHaleyBirthday_Finish:1"] = "",

                ["Event:DatingHarveyBirthday_Harvey:0"] = "",
                ["Event:DatingHarveyBirthday_Harvey:1"] = "",
                ["Event:DatingHarveyBirthday_Harvey:2"] = "",
                ["Event:DatingHarveyBirthday_Harvey:3"] = "",
                ["Event:DatingHarveyBirthday_Finish:0"] = "",
                ["Event:DatingHarveyBirthday_Finish:1"] = "",

                ["Event:DatingElliottBirthday_Elliott:0"] = "",
                ["Event:DatingElliottBirthday_Elliott:1"] = "",
                ["Event:DatingElliottBirthday_Elliott:2"] = "",
                ["Event:DatingElliottBirthday_Elliott:3"] = "",
                ["Event:DatingElliottBirthday_Elliott:4"] = "",
                ["Event:DatingElliottBirthday_Elliott:5"] = "",
                ["Event:DatingElliottBirthday_Finish:0"] = "",
                ["Event:DatingElliottBirthday_Finish:1"] = "",

                ["Event:DatingSamBirthday_Sam:0"] = "",
                ["Event:DatingSamBirthday_Sam:1"] = "",
                ["Event:DatingSamBirthday_Sam:2"] = "",
                ["Event:DatingSamBirthday_Sam:3"] = "",
                ["Event:DatingSamBirthday_Finish:0"] = "",
                ["Event:DatingSamBirthday_Finish:1"] = "",


                ["Event:DatingAlexBirthday_Alex:0"] = "",
                ["Event:DatingAlexBirthday_Alex:1"] = "",
                ["Event:DatingAlexBirthday_Alex:2"] = "",
                ["Event:DatingAlexBirthday_Alex:3"] = "",
                ["Event:DatingAlexBirthday_Finish:0"] = "",
                ["Event:DatingAlexBirthday_Finish:1"] = "",


                ["Event:DatingShaneBirthday_Shane:0"] = "",
                ["Event:DatingShaneBirthday_Shane:1"] = "",
                ["Event:DatingShaneBirthday_Shane:2"] = "",
                ["Event:DatingShaneBirthday_Shane:3"] = "",
                ["Event:DatingShaneBirthday_Finish:0"] = "",
                ["Event:DatingShaneBirthday_Finish:1"] = "",
                ["Event:PartyOver"] = ""
            },
            [StardewValley.LocalizedContentManager.LanguageCode.ko] = new Dictionary<string, string>()
            {
                ["Mail:birthdayMom"] = "",
                ["Mail:birthdayDad"] = "",
                ["Mail:birthdayJunimos"] = "",
                ["Mail:birthdayDatingPenny"] = "",
                ["Mail:birthdayDatingMaru"] = "",
                ["Mail:birthdayDatingSebastian"] = "",
                ["Mail:birthdayDatingLeah"] = "",
                ["Mail:birthdayDatingAbigail"] = "",
                ["Mail:birthdayDatingAbigail_Wednesday"] = "",
                ["Mail:birthdayDatingEmily"] = "",
                ["Mail:birthdayDatingHaley"] = "",
                ["Mail:birthdayDatingHarvey"] = "",
                ["Mail:birthdayDatingElliott"] = "",
                ["Mail:birthdayDatingSam"] = "",
                ["Mail:birthdayDatingAlex"] = "",
                ["Mail:birthdayDatingShane"] = "",

                ["Happy Birthday: Star Message"] = "",
                ["Happy Birthday: Farmhand Birthday Message"] = "",
                ["Season"] = "Season",
                ["Date"] = "Date",
                ["BirthdayError_FestivalDay"] = "",
                ["FavoriteGift"] = "",

                ["Event:JunimoBirthdayParty_0"] = "",
                ["Event:JunimoBirthdayParty_1"] = "",
                ["Event:DatingPennyBirthday_Pam:0"] = "",
                ["Event:DatingPennyBirthday_Pam:1"] = "",
                ["Event:DatingPennyBirthday_Pam:2"] = "",
                ["Event:DatingPennyBirthday_Penny:0"] = "",
                ["Event:DatingPennyBirthday_Penny:1"] = "",
                ["Event:DatingPennyBirthday_Penny:2"] = "",
                ["Event:DatingPennyBirthday_Penny:3"] = "",
                ["Event:DatingPennyBirthday_Penny:4"] = "",
                ["Event:DatingPennyBirthday_Finish:0"] = "",
                ["Event:DatingPennyBirthday_Finish:1"] = "",

                ["Event:DatingMaruBirthday_Demetrius:0"] = "",
                ["Event:DatingMaruBirthday_Demetrius:1"] = "",
                ["Event:DatingMaruBirthday_Maru:0"] = "",
                ["Event:DatingMaruBirthday_Maru:1"] = "",
                ["Event:DatingMaruBirthday_Maru:2"] = "",
                ["Event:DatingMaruBirthday_Maru:3"] = "",
                ["Event:DatingMaruBirthday_Robin:0"] = "",
                ["Event:DatingMaruBirthday_Robin:1"] = "",
                ["Event:DatingMaruBirthday_Sebastian:0"] = "",
                ["Event:DatingMaruBirthday_Sebastian:1"] = "",
                ["Event:DatingMaruBirthday_Finish:0"] = "",
                ["Event:DatingMaruBirthday_Finish:1"] = "",

                ["Event:DatingSebastianBirthday_Demetrius:0"] = "",
                ["Event:DatingSebastianBirthday_Demetrius:1"] = "",
                ["Event:DatingSebastianBirthday_Maru:0"] = "",
                ["Event:DatingSebastianBirthday_Maru:1"] = "",
                ["Event:DatingSebastianBirthday_Robin:0"] = "",
                ["Event:DatingSebastianBirthday_Robin:1"] = "",
                ["Event:DatingSebastianBirthday_Robin:2"] = "",
                ["Event:DatingSebastianBirthday_Sebastian:0"] = "",
                ["Event:DatingSebastianBirthday_Sebastian:1"] = "",
                ["Event:DatingSebastianBirthday_Sebastian:2"] = "",
                ["Event:DatingSebastianBirthday_Finish:0"] = "",
                ["Event:DatingSebastianBirthday_Finish:1"] = "",

                ["Event:DatingLeahBirthday_Leah:0"] = "",
                ["Event:DatingLeahBirthday_Leah:1"] = "",
                ["Event:DatingLeahBirthday_Leah:2"] = "",
                ["Event:DatingLeahBirthday_Leah:3"] = "",
                ["Event:DatingLeahBirthday_Leah:4"] = "",
                ["Event:DatingLeahBirthday_Finish:0"] = "",
                ["Event:DatingLeahBirthday_Finish:1"] = "",

                ["Event:DatingAbigailBirthday_Abigail:0"] = "",
                ["Event:DatingAbigailBirthday_Abigail:1"] = "",
                ["Event:DatingAbigailBirthday_Abigail:2"] = "",
                ["Event:DatingAbigailBirthday_Abigail:3"] = "",
                ["Event:DatingAbigailBirthday_Abigail:4"] = "",
                ["Event:DatingAbigailBirthday_Caroline:0"] = "",
                ["Event:DatingAbigailBirthday_Caroline:1"] = "",
                ["Event:DatingAbigailBirthday_Caroline:2"] = "",
                ["Event:DatingAbigailBirthday_Pierre:0"] = "",
                ["Event:DatingAbigailBirthday_Pierre:1"] = "",
                ["Event:DatingAbigailBirthday_Finish:0"] = "",
                ["Event:DatingAbigailBirthday_Finish:1"] = "",

                ["Event:DatingAbigailBirthday_Mine_Abigail:0"] = "",
                ["Event:DatingAbigailBirthday_Mine_Abigail:1"] = "",
                ["Event:DatingAbigailBirthday_Mine_Abigail:2"] = "",
                ["Event:DatingAbigailBirthday_Mine_Abigail:3"] = "",
                ["Event:DatingAbigailBirthday_Mine_Abigail:4"] = "",
                ["Event:DatingAbigailBirthday_Mine_Finish:0"] = "",
                ["Event:DatingAbigailBirthday_Mine_Finish:1"] = "",

                ["Event:DatingEmilyBirthday_Emily:0"] = "",
                ["Event:DatingEmilyBirthday_Emily:1"] = "",
                ["Event:DatingEmilyBirthday_Emily:2"] = "",
                ["Event:DatingEmilyBirthday_Emily:3"] = "",
                ["Event:DatingEmilyBirthday_Emily:4"] = "",
                ["Event:DatingEmilyBirthday_Emily:5"] = "",
                ["Event:DatingEmilyBirthday_Finish:0"] = "",
                ["Event:DatingEmilyBirthday_Finish:1"] = "",

                ["Event:DatingHaleyBirthday_Haley:0"] = "",
                ["Event:DatingHaleyBirthday_Haley:1"] = "",
                ["Event:DatingHaleyBirthday_Haley:2"] = "",
                ["Event:DatingHaleyBirthday_Haley:3"] = "",
                ["Event:DatingHaleyBirthday_Haley:4"] = "",
                ["Event:DatingHaleyBirthday_Finish:0"] = "",
                ["Event:DatingHaleyBirthday_Finish:1"] = "",

                ["Event:DatingHarveyBirthday_Harvey:0"] = "",
                ["Event:DatingHarveyBirthday_Harvey:1"] = "",
                ["Event:DatingHarveyBirthday_Harvey:2"] = "",
                ["Event:DatingHarveyBirthday_Harvey:3"] = "",
                ["Event:DatingHarveyBirthday_Finish:0"] = "",
                ["Event:DatingHarveyBirthday_Finish:1"] = "",

                ["Event:DatingElliottBirthday_Elliott:0"] = "",
                ["Event:DatingElliottBirthday_Elliott:1"] = "",
                ["Event:DatingElliottBirthday_Elliott:2"] = "",
                ["Event:DatingElliottBirthday_Elliott:3"] = "",
                ["Event:DatingElliottBirthday_Elliott:4"] = "",
                ["Event:DatingElliottBirthday_Elliott:5"] = "",
                ["Event:DatingElliottBirthday_Finish:0"] = "",
                ["Event:DatingElliottBirthday_Finish:1"] = "",

                ["Event:DatingSamBirthday_Sam:0"] = "",
                ["Event:DatingSamBirthday_Sam:1"] = "",
                ["Event:DatingSamBirthday_Sam:2"] = "",
                ["Event:DatingSamBirthday_Sam:3"] = "",
                ["Event:DatingSamBirthday_Finish:0"] = "",
                ["Event:DatingSamBirthday_Finish:1"] = "",


                ["Event:DatingAlexBirthday_Alex:0"] = "",
                ["Event:DatingAlexBirthday_Alex:1"] = "",
                ["Event:DatingAlexBirthday_Alex:2"] = "",
                ["Event:DatingAlexBirthday_Alex:3"] = "",
                ["Event:DatingAlexBirthday_Finish:0"] = "",
                ["Event:DatingAlexBirthday_Finish:1"] = "",


                ["Event:DatingShaneBirthday_Shane:0"] = "",
                ["Event:DatingShaneBirthday_Shane:1"] = "",
                ["Event:DatingShaneBirthday_Shane:2"] = "",
                ["Event:DatingShaneBirthday_Shane:3"] = "",
                ["Event:DatingShaneBirthday_Finish:0"] = "",
                ["Event:DatingShaneBirthday_Finish:1"] = "",
                ["Event:PartyOver"] = ""
            },
            [StardewValley.LocalizedContentManager.LanguageCode.it] = new Dictionary<string, string>()
            {
                ["Mail:birthdayMom"] = "Caro @,^  Buon compleanno amore. È stato meraviglioso vederti crescere e diventare l'uomo gentile, generoso e lavoratore che abbiamo sempre voluto che tu diventassi. Spero tu possa continuare a creare fantastici ricordi con le persone che ami. ^  Con amore, Mamma ^ P.S. Ho allegato un qualcosina che ti può tornare utile. %item object 221 1 %%",
                ["Mail:birthdayDad"] = "Caro @,^  Tanti auguri ragazzo. Qui è tutto molto tranquillo da quando non ci sei, ma tua madre ed io sappiamo che stai rendendo tuo nonno e noi fieri.  Sapiamo entrambi che vivere da soli può essere dura ma noi crediamo in te al cento per cento, continua a seguire i tuoi sogni.^  Con amore, Papà ^ P.S. Ti ho allegato un pò di soldi per eaiutarti con la fattoria. Buona fortuna! %item money 5000 5001 %%",
                ["Mail:birthdayJunimos"] = "",
                ["Mail:birthdayDatingPenny"] = "",
                ["Mail:birthdayDatingMaru"] = "",
                ["Mail:birthdayDatingSebastian"] = "",
                ["Mail:birthdayDatingLeah"] = "",
                ["Mail:birthdayDatingAbigail"] = "",
                ["Mail:birthdayDatingAbigail_Wednesday"] = "",
                ["Mail:birthdayDatingEmily"] = "",
                ["Mail:birthdayDatingHaley"] = "",
                ["Mail:birthdayDatingHarvey"] = "",
                ["Mail:birthdayDatingElliott"] = "",
                ["Mail:birthdayDatingSam"] = "",
                ["Mail:birthdayDatingAlex"] = "",
                ["Mail:birthdayDatingShane"] = "",

                ["Happy Birthday: Star Message"] = "Oggi è il tuo compleanno! Buon compleanno!",
                ["Happy Birthday: Farmhand Birthday Message"] = "È il compleanno di @! Tanti auguri a loro!",
                ["Season"] = "Season",
                ["Date"] = "Date",
                ["BirthdayError_FestivalDay"] = "",
                ["FavoriteGift"] = "",

                ["Event:JunimoBirthdayParty_0"] = "",
                ["Event:JunimoBirthdayParty_1"] = "",
                ["Event:DatingPennyBirthday_Pam:0"] = "",
                ["Event:DatingPennyBirthday_Pam:1"] = "",
                ["Event:DatingPennyBirthday_Pam:2"] = "",
                ["Event:DatingPennyBirthday_Penny:0"] = "",
                ["Event:DatingPennyBirthday_Penny:1"] = "",
                ["Event:DatingPennyBirthday_Penny:2"] = "",
                ["Event:DatingPennyBirthday_Penny:3"] = "",
                ["Event:DatingPennyBirthday_Penny:4"] = "",
                ["Event:DatingPennyBirthday_Finish:0"] = "",
                ["Event:DatingPennyBirthday_Finish:1"] = "",

                ["Event:DatingMaruBirthday_Demetrius:0"] = "",
                ["Event:DatingMaruBirthday_Demetrius:1"] = "",
                ["Event:DatingMaruBirthday_Maru:0"] = "",
                ["Event:DatingMaruBirthday_Maru:1"] = "",
                ["Event:DatingMaruBirthday_Maru:2"] = "",
                ["Event:DatingMaruBirthday_Maru:3"] = "",
                ["Event:DatingMaruBirthday_Robin:0"] = "",
                ["Event:DatingMaruBirthday_Robin:1"] = "",
                ["Event:DatingMaruBirthday_Sebastian:0"] = "",
                ["Event:DatingMaruBirthday_Sebastian:1"] = "",
                ["Event:DatingMaruBirthday_Finish:0"] = "",
                ["Event:DatingMaruBirthday_Finish:1"] = "",

                ["Event:DatingSebastianBirthday_Demetrius:0"] = "",
                ["Event:DatingSebastianBirthday_Demetrius:1"] = "",
                ["Event:DatingSebastianBirthday_Maru:0"] = "",
                ["Event:DatingSebastianBirthday_Maru:1"] = "",
                ["Event:DatingSebastianBirthday_Robin:0"] = "",
                ["Event:DatingSebastianBirthday_Robin:1"] = "",
                ["Event:DatingSebastianBirthday_Robin:2"] = "",
                ["Event:DatingSebastianBirthday_Sebastian:0"] = "",
                ["Event:DatingSebastianBirthday_Sebastian:1"] = "",
                ["Event:DatingSebastianBirthday_Sebastian:2"] = "",
                ["Event:DatingSebastianBirthday_Finish:0"] = "",
                ["Event:DatingSebastianBirthday_Finish:1"] = "",

                ["Event:DatingLeahBirthday_Leah:0"] = "",
                ["Event:DatingLeahBirthday_Leah:1"] = "",
                ["Event:DatingLeahBirthday_Leah:2"] = "",
                ["Event:DatingLeahBirthday_Leah:3"] = "",
                ["Event:DatingLeahBirthday_Leah:4"] = "",
                ["Event:DatingLeahBirthday_Finish:0"] = "",
                ["Event:DatingLeahBirthday_Finish:1"] = "",

                ["Event:DatingAbigailBirthday_Abigail:0"] = "",
                ["Event:DatingAbigailBirthday_Abigail:1"] = "",
                ["Event:DatingAbigailBirthday_Abigail:2"] = "",
                ["Event:DatingAbigailBirthday_Abigail:3"] = "",
                ["Event:DatingAbigailBirthday_Abigail:4"] = "",
                ["Event:DatingAbigailBirthday_Caroline:0"] = "",
                ["Event:DatingAbigailBirthday_Caroline:1"] = "",
                ["Event:DatingAbigailBirthday_Caroline:2"] = "",
                ["Event:DatingAbigailBirthday_Pierre:0"] = "",
                ["Event:DatingAbigailBirthday_Pierre:1"] = "",
                ["Event:DatingAbigailBirthday_Finish:0"] = "",
                ["Event:DatingAbigailBirthday_Finish:1"] = "",

                ["Event:DatingAbigailBirthday_Mine_Abigail:0"] = "",
                ["Event:DatingAbigailBirthday_Mine_Abigail:1"] = "",
                ["Event:DatingAbigailBirthday_Mine_Abigail:2"] = "",
                ["Event:DatingAbigailBirthday_Mine_Abigail:3"] = "",
                ["Event:DatingAbigailBirthday_Mine_Abigail:4"] = "",
                ["Event:DatingAbigailBirthday_Mine_Finish:0"] = "",
                ["Event:DatingAbigailBirthday_Mine_Finish:1"] = "",

                ["Event:DatingEmilyBirthday_Emily:0"] = "",
                ["Event:DatingEmilyBirthday_Emily:1"] = "",
                ["Event:DatingEmilyBirthday_Emily:2"] = "",
                ["Event:DatingEmilyBirthday_Emily:3"] = "",
                ["Event:DatingEmilyBirthday_Emily:4"] = "",
                ["Event:DatingEmilyBirthday_Emily:5"] = "",
                ["Event:DatingEmilyBirthday_Finish:0"] = "",
                ["Event:DatingEmilyBirthday_Finish:1"] = "",

                ["Event:DatingHaleyBirthday_Haley:0"] = "",
                ["Event:DatingHaleyBirthday_Haley:1"] = "",
                ["Event:DatingHaleyBirthday_Haley:2"] = "",
                ["Event:DatingHaleyBirthday_Haley:3"] = "",
                ["Event:DatingHaleyBirthday_Haley:4"] = "",
                ["Event:DatingHaleyBirthday_Finish:0"] = "",
                ["Event:DatingHaleyBirthday_Finish:1"] = "",

                ["Event:DatingHarveyBirthday_Harvey:0"] = "",
                ["Event:DatingHarveyBirthday_Harvey:1"] = "",
                ["Event:DatingHarveyBirthday_Harvey:2"] = "",
                ["Event:DatingHarveyBirthday_Harvey:3"] = "",
                ["Event:DatingHarveyBirthday_Finish:0"] = "",
                ["Event:DatingHarveyBirthday_Finish:1"] = "",

                ["Event:DatingElliottBirthday_Elliott:0"] = "",
                ["Event:DatingElliottBirthday_Elliott:1"] = "",
                ["Event:DatingElliottBirthday_Elliott:2"] = "",
                ["Event:DatingElliottBirthday_Elliott:3"] = "",
                ["Event:DatingElliottBirthday_Elliott:4"] = "",
                ["Event:DatingElliottBirthday_Elliott:5"] = "",
                ["Event:DatingElliottBirthday_Finish:0"] = "",
                ["Event:DatingElliottBirthday_Finish:1"] = "",

                ["Event:DatingSamBirthday_Sam:0"] = "",
                ["Event:DatingSamBirthday_Sam:1"] = "",
                ["Event:DatingSamBirthday_Sam:2"] = "",
                ["Event:DatingSamBirthday_Sam:3"] = "",
                ["Event:DatingSamBirthday_Finish:0"] = "",
                ["Event:DatingSamBirthday_Finish:1"] = "",


                ["Event:DatingAlexBirthday_Alex:0"] = "",
                ["Event:DatingAlexBirthday_Alex:1"] = "",
                ["Event:DatingAlexBirthday_Alex:2"] = "",
                ["Event:DatingAlexBirthday_Alex:3"] = "",
                ["Event:DatingAlexBirthday_Finish:0"] = "",
                ["Event:DatingAlexBirthday_Finish:1"] = "",


                ["Event:DatingShaneBirthday_Shane:0"] = "",
                ["Event:DatingShaneBirthday_Shane:1"] = "",
                ["Event:DatingShaneBirthday_Shane:2"] = "",
                ["Event:DatingShaneBirthday_Shane:3"] = "",
                ["Event:DatingShaneBirthday_Finish:0"] = "",
                ["Event:DatingShaneBirthday_Finish:1"] = "",
                ["Event:PartyOver"] = ""
            },
            [StardewValley.LocalizedContentManager.LanguageCode.tr] = new Dictionary<string, string>()
            {
                ["Mail:birthdayMom"] = "",
                ["Mail:birthdayDad"] = "",
                ["Mail:birthdayJunimos"] = "",
                ["Mail:birthdayDatingPenny"] = "",
                ["Mail:birthdayDatingMaru"] = "",
                ["Mail:birthdayDatingSebastian"] = "",
                ["Mail:birthdayDatingLeah"] = "",
                ["Mail:birthdayDatingAbigail"] = "",
                ["Mail:birthdayDatingAbigail_Wednesday"] = "",
                ["Mail:birthdayDatingEmily"] = "",
                ["Mail:birthdayDatingHaley"] = "",
                ["Mail:birthdayDatingHarvey"] = "",
                ["Mail:birthdayDatingElliott"] = "",
                ["Mail:birthdayDatingSam"] = "",
                ["Mail:birthdayDatingAlex"] = "",
                ["Mail:birthdayDatingShane"] = "",

                ["Happy Birthday: Star Message"] = "",
                ["Happy Birthday: Farmhand Birthday Message"] = "",
                ["Season"] = "Season",
                ["Date"] = "Date",
                ["BirthdayError_FestivalDay"] = "",
                ["FavoriteGift"] = "",

                ["Event:JunimoBirthdayParty_0"] = "",
                ["Event:JunimoBirthdayParty_1"] = "",
                ["Event:DatingPennyBirthday_Pam:0"] = "",
                ["Event:DatingPennyBirthday_Pam:1"] = "",
                ["Event:DatingPennyBirthday_Pam:2"] = "",
                ["Event:DatingPennyBirthday_Penny:0"] = "",
                ["Event:DatingPennyBirthday_Penny:1"] = "",
                ["Event:DatingPennyBirthday_Penny:2"] = "",
                ["Event:DatingPennyBirthday_Penny:3"] = "",
                ["Event:DatingPennyBirthday_Penny:4"] = "",
                ["Event:DatingPennyBirthday_Finish:0"] = "",
                ["Event:DatingPennyBirthday_Finish:1"] = "",

                ["Event:DatingMaruBirthday_Demetrius:0"] = "",
                ["Event:DatingMaruBirthday_Demetrius:1"] = "",
                ["Event:DatingMaruBirthday_Maru:0"] = "",
                ["Event:DatingMaruBirthday_Maru:1"] = "",
                ["Event:DatingMaruBirthday_Maru:2"] = "",
                ["Event:DatingMaruBirthday_Maru:3"] = "",
                ["Event:DatingMaruBirthday_Robin:0"] = "",
                ["Event:DatingMaruBirthday_Robin:1"] = "",
                ["Event:DatingMaruBirthday_Sebastian:0"] = "",
                ["Event:DatingMaruBirthday_Sebastian:1"] = "",
                ["Event:DatingMaruBirthday_Finish:0"] = "",
                ["Event:DatingMaruBirthday_Finish:1"] = "",

                ["Event:DatingSebastianBirthday_Demetrius:0"] = "",
                ["Event:DatingSebastianBirthday_Demetrius:1"] = "",
                ["Event:DatingSebastianBirthday_Maru:0"] = "",
                ["Event:DatingSebastianBirthday_Maru:1"] = "",
                ["Event:DatingSebastianBirthday_Robin:0"] = "",
                ["Event:DatingSebastianBirthday_Robin:1"] = "",
                ["Event:DatingSebastianBirthday_Robin:2"] = "",
                ["Event:DatingSebastianBirthday_Sebastian:0"] = "",
                ["Event:DatingSebastianBirthday_Sebastian:1"] = "",
                ["Event:DatingSebastianBirthday_Sebastian:2"] = "",
                ["Event:DatingSebastianBirthday_Finish:0"] = "",
                ["Event:DatingSebastianBirthday_Finish:1"] = "",

                ["Event:DatingLeahBirthday_Leah:0"] = "",
                ["Event:DatingLeahBirthday_Leah:1"] = "",
                ["Event:DatingLeahBirthday_Leah:2"] = "",
                ["Event:DatingLeahBirthday_Leah:3"] = "",
                ["Event:DatingLeahBirthday_Leah:4"] = "",
                ["Event:DatingLeahBirthday_Finish:0"] = "",
                ["Event:DatingLeahBirthday_Finish:1"] = "",

                ["Event:DatingAbigailBirthday_Abigail:0"] = "",
                ["Event:DatingAbigailBirthday_Abigail:1"] = "",
                ["Event:DatingAbigailBirthday_Abigail:2"] = "",
                ["Event:DatingAbigailBirthday_Abigail:3"] = "",
                ["Event:DatingAbigailBirthday_Abigail:4"] = "",
                ["Event:DatingAbigailBirthday_Caroline:0"] = "",
                ["Event:DatingAbigailBirthday_Caroline:1"] = "",
                ["Event:DatingAbigailBirthday_Caroline:2"] = "",
                ["Event:DatingAbigailBirthday_Pierre:0"] = "",
                ["Event:DatingAbigailBirthday_Pierre:1"] = "",
                ["Event:DatingAbigailBirthday_Finish:0"] = "",
                ["Event:DatingAbigailBirthday_Finish:1"] = "",

                ["Event:DatingAbigailBirthday_Mine_Abigail:0"] = "",
                ["Event:DatingAbigailBirthday_Mine_Abigail:1"] = "",
                ["Event:DatingAbigailBirthday_Mine_Abigail:2"] = "",
                ["Event:DatingAbigailBirthday_Mine_Abigail:3"] = "",
                ["Event:DatingAbigailBirthday_Mine_Abigail:4"] = "",
                ["Event:DatingAbigailBirthday_Mine_Finish:0"] = "",
                ["Event:DatingAbigailBirthday_Mine_Finish:1"] = "",

                ["Event:DatingEmilyBirthday_Emily:0"] = "",
                ["Event:DatingEmilyBirthday_Emily:1"] = "",
                ["Event:DatingEmilyBirthday_Emily:2"] = "",
                ["Event:DatingEmilyBirthday_Emily:3"] = "",
                ["Event:DatingEmilyBirthday_Emily:4"] = "",
                ["Event:DatingEmilyBirthday_Emily:5"] = "",
                ["Event:DatingEmilyBirthday_Finish:0"] = "",
                ["Event:DatingEmilyBirthday_Finish:1"] = "",

                ["Event:DatingHaleyBirthday_Haley:0"] = "",
                ["Event:DatingHaleyBirthday_Haley:1"] = "",
                ["Event:DatingHaleyBirthday_Haley:2"] = "",
                ["Event:DatingHaleyBirthday_Haley:3"] = "",
                ["Event:DatingHaleyBirthday_Haley:4"] = "",
                ["Event:DatingHaleyBirthday_Finish:0"] = "",
                ["Event:DatingHaleyBirthday_Finish:1"] = "",

                ["Event:DatingHarveyBirthday_Harvey:0"] = "",
                ["Event:DatingHarveyBirthday_Harvey:1"] = "",
                ["Event:DatingHarveyBirthday_Harvey:2"] = "",
                ["Event:DatingHarveyBirthday_Harvey:3"] = "",
                ["Event:DatingHarveyBirthday_Finish:0"] = "",
                ["Event:DatingHarveyBirthday_Finish:1"] = "",

                ["Event:DatingElliottBirthday_Elliott:0"] = "",
                ["Event:DatingElliottBirthday_Elliott:1"] = "",
                ["Event:DatingElliottBirthday_Elliott:2"] = "",
                ["Event:DatingElliottBirthday_Elliott:3"] = "",
                ["Event:DatingElliottBirthday_Elliott:4"] = "",
                ["Event:DatingElliottBirthday_Elliott:5"] = "",
                ["Event:DatingElliottBirthday_Finish:0"] = "",
                ["Event:DatingElliottBirthday_Finish:1"] = "",

                ["Event:DatingSamBirthday_Sam:0"] = "",
                ["Event:DatingSamBirthday_Sam:1"] = "",
                ["Event:DatingSamBirthday_Sam:2"] = "",
                ["Event:DatingSamBirthday_Sam:3"] = "",
                ["Event:DatingSamBirthday_Finish:0"] = "",
                ["Event:DatingSamBirthday_Finish:1"] = "",


                ["Event:DatingAlexBirthday_Alex:0"] = "",
                ["Event:DatingAlexBirthday_Alex:1"] = "",
                ["Event:DatingAlexBirthday_Alex:2"] = "",
                ["Event:DatingAlexBirthday_Alex:3"] = "",
                ["Event:DatingAlexBirthday_Finish:0"] = "",
                ["Event:DatingAlexBirthday_Finish:1"] = "",


                ["Event:DatingShaneBirthday_Shane:0"] = "",
                ["Event:DatingShaneBirthday_Shane:1"] = "",
                ["Event:DatingShaneBirthday_Shane:2"] = "",
                ["Event:DatingShaneBirthday_Shane:3"] = "",
                ["Event:DatingShaneBirthday_Finish:0"] = "",
                ["Event:DatingShaneBirthday_Finish:1"] = "",
                ["Event:PartyOver"] = ""
            },
            [StardewValley.LocalizedContentManager.LanguageCode.hu] = new Dictionary<string, string>()
            {
                ["Mail:birthdayMom"] = "",
                ["Mail:birthdayDad"] = "",
                ["Mail:birthdayJunimos"] = "",
                ["Mail:birthdayDatingPenny"] = "",
                ["Mail:birthdayDatingMaru"] = "",
                ["Mail:birthdayDatingSebastian"] = "",
                ["Mail:birthdayDatingLeah"] = "",
                ["Mail:birthdayDatingAbigail"] = "",
                ["Mail:birthdayDatingAbigail_Wednesday"] = "",
                ["Mail:birthdayDatingEmily"] = "",
                ["Mail:birthdayDatingHaley"] = "",
                ["Mail:birthdayDatingHarvey"] = "",
                ["Mail:birthdayDatingElliott"] = "",
                ["Mail:birthdayDatingSam"] = "",
                ["Mail:birthdayDatingAlex"] = "",
                ["Mail:birthdayDatingShane"] = "",

                ["Happy Birthday: Star Message"] = "",
                ["Happy Birthday: Farmhand Birthday Message"] = "",
                ["Season"] = "Season",
                ["Date"] = "Date",
                ["BirthdayError_FestivalDay"] = "",
                ["FavoriteGift"] = "",

                ["Event:JunimoBirthdayParty_0"] = "",
                ["Event:JunimoBirthdayParty_1"] = "",
                ["Event:DatingPennyBirthday_Pam:0"] = "",
                ["Event:DatingPennyBirthday_Pam:1"] = "",
                ["Event:DatingPennyBirthday_Pam:2"] = "",
                ["Event:DatingPennyBirthday_Penny:0"] = "",
                ["Event:DatingPennyBirthday_Penny:1"] = "",
                ["Event:DatingPennyBirthday_Penny:2"] = "",
                ["Event:DatingPennyBirthday_Penny:3"] = "",
                ["Event:DatingPennyBirthday_Penny:4"] = "",
                ["Event:DatingPennyBirthday_Finish:0"] = "",
                ["Event:DatingPennyBirthday_Finish:1"] = "",

                ["Event:DatingMaruBirthday_Demetrius:0"] = "",
                ["Event:DatingMaruBirthday_Demetrius:1"] = "",
                ["Event:DatingMaruBirthday_Maru:0"] = "",
                ["Event:DatingMaruBirthday_Maru:1"] = "",
                ["Event:DatingMaruBirthday_Maru:2"] = "",
                ["Event:DatingMaruBirthday_Maru:3"] = "",
                ["Event:DatingMaruBirthday_Robin:0"] = "",
                ["Event:DatingMaruBirthday_Robin:1"] = "",
                ["Event:DatingMaruBirthday_Sebastian:0"] = "",
                ["Event:DatingMaruBirthday_Sebastian:1"] = "",
                ["Event:DatingMaruBirthday_Finish:0"] = "",
                ["Event:DatingMaruBirthday_Finish:1"] = "",

                ["Event:DatingSebastianBirthday_Demetrius:0"] = "",
                ["Event:DatingSebastianBirthday_Demetrius:1"] = "",
                ["Event:DatingSebastianBirthday_Maru:0"] = "",
                ["Event:DatingSebastianBirthday_Maru:1"] = "",
                ["Event:DatingSebastianBirthday_Robin:0"] = "",
                ["Event:DatingSebastianBirthday_Robin:1"] = "",
                ["Event:DatingSebastianBirthday_Robin:2"] = "",
                ["Event:DatingSebastianBirthday_Sebastian:0"] = "",
                ["Event:DatingSebastianBirthday_Sebastian:1"] = "",
                ["Event:DatingSebastianBirthday_Sebastian:2"] = "",
                ["Event:DatingSebastianBirthday_Finish:0"] = "",
                ["Event:DatingSebastianBirthday_Finish:1"] = "",

                ["Event:DatingLeahBirthday_Leah:0"] = "",
                ["Event:DatingLeahBirthday_Leah:1"] = "",
                ["Event:DatingLeahBirthday_Leah:2"] = "",
                ["Event:DatingLeahBirthday_Leah:3"] = "",
                ["Event:DatingLeahBirthday_Leah:4"] = "",
                ["Event:DatingLeahBirthday_Finish:0"] = "",
                ["Event:DatingLeahBirthday_Finish:1"] = "",

                ["Event:DatingAbigailBirthday_Abigail:0"] = "",
                ["Event:DatingAbigailBirthday_Abigail:1"] = "",
                ["Event:DatingAbigailBirthday_Abigail:2"] = "",
                ["Event:DatingAbigailBirthday_Abigail:3"] = "",
                ["Event:DatingAbigailBirthday_Abigail:4"] = "",
                ["Event:DatingAbigailBirthday_Caroline:0"] = "",
                ["Event:DatingAbigailBirthday_Caroline:1"] = "",
                ["Event:DatingAbigailBirthday_Caroline:2"] = "",
                ["Event:DatingAbigailBirthday_Pierre:0"] = "",
                ["Event:DatingAbigailBirthday_Pierre:1"] = "",
                ["Event:DatingAbigailBirthday_Finish:0"] = "",
                ["Event:DatingAbigailBirthday_Finish:1"] = "",

                ["Event:DatingAbigailBirthday_Mine_Abigail:0"] = "",
                ["Event:DatingAbigailBirthday_Mine_Abigail:1"] = "",
                ["Event:DatingAbigailBirthday_Mine_Abigail:2"] = "",
                ["Event:DatingAbigailBirthday_Mine_Abigail:3"] = "",
                ["Event:DatingAbigailBirthday_Mine_Abigail:4"] = "",
                ["Event:DatingAbigailBirthday_Mine_Finish:0"] = "",
                ["Event:DatingAbigailBirthday_Mine_Finish:1"] = "",

                ["Event:DatingEmilyBirthday_Emily:0"] = "",
                ["Event:DatingEmilyBirthday_Emily:1"] = "",
                ["Event:DatingEmilyBirthday_Emily:2"] = "",
                ["Event:DatingEmilyBirthday_Emily:3"] = "",
                ["Event:DatingEmilyBirthday_Emily:4"] = "",
                ["Event:DatingEmilyBirthday_Emily:5"] = "",
                ["Event:DatingEmilyBirthday_Finish:0"] = "",
                ["Event:DatingEmilyBirthday_Finish:1"] = "",

                ["Event:DatingHaleyBirthday_Haley:0"] = "",
                ["Event:DatingHaleyBirthday_Haley:1"] = "",
                ["Event:DatingHaleyBirthday_Haley:2"] = "",
                ["Event:DatingHaleyBirthday_Haley:3"] = "",
                ["Event:DatingHaleyBirthday_Haley:4"] = "",
                ["Event:DatingHaleyBirthday_Finish:0"] = "",
                ["Event:DatingHaleyBirthday_Finish:1"] = "",

                ["Event:DatingHarveyBirthday_Harvey:0"] = "",
                ["Event:DatingHarveyBirthday_Harvey:1"] = "",
                ["Event:DatingHarveyBirthday_Harvey:2"] = "",
                ["Event:DatingHarveyBirthday_Harvey:3"] = "",
                ["Event:DatingHarveyBirthday_Finish:0"] = "",
                ["Event:DatingHarveyBirthday_Finish:1"] = "",

                ["Event:DatingElliottBirthday_Elliott:0"] = "",
                ["Event:DatingElliottBirthday_Elliott:1"] = "",
                ["Event:DatingElliottBirthday_Elliott:2"] = "",
                ["Event:DatingElliottBirthday_Elliott:3"] = "",
                ["Event:DatingElliottBirthday_Elliott:4"] = "",
                ["Event:DatingElliottBirthday_Elliott:5"] = "",
                ["Event:DatingElliottBirthday_Finish:0"] = "",
                ["Event:DatingElliottBirthday_Finish:1"] = "",

                ["Event:DatingSamBirthday_Sam:0"] = "",
                ["Event:DatingSamBirthday_Sam:1"] = "",
                ["Event:DatingSamBirthday_Sam:2"] = "",
                ["Event:DatingSamBirthday_Sam:3"] = "",
                ["Event:DatingSamBirthday_Finish:0"] = "",
                ["Event:DatingSamBirthday_Finish:1"] = "",


                ["Event:DatingAlexBirthday_Alex:0"] = "",
                ["Event:DatingAlexBirthday_Alex:1"] = "",
                ["Event:DatingAlexBirthday_Alex:2"] = "",
                ["Event:DatingAlexBirthday_Alex:3"] = "",
                ["Event:DatingAlexBirthday_Finish:0"] = "",
                ["Event:DatingAlexBirthday_Finish:1"] = "",


                ["Event:DatingShaneBirthday_Shane:0"] = "",
                ["Event:DatingShaneBirthday_Shane:1"] = "",
                ["Event:DatingShaneBirthday_Shane:2"] = "",
                ["Event:DatingShaneBirthday_Shane:3"] = "",
                ["Event:DatingShaneBirthday_Finish:0"] = "",
                ["Event:DatingShaneBirthday_Finish:1"] = "",
                ["Event:PartyOver"] = ""
            },
        };

        /// <summary>Used to load all of the default birthday greetings.</summary>
        public void createBirthdayGreetings()
        {
            var serializer = JsonSerializer.Create();
            serializer.Formatting = Formatting.Indented;

            string birthdayFileDict = HappyBirthday.Config.translationInfo.getJSONForTranslation("BirthdayWishes", HappyBirthday.Config.translationInfo.CurrentTranslation);
            string path = Path.Combine("ModAssets", "Dialogue", HappyBirthday.Config.translationInfo.getFileExtentionForDirectory(Framework.TranslationInfo.LanguageName.English), birthdayFileDict);

            //Handle normal birthday wishes.
            if (!File.Exists(Path.Combine(HappyBirthday.ModHelper.DirectoryPath, path)))
            {
                //HappyBirthday.ModMonitor.Log("Creating Villager Birthday Messages", StardewModdingAPI.LogLevel.Alert);
                HappyBirthday.ModHelper.Data.WriteJsonFile<Dictionary<string, string>>(path, this.defaultBirthdayWishes);
                this.birthdayWishes = this.defaultBirthdayWishes;
            }
            else
                this.birthdayWishes = HappyBirthday.ModHelper.Data.ReadJsonFile<Dictionary<string, string>>(path);

            //handle spouse birthday wishes.
            string spouseBirthdayFileDict = HappyBirthday.Config.translationInfo.getJSONForTranslation("SpouseBirthdayWishes", HappyBirthday.Config.translationInfo.CurrentTranslation);
            string spousePath = Path.Combine("ModAssets", "Dialogue", HappyBirthday.Config.translationInfo.getFileExtentionForDirectory(HappyBirthday.Config.translationInfo.CurrentTranslation), spouseBirthdayFileDict);
            if (!File.Exists(Path.Combine(HappyBirthday.ModHelper.DirectoryPath, spousePath)))
            {
                HappyBirthday.ModMonitor.Log("Creating Spouse Messages", StardewModdingAPI.LogLevel.Alert);
                HappyBirthday.ModHelper.Data.WriteJsonFile<Dictionary<string, string>>(spousePath, this.defaultSpouseBirthdayWishes);
                this.spouseBirthdayWishes = this.defaultSpouseBirthdayWishes;
            }
            else
                this.spouseBirthdayWishes = HappyBirthday.ModHelper.Data.ReadJsonFile<Dictionary<string, string>>(spousePath);

            //Non-english logic for creating templates.
            foreach (var translation in HappyBirthday.Config.translationInfo.TranslationCodes)
            {
                if (translation.Key == Framework.TranslationInfo.LanguageName.English)
                    continue;

                string basePath = Path.Combine(HappyBirthday.ModHelper.DirectoryPath, "ModAssets", "Dialogue", HappyBirthday.Config.translationInfo.getFileExtentionForDirectory(translation.Key));
                if (!Directory.Exists(basePath))
                {
                    Directory.CreateDirectory(basePath);
                }
                string tempBirthdayFile = Path.Combine("ModAssets", "Dialogue", HappyBirthday.Config.translationInfo.getFileExtentionForDirectory(translation.Key), HappyBirthday.Config.translationInfo.getJSONForTranslation("BirthdayWishes", translation.Key));
                string tempSpouseBirthdayFile = Path.Combine("ModAssets", "Dialogue", HappyBirthday.Config.translationInfo.getFileExtentionForDirectory(translation.Key), HappyBirthday.Config.translationInfo.getJSONForTranslation("SpouseBirthdayWishes", translation.Key));


                Dictionary<string, string> tempBirthdayDict = new Dictionary<string, string>();
                if (!File.Exists(Path.Combine(HappyBirthday.ModHelper.DirectoryPath, tempBirthdayFile)))
                {
                    foreach (var pair in this.defaultBirthdayWishes)
                        tempBirthdayDict.Add(pair.Key, "");
                    HappyBirthday.ModHelper.Data.WriteJsonFile<Dictionary<string, string>>(tempBirthdayFile, tempBirthdayDict);
                }
                else
                    tempBirthdayDict = HappyBirthday.ModHelper.Data.ReadJsonFile<Dictionary<string, string>>(tempBirthdayFile);


                Dictionary<string, string> tempSpouseBirthdayDict = new Dictionary<string, string>();
                if (!File.Exists(Path.Combine(HappyBirthday.ModHelper.DirectoryPath, tempSpouseBirthdayFile)))
                {
                    foreach (var pair in this.defaultSpouseBirthdayWishes)
                        tempSpouseBirthdayDict.Add(pair.Key, "");
                    HappyBirthday.ModHelper.Data.WriteJsonFile<Dictionary<string, string>>(tempSpouseBirthdayFile, tempSpouseBirthdayDict);
                }
                else
                    tempSpouseBirthdayDict = HappyBirthday.ModHelper.Data.ReadJsonFile<Dictionary<string, string>>(tempSpouseBirthdayFile);

                //Set translated birthday info.
                if (HappyBirthday.Config.translationInfo.CurrentTranslation == translation.Key)
                {
                    this.birthdayWishes = tempBirthdayDict;
                    this.spouseBirthdayWishes = tempSpouseBirthdayDict;
                    HappyBirthday.ModMonitor.Log("Language set to: " + translation);
                }
            }
        }

        /// <summary>
        /// Gets a translated string from the mod's json files.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetTranslatedString(string key)
        {
            StardewValley.LocalizedContentManager.LanguageCode code = HappyBirthday.Config.translationInfo.TranslationCodes[HappyBirthday.Config.translationInfo.CurrentTranslation];
            if (string.IsNullOrEmpty(key)) return "";
            if (HappyBirthday.Instance.messages.translatedStrings[code].ContainsKey(key))
            {
                string value = HappyBirthday.Instance.messages.translatedStrings[code][key];
                if (string.IsNullOrEmpty(value))
                {
                    return GetEnglishMessageString(key);
                }
                else return value;
            }
            else
            {
                if (HappyBirthday.Config.defaultToEnglishTranslation)
                {
                    return GetEnglishMessageString(key);
                }
                else
                {
                    return "";
                }
            }

        }

        public static string GetEnglishMessageString(string key)
        {
            StardewValley.LocalizedContentManager.LanguageCode code = StardewValley.LocalizedContentManager.LanguageCode.en;
            if (HappyBirthday.Instance.messages.translatedStrings[code].ContainsKey(key))
            {
                return HappyBirthday.Instance.messages.translatedStrings[code][key];
            }
            else
            {
                return "";
            }
        }

        public void loadTranslationStrings()
        {

            //Non-english logic for creating templates.
            foreach (var translation in HappyBirthday.Config.translationInfo.TranslationCodes)
            {

                StardewValley.LocalizedContentManager.LanguageCode code = translation.Value;

                string basePath = Path.Combine(HappyBirthday.ModHelper.DirectoryPath, "ModAssets", "Dialogue", HappyBirthday.Config.translationInfo.getFileExtentionForDirectory(translation.Key));
                string stringsFile = Path.Combine("ModAssets", "Dialogue", HappyBirthday.Config.translationInfo.getFileExtentionForDirectory(translation.Key), HappyBirthday.Config.translationInfo.getJSONForTranslation("TranslatedStrings", translation.Key));


                if (!File.Exists(Path.Combine(HappyBirthday.ModHelper.DirectoryPath, stringsFile)))
                {
                    HappyBirthday.ModHelper.Data.WriteJsonFile<Dictionary<string, string>>(stringsFile, this.translatedStrings[code]);
                }
                else
                    this.translatedStrings[code] = HappyBirthday.ModHelper.Data.ReadJsonFile<Dictionary<string, string>>(stringsFile);

            }
        }

        /// <summary>
        /// Loads some strings from the StringsFromCS file for affectionate spouse words.
        /// </summary>
        /// <returns></returns>
        public string getAffectionateSpouseWord()
        {

            List<string> words = new List<string>();
            string dict = Path.Combine("Strings", "StringsFromCSFiles");
            if (Game1.player.IsMale)
            {

                words.Add(HappyBirthday.Config.translationInfo.LoadStringFromXNBFile(dict, "NPC.cs.4507", HappyBirthday.Config.translationInfo.CurrentTranslation));
                words.Add(HappyBirthday.Config.translationInfo.LoadStringFromXNBFile(dict, "NPC.cs.4509", HappyBirthday.Config.translationInfo.CurrentTranslation));
                words.Add(HappyBirthday.Config.translationInfo.LoadStringFromXNBFile(dict, "NPC.cs.4511", HappyBirthday.Config.translationInfo.CurrentTranslation));
                words.Add(HappyBirthday.Config.translationInfo.LoadStringFromXNBFile(dict, "NPC.cs.4514", HappyBirthday.Config.translationInfo.CurrentTranslation));
                words.Add(HappyBirthday.Config.translationInfo.LoadStringFromXNBFile(dict, "NPC.cs.4515", HappyBirthday.Config.translationInfo.CurrentTranslation));


            }
            else
            {
                words.Add(HappyBirthday.Config.translationInfo.LoadStringFromXNBFile(dict, "NPC.cs.4512", HappyBirthday.Config.translationInfo.CurrentTranslation));
                words.Add(HappyBirthday.Config.translationInfo.LoadStringFromXNBFile(dict, "NPC.cs.4513", HappyBirthday.Config.translationInfo.CurrentTranslation));

            }
            words.Add(HappyBirthday.Config.translationInfo.LoadStringFromXNBFile(dict, "NPC.cs.4508", HappyBirthday.Config.translationInfo.CurrentTranslation));
            words.Add(HappyBirthday.Config.translationInfo.LoadStringFromXNBFile(dict, "NPC.cs.4510", HappyBirthday.Config.translationInfo.CurrentTranslation));
            words.Add(HappyBirthday.Config.translationInfo.LoadStringFromXNBFile(dict, "NPC.cs.4516", HappyBirthday.Config.translationInfo.CurrentTranslation));
            words.Add(HappyBirthday.Config.translationInfo.LoadStringFromXNBFile(dict, "NPC.cs.4517", HappyBirthday.Config.translationInfo.CurrentTranslation));
            words.Add(HappyBirthday.Config.translationInfo.LoadStringFromXNBFile(dict, "NPC.cs.4518", HappyBirthday.Config.translationInfo.CurrentTranslation));
            words.Add(HappyBirthday.Config.translationInfo.LoadStringFromXNBFile(dict, "NPC.cs.4519", HappyBirthday.Config.translationInfo.CurrentTranslation));
            words.Add(HappyBirthday.Config.translationInfo.LoadStringFromXNBFile(dict, "NPC.cs.4522", HappyBirthday.Config.translationInfo.CurrentTranslation));
            words.Add(HappyBirthday.Config.translationInfo.LoadStringFromXNBFile(dict, "NPC.cs.4523", HappyBirthday.Config.translationInfo.CurrentTranslation));

            if (HappyBirthday.Config.translationInfo.CurrentTranslation == Framework.TranslationInfo.LanguageName.English)
            {
                words.Add("Pumpkin"); //Because this is cute.
            }

            string s = words[Game1.random.Next(0, words.Count - 1)];

            return s.ToLowerInvariant();
        }

        public string getTimeOfDayString()
        {
            if (Game1.timeOfDay >= 600 && Game1.timeOfDay < 1200)
            {
                return "morning";
            }
            else if (Game1.timeOfDay >= 1200 && Game1.timeOfDay < 600)
            {
                return "afternoon";
            }
            else return "evening";
        }

        /// <summary>
        /// Gets the actual birthday message.
        /// </summary>
        /// <param name="SpeakerName"></param>
        /// <returns></returns>
        public string generateSpouseBirthdayDialogue(string SpeakerName)
        {
            return this.spouseEnglishGeneratedMessages[SpeakerName].Invoke(SpeakerName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SpeakerName"></param>
        /// <returns></returns>
        private string generateSpouseMessage(string SpeakerName)
        {
            StringBuilder b = new StringBuilder();
            switch (SpeakerName)
            {
                case ("Alex"):
                    b.Append("Hey ");
                    b.Append(this.getAffectionateSpouseWord());
                    b.Append(".");
                    b.Append("I'm so glad that I married you. You make every day feel like winning a sports match. Happy birthday! $l");

                    break;
                case ("Elliott"):
                    b.Append("Good ");
                    b.Append(this.getTimeOfDayString());
                    b.Append(" ");
                    b.Append(this.getAffectionateSpouseWord());
                    b.Append(".");
                    b.Append("I was just thinking on how you have been a muse for my work. You inspire me every day I spend with you. Happy birthday! $l");

                    break;
                case ("Harvey"):
                    b.Append("Good ");
                    b.Append(this.getTimeOfDayString());
                    b.Append(" ");
                    b.Append(this.getAffectionateSpouseWord());
                    b.Append(".");
                    b.Append("I was just thinking on how invigorated I've felt since marrying you. When I look at you I feel as I'm positively glowing with joy. Happy birthday! $l");

                    break;
                case ("Sam"):
                    b.Append("Good ");
                    b.Append(this.getTimeOfDayString());
                    b.Append(" ");
                    b.Append(this.getAffectionateSpouseWord());
                    b.Append(".");
                    b.Append("You know I never saw myself settling down before I met you, but now that I have I feel like I never want to look back. Happy birthday! $l");

                    break;
                case ("Sebastian"):
                    b.Append("I was never a big celebrater of birthdays but with you, today is special. Happy birthday ");
                    b.Append(this.getAffectionateSpouseWord());
                    b.Append(".");
                    b.Append("$l");

                    break;
                case ("Shane"):
                    b.Append("I never though I'd enjoy annything but drinking on birthdays but you have shown me how to live a great life. Happy birthday ");
                    b.Append(this.getAffectionateSpouseWord());
                    b.Append(".");
                    b.Append("$l");

                    break;
                case ("Abigail"):
                    b.Append("Hey ");
                    b.Append(this.getAffectionateSpouseWord());
                    b.Append(".");
                    b.Append("You know every day feels like a great adventure with you! Happy birthday!");
                    b.Append("$l");
                    break;

                case ("Emily"):
                    b.Append("You know whenever I tried to read my fortune I never thought that I'd be as happy as I am with you! Happy birthday ");
                    b.Append(this.getAffectionateSpouseWord());
                    b.Append("!");
                    b.Append("$l");

                    break;
                case ("Haley"):
                    b.Append("You know ");
                    b.Append(this.getAffectionateSpouseWord());
                    b.Append(", ");
                    b.Append("we have come a long way since we first met but I'm glad we took this journey together. Every day feels picture perfect with you. Happy birthday!");
                    b.Append("$l");

                    break;
                case ("Leah"):
                    b.Append("Hey, ");
                    b.Append(this.getTimeOfDayString());
                    b.Append(" ");
                    b.Append(this.getAffectionateSpouseWord());
                    b.Append(".");
                    b.Append("If it wasn't for you who knows what I would be doing right now. I might sculpt wood, but being with you has helped scult my life.");
                    b.Append("Happy birthday!");
                    b.Append("$l");

                    break;
                case ("Maru"):
                    b.Append("Happy birthday");
                    b.Append(" ");
                    b.Append(this.getAffectionateSpouseWord());
                    b.Append(".");
                    b.Append("I always thought I had to look into space to find a shooting star but when I look at you I realize I already have one.");
                    b.Append("$l");
                    break;
                case ("Penny"):
                    b.Append("Happy birthday");
                    b.Append(" ");
                    b.Append(this.getAffectionateSpouseWord());
                    b.Append(". ");
                    b.Append("I always read about happy endings in books but being with you makes me realize that it continues well into the future. Here is to many more years to come.");
                    b.Append("$l");
                    break;
                default:
                    b.Append("Good ");
                    b.Append(this.getTimeOfDayString());
                    b.Append(" ");
                    b.Append(this.getAffectionateSpouseWord());
                    b.Append(".");
                    b.Append("I hope we get to spend many more years together. Happy birthday.");
                    b.Append("$h");

                    break;
            }
            return b.ToString();
        }


        public string getBirthdayMessage(string NPC)
        {
            if (Game1.player.friendshipData.ContainsKey(NPC))
            {
                if (Game1.player.getSpouse() != null) {
                    if (Game1.player.getSpouse().Name.Equals(NPC))
                    {
                        if (string.IsNullOrEmpty(this.spouseBirthdayWishes[NPC]))
                        {
                            return this.generateSpouseBirthdayDialogue(NPC);
                        }
                        else
                        {
                            return this.spouseBirthdayWishes[NPC];
                        }
                    }
                    else
                    {
                        if (this.birthdayWishes.ContainsKey(NPC))
                        {
                            return this.birthdayWishes[NPC];
                        }
                    }
                }
                else
                {
                    if (this.birthdayWishes.ContainsKey(NPC))
                    {
                        return this.birthdayWishes[NPC];
                    }
                    else
                    {
                        return "Happy birthday @!";
                    }
                }

            }
            else
            {
                if (this.birthdayWishes.ContainsKey(NPC))
                {
                    return this.birthdayWishes[NPC];
                }
                else
                {
                    return "Happy birthday @!";
                }
            }
            return "Happy birthday @!";
        }
    }
}


