using AdvancedKeyBindings.StaticHelpers;
using StardewValley;
using StardewValley.Menus;

namespace AdvancedKeyBindings.Extensions.MenuExtensions
{
    public static class AnimalQueryWindowExtensions
    {
        /// <summary>
        /// Gets the animal associated with the menu
        /// </summary>
        /// <param name="menu"></param>
        /// <returns>The animal</returns>
        public static FarmAnimal GetAnimal(this AnimalQueryMenu menu)
        {
            var reflector = StaticReflectionHelper.GetInstance().GetReflector();
            
            return reflector.GetField<FarmAnimal>(menu, "animal").GetValue();
        }

        /// <summary>
        /// Returns if the animal is currently being placed or not
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        public static bool IsAnimalPlacementMode(this AnimalQueryMenu menu)
        {
            var reflector = StaticReflectionHelper.GetInstance().GetReflector();
            
            return reflector.GetField<bool>(menu, "movingAnimal").GetValue();
        }
    }
}