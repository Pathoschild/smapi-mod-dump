using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailFrameworkMod;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Locations;
using AnimalHusbandryMod.cooking;
using DataLoader = AnimalHusbandryMod.common.DataLoader;

namespace AnimalHusbandryMod.recipes
{
    public class RecipesLoader : IAssetEditor
    {
        public MeatFridayChannel MeatFridayChannel { get; }

        public RecipesLoader()
        {
            MeatFridayChannel = new MeatFridayChannel();
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals("Data\\CookingRecipes");
        }

        public void Edit<T>(IAssetData asset)
        {
            var data = asset.AsDictionary<string, string>().Data;
            AddCookingRecipe(data, Cooking.Meatloaf);
            AddCookingRecipe(data, Cooking.OrangeChicken);
            AddCookingRecipe(data, Cooking.MonteCristo);
            AddCookingRecipe(data, Cooking.BaconCheeseburger);
            AddCookingRecipe(data, Cooking.RoastDuck);
            AddCookingRecipe(data, Cooking.RabbitAuVin);
            AddCookingRecipe(data, Cooking.SteakFajitas);
            AddCookingRecipe(data, Cooking.GlazedHam);
            AddCookingRecipe(data, Cooking.SummerSausage);
            AddCookingRecipe(data, Cooking.SweetAndSourPork);
            AddCookingRecipe(data, Cooking.RabbitStew);
            AddCookingRecipe(data, Cooking.WinterDuck);
            AddCookingRecipe(data, Cooking.SteakWithMushrooms);
            AddCookingRecipe(data, Cooking.CowboyDinner);
            AddCookingRecipe(data, Cooking.Bacon);
        }

        private void AddCookingRecipe(IDictionary<string, string> data, Cooking cooking)
        {
            data[cooking.GetDescription()] = cooking.GetRecipeString();
        }

        public void LoadMails()
        {
            MailDao.SaveLetter(new Letter("meatloafRecipe", DataLoader.i18n.Get("Cooking.Meatloaf.Letter"), Cooking.Meatloaf.GetDescription(), (letter) => !Game1.player.cookingRecipes.ContainsKey(letter.Recipe) && GetNpcFriendship("Lewis") >= 9 * 250 && GetNpcFriendship("Marnie") >= 7 * 250, (l) => Game1.player.mailReceived.Add(l.Id)) {GroupId = "AHM.CookingRecipe", Title = DataLoader.i18n.Get("Cooking.Meatloaf.Letter.Title") });

            MailDao.SaveLetter(new Letter("baconCheeseburgerRecipe", DataLoader.i18n.Get("Cooking.BaconCheeseburger.Letter"), Cooking.BaconCheeseburger.GetDescription(), (letter) => !Game1.player.cookingRecipes.ContainsKey(letter.Recipe) && GetNpcFriendship("Gus") >= 9 * 250 && SDate.Now() > new SDate(16, "fall", 1), (l) => Game1.player.mailReceived.Add(l.Id)) { GroupId = "AHM.CookingRecipe", Title = DataLoader.i18n.Get("Cooking.BaconCheeseburger.Letter.Title") });

            MailDao.SaveLetter(new Letter("sweetAndSourPorkRecipe", DataLoader.i18n.Get("Cooking.SweetAndSourPork.Letter"), Cooking.SweetAndSourPork.GetDescription(), (letter) => !Game1.player.cookingRecipes.ContainsKey(letter.Recipe) && GetNpcFriendship("Jodi") >= 9 * 250 && GetNpcFriendship("Kent") >= 9 * 250, (l) => Game1.player.mailReceived.Add(l.Id)) { GroupId = "AHM.CookingRecipe", Title = DataLoader.i18n.Get("Cooking.SweetAndSourPork.Letter.Title") });

            Func<Letter, bool> glazedHamCondition = (letter) => !Game1.player.cookingRecipes.ContainsKey(letter.Recipe)
                                        && GetNpcFriendship("Clint") >= 9
                                        && Game1.stats.GeodesCracked > 80;
            MailDao.SaveLetter(new Letter("glazedHamRecipe", DataLoader.i18n.Get("Cooking.GlazedHam.Letter"), Cooking.GlazedHam.GetDescription(), glazedHamCondition, (l) => Game1.player.mailReceived.Add(l.Id)) { GroupId = "AHM.CookingRecipe", Title = DataLoader.i18n.Get("Cooking.GlazedHam.Letter.Title") });

            MailDao.SaveLetter(new Letter("cowboyDinnerkRecipe", DataLoader.i18n.Get("Cooking.CowboyDinner.Letter"), Cooking.CowboyDinner.GetDescription(), (letter) => !Game1.player.cookingRecipes.ContainsKey(letter.Recipe) && (Game1.getLocationFromName("ArchaeologyHouse") as LibraryMuseum)?.museumPieces.Count() >= 70, (l) => Game1.player.mailReceived.Add(l.Id)) { GroupId = "AHM.CookingRecipe", Title = DataLoader.i18n.Get("Cooking.CowboyDinner.Letter.Title") });
            
            MailDao.SaveLetter(new Letter("rabbitStewRecipe", DataLoader.i18n.Get("Cooking.RabbitStew.Letter"), Cooking.RabbitStew.GetDescription(), (letter) => !Game1.player.cookingRecipes.ContainsKey(letter.Recipe) && GetNpcFriendship("Linus") >= 9 * 250 && (Game1.stats.TimesUnconscious >= 1 || Game1.player.deepestMineLevel >= 100), (l) => Game1.player.mailReceived.Add(l.Id)) { GroupId = "AHM.CookingRecipe", Title = DataLoader.i18n.Get("Cooking.RabbitStew.Letter.Title") });

            MailDao.SaveLetter(new Letter("monteCristoRecipe", DataLoader.i18n.Get("Cooking.MonteCristo.Letter"), Cooking.MonteCristo.GetDescription(), (letter) => !Game1.player.cookingRecipes.ContainsKey(letter.Recipe) && GetNpcFriendship("Leah") >= 8 * 250 && Game1.stats.ItemsForaged >= 1200, (l) => Game1.player.mailReceived.Add(l.Id)) { GroupId = "AHM.CookingRecipe", Title = DataLoader.i18n.Get("Cooking.MonteCristo.Letter.Title") });

            MailDao.SaveLetter(new Letter("steakWithMushroomsRecipe", DataLoader.i18n.Get("Cooking.SteakWithMushrooms.Letter"), Cooking.SteakWithMushrooms.GetDescription(), (letter) => !Game1.player.cookingRecipes.ContainsKey(letter.Recipe) && GetNpcFriendship("Alex") >= 8 * 250 && Game1.stats.MonstersKilled >= 1000, (l) => Game1.player.mailReceived.Add(l.Id)) { GroupId = "AHM.CookingRecipe", Title = DataLoader.i18n.Get("Cooking.SteakWithMushrooms.Letter.Title") });
        }

        private int GetNpcFriendship(string name)
        {
            if (Game1.player.friendshipData.ContainsKey(name))
            {
                return Game1.player.friendshipData[name].Points;
            }
            else
            {
                return 0;
            }
        }

        public void AddAllMeatRecipes(string arg1, string[] arg2)
        {
            if (Context.IsWorldReady)
            {
                foreach (Cooking cooking in Enum.GetValues(typeof(Cooking)))
                {
                    if (!Game1.player.cookingRecipes.ContainsKey(cooking.GetDescription()))
                    {
                        Game1.player.cookingRecipes.Add(cooking.GetDescription(), 0);
                        AnimalHusbandryModEntry.monitor.Log($"Added {cooking.GetDescription()} recipe to the player.", LogLevel.Info);
                    }
                }
            }
            else
            {
                AnimalHusbandryModEntry.monitor.Log("No player loaded to add the recipes.", LogLevel.Info);
            }
        }
    }
}
