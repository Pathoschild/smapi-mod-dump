/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kqrse/StardewValleyMods-aedenthorn
**
*************************************************/

using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace UtilityGrid
{
    public class UtilitySystemDictData
    {
        public Dictionary<string, UtilitySystemData> dict = new Dictionary<string, UtilitySystemData>();
    }
    public class UtilitySystemData
    {
        public List<int[]> waterData = new List<int[]>();
        public List<int[]> electricData = new List<int[]>();

        public UtilitySystemData()
        {
        }

        public UtilitySystemData(Dictionary<Vector2, GridPipe> waterPipes, Dictionary<Vector2, GridPipe> electricPipes)
        {
            foreach (var kvp in waterPipes)
            {
                waterData.Add(new int[]{ (int)kvp.Key.X, (int)kvp.Key.Y, (int)kvp.Value.index, (int)kvp.Value.rotation });
            }
            foreach (var kvp in electricPipes)
            {
                electricData.Add(new int[] { (int)kvp.Key.X, (int)kvp.Key.Y, (int)kvp.Value.index, (int)kvp.Value.rotation });
            }
        }
    }
}