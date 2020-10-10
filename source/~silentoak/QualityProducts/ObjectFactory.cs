/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/silentoak/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using SObject = StardewValley.Object;

namespace QualityProducts.Processors
{
    internal static class ObjectFactory
    {
        public static SObject FlowerFromHoneyType(SObject.HoneyType honeyType)
        {
            if (honeyType == SObject.HoneyType.Wild)
                return null;

            return new SObject(Vector2.Zero, (int)honeyType, null, false, false, false, false);
        }
    }
}