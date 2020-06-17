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
        public static int initialAbsoluteID;
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

        public int relativeID;
        public String name;
        public int price;
        public bool availableOutdoors;
        public bool availableIndoors;
        public int fragility;
        //public int isLamp;
        public int edibility = -300;
        public String typeAndCategory;

        public String ingridients;
        public String location = "Home";
        public String unlockConditions;

        public Type dynamicObjectType;
        public int maxAnimationIndex;
        public int millisecondsBetweenAnimation;

        //-----------------

        public int getAbsoluteID()
        {
            return initialAbsoluteID + relativeID;
        }

        private String getTranslationBaseName()
        {
            return this.name.ToLower().Replace(' ', '-');
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
            bigCraftablesInformationData[getAbsoluteID()] = $"{name}/{price}/{edibility}/{typeAndCategory}/{getDescription()}/{availableOutdoors}/{availableIndoors}/{fragility}/{getLabel()}";
        }

        public void addCraftingRecipe(IDictionary<string, string> craftingRecipesData)
        {
            craftingRecipesData[this.name] = $"{ingridients}/{location}/{getAbsoluteID()}/true/{unlockConditions}/{getLabel()}";
        }
    }
}
