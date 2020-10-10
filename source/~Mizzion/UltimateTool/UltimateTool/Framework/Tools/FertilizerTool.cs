/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using UltimateTool.Framework.Configuration;
using SFarmer = StardewValley.Farmer;
using SObject = StardewValley.Object;

namespace UltimateTool.Framework.Tools
{
    internal class FertilizerTool : BaseTool
    {
        private readonly FertilizerConfig _config;

        public FertilizerTool(FertilizerConfig config)
        {
            _config = config;
        }

        public override bool IsEnabled(SFarmer who, Tool tool, Item item, GameLocation location)
        {
            return _config.Enabled && item?.Category == SObject.fertilizerCategory;
        }

        public override bool Apply(Vector2 tile, SObject tileObj, TerrainFeature tileFeature, SFarmer who, Tool tool, Item item, GameLocation location)
        {
            if(item == null)
            {
                return false;
            }
            if(!(tileFeature is HoeDirt dirt) || dirt.fertilizer.Value != HoeDirt.noFertilizer)
            {
                return false;
            }
            if(ResourceClumpCoveringTile(location, tile) != null)
            {
                return false;
            }
            dirt.fertilizer.Value = item.ParentSheetIndex;
            RemoveItem(who, item);
            return true;
        }
    }
}
