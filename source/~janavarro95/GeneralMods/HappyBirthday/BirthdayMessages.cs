using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omegasis.HappyBirthday
{
    public class BirthdayMessages
    {
        /// <summary>
        /// The actual birthday wishes given by an npc.
        /// </summary>
        public Dictionary<string, string> birthdayWishes;

        public Dictionary<string, string> spouseBirthdayWishes;

        /// <summary>
        /// TODO: Make this.
        /// </summary>
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

        /// <summary>
        /// Used to contain
        /// </summary>
        public Dictionary<string, string> defaultBirthdayWishes = new Dictionary<string, string>()
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

        /// <summary>
        /// Used to load all of the default birthday greetings.
        /// </summary>
        public void createBirthdayGreetings()
        {

            var serializer = JsonSerializer.Create();
            serializer.Formatting = Formatting.Indented;

            //English logic.
            string defaultPath = Path.Combine(HappyBirthday.ModHelper.DirectoryPath, "Content", "Dialogue", HappyBirthday.Config.translationInfo.currentTranslation);
            if (!Directory.Exists(defaultPath)) Directory.CreateDirectory(defaultPath);

            string birthdayFileDict=HappyBirthday.Config.translationInfo.getjsonForTranslation("BirthdayWishes", HappyBirthday.Config.translationInfo.currentTranslation);
            string path = Path.Combine( "Content", "Dialogue", HappyBirthday.Config.translationInfo.currentTranslation, birthdayFileDict);

            //Handle normal birthday wishes.
            if (!File.Exists(Path.Combine(HappyBirthday.ModHelper.DirectoryPath,path)))
            {

                HappyBirthday.ModHelper.Data.WriteJsonFile<Dictionary<string, string>>(path, defaultBirthdayWishes);
                this.birthdayWishes = defaultBirthdayWishes;
            }
            else
            {
                birthdayWishes = HappyBirthday.ModHelper.Data.ReadJsonFile<Dictionary<string, string>>(path);
            }

            //handle spouse birthday wishes.
            string spouseBirthdayFileDict = HappyBirthday.Config.translationInfo.getjsonForTranslation("SpouseBirthdayWishes", HappyBirthday.Config.translationInfo.currentTranslation);
            string spousePath = Path.Combine("Content", "Dialogue", HappyBirthday.Config.translationInfo.currentTranslation, spouseBirthdayFileDict);
            if (!File.Exists(Path.Combine(HappyBirthday.ModHelper.DirectoryPath,spousePath)))
            {
                HappyBirthday.ModMonitor.Log("Creating Spouse Messages", StardewModdingAPI.LogLevel.Alert);
                HappyBirthday.ModHelper.Data.WriteJsonFile<Dictionary<string, string>>(spousePath, defaultSpouseBirthdayWishes);
                this.spouseBirthdayWishes = defaultSpouseBirthdayWishes;
            }
            else
            {
                spouseBirthdayWishes = HappyBirthday.ModHelper.Data.ReadJsonFile<Dictionary<string, string>>(spousePath);
            }

            //Non-english logic for creating templates.
            foreach(var translation in HappyBirthday.Config.translationInfo.translationCodes)
            {
                if (translation.Key == "English") continue;
                string basePath = Path.Combine(HappyBirthday.ModHelper.DirectoryPath,"Content", "Dialogue", translation.Key);
                if (!Directory.Exists(basePath)) Directory.CreateDirectory(basePath);
                string tempBirthdayFile =Path.Combine("Content", "Dialogue", translation.Key,  HappyBirthday.Config.translationInfo.getjsonForTranslation("BirthdayWishes", translation.Key));
                string tempSpouseBirthdayFile =Path.Combine("Content", "Dialogue", translation.Key, HappyBirthday.Config.translationInfo.getjsonForTranslation("SpouseBirthdayWishes", translation.Key));


                Dictionary<string, string> tempBirthdayDict = new Dictionary<string, string>();
                if (!File.Exists(Path.Combine(HappyBirthday.ModHelper.DirectoryPath, tempBirthdayFile)))
                {
                    
                    foreach (var pair in defaultBirthdayWishes)
                    {
                        tempBirthdayDict.Add(pair.Key, "");
                    }
                    HappyBirthday.ModHelper.Data.WriteJsonFile<Dictionary<string, string>>(tempBirthdayFile, tempBirthdayDict);
                }
                else
                {
                    tempBirthdayDict = HappyBirthday.ModHelper.Data.ReadJsonFile<Dictionary<string, string>>(tempBirthdayFile);
                }


                Dictionary<string, string> tempSpouseBirthdayDict = new Dictionary<string, string>();
                if (!File.Exists(Path.Combine(HappyBirthday.ModHelper.DirectoryPath, tempSpouseBirthdayFile)))
                {
                    
                    foreach (var pair in defaultSpouseBirthdayWishes)
                    {
                        tempSpouseBirthdayDict.Add(pair.Key, "");
                    }
                    HappyBirthday.ModHelper.Data.WriteJsonFile<Dictionary<string, string>>(tempSpouseBirthdayFile, tempSpouseBirthdayDict);
                }
                else
                {
                    tempBirthdayDict = HappyBirthday.ModHelper.Data.ReadJsonFile<Dictionary<string, string>>(tempSpouseBirthdayFile);
                }

                //Set translated birthday info.
                if (HappyBirthday.Config.translationInfo.currentTranslation == translation.Key)
                {
                    this.birthdayWishes = tempBirthdayDict;
                    this.spouseBirthdayWishes = tempSpouseBirthdayDict;
                    HappyBirthday.ModMonitor.Log("Language set to: " + translation);
                }

            }
        }


    }
}
