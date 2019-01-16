using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Objects;
namespace Vocalization.Framework
{
    public class Vocabulary
    {
        public static string[] getRandomNegativeItemSlanderNouns(LanguageName language)
        {
            string[] strArray = Vocalization.config.translationInfo.LoadString(Path.Combine("Strings", "Lexicon:RandomNegativeItemNoun"), language).Split('#');
            return strArray;
        }

        public static string[] getRandomDeliciousAdjectives(LanguageName language, NPC n = null)
        {
            string[] strArray;
            if (n != null && n.Age == 2)
                strArray = Vocalization.config.translationInfo.LoadString(Path.Combine("Strings", "Lexicon:RandomDeliciousAdjective_Child"), language).Split('#');
            else
                strArray = Vocalization.config.translationInfo.LoadString(Path.Combine("Strings", "Lexicon:RandomDeliciousAdjective"), language).Split('#');
            return strArray;
        }

        public static string[] getRandomNegativeFoodAdjectives(LanguageName language, NPC n = null)
        {
            string[] strArray;
            if (n != null && n.Age == 2)
                strArray = Vocalization.config.translationInfo.LoadString(Path.Combine("Strings", "Lexicon:RandomNegativeFoodAdjective_Child"), language).Split('#');
            else if (n != null && n.Manners == 1)
                strArray = Vocalization.config.translationInfo.LoadString(Path.Combine("Strings", "Lexicon:RandomNegativeFoodAdjective_Polite"), language).Split('#');
            else
                strArray = Vocalization.config.translationInfo.LoadString(Path.Combine("Strings", "Lexicon:RandomNegativeFoodAdjective"), language).Split('#');
            return strArray;
        }

        public static string[] getRandomSlightlyPositiveAdjectivesForEdibleNoun(LanguageName language, NPC n = null)
        {
            string[] strArray = Vocalization.config.translationInfo.LoadString(Path.Combine("Strings", "Lexicon:RandomSlightlyPositiveFoodAdjective"), language).Split('#');
            return strArray;
        }

        public static string[] getRandomNegativeAdjectivesForEventOrPerson(LanguageName language, NPC n = null)
        {
            Random random = new Random((int)Game1.stats.DaysPlayed + (int)Game1.uniqueIDForThisGame / 2);
            string[] strArray;
            if (n != null && n.Age != 0)
                strArray = Vocalization.config.translationInfo.LoadString(Path.Combine("Strings", "Lexicon:RandomNegativeAdjective_Child"), language).Split('#');
            else if (n != null && n.Gender == 0)
                strArray = Vocalization.config.translationInfo.LoadString(Path.Combine("Strings", "Lexicon:RandomNegativeAdjective_AdultMale"), language).Split('#');
            else if (n != null && n.Gender == 1)
                strArray = Vocalization.config.translationInfo.LoadString(Path.Combine("Strings", "Lexicon:RandomNegativeAdjective_AdultFemale"), language).Split('#');
            else
                strArray = Vocalization.config.translationInfo.LoadString(Path.Combine("Strings", "Lexicon:RandomNegativeAdjective_PlaceOrEvent"), language).Split('#');
            return strArray;
        }

        public static string[] getRandomPositiveAdjectivesForEventOrPerson(LanguageName language, NPC n = null)
        {
            //Random random = new Random((int)Game1.stats.DaysPlayed + (int)Game1.uniqueIDForThisGame / 2);
            string[] strArray;
            if (n != null && n.Age != 0)
                strArray = Vocalization.config.translationInfo.LoadString(Path.Combine("Strings", "Lexicon:RandomPositiveAdjective_Child"), language).Split('#');
            else if (n != null && n.Gender == 0)
                strArray = Vocalization.config.translationInfo.LoadString(Path.Combine("Strings", "Lexicon:RandomPositiveAdjective_AdultMale"), language).Split('#');
            else if (n != null && n.Gender == 1)
                strArray = Vocalization.config.translationInfo.LoadString(Path.Combine("Strings", "Lexicon:RandomPositiveAdjective_AdultFemale"), language).Split('#');
            else
                strArray = Vocalization.config.translationInfo.LoadString(Path.Combine("Strings", "Lexicon:RandomPositiveAdjective_PlaceOrEvent"), language).Split('#');
            return strArray;
        }

        public static List<string> getSeasons()
        {
            return new List<string> { "spring", "summer", "fall", "winter" };
        }

        /// <summary>Gets a list of all of the possible cooking recipes in Stardew Valley.</summary>
        public static List<string> getAllCookingRecipes(LanguageName language)
        {
            List<string> recipes = new List<string>();
            Dictionary<string, string> cookingDict = Game1.content.Load<Dictionary<string, string>>(Path.Combine("Data", "TV", Vocalization.config.translationInfo.getXNBForTranslation("CookingChannel", language)));

            if (language == LanguageName.English)
            {
                foreach (KeyValuePair<string, string> pair in cookingDict)
                {
                    string name = pair.Value.Split('/').ElementAt(0);
                    recipes.Add(name);
                }
            }
            else
            {
                foreach (KeyValuePair<string, string> pair in cookingDict)
                {
                    string[] data = pair.Value.Split('/');
                    string name = data.ElementAt(data.Length - 1);
                    recipes.Add(name);
                }
            }
            return recipes;
        }

        public static List<string> getCarpenterStock(LanguageName language)
        {
            List<string> stock = new List<string>();
            Vocalization.config.translationInfo.changeLocalizedContentManagerFromTranslation(language);

            for (int i = 0; i <= 1854; i++)
            {
                try
                {
                    Furniture f = new Furniture(i, Vector2.Zero);
                    stock.Add(f.DisplayName);
                }
                catch { }
            }
            Vocalization.config.translationInfo.resetLocalizationCode();
            return stock;
        }

        public static List<string> getMerchantStock(LanguageName language)
        {
            List<string> stock = new List<string>();
            Dictionary<int, string> objDict = Game1.content.Load<Dictionary<int, string>>(Path.Combine("Data", Vocalization.config.translationInfo.getXNBForTranslation("ObjectInformation", language)));
            //Vocalization.ModMonitor.Log("LOAD THE OBJECT INFO: ", LogLevel.Alert);
            foreach (KeyValuePair<int, string> pair in objDict)
            {
                for (int i = 0; i <= 3; i++)
                {
                    StardewValley.Object obj = new StardewValley.Object(pair.Key, 1, false, -1, i);
                    stock.Add(obj.DisplayName);
                }
            }
            foreach (string item in getCarpenterStock(language))
                stock.Add(item);
            return stock;
        }

        public static string getProperArticleForWord(string displayName, LanguageName language)
        {
            Vocalization.config.translationInfo.changeLocalizedContentManagerFromTranslation(language);
            string s = Lexicon.getProperArticleForWord(displayName);
            Vocalization.config.translationInfo.resetLocalizationCode();
            return s;
        }
    }
}
