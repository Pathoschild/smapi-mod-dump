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

internal class RecentCompletedSO : AbstractToken
{
    public override bool UpdateContext()
    {
        List<string> recentCompletedSO = RecentSOManager.GetKeys(7u)?.OrderBy(a => a)?.ToList();
        if (recentCompletedSO == SpecialOrdersCache)
        {
            return false;
        }
        else
        {
            SpecialOrdersCache = recentCompletedSO;
            return true;
        }
    }
}
