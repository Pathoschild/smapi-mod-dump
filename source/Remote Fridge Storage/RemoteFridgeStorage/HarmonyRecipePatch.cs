using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley;
using Object = StardewValley.Object;
// ReSharper disable InconsistentNaming

// Just some IDE things.
// ReSharper disable RedundantTypeArgumentsOfMethod
// ReSharper disable InvertIf
// ReSharper disable SuggestVarOrType_BuiltInTypes

namespace RemoteFridgeStorage
{
    internal static class HarmonyRecipePatchConsumeIngredients
    {
        public static bool Prefix(CraftingRecipe __instance)
        {
            // Copied from CraftingRecipe consumeIngredients method.
            // The only change in this is the use of the ModEntry Fridge() method.
            // The private fields are accessed through reflection.
            var recipeList = Util.GetField<Dictionary<int, int>>(__instance, "recipeList");
            for (int index1 = recipeList.Count - 1; index1 >= 0; --index1)
            {
                int recipe1 = recipeList[recipeList.Keys.ElementAt<int>(index1)];
                bool flag = false;
                for (int index2 = Game1.player.items.Count - 1; index2 >= 0; --index2)
                {
                    if (Game1.player.items[index2] != null && Game1.player.items[index2] is Object &&
                        !(bool) ((NetFieldBase<bool, NetBool>) (Game1.player.items[index2] as Object).bigCraftable) &&
                        ((int) ((NetFieldBase<int, NetInt>) Game1.player.items[index2].parentSheetIndex) ==
                         recipeList.Keys.ElementAt<int>(index1) || Game1.player.items[index2].Category ==
                         recipeList.Keys.ElementAt<int>(index1)))
                    {
                        int recipe2 = recipeList[recipeList.Keys.ElementAt<int>(index1)];
                        recipeList[recipeList.Keys.ElementAt<int>(index1)] -=
                            Game1.player.items[index2].Stack;
                        Game1.player.items[index2].Stack -= recipe2;
                        if (Game1.player.items[index2].Stack <= 0)
                            Game1.player.items[index2] = (Item) null;
                        if (recipeList[recipeList.Keys.ElementAt<int>(index1)] <= 0)
                        {
                            recipeList[recipeList.Keys.ElementAt<int>(index1)] = recipe1;
                            flag = true;
                            break;
                        }
                    }
                }

                if (__instance.isCookingRecipe && !flag)
                {
                    //We do not need to check for the current location.
                    // FarmHouse currentLocation = Game1.currentLocation as FarmHouse;
                    // if (currentLocation != null)
                    {
                        // Use 
                        // var fridgeItems = currentLocation.fridge.Value.items;
                        var fridgeItems = ModEntry.Fridge();
                        for (int index2 = fridgeItems.Count - 1; index2 >= 0; --index2)
                        {
                            if (fridgeItems[index2] != null &&
                                fridgeItems[index2] is Object &&
                                ((int) ((NetFieldBase<int, NetInt>) fridgeItems[index2]
                                     .parentSheetIndex) == recipeList.Keys.ElementAt<int>(index1) ||
                                 fridgeItems[index2].Category ==
                                 recipeList.Keys.ElementAt<int>(index1)))
                            {
                                int recipe2 = recipeList[recipeList.Keys.ElementAt<int>(index1)];
                                recipeList[recipeList.Keys.ElementAt<int>(index1)] -=
                                    fridgeItems[index2].Stack;
                                fridgeItems[index2].Stack -= recipe2;
                                if (fridgeItems[index2].Stack <= 0)
                                    fridgeItems[index2] = (Item) null;
                                if (recipeList[recipeList.Keys.ElementAt<int>(index1)] <= 0)
                                {
                                    recipeList[recipeList.Keys.ElementAt<int>(index1)] = recipe1;
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }
    }

    internal class HarmonyRecipePatchDraw
    {
        public static bool Prefix(CraftingRecipe __instance, SpriteBatch b, Vector2 position, int width)
        {
            //Copied from CraftingRecipe drawRecipeDescription
            // The only change in this is the use of the ModEntry Fridge() method.
            // The private fields are accessed through reflection.
            b.Draw(Game1.staminaRect,
                new Rectangle((int) ((double) position.X + 8.0),
                    (int) ((double) position.Y + 32.0 + (double) Game1.smallFont.MeasureString("Ing").Y) - 4 - 2,
                    width - 32, 2), Game1.textColor * 0.35f);
            Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.567"),
                Game1.smallFont, position + new Vector2(8f, 28f), Game1.textColor * 0.75f, 1f, -1f, -1, -1, 1f, 3);
            var instanceRecipeList = Util.GetField<Dictionary<int, int>>(__instance, "recipeList");
            for (int index = 0; index < instanceRecipeList.Count; ++index)
            {
                Color color = Game1.player.hasItemInInventory(instanceRecipeList.Keys.ElementAt<int>(index),
                    instanceRecipeList.Values.ElementAt<int>(index), 8)
                    ? Game1.textColor
                    : Color.Red;
                //Get the items from the fridge
                var fridgeItems = ModEntry.Fridge();
                if (__instance.isCookingRecipe && Game1.player.hasItemInList(
                        (IList<Item>) fridgeItems,
                        instanceRecipeList.Keys.ElementAt<int>(index), instanceRecipeList.Values.ElementAt<int>(index), 8))
                    color = Game1.textColor;
                b.Draw(Game1.objectSpriteSheet,
                    new Vector2(position.X, position.Y + 64f + (float) (index * 64 / 2) + (float) (index * 4)),
                    new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet,
                        __instance.getSpriteIndexFromRawIndex(instanceRecipeList.Keys.ElementAt<int>(index)), 16, 16)),
                    Color.White, 0.0f, Vector2.Zero, 2f, SpriteEffects.None, 0.86f);
                Utility.drawTinyDigits(instanceRecipeList.Values.ElementAt<int>(index), b,
                    new Vector2(
                        position.X + 32f - Game1.tinyFont
                            .MeasureString(string.Concat((object) instanceRecipeList.Values.ElementAt<int>(index))).X,
                        (float) ((double) position.Y + 64.0 + (double) (index * 64 / 2) + (double) (index * 4) + 21.0)),
                    2f, 0.87f, Color.AntiqueWhite);
                Utility.drawTextWithShadow(b, __instance.getNameFromIndex(instanceRecipeList.Keys.ElementAt<int>(index)),
                    Game1.smallFont,
                    new Vector2((float) ((double) position.X + 32.0 + 8.0),
                        (float) ((double) position.Y + 64.0 + (double) (index * 64 / 2) + (double) (index * 4) + 4.0)),
                    color, 1f, -1f, -1, -1, 1f, 3);
            }

            b.Draw(Game1.staminaRect,
                new Rectangle((int) position.X + 8, (int) position.Y + 64 + 4 + instanceRecipeList.Count * 36, width - 32,
                    2), Game1.textColor * 0.35f);
            Utility.drawTextWithShadow(b, Game1.parseText(Util.GetField<string>(__instance,"description"), Game1.smallFont, width - 8),
                Game1.smallFont, position + new Vector2(0.0f, (float) (76 + instanceRecipeList.Count * 36)),
                Game1.textColor * 0.75f, 1f, -1f, -1, -1, 1f, 3);
            return false;
        }
    }

    internal static class Util
    {
        public static T GetField<T>(CraftingRecipe instance, string field)
        {
            return ModEntry.Instance.Helper.Reflection.GetField<T>(instance, field, false).GetValue();
        }
    }
    
}