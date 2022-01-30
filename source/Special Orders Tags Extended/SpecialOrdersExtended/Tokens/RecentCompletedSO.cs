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
/// Token that gets all Special Orders completed within the last seven in-game days.
/// </summary>
internal class RecentCompletedSO : AbstractToken
{
    /// <inheritdoc/>
    public override bool UpdateContext()
    {
        List<string>? recentCompletedSO = RecentSOManager.GetKeys(7u)?.OrderBy(a => a)?.ToList();
        return this.UpdateCache(recentCompletedSO);
    }
}
