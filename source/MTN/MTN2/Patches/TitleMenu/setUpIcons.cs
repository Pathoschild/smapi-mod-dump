using MTN2.Menus;
using StardewValley.Menus;


namespace MTN2.Patches.TitleMenuPatches
{
    /// <summary>
    /// REASON FOR PATCHING: CharacterCustomization Menu with 5+
    /// farm selection.
    /// </summary>
    public class setUpIconsPatch
    {
        /// <summary>
        /// Prefix Method. Occurs before the original method is executed.
        /// 
        /// Checks to see if the subMenu is of <see cref="CharacterCustomizationMTN"/>.
        /// </summary>
        /// <returns><c>false</c> if it is a <see cref="CharacterCustomizationMTN"/>. Otherwise, <c>true</c>.</returns>
        public static bool Prefix() {
            return (TitleMenu.subMenu is CharacterCustomizationMTN) ? false : true;
        }
    }
}
