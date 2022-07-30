/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

namespace SpecialOrdersExtended.Tokens;

/// <summary>
/// Token that gets all completed special orders.
/// </summary>
internal class CompletedSpecialOrders : AbstractToken
{
    /// <inheritdoc/>
    public override bool UpdateContext()
    {
        List<string>? specialOrderNames;
        if (Context.IsWorldReady)
        {
            specialOrderNames = Game1.player.team.completedSpecialOrders.Keys.OrderBy(a => a)?.ToList();
        }
        else
        {
            specialOrderNames = SaveGame.loaded?.completedSpecialOrders?.OrderBy(a => a)?.ToList();
        }
        return this.UpdateCache(specialOrderNames);
    }
}
