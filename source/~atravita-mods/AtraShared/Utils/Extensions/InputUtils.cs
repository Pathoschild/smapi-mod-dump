/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

namespace AtraShared.Utils.Extensions;

/// <summary>
/// Extension methods on InputHelper.
/// </summary>
public static class InputUtils
{
    /// <summary>
    /// Asks inputhelp to supresses the two usual click buttons.
    /// </summary>
    /// <param name="inputHelper">Smapi's inputhelper.</param>
    public static void SurpressClickInput(this IInputHelper inputHelper)
    {
        inputHelper.Suppress(Game1.options.actionButton[0].ToSButton());
        inputHelper.Suppress(Game1.options.useToolButton[0].ToSButton());
    }
}