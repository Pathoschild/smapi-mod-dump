/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ribeena/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;

namespace DynamicBodies.Data
{
    public class ShirtOverlay
    {
        public Dictionary<string, IList<string>> overlays = new Dictionary<string, IList<string>>();
        private Dictionary<string, int> overlayIndex = new Dictionary<string, int>();
        public IContentPack contentPack;
        public int total = 0;
        public ShirtOverlay()
        {
            foreach(var kvp in overlays)
            {
                total += overlays[kvp.Key].Count;
            }
        }

        public int SetIndex(string ClothingName, int index)
        {
            overlayIndex[ClothingName] = index;
            return index+overlays.Count;
        }

        public int GetIndex(string ClothingName, bool isMale = true)
        {
            if(overlays.ContainsKey(ClothingName))
            {
                if (overlayIndex.ContainsKey(ClothingName))
                {
                    if (isMale || overlays[ClothingName].Count < 2)
                    {
                        return overlayIndex[ClothingName];
                    } else
                    {
                        return overlayIndex[ClothingName] + 1;
                    }
                }
                return -2;
            }
            return -1;
        }
    }
}
