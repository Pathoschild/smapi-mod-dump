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