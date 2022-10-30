/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NeroYuki/StardewSurvivalProject
**
*************************************************/

using System;
using System.Collections.Generic;
using StardewValley;

namespace StardewSurvivalProject.source.data
{
    public class ItemNameCache
    {
        public static Dictionary<string, int> name_to_id = new Dictionary<string, int>();
        
        //return false if cant find the item
        public static bool cacheItem(string name)
        {
            foreach (KeyValuePair<int, string> itemInfoString in Game1.objectInformation)
            {
                //check string start for object name
                if (itemInfoString.Value.StartsWith(name + "/"))
                {
                    name_to_id.Add(name, itemInfoString.Key);
                    return true;
                }
            }
            return false;
        }

        public static void clearCache()
        {
            name_to_id.Clear();
        }

        public static int getIDFromCache(string name)
        {
            if (name_to_id.ContainsKey(name))
            {
                return name_to_id[name];
            }
            return -1;
        }
    }
}
