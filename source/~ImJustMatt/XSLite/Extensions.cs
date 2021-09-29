/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace XSLite
{
    using StardewValley;

    internal static class Extensions
    {
        public static bool TryGetStorage(this Item item, out Storage storage)
        {
            if (!item.modData.TryGetValue($"{XSLite.ModPrefix}/Storage", out string storageName))
            {
                storageName = item.Name;
            }

            return XSLite.Storages.TryGetValue(storageName, out storage) && item.Category == -9;
        }
    }
}