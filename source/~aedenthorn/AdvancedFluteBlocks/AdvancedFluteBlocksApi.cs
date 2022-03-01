/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

namespace AdvancedFluteBlocks
{
    public class AdvancedFluteBlocksApi
    {
        public string GetFluteBlockToneFromIndex(int index)
        {
            var tones = ModEntry.Config.ToneList.Split(',');
            if (index >= tones.Length)
                return null;
            return tones[index];
        }
    }
}