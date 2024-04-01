/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/csandwith/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;

namespace CustomMonsterFloors
{
    public class CustomOreNode
    {
        public Dictionary<int, double> dropItems = new Dictionary<int, double>();
        public string spriteName;
        public int spawnChance;

        public CustomOreNode(string nodeInfo)
        {
            string[] infos = nodeInfo.Split('/');
            this.spriteName = infos[0];
            this.spawnChance = int.Parse(infos[1]);
            string[] dropItems = infos[2].Split(';');
            foreach(string item in dropItems)
            {
                string[] itema = item.Split(',');
                this.dropItems.Add(int.Parse(itema[0]), double.Parse(itema[1]));
            }
        }
    }
}