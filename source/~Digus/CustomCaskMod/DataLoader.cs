using System.Collections.Generic;
using MailFrameworkMod;
using StardewModdingAPI;
using StardewValley;

namespace CustomCaskMod
{
    internal class DataLoader
    {
        public static IModHelper Helper;
        public static ITranslationHelper I18N;
        public static ModConfig ModConfig;
        public static Dictionary<int, float> CaskData;

        public DataLoader(IModHelper helper)
        {
            Helper = helper;
            I18N = helper.Translation;
            ModConfig = helper.ReadConfig<ModConfig>();

            CaskData = DataLoader.Helper.ReadJsonFile<Dictionary<int, float>>("data\\CaskData.json") ?? new Dictionary<int, float>() { { 342, 2.66f }, { 724, 2f } };
            DataLoader.Helper.WriteJsonFile("data\\CaskData.json", CaskData);

            if (!ModConfig.DisableLetter)
            {
                MailDao.SaveLetter
                (
                    new Letter
                    (
                        "CustomCaskRecipe"
                        , I18N.Get("CustomCask.RecipeLetter")
                        , "Cask"
                        , (l) => !Game1.player.mailReceived.Contains(l.Id) && !Game1.player.mailReceived.Contains("CustomCask.Letter") && (Utility.getHomeOfFarmer(Game1.player).upgradeLevel >= 3 || ModConfig.EnableCasksAnywhere) && !Game1.player.craftingRecipes.ContainsKey("Cask")
                        , (l) => Game1.player.mailReceived.Add(l.Id)
                    )
                );
                MailDao.SaveLetter
                (
                    new Letter
                    (
                        "CustomCask"
                        , I18N.Get("CustomCask.Letter")
                        , (l) => !Game1.player.mailReceived.Contains(l.Id) && !Game1.player.mailReceived.Contains("CustomCask.RecipeLetter") && (Utility.getHomeOfFarmer(Game1.player).upgradeLevel >= 3 || ModConfig.EnableCasksAnywhere) && Game1.player.craftingRecipes.ContainsKey("Cask")
                        , (l) => Game1.player.mailReceived.Add(l.Id)
                    )
                );
            }
        }
    }
}