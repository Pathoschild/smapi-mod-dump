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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omegasis.Revitalize.Framework.SaveData.Player
{
    public class PlayerSaveData:SaveDataInfo
    {
        public Dictionary<string, string> unlockedCraftingRecipes;

        /// <summary>
        /// Checks to see if the player has bought the subscription to receive free movie tickets in the mail on the first of every season.
        /// </summary>
        public bool hasMovieTheaterTicketSubscription;


        public PlayerSaveData()
        {
            this.unlockedCraftingRecipes = new Dictionary<string, string>();
            this.hasMovieTheaterTicketSubscription = false;
        }

        public virtual void addUnlockedCraftingRecipe(string RecipeBookId, string RecipeId)
        {
            if (this.unlockedCraftingRecipes.ContainsKey(RecipeBookId)) return;
            this.unlockedCraftingRecipes.Add(RecipeBookId, RecipeId);
        }

        public override void load()
        {
            foreach(KeyValuePair<string,string> recipe in this.unlockedCraftingRecipes)
            {
                RevitalizeModCore.ModContentManager.craftingManager.learnCraftingRecipe(recipe.Key, recipe.Value,false);
            }
        }

    }
}
