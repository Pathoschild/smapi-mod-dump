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

/// <summary>
/// Token that lists all available special orders.
/// </summary>
internal class AvailableSpecialOrders : AbstractToken
{
    /// <inheritdoc/>
    public override bool UpdateContext()
    {
        if (SpecialOrder.IsSpecialOrdersBoardUnlocked())
        {
            List<string>? specialOrderNames = Game1.player?.team?.availableSpecialOrders?.Select((SpecialOrder s) => s.questKey.Value).OrderBy(a => a).ToList()
                ?? SaveGame.loaded?.availableSpecialOrders?.Select((SpecialOrder s) => s.questKey.Value).OrderBy(a => a).ToList();
            return this.UpdateCache(specialOrderNames);
        }
        else
        {
            return this.UpdateCache(new List<string>());
        }
    }
}
