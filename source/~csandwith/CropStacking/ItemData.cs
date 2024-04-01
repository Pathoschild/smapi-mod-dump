/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/csandwith/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;

namespace CropStacking
{
    public class ItemData
    {
        public string id;
        public int stack;
        public int quality;
        public Object.PreserveType preserveType;
        public string preservedParentSheetIndex;
        public Color? color;
    }
}