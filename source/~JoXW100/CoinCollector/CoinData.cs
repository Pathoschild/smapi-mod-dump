/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JoXW100/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;

namespace CoinCollector
{
    public class CoinData
    {
        public int index;
        public string name;
        public string setName;
        public string texturePath;
        public List<string> locations;
        public float rarity;
        public int parentSheetIndex;
        public bool isDGA = false;
    }
}