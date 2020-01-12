using StardewValley;

namespace ProducerFrameworkMod
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
            switch (preserveType)
            {
                case Object.PreserveType.Wine:
                    return $"{preserveParentName} Wine";
                case Object.PreserveType.Jelly:
                    return $"{preserveParentName} Jelly";
                case Object.PreserveType.Pickle:
                    return $"Pickled {preserveParentName}";
                case Object.PreserveType.Juice:
                    return $"{preserveParentName} Juice";
                case Object.PreserveType.Roe:
                    return $"{preserveParentName} Roe";
                case Object.PreserveType.AgedRoe:
                    return $"Aged {preserveParentName}";
            }
            return null;
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
                case -2:
                    return Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.569");
                case -81:
                    return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12869");
                case -80:
                    return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12866");
                case -79:
                    return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12854");
                case -75:
                    return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12851");
                case -74:
                    return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12855");
                case -28:
                    return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12867");
                case -27:
                    return DataLoader.Helper.Translation.Get("Object.Category.TappedTreeProduct");
                case -26:
                    return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12862");
                case -25:
                case -7:
                    return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12853");
                case -24:
                    return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12859");
                case -22:
                    return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12858");
                case -21:
                    return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12857");
                case -20:
                    return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12860");
                case -19:
                    return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12856");
                case -18:
                    return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12864");
                case -14:
                    return DataLoader.Helper.Translation.Get("Object.Category.Meat");
                case -16:
                    return DataLoader.Helper.Translation.Get("Object.Category.BuildingResources");
                case -15:
                    return DataLoader.Helper.Translation.Get("Object.Category.MetalResources");
                case -12:
                    return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12850");
                case -8:
                    return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12863");
                default:
                    return "???";
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
