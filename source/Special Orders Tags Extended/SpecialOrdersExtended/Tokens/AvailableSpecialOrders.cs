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

internal class AvailableSpecialOrders : AbstractToken
{

    /// <summary>Update the values when the context changes.</summary>
    /// <returns>Returns whether the value changed, which may trigger patch updates.</returns>
    public override bool UpdateContext()
    {
        List<string> specialOrderNames = Game1.player?.team?.availableSpecialOrders?.Select((SpecialOrder s) => s.questKey.ToString()).OrderBy(a => a).ToList()
            ?? SaveGame.loaded?.availableSpecialOrders?.Select((SpecialOrder s) => s.questKey.ToString()).OrderBy(a => a).ToList();
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
