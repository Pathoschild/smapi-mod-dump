/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QualityBait
{
    public interface IApi
    {
        int GetQuality(int currentQuality, int baitQuality);

        int GetQuality(string itemId, int currentQuality, int baitQuality);
    }

    public class Api : IApi
    {
        /// <summary>
        /// Obsolete, use <see cref="GetQuality(string, int, int)"/> instead
        /// </summary>
        [Obsolete("Replaced with new variant which filters out trash")]
        public int GetQuality(int currentQuality, int baitQuality) => ModEntry.GetQualityForCatch("(O)145", currentQuality, baitQuality); //Use sunfish id for simplicity

        public int GetQuality(string itemId, int currentQuality, int baitQuality) => ModEntry.GetQualityForCatch(itemId, currentQuality, baitQuality);
    }
}
