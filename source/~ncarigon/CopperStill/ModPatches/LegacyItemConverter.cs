/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ncarigon/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using SObject = StardewValley.Object;

namespace CopperStill.ModPatches {
    internal static class LegacyItemConverter {
        public static void Register() {
            ModEntry.ModHarmony?.Patch(
                    original: AccessTools.Method(typeof(Farmer), "addItemToInventory", new Type[] { typeof(Item) }),
                    prefix: new HarmonyMethod(typeof(LegacyItemConverter), nameof(Prefix_Farmer_addItemToInventory))
                );
            ModEntry.ModHarmony?.Patch(
                original: AccessTools.Method(typeof(ItemRegistry), "GetDataOrErrorItem"),
                prefix: new HarmonyMethod(typeof(LegacyItemConverter), nameof(Prefix_ItemRegistry_GetDataOrErrorItem))
            );
        }

        private class LegacyItemConversion {
            public string ItemType { get; private set; }
            public string CurrentId { get; private set; }
            public string[] LegacyIds { get; private set; }

            public LegacyItemConversion(string itemType, string currentId, params string[] legacyIds) {
                this.ItemType = itemType;
                this.CurrentId = currentId;
                this.LegacyIds = new List<string>() { currentId }.Concat(legacyIds ?? Array.Empty<string>()).ToArray();
            }
        }

        private static readonly LegacyItemConversion[] CopperStillItems = new LegacyItemConversion [] {
            new LegacyItemConversion("(BC)", "Still"),
            new LegacyItemConversion("(O)", "JuniperBerry", "Juniper_Berry"),
            new LegacyItemConversion("(O)", "Brandy"),
            new LegacyItemConversion("(O)", "Vodka"),
            new LegacyItemConversion("(O)", "Gin"),
            new LegacyItemConversion("(O)", "Moonshine"),
            new LegacyItemConversion("(O)", "Whiskey"),
            new LegacyItemConversion("(O)", "Sake"),
            new LegacyItemConversion("(O)", "Soju"),
            new LegacyItemConversion("(O)", "TequilaBlanco", "Tequila_Blanco"),
            new LegacyItemConversion("(O)", "TequilaAnejo", "Tequila_Anejo"),
            new LegacyItemConversion("(O)", "RumWhite", "White_Rum"),
            new LegacyItemConversion("(O)", "RumDark", "Dark_Rum")
        };

        private static void Prefix_Farmer_addItemToInventory(
            ref Item item
        ) {
            if (TrySwapLegacyItem(item.QualifiedItemId, out var currentId)) {
                var legacyObj = item as SObject;
                item = ItemRegistry.Create(currentId, item.Stack, item.Quality);
                if (legacyObj is not null && item is SObject currentObj) {
                    currentObj.preservedParentSheetIndex.Value = legacyObj.preservedParentSheetIndex.Value;
                    currentObj.Quality = legacyObj.Quality;
                    currentObj.Price = legacyObj.Price;
                    currentObj.Stack = legacyObj.Stack;
                    currentObj.Name = legacyObj.Name;
                    if (!string.IsNullOrWhiteSpace(currentObj.preservedParentSheetIndex.Value)) {
                        currentObj.displayNameFormat = "[LocalizedText Strings/Objects:NCarigon.CopperStillCP_Flavor %PRESERVED_DISPLAY_NAME %DISPLAY_NAME]";
                    }
                }
            }
        }

        private static void Prefix_ItemRegistry_GetDataOrErrorItem(
            ref string itemId
        ) {
            if (TrySwapLegacyItem(itemId, out var currentId) && currentId is not null) {
                itemId = currentId;
            }
        }

        private static bool TrySwapLegacyItem(string itemId, out string? currentId) {
            foreach (var legacyItem in CopperStillItems) {
                foreach (var legacyId in legacyItem.LegacyIds) {
                    if (string.Compare(itemId, $"{legacyItem.ItemType}{legacyId}") == 0
                        || string.Compare(itemId, $"{legacyItem.ItemType}NCarigon.CopperStillJA_{legacyId}") == 0
                    ) {
                        currentId = $"{legacyItem.ItemType}NCarigon.CopperStillCP_{legacyItem.CurrentId}";
                        return true;
                    }
                }
            }
            currentId = null;
            return false;
        }
    }
}
