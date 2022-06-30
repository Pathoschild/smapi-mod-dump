/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Taxes.Extensions;

#region using directives

using StardewValley;

#endregion using directives

/// <summary>Extensions for the <see cref="Game1"/> class.</summary>
public static class Game1Extensions
{
    /// <summary>Get the name of the preceding season.</summary>
    public static string GetPrecedingSeason(this Game1 game1)
    {
        return Game1.currentSeason switch
        {
            "spring" => "winter",
            "summer" => "spring",
            "fall" => "summer",
            "winter" => "fall",
            _ => string.Empty
        };
    }
}