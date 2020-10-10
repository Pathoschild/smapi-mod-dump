/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Drachenkaetzchen/AdvancedKeyBindings
**
*************************************************/

using AdvancedKeyBindings.StaticHelpers;
using StardewValley;
using StardewValley.Menus;

namespace AdvancedKeyBindings.Extensions.MenuExtensions
{
    public static class PurchaseAnimalsMenuExtension
    {
        /// <summary>
        /// Returns the animal currently being purchased
        /// </summary>
        /// <param name="menu"></param>
        /// <returns>The animal</returns>
        public static FarmAnimal GetAnimalBeingPurchased(this PurchaseAnimalsMenu menu)
        {
            var reflector = StaticReflectionHelper.GetInstance().GetReflector();

            return reflector.GetField<FarmAnimal>(menu, "animalBeingPurchased").GetValue();
        }

        public static bool IsAnimalPlacementMode(this PurchaseAnimalsMenu menu)
        {
            var reflector = StaticReflectionHelper.GetInstance().GetReflector();

            return reflector.GetField<bool>(menu, "onFarm").GetValue() &&
                   !reflector.GetField<bool>(menu, "namingAnimal").GetValue();
        }
    }
}