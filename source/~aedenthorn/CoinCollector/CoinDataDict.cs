/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

//using Harmony;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace CoinCollector
{
    public class CoinDataDict
    {
        public List<CoinData> data = new List<CoinData>();
    }

    public class CoinData
    {
        public string id;
        public string setName;
        public float rarity;
        public int parentSheetIndex;
        public bool isDGA = false;
    }
}