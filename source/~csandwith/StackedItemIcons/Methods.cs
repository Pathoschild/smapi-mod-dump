/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/csandwith/StardewValleyMods
**
*************************************************/


using StardewValley;
using System.Collections.Generic;
using System.IO;

namespace StackedItemIcons
{
    public partial class ModEntry
    {
        public static bool AllowedToShow(Object instance)
        {
            if(instance.bigCraftable.Value)
            {
                return false;
            }
            if (!allowList.TryGetValue(instance.Name, out bool allowed))
            {
                allowList[instance.Name] = true;
                writeAllowed = true;
                return true;
            }
            else
                return allowed;
        }

    }
}