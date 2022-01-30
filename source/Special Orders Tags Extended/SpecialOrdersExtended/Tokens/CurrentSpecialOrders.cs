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
/// Token that gets all current special orders.
/// </summary>
internal class CurrentSpecialOrders : AbstractToken
{
    /// <inheritdoc/>
    public override bool UpdateContext()
    {
        List<string>? specialOrderNames = Game1.player?.team?.specialOrders?.Select((SpecialOrder s) => s.questKey.ToString())?.OrderBy(a => a)?.ToList()
            ?? SaveGame.loaded?.specialOrders?.Select((SpecialOrder s) => s.questKey.ToString())?.OrderBy(a => a)?.ToList();
        return this.UpdateCache(specialOrderNames);
    }
}
