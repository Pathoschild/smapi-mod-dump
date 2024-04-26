/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kqrse/StardewValleyMods-aedenthorn
**
*************************************************/

using StardewValley;

namespace StardewRPG
{
    public interface IStardewRPGApi
    {
        public int GetStatMod(int statValue);
        public int GetStatValue(Farmer farmer, string stat, int defaultValue = -1);
        public void GainExperience(Farmer farmer, int howMuch);
    }

    public class StardewRPGApi
    {
        public int GetStatMod(int statValue)
        {
            return ModEntry.GetStatMod(statValue);
        }
        public int GetStatValue(Farmer farmer, string stat, int defaultValue = -1)
        {
            return ModEntry.GetStatValue(farmer, stat, defaultValue);
        }
        public void GainExperience(ref Farmer farmer, int howMuch)
        {
            ModEntry.GainExperience(ref farmer, howMuch);
        }
    }
}