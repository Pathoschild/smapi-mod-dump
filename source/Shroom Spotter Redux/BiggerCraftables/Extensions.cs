/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

namespace BiggerCraftables
{
    public static class Extensions
    {
        public static string BiggerIndexKey => $"{Mod.instance.ModManifest.UniqueID}/BiggerIndex";

        public static int GetBiggerIndex( this StardewValley.Object obj )
        {
            return obj.modData.ContainsKey( BiggerIndexKey ) ? int.Parse( obj.modData[ BiggerIndexKey ] ) : 0;
        }

        public static void SetBiggerIndex( this StardewValley.Object obj, int index )
        {
            obj.modData[ BiggerIndexKey ] = index.ToString();
        }
    }
}
