/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sonozuki/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System.Reflection;

namespace FarmAnimalVarietyRedux.Patches
{
    /// <summary>Contains patches for patching game code in the <see cref="AnimalQueryMenu"/> class.</summary>
    internal class AnimalQueryMenuPatch
    {
        /*********
        ** Internal Methods
        *********/
        /// <summary>The prefix for the <see cref="AnimalQueryMenu(FarmAnimal)"/> constructor.</summary>
        /// <param name="animal">The animal the menu is for.</param>
        /// <param name="__instance">The <see cref="AnimalQueryMenu"/> instance being patched.</param>
        /// <returns><see langword="false"/>, meaning the original method will not get ran.</returns>
        /// <remarks>This is used stop the original constructor from running, this is because it had a tendency to throw errors with custom animals.</remarks>
        internal static bool ConstructorPrefix(FarmAnimal animal, AnimalQueryMenu __instance)
        {
            // set the animal property, this is required as this is what gets retrieved when swapping to the custom menu
            typeof(AnimalQueryMenu).GetField("animal", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, animal);
            return false;
        }

        /// <summary>The prefix for the <see cref="AnimalQueryMenu.draw(SpriteBatch)"/> method.</summary>
        /// <returns><see langword="false"/>, meaning the original method will not get ran.</returns>
        /// <remarks>This is needed as there is one frame that the game tries to draw the original menu, and with the constructor patched out it instantly errors causing the menu to crash.</remarks>
        internal static bool DrawPrefix() => false;
    }
}
