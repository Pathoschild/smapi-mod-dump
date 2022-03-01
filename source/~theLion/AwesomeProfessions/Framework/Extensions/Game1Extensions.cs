/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Extensions;

#region using directives

using System.Linq;
using StardewModdingAPI;
using StardewValley;

#endregion using directives

internal static class Game1Extensions
{
    /// <summary>Whether any farmer in the current game session has a specific profession.</summary>
    /// <param name="professionName">The name of the profession.</param>
    /// <param name="numberOfPlayersWithThisProfession">How many players have this profession.</param>
    public static bool DoesAnyPlayerHaveProfession(this Game1 game1, Profession profession,
        out int numberOfPlayersWithThisProfession)
    {
        if (!Context.IsMultiplayer)
            if (Game1.player.HasProfession(profession))
            {
                numberOfPlayersWithThisProfession = 1;
                return true;
            }

        numberOfPlayersWithThisProfession = Game1.getOnlineFarmers()
            .Count(f => f.HasProfession(profession));
        return numberOfPlayersWithThisProfession > 0;
    }
}