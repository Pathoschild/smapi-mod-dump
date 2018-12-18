using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sObj = StardewValley.Object;
namespace Vocalization.Framework
{
    public class Vocabulary
    {

        public static string[] getRandomNegativeItemSlanderNouns(string translation)
        {
            string[] strArray = Vocalization.config.translationInfo.LoadString(Path.Combine("Strings", "Lexicon:RandomNegativeItemNoun"), translation).Split('#');
            return strArray;
        }

        public static string[] getRandomDeliciousAdjectives(string translation, NPC n = null)
        {
            string[] strArray;
            if (n != null && n.Age == 2)
                strArray = Vocalization.config.translationInfo.LoadString(Path.Combine("Strings", "Lexicon:RandomDeliciousAdjective_Child"), translation).Split('#');
            else
                strArray = Vocalization.config.translationInfo.LoadString(Path.Combine("Strings", "Lexicon:RandomDeliciousAdjective"), translation).Split('#');
            return strArray;
        }

        public static string[] getRandomNegativeFoodAdjectives(string translation, NPC n = null)
        {
            string[] strArray;
            if (n != null && n.Age == 2)
                strArray = Vocalization.config.translationInfo.LoadString(Path.Combine("Strings", "Lexicon:RandomNegativeFoodAdjective_Child"), translation).Split('#');
            else if (n != null && n.Manners == 1)
                strArray = Vocalization.config.translationInfo.LoadString(Path.Combine("Strings", "Lexicon:RandomNegativeFoodAdjective_Polite"), translation).Split('#');
            else
                strArray = Vocalization.config.translationInfo.LoadString(Path.Combine("Strings", "Lexicon:RandomNegativeFoodAdjective"), translation).Split('#');
            return strArray;
        }

        public static string[] getRandomSlightlyPositiveAdjectivesForEdibleNoun(string translation, NPC n = null)
        {
            string[] strArray = Vocalization.config.translationInfo.LoadString(Path.Combine("Strings", "Lexicon:RandomSlightlyPositiveFoodAdjective"), translation).Split('#');
            return strArray;
        }

        public static string[] getRandomNegativeAdjectivesForEventOrPerson(string translation, NPC n = null)
        {
            Random random = new Random((int)Game1.stats.DaysPlayed + (int)Game1.uniqueIDForThisGame / 2);
            string[] strArray;
            if (n != null && n.Age != 0)
                strArray = Vocalization.config.translationInfo.LoadString(Path.Combine("Strings", "Lexicon:RandomNegativeAdjective_Child"), translation).Split('#');
            else if (n != null && n.Gender == 0)
                strArray = Vocalization.config.translationInfo.LoadString(Path.Combine("Strings", "Lexicon:RandomNegativeAdjective_AdultMale"), translation).Split('#');
            else if (n != null && n.Gender == 1)
                strArray = Vocalization.config.translationInfo.LoadString(Path.Combine("Strings", "Lexicon:RandomNegativeAdjective_AdultFemale"), translation).Split('#');
            else
                strArray = Vocalization.config.translationInfo.LoadString(Path.Combine("Strings", "Lexicon:RandomNegativeAdjective_PlaceOrEvent"), translation).Split('#');
            return strArray;
        }

        public static string[] getRandomPositiveAdjectivesForEventOrPerson(string translation, NPC n = null)
        {
            Random random = new Random((int)Game1.stats.DaysPlayed + (int)Game1.uniqueIDForThisGame / 2);
            string[] strArray;
            if (n != null && n.Age != 0)
                strArray = Vocalization.config.translationInfo.LoadString(Path.Combine("Strings", "Lexicon:RandomPositiveAdjective_Child"), translation).Split('#');
            else if (n != null && n.Gender == 0)
                strArray = Vocalization.config.translationInfo.LoadString(Path.Combine("Strings", "Lexicon:RandomPositiveAdjective_AdultMale"), translation).Split('#');
            else if (n != null && n.Gender == 1)
                strArray = Vocalization.config.translationInfo.LoadString(Path.Combine("Strings", "Lexicon:RandomPositiveAdjective_AdultFemale"), translation).Split('#');
            else
                strArray = Vocalization.config.translationInfo.LoadString(Path.Combine("Strings", "Lexicon:RandomPositiveAdjective_PlaceOrEvent"), translation).Split('#');
            return strArray;
        }


        public static List<string> getSeasons()
        {
            List<string> seasons = new List<string>();
            seasons.Add("spring");
            seasons.Add("summer");
            seasons.Add("fall");
            seasons.Add("winter");
            return seasons;
        }

        /// <summary>
        /// Gets a list of all of the possible cooking recipes in Stardew Valley.
        /// </summary>
        /// <param name="translation"></param>
        /// <returns></returns>
        public static List<string> getAllCookingRecipes(string translation)
        {
            List<string> recipes = new List<string>();
            Dictionary<string, string> cookingDict = Game1.content.Load<Dictionary<string, string>>(Path.Combine("Data", "TV", Vocalization.config.translationInfo.getXNBForTranslation("CookingChannel", translation)));

            if (Vocalization.config.translationInfo.getTranslationNameFromPath(translation) == "English")
            {
                foreach(KeyValuePair<string,string> pair in cookingDict)
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

        public static List<string> getCarpenterStock(string translation)
        {
            List<string> stock = new List<string>();
            Vocalization.config.translationInfo.changeLocalizedContentManagerFromTranslation(translation);

            for(int i=0; i <= 1854; i++)
            {
                try
                {
                    Furniture f = new Furniture(i, Vector2.Zero);
                    stock.Add(f.DisplayName);
                }
                catch(Exception err)
                {

                }
            }
            Vocalization.config.translationInfo.resetLocalizationCode();
            return stock;
        }

        public static List<string> getMerchantStock(string translation)
        {
            List<string> stock = new List<string>();
            Dictionary<int, string> objDict = Game1.content.Load<Dictionary<int, string>>(Path.Combine("Data", Vocalization.config.translationInfo.getXNBForTranslation("ObjectInformation", translation)));
            //Vocalization.ModMonitor.Log("LOAD THE OBJECT INFO: ", LogLevel.Alert);
            foreach (KeyValuePair<int, string> pair in objDict)
            {
                for (int i = 0; i <= 3; i++)
                {
                    StardewValley.Object obj = new StardewValley.Object(pair.Key, 1, false, -1, i);
                    stock.Add(obj.DisplayName);
                }
            }
            foreach(var item in getCarpenterStock(translation))
            {
                stock.Add(item);
            }
            return stock;
        }

        public static string getProperArticleForWord(string displayName, string translation)
        {
            Vocalization.config.translationInfo.changeLocalizedContentManagerFromTranslation(translation);
            string s=Lexicon.getProperArticleForWord(displayName);
            Vocalization.config.translationInfo.resetLocalizationCode();
            return s;
        }
    }
}
