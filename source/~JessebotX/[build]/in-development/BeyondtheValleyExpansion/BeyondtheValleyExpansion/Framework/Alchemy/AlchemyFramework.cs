using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.Menus;
using StardewModdingAPI;
using Microsoft.Xna.Framework.Graphics;

namespace BeyondTheValleyExpansion.Framework.Alchemy
{
    /// <summary> the code for the alchemy feature </summary>
    class AlchemyFramework
    {
        /// <summary> instance of <see cref="AlchemyProperties"/> contains all the alchemy potion properties </summary>
        private AlchemyProperties _Properties = new AlchemyProperties();

        /// <summary> stores item ids that are currently in used </summary>
        public static List<int> ItemsInUsed = new List<int>();
        /// <summary> options for the alchemy menu </summary>
        public List<Response> AlchemyMenuOptions = new List<Response>();

        /// <summary> if alchemy feature is unlocked </summary>
        public bool UnlockedAlchemy;

        /// <summary> Current amount of items in pot under Gem Category</summary>
        public static float GemCategoryItems = 0f;

        /// <summary> Entry point for Alchemy </summary>
        /// <param name="who"> the player </param>
        /// <param name="ans"> the chosen option </param>
        public void Alchemy(Farmer who, string ans)
        {
            // display name translations for all potion properties
            _Properties.Potency.DisplayName = RefMod.i18n.Get("alchemy-potency.name");
            _Properties.Density.DisplayName = RefMod.i18n.Get("alchemy-density.name");
            _Properties.Growth.DisplayName = RefMod.i18n.Get("alchemy-growth.name");
            _Properties.Fire.DisplayName = RefMod.i18n.Get("alchemy-fire.name");
            _Properties.Water.DisplayName = RefMod.i18n.Get("alchemy-water.name");
            _Properties.Purity.DisplayName = RefMod.i18n.Get("alchemy-purity.name");

            switch (ans)
            {
                case "mix-ingredients":
                    this.MixIngredients(who, ans);
                    break;
                case "remove-ingredients":
                    this.RemoveIngredients(who, ans);
                    break;
                case "add-ingredients":
                    this.AddIngredients(who, ans);
                    break;
                default:
                    RefMod.ModMonitor.Log($"Looks like something went wrong with Alchemy options for {who.Name} with {ans}", LogLevel.Error);
                    break;
            }
        }

        /// <summary> Mix all ingredients in the alchemy pot </summary>
        /// <param name="who"> the player </param>
        /// <param name="ans"> the chosen option </param>
        private void MixIngredients(Farmer who, string ans)
        {

        }

        /// <summary> Remove all ingredients in the alchemy pot </summary>
        /// <param name="who"> the player </param>
        /// <param name="ans"> the chosen option </param>
        private void RemoveIngredients(Farmer who, string ans)
        {

        }

        /// <summary> Add an ingredient in the alchemy pot </summary>
        /// <param name="who"> the player </param>
        /// <param name="ans"> the chosen option </param>
        private void AddIngredients(Farmer who, string ans)
        {
            if (
                Game1.player.CurrentItem?.Category == RefObjectCategory.Object_artisanGoodsCategory ||
                Game1.player.CurrentItem?.Category == RefObjectCategory.Object_baitCategory ||
                Game1.player.CurrentItem?.Category == RefObjectCategory.Object_buildingResources ||
                Game1.player.CurrentItem?.Category == RefObjectCategory.Object_CookingCategory ||
                Game1.player.CurrentItem?.Category == RefObjectCategory.Object_eggCategory ||
                Game1.player.CurrentItem?.Category == RefObjectCategory.Object_fertilizerCategory ||
                Game1.player.CurrentItem?.Category == RefObjectCategory.Object_FishCategory ||
                Game1.player.CurrentItem?.Category == RefObjectCategory.Object_flowersCategory ||
                Game1.player.CurrentItem?.Category == RefObjectCategory.Object_FruitsCategory ||
                Game1.player.CurrentItem?.Category == RefObjectCategory.Object_GemCategory ||
                Game1.player.CurrentItem?.Category == RefObjectCategory.Object_GreensCategory ||
                Game1.player.CurrentItem?.Category == RefObjectCategory.Object_ingredientsCategory ||
                Game1.player.CurrentItem?.Category == RefObjectCategory.Object_junkCategory ||
                Game1.player.CurrentItem?.Category == RefObjectCategory.Object_meatCategory ||
                Game1.player.CurrentItem?.Category == RefObjectCategory.Object_metalResources ||
                Game1.player.CurrentItem?.Category == RefObjectCategory.Object_MilkCategory ||
                Game1.player.CurrentItem?.Category == RefObjectCategory.Object_mineralsCategory ||
                Game1.player.CurrentItem?.Category == RefObjectCategory.Object_monsterLootCategory ||
                Game1.player.CurrentItem?.Category == RefObjectCategory.Object_SeedsCategory ||
                Game1.player.CurrentItem?.Category == RefObjectCategory.Object_sellAtFishShopCategory ||
                Game1.player.CurrentItem?.Category == RefObjectCategory.Object_sellAtPierres ||
                Game1.player.CurrentItem?.Category == RefObjectCategory.Object_sellAtPierresAndMarnies ||
                Game1.player.CurrentItem?.Category == RefObjectCategory.Object_syrupCategory ||
                Game1.player.CurrentItem?.Category == RefObjectCategory.Object_VegetableCategory
            )
            {
                if (Game1.player.CurrentItem?.Category == RefObjectCategory.Object_GemCategory)
                    _Properties.AlchemyGemCategory(who);

                RefMod.ModMonitor.Log(
                    $"{who.Name} added {Game1.player.CurrentItem.DisplayName} into the Alchemy Pot. \n" +
                    $"Property values: \n" +
                    $"{_Properties.Potency.DisplayName}: {_Properties.Potency.ToString()}; " +
                    $"{_Properties.Density.DisplayName}: {_Properties.Density.ToString()};" +
                    $"{_Properties.Growth.DisplayName}: {_Properties.Growth.ToString()}; " +
                    $"\n" +
                    $"{_Properties.Fire.DisplayName}: {_Properties.Fire.ToString()};" +
                    $"{_Properties.Water.DisplayName}: {_Properties.Water.ToString()};" +
                    $"{_Properties.Purity.DisplayName}: {_Properties.Purity.ToString()};" +
                    LogLevel.Trace
                );
            }

            else
                this.AlchemyFailed();

            RefMod.ModHelper.Data.WriteSaveData("AlchemyItemsInUsed", ItemsInUsed);
        }

        /// <summary> Item cannot be used in Alchemy </summary>
        public void AlchemyFailed()
        {
            // item cannot be used in alchemy
            Game1.drawObjectDialogue(RefMod.i18n.Get("alchemy-failed.1"));
            RefMod.ModMonitor.Log($"Item [{Game1.player.CurrentItem.ParentSheetIndex.ToString()}] doesn't seem to be useable in Alchemy", LogLevel.Info);
        }
    }
}
