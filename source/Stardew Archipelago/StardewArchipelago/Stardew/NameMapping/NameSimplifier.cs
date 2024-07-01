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
using System.Globalization;
using StardewArchipelago.Constants.Vanilla;
using StardewValley;

namespace StardewArchipelago.Stardew.NameMapping
{
    public class NameSimplifier
    {
        public NameSimplifier()
        {
        }

        public string GetSimplifiedName(Item item)
        {
            var name = item.Name;
            if (item is Object && _renamedObjects.ContainsKey(item.QualifiedItemId))
            {
                name = _renamedObjects[item.QualifiedItemId];
            }

            if (PowerBooks.BookIdsToNames.ContainsKey(item.ItemId))
            {
                name = PowerBooks.BookIdsToNames[item.ItemId];
            }

            foreach (var (oldChar, newChar) in _simplifiedChars)
            {
                name = name.Replace(oldChar, newChar);
            }

            if (name.Contains("moonslime.Archaeology."))
            {
                var ti = CultureInfo.CurrentCulture.TextInfo;
                var displayName = ti.ToTitleCase(item.DisplayName);
                if (name.Contains("strange_doll_green"))
                {
                    displayName += " (Green)";
                }
                if (name.Contains("trilobite")) // Temporary fix.
                {
                    displayName = displayName.Replace("Trilobite Fossil", "Trilobite");
                }
                name = displayName;
            }

            if (item is not Object shippedObject)
            {
                return name;
            }

            foreach (var simplifiedName in _simplifiedNames)
            {
                if (name.Contains(simplifiedName))
                {
                    return simplifiedName;
                }
            }

            if (shippedObject.preserve.Value.HasValue)
            {
                switch (shippedObject.preserve.Value.GetValueOrDefault())
                {
                    case Object.PreserveType.Wine:
                        return "Wine";
                    case Object.PreserveType.Jelly:
                        return "Jelly";
                    case Object.PreserveType.Pickle:
                        return "Pickles";
                    case Object.PreserveType.Juice:
                        return "Juice";
                    case Object.PreserveType.Roe:
                        return "Roe";
                    case Object.PreserveType.AgedRoe:
                        return "Aged Roe";
                    case Object.PreserveType.DriedFruit:
                        return "Dried Fruit";
                    case Object.PreserveType.DriedMushroom:
                        return "Dried Mushrooms";
                    case Object.PreserveType.SmokedFish:
                        return "Smoked Fish";
                    case Object.PreserveType.Bait:
                        return "Targeted Bait";
                }
            }

            return name;
        }

        private static readonly Dictionary<string, string> _renamedObjects = new()
        {
            { QualifiedItemIds.STRANGE_DOLL_GREEN, "Strange Doll (Green)" },
            { QualifiedItemIds.BROWN_EGG, "Egg (Brown)" },
            { QualifiedItemIds.LARGE_BROWN_EGG, "Large Egg (Brown)" },
            { QualifiedItemIds.LARGE_GOAT_MILK, "Large Goat Milk" },
            { QualifiedItemIds.COOKIES, "Cookies" },
            { QualifiedItemIds.DRIED_FRUIT, "Dried Fruit" },
            { QualifiedItemIds.DRIED_MUSHROOMS, "Dried Mushrooms" },
            { QualifiedItemIds.SMOKED_FISH, "Smoked Fish" },
        };

        private static readonly List<string> _simplifiedNames = new()
        {
            "Honey",
            "Secret Note",
            "Journal Scrap",
        };

        private static readonly Dictionary<string, string> _simplifiedChars = new()
        {
            { "ñ", "n" },
            { "Ñ", "N" },
        };
    }
}
