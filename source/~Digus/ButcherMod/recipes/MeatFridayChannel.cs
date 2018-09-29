using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnimalHusbandryMod.common;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Objects;
using AnimalHusbandryMod.cooking;
using PyTK.CustomTV;

namespace AnimalHusbandryMod.recipes
{
    public class MeatFridayChannel
    {
        private TemporaryAnimatedSprite _queenSprite;

        private readonly Dictionary<int, string> _recipes;

        public MeatFridayChannel()
        {
            this._recipes = new Dictionary<int, string>();
            _recipes.Add(2, Cooking.RoastDuck.GetRecipeChannelString());
            _recipes.Add(3, Cooking.Bacon.GetRecipeChannelString());
            _recipes.Add(4, Cooking.SummerSausage.GetRecipeChannelString());
            _recipes.Add(5, Cooking.OrangeChicken.GetRecipeChannelString());
            _recipes.Add(6, Cooking.SteakFajitas.GetRecipeChannelString());
            _recipes.Add(7, Cooking.RabbitAuVin.GetRecipeChannelString());
            _recipes.Add(8, Cooking.WinterDuck.GetRecipeChannelString());
        }

        public void CheckChannelDay()
        {
            CustomTVMod.removeKey("MeatFriday");

            if(SDate.Now().DayOfWeek == DayOfWeek.Friday)
            {
                int recipe = GetRecipeNumber();
                if (recipe >= 2)
                {
                    Boolean rerun = Game1.stats.DaysPlayed % 2 == 0U;

                    string name = DataLoader.i18n.Get("TV.MeatFriday.ChannelDisplayName");
                    if (rerun)
                    {
                        name += " " + DataLoader.i18n.Get("TV.MeatFriday.ReRunDisplaySuffix"); ;
                    }
                    CustomTVMod.addChannel("MeatFriday", name, ShowQueenAnnouncement);
                }
            }
        }

        private static int GetRecipeNumber()
        {
            return (int)(Game1.stats.DaysPlayed % 112U / 14) + 1;
        }

        private void ShowQueenAnnouncement(TV tv, TemporaryAnimatedSprite sprite, StardewValley.Farmer farmer, string answer)
        {
            _queenSprite = new TemporaryAnimatedSprite(Game1.mouseCursorsName, new Rectangle(602, 361, 42, 28), 150f, 2, 999999, tv.getScreenPosition(), false, false, (float)((double)(tv.boundingBox.Bottom - 1) / 10000.0 + 9.99999974737875E-06), 0.0f, Color.White, tv.getScreenSizeModifier(), 0.0f, 0.0f, 0.0f, false);
            CustomTVMod.showProgram(_queenSprite, DataLoader.i18n.Get("TV.MeatFriday.Announcement"), ShowRecipePresentation);
        }

        private void ShowRecipePresentation()
        {
            string text = _recipes[GetRecipeNumber()].Split('/')[1];
            CustomTVMod.showProgram(_queenSprite, text, AddRecipe);
        }

        private void AddRecipe()
        {
            string[] recipeSplit = _recipes[GetRecipeNumber()].Split('/');
            string recipeKey = recipeSplit[0];
            string recipeName = recipeSplit[2];
            string addRecipeText;
            if (!Game1.player.cookingRecipes.ContainsKey(recipeKey))
            {
                addRecipeText = Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13153", (object) recipeName);
                Game1.player.cookingRecipes.Add(recipeKey, 0);
            }
            else
            {
                addRecipeText = Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13151", (object) recipeName);
            }
            CustomTVMod.showProgram(_queenSprite, addRecipeText, CustomTVMod.endProgram);
        }
    }
}
