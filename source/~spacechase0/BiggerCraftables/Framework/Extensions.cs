/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using SObject = StardewValley.Object;

namespace BiggerCraftables.Framework
{
    internal static class Extensions
    {
        public static string BiggerIndexKey => $"{Mod.Instance.ModManifest.UniqueID}/BiggerIndex";

        public static int GetBiggerIndex(this SObject obj)
        {
            return obj.modData.TryGetValue(Extensions.BiggerIndexKey, out string rawIndex)
                ? int.Parse(rawIndex)
                : 0;
        }

        public static void SetBiggerIndex(this SObject obj, int index)
        {
            obj.modData[Extensions.BiggerIndexKey] = index.ToString();
        }
    }
}
