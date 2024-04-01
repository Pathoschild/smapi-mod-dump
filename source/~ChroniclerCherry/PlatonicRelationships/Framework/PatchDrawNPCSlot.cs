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
    //patching the method SocialPage.drawNPCSlot()
    public static class PatchDrawNpcSlotHeart
    {
        internal static void Prefix(ref bool isDating)
        {
            isDating = true; // don't lock hearts
        }
    }
}
