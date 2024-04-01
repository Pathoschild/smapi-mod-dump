/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using JsonAssets.Framework;
using StardewValley;

namespace JsonAssets.Data
{
    public class ObjectRecipe
    {
        /*********
        ** Accessors
        *********/
        public string SkillUnlockName { get; set; } = null;
        public int SkillUnlockLevel { get; set; } = -1;

        public int ResultCount { get; set; } = 1;
        public IList<ObjectIngredient> Ingredients { get; set; } = new List<ObjectIngredient>();

        public bool IsDefault { get; set; } = false;
        public bool CanPurchase { get; set; } = false;
        public int PurchasePrice { get; set; }
        public string PurchaseFrom { get; set; } = "Gus";
        public IList<string> PurchaseRequirements { get; set; } = new List<string>();
        public IList<PurchaseData> AdditionalPurchaseData { get; set; } = new List<PurchaseData>();


        /*********
        ** Public methods
        *********/
        internal string GetRecipeString(ObjectData parent)
        {
            string str = "";
            foreach (var ingredient in this.Ingredients)
            {
                string ingredientName = ingredient.Object.ToString().FixIdJA();
                // If the original object name is an integer, it's a category or an original ID
                if (int.TryParse(ingredient.Object.ToString(), out int ingredIndex))
                {
                    ingredientName = ingredIndex.ToString();
                }
                // If the object isn't an integer, check if it's the name of an existing item
                else if (ItemRegistry.GetDataOrErrorItem(ingredientName).IsErrorItem)
                {
                    Item tryGetItem = Utility.fuzzyItemSearch(ingredientName);
                    if (tryGetItem != null)
                    {
                        ingredientName = tryGetItem.ItemId;
                    }
                }
                // Otherwise leave name untouched
                str += ingredientName + " " + ingredient.Count + " ";
            }
            str = str.Substring(0, str.Length - 1);
            if (parent.Category != ObjectCategory.Cooking)
                str += "/what is this for?";
            else
                str += "/9999 9999";
            str += $"/{parent.Name.FixIdJA()} {this.ResultCount}/";
            if (parent.Category != ObjectCategory.Cooking)
                str += "false/";
            if (this.SkillUnlockName?.Length > 0 && this.SkillUnlockLevel > 0)
                str += "" + this.SkillUnlockName + " " + this.SkillUnlockLevel;
            else
                str += "null";
            //if (LocalizedContentManager.CurrentLanguageCode != LocalizedContentManager.LanguageCode.en)
                str += "/" + parent.LocalizedName();
            return str;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Normalize the model after it's deserialized.</summary>
        /// <param name="context">The deserialization context.</param>
        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            this.Ingredients ??= new List<ObjectIngredient>();
            this.PurchaseRequirements ??= new List<string>();
            this.AdditionalPurchaseData ??= new List<PurchaseData>();

            this.Ingredients.FilterNulls();
            this.PurchaseRequirements.FilterNulls();
            this.AdditionalPurchaseData.FilterNulls();
        }
    }
}
