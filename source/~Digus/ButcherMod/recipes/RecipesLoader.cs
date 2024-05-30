/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MailFrameworkMod;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Locations;
using AnimalHusbandryMod.cooking;
using DataLoader = AnimalHusbandryMod.common.DataLoader;
using AnimalHusbandryMod.common;
using StardewModdingAPI.Events;

namespace AnimalHusbandryMod.recipes
{
    public class RecipesLoader
    {
        public RecipesLoader()
        {
            TvController.AddChannel(new MeatFridayChannel());
        }

        public void Edit(object sender, AssetRequestedEventArgs args)
        {
            if (args.NameWithoutLocale.IsEquivalentTo("Data/CookingRecipes"))
            {
                args.Edit(asset =>
                {
                    if (DataLoader.ModConfig.DisableMeat) return;
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
                });

            }
        }

        private void AddCookingRecipe(IDictionary<string, string> data, Cooking cooking)
        {
            data[cooking.GetDescription()] = cooking.GetRecipeString();
        }

        public static void LoadMails()
        {
            if (DataLoader.ModConfig.DisableMeat) return;

            MailRepository.SaveLetter(new Letter("meatloafRecipe", DataLoader.i18n.Get("Cooking.Meatloaf.Letter"), Cooking.Meatloaf.GetDescription(), (letter) => !Game1.player.cookingRecipes.ContainsKey(letter.Recipe) && GetNpcFriendship("Lewis") >= 9 * 250 && GetNpcFriendship("Marnie") >= 7 * 250 && !DataLoader.ModConfig.DisableMeat, (l) => Game1.player.mailReceived.Add(l.Id)) {GroupId = "AHM.CookingRecipe", Title = DataLoader.i18n.Get("Cooking.Meatloaf.Letter.Title") });

            MailRepository.SaveLetter(new Letter("baconCheeseburgerRecipe", DataLoader.i18n.Get("Cooking.BaconCheeseburger.Letter"), Cooking.BaconCheeseburger.GetDescription(), (letter) => !Game1.player.cookingRecipes.ContainsKey(letter.Recipe) && GetNpcFriendship("Gus") >= 9 * 250 && SDate.Now() > new SDate(16, "fall", 1) && !DataLoader.ModConfig.DisableMeat, (l) => Game1.player.mailReceived.Add(l.Id)) { GroupId = "AHM.CookingRecipe", Title = DataLoader.i18n.Get("Cooking.BaconCheeseburger.Letter.Title") });

            MailRepository.SaveLetter(new Letter("sweetAndSourPorkRecipe", DataLoader.i18n.Get("Cooking.SweetAndSourPork.Letter"), Cooking.SweetAndSourPork.GetDescription(), (letter) => !Game1.player.cookingRecipes.ContainsKey(letter.Recipe) && GetNpcFriendship("Jodi") >= 9 * 250 && GetNpcFriendship("Kent") >= 9 * 250 && !DataLoader.ModConfig.DisableMeat, (l) => Game1.player.mailReceived.Add(l.Id)) { GroupId = "AHM.CookingRecipe", Title = DataLoader.i18n.Get("Cooking.SweetAndSourPork.Letter.Title") });

            Func<Letter, bool> glazedHamCondition = (letter) => !Game1.player.cookingRecipes.ContainsKey(letter.Recipe)
                                        && GetNpcFriendship("Clint") >= 9
                                        && Game1.stats.GeodesCracked > 80
                                        && !DataLoader.ModConfig.DisableMeat;
            MailRepository.SaveLetter(new Letter("glazedHamRecipe", DataLoader.i18n.Get("Cooking.GlazedHam.Letter"), Cooking.GlazedHam.GetDescription(), glazedHamCondition, (l) => Game1.player.mailReceived.Add(l.Id)) { GroupId = "AHM.CookingRecipe", Title = DataLoader.i18n.Get("Cooking.GlazedHam.Letter.Title") });

            MailRepository.SaveLetter(new Letter("cowboyDinnerkRecipe", DataLoader.i18n.Get("Cooking.CowboyDinner.Letter"), Cooking.CowboyDinner.GetDescription(), (letter) => !Game1.player.cookingRecipes.ContainsKey(letter.Recipe) && (Game1.getLocationFromName("ArchaeologyHouse") as LibraryMuseum)?.museumPieces.Count() >= 70 && !DataLoader.ModConfig.DisableMeat, (l) => Game1.player.mailReceived.Add(l.Id)) { GroupId = "AHM.CookingRecipe", Title = DataLoader.i18n.Get("Cooking.CowboyDinner.Letter.Title") });

            MailRepository.SaveLetter(new Letter("rabbitStewRecipe", DataLoader.i18n.Get("Cooking.RabbitStew.Letter"), Cooking.RabbitStew.GetDescription(), (letter) => !Game1.player.cookingRecipes.ContainsKey(letter.Recipe) && GetNpcFriendship("Linus") >= 9 * 250 && (Game1.stats.TimesUnconscious >= 1 || Game1.player.deepestMineLevel >= 100) && !DataLoader.ModConfig.DisableMeat, (l) => Game1.player.mailReceived.Add(l.Id)) { GroupId = "AHM.CookingRecipe", Title = DataLoader.i18n.Get("Cooking.RabbitStew.Letter.Title") });

            MailRepository.SaveLetter(new Letter("monteCristoRecipe", DataLoader.i18n.Get("Cooking.MonteCristo.Letter"), Cooking.MonteCristo.GetDescription(), (letter) => !Game1.player.cookingRecipes.ContainsKey(letter.Recipe) && GetNpcFriendship("Leah") >= 8 * 250 && Game1.stats.ItemsForaged >= 1200 && !DataLoader.ModConfig.DisableMeat, (l) => Game1.player.mailReceived.Add(l.Id)) { GroupId = "AHM.CookingRecipe", Title = DataLoader.i18n.Get("Cooking.MonteCristo.Letter.Title") });

            MailRepository.SaveLetter(new Letter("steakWithMushroomsRecipe", DataLoader.i18n.Get("Cooking.SteakWithMushrooms.Letter"), Cooking.SteakWithMushrooms.GetDescription(), (letter) => !Game1.player.cookingRecipes.ContainsKey(letter.Recipe) && GetNpcFriendship("Alex") >= 8 * 250 && Game1.stats.MonstersKilled >= 1000 && !DataLoader.ModConfig.DisableMeat, (l) => Game1.player.mailReceived.Add(l.Id)) { GroupId = "AHM.CookingRecipe", Title = DataLoader.i18n.Get("Cooking.SteakWithMushrooms.Letter.Title") });
        }

        private static int GetNpcFriendship(string name)
        {
            return Game1.player.friendshipData.ContainsKey(name) ? Game1.player.friendshipData[name].Points : 0;
        }

        public static void AddAllMeatRecipes(string arg1 = null, string[] arg2 = null)
        {
            if (Context.IsWorldReady)
            {
                var meatRecipeNumber = 0;
                foreach (Cooking cooking in Enum.GetValues(typeof(Cooking)))
                {
                    if (!Game1.player.cookingRecipes.ContainsKey(cooking.GetDescription()))
                    {
                        Game1.player.cookingRecipes.Add(cooking.GetDescription(), 0);
                        AnimalHusbandryModEntry.monitor.Log($"Added {cooking.GetDescription()} recipe to the player.", LogLevel.Info);
                        meatRecipeNumber++;
                    }
                }

                if (meatRecipeNumber > 0 && Game1.hudMessages.Count > 0)
                {
                    Regex regex = new Regex(@"[\d]+");
                    var matchCollection = regex.Matches(Game1.hudMessages.Last()?.message);
                    if (int.TryParse(matchCollection[0].Value, out var oldNumber))
                    {
                        Game1.hudMessages.Last().message = Game1.content.LoadString("Strings\\1_6_Strings:QoS_Cookbook", meatRecipeNumber + oldNumber);
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
