/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mrveress/SDVMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile;

namespace SeedMachines.Framework.BigCraftables
{
    public abstract class IBigCraftableWrapper
    {
        private static Dictionary<String, IBigCraftableWrapper> wrappers = new Dictionary<String, IBigCraftableWrapper>();

        static IBigCraftableWrapper()
        {
            addWrapper(new SeedMachineWrapper());
            addWrapper(new SeedBanditWrapper());
        }

        public static void addWrapper(IBigCraftableWrapper wrapper)
        {
            wrappers.Add(wrapper.name, wrapper);
        }

        public static IBigCraftableWrapper getWrapper(String name)
        {
            return wrappers[name];
        }

        public static Dictionary<String, IBigCraftableWrapper> getAllWrappers()
        {
            return wrappers;
        }

        public static void addAllRecipies()
        {
            foreach (IBigCraftableWrapper wrapper in wrappers.Values)
            {
                wrapper.addReceipt();
            }
        }

        public static void checkAndInjectDynamicObject(OverlaidDictionary objects, Vector2 key)
        {
            if (!(objects[key] is IBigCraftable) && wrappers.ContainsKey(objects[key].name))
            {
                IBigCraftableWrapper wrapper = wrappers[objects[key].name];
                objects[key] = (IBigCraftable)Activator.CreateInstance(wrapper.dynamicObjectType, objects[key], wrapper);
            }
        }

        public static void checkAndRemoveDynamicObject(OverlaidDictionary objects, Vector2 key)
        {
            if (objects[key] is IBigCraftable)
            {
                objects[key] = ((IBigCraftable)objects[key]).baseObject;
            }
        }

        public static void addBigCraftablesInformations(IDictionary<int, string> bigCraftablesInformationData)
        {
            foreach (IBigCraftableWrapper wrapper in wrappers.Values)
            {
                wrapper.addBigCraftablesInformation(bigCraftablesInformationData);
            }
        }

        public static void addCraftingRecipes(IDictionary<string, string> craftingRecipesData)
        {
            foreach (IBigCraftableWrapper wrapper in wrappers.Values)
            {
                wrapper.addCraftingRecipe(craftingRecipesData);
            }
        }

        //-----------------

        public int itemID;
        public String name;
        public int price;
        public bool availableOutdoors;
        public bool availableIndoors;
        public int fragility;
        //public int isLamp;
        public int edibility = -300;
        public String typeAndCategory;

        public String ingredients;
        public String location = "Home";
        public String unlockConditions;

        public Type dynamicObjectType;
        public int maxAnimationIndex;
        public int millisecondsBetweenAnimation;

        //-----------------

        private String getTranslationBaseName()
        {
            return this.name.ToLower().Replace(' ', '-');
        }

        public String getDefaultLabel()
        {
            return CustomTranslator.getTranslation("default", getTranslationBaseName() + ".label");
        }
        public String getDefaultDescription()
        {
            return CustomTranslator.getTranslation("default", getTranslationBaseName() + ".description");
        }

        public IDictionary<String, String> getAllTranslationsForLabel()
        {
            return CustomTranslator.getAllTranslationsByLocales(getTranslationBaseName() + ".label");
        }
        public IDictionary<String, String> getAllTranslationsForDescription()
        {
            return CustomTranslator.getAllTranslationsByLocales(getTranslationBaseName() + ".description");
        }

        public String getLabel()
        {
            return ModEntry.modHelper.Translation.Get(getTranslationBaseName() + ".label");
        }

        public String getDescription()
        {
            return ModEntry.modHelper.Translation.Get(getTranslationBaseName() + ".description");
        }

        public void addReceipt()
        {
            if (!Game1.player.craftingRecipes.Keys.Contains(this.name))
            {
                Game1.player.craftingRecipes.Add(this.name, 0);
            }
        }

        public void addBigCraftablesInformation(IDictionary<int, string> bigCraftablesInformationData)
        {
            bigCraftablesInformationData[itemID] = $"{name}/{price}/{edibility}/{typeAndCategory}/{getDescription()}/{availableOutdoors}/{availableIndoors}/{fragility}/{getLabel()}";
        }

        public void addCraftingRecipe(IDictionary<string, string> craftingRecipesData)
        {
            craftingRecipesData[this.name] = $"{ingredients}/{location}/{itemID}/true/{unlockConditions}/{getLabel()}";
        }

        public JsonAssetsBigCraftableModel getJsonAssetsModel()
        {
            JsonAssetsBigCraftableModel result = new JsonAssetsBigCraftableModel();
            result.Name = this.name;
            result.Description = this.getDefaultDescription();
            result.Price = this.price;
            result.IsDefault = true;
            result.ProvidesLight = false;
            result.ReserveExtraIndexCount = this.maxAnimationIndex;

            result.Recipe = new JARecipe();
            result.Recipe.CanPurchase = false;
            result.Recipe.ResultCount = 1;
            result.Recipe.Ingredients = new List<IDictionary<String, int>>();
            string[] splittedIngredients = this.ingredients.Split(' ');
            for (int i = 0; i < splittedIngredients.Length; i+=2)
            {
                IDictionary<String, int> ingredientMap = new Dictionary<string, int>();
                ingredientMap.Add("Object", Int32.Parse(splittedIngredients[i]));
                ingredientMap.Add("Count", Int32.Parse(splittedIngredients[i+1]));
                result.Recipe.Ingredients.Add(ingredientMap);
            }

            result.NameLocalization = getAllTranslationsForLabel();
            result.DescriptionLocalization = getAllTranslationsForDescription();

            return result;
        }
    }
}
