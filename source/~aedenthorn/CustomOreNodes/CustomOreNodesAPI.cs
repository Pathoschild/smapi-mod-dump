/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using StardewValley;
using System.Collections.Generic;
using System.Linq;

namespace CustomOreNodes
{
    public class CustomOreNodesAPI
    {
        public int GetCustomOreNodeIndex(string id)
        {
            var node = ModEntry.customOreNodesList.Find(n => n.id == id);
            if (node == null)
                return -1;
            return node.parentSheetIndex;
        }
        public List<object> GetCustomOreNodes()
        {
            return new List<object>(ModEntry.customOreNodesList);
        }
        public List<string> GetCustomOreNodeIDs() 
        {
            return ModEntry.customOreNodesList.Select(n => n.id).ToList();
        }
    }
}