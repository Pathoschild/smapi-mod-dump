/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kqrse/StardewValleyMods-aedenthorn
**
*************************************************/

namespace BedTweaks
{
    public class BedTweaksAPI
    {
        public int GetBedWidth()
        {
            if (!ModEntry.Config.EnableMod)
                    return 3;
            return ModEntry.Config.BedWidth;
        }
    }
}