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
using StardewValley;
using Object = StardewValley.Object;

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
            if (_renamedItems.ContainsKey(item.ParentSheetIndex))
            {
                name = _renamedItems[item.ParentSheetIndex];
            }

            foreach (var (oldChar, newChar) in _simplifiedChars)
            {
                name = name.Replace(oldChar, newChar);
            }

            if (name.Contains("moonslime.excavation."))
            {
                TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
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
                }
            }

            return name;
        }

        private static readonly Dictionary<int, string> _renamedItems = new()
        {
            { 126, "Strange Doll (Green)"},
            { 180, "Egg (Brown)" },
            { 182, "Large Egg (Brown)" },
            { 438, "Large Goat Milk" },
            { 223, "Cookies" },
        };

        private static readonly List<string> _simplifiedNames = new()
        {
            "Honey",
            "Secret Note",
            "Journal Scrap",
        };

        private static readonly Dictionary<string, string> _simplifiedChars = new()
        {
            {"ñ", "n"},
            {"Ñ", "N"},
        };
    }
}
