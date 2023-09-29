/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewValley.Objects;
using System;

namespace ChestFullnessTextures
{
    public partial class ModEntry
    {
        private static ChestTextureData GetChestData(Chest instance, ChestTextureDataShell dataList)
        {
            foreach (var data in dataList.Entries)
            {
                if(data.items <= instance.items.Count)
                    return data;
            }
            return null;
        }
    }
}