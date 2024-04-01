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
using System.Threading.Tasks;
using AnimalHusbandryMod.common;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Objects;
using AnimalHusbandryMod.cooking;
using DataLoader = AnimalHusbandryMod.common.DataLoader;

namespace AnimalHusbandryMod.recipes
{
    public class MeatFridayChannel : Channel
    {
        private Dictionary<int, string> _recipes;

        public string GetName => "MeatFriday";
        public string GetScreenTextureName => Game1.mouseCursorsName;
        public Rectangle GetScreenSourceRectangle => new Rectangle(602, 361, 42, 28);

        public string GetDisplayName  {
            get {
                Boolean rerun = Game1.stats.DaysPlayed % 2 == 0U;
                string name = DataLoader.i18n.Get("TV.MeatFriday.ChannelDisplayName");
                if (rerun)
                {
                    name += " " + DataLoader.i18n.Get("TV.MeatFriday.ReRunDisplaySuffix"); ;
                }
                return name;
            }
        }

        public MeatFridayChannel()
        {
            ReloadEpisodes();
        }

        public void ReloadEpisodes()
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

        public bool CheckChannelDay()
        {
            if (!DataLoader.ModConfig.DisableMeat && SDate.Now().DayOfWeek == DayOfWeek.Friday)
            {
                int recipe = GetRecipeNumber();
                if (recipe >= 2)
                {
                    return true;
                }
            }
            return false;
        }

        public string[] GetEpisodesText()
        {
            return new string[]
            {
                DataLoader.i18n.Get("TV.MeatFriday.Announcement")
                , GetRecipePresentation()
                , GetAddRecipeText()
            };
        }

        private static int GetRecipeNumber()
        {
            return (int)(Game1.stats.DaysPlayed % 112U / 14) + 1;
        }

        private string GetRecipePresentation()
        {
            return _recipes[GetRecipeNumber()].Split('/')[1];
        }

        /// <summary>
        /// Get the add recipe text and also add the recipe if not known.
        /// </summary>
        /// <returns>The add recipe text</returns>
        private string GetAddRecipeText()
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
            return addRecipeText;
        }
    }
}
