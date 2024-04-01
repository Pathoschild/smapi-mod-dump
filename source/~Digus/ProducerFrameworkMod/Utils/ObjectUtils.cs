/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using System;
using StardewValley;
using StardewValley.BellsAndWhistles;
using Object = StardewValley.Object;

namespace ProducerFrameworkMod.Utils
{
    internal class ObjectUtils
    {
        public const char ObjectSeparator = '/';

        internal static bool IsObjectStringFromObjectName(string objectString, string objectName)
        {
            return objectString.StartsWith(objectName + ObjectSeparator);
        }

        internal static string GetObjectParameter(string objectString, int position)
        {
            return objectString.Split(ObjectSeparator)[position];
        }

        internal static string GetPreserveName(Object.PreserveType preserveType, string preserveParentName)
        {
            return preserveType switch
            {
                Object.PreserveType.Wine => $"{preserveParentName} Wine",
                Object.PreserveType.Jelly => $"{preserveParentName} Jelly",
                Object.PreserveType.Pickle => $"Pickled {preserveParentName}",
                Object.PreserveType.Juice => $"{preserveParentName} Juice",
                Object.PreserveType.Roe => $"{preserveParentName} Roe",
                Object.PreserveType.AgedRoe => $"Aged {preserveParentName}",
                Object.PreserveType.Honey => $"{preserveParentName ?? "Wild"} Honey",
                Object.PreserveType.DriedFruit => Lexicon.makePlural($"Dried {preserveParentName}"),
                Object.PreserveType.DriedMushroom => Lexicon.makePlural($"Dried {preserveParentName}"),
                Object.PreserveType.SmokedFish => "Smoked {preserveParentName}",
                Object.PreserveType.Bait => $"{preserveParentName}  Bait",
                _ => null
            };
        }

        internal static string GetCategoryName(int categoryIndex)
        {
            switch (categoryIndex)
            {
                case -6:
                    return Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.573");
                case -5:
                    return Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.572");
                case -4:
                    return Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.571");
                case -3:
                    return Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.570");
                case -2:
                    return Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.569");
                case -1:
                    return Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.568");
                case -27:
                    return DataLoader.Helper.Translation.Get("Object.Category.TappedTreeProduct");
                case -14:
                    return DataLoader.Helper.Translation.Get("Object.Category.Meat");
                case -16:
                    return DataLoader.Helper.Translation.Get("Object.Category.BuildingResources");
                case -15:
                    return DataLoader.Helper.Translation.Get("Object.Category.MetalResources");
                case -777:
                    return Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.574");
                default:
                    var categoryDisplayName = Object.GetCategoryDisplayName(categoryIndex);
                    return  !string.IsNullOrEmpty(categoryDisplayName) ? categoryDisplayName : "???" ;
            }
        }
    }

    internal enum ObjectParameter
    {
        Name = 0,
        Price = 1,
        DisplayName = 4
    }
}
