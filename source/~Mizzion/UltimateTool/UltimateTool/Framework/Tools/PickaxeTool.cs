using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using UltimateTool.Framework.Configuration;
using SFarmer = StardewValley.Farmer;
using SObject = StardewValley.Object;

namespace UltimateTool.Framework.Tools
{
    internal class PickaxeTool : BaseTool
    {
        private readonly PickaxeConfig _config;

        private readonly IDictionary<int, int> _resourceUpgradeNeeded = new Dictionary<int, int>
        {
            [ResourceClump.meteoriteIndex] = Tool.gold,
            [ResourceClump.boulderIndex] = Tool.steel
        };

        public PickaxeTool(PickaxeConfig config)
        {
            _config = config;
        }
     
        public override bool IsEnabled(SFarmer who, Tool tool, Item item, GameLocation location)
        {
            return tool is Pickaxe;
        }

        public override bool Apply(Vector2 tile, SObject tileObj, TerrainFeature tileFeature, SFarmer who, Tool tool, Item item, GameLocation location)
        {
            if(_config.CutDebris && tileObj?.Name == "Stone")
            {
                return UseToolOnTile(tool, tile);
            }
            
            if(tileFeature is HoeDirt dirt)
            {
                if(_config.ClearDirt && dirt.crop == null)
                {
                    return UseToolOnTile(tool, tile);
                }

                if (_config.CutDeadCrops && dirt.crop.dead.Value)
                {
                    return UseToolOnTile(tool, tile);
                }
            }

            if (_config.ClearBouldersAndMeteor)
            {
                ResourceClump rc = ResourceClumpCoveringTile(location, tile);
                if(rc != null && _resourceUpgradeNeeded.ContainsKey(rc.parentSheetIndex.Value) && tool.UpgradeLevel >= _resourceUpgradeNeeded[rc.parentSheetIndex.Value])
                {
                    UseToolOnTile(tool, tile);
                }
            }
            return false;
        }
    }
}
