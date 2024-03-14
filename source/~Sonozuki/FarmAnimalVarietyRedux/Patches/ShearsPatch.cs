/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sonozuki/StardewMods
**
*************************************************/

using StardewValley;
using StardewValley.Tools;

namespace FarmAnimalVarietyRedux.Patches
{
    /// <summary>Contains patches for patching game code in the <see cref="Shears"/> class.</summary>
    internal class ShearsPatch
    {
        /*********
        ** Internal Methods
        *********/
        /// <summary>The prefix for the <see cref="Shears.beginUsing(GameLocation, int, int, Farmer)"/> method.</summary>
        /// <param name="who">The farmer that is using the tool.</param>
        /// <param name="__result">The return value of the method being patched.</param>
        /// <returns><see langword="false"/>, meaning the original method will not get ran.</returns>
        /// <remarks>This is used to remove the base game handling for the Shears behavior, this is because FAVR reimplements a generic tool handling system which handles the Shears behavior automatically.</remarks>
        internal static bool BeginUsingPrefix(Farmer who, ref bool __result)
        {
            who.Halt();
            var oldFrame = who.FarmerSprite.CurrentFrame;
            who.FarmerSprite.animateOnce(283 + who.FacingDirection, 50, 4);
            who.FarmerSprite.oldFrame = oldFrame;
            who.UsingTool = true;
            who.CanMove = false;

            __result = true;
            return false;
        }
    }
}
