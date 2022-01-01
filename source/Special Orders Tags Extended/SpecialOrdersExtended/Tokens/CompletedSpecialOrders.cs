/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/SpecialOrdersExtended
**
*************************************************/

namespace SpecialOrdersExtended.Tokens;

internal class CompletedSpecialOrders : AbstractToken
{
    /// <summary>Update the values when the context changes.</summary>
    /// <returns>Returns whether the value changed, which may trigger patch updates.</returns>
    public override bool UpdateContext()
    {
        List<string> specialOrderNames;
        if (Context.IsWorldReady)
        {
            specialOrderNames = Game1.player.team.completedSpecialOrders.Keys.OrderBy(a => a)?.ToList();
        }
        else
        {
            specialOrderNames = SaveGame.loaded?.completedSpecialOrders?.OrderBy(a => a)?.ToList();
        }

        if (specialOrderNames == SpecialOrdersCache)
        {
            return false;
        }
        else
        {
            SpecialOrdersCache = specialOrderNames;
            return true;
        }
    }
}
