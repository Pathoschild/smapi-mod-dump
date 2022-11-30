/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sophiesalacia/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;
using StardewValley.Tools;
using Object = StardewValley.Object;

namespace InventoryRandomizer;

internal static class InventoryRandomizer
{
    private static readonly Random Rand = new();
    private static int TotalWeight = 8;

    internal static void RandomizeInventory()
    {
        // recheck configured weights
        TotalWeight = Globals.Config.GetTotalWeight();

        IList<Item> inventory = Game1.player.Items;

        for (int i = 0; i < inventory.Count; i++)
        {
            Item item = inventory[i];

            if (item is null)
                continue;

            // if item is stackable and currently is stacked, respect quantity
            if (item.Stack > 1)
            {
                inventory[i] = GetRandomItemStack(item.Stack);
            }
            else
            {
                inventory[i] = GetRandomSingleItem();
            }
        }
    }

    private static Item GetRandomItemStack(int quantity)
    {
        Item item = GetRandomItem();

        while (!IsStackable(item) || IsRecipe(item))
        {
            item = GetRandomItem();
        }

        item.Stack = quantity;

        return item;
    }

    private static Item GetRandomSingleItem()
    {
        return GetRandomItem();
    }

    private static Item GetRandomItem()
    {
        // weighted random stuff
        int randValue = Rand.Next(1, TotalWeight + 1);

        randValue -= Globals.Config.BigCraftablesWeight;
        if (randValue < 0)
        {
            return GetRandomBigCraftable();
        }

        randValue -= Globals.Config.BootsWeight;
        if (randValue < 0)
        {
            return GetRandomBoots();
        }

        randValue -= Globals.Config.ClothingWeight;
        if (randValue < 0)
        {
            return GetRandomClothing();
        }

        randValue -= Globals.Config.FurnitureWeight;
        if (randValue < 0)
        {
            return GetRandomFurniture();
        }

        randValue -= Globals.Config.HatsWeight;
        if (randValue < 0)
        {
            return GetRandomHat();
        }

        randValue -= Globals.Config.ObjectsWeight;
        if (randValue < 0)
        {
            return GetRandomObject();
        }

        randValue -= Globals.Config.WeaponsWeight;
        if (randValue < 0)
        {
            return GetRandomWeapon();
        }

        return GetRandomTool();
    }

    private static Item GetRandomBigCraftable()
    {
        (int id, string data) = GetRandomElementFromDictionary(AssetManager.CachedBigCraftablesInfo);

        string[] fields = data.Split('/');
        string name = fields[0];

        // if recipe exists and player doesn't already know it, configurable chance they'll receive the recipe instead
        bool isRecipe = AssetManager.CachedRecipes.ContainsKey(name) && !Game1.player.knowsRecipe(name) &&
                        Rand.NextDouble() < Globals.Config.RecipeChance;

        return new Object(Vector2.Zero, id, isRecipe);
    }

    private static Item GetRandomBoots()
    {
        (int id, _) = GetRandomElementFromDictionary(AssetManager.CachedBoots);

        return new Boots(id);
    }

    private static Item GetRandomClothing()
    {
        (int id, _) = GetRandomElementFromDictionary(AssetManager.CachedClothingInfo);

        Clothing clothing = new(id);

        // if dyeable, dye a random color
        if (clothing.dyeable.Value)
        {
            clothing.clothesColor.Value = new Color(Rand.Next(255), Rand.Next(255), Rand.Next(255));
        }

        return clothing;
    }

    private static Item GetRandomFurniture()
    {
        (int id, _) = GetRandomElementFromDictionary(AssetManager.CachedFurniture);

        return Furniture.GetFurnitureInstance(id);
    }

    private static Item GetRandomHat()
    {
        (int id, _) = GetRandomElementFromDictionary(AssetManager.CachedHats);

        return new Hat(id);
    }

    private static Item GetRandomObject()
    {
        (int id, string data) = GetRandomElementFromDictionary(AssetManager.CachedObjectInfo);

        string[] fields = data.Split('/');
        string name = fields[0];
        string type = fields[3];

        // if recipe exists and player doesn't already know it, 25% chance they'll receive the recipe instead
        bool isRecipe = AssetManager.CachedRecipes.ContainsKey(name) && !Game1.player.knowsRecipe(name) &&
                        Rand.NextDouble() < Globals.Config.RecipeChance;

        // random quality level - 0, 1, 2, or 4
        int quality = Rand.Next(4);
        quality = quality == 3 ? 4 : quality;

        // rings need to use the ring constructor
        if (name != "Wedding Ring" && type == "Ring")
        {
            return new Ring(id);
        }

        return new Object(id, 1, isRecipe, quality: quality);
    }

    private static Item GetRandomWeapon()
    {
        (int id, _) = GetRandomElementFromDictionary(AssetManager.CachedWeapons);

        // Slingshots are their own class, everything else is a regular melee weapon
        return id is 32 or 33 or 34 ? new Slingshot(id) : new MeleeWeapon(id);
    }

    private static Item GetRandomTool()
    {
        int whichTool = Rand.Next(9);
        int upgradeLevel = Rand.Next(5);

        switch (whichTool)
        {
            case 0:
                return new Axe {UpgradeLevel = upgradeLevel};

            case 1:
                return new Pickaxe {UpgradeLevel = upgradeLevel};

            case 2:
                return new Hoe {UpgradeLevel = upgradeLevel};

            case 3:
                return new WateringCan {UpgradeLevel = upgradeLevel};

            case 4:
                upgradeLevel = Math.Min(3, upgradeLevel); // fishing rod upgrade level caps out at 3
                return new FishingRod {UpgradeLevel = upgradeLevel};

            case 5:
                return new MilkPail();

            case 6:
                return new Shears();

            case 7:
                return new Pan();

            case 8:
                return new Wand();

            default:
                return null;
        }
    }

    private static (int id, string data) GetRandomElementFromDictionary(Dictionary<int, string> dictionary)
    {
        int index = Rand.Next(dictionary.Count);
        KeyValuePair<int, string> elem = dictionary.ElementAt(index);
        return (elem.Key, elem.Value);
    }

    private static bool IsRecipe(Item item)
    {
        return item is Object {IsRecipe: true};
    }

    private static bool IsStackable(Item item)
    {
        return item.maximumStackSize() > 1;
    }
}
