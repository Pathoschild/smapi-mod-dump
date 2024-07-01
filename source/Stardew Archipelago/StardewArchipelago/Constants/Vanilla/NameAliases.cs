/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System.Collections.Generic;

namespace StardewArchipelago.Constants.Vanilla
{
    public static class NameAliases
    {
        public static List<List<string>> ItemNameAliasGroups = new()
        {
            new List<string> { "L. Goat Milk", "Large Goat Milk", "Goat Milk (Large)" },
            new List<string> { "L. Milk", "Large Milk", "Milk (Large)" },
            new List<string> { "Egg (Brown)", "Brown Egg" },
            new List<string> { "Large Egg (Brown)", "Large Brown Egg" },
            new List<string> { "Cookie", "Cookies" },
            new List<string> { "Pina Colada", "Piña Colada" },
            new List<string> { "Dried Fruit", "DriedFruit", "Dried Fruits", "DriedFruits" },
            new List<string> { "Dried Mushrooms", "DriedMushrooms", "Dried Mushroom", "DriedMushroom" },
            new List<string> { "Smoked Fish", "SmokedFish", "Smoked" },
        };

        public static Dictionary<string, string> RecipeNameAliases = new()
        {
            { "Cheese Cauli.", "Cheese Cauliflower" },
            { "Dish o' The Sea", "Dish O' The Sea" },
            { "Eggplant Parm.", "Eggplant Parmesan" },
            { "Cran. Sauce", "Cranberry Sauce" },
            { "Vegetable Stew", "Vegetable Medley" },
        };

        public static Dictionary<string, string> NPCNameAliases = new()
        {
            { "MisterGinger", "Mr. Ginger" },
            { "MarlonFay", "Marlon" },
            { "GuntherSilvian", "Gunther" },
            { "MorrisTod", "Morris" },
            { "Goblin", "Zic" },
            { "ichortower.HatMouseLacey_Lacey", "Lacey"},
            { "SPB_Alec", "Alec"},
        };
    }
}
