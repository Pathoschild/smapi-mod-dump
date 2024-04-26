/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kqrse/StardewValleyMods-aedenthorn
**
*************************************************/

using System.Collections.Generic;

namespace StardewValleyOrigins
{
    public class WorldStateData
    {
        public List<string> npcs = new List<string>();
        public List<string> events = new List<string>();
        public List<string> mail = new List<string>();
        public List<int> mapPoints = new List<int>();
        public bool shippingBin;
        public bool bus;
        public bool minecarts;
        public bool marniesLivestock;
        public bool townBoard;
        public bool blacksmith;
        public bool farmHouse;
        public bool specialOrdersBoard;
        public bool linusCampfire;
    }
}