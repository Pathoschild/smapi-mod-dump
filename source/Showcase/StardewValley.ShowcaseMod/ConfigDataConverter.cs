/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/Stardew_Valley_Showcase_Mod
**
*************************************************/

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