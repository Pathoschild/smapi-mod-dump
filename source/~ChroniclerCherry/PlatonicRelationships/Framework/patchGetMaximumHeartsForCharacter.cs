/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ChroniclerCherry/stardew-valley-mods
**
*************************************************/

namespace PlatonicRelationships.Framework
{
    class patchGetMaximumHeartsForCharacter
    {
        internal static void Postfix(ref int __result)
        {
            if (__result == 8)
                __result = 10;
        }
    }
}
