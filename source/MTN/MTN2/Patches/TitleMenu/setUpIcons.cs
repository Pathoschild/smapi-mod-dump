/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Dawilly/MTN2
**
*************************************************/

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
