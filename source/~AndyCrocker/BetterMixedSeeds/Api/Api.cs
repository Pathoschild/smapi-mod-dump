/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

namespace BetterMixedSeeds
{
    /// <summary>Provides basic crop apis.</summary>
    public class Api : IApi
    {
        /*********
        ** Public Methods
        *********/
        /// <inheritdoc/>
        public void ForceExcludeCrop(params string[] cropNames)
        {
            if (cropNames == null || cropNames.Length == 0)
                return;

            ModEntry.Instance.Monitor.Log($"A mod has forcibly excluded: {string.Join(", ", cropNames)}");
            ModEntry.Instance.CropsToExclude.AddRange(cropNames);
        }
    }
}
