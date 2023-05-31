/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/SolidFoundations
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidFoundations.Framework.Utilities.Backport
{
    // TODO: When updated to SDV v1.6, this class should be deleted in favor of using StardewValley.Utility 
    internal static class Toolkit
    {
        public static Item CreateItemByID(string id, int amount, int quality)
        {
            int itemId = -1;
            string parsedId = id.Replace(" ", string.Empty);
            if (int.TryParse(parsedId, out itemId) is false && parsedId.StartsWith('('))
            {
                string key = parsedId.Substring(0, parsedId.IndexOf(')') + 1);
                parsedId = new System.String(parsedId.Where(System.Char.IsDigit).ToArray());

                if (int.TryParse(parsedId, out itemId))
                {
                    switch (key.ToUpper())
                    {
                        case "(O)":
                            return CreateItemObject(itemId, amount, quality);
                        case "(BC)":
                            return CreateItemBigCraftable(itemId, amount, quality);
                        case "(F)":
                            return CreateItemFurniture(itemId, amount, quality);
                    }
                }
            }

            if (itemId == -1)
            {
                return null;
            }
            return CreateItemObject(itemId, amount, quality);
        }

        public static Item CreateItemObject(int itemId, int amount, int quality)
        {
            if (itemId == 93)
            {
                return new Torch(Vector2.Zero, amount, itemId);
            }
            if (itemId == 812)
            {
                return new ColoredObject(itemId, 1, Color.White);
            }
            if (Game1.objectInformation.ContainsKey(itemId))
            {
                if (Game1.objectInformation[itemId].Split('/')[3] == "-96" && !(itemId == 801))
                {
                    if (itemId == 880)
                    {
                        return new CombinedRing(itemId);
                    }
                    return new Ring(itemId);
                }
            }

            return new Object(itemId, amount, isRecipe: false, -1, quality);
        }

        public static Item CreateItemBigCraftable(int itemId, int amount, int quality)
        {
            return new Object(Vector2.Zero, itemId)
            {
                Stack = amount,
                Quality = quality
            };
        }

        public static Item CreateItemFurniture(int itemId, int amount, int quality)
        {
            Furniture furnitureInstance = Furniture.GetFurnitureInstance(itemId, Vector2.Zero);
            furnitureInstance.Stack = amount;
            furnitureInstance.Quality = quality;

            return furnitureInstance;
        }

        public static int GetNumberOfItemThatCanBeAddedToThisInventoryList(Item item, IList<Item> list, int list_max_items)
        {
            int num = 0;
            foreach (Item item2 in list)
            {
                if (item2 == null)
                {
                    num += item.maximumStackSize();
                }
                else if (item2 != null && item2.canStackWith(item) && item2.getRemainingStackSpace() > 0)
                {
                    num += item2.getRemainingStackSpace();
                }
            }
            for (int i = 0; i < list_max_items - list.Count; i++)
            {
                num += item.maximumStackSize();
            }
            return num;
        }

        public static Item ConsumeStack(Item item, int amount)
        {
            if (amount == 0)
            {
                return item;
            }
            if (item.Stack - amount <= 0)
            {
                return null;
            }

            item.Stack -= amount;
            return item;
        }

    }
}
