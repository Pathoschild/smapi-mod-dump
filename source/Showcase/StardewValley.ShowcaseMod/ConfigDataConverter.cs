using Igorious.StardewValley.DynamicApi2.Data;
using Igorious.StardewValley.DynamicApi2.Extensions;
using Igorious.StardewValley.ShowcaseMod.ModConfig;

namespace Igorious.StardewValley.ShowcaseMod
{
    public static class ConfigDataConverter
    {
        public static FurnitureInfo ToFurnitureData(ShowcaseConfig c)
        {
            return new FurnitureInfo
            {
                ID = c.ID,
                Name = c.Name,
                Kind = c.Kind.ToLower(),
                Size = c.Size,
                BoundingBox = c.BoundingBox,
                Price = c.Price,
                Rotations = c.Rotations,
            };
        }
    }
}