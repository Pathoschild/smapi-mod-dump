/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/rtrox/LineSprinklersRedux
**
*************************************************/

using StardewValley;
using StardewValley.GameData.BigCraftables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LineSprinklersRedux.Framework.Data
{
    internal class CustomFields
    {
        public static string BaseSpriteKey => $"{ModConstants.ModKeySpace}_BaseSprite";
        public static string RangeKey => $"{ModConstants.ModKeySpace}_Range";
        private static bool TryGetInt(SObject obj, string key, out int ret) 
        {
            ret = -1;
            if (ItemRegistry.GetData(obj.QualifiedItemId)?.RawData is not BigCraftableData data)
            {
                return false;
            }

            var result = data.CustomFields.TryGetValue(key, out var baseSprite);

            if (!result || !int.TryParse(baseSprite, out ret))
            {
                return false;
            }
            return true;
        }

        public static void SetBaseSprite(BigCraftableData data)
        {
            data.CustomFields[BaseSpriteKey] = data.SpriteIndex.ToString();
        }

        public static int GetBaseSprite(SObject obj)
        {
            if (TryGetInt(obj, BaseSpriteKey, out int ret))
            {
                return ret;
            }
            return -1;
        }

        public static void SetRange(BigCraftableData data, int range)
        {
            data.CustomFields[RangeKey] = range.ToString();
        }
        public static int GetRange(SObject obj)
        {
            if (TryGetInt(obj, RangeKey, out int ret)) {
                return ret;
            }
            return -1;
        }
    }
}
