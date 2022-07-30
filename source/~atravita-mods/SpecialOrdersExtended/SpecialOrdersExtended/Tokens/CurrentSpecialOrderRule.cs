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
/// Token that gets all current active special order rules.
/// </summary>
internal class CurrentSpecialOrderRule : AbstractToken
{
    /// <inheritdoc/>
    public override bool UpdateContext()
    {
        List<string>? rules;
        if (Context.IsWorldReady)
        {
            rules = Game1.player.team.specialOrders
                .SelectMany((SpecialOrder i) => i.specialRule.Value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                .OrderBy(a => a).ToList();
        }
        else
        {
            rules = SaveGame.loaded?.specialOrders
                ?.SelectMany((SpecialOrder i) => i.specialRule.Value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                ?.OrderBy(a => a)?.ToList();
        }

        return this.UpdateCache(rules);
    }
}
