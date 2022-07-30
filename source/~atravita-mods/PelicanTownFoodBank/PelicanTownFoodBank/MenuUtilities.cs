/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using StardewModdingAPI.Utilities;

namespace PelicanTownFoodBank;

internal class MenuUtilities
{
    private static KeybindList ctrl = KeybindList.Parse("LeftControl, RightControl");
    private static KeybindList shift = KeybindList.Parse("LeftShift, RightShift");

    internal static int GetIdealQuantityFromKeyboardState()
        => ctrl.IsDown() ? 25 :
            shift.IsDown() ? 5 : 1;
}