/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

using DaLion.Stardew.Alchemy.Framework.UI;
using StardewValley;

namespace DaLion.Stardew.Alchemy;

#region using directives

using StardewModdingAPI;

#endregion using directives

internal static class ConsoleCommands
{
    /// <summary>Register all internally defined console commands.</summary>
    /// <param name="helper">The console command API.</param>
    internal static void Register(this ICommandHelper helper)
    {
        helper.Add("tog_menu", "Open the mixing menu.", OpenMixingMenu);
    }

    #region command handlers

    internal static void OpenMixingMenu(string command, string[] args)
    {
        var menu = new AlchemyMenu();
        Game1.activeClickableMenu = menu;
    }

    #endregion command handlers

    #region private methods

    #endregion private methods
}